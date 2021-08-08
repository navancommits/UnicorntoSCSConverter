<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScsRuleEditorxxx.aspx.cs" Inherits="UnicorntoSCSConverter.WebForm1" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
</head>
<body>
    <form id="form1" runat="server">
       
        <div>
	<asp:label id="lblMessage" runat="server" enableviewstate="false" />
	<asp:gridview id="gvCustomers" runat="server" autogeneratecolumns="false" datakeynames="CustomerID"
onrowcancelingedit="gvCustomers_RowCancelingEdit" onrowediting="gvCustomers_RowEditing"
onrowdeleting="gvCustomers_RowDeleting" showfooter="true" style="margin-top: 20px;"
onrowupdating="gvCustomers_RowUpdating" onrowcommand="gvCustomers_RowCommand">
		<columns>
			<asp:templatefield headertext="Action">
				<itemtemplate>
					<asp:linkbutton id="btnEdit" runat="server" causesvalidation="false" commandname="Edit"
text="Edit" />
					<asp:linkbutton id="btnDelete" runat="server" causesvalidation="false" commandname="Delete"
text="Delete" />
				</itemtemplate>
				<edititemtemplate>
					<asp:linkbutton id="btnUpdate" runat="server" commandname="Update" text="Update" />
					<asp:linkbutton id="btnCancel" runat="server" causesvalidation="false" commandname="Cancel"
text="Cancel" />
				</edititemtemplate>
				<footertemplate>
					<asp:linkbutton id="btnInsert" runat="server" commandname="Insert" text="Insert" />
				</footertemplate>
			</asp:templatefield>
			<asp:templatefield headertext="ID">
				<itemtemplate>
					<%# Eval("CustomerID") %>
				</itemtemplate>
				<edititemtemplate>
					<%# Eval("CustomerID") %>
				</edititemtemplate>
			</asp:templatefield>
			<asp:templatefield headertext="First Name">
				<itemtemplate>
					<%# Eval("FirstName") %>
				</itemtemplate>
				<edititemtemplate>
					<asp:textbox id="txtFirstName" runat="server" text='<%# Bind("FirstName") %>' />
				</edititemtemplate>
				<footertemplate>
					<asp:textbox id="txtFirstName" runat="server" text='<%# Bind("FirstName") %>' />
				</footertemplate>
			</asp:templatefield>
			<asp:templatefield headertext="Last Name">
				<itemtemplate>
					<%# Eval("LastName") %>
				</itemtemplate>
				<edititemtemplate>
					<asp:textbox id="txtLastName" runat="server" text='<%# Bind("LastName") %>' />
				</edititemtemplate>
				<footertemplate>
					<asp:textbox id="txtLastName" runat="server" text='<%# Bind("LastName") %>' />
				</footertemplate>
			</asp:templatefield>
		</columns>
	</asp:gridview>
        </div>
    </form>
</body>
</html>
