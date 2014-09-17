<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="TestInternet.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.TestInternet" %>
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Test Internet</title>
		<LINK href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header>
			<tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<table cellSpacing="1" cellPadding="4" width="680" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td>
						URL:<asp:TextBox id="tbURL" runat="server" Width="496px" EnableViewState="False">http://finance.easychart.net</asp:TextBox>
						<asp:Button id="btnFetch" runat="server" Text="Fetch" EnableViewState="False"></asp:Button>
						<asp:Button id="btnTestRead" runat="server" Text="Read"></asp:Button>
						<br>
						<asp:TextBox id="tbResult" runat="server" TextMode="MultiLine" Width="624px" Height="448px" EnableViewState="False"></asp:TextBox><FONT face="ËÎÌו"></FONT>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
