<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="EmployeeSys.WebForm1" %>

<!DOCTYPE html PUBLIC "-//W3C//DTD XHTML 1.0 Transitional//EN" "http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd">

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body bgcolor="#669999">
    <form id="form1" runat="server">
    <h4>This Application is Created by vithal wadje for C# corner</h4>
     <table>
    <tr>
    <td>First Name</td>
<td>
   <asp:TextBox ID="TextBox1" runat="server"></asp:TextBox>    
   
        </td>    
    </tr>
    
     <tr>
    <td>Middle Name</td>
<td>
    <asp:TextBox ID="TextBox2" runat="server"></asp:TextBox>      
   
         </td>    
    </tr>
      <tr>
    <td>Last Name</td>
<td>
   <asp:TextBox ID="TextBox3" runat="server"></asp:TextBox>         
  
          </td>    
    </tr>
    <tr>
    <td>
        <asp:Button ID="Button1" runat="server" Text="Save" onclick="empsave" />
    </td>
    </tr>
    </table>
    <asp:label ID="Label1" runat="server" text="Label"></asp:label>
    <asp:HiddenField ID="HiddenField1" runat="server" />
    <asp:GridView ID="GridView1" runat="server"  DataKeyNames="RuleID" OnRowEditing="edit" 
        OnRowCancelingEdit="canceledit"
        OnRowDeleting="delete"
        OnRowUpdating="update"
        
         CellPadding="4" ForeColor="#333333" 
        GridLines="None">
        <AlternatingRowStyle BackColor="White" />
        <Columns>
            <asp:CommandField ShowEditButton="True" />
            <asp:CommandField ShowDeleteButton="True" />
        </Columns>
        <FooterStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <HeaderStyle BackColor="#990000" Font-Bold="True" ForeColor="White" />
        <PagerStyle BackColor="#FFCC66" ForeColor="#333333" HorizontalAlign="Center" />
        <RowStyle BackColor="#FFFBD6" ForeColor="#333333" />
        <SelectedRowStyle BackColor="#FFCC66" Font-Bold="True" ForeColor="Navy" />
        <SortedAscendingCellStyle BackColor="#FDF5AC" />
        <SortedAscendingHeaderStyle BackColor="#4D0000" />
        <SortedDescendingCellStyle BackColor="#FCF6C0" />
        <SortedDescendingHeaderStyle BackColor="#820000" />
    </asp:GridView>
    
    
    </form>
</body>
</html>

