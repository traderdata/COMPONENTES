<%@ Page language="c#" Codebehind="PriceList.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.PriceList" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfPriceList" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<asp:Label id="lCount" runat="server" Visible="False"></asp:Label>
			<B>Stock Symbol:</B>
			<asp:textbox id="tbSymbol" runat="server">ABC</asp:textbox>
			<asp:button id=btnOK runat="server" Text="OK" CssClass="ChartButton"></asp:button>
			<asp:datagrid id="dgList" runat="server" AutoGenerateColumns="False" PageSize="30" 
				AllowCustomPaging="True" BorderColor="Tan" BorderWidth="1px" BackColor="LightGoldenrodYellow"
				CellPadding="2" ForeColor="Black">
				<Columns>
					<asp:BoundColumn DataField="Symbol" SortExpression="Symbol" HeaderText="Symbol"></asp:BoundColumn>
					<asp:BoundColumn DataField="Date" SortExpression="Date" HeaderText="Date" DataFormatString="{0:yyyy-MM-dd}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Open" SortExpression="Open desc" HeaderText="Open" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="High" SortExpression="High desc" HeaderText="High" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Low" SortExpression="Low desc" HeaderText="Low" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="CLose" SortExpression="Close desc" HeaderText="CLose" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Volume" SortExpression="Volume desc" HeaderText="Volume" DataFormatString="{0:f2}"></asp:BoundColumn>
					<asp:BoundColumn DataField="Change" SortExpression="Change desc" HeaderText="Change" DataFormatString="{0:p2}"></asp:BoundColumn>
				</Columns>

				<FooterStyle BackColor="Tan"></FooterStyle>
				<SelectedItemStyle ForeColor="GhostWhite" BackColor="DarkSlateBlue"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="PaleGoldenrod"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" BackColor="Tan"></HeaderStyle>
				<PagerStyle HorizontalAlign="Center" ForeColor="DarkSlateBlue" BackColor="PaleGoldenrod" Mode="NumericPages"></PagerStyle>
			</asp:datagrid><br>
			<tl:footer id="Footer" runat="server"></tl:footer>
		</form>
	</body>
</HTML>
