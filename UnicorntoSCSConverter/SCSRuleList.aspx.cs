using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UnicorntoSCSConverter
{
    public partial class SCSRuleList : System.Web.UI.Page
   {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetInitialRow();
            }
            //else
            //{
            //    if (Session["CurrentTable"] != null)
            //    {
            //        DataTable dt = new DataTable();
            //        dt = (DataTable)Session["CurrentTable"];

            //        grdVwRuleList.DataSource = dt;
            //        grdVwRuleList.DataBind();
            //    }

            //}
        }

        private void AddNewRowToGrid()
        {

            int rowIndex = 0;



            if (Session["CurrentTable"] != null)

            {

                DataTable dtCurrentTable = (DataTable)Session["CurrentTable"];

                DataRow drCurrentRow = null;

                if (dtCurrentTable.Rows.Count > 0)
                {

                    for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)

                    {

                        //extract the TextBox values

                        DropDownList box1 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[1].FindControl("ddlScope");

                        DropDownList box2 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[2].FindControl("ddlAllowedOperation");

                        TextBox box3 = (TextBox)grdVwRuleList.Rows[rowIndex].Cells[3].FindControl("txtPath");



                        drCurrentRow = dtCurrentTable.NewRow();

                        drCurrentRow["RuleID"] = i + 1;



                        dtCurrentTable.Rows[i - 1]["Scope"] = box1.SelectedValue;

                        dtCurrentTable.Rows[i - 1]["AllowedOperation"] = box2.SelectedValue;

                        dtCurrentTable.Rows[i - 1]["Path"] = box3.Text;



                        rowIndex++;

                    }

                    dtCurrentTable.Rows.Add(drCurrentRow);

                    ViewState["CurrentTable"] = dtCurrentTable;



                    grdVwRuleList.DataSource = dtCurrentTable;

                    grdVwRuleList.DataBind();

                }

            }

            else

            {

                Response.Write("ViewState is null");

            }



            //Set Previous Data on Postbacks

            SetPreviousData();

        }

        private void SetPreviousData()

        {

            int rowIndex = 0;

            if (Session["CurrentTable"] != null)

            {

                DataTable dt = (DataTable)Session["CurrentTable"];

                if (dt.Rows.Count > 0)

                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {

                       

                        DropDownList box1 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[1].FindControl("ddlScope");

                        DropDownList box2 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[2].FindControl("ddlAllowedOperation");

                        TextBox box3 = (TextBox)grdVwRuleList.Rows[rowIndex].Cells[3].FindControl("txtPath");



                        box1.Text = dt.Rows[i]["Scope"].ToString();

                        box2.Text = dt.Rows[i]["AllowedOperation"].ToString();

                        box3.Text = dt.Rows[i]["Path"].ToString();



                        rowIndex++;

                    }

                }

            }

        }

        private void SetInitialRow()
        {

            DataTable dt = new DataTable();

            DataRow dr = null;

            dt.Columns.Add(new DataColumn("RuleID", typeof(string)));

            dt.Columns.Add(new DataColumn("Scope", typeof(string)));

            dt.Columns.Add(new DataColumn("AllowedOperation", typeof(string)));

            dt.Columns.Add(new DataColumn("Path", typeof(string)));


            dr = dt.NewRow();

            dr["RuleID"] = 1;


            dr["Scope"] = "ignored";

            dr["AllowedOperation"] = "CreateUpdateAndDelete";

            dr["Path"] = string.Empty;


            dt.Rows.Add(dr);

            //dr = dt.NewRow();

            //Store the DataTable in ViewState

            Session["CurrentTable"] = dt;



            grdVwRuleList.DataSource = dt;

            grdVwRuleList.DataBind();

        }

        //public void InsertRule(string path, string allowedOperation, string scope)
        //{
        //    RuleList ruleList = new RuleList();
        //    List<RuleList> listofrules = new List<RuleList>();
        //    listofrules = (List<RuleList>)Session["Rules"];

        //    ruleList.AllowedOperation = allowedOperation;
        //    ruleList.Scope = scope;
        //    ruleList.Path = path;
        //    listofrules.Add(ruleList);
        //    objRuleListDataSource.;

        //    grdVwRuleList.DataSource = listofrules;

        //    grdVwRuleList.DataBind();
        //}

        public void UpdateRule(string path, string allowedOperation, string scope)
        {
        }

        protected void grdVwRuleList_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            if (e.CommandName == "Update")
            {

                Label lblRuleID = new Label();

                lblRuleID = grdVwRuleList.Rows[Convert.ToInt16(e.CommandArgument)].Cells[1].Controls[1] as Label;



                DropDownList ddlScope = new DropDownList();

                ddlScope = grdVwRuleList.Rows[Convert.ToInt16(e.CommandArgument)].Cells[2].Controls[1] as DropDownList;



                DropDownList ddlAllowedOperation = new DropDownList();

                ddlAllowedOperation = grdVwRuleList.Rows[Convert.ToInt16(e.CommandArgument)].Cells[2].Controls[1] as DropDownList;



                TextBox txtPath = new TextBox();

                txtPath = grdVwRuleList.Rows[Convert.ToInt16(e.CommandArgument)].Cells[3].Controls[1] as TextBox;



                objRuleListDataSource.UpdateParameters["Path"].DefaultValue = txtPath.Text;
                objRuleListDataSource.UpdateParameters["AllowedOperation"].DefaultValue = ddlAllowedOperation.SelectedValue;
                objRuleListDataSource.UpdateParameters["Scope"].DefaultValue = ddlScope.SelectedValue;


            }

        }

        public void DeleteRule(int RuleID)
        {

           

        }

        protected void grdVwRuleList_RowDataBound(object sender, GridViewRowEventArgs e)
        {

            if (e.Row.RowType == DataControlRowType.DataRow)
            {

                LinkButton btnDelete = new LinkButton();

                btnDelete = e.Row.Cells[0].Controls[2] as LinkButton;

                //below check is necessary, else it will give problem while editing record.
                if (btnDelete.CommandName == "Delete")

                    btnDelete.OnClientClick = "return confirm('do you want to delete ?');";

            }

        }


        public List<RuleList> GetAllRules()
        {
            List<RuleList> listofrules = new List<RuleList>();

            RuleList ruleList = new RuleList();
            ruleList.RuleID = 1;
            ruleList.Path = "/brands/abc/xxx";
            ruleList.Scope = "ignored";
            ruleList.AllowedOperation = "CreateOnly";
            listofrules.Add(ruleList);

            ruleList = new RuleList();
            ruleList.RuleID = 2;
            ruleList.Path = "/brands/abc";
            ruleList.Scope = "ItemAndDescendants";
            ruleList.AllowedOperation = "CreateUpdateAndDelete";
            //ruleList.ListofRules.Add(ruleList);

            listofrules.Add(ruleList);
            Session["Rules"] = listofrules;
            ruleList.ListofRules = listofrules;
            return listofrules;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            AddNewRowToGrid();

            //objRuleListDataSource.InsertParameters["Path"].DefaultValue = GetTxtBoxValue("txtPath");

            //objRuleListDataSource.InsertParameters["AllowedOperation"].DefaultValue = GetGridDropDownValue("ddlAllowedOperation");

            //objRuleListDataSource.InsertParameters["Scope"].DefaultValue = GetGridDropDownValue("ddlScope");

            //objRuleListDataSource.Insert();

        }

        public string GetGridDropDownValue(string ctlID)
        {
                DropDownList txt = (DropDownList)grdVwRuleList.FooterRow.FindControl(ctlID);
                return txt.Text;
        }

        public string GetTxtBoxValue(string ctlID)
        {
            TextBox txt = (TextBox)grdVwRuleList.FooterRow.FindControl(ctlID);
            return txt.Text;
        }

        protected void grdVwRuleList_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        protected void grdVwRuleList_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            DataTable dt = new DataTable();


            int rowIndex = 0;

            if (Session["CurrentTable"] != null)

            {

                dt = (DataTable)Session["CurrentTable"];
                dt.Rows.RemoveAt(e.RowIndex+1);
                Session["CurrentTable"] = dt;

                if (dt.Rows.Count > 0)

                {

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {



                        DropDownList box1 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[1].FindControl("ddlScope");

                        DropDownList box2 = (DropDownList)grdVwRuleList.Rows[rowIndex].Cells[2].FindControl("ddlAllowedOperation");

                        TextBox box3 = (TextBox)grdVwRuleList.Rows[rowIndex].Cells[3].FindControl("txtPath");



                        box1.Text = !string.IsNullOrWhiteSpace(dt.Rows[i]["Scope"].ToString()) ? dt.Rows[i] ["Scope"].ToString(): "ItemAndDescendants";

                        box2.Text = !string.IsNullOrWhiteSpace(dt.Rows[i]["AllowedOperation"].ToString()) ? dt.Rows[i]["AllowedOperation"].ToString() : "CreateUpdateAndDelete";

                        box3.Text = !string.IsNullOrWhiteSpace(dt.Rows[i]["Path"].ToString()) ? dt.Rows[i]["Path"].ToString() : string.Empty;



                        rowIndex++;

                    }

                }

            }

        }
    }
}