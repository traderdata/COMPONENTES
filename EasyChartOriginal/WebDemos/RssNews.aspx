<%@ Page language="c#" Codebehind="RssNews.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.RssNews" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="RssClient" src="~/RssClient.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" > 
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfNews" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>

			<tl:RssClient id="RssClient" runat="server" RssUrl="http://today.reuters.com/rss/businessNews/" Title="Business News By"></tl:RssClient>

			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>

