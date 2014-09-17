<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="TestServer.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin.TestServer" %>
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Demos</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<tl:Admheader id="Admheader" runat="server"></tl:Admheader>
			<table cellSpacing="1" cellPadding="4" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td>
						Culture:
						<asp:Label id="lCulture" runat="server"></asp:Label><BR>
						UTC Time:
						<asp:Label id="lUTCNow" runat="server"></asp:Label><br>
						Time Zone:
						<asp:Label id="lTimeZone" runat="server"></asp:Label>
						<hr>
						Date Format:
						<asp:TextBox id="tbDateFormat" runat="server"></asp:TextBox><br>
						Date:
						<asp:TextBox id="tbDate" runat="server"></asp:TextBox>
						<asp:Button id="btnTest" runat="server" Text="Test"></asp:Button><br>
						<asp:Label id="lDateResult" runat="server"></asp:Label>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer>
		</form>
	</body>
</HTML>
