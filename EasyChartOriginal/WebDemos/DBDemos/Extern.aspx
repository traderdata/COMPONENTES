<%@ Page language="c#" Codebehind="Extern.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Extern" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<HTML>
	<HEAD>
		<title>Extern data</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Extern" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<font color="red"><B>This demo shows how to load extern data to the chart!</B></font><br>
			Stock Symbol:<asp:TextBox id="tbSymbol" runat="server">^DJI</asp:TextBox>
			<asp:Button id="btnUpdate" runat="server" Text="Update Chart" CssClass="ChartButton"></asp:Button>
			<hr>
			<table>
				<tr>
					<td>
						Name =
						<asp:TextBox id="tbName1" runat="server">L1</asp:TextBox>
						<br>
						Custom Data 1
						<br>
						<asp:TextBox id="tbLine1" runat="server" TextMode="MultiLine" Width="300px" Height="120px"></asp:TextBox>
					</td>
					<td width="40"></td>
					<td>
						Name =
						<asp:TextBox id="tbName2" runat="server">L2</asp:TextBox>
						<br>
						Custom Data 2
						<br>
						<asp:TextBox id="tbLine2" runat="server" TextMode="MultiLine" Width="300px" Height="120px"></asp:TextBox>
					</td>
				</tr>
			</table>
			<br>
			<asp:Image id="iChart" runat="server"></asp:Image>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
		</TR></TBODY></TABLE></FORM>
	</body>
</HTML>
