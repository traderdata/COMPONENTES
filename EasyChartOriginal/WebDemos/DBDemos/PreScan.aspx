<%@ Page language="c#" Codebehind="PreScan.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.PreScan" EnableViewState="false" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Scanning</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfPreScan" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<table width="700" id="tblTechnical" runat="server" visible="false" cellpadding="0" cellspacing="0">
				<tr>
					<td>
						<B><FONT face="Arial" color="green">Technical Screens</FONT></B>
					</td>
					<td align="right">
						<asp:Label id="lDate1" runat="server" Font-Bold="True"></asp:Label>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<asp:DataGrid id="dgPreScan1" runat="server" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px"
							BackColor="White" CellPadding="2" Width="100%" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
							<ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
							<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
			<table width="700" id="tblCandle" runat="server" visible="false" cellpadding="0" cellspacing="0">
				<tr>
					<td>
						<B><FONT face="Arial" color="green">Candlestick Screens</FONT></B>
					</td>
					<td align="right">
						<asp:Label id="lDate2" runat="server" Font-Bold="True"></asp:Label>
					</td>
				</tr>
				<tr>
					<td colspan="2">
						<asp:DataGrid id="dgPreScan2" runat="server" BorderColor="#E7E7FF" BorderStyle="None" BorderWidth="1px"
							BackColor="White" CellPadding="2" Width="100%" AutoGenerateColumns="False">
							<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
							<ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
							<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
						</asp:DataGrid>
					</td>
				</tr>
			</table>
			<br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
