<%@ Register TagPrefix="tl" TagName="SelectFormula" src="~/SelectFormula.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="BackTesting.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.BackTesting" Culture="en-US"%>
<HTML>
	<HEAD>
		<title>Easy financial chart - Back Testing</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="wfBackTesting" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<table width="800">
				<tr>
					<td>Symbol:<asp:textbox id="tbSymbol" runat="server">MSFT</asp:textbox>
						Testing Bars Count:<asp:textbox id="tbBars" runat="server">1000</asp:textbox>
						<br>
						<br>
						<tl:selectformula id="sfBackTesting" runat="server"></tl:selectformula><br>
						<asp:button id="btnTesting" runat="server" Text="Start Testing"></asp:button>
						<br>
						<asp:Label id="lMsg" runat="server" ForeColor="Red" EnableViewState="False"></asp:Label>
						Trade Log:
						<asp:datagrid id="dgResult" runat="server" CellPadding="2" BackColor="White" BorderWidth="1px"
							BorderStyle="None" BorderColor="Black" AutoGenerateColumns="False" Width="576px" ShowFooter="True"
							EnableViewState="False">
							<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
							<ItemStyle ForeColor="Black" BackColor="#C0FFC0"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
							<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
							<Columns>
								<asp:BoundColumn DataField="EntryDate" HeaderText="Entry Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
								<asp:BoundColumn DataField="EntryPrice" HeaderText="Entry Price" DataFormatString="{0:c2}"></asp:BoundColumn>
								<asp:BoundColumn DataField="ExitDate" HeaderText="Exit Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
								<asp:BoundColumn DataField="ExitPrice" HeaderText="Exit Price" DataFormatString="{0:c2}"></asp:BoundColumn>
								<asp:BoundColumn DataField="ProfitPercent" HeaderText="Profit%" DataFormatString="{0:p2}"></asp:BoundColumn>
								<asp:BoundColumn DataField="BarsHold" HeaderText="Bars Hold" DataFormatString="{0:f2}"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
