<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="ManageSymbols.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin.ManageSymbols" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Manage symbols</title>
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
						<div class="Title">Delete Symbols</div>
						Enter stock symbols below, one symbol per line.
						<br>
						<asp:TextBox id="tbRemain" runat="server" TextMode="MultiLine" Width="600px" Height="100px"></asp:TextBox>
						<br>
						<asp:Button id="btnRemain" runat="server" Text="Remain stocks above and delete others"></asp:Button>
						<asp:Button id="btnDelete" runat="server" Text="Delete stocks above"></asp:Button>
						<hr>
						<div class="Title">Import Symbols</div>
						Format : Symbol;Stock Name;Exchange:<br>
						<font color="blue">Samples:</font><br>
						ABCW;Anchor BanCorp Wisconsin Inc.;NASDAQ<br>
						ABM;Abm Industries Inc;NYSE<br>
						<asp:TextBox id="tbAddSymbol" runat="server" Height="100px" Width="600px" TextMode="MultiLine"></asp:TextBox>
						<asp:Button id="btnAdd" runat="server" Text="Import"></asp:Button>
						<asp:label id="lAddMsg" runat="server" ForeColor="Red"></asp:label>
						<br>
						<hr>
						<div class="Title">Export Symbols</div>
						<asp:TextBox id="tbExport" runat="server" Height="100px" Width="600px" TextMode="MultiLine"></asp:TextBox>
						<asp:Button id="btnExport" runat="server" Text="Export"></asp:Button>
						<hr>
						<div class="Title">Others</div>
						<asp:Button id="btnUpdateRealtime" runat="server" Text="Update stock list"></asp:Button>
						<asp:Button id="btnClearCache" runat="server" Text="ClearCache"></asp:Button>
						<asp:Button id="btnDeleteAll" runat="server" Text="Delete All Historical Data"></asp:Button>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
