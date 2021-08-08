using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UnicorntoSCSConverter
{
    public partial class SCSRuleEditor : System.Web.UI.Page
    {
        private RuleList ruleList = new RuleList();
        protected void Page_Load(object sender, EventArgs e)
        {
            //grdVwRuleEditor.DataSource = objDSRuleList;
        }

        protected void grdVwRuleEditor_RowCommand(object sender, GridViewCommandEventArgs e)
        {

            

        }

        public void InsertNewRule(RuleList ruleList)
        {
            ruleList.ListofRules.Add(ruleList);
        }


        protected void grdVwRuleEditor_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
        {

        }

        protected void grdVwRuleEditor_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {

        }

        protected void grdVwRuleEditor_RowUpdating(object sender, GridViewEditEventArgs e)
        {
            
        }

        protected void grdVwRuleEditor_RowUpdated(object sender, GridViewUpdatedEventArgs e)
        {

            
        }

        private void BindData()
        {
            grdVwRuleEditor.DataBind();
        }
        

        protected void ObjectDataSource1_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {

        }

        public List<RuleList> GetAllRules()
        {
            List<RuleList> listofrules = new List<RuleList>();

            RuleList ruleList = new RuleList();
            ruleList.Path = "/brands/abc/xxx";
            ruleList.Scope = "ignored";
            ruleList.AllowedOperation = "CreateOnly";
            listofrules.Add(ruleList);

            ruleList = new RuleList();
            ruleList.Path = "/brands/abc";
            ruleList.Scope = "ItemAndDescendants";
            ruleList.AllowedOperation = "CreateUpdateAndDelete";
            //ruleList.ListofRules.Add(ruleList);

            listofrules.Add(ruleList);
            //Session["Rules"] = listofrules;
            ruleList.ListofRules = listofrules;
            return listofrules;
        }

        protected void ObjectDataSource1_OnUpdating(object sender, ObjectDataSourceMethodEventArgs e)
        {
            //throw new NotImplementedException();
            BindData();
        }

        protected void grdVwRuleEditor_RowUpdating(object sender, GridViewUpdateEventArgs e)
        {
            //Bind data to the GridView control.
            BindData();
        }

        protected void grdVwRuleEditor_RowCommand1(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandSource.ToString() == "Edit")
            {

                string pushOperation = ((TextBox)grdVwRuleEditor.FooterRow.FindControl("ddlPushOperation")).Text;
                string scope = ((TextBox)grdVwRuleEditor.FooterRow.FindControl("ddlScope")).Text;

                string path = ((TextBox)grdVwRuleEditor.FooterRow.FindControl("txtPath")).Text;

                ObjectDataSource1.UpdateParameters.Add("Scope", scope);
                ObjectDataSource1.UpdateParameters.Add("Path", path);
                ObjectDataSource1.UpdateParameters.Add("AllowedOperation", pushOperation);

                ObjectDataSource1.Update();
                
            }
        }
    }




    public class RuleList
    {

        /* PRIVATE FIELDS */

        private int _ID;

        private string _allowedOperation;

        private string _scope;

        private string _path;

        public List<RuleList> _listofRules;

        /* PUBLIC PROPERTIES */

        public int RuleID
        {
            get => _ID;

            set => _ID = value;
        }

        public List<RuleList> ListofRules
        {
            get => _listofRules;

            set => _listofRules = value;
        }

        public string AllowedOperation
        {

            get => !string.IsNullOrEmpty(_allowedOperation) ? _allowedOperation : null;

            set => _allowedOperation = value;
        }

        public string Path
        {

            get => !string.IsNullOrEmpty(_path) ? _path : null;

            set => _path = value;
        }

        public string Scope
        {

            get => !string.IsNullOrEmpty(_scope) ? _scope : null;

            set => _scope = value;
        }


        
    }
}