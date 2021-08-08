<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="SCSRuleEditorold.aspx.cs" Inherits="UnicorntoSCSConverter.SCSRuleEditor"  %>
<%@ Import Namespace="UnicorntoSCSConverter" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <asp:GridView ID="grdVwRuleEditor" runat="server" AutoGenerateColumns="False"   DataSourceID="ObjectDataSource1">
            <Columns>
                <asp:BoundField DataField="ID" HeaderText="ID" ReadOnly="True" SortExpression="ID" />
                <asp:TemplateField HeaderText="AllowedOperation" SortExpression="AllowedOperation">
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlAllowedOperation" runat="server" Text='<%# Bind("AllowedOperation") %>'>
                            <asp:ListItem value="CreateUpdateAndDelete">CreateUpdateAndDelete</asp:ListItem>
                            <asp:ListItem value="CreateAndUpdate">CreateAndUpdate</asp:ListItem>
                            <asp:ListItem value="CreateOnly">CreateOnly</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label1" runat="server" Text='<%# Bind("AllowedOperation") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Path" SortExpression="Path">
                    <EditItemTemplate>
                        <asp:TextBox ID="TextBox2" runat="server" Text='<%# Bind("Path") %>'></asp:TextBox>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label2" runat="server" Text='<%# Bind("Path") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:TemplateField HeaderText="Scope" SortExpression="Scope">
                    <EditItemTemplate>
                        <asp:DropDownList ID="ddlScope" runat="server" Text='<%# Bind("Scope") %>'>
                            <asp:ListItem value="ItemAndDescendants">ItemAndDescendants</asp:ListItem>
                            <asp:ListItem value="ItemAndChildren">ItemAndChildren</asp:ListItem>
                            <asp:ListItem value="DescendantsOnly">DescendantsOnly</asp:ListItem>
                            <asp:ListItem value="SingleItem">SingleItem</asp:ListItem>
                            <asp:ListItem value="ignored">Ignored</asp:ListItem>
                        </asp:DropDownList>
                    </EditItemTemplate>
                    <ItemTemplate>
                        <asp:Label ID="Label3" runat="server" Text='<%# Bind("Scope") %>'></asp:Label>
                    </ItemTemplate>
                </asp:TemplateField>
                <asp:CommandField HeaderText="Edit" ShowEditButton="True" />
                <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" />
            </Columns>
        </asp:GridView>
        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" OnSelecting="ObjectDataSource1_Selecting" SelectMethod="GetAllRules" OnUpdating="ObjectDataSource1_OnUpdating" TypeName="UnicorntoSCSConverter.SCSRuleEditor">
            
        </asp:ObjectDataSource>
<%--        <div>
                <asp:GridView ID="grdVwRuleEditor" runat="server" AutoGenerateColumns="False" OnRowCancelingEdit="grdVwRuleEditor_RowCancelingEdit" OnRowEditing="grdVwRuleEditor_RowEditing" DataKeyNames="ID" OnRowUpdating="grdVwRuleEditor_RowUpdating" OnRowUpdated="grdVwRuleEditor_RowUpdated" OnRowDeleting="grdVwRuleEditor_RowDeleting" ShowFooter="True" OnRowCommand="grdVwRuleEditor_RowCommand">
                    <Columns>

                        <asp:TemplateField HeaderText="Order">

                            <ItemTemplate>

                                <asp:Label ID="lblOrder" runat="server"></asp:Label>

                            </ItemTemplate>

                            <FooterTemplate>

                                <asp:Button ID="Btn_Add" runat="server" CommandName="AddRule" Text="Add" />

                            </FooterTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Scope">

                            <ItemTemplate>

                                <asp:Label   DataField="ProfileId" ID="lblScope" Text='<%# Eval("Scope") %>' runat="server"></asp:Label>

                            </ItemTemplate>

                            <EditItemTemplate>

                                <asp:DropDownList ID="ddlScope" Text='<%# Eval("Scope") %>' runat="server"></asp:DropDownList>

                            </EditItemTemplate>

                            <FooterTemplate>

                                <asp:DropDownList ID="ddlScope" runat="server">
                                    <asp:ListItem value="ItemAndDescendants">ItemAndDescendants</asp:ListItem>
                                    <asp:ListItem value="ItemAndChildren">ItemAndChildren</asp:ListItem>
                                    <asp:ListItem value="DescendantsOnly">DescendantsOnly</asp:ListItem>
                                    <asp:ListItem value="SingleItem">SingleItem</asp:ListItem>
                                    <asp:ListItem value="ignored">Ignored</asp:ListItem>
                                </asp:DropDownList>

                            </FooterTemplate>

                        </asp:TemplateField>

                        <asp:TemplateField HeaderText="Allow Push Operation">

                            <ItemTemplate>

                                <asp:Label ID="lblPushOperation" Text='<%# Eval("PushOperation") %>' runat="server"></asp:Label>

                            </ItemTemplate>

                            <EditItemTemplate>

                                <asp:DropDownList ID="ddlPushOperation" Text='<%# Eval("PushOperation") %>' runat="server"></asp:DropDownList>

                            </EditItemTemplate>

                            <FooterTemplate>

                                <asp:DropDownList ID="ddlPushOperation" runat="server"></asp:DropDownList>

                            </FooterTemplate>

                        </asp:TemplateField>
                        <asp:TemplateField HeaderText="In Path">

                            <ItemTemplate>

                                <asp:Label ID="lblPath" Text='<%# Eval("Path") %>' runat="server"></asp:Label>

                            </ItemTemplate>

                            <EditItemTemplate>

                                <asp:TextBox ID="txtPath" Text='<%# Eval("Path") %>' runat="server"></asp:TextBox>

                            </EditItemTemplate>

                            <FooterTemplate>

                                <asp:TextBox ID="txtPath" runat="server"></asp:TextBox>

                            </FooterTemplate>

                        </asp:TemplateField>

                        <asp:CommandField HeaderText="Edit" ShowEditButton="True" />

                        <asp:CommandField HeaderText="Delete" ShowDeleteButton="True" />

                    </Columns>

                </asp:GridView>
               
                <br />
            </div>--%>
        <%--<asp:ObjectDataSource DeleteMethod="DeletRule" InsertMethod="AddRule" ID="objDSRuleList" runat="server" SelectMethod="GetAllRules" UpdateMethod="UpdateRule" TypeName="UnicorntoSCSConverter.RuleList">

        </asp:ObjectDataSource>--%>
    </form>
</body>
</html>
