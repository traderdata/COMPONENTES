<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Page language="c#" Codebehind="FormulaValue.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.FormulaValue" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - Scanning</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfFormulaValue" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><asp:label id="lChartPage" runat="server" Visible="False">Overview.aspx</asp:label>
			<asp:label id="lReplace" runat="server" Visible="False">20ema=1.00:>|0.00:<;50sma=1.00:>|0.00:<;Today=1.00:Short|0.00:Long;Yesterday=1.00:Short|0.00:Long</asp:label>
			<asp:label id="lGroup" runat="server" Visible="False">1;Dow30;;AA,ALD,AXP,BA,C,CAT,CHV,DD,DIS,EK,GE,GM,GT,HWP,IBM,IP,JNJ,JPM,KO,MCD,MMM,MO,MRK,PG,S,T,UK,UTX,WMT,XON|2;ETF;;EWZ,BDH,EWY,IBB,EEM,XLE,EWW,EWJ,UTH,EWO,BHH,XLU,EWC,IWO,BBH,IWM,IJR,IWN,SWH,WMH,IJS,PPH,XLV,IYR,EFA,GLS,IAU,OIH,QQQQ,IJH,EWA,IWD,MDY,EWU,IWB,IVE,IWF,XLI,$SPX,IVV,SPY,IVW,EWS,EWI,SMH,DVY,DIA,MKH,EWG,XLK,OEF,XLF,XLP,EWH,IIH,DHY,HHH,XLB,RTH,XLY,HYP,TIP,DHF,RKH,EWM,SHY,IEF,AGG,TTH,LQD,FXI,EWT,TLT|3;Nasdaq100;;CHIR,ATYT,AAPL,SIRI,GRMN,SEBL,BRCM,ADSK,SUNW,RIMM,SPER,MEDI,SYMC,ISIL,CMVT,SNDK,ESRX,XMSR,LVLT,AMGN,CTSHFLEX,MRVL,GILD,CTXS,ERICY,CSCO,CHPK,JSDU,CELG,DISH,SNPS,LRCX,PIXR,NTLI,MXIM,MLNM,KLAC,CECO,JNPR,APCC,WFMI,GENZ,XLNX,TEVA,SAIL,BEAS,BIIB,TLAB,ADBE,LNCE,FAST,PCAR,CDWC,MERQ,APOL,WYNN,ORCL,VRSN,XRAY,LLTC,AMAT,PAYX,INTU,MCIP,IACI,SSCC,FISV,QLGC,COST,AMZN,YHOO,LAMR,MCHP,BMET,SANM,EXPD,PDCO,MSFT,SPLS,NTAP,NVLS,DLTR,INTC,CMCSA,MOLX,BBBY,CTAS,SHLD,SBUX,EBAY,DELL,ERTS,IVGN,ALTR,PETM,LBTYA|4;SP100;;OMX,UIS,hal,exc,leh,medi,gs,rok,nsm,WMB,XOM,AXP,MO,HPQ,LU,AMGN,CSCO,XRX,EP,MMM,CPB,IP,PFE,RSH,WFT,SO,C,PG,CSC,TYC,BNI,G,ORCL,TXN,JNJ,MCD,AEP,BDK,WY,GE,MWD,hd,bud,gd,ati,hon,mrk,twx,MDT,HCA,PEP,AIG,BAX,BHI,JPM,CL,NSC,KO,EK,CI,UTX,VIA.B,BMY,USB,HIG,F,IBM,DD,WFC,HNZ,AES,T,CCU,HET,SLB,ABT,WMT,S,VZ,DOW,SBC,RTN,SLE,BAC,MSFT,DIS,FDX,EMC</asp:label>
			<asp:label id="lFormat" runat="server" Visible="False">R%=p2;21dayhvolme=f0</asp:label>
			<asp:label id="lColumn" runat="server" Visible="False"></asp:label>
			<table>
				<TBODY>
					<tr>
						<td colSpan="2"><asp:literal id="lGroupLink" Runat="server"></asp:literal></td>
					</tr>
					<tr>
						<td><asp:label id="lTotal" runat="server"></asp:label></td>
						<td align="right"><asp:label id="lDate" runat="server" Font-Bold="True"></asp:label></td>
					</tr>
					<tr>
						<td colSpan="2"><asp:datagrid id="dgList" runat="server" AllowCustomPaging="True" ForeColor="Black" PageSize="30"
								CellPadding="2" BackColor="White" BorderWidth="1px" BorderColor="DimGray" AllowPaging="True" AutoGenerateColumns="False">
								<FooterStyle BackColor="#CCCC99"></FooterStyle>
								<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#CE5D5A"></SelectedItemStyle>
								<AlternatingItemStyle BackColor="White"></AlternatingItemStyle>
								<ItemStyle BackColor="#F7F7DE"></ItemStyle>
								<HeaderStyle Font-Bold="True" ForeColor="Black" BackColor="#CCCC66"></HeaderStyle>
								<PagerStyle HorizontalAlign="Right" ForeColor="Black" BackColor="#F7F7DE" Mode="NumericPages"></PagerStyle>
							</asp:datagrid><tl:footer id="Footer" runat="server"></tl:footer>
		</form>
		</TD></TR></TBODY></TABLE>
	</body>
</HTML>
