<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="PFW.CSIST203.Project4._Default" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style type="text/css">
        .auto-style1 {
            height: 35px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <table align="center" border="1">
            <tr>
                <td>&nbsp;</td>
                <td align="center">Project #4</td>
                <td>&nbsp;</td>
            </tr>
            <tr>
                <td align="right" class="auto-style1">
                    <asp:Button ID="btnPrevious" runat="server" Text="Previous" CausesValidation="False" OnClick="btnPrevious_Click" />
                    </td>
                <td align="center" class="auto-style1"><asp:Label ID="lblID" runat="server" Text="Label"></asp:Label>
                </td>
                <td align="left" class="auto-style1"><asp:Button ID="btnNext" runat="server" Text="Next" CausesValidation="False" OnClick="btnNext_Click" />
                </td>
            </tr>
            <tr>
                <td align="left">First Name:</td>
                <td colspan="2"><asp:TextBox ID="txtFirstName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">Last Name:</td>
                <td colspan="2"><asp:TextBox ID="txtLastName" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">E-Mail Address:</td>
                <td colspan="2"><asp:TextBox ID="txtEmailAddress" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">Business Phone:</td>
                <td colspan="2"><asp:TextBox ID="txtBusinessPhone" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">Company:</td>
                <td colspan="2"><asp:TextBox ID="txtCompany" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="left">Job Title:</td>
                <td colspan="2"><asp:TextBox ID="txtJobTitle" runat="server"></asp:TextBox>
                </td>
            </tr>
            <tr>
                <td align="right">
                    <asp:Button ID="btnSave" runat="server" Text="Save" OnClick="btnSave_Click" />
                    </td>
                <td align="center">
                    <asp:Button ID="btnReset" runat="server" Text="Reset" CausesValidation="False" OnClick="btnReset_Click" />
                    </td>
                <td align="left"><asp:Button ID="btnNewEntry" runat="server" Text="New Entry" CausesValidation="False" OnClick="btnNewEntry_Click" />
                </td>
            </tr>
        </table>
    </form>
</body>
</html>
