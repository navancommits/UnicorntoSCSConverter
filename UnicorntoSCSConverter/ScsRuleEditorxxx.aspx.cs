using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace UnicorntoSCSConverter
{
    public partial class WebForm1 : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                this.BindGridView();
            }
        }

		private List<Customer> Customers
		{
			get
			{
				if (this.ViewState["Customer"] == null)
				{
					this.ViewState["Customer"] = this.GetCustomers();
				}

				return this.ViewState["Customer"] as List<Customer>;
			}
		}

		private void BindGridView()
		{
			gvCustomers.DataSource = this.Customers;
			gvCustomers.DataBind();
		}

		private List<Customer> GetCustomers()
		{
			List<Customer> customers = new List<Customer>();
			customers.Add(new Customer(1, "John", "Doe"));
			customers.Add(new Customer(2, "Jane", "Doe"));
			customers.Add(new Customer(3, "Jack", "BeNimble"));

			return customers;
		}


		protected void gvCustomers_RowEditing(object sender, GridViewEditEventArgs e)
		{
			gvCustomers.EditIndex = e.NewEditIndex;
			this.BindGridView();
		}

		protected void gvCustomers_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
		{
			gvCustomers.EditIndex = -1;
			this.BindGridView();
		}

		protected void gvCustomers_RowUpdating(object sender, GridViewUpdateEventArgs e)
		{
			GridViewRow row = gvCustomers.Rows[e.RowIndex];
			int customerID = Convert.ToInt32(gvCustomers.DataKeys[e.RowIndex]["CustomerID"]);
			TextBox txtFirstName = row.FindControl("txtFirstName") as TextBox;
			TextBox txtLastName = row.FindControl("txtLastName") as TextBox;

			if ((txtFirstName != null) && (txtLastName != null))
			{
				Customer customer = this.Customers.Find(c => c.CustomerID == customerID);
				customer.FirstName = txtFirstName.Text.Trim();
				customer.LastName = txtLastName.Text.Trim();

				lblMessage.Text = String.Format(
					"Customer '{0} {1}' successfully updated.",
					customer.FirstName,
					customer.LastName);

				gvCustomers.EditIndex = -1;
				this.BindGridView();
			}
		}

		protected void gvCustomers_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			int customerID = Convert.ToInt32(gvCustomers.DataKeys[e.RowIndex]["CustomerID"]);
			Customer customer = this.Customers.Find(c => c.CustomerID == customerID);
			this.Customers.Remove(customer);

			lblMessage.Text = String.Format(
				"Customer '{0} {1}' successfully deleted.",
				customer.FirstName,
				customer.LastName);

			this.BindGridView();
		}

		protected void gvCustomers_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName.Equals("Insert"))
			{
				GridViewRow row = gvCustomers.FooterRow;
				TextBox txtFirstName = row.FindControl("txtFirstName") as TextBox;
				TextBox txtLastName = row.FindControl("txtLastName") as TextBox;

				if ((txtFirstName != null) && (txtLastName != null))
				{
					Customer customer = new Customer
					{
						CustomerID = this.Customers.Max(c => c.CustomerID) + 1,
						FirstName = txtFirstName.Text.Trim(),
						LastName = txtLastName.Text.Trim()
					};
					this.Customers.Add(customer);

					lblMessage.Text = String.Format(
						"Customer '{0} {1}' successfully added.",
						customer.FirstName,
						customer.LastName);

					this.BindGridView();
				}
			}
		}

		[Serializable()]
		private class Customer
		{
			public int CustomerID { get; set; }
			public string FirstName { get; set; }
			public string LastName { get; set; }

			public Customer()
			{
			}

			public Customer(int customerID, string firstName, string lastName)
			{
				this.CustomerID = customerID;
				this.FirstName = firstName;
				this.LastName = lastName;
			}
		}
	}
}