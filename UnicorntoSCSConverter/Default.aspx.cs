using System;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Configuration;


namespace EmployeeSys
{
    public partial class WebForm1 : System.Web.UI.Page
    {
       
        protected void Page_Load(object sender, EventArgs e)
        {
            if (IsPostBack == false)
            {
                SetInitialRow();

            
            }
            Label1.Visible = false;
        }

        private void SetInitialRow()
        {
            HiddenField1.Value = "view";

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




            GridView1.DataSource = dt;

            GridView1.DataBind();

        }

        //private void AddNewRowToGrid()
        //{

        //    int rowIndex = 0;



        //    if (Session["CurrentTable"] != null)

        //    {

        //        DataTable dtCurrentTable = (DataTable)Session["CurrentTable"];

        //        DataRow drCurrentRow = null;

        //        if (dtCurrentTable.Rows.Count > 0)
        //        {

        //            for (int i = 1; i <= dtCurrentTable.Rows.Count; i++)

        //            {

        //                //extract the TextBox values

        //                DropDownList box1 = (DropDownList)GridView1.Rows[rowIndex].Cells[1].FindControl("ddlScope");

        //                DropDownList box2 = (DropDownList)GridView1.Rows[rowIndex].Cells[2].FindControl("ddlAllowedOperation");

        //                TextBox box3 = (TextBox)GridView1.Rows[rowIndex].Cells[3].FindControl("txtPath");



        //                drCurrentRow = dtCurrentTable.NewRow();

        //                drCurrentRow["RuleID"] = i + 1;



        //                dtCurrentTable.Rows[i - 1]["Scope"] = box1.SelectedValue;

        //                dtCurrentTable.Rows[i - 1]["AllowedOperation"] = box2.SelectedValue;

        //                dtCurrentTable.Rows[i - 1]["Path"] = box3.Text;



        //                rowIndex++;

        //            }

        //            dtCurrentTable.Rows.Add(drCurrentRow);

        //            ViewState["CurrentTable"] = dtCurrentTable;



        //            grdVwRuleList.DataSource = dtCurrentTable;

        //            grdVwRuleList.DataBind();

        //        }

        //    }

        //    else

        //    {

        //        Response.Write("ViewState is null");

        //    }



        //    //Set Previous Data on Postbacks

        //    SetPreviousData();

        //}


        //private void AddNewRow(string scope,string allowedoperation, string path)
        //{


        //    DataRow dr = null;

        //    dt.Columns.Add(new DataColumn("RuleID", typeof(string)));

        //    dt.Columns.Add(new DataColumn("Scope", typeof(string)));

        //    dt.Columns.Add(new DataColumn("AllowedOperation", typeof(string)));

        //    dt.Columns.Add(new DataColumn("Path", typeof(string)));


        //    dr = dt.NewRow();

        //    dr["RuleID"] = 1;


        //    dr["Scope"] = scope;

        //    dr["AllowedOperation"] = allowedoperation;

        //    dr["Path"] = path;


        //    dt.Rows.Add(dr);

        //    //dr = dt.NewRow();

        //    //Store the DataTable in ViewState




        //    GridView1.DataSource = dt;

        //    GridView1.DataBind();

        //}

        protected void empsave(object sender, EventArgs e)
        {

            HiddenField1.Value = "Insert";
            //AddNewRow(TextBox1.Text, TextBox2.Text, TextBox3.Text);

            gedata();

        }
        public void gedata()
        {
            HiddenField1.Value = "view";

            SetInitialRow();

        }
        protected void edit(object sender, GridViewEditEventArgs e)
        {
            GridView1.EditIndex= e.NewEditIndex;
            gedata();


           
        }
        protected void canceledit(object sender, GridViewCancelEditEventArgs e)
        {

            GridView1.EditIndex = -1;
            gedata();
        }
        protected void update(object sender, GridViewUpdateEventArgs e)
        {
            int id=int.Parse(GridView1.DataKeys[e.RowIndex].Value.ToString());
            HiddenField1.Value = "update";
            
            GridView1.EditIndex = -1;
            gedata();
        
        }

        protected void delete(object sender, GridViewDeleteEventArgs e)
        {
           int id = int.Parse(GridView1.DataKeys[e.RowIndex].Value.ToString());
            HiddenField1.Value = "Delete";
            gedata();
        
        
        
        }
    }
}