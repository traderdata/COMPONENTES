<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="StockList.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.StockList"%>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfStockList" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<asp:Label id="lDefaultSort" runat="server" Visible="False"></asp:Label>
			<asp:label id="lCondition" runat="server" EnableViewState="False"></asp:label><br>
			<asp:label id="lTotal" runat="server"></asp:label><br>
			<asp:datagrid id="dgList" runat="server" AutoGenerateColumns="False" PageSize="30" AllowPaging="True"
				AllowSorting="True" AllowCustomPaging="True" BorderColor="Tan" BorderWidth="1px" BackColor="LightGoldenrodYellow"
				CellPadding="2" ForeColor="Black">
				<FooterStyle BackColor="Tan"></FooterStyle>
				<SelectedItemStyle ForeColor="GhostWhite" BackColor="DarkSlateBlue"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="PaleGoldenrod"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" BackColor="Tan"></HeaderStyle>
				<PagerStyle HorizontalAlign="Center" ForeColor="DarkSlateBlue" BackColor="PaleGoldenrod" Mode="NumericPages"></PagerStyle>
			</asp:datagrid><br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
		<!--			
				<Columns>
					<asp:HyperLinkColumn Text="Chart" DataNavigateUrlField="QuoteCode" DataNavigateUrlFormatString="CustomChart.aspx?Symbol={0}"></asp:HyperLinkColumn>
					<asp:BoundColumn DataField="QuoteCode" SortExpression="a.QuoteCode" HeaderText="Symbol"></asp:BoundColumn>
					<asp:BoundColumn DataField="QuoteName" SortExpression="QuoteName"  HeaderText="Name"></asp:BoundColumn>
					<asp:BoundColumn DataField="Date" SortExpression="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Last" SortExpression="Last desc" HeaderText="Last" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="OpenA" SortExpression="OpenA desc" HeaderText="Open" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="HighA" SortExpression="HighA desc" HeaderText="High" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="LowA" SortExpression="LowA desc" HeaderText="Low" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="CLoseA" SortExpression="CloseA desc" HeaderText="CLose" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="VolumeA" SortExpression="VolumeA desc" HeaderText="Volume" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="ChangeA" SortExpression="ChangeA desc" HeaderText="Change" DataFormatString="{0:p2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Exchange" SortExpression="Exchange" HeaderText="Exchange"></asp:BoundColumn>
				</Columns>
//-->
	</body>
</HTML>
