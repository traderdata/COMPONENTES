<%@ Page language="c#" Codebehind="DBWebChart.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBWebChart" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<LINK href="<%=WebDemos.Config.Path%>m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfWebChart" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<asp:CheckBox id="cbRealTime" runat="server" Text="Download real-time quote from yahoo" AutoPostBack="True"></asp:CheckBox>
			<table cellSpacing="1" cellPadding="4" width="740" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td><B>Stock Symbol:</B><asp:textbox id="tbCode" runat="server">^DJI</asp:textbox>
						<asp:button id="btnOK" runat="server" Text="Draw Chart" CssClass="ChartButton"></asp:button><B>Skin:</B>
						<asp:dropdownlist id="ddlSkin" runat="server" AutoPostBack="True" CssClass="DropArrow"></asp:dropdownlist><B>Size:</B>
						<asp:dropdownlist id="ddlSize" runat="server" AutoPostBack="True" CssClass="DropArrow">
							<asp:ListItem Value="520">520</asp:ListItem>
							<asp:ListItem Value="620">620</asp:ListItem>
							<asp:ListItem Value="700">700</asp:ListItem>
							<asp:ListItem Value="780" Selected="True">780</asp:ListItem>
							<asp:ListItem Value="900">900</asp:ListItem>
							<asp:ListItem Value="1000">1000</asp:ListItem>
							<asp:ListItem Value="1260">1260</asp:ListItem>
							<asp:ListItem Value="1580">1580</asp:ListItem>
						</asp:dropdownlist>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%">
							<tr>
								<td><B>Range:</B>
									<asp:radiobuttonlist id="rblRange" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
										<asp:ListItem Value="3m">3m</asp:ListItem>
										<asp:ListItem Value="6m" Selected="True">6m</asp:ListItem>
										<asp:ListItem Value="1y">1y</asp:ListItem>
										<asp:ListItem Value="2y">2y</asp:ListItem>
										<asp:ListItem Value="5y">5y</asp:ListItem>
										<asp:ListItem Value="max">max</asp:ListItem>
									</asp:radiobuttonlist></td>
								<td><B>Type:</B><asp:radiobuttonlist id="rblType" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
										<asp:ListItem Value="0">HLC</asp:ListItem>
										<asp:ListItem Value="1">OHLC</asp:ListItem>
										<asp:ListItem Value="2">Line</asp:ListItem>
										<asp:ListItem Value="3" Selected="True">Candle</asp:ListItem>
									</asp:radiobuttonlist>
								</td>
								<td><B>Scale:</B>
									<asp:radiobuttonlist id="rblScale" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
										<asp:ListItem Value="0" Selected="True">Linear</asp:ListItem>
										<asp:ListItem Value="1">Log</asp:ListItem>
									</asp:radiobuttonlist></td>
							</tr>
						</table>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%">
							<tr>
								<td><B>Moving Avg:</B><asp:checkboxlist id="cblMA" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
										<asp:ListItem Value="9">9</asp:ListItem>
										<asp:ListItem Value="13" Selected="True">13</asp:ListItem>
										<asp:ListItem Value="25">25</asp:ListItem>
										<asp:ListItem Value="50" Selected="True">50</asp:ListItem>
										<asp:ListItem Value="150">150</asp:ListItem>
										<asp:ListItem Value="200" Selected="True">200</asp:ListItem>
									</asp:checkboxlist>
								</td>
								<td><B>EMA:</B><asp:checkboxlist id="cblEMA" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
										<asp:ListItem Value="9">9</asp:ListItem>
										<asp:ListItem Value="13">13</asp:ListItem>
										<asp:ListItem Value="25">25</asp:ListItem>
										<asp:ListItem Value="50">50</asp:ListItem>
										<asp:ListItem Value="150">150</asp:ListItem>
										<asp:ListItem Value="200">200</asp:ListItem>
									</asp:checkboxlist>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td><B>Indicators:</B>
						<asp:checkboxlist id="cblIndicators" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
							RepeatLayout="Flow">
							<asp:ListItem Value="VOLMA">VOLMA</asp:ListItem>
							<asp:ListItem Value="MACD" Selected="True">MACD</asp:ListItem>
							<asp:ListItem Value="FastSTO">Fast Stoch</asp:ListItem>
							<asp:ListItem Value="SlowSTO">Slow Stoch</asp:ListItem>
							<asp:ListItem Value="ROC">ROC</asp:ListItem>
							<asp:ListItem Value="TRIX">TRIX</asp:ListItem>
							<asp:ListItem Value="AreaRSI" Selected="True">RSI</asp:ListItem>
							<asp:ListItem Value="ATR" Selected="True">ATR</asp:ListItem>
							<asp:ListItem Value="WR">W%R</asp:ListItem>
						</asp:checkboxlist></td>
				</tr>
				<tr bgColor="whitesmoke">
					<td><B>Overlays:</B>
						<asp:checkboxlist id="cblOverlays" runat="server" AutoPostBack="True" RepeatDirection="Horizontal"
							RepeatLayout="Flow">
							<asp:ListItem Value="BBI">BBI</asp:ListItem>
							<asp:ListItem Value="SAR">SAR</asp:ListItem>
							<asp:ListItem Value="AreaBB" Selected="True">Bollinger Bands</asp:ListItem>
						</asp:checkboxlist></td>
				</tr>
				<tr bgColor="whitesmoke">
					<td><B>Compare:</B>
						<asp:literal id="lCompare" runat="server"></asp:literal>vs
						<asp:textbox id="tbCompare" runat="server"></asp:textbox><asp:checkboxlist id="cblCompare" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow">
							<asp:ListItem Value="^COMP">Nasdaq</asp:ListItem>
							<asp:ListItem Value="^DJI">Dow</asp:ListItem>
							<asp:ListItem Value="^IDX">S&amp;P</asp:ListItem>
						</asp:checkboxlist><asp:button id="btnCompare" runat="server" Text="Draw Chart" CssClass="ChartButton"></asp:button></td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<asp:ImageButton id="ibChart" runat="server" ToolTip="Click on the chart to add a 'cross-hair' price indicator tool."></asp:ImageButton></td>
				</tr>
			</table>
			<br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
