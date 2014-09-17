<%@ Page language="c#" Codebehind="FormulaHelp.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.FormulaHelp" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Formula Help</title>
		<LINK href="m.css" type="text/css" rel="stylesheet">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Explore" method="post" runat="server">
			<tl:header id="Header" runat="server" EnableViewState="False"></tl:header><tl:demoheader id="DemoHeader" runat="server" EnableViewState="False"></tl:demoheader>
			<br>
			<table runat="server" id="tblMsg" cellpadding="0" cellspacing="0">
				<tr>
					<td>
						1.Below table shows all basic method supported by formula script language.
						<BR>
						2.Show line name before ":" . For example DIFF:C-O , formula line DIFF will 
						show in the chart.
						<BR>
						3.Don't show line name before ":=". For example A:=(C+O)/2, formula line A will 
						not show in the chart.It's a intermedial variant.
						<BR>
						4.Formula line seperate by ";".
						<br>
						5.Supported operators : + - * / &gt; &lt; &gt;= &lt;= = = ! = &amp; | ^ % ! ~ 
						++ -- 6.$AAA equals to ORGDATA(AAA) 7."MACD[DIFF]#WEEK2" equals to 
						FML('MACD[DIFF]#WEEK2')
						<asp:Panel Runat="server" id="Panel1">AAA</asp:Panel>
						<BR>
						<BR>
						100+ basic method can be used in formula script language.
					</td>
				</tr>
			</table>
			<table runat="server" id="tblAllMethod" cellpadding="0" cellspacing="0" visible="false">
				<tr>
					<td>
						<a href="FormulaHelp.aspx">All basic methods</a><br>
						<br>
					</td>
				</tr>
			</table>
			<asp:DataGrid id="dgMethods" runat="server" BorderColor="#FF8000" BorderWidth="1px" BackColor="LightGoldenrodYellow"
				CellPadding="2" ForeColor="Black" EnableViewState="False">
				<FooterStyle BackColor="Tan"></FooterStyle>
				<SelectedItemStyle ForeColor="GhostWhite" BackColor="DarkSlateBlue"></SelectedItemStyle>
				<AlternatingItemStyle BackColor="PaleGoldenrod"></AlternatingItemStyle>
				<HeaderStyle Font-Bold="True" BackColor="GreenYellow"></HeaderStyle>
				<PagerStyle HorizontalAlign="Center" ForeColor="DarkSlateBlue" BackColor="PaleGoldenrod"></PagerStyle>
			</asp:DataGrid>
			<tl:footer id="Footer" runat="server" EnableViewState="False"></tl:footer></form>
	</body>
</HTML>
