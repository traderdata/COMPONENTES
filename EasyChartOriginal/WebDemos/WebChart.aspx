<%@ Page language="c#" Codebehind="WebChart.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.WebChart" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="YahooQuotes" src="YahooQuotes.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="fmWebChart" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>

			<B>The chart data is downloaded from yahoo finance, and cached at local server.</B>
			<table cellSpacing="1" cellPadding="4" width="680" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td><B>Enter Sympol:</B><asp:textbox id="tbCode" runat="server">MSFT</asp:textbox>
						<asp:button id="btnOK" runat="server" Text="OK"></asp:button>
						<B>Skin:</B>
						<asp:DropDownList id="ddlSkin" runat="server" AutoPostBack="True"></asp:DropDownList><B>Size:</B>
						<asp:DropDownList id="ddlSize" runat="server" AutoPostBack="True">
							<asp:ListItem Value="640" Selected="True">640*480</asp:ListItem>
							<asp:ListItem Value="800">800*600</asp:ListItem>
							<asp:ListItem Value="1024">1024*768</asp:ListItem>
						</asp:DropDownList>
						<b>Width:</b><asp:DropDownList id="ddlWidth" runat="server" AutoPostBack="True">
							<asp:ListItem Value="1" Selected="True">1</asp:ListItem>
							<asp:ListItem Value="1.6">1.6</asp:ListItem>
							<asp:ListItem Value="2">2</asp:ListItem>
						</asp:DropDownList>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%">
							<tr>
								<td><B>Range:</B>
									<asp:radiobuttonlist id="rblRange" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True">
										<asp:ListItem Value="3m">3m</asp:ListItem>
										<asp:ListItem Value="6m" Selected="True">6m</asp:ListItem>
										<asp:ListItem Value="1y">1y</asp:ListItem>
										<asp:ListItem Value="2y">2y</asp:ListItem>
										<asp:ListItem Value="5y">5y</asp:ListItem>
										<asp:ListItem Value="max">max</asp:ListItem>
									</asp:radiobuttonlist></td>
								<td><B>Type:</B><asp:radiobuttonlist id="rblType" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True">
										<asp:ListItem Value="0">Bar1</asp:ListItem>
										<asp:ListItem Value="1">Bar2</asp:ListItem>
										<asp:ListItem Value="2">Line</asp:ListItem>
										<asp:ListItem Value="3" Selected="True">Candle</asp:ListItem>
									</asp:radiobuttonlist>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<table cellSpacing="0" cellPadding="0" width="100%">
							<tr>
								<td><B>Moving Avg:</B><asp:checkboxlist id="cblMA" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True">
										<asp:ListItem Value="5">5</asp:ListItem>
										<asp:ListItem Value="10">10</asp:ListItem>
										<asp:ListItem Value="20">20</asp:ListItem>
										<asp:ListItem Value="50">50</asp:ListItem>
										<asp:ListItem Value="100">100</asp:ListItem>
										<asp:ListItem Value="200">200</asp:ListItem>
									</asp:checkboxlist>
								</td>
								<td><B>EMA:</B><asp:checkboxlist id="cblEMA" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True">
										<asp:ListItem Value="5">5</asp:ListItem>
										<asp:ListItem Value="10" Selected="True">10</asp:ListItem>
										<asp:ListItem Value="20">20</asp:ListItem>
										<asp:ListItem Value="50" Selected="True">50</asp:ListItem>
										<asp:ListItem Value="100">100</asp:ListItem>
										<asp:ListItem Value="200" Selected="True">200</asp:ListItem>
									</asp:checkboxlist>
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td><B>Indicators:</B>
						<asp:checkboxlist id="cblIndicators" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
							AutoPostBack="True">
							<asp:ListItem Value="VOLMA">VOLMA</asp:ListItem>
							<asp:ListItem Value="MACD" Selected="True">MACD</asp:ListItem>
							<asp:ListItem Value="FastSTO">Fast Stoch</asp:ListItem>
							<asp:ListItem Value="SlowSTO">Slow Stoch</asp:ListItem>
							<asp:ListItem Value="ROC">ROC</asp:ListItem>
							<asp:ListItem Value="TRIX">TRIX</asp:ListItem>
							<asp:ListItem Value="RSI">RSI</asp:ListItem>
							<asp:ListItem Value="WVAD">WVAD</asp:ListItem>
							<asp:ListItem Value="ATR">ATR</asp:ListItem>
						</asp:checkboxlist></td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
					<table width=100%><tr><td>
					<B>Overlays:</B>
						<asp:checkboxlist id="cblOverlays" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal"
							AutoPostBack="True">
							<asp:ListItem Value="AreaBB">Bollinger Bands</asp:ListItem>
							<asp:ListItem Value="BBI">BBI</asp:ListItem>
							<asp:ListItem Value="SAR" Selected="True">SAR</asp:ListItem>
						</asp:checkboxlist>
						</td><td>
						<B>Scale:</B>
							<asp:radiobuttonlist id="rblScale" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" AutoPostBack="True">
								<asp:ListItem Value="0" Selected="True">Linear</asp:ListItem>
								<asp:ListItem Value="1">Log</asp:ListItem>
							</asp:radiobuttonlist>

					</td></tr></table>	
						</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td><B>Compare:</B>
						<asp:Literal id="lCompare" runat="server"></asp:Literal>
						vs
						<asp:TextBox id="tbCompare" runat="server"></asp:TextBox>
						<asp:checkboxlist id="cblCompare" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal">
							<asp:ListItem Value="^IXIC">Nasdaq</asp:ListItem>
							<asp:ListItem Value="^DJI">Dow</asp:ListItem>
							<asp:ListItem Value="^SPX">S&amp;P</asp:ListItem>
						</asp:checkboxlist>
						<asp:Button id="btnCompare" runat="server" Text="Compare"></asp:Button></td>
				</tr>
				<tr bgcolor="whitesmoke">
					<td>
						<asp:literal id="lChart" runat="server"></asp:literal>
						<tl:YahooQuotes id="YahooQuotes" runat="server" SymbolControl="tbCode" Width="640"></tl:YahooQuotes>
					</td>
				</tr>
			</table>
			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>
