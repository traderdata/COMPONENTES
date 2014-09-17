<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="Intraday.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.Intraday" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Intraday chart</title>
		<link href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfIntraday" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>
			<br>
			<asp:DropDownList id="ddlSymbols" runat="server"></asp:DropDownList>
			<asp:DropDownList id="ddlCycle" runat="server">
				<asp:ListItem Value="Second30">30 seconds</asp:ListItem>
				<asp:ListItem Value="Minute1">1 minute</asp:ListItem>
				<asp:ListItem Value="Minute2" Selected="True">2 minutes</asp:ListItem>
				<asp:ListItem Value="Minute5">5 minutes</asp:ListItem>
				<asp:ListItem Value="Minute15">15 minutes</asp:ListItem>
				<asp:ListItem Value="Minute30">30 minutes</asp:ListItem>
				<asp:ListItem Value="Hour1">1 hour</asp:ListItem>
			</asp:DropDownList>
			<asp:DropDownList id="ddlSkin" runat="server"></asp:DropDownList>
			<asp:DropDownList id="ddlTotal" runat="server">
				<asp:ListItem Value="1">1 Day</asp:ListItem>
				<asp:ListItem Value="2">2 Days</asp:ListItem>
				<asp:ListItem Value="3">3 Days</asp:ListItem>
				<asp:ListItem Value="4">4 Days</asp:ListItem>
				<asp:ListItem Value="5">5 Days</asp:ListItem>
				<asp:ListItem Value="6">6 Days</asp:ListItem>
				<asp:ListItem Value="7">7 Days</asp:ListItem>
				<asp:ListItem Value="3m">3 Months</asp:ListItem>
				<asp:ListItem Value="6m">6 Months</asp:ListItem>
				<asp:ListItem Value="1y">1 Year</asp:ListItem>
				<asp:ListItem Value="2y">2 Year</asp:ListItem>
				<asp:ListItem Value="3y">5 Year</asp:ListItem>
			</asp:DropDownList>
			<asp:CheckBox id="cbFixedTime" runat="server" Text="FixedTime" Checked="True"></asp:CheckBox>
			<asp:Button id="btnShowChart" runat="server" Text="Show Chart"></asp:Button>
			<br>
			<asp:Image id="imgChart" runat="server"></asp:Image>
			<br>
			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>
