using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Management.Automation;
using System.Management.Automation.Runspaces;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnicorntoSCSConverter
{
    public partial class ScsModuleRuleEditor : System.Web.UI.Page
    {
        private string ConditionalLogic;
        string projectPath = "C:\\projects\\MyProjectBase\\src";
        string basePath = "C:\\projects\\MyProjectBase";

        private bool FindItem(string siteName,string dbName, string itemPath)
        {
            Sitecore.Context.SetActiveSite(siteName);

            // Pull the start path of the site
            string startPath = Sitecore.Context.Site.StartPath;

            // Pull the database name
            string databaseName = Sitecore.Context.Site.Database.Name;

            // Load the web database, and get item
            var db = Sitecore.Data.Database.GetDatabase(databaseName);
            var item = db.GetItem(itemPath);

            if (item == null) return false;

            return true;
        }

        private void AddModuleJsonUnderFeatureFolders()
        {
            IEnumerable<string> folders = Directory.EnumerateDirectories(projectPath + "\\feature");

            if (!folders.Any()) return;

            foreach(string folder in folders)
            {
                string FolderName = new DirectoryInfo(System.IO.Path.GetFileName(folder)).Name;
                SerializeFeatureContent(FolderName);
            }                
        }

        private void AddModuleJsonUnderProjectFolders()
        {
            IEnumerable<string> folders = Directory.EnumerateDirectories(projectPath + "\\project");

            if (!folders.Any()) return;

            foreach (string folder in folders)
            {
                string FolderName = new DirectoryInfo(System.IO.Path.GetFileName(folder)).Name;
                SerializeProjectContent(FolderName);
            }
        }

        protected void Page_Load(object sender, EventArgs e)
		{
            
            string[] fileList = Directory.GetFiles(basePath, "*.module.json", SearchOption.AllDirectories);
            //bool itemExists = FindItem("website", "master", "/sitecore/content/home");

            //if (fileList.Any()) { btnModuleLoad.Visible = false; }

            string[] userJson = Directory.GetFiles(basePath, "user.json", SearchOption.AllDirectories);

            if (!userJson.Any())
            {
                btnModuleLoad.Text = "Setup Serialization";
                CreateScsPs1();
            }
                
            else if (!fileList.Any()) btnModuleLoad.Text = "Setup Module JSON files";
            else if (fileList.Any() && userJson.Any())
            {
                btnModuleLoad.Text = "Setup Module JSON files";
                btnModuleLoad.Enabled = false;
                btnSync.Visible = true;
                btnSerialize.Visible = true;
            }          


            if (!this.IsPostBack)
			{
                ddlModuleJsonList.Items.Clear();
                foreach (string file in fileList)
                {
                    ddlModuleJsonList.Items.Add(file);
                }
                this.BindGridView();
                ViewState["ModuleFileName"] = ddlModuleJsonList.SelectedValue;
                LoadModuleJson();
            }
            else
            {
               // ddlModuleJsonList.SelectedValue = (string)ViewState["ModuleFileName"];
            }

        }

        private void LoadCorrectInfo()
        {
            this.txtModuleJson.Text = ShowContent(ddlModuleJsonList.SelectedValue);
            Module module = JsonConvert.DeserializeObject<Module>(this.txtModuleJson.Text);

            ViewState["Module"] = module;

            ddlIncludeList.Items.Clear();
            foreach (Include include in module?.Info?.IncludeList)
            {
                ddlIncludeList.Items.Add(include.Name);
            }

            string str = string.Empty;
            using (Stream ms = new MemoryStream(Encoding.ASCII.GetBytes(str)))
            using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
            {
                string str2 = sr.ReadToEnd();
                Console.WriteLine(string.Join(",", str2.Select(c => ((int)c))));
            }
        }

		private List<Rule> Rules
		{
			get
			{
				if (this.ViewState["Rules"] == null)
				{
					this.ViewState["Rules"] = this.GetRules();
				}

				return this.ViewState["Rules"] as List<Rule>;
			}
		}

        //private Module ModuleData
        //{
        //    get
        //    {
        //        if (this.ViewState["Module"] == null)
        //        {
        //            this.ViewState["Module"] = LoadModuleJson();
        //        }

        //        return this.ViewState["Module"] as Module;
        //    }
        //}

        private void BindGridView()
		{
			gvRules.DataSource = this.Rules;
            gvRules.DataBind();
		}

		private List<Rule> GetRules()
        {
            var rules = new List<Rule>
            {
                new Rule(1, "ItemAndDescendants", "CreateUpdateAndDelete","/brands"),
                new Rule(2, "DescendantsOnly", "CreateAndUpdate","/aboutus"),
                new Rule(3, "SingleItem", "CreateOnly","/contactus")
            };

            return rules;
		}


		protected void gvRules_RowEditing(object sender, GridViewEditEventArgs e)
		{
			gvRules.EditIndex = e.NewEditIndex;
			this.BindGridView();
		}

		protected void gvRules_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
            gvRules.EditIndex = -1;
			this.BindGridView();
		}

		protected void gvRules_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			GridViewRow row = gvRules.Rows[e.RowIndex];
			int ruleId = Convert.ToInt32(gvRules.DataKeys[e.RowIndex]["RuleId"]);
			TextBox txtPath = row.FindControl("txtPath") as TextBox;
			DropDownList ddlScope = row.FindControl("ddlScope") as DropDownList;
            DropDownList ddlAllowedOperation = row.FindControl("ddlAllowedOperation") as DropDownList;

			if ((txtPath != null) && (ddlScope != null) && (ddlAllowedOperation != null))
			{
				Rule rule = this.Rules.Find(c => c.RuleId == ruleId);
                rule.Path = txtPath.Text.Trim();
				rule.Scope = ddlScope.Text.Trim();
                rule.AllowedOperation = ddlAllowedOperation.Text.Trim();

				//lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully updated.";

				gvRules.EditIndex = -1;
				this.BindGridView();
			}
		}
		
        private string BuildRules(List<Rule> Rules)
        {
            string jsonResponse = string.Empty;
            int intCounter = 0;
            string ruleExplanation = string.Empty;

            if (Rules.Count == 0) return jsonResponse;

            jsonResponse += "        \"rules\": [\r";

			foreach (Rule rule in Rules)
            {
                jsonResponse+= "          {\r";

                if (!string.IsNullOrWhiteSpace(ruleExplanation)) ruleExplanation += "\r Else If your Sitecore item path falls under sub-path ";
                if (string.IsNullOrWhiteSpace(ruleExplanation)) ruleExplanation += "If your Sitecore item path falls under sub-path ";

                if (rule?.Scope?.Trim().ToLowerInvariant() == "ignored")
                {
                    ruleExplanation += rule.Path + " then ";
                    ruleExplanation += "\r\t path is ignored from serialization";
                }
                else
                {
                    ruleExplanation += rule.Path + " then ";
                    string scope = string.Empty;
                    string allowedoperation = string.Empty;

                    if (rule?.AllowedOperation?.Trim().ToLowerInvariant() == "createupdateanddelete" || rule?.AllowedOperation?.Trim().ToLowerInvariant() ==string.Empty)
                    {
                        allowedoperation = " allow to create, update and delete ";
                    }
                    else if (rule?.AllowedOperation?.Trim().ToLowerInvariant() == "createandupdate")
                    {
                        allowedoperation = " allow to create and, update ";
                    }
                    else if (rule?.AllowedOperation?.Trim().ToLowerInvariant() == "createonly")
                    {
                        allowedoperation = " allow only to create ";
                    }

                    if (rule?.Scope?.Trim().ToLowerInvariant() == "itemanddescendants")
                    {
                        scope = "the item and its immediate children";
                    }
                    else if (rule?.Scope?.Trim().ToLowerInvariant() == "descendantsonly")
                    {
                        scope = "just the immediate children of the item (excluding the item by itself)";
                    }
                    else if (rule?.Scope?.Trim().ToLowerInvariant() == "itemandchildren")
                    {
                        scope = "the item and children at all levels";
                    }
                    else if (rule?.Scope?.Trim().ToLowerInvariant() == "singleitem")
                    {
                        scope = "just the item ";
                    }
                    

                    ruleExplanation += "\r\t" + allowedoperation + scope;
                }

                jsonResponse += "             \"scope\" : \"" + rule.Scope + "\",\r";
                jsonResponse += "             \"path\" : \"" + rule.Path + "\",\r";
                if (!string.IsNullOrWhiteSpace(rule.AllowedOperation)) jsonResponse += "             \"allowedPushOperations\" : \"" + rule.AllowedOperation + "\"\r";

				jsonResponse += "          }";
                intCounter += 1;

                if (intCounter < Rules.Count) jsonResponse += ",";

                jsonResponse += "\r";
            }

            jsonResponse += "         ]\r";

            ConditionalLogic = ruleExplanation;


            return jsonResponse;
        }

		protected void gvRules_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int ruleId = Convert.ToInt32(gvRules.DataKeys[e.RowIndex]["RuleId"]);
			Rule rule = this.Rules.Find(r => r.RuleId == ruleId);
			this.Rules.Remove(rule);

			//lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully deleted.";

			this.BindGridView();
		}

		protected void gvRules_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName.Equals("Insert"))
			{
				GridViewRow row = gvRules.FooterRow;
				TextBox txtPath = row.FindControl("txtPath") as TextBox;
				DropDownList ddlScope = row.FindControl("ddlScope") as DropDownList;
                DropDownList ddlAllowedOperation = row.FindControl("ddlAllowedOperation") as DropDownList;

				if ((txtPath != null) && (ddlScope != null) && (ddlAllowedOperation != null))
				{
					Rule rule = new Rule
					{
						RuleId = this.Rules.Max(r => r.RuleId) + 1,
						Path = txtPath.Text.Trim(),
						AllowedOperation = ddlAllowedOperation.Text.Trim(),
                        Scope = ddlScope.Text.Trim(),
					};
					this.Rules.Add(rule);

					//lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully added.";

					this.BindGridView();
				}
			}
		}

        // Module myDeserializedClass = JsonConvert.DeserializeObject<Module>(myJsonResponse);
        [Serializable()]
        public class Include
        {
            [JsonProperty("name")]
			public string Name { get; set; }
            [JsonProperty("path")]
			public string Path { get; set; }
            [JsonProperty("database")]
			public string Database { get; set; }
            [JsonProperty("rules")]
			public List<Rule> RuleList { get; set; }
        }

        [Serializable()]
        public class ModuleInfo
        {
            [JsonProperty("includes")]
			public List<Include> IncludeList { get; set; }
        }

        [Serializable()]
        public class Module
        {
            [JsonProperty("namespace")]
            public string Namespace { get; set; }

            [JsonProperty("references")]
            public List<string> References { get; set; }

            [JsonProperty("items")]
			public ModuleInfo Info { get; set; }
        }

		[Serializable()]
		public class Rule
		{
			public int RuleId { get; set; }
            public string Path { get; set; }
			public string Scope { get; set; }
			public string AllowedOperation { get; set; }

			public Rule()
			{
			}

			public Rule(int ruleId, string scope, string allowedOperation,string path)
			{
				this.RuleId = ruleId;
				this.Scope = scope;
				this.AllowedOperation = allowedOperation;
                this.Path = path;
			}
		}

        private void RunPSScript()
        {
            // Initialize PowerShell engine
            var shell = PowerShell.Create();

            // Add the script to the PowerShell object
            shell.Commands.AddScript(basePath + "\\scs.ps1");

            // Execute the script
            var results = shell.Invoke();

            // display results, with BaseObject converted to string
            // Note : use |out-string for console-like output
            if (results.Count > 0)
            {
                // We use a string builder ton create our result text
                var builder = new StringBuilder();

                foreach (var psObject in results)
                {
                    // Convert the Base Object to a string and append it to the string builder.
                    // Add \r\n for line breaks
                    builder.Append(psObject.BaseObject.ToString() + "\r\n");
                }

                // Encode the string in HTML (prevent security issue with 'dangerous' caracters like < >
            }
        }

        protected void btnModuleLoad_Click(object sender, EventArgs e)
        {
            switch (btnModuleLoad.Text.Trim().ToLower())
            {
                case "setup serialization":
                    RunPSScript();
                    break;
                case "setup module json files":
                    AddModuleJsonUnderFeatureFolders();
                    SerializeFoundationContent();
                    AddModuleJsonUnderProjectFolders();
                    break;
                case "load module information":
                    ViewState["ModuleFileName"] = ddlModuleJsonList.SelectedValue;
                    LoadModuleJson(); ;
                    break;
            }      
        }

        private string GetProjectLines(string projectName)
        {
            var concatLines = string.Empty;

            concatLines += "{\r";
            concatLines += "  \"namespace\": \"Project." + projectName + "\",\r";
            concatLines += "  \"references\": [\r";
            concatLines += "  \"Foundation.*\"],\r";
            concatLines += "  \"Feature.*\"],\r";
            concatLines += "  \"items\": {\r";
            concatLines += "    \"includes\": [\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Templates\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Project/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Renderings\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/renderings/Project/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"PlaceholderSettings\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/placeholder settings/Project/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Models\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/models/Project/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Content\",\r";
            concatLines += "        \"path\": \"/sitecore/content/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Profiling\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Marketing Control Panel/Profiles/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Outcomes\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Marketing Control Panel/Outcomes/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Campaigns\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Marketing Control Panel/Campaigns/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "        \"name\": \"Goals\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Marketing Control Panel/Goals/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Media\",\r";
            concatLines += "        \"path\": \"/sitecore/media library/Project/" + projectName + "\",\r";
            concatLines += "        \"database\": \"master\"\r";
            concatLines += "      }\r";
            concatLines += "    ]\r";
            concatLines += "  }\r";
            concatLines += "}";

            return concatLines;
        }

        private string GetScsPSLines()
        {
            var concatLines = string.Empty;
            string DomainName = HttpContext.Current.Request.Url.Host;
            string idServerUrl = FetchIdentityServerfromConfig();

            concatLines += "Set-Location -Path \"" + basePath + "\";dotnet new tool-manifest; dotnet tool install Sitecore.CLI --add-source https://sitecore.myget.org/F/sc-packages/api/v3/index.json; dotnet sitecore init;dotnet sitecore login --authority " + idServerUrl + " --cm https://" + DomainName + " --allow-write true;dotnet sitecore plugin add -n Sitecore.DevEx.Extensibility.Serialization";
            
            return concatLines;
        }

        private string FetchIdentityServerfromConfig()
        {
            //App_Config\Sitecore\Owin.Authentication.IdentityServer
            string configFile= HttpContext.Current.Server.MapPath("~/App_Config/Sitecore/Owin.Authentication.IdentityServer/Sitecore.Owin.Authentication.IdentityServer.config");

            IEnumerable<string> lines = File.ReadLines(configFile);
            
            foreach (string line in lines)
            {
                if (line.Contains("sc.variable name=\"identityServerAuthority\""))
                {
                    int startOfIdServer = line.LastIndexOf("value=\"");
                    int rightofDblQuote= line.LastIndexOf("\"");
                    int width = rightofDblQuote - startOfIdServer;

                    string idServerUrl = line.Substring(startOfIdServer+7, width-7);

                    return idServerUrl;
                }
            }

            return string.Empty;
        }


        private string GetFeatureLines(string featureName)
        {
                var concatLines = string.Empty;

                concatLines += "{\r";
                concatLines += "  \"namespace\": \"Feature." + featureName + "\",\r";
                concatLines += "  \"references\": [\r";
                concatLines += "  \"Foundation.*\"],\r";
                concatLines += "  \"items\": {\r";
                concatLines += "    \"includes\": [\r";
                concatLines += "      {\r";
                concatLines += "        \"name\": \"Templates\",\r";
                concatLines += "        \"path\": \"/sitecore/templates/Feature/" + featureName + "\",\r";
                concatLines += "        \"database\": \"master\"\r";
                concatLines += "      },\r";
                concatLines += "      {\r";
                concatLines += "        \"name\": \"Renderings\",\r";
                concatLines += "        \"path\": \"/sitecore/layout/renderings/Feature/" + featureName + "\",\r";
                concatLines += "        \"database\": \"master\"\r";
                concatLines += "      },\r";
                concatLines += "      {\r";
                concatLines += "        \"name\": \"Media\",\r";
                concatLines += "        \"path\": \"/sitecore/media library/Feature/" + featureName + "\",\r";
                concatLines += "        \"database\": \"master\"\r";
                concatLines += "      }\r";
                concatLines += "    ]\r";
                concatLines += "  }\r";
                concatLines += "}";

                return concatLines;
        }

        private string GetFoundationLines()
        {
            var concatLines = string.Empty;

            concatLines += "{\r";
            concatLines += "  \"namespace\": \"Foundation.Content\",\r";
            concatLines += "  \"references\": [\r";
            concatLines += "  ],\r";
            concatLines += "  \"items\": {\r";
            concatLines += "    \"includes\": [\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Settings.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Settings/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Settings.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Settings/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Settings.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/system/Settings/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Media.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/media library/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Media.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/media library/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Media.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/media library/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Templates.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Templates.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Templates.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Layouts.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/layouts/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Layouts.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/layouts/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Layouts.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/layouts/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Models.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/models/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Models.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/models/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Models.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/models/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"PlaceholderSettings.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/Placeholder Settings/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"PlaceholderSettings.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/Placeholder Settings/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"PlaceholderSettings.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/Placeholder Settings/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Renderings.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/renderings/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Renderings.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/renderings/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Renderings.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/layout/renderings/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Branches.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/branches/Feature\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Branches.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/branches/Foundation\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Branches.Project\",\r";
            concatLines += "        \"path\": \"/sitecore//templates/branches/Project\",\r";
            concatLines += "        \"database\": \"master\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Core.Templates.Feature\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Feature\",\r";
            concatLines += "        \"database\": \"core\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Core.Templates.Foundation\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Foundation\",\r";
            concatLines += "        \"database\": \"core\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      },\r";
            concatLines += "      {\r";
            concatLines += "        \"name\": \"Core.Templates.Project\",\r";
            concatLines += "        \"path\": \"/sitecore/templates/Project\",\r";
            concatLines += "        \"database\": \"core\",\r";
            concatLines += "        \"scope\": \"SingleItem\"\r";
            concatLines += "      }\r";
            concatLines += "    ]\r";
            concatLines += "  }\r";
            concatLines += "}";

            return concatLines;
        }

        private void SerializeFeatureContent(string featureName)
        {
            string fileName = projectPath + @"\Feature\" + featureName + @"\" + featureName  + ".module.json";
            FileInfo fi = new FileInfo(fileName);
            string featureLines = GetFeatureLines(featureName);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(featureLines);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }

        private void SerializeProjectContent(string projectName)
        {
            string fileName = projectPath + @"\Project\" + projectName + @"\" + projectName + ".module.json";
            FileInfo fi = new FileInfo(fileName);
            string projectLines = GetProjectLines(projectName);

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(projectLines);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }

        private void SerializeFoundationContent()
        {
            string fileName = projectPath + @"\Foundation\Serialization\Serialization.module.json";
            FileInfo fi = new FileInfo(fileName);
            string foundationLines = GetFoundationLines();

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(foundationLines);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }

        private void CreateScsPs1()
        {
            string fileName = basePath + @"\scs.ps1";
            FileInfo fi = new FileInfo(fileName);
            string foundationLines = GetScsPSLines();

            try
            {
                // Check if file already exists. If yes, delete it.     
                if (fi.Exists)
                {
                    fi.Delete();
                }

                // Create a new file     
                using (StreamWriter sw = fi.CreateText())
                {
                    sw.WriteLine(foundationLines);
                }

            }
            catch (Exception Ex)
            {
                Console.WriteLine(Ex.ToString());
            }

        }

        private void LoadModuleJson()
        {
            try
            {
                this.txtModuleJson.Text = ShowContent(ddlModuleJsonList.SelectedValue);
                Module module = JsonConvert.DeserializeObject<Module>(this.txtModuleJson.Text);
                ViewState["Module"] = txtModuleJson.Text;

                if (module == null) return;

                ddlIncludeList.Items.Clear();
                foreach (Include include in module?.Info?.IncludeList)
                {
                    ddlIncludeList.Items.Add(include.Name);
                }

                string str = string.Empty;
                using (Stream ms = new MemoryStream(Encoding.ASCII.GetBytes(str)))
                using (StreamReader sr = new StreamReader(ms, Encoding.UTF8))
                {
                    string str2 = sr.ReadToEnd();
                    Console.WriteLine(string.Join(",", str2.Select(c => ((int)c))));
                }
            }
            catch(Exception ex)
            {
                lblStatus.Text = "Error in Module Json";
            }
        }
		
        public string ShowContent(string path)
        {
            string strInput = "";
            string GetStream = "";

            if (File.Exists(path))
            {
                StreamReader sr = new StreamReader(path, Encoding.UTF8);
                strInput = sr.ReadToEnd();
                while (strInput != null)
                {
                    GetStream += strInput;
                    strInput = sr.ReadLine();
                }
                sr.Close();
            }
            else
            {
                //Response.Write("file does not exist!");
                lblStatus.Text = "Module JSON does not exist, perform initial serialization";
            }
            return GetStream;
        }

        protected void btnEditRules_Click(object sender, EventArgs e)
        {
            string moduleJsonString = string.Empty;
            int intIncludeCounter = 0;
            int intRefCounter = 0;
            string rulesJson=BuildRules(Rules);

            txtModuleJson.Text = (string)ViewState["Module"];

            Module module = JsonConvert.DeserializeObject<Module>(this.txtModuleJson.Text); 

            if (module == null) return;

            moduleJsonString += "{\r";
            moduleJsonString += "  \"namespace\": \"" + module.Namespace + "\",\r";

            if (module.References!=null)
            {
                moduleJsonString += "  \"references\": [\r";

                foreach (string reference in module.References)
                {
                    moduleJsonString += "    \"" + reference + "\"";
                    intRefCounter += 1;

                    if (intRefCounter < module.References.Count)
                    {
                        moduleJsonString += ",";
                    }

                    moduleJsonString += "\r";
                }

                moduleJsonString += "  ],\r";
            }
            
            if (!module.Info.IncludeList.Any()) return;

            moduleJsonString += "  \"items\": {\r";
            moduleJsonString += "    \"includes\": [\r";
            

            foreach (Include include in module.Info.IncludeList)
            {
                moduleJsonString += "      {\r";
                moduleJsonString += "        \"name\": \"" + include.Name + "\",\r";
                moduleJsonString += "        \"path\": \"" + include.Path + "\",\r";
                moduleJsonString += "        \"database\": \"" + include.Database + "\"";

                if (include?.RuleList?.Any()==true) moduleJsonString += ",\r" + BuildRules(include.RuleList);

                //txtIfElseBlock.Text = "In case of Sitecore Item path - " + include.Path + " in Sitecore " + UppercaseFirst(include.Database) + " DB: " + "\r\r" + ConditionalLogic;
                    //txtIfElseBlock.Text = "Sitecore Database: " + UppercaseFirst(include.Database) + "\r";
                    //txtIfElseBlock.Text += "Sitecore Root Path: " + include.Path + "\r\r";

                txtIfElseBlock.Text = "Sitecore Database: Master\r";
                txtIfElseBlock.Text += "Sitecore Root Path: \"/sitecore/system/Settings/Feature\"\r\r";
                //if (!FindItem("shell", include.Database, include.Path)) lblStatus.Text = include.Path + " not found in " + UppercaseFirst(include.Database);

                txtIfElseBlock.Text += ConditionalLogic;

                if (!string.IsNullOrWhiteSpace(rulesJson))
                {
                    moduleJsonString += ",\r";

                    if (ddlIncludeList.Text.Trim() == include.Name.Trim())
                    {
                        moduleJsonString += rulesJson;
                        rulesJson = string.Empty;
                    }
                }

                moduleJsonString += "\r";

               intIncludeCounter += 1;
                moduleJsonString += "      }";

                if(intIncludeCounter< module.Info.IncludeList.Count) moduleJsonString += ",";

                moduleJsonString += "\r";
            }

            moduleJsonString += "    ]\r";

            moduleJsonString += "  }\r";
            
            moduleJsonString += "}";

            txtModuleJson.Text = moduleJsonString;
            ViewState["Module"] =txtModuleJson.Text;
        }

        private static bool IsValidJson(string strInput)
        {
            if (string.IsNullOrWhiteSpace(strInput)) { return false; }
            strInput = strInput.Trim();
            if ((strInput.StartsWith("{") && strInput.EndsWith("}")) || //For object
                (strInput.StartsWith("[") && strInput.EndsWith("]"))) //For array
            {
                try
                {
                    var obj = JToken.Parse(strInput);
                    return true;
                }
                catch (JsonReaderException jex)
                {
                    //Exception in parsing json
                    Console.WriteLine(jex.Message);
                    return false;
                }
                catch (Exception ex) //some other exception
                {
                    Console.WriteLine(ex.ToString());
                    return false;
                }
            }

            return false;
        }

        protected void btnValidateJson_Click(object sender, EventArgs e)
        {
            if (!IsValidJson(txtModuleJson.Text))
            {
                btnValidateJson.Text = "Invalid Json?";
                btnValidateJson.BackColor = Color.Red;
            }
            else
            {
                btnValidateJson.Text = "Valid Json";
                btnValidateJson.BackColor = Color.Green;
            }    
        }

        protected void btnSaveAs_Click(object sender, EventArgs e)
        {
            string filepath = (string)ViewState["ModuleFileName"];
            if (!string.IsNullOrWhiteSpace(filepath))
            {
                try
                {
                    File.WriteAllText(filepath , txtModuleJson.Text);
                    //ModuleFileUpload.SaveAs(projectpath + "\\" +
                    //                        ViewState["ModuleFileName"].ToString());
                    //Label1.Text = "File name: " +
                    //              FileUpload1.PostedFile.FileName + "<br>" +
                    //              FileUpload1.PostedFile.ContentLength + " kb<br>" +
                    //              "Content type: " +
                    //              FileUpload1.PostedFile.ContentType;
                }
                catch (Exception ex)
                {
                    // Label1.Text = "ERROR: " + ex.Message.ToString();
                }
                
            }
        }

        private static string UppercaseFirst(string s)
        {
            // Check for empty string.
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            // Return char and concat substring.
            return char.ToUpper(s[0]) + s.Substring(1);
        }

        protected void ddlModuleJsonList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["ModuleFileName"] = ddlModuleJsonList.SelectedValue;

            LoadModuleJson();
        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            LaunchPSScript(@".\sync.ps1", basePath);
        }

        public static void LaunchPSScript(string scriptname, string workingDirectory = ".")
        {
            var script = scriptname;
            var startInfo = new ProcessStartInfo()
            {
                FileName = @"powershell.exe",
                WorkingDirectory = workingDirectory,
                Arguments = $"-NoProfile -noexit -ExecutionPolicy Bypass \"{script}\"",
                UseShellExecute = false
            };
            Process.Start(startInfo);
        }

        protected void Button2_Click(object sender, EventArgs e)
        {
            LaunchPSScript(@".\serialize.ps1"  , basePath);           
        }

        protected void ddlIncludeList_SelectedIndexChanged(object sender, EventArgs e)
        {
            ViewState["ModuleFileName"] = ddlModuleJsonList.SelectedValue;

            LoadModuleJson();
        }

        protected void ddlModuleJsonList_TextChanged(object sender, EventArgs e)
        {
            
        }

        private static string ExecuteCommandAsAdmin(string command)
        {
            command = "dir";
            ProcessStartInfo procStartInfo = new ProcessStartInfo()
            {
                RedirectStandardError = true,
                RedirectStandardOutput = true,
                UseShellExecute = false,
                CreateNoWindow = true,
                FileName = "runas.exe",
                Arguments = "/user:Administrator cmd /K " + command
            };

            using (Process proc = new Process())
            {
                proc.StartInfo = procStartInfo;
                proc.Start();

                string output = proc.StandardOutput.ReadToEnd();

                if (string.IsNullOrEmpty(output))
                    output = proc.StandardError.ReadToEnd();

                return output;
            }
        }

        protected void btnModuleLoad0_Click(object sender, EventArgs e)
        {
            //Runspace cmdlet = RunspaceFactory.CreateRunspace();
            //cmdlet.Open();
            //RunspaceInvoke scriptInvoker = new RunspaceInvoke(cmdlet);
            //// set powershell execution policy to unrestricted
            //scriptInvoker.Invoke("Set-ExecutionPolicy Unrestricted");
            //// create a pipeline and load it with command object
            //Pipeline pipeline = cmdlet.CreatePipeline();
            //Command cmd = new Command("Set-Location -Path \"" + basePath + "\";dotnet new tool-manifest; dotnet tool install Sitecore.CLI --add-source https://sitecore.myget.org/F/sc-packages/api/v3/index.json; ");
            //// Using Get-SPFarm powershell command
            //pipeline.Commands.Add(cmd);
            //// this will format the output
            //IEnumerable<PSObject> output = pipeline.Invoke();
            //pipeline.Stop();
            //cmdlet.Close();
            //// process each object in the output and append to stringbuilder 
            //StringBuilder results = new StringBuilder();
            //foreach (PSObject obj in output)
            //{
            //    results.AppendLine(obj.ToString());
            //}

            //ExecuteCommandAsAdmin("powershell .\\scs.ps1");

            //try
            //{

            //    string tempGETCMD = null;
            //    Process CMDprocess = new Process();
            //    System.Diagnostics.ProcessStartInfo StartInfo = new System.Diagnostics.ProcessStartInfo();
            //    StartInfo.FileName = "cmd"; //starts cmd window
            //    StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            //    StartInfo.CreateNoWindow = true;
            //    StartInfo.RedirectStandardInput = true;
            //    StartInfo.RedirectStandardOutput = true;
            //    StartInfo.UseShellExecute = false; //required to redirect
            //    //StartInfo.Arguments = "/user:Administrator \"cmd /K " + command + "\""
            //    StartInfo.Arguments = "/user:Administrator \"cmd /K \"";
            //    CMDprocess.StartInfo = StartInfo;
            //    CMDprocess.Start();
            //    using (System.IO.StreamReader SR = CMDprocess.StandardOutput)
            //    {
            //        using (System.IO.StreamWriter SW = CMDprocess.StandardInput)
            //        {
            //            SW.WriteLine("@echo on");
            //            SW.WriteLine("cd " + basePath);
            //            SW.WriteLine("powershell .\\scs.ps1");
            //            SW.WriteLine("exit"); //exits command prompt window
            //        }
            //        tempGETCMD = SR.ReadToEnd(); //returns results of the command window
            //    }
            //}
            //catch (Exception ex)
            //{
            //}

            string tempGETCMD = null;
            Process CMDprocess = new Process();
            System.Diagnostics.ProcessStartInfo StartInfo = new System.Diagnostics.ProcessStartInfo();
            StartInfo.FileName = "cmd";
            StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            StartInfo.CreateNoWindow = true;
            StartInfo.RedirectStandardInput = true;
            StartInfo.RedirectStandardOutput = true;//redirect the standard output
            StartInfo.UseShellExecute = false;
            CMDprocess.StartInfo = StartInfo;
            CMDprocess.Start();

            System.IO.StreamReader SR = CMDprocess.StandardOutput;
            System.IO.StreamWriter SW = CMDprocess.StandardInput;
            SW.WriteLine("@echo on");

            SW.WriteLine("cd C:\\Program Files\\PowerShell\\");

            //SW.WriteLine("powershell D:\\poweron.ps1 parameters");
            SW.WriteLine("powershell c:\\projects\\myprojectbase\\scs.ps1");

            //SW.WriteLine("exit");
            tempGETCMD = SR.ReadToEnd(); //returns results of the command window

            SW.Close();
            SR.Close();
        }
    }
}