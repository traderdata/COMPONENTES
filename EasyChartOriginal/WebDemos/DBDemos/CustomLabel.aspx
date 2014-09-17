<%@ Page language="c#" Codebehind="CustomLabel.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.CustomLabel" culture="en-US" uiCulture="en-US"%>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart</title>
		<LINK href="<%=WebDemos.Config.Path%>m.css" type=text/css rel=stylesheet>
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="wfCustomLabel" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><br>
			<table>
				<tr>
					<td><B>Symbol:</B>
						<asp:TextBox id="tbSymbol" runat="server">CFC</asp:TextBox>
						<b>Skin:</b>
						<asp:dropdownlist id="ddlSkin" runat="server"></asp:dropdownlist></td>
				</tr>
				<tr>
					<td bgColor="#ffffcc"><B>Color Band</B> &nbsp; Start Date:
						<asp:textbox id="tbStartDate" runat="server" Width="120px"></asp:textbox>End 
						Date:
						<asp:textbox id="tbEndDate" runat="server" Width="120px"></asp:textbox><asp:button id="btnOK" runat="server" CssClass="ChartButton" Text="Draw Chart"></asp:button></td>
				</tr>
				<tr>
					<td bgColor="honeydew"><B>Text Label</B> &nbsp; Date:<asp:textbox id="tbDate" runat="server" Width="120px"></asp:textbox>
						Value:<asp:dropdownlist id="ddlDataType" runat="server">
							<asp:ListItem Value="OPEN">OPEN</asp:ListItem>
							<asp:ListItem Value="HIGH">HIGH</asp:ListItem>
							<asp:ListItem Value="LOW">LOW</asp:ListItem>
							<asp:ListItem Value="CLOSE" Selected="True">CLOSE</asp:ListItem>
						</asp:dropdownlist>
						Stick Align:
						<asp:dropdownlist id="ddlAlign" runat="server">
							<asp:ListItem Value="LeftTop">LeftTop</asp:ListItem>
							<asp:ListItem Value="RightTop">RightTop</asp:ListItem>
							<asp:ListItem Value="RightBottom">RightBottom</asp:ListItem>
							<asp:ListItem Value="LeftBottom">LeftBottom</asp:ListItem>
						</asp:dropdownlist><asp:checkbox id="cbAddPrice" runat="server" Text="Add Price" Checked="True"></asp:checkbox><br>
						Text:<asp:textbox id="tbText" runat="server" Width="528px" Height="80px" TextMode="MultiLine">ToB</asp:textbox></td>
				</tr>
			</table>
			<br>
			<asp:HyperLink id="hlChart" runat="server"></asp:HyperLink>
			<asp:Label id="lWidth" runat="server" Visible="False">800</asp:Label>
			<asp:Label id="lHeight" runat="server" Visible="False">600</asp:Label><br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
