<%@ Page language="c#" Codebehind="CustomChart.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.CustomChart"%>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="SelectDateRange" src="~/SelectDateRange.ascx" %>
<HTML>
	<HEAD>
		<title>Easy Stock Chart </title>
		<LINK href="<%=WebDemos.Config.Path%>m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="wfCustomChart" method="post" runat="server">
			<asp:literal id="lScript" runat="server"></asp:literal><tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><br>
			<asp:literal id="lOLname" runat="server" Visible="False">ol</asp:literal>
			<asp:literal id="lCookieIndi" runat="server" Visible="False">INDICATOR</asp:literal>
			<asp:literal id="lCookieOver" runat="server" Visible="False">OVERLAY</asp:literal>
			<asp:DropDownList id="ddlExchange" runat="server" DataValueField="value" DataTextField="text" AutoPostBack="True"  Visible="False"></asp:DropDownList>
			<B>Stock Symbol:</B>
			<asp:textbox id="tbCode" runat="server">ABC</asp:textbox>
			<asp:DropDownList id="ddlSymbol" runat="server" DataValueField="Company_Id" DataTextField="StockName"
				AutoPostBack="True" Visible="False"></asp:DropDownList>&nbsp;
			<asp:button id="btnOK" runat="server" CssClass="ChartButton" Text="Draw Chart"></asp:button><asp:checkbox id="cbRealTime" runat="server" Text="Include today's quote" AutoPostBack="True"></asp:checkbox><br>
			<br>
			<asp:ImageButton id="ibChart" runat="server" ToolTip="Click on the chart to add a 'cross-hair' price indicator tool."></asp:ImageButton>
			<asp:Label id="lRedirect" runat="server"></asp:Label><br>
			<B>Price Plot Attributes:</B><br>
			<table>
				<tr>
					<td>
						<tl:SelectDateRange id="SelectDateRange" runat="server"></tl:SelectDateRange><br>
						<br>
						<B>Skin:</B>
						<asp:dropdownlist id="ddlSkin" runat="server" AutoPostBack="True"></asp:dropdownlist><STRONG>&nbsp;Chart 
							Size:</STRONG>
						<asp:dropdownlist id="ddlSize" runat="server" AutoPostBack="True">
							<asp:ListItem Value="460">460</asp:ListItem>
							<asp:ListItem Value="520">520</asp:ListItem>
							<asp:ListItem Value="640">640</asp:ListItem>
							<asp:ListItem Value="700">700</asp:ListItem>
							<asp:ListItem Value="780">780</asp:ListItem>
							<asp:ListItem Value="900">900</asp:ListItem>
							<asp:ListItem Value="1000">1000</asp:ListItem>
							<asp:ListItem Value="1260">1260</asp:ListItem>
							<asp:ListItem Value="1580">1580</asp:ListItem>
						</asp:dropdownlist><br>
						<B>Type:</B>
						<asp:radiobuttonlist id="rblType" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
							<asp:ListItem Value="0">HLC Bars</asp:ListItem>
							<asp:ListItem Value="1">OHLC Bars</asp:ListItem>
							<asp:ListItem Value="2">Line</asp:ListItem>
							<asp:ListItem Value="3" Selected="True">Candle</asp:ListItem>
						</asp:radiobuttonlist>&nbsp; <B>Scale:</B>
						<asp:radiobuttonlist id="rblScale" runat="server" AutoPostBack="True" RepeatDirection="Horizontal" RepeatLayout="Flow">
							<asp:ListItem Value="0" Selected="True">Linear</asp:ListItem>
							<asp:ListItem Value="1">Log</asp:ListItem>
						</asp:radiobuttonlist></td>
				</tr>
			</table>
			<hr>
			<b>Price Overlays:</b>
			<asp:dropdownlist id="ddlOver" runat="server" Visible="False" EnableViewState="False">
				<asp:ListItem Value="">-- None --</asp:ListItem>
				<asp:ListItem Value="HL">Horizontal Line</asp:ListItem>
				<asp:ListItem Value="MA">Simple Moving Average</asp:ListItem>
				<asp:ListItem Value="EMA">Exponential Moving Average</asp:ListItem>
				<asp:ListItem Value="BB">Bollinger Bands</asp:ListItem>
				<asp:ListItem Value="AreaBB">Bollinger Bands(Area)</asp:ListItem>
				<asp:ListItem Value="SAR">Parabolic SAR</asp:ListItem>
				<asp:ListItem Value="ZIGLABEL">Zig Label</asp:ListItem>
				<asp:ListItem Value="ZIG">ZigZag</asp:ListItem>
				<asp:ListItem Value="ZIGICON">Zig Icon</asp:ListItem>
				<asp:ListItem Value="SR">Support & Resistance</asp:ListItem>
				<asp:ListItem Value="SRAxisY">Support & Resistance(AxisY)</asp:ListItem>
				<asp:ListItem Value="COMPARE">Compare</asp:ListItem>
				<asp:ListItem Value="COMPARE2">Another Compare</asp:ListItem>
				<asp:ListItem Value="Fibonnaci">Fibonnaci retracements</asp:ListItem>
				<asp:ListItem Value="LinRegr">Linear Regression Channels</asp:ListItem>
				<asp:ListItem Value="ZigW">Zig /w Retracement</asp:ListItem>
				<asp:ListItem Value="ZigSR">Zig support and resistance</asp:ListItem>
			</asp:dropdownlist><asp:literal id="lOver" runat="server" Text="MA(13);MA(50)" Visible="False"></asp:literal>
			<table>
				<tr>
					<td width="20"></td>
					<td id="tdOverLay" runat="server"></td>
				</tr>
			</table>
			<hr>
			<b>Indicator Windows:</b>
			<asp:dropdownlist id="ddlIndi" runat="server" Visible="False" EnableViewState="False">
				<asp:ListItem Value="">-- None --</asp:ListItem>
				<asp:ListItem Value="MACD">MACD</asp:ListItem>
				<asp:ListItem Value="OBV">On Balance Volume</asp:ListItem>
				<asp:ListItem Value="ATR">Average True Range</asp:ListItem>
				<asp:ListItem Value="VOLMA">VOLUME</asp:ListItem>
				<asp:ListItem Value="FastSTO">Fast Stochastics</asp:ListItem>
				<asp:ListItem Value="SlowSTO">Slow Stochastics</asp:ListItem>
				<asp:ListItem Value="ROC">Rate of Change</asp:ListItem>
				<asp:ListItem Value="TRIX">Triple exponentially</asp:ListItem>
				<asp:ListItem Value="WR">Williams %R</asp:ListItem>
				<asp:ListItem Value="RSI">Relative Strength Index</asp:ListItem>
				<asp:ListItem Value="AreaRSI">Relative Strength Index(Area)</asp:ListItem>
				<asp:ListItem Value="CCI">CCI</asp:ListItem>
				<asp:ListItem Value="AD">Accum/Dist</asp:ListItem>
				<asp:ListItem Value="CMF">Chaikin Money Flow</asp:ListItem>
				<asp:ListItem Value="PPO">Price Oscillator (PPO)</asp:ListItem>
				<asp:ListItem Value="StochRSI">StochRSI</asp:ListItem>
				<asp:ListItem Value="ULT">Ultimate Oscillator</asp:ListItem>
				<asp:ListItem Value="BBWidth">Bollinger Band Width</asp:ListItem>
				<asp:ListItem Value="PVO">Percentage Volume Oscillator</asp:ListItem>
				<asp:ListItem Value="PR">Price Relative</asp:ListItem>
				<asp:ListItem Value="BBI">BBI</asp:ListItem>
				<asp:ListItem Value="CmpIndi">Compare Indicator</asp:ListItem>
				<asp:ListItem Value="ADX">Average Directional Index</asp:ListItem>
				<asp:ListItem Value="RefIndi">Reference indicator</asp:ListItem>
				<asp:ListItem Value="MFI">Money Flow Index</asp:ListItem>
			</asp:dropdownlist><asp:literal id="lIndicator" runat="server" Text="AreaRSI(14){U};VOLMA(60);MACD(26,12,9);ATR(10)"
				Visible="False"></asp:literal>
			<table>
				<tr>
					<td width="20"></td>
					<td id="tdIndicator" runat="server"></td>
				</tr>
			</table>
			<asp:button id="btnUpdate" runat="server" CssClass="ChartButton" Text="Draw Chart"></asp:button><br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
