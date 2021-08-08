<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScsRuleEditor.aspx.cs" Inherits="UnicorntoSCSConverter.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
   
</head>
<body>
    <form id="form1" runat="server">
            <div class="jumbotron">
                <h1 align="center">SCS Module Rule Viewer and Editor</h1>   
                <h6 align="center" style="color: #808080; font-style: italic" >Poor man's Module Explorer</h6>   
            </div>
            <div class="row" style="margin-bottom: 10px">
                    <div  class="col-md-6" align="right"><asp:FileUpload ID="ModuleFileUpload" runat="server" /></div>
                    <div  class="col-md-6"><asp:Button ID="btnModuleLoad" runat="server"
                                Text="Load Module JSON" OnClick="btnModuleLoad_Click" BackColor="#99CCFF" /></div>

            </div>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                        <div class="col-md-6">
                            <div align="center" style="background:lightgray; "><asp:TextBox ID="txtModuleJson" runat="server" TextMode="MultiLine"  Height="400px" Width="750px"></asp:TextBox><p align='right'>
                                <asp:Button ID="btnValidateJson" Text="Validate Json" runat="server"
                                                                                                                                                                                                           OnClick="btnValidateJson_Click" BackColor="Red" /></p></div>
                            <div  align="center" style="background:lightgoldenrodyellow;"><asp:TextBox ID="txtIfElseBlock" runat="server" TextMode="MultiLine"  Height="400px" Width="750px"></asp:TextBox></div>
                        </div>
                        <div class="col-md-6" style="background:lightcyan;">
                            <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                            <asp:Label ID="lblIncludeList" runat="server" Text="Select Include Name: "></asp:Label><asp:DropDownList id="ddlIncludeList" runat="server"/>
	                        <asp:gridview ID="gvRules" runat="server" autogeneratecolumns="False" datakeynames="RuleId"
                            onrowcancelingedit="gvRules_RowCancelingEdit" onrowediting="gvRules_RowEditing"
                            onrowdeleting="gvRules_RowDeleting" showfooter="True" style="margin-top: 20px;"
                            onrowupdating="gvRules_RowUpdating" onrowcommand="gvRules_RowCommand">
		                            <columns>
			                            <asp:templatefield headertext="Action">
				                            <itemtemplate>
					                            <asp:linkbutton id="btnEdit" runat="server" causesvalidation="false" commandname="Edit" text="Edit" />
					                            <asp:linkbutton id="btnDelete" runat="server" causesvalidation="false" commandname="Delete" text="Delete" />
				                            </itemtemplate>
				                            <edititemtemplate>
					                            <asp:linkbutton id="btnUpdate" runat="server" commandname="Update" text="Update" />
					                            <asp:linkbutton id="btnCancel" runat="server" causesvalidation="false" commandname="Cancel" text="Cancel" />
				                            </edititemtemplate>
				                            <footertemplate>
					                            <asp:linkbutton id="btnInsert" runat="server" commandname="Insert" text="Insert" />
				                            </footertemplate>
			                            </asp:templatefield>
			                            <asp:templatefield  headertext="RuleId" Visible="False">
				                            <itemtemplate>
					                            <%# Eval("RuleId") %>
				                            </itemtemplate>
				                            <edititemtemplate>
					                            <%# Eval("RuleId") %>
				                            </edititemtemplate>
			                            </asp:templatefield>
			                            <asp:templatefield headertext="Path">
				                            <itemtemplate>
					                            <%# Eval("Path") %>
				                            </itemtemplate>
				                            <edititemtemplate>
					                            <asp:textbox id="txtPath" runat="server" text='<%# Bind("Path") %>' />
				                            </edititemtemplate>
				                            <footertemplate>
					                            <asp:textbox id="txtPath" runat="server" text='<%# Bind("Path") %>' />
				                            </footertemplate>
			                            </asp:templatefield>
			                            <asp:templatefield headertext="Scope">
				                            <itemtemplate>
					                            <%# Eval("Scope") %>
				                            </itemtemplate>
				                            <edititemtemplate>
                                                <asp:DropDownList ID="ddlScope" runat="server" DataValueField='<%# Bind("Scope") %>'>
                                                    <asp:ListItem value="ItemAndDescendants">ItemAndDescendants</asp:ListItem>
                                                    <asp:ListItem value="ItemAndChildren">ItemAndChildren</asp:ListItem>
                                                    <asp:ListItem value="DescendantsOnly">DescendantsOnly</asp:ListItem>
                                                    <asp:ListItem value="SingleItem">SingleItem</asp:ListItem>
                                                    <asp:ListItem value="ignored">Ignored</asp:ListItem>
                                                </asp:DropDownList>
				                            </edititemtemplate>
				                            <footertemplate>
                                                <asp:DropDownList ID="ddlScope" runat="server" DataValueField='<%# Bind("Scope") %>'>
                                                    <asp:ListItem value="ItemAndDescendants">ItemAndDescendants</asp:ListItem>
                                                    <asp:ListItem value="ItemAndChildren">ItemAndChildren</asp:ListItem>
                                                    <asp:ListItem value="DescendantsOnly">DescendantsOnly</asp:ListItem>
                                                    <asp:ListItem value="SingleItem">SingleItem</asp:ListItem>
                                                    <asp:ListItem value="ignored">Ignored</asp:ListItem>
                                                </asp:DropDownList>
				                            </footertemplate>
			                            </asp:templatefield>
                                        <asp:templatefield headertext="AllowedOperation">
				                            <itemtemplate>
					                            <%# Eval("AllowedOperation") %>
				                            </itemtemplate>
				                            <edititemtemplate>
                                                <asp:DropDownList ID="ddlAllowedOperation" runat="server" DataValueField='<%# Bind("AllowedOperation") %>'>
                                                    <asp:ListItem value="CreateUpdateAndDelete">CreateUpdateAndDelete</asp:ListItem>
                                                    <asp:ListItem value="CreateAndUpdate">CreateAndUpdate</asp:ListItem>
                                                    <asp:ListItem value="CreateOnly">CreateOnly</asp:ListItem>
                                                </asp:DropDownList>
				                            </edititemtemplate>
				                            <footertemplate>
                                                <asp:DropDownList ID="ddlAllowedOperation" runat="server" DataValueField='<%# Bind("AllowedOperation") %>'>
                                                    <asp:ListItem value="CreateUpdateAndDelete">CreateUpdateAndDelete</asp:ListItem>
                                                    <asp:ListItem value="CreateAndUpdate">CreateAndUpdate</asp:ListItem>
                                                    <asp:ListItem value="CreateOnly">CreateOnly</asp:ListItem>
                                                </asp:DropDownList>
				                            </footertemplate>
			                            </asp:templatefield>
		                        </columns>
	                        </asp:gridview>
                            <asp:Button ID="btnEditRules" runat="server"  style="margin-top: 10px;"  BackColor="#99CCFF" align="right" Text="Edit Rules" OnClick="btnEditRules_Click" />
                        </div>
                    </div>
                </div>
            </div>     
           
            
            <%--<div class="panel panel-default">
                <div class="panel-body"><h2 align="center">SCS Rule Editor</h2></div>
                <div class="panel-body">
                    
                    <div class="row">
                        <div class="col-md-6">
                            <div style="background:lightgray; height:100px">Column 1</div>
                            <div style="background:lightgoldenrodyellow; height:200px">Column 2</div>
                        </div>
                        <div class="col-md-6" style="background:lightcyan; height:300px">Column 3</div>
                    </div>
                </div>
            </div>--%>
        
    </form>
</body>
</html>
