<%@ Page language="c#" Codebehind="HistoryPreScan.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.HistoryPreScan" Culture="en-US" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Historical Prescan</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfPreScan" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			History Pre-scan result:
			<asp:Label id="lFormat" runat="server" Visible="False">{0:MMM dd,yyyy}</asp:Label>
			<asp:Label id="lSeparator" runat="server" Visible="False"> , </asp:Label>
			<br>
			<asp:Literal id="lPrescan" runat="server"></asp:Literal>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
