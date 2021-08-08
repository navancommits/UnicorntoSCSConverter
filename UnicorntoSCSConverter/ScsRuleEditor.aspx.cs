using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace UnicorntoSCSConverter
{
    public partial class WebForm2 : System.Web.UI.Page
    {

		protected void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				this.BindGridView();
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

				lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully updated.";

				gvRules.EditIndex = -1;
				this.BindGridView();
			}
		}
		
        private string BuildRules()
        {
            string jsonResponse = string.Empty;
            int intCounter = 0;

            if (Rules.Count == 0) return jsonResponse;

            jsonResponse += "        \"rules\": [\r";

			foreach (Rule rule in Rules)
            {
                jsonResponse+= "          {\r";

                jsonResponse += "             \"scope\" : \"" + rule.Scope + "\",\r";
                jsonResponse += "             \"path\" : \"" + rule.Path + "\",\r";
                jsonResponse += "             \"allowedPushOperations\" : \"" + rule.AllowedOperation + "\"\r";

				jsonResponse += "          }";
                intCounter += 1;

                if (intCounter < Rules.Count) jsonResponse += ",";

                jsonResponse += "\r";
            }

            jsonResponse += "         ]\r";

			return jsonResponse;
        }

		protected void gvRules_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int ruleId = Convert.ToInt32(gvRules.DataKeys[e.RowIndex]["RuleId"]);
			Rule rule = this.Rules.Find(r => r.RuleId == ruleId);
			this.Rules.Remove(rule);

			lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully deleted.";

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

					lblMessage.Text = $"Rule '{rule.Path} {rule.Scope}' successfully added.";

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

        protected void btnModuleLoad_Click(object sender, EventArgs e)
        {
            // Please change the value of path which used to store the file.
            string path = AppDomain.CurrentDomain.BaseDirectory + "UploadFiles\\" + this.ModuleFileUpload.FileName;
            this.ModuleFileUpload.SaveAs(path);
            this.txtModuleJson.Text = ShowContent(path);
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
                Response.Write("file does not exist!");
            }
            return GetStream;
        }

        protected void btnEditRules_Click(object sender, EventArgs e)
        {
            string moduleJsonString = string.Empty;
            int intIncludeCounter = 0;
            int intRefCounter = 0;
            string rulesJson=BuildRules();
            Module module = (Module)ViewState["Module"];

            if (module == null) return;

            moduleJsonString += "{\r";
            moduleJsonString += "  \"namespace\": \"" + module.Namespace + "\",\r";

            if (module.References.Any())
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

                if (!string.IsNullOrWhiteSpace(rulesJson))
                {
                    moduleJsonString += ",\r";

                    if (ddlIncludeList.Text.Trim() == include.Name.Trim())
                    {
                        moduleJsonString += rulesJson;
                    }

                    rulesJson=string.Empty;
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
    }
}