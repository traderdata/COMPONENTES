<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="Scan.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.Scan" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Scanning</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfScan" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<table>
				<tr>
					<td>
						Exchange :
						<asp:DropDownList id="ddlExchange" runat="server" DataTextField="Text" DataValueField="Value"></asp:DropDownList>
						Formulas:
						<asp:DropDownList id="ddlFormula" runat="server" AutoPostBack="True"></asp:DropDownList>
						<br>
						<table border="1" cellpadding="2" cellspacing="0">
							<tr>
								<td>
									<B>Full Name</B>
								</td>
								<td>
									<B>Script</B>
								</td>
								<td>
									<B>Description</B>
								</td>
							</tr>
							<tr>
								<td>
									<asp:Label id="lFullName" runat="server"></asp:Label></td>
								<td>
									<asp:Label id="lCode" runat="server" CssClass="Script"></asp:Label></td>
								<td>
									<asp:Label id="lDescription" runat="server"></asp:Label>
								</td>
							</tr>
						</table>
						<b>Parameters</b>
						<asp:Label id="lParam" runat="server"></asp:Label>
						<asp:Button id="btnScan" runat="server" Text="Scan"></asp:Button>
					</td>
				</tr>
			</table>
			<br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
