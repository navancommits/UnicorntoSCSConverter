<%@ Page Language="C#" AutoEventWireup="true" ValidateRequest="false" CodeBehind="UnicorntoSCSConverter.aspx.cs" Inherits="UnicorntoSCSConverter.UnicorntoSCSConverter" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <link rel="stylesheet" href="https://maxcdn.bootstrapcdn.com/bootstrap/4.0.0/css/bootstrap.min.css" integrity="sha384-Gn5384xqQ1aoWXA+058RXPxPg6fy4IWvTNh0E263XmFcJlSAwiGgFAW/dAiS6JXm" crossorigin="anonymous">
</head>
<body>
    <form runat="server">
        <div class="container">
                <div class="row">
                    <p><strong>Paste Unicorn Serialization Code here: </strong> </p>
                </div>
                <div class="row">
                    <asp:TextBox ID="txtConfig" runat="server" TextMode="MultiLine" Height="435px" Width="1136px"></asp:TextBox>
                </div>
                <div class="row">
                    <asp:Button ID="btnConvert" runat="server" Text="Convert" OnClick="btnConvert_Click" />
                </div>
                <div class="row">
                    <strong>Output SCS JSON:</strong>
                </div>
                 <div class="row">
                    <asp:TextBox ID="txtJson" runat="server" TextMode="MultiLine" Height="435px" Width="1136px"></asp:TextBox>
                </div>
        </div>
    </form>
</body>
</html>
