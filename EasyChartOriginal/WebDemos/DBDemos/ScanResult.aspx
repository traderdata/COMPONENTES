<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="ScanResult.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.ScanResult" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Scanning</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0" marginheight="0">
		<form id="wfScan" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header>
			<tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<asp:datagrid id="dgScanResult" runat="server" AutoGenerateColumns="False" AllowCustomPaging="True"
				ForeColor="Black" PageSize="30" CellPadding="2" BackColor="White" BorderWidth="1px" BorderColor="DimGray"
				AllowPaging="True">
				<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#CE5D5A"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="White"></AlternatingItemStyle>
				<ItemStyle BackColor="#F7F7DE"></ItemStyle>
				<HeaderStyle Font-Bold="True" ForeColor="Black" BackColor="#CCCC66"></HeaderStyle>
				<FooterStyle BackColor="#CCCC99"></FooterStyle>
				<Columns>
					<asp:HyperLinkColumn DataNavigateUrlField="ConditionId" DataNavigateUrlFormatString="StockList.aspx?ConditionId={0}"
						DataTextField="Condition" HeaderText="Formula Name"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="Total" HeaderText="Total Stocks"></asp:BoundColumn>
					<asp:BoundColumn DataField="ResultCount" HeaderText="Result Count"></asp:BoundColumn>
					<asp:BoundColumn DataField="StartTime" HeaderText="StartTime"></asp:BoundColumn>
					<asp:BoundColumn DataField="EndTime" HeaderText="EndTime"></asp:BoundColumn>
					<asp:BoundColumn DataField="ScanTime" HeaderText="ScanTime"></asp:BoundColumn>
					<asp:BoundColumn DataField="Exchange" HeaderText="Exchange"></asp:BoundColumn>
				</Columns>
				<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
			</asp:datagrid>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
