<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ScsModuleRuleEditor.aspx.cs" Inherits="UnicorntoSCSConverter.WebForm2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
   <style>
       body {
           font-family: "Helvetica", Arial, serif;
           background: #F0F0F0;
       }
       .loader {
           height: 80px;
           width: 98%;
           background: #d0cfcf;
           margin: 0 1% 0.5% 1%;
           padding-top: 15px;
           padding-left: 5px;
           padding-right: 5px;
       }

      
       .header {
           height: 5%;
           width: 98%;
           margin: 0.5% 1% 0.5% 1%;
           padding-top: 1%;
           padding-bottom: 1%;
           text-align: center;
           color: #313335;
           font-size: x-large;
       }

       #headerbg {
           background: url('https://localhost:44306/leadrerbg.png') no-repeat;
       }
       
       .panel {
           margin: 1% 1% 0% 0%;
       }

       .row {
           margin-left: 1%;
       }

       #gViewContainer {
           background: #D0CFCF;
           width: 96%;
       }

       #dvModuleJson {
           height: 730px;
       }

       .btn {
           background: #0077b5;
       }
   </style>
    <link rel="shortcut icon" type="image/x-icon" href="favicon.png" />
</head>
<body>
    <form id="form1" runat="server">
            <div id="headerbg" class ="header">
                <img align="left" src="sclogo.png" alt="Sitecore Rocks"  />
                <h1 align="center">SCS Module Rule Viewer and Editor</h1>   
                <h6 align="center" style="color: #808080; font-style: italic" >Poor man's Module Explorer</h6>   
            </div>
            <div class="loader">
                <table  width="100%"><tr><td width="50%" align="left"><asp:FileUpload ID="ModuleFileUpload" runat="server"  /></td>
                    <td  width="50%" style="margin-right: 10px;" align="right"><asp:Button ID="btnModuleLoad" runat="server"
                                Text="1. Load Module JSON" OnClick="btnModuleLoad_Click"   class="btn btn-primary" /></td></tr></table>

            </div>
            <div class="panel panel-default">
                <div class="panel-body">
                    <div class="row">
                                                <div id="gViewContainer"  class="col-lg-6 col-md-12  col-xs-12">
                            <div>
                                <asp:Label ID="lblMessage" runat="server" EnableViewState="false"></asp:Label>
                                <asp:Label ID="lblIncludeList" runat="server" Text="Select Include Name: "></asp:Label><asp:DropDownList id="ddlIncludeList" runat="server"/>
                                <div class="table-responsive">  
	                            <asp:gridview ID="gvRules" runat="server" autogeneratecolumns="False" datakeynames="RuleId"  Width="98%"
                                onrowcancelingedit="gvRules_RowCancelingEdit" onrowediting="gvRules_RowEditing"
                                onrowdeleting="gvRules_RowDeleting" showfooter="True" style="margin-top: 20px;"
                                onrowupdating="gvRules_RowUpdating" onrowcommand="gvRules_RowCommand" BackColor="White" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px" CellPadding="3" ForeColor="Black" GridLines="Vertical">
		                                <AlternatingRowStyle BackColor="#CCCCCC" />
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
                                            <asp:templatefield headertext="Allowed Operation">
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
	                                    <FooterStyle BackColor="#CCCCCC" />
                                        <HeaderStyle BackColor="#313335" Font-Bold="True" ForeColor="lightgray" />
                                        <PagerStyle BackColor="#999999" ForeColor="Black" HorizontalAlign="Center" />
                                        <SelectedRowStyle BackColor="#000099" Font-Bold="True" ForeColor="White" />
                                        <SortedAscendingCellStyle BackColor="#F1F1F1" />
                                        <SortedAscendingHeaderStyle BackColor="#808080" />
                                        <SortedDescendingCellStyle BackColor="#CAC9C9" />
                                        <SortedDescendingHeaderStyle BackColor="#383838" />
	                            </asp:gridview>
                                    </div>
                                <p align="right"><asp:Button ID="btnEditRules" runat="server"  style="margin-top: 10px; margin-right: 10px;"  class="btn btn-primary" Text="2. Edit Rules" OnClick="btnEditRules_Click" /></p>
                                </div>
                                <div>
                                    <asp:TextBox ID="txtIfElseBlock" runat="server" TextMode="MultiLine"  Height="420px" Width="98%">Coming soon... Rule Explanation here.....</asp:TextBox>
                                </div>
                            </div>

                        <div class="col-lg-6 col-md-12  col-xs-12">
                            <div id="dvModuleJson" align="center" style="background:#D0CFCF; "><asp:TextBox ID="txtModuleJson" runat="server" TextMode="MultiLine"  Height="680px" Width="98%"></asp:TextBox><p align='right'>
                                <asp:Button ID="btnValidateJson" Text="Valid JSON?" runat="server" OnClick="btnValidateJson_Click" class="btn btn-primary"  />
                                <asp:Button ID="btnSaveAs" Text="3. Save JSON" runat="server" OnClick="btnSaveAs_Click" class="btn btn-primary" /></p></div>
                          
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
