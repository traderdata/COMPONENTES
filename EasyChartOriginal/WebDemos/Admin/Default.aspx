<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin._Default" %>
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
			<br>
			
			
			<br>
			</TABLE>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
