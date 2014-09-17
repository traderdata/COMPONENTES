<%@ Page language="c#" Codebehind="Explore.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Explore" EnableViewState="false" %>
<%@ Register TagPrefix="iewc" Namespace="Microsoft.Web.UI.WebControls" Assembly="Microsoft.Web.UI.WebControls" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Formula Explore</title>
		<LINK href="m.css" type="text/css" rel="stylesheet">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Explore" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<b>This demo shows that the Formula script was managed as xml tree.<br>
				Click on Formula script (BBI,CYS etc.) on the tree to show detail </b>
			<table>
				<tr>
					<td vAlign="top" width="200">
						<asp:DropDownList id="ddlFormulaFile" runat="server" AutoPostBack="True">
							<asp:ListItem Value="basic.fml">basic.fml</asp:ListItem>
							<asp:ListItem Value="Scan.fml">Scan.fml</asp:ListItem>
							<asp:ListItem Value="Test.fml">Test.fml</asp:ListItem>
						</asp:DropDownList>
						<br>
						<iewc:treeview id="FormulaTree" runat="server" CssClass="Tree"></iewc:treeview>
					</td>
					<td vAlign="top" runat="server" id="tdChart">
						<B>Enter Sympol:</B>
						<asp:TextBox id="tbCode" runat="server">MSFT</asp:TextBox>
						<br>
						<b>Formula Parameter:</b><br>
						<asp:Literal id="lParam" runat="server"></asp:Literal>
						<asp:Button id="btnOK" runat="server" Text="Get Chart"></asp:Button>
						<br>
						<br>
						<asp:Literal id="lChart" runat="server"></asp:Literal><br>
						<b>Formula Name:</b><asp:Literal id="lName" runat="server"></asp:Literal><br>
						<b>Formula Full Name:</b><asp:Literal id="lFullName" runat="server"></asp:Literal><br>
						<b>Formula Script:</b> <font color="blue">
							<asp:Literal id="lCode" runat="server"></asp:Literal></font> <b><font color="red">
								<asp:Literal id="lMainView" runat="server" Text="MainView"></asp:Literal></font></b><br>
						<br>
						<b>Formula Description:</b><asp:Literal id="lDescription" runat="server"></asp:Literal><BR>
					</td>
				</tr>
			</table>
			<br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
