<%@ Page language="c#" Codebehind="IntradayGallery.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.IntradayGallery" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Intraday gallery</title>
		<link href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0" >
		<form id="wfIntradayGallery" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>
			<asp:Label id="lNavigate" runat="server" Visible="False">Intraday.aspx?(Symbol)</asp:Label>
			<br>
			<table cellpadding="0" cellspacing="0">
				<tr>
					<td id="tdGallery" runat="server">
					</td>
				</tr>
			</table>
			<br>
			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>
