<%@ Control Language="c#" AutoEventWireup="false" Codebehind="YahooQuotes.ascx.cs" Inherits="WebDemos.YahooQuotes" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:Panel Runat="server" ID="pnQuotes">
	<asp:Label id="lSymbol" runat="server" Font-Bold="True"></asp:Label>
	<asp:Label id="lName" runat="server" Font-Bold="True"></asp:Label>
	<TABLE cellSpacing="1" cellPadding="4" width="100%" bgColor="silver">
		<TR>
			<TD bgColor="whitesmoke">Last Trade
			</TD>
			<TD bgColor="white">
				<asp:Label id="lLastTradePriceOnly" runat="server" Font-Bold="True" Font-Size="12"></asp:Label></TD>
			<TD bgColor="whitesmoke">High
			</TD>
			<TD bgColor="white">
				<asp:Label id="lDaysHigh" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">EPS
			</TD>
			<TD bgColor="white">
				<asp:Label id="lEPSEstimateCurrentYear" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Trade Time
			</TD>
			<TD bgColor="white">
				<asp:Label id="lLastTradeTime" runat="server"></asp:Label>
			</TD>
			<TD bgColor="whitesmoke">Low
			</TD>
			<TD bgColor="white">
				<asp:Label id="lDaysLow" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">EPS next year
			</TD>
			<TD bgColor="white">
				<asp:Label id="lEPSEstimateNextYear" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Change
			</TD>
			<TD bgColor="white">
				<asp:Label id="lChangePercentChange" runat="server" Font-Bold="True"></asp:Label></TD>
			<TD bgColor="whitesmoke">52 week high
			</TD>
			<TD bgColor="white">
				<asp:Label id="l52weekhigh" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">P/E next year
			</TD>
			<TD bgColor="white">
				<asp:Label id="lPrice_EPSEstimateNextYear" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Bid
			</TD>
			<TD bgColor="white">
				<asp:Label id="lBid" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">52 week low
			</TD>
			<TD bgColor="white">
				<asp:Label id="l52weeklow" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Price/Sale
			</TD>
			<TD bgColor="white">
				<asp:Label id="lPrice_Sales" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Ask
			</TD>
			<TD bgColor="white">
				<asp:Label id="lAsk" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">50 day MA
			</TD>
			<TD bgColor="white">
				<asp:Label id="l50dayMovingAverage" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">PEG Growth
			</TD>
			<TD bgColor="white">
				<asp:Label id="lPEG_Ratio" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Volume
			</TD>
			<TD bgColor="white">
				<asp:Label id="lVolume" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">200 day MA
			</TD>
			<TD bgColor="white">
				<asp:Label id="l200dayMovingAverage" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Dividend/Share
			</TD>
			<TD bgColor="white">
				<asp:Label id="lDividend_Share" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Avg Volume
			</TD>
			<TD bgColor="white">
				<asp:Label id="lAverageDailyVolume" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Market Cap
			</TD>
			<TD bgColor="white">
				<asp:Label id="lMarketCapitalization" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Div Yield
			</TD>
			<TD bgColor="white">
				<asp:Label id="lDividendYield" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Open
			</TD>
			<TD bgColor="white">
				<asp:Label id="lOpen" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Book Value
			</TD>
			<TD bgColor="white">
				<asp:Label id="lBookValue" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Short Ratio
			</TD>
			<TD bgColor="white">
				<asp:Label id="lShortRatio" runat="server"></asp:Label></TD>
		</TR>
		<TR>
			<TD bgColor="whitesmoke">Prv Close
			</TD>
			<TD bgColor="white">
				<asp:Label id="lPreviousClose" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Price / Book
			</TD>
			<TD bgColor="white">
				<asp:Label id="lPrice_Book" runat="server"></asp:Label></TD>
			<TD bgColor="whitesmoke">Stock Exchange
			</TD>
			<TD bgColor="white">
				<asp:Label id="lStockExchange" runat="server"></asp:Label></TD>
		</TR>
	</TABLE>
</asp:Panel>
