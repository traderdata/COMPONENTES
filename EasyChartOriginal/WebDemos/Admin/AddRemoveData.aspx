<%@ Page language="c#" Codebehind="AddRemoveData.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin.AddRemoveData" ValidateRequest="false" %>
<%@ Register TagPrefix="tl" TagName="DatePicker" src="~/DatePicker.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Demos</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><tl:admheader id="Admheader" runat="server"></tl:admheader><br>
			<br>
			Symbol:
			<asp:textbox id="tbSymbol" runat="server"></asp:textbox><br>
			<asp:label id="lMsg" runat="server" ForeColor="Red"></asp:label>
			<hr>
			Data format:
			<asp:dropdownlist id="ddlFormat" runat="server">
				<asp:ListItem Value="DATE(%d-MMM-yyyy),Open,High,Low,Close,Volume,AdjClose">dd-MMM-yyyy,Open,High,Low,Close,Volume,AdjClose</asp:ListItem>
				<asp:ListItem Value="DATE(%d-MMM-yy),Open,High,Low,Close,Volume">dd-MMM-yy,Open,High,Low,Close,Volume</asp:ListItem>
				<asp:ListItem Value="DATE(MM\/dd\/yyyy),High,Low,Close">MM/dd/yyyy,High,Low,Close</asp:ListItem>
				<asp:ListItem Value="DATE(yyyy),Close">yyyy,Close</asp:ListItem>
				<asp:ListItem Value="TICKER,DATE(yyyyMM%d),HIGH,LOW,CLOSE,VOLUME">TICKER,YYYYMMDD,HIGH,LOW,CLOSE,VOLUME</asp:ListItem>
				<asp:ListItem Value="TICKER,PER,DATE(yyyyMM%d),TIME,OPEN,HIGH,LOW,CLOSE,VOLUME,OPENINT">TICKER,PER,YYYYMMDD,TIME,OPEN,HIGH,LOW,CLOSE,VOLUME,OPENINT</asp:ListItem>
				<asp:ListItem Value="TICKER,DATE(yyyy-MM-%d),OPEN,HIGH,LOW,CLOSE,VOLUME">TICKER,yyyy-MM-dd,OPEN,HIGH,LOW,CLOSE,VOLUME</asp:ListItem>
				<asp:ListItem Value="TICKER,DATE(MM\/dd\/yyyy),OPEN,HIGH,LOW,CLOSE">TICKER,MM/dd/yyyy,OPEN,HIGH,LOW,CLOSE</asp:ListItem>
				<asp:ListItem Value="TICKER,DATE(MM\/dd\/yyyy),OPEN,HIGH,LOW,CLOSE,VOLUME">TICKER,MM/dd/yyyy,OPEN,HIGH,LOW,CLOSE,VOLUME</asp:ListItem>
			</asp:dropdownlist><br>
			Separator:
			<asp:dropdownlist id="ddlSeparator" runat="server">
				<asp:ListItem Value=",">Comma</asp:ListItem>
				<asp:ListItem Value="Tab">Tab</asp:ListItem>
				<asp:ListItem Value=";">Semicolon</asp:ListItem>
			</asp:dropdownlist>&nbsp;Date Format:
			<asp:dropdownlist id="ddlDateFormat" runat="server">
				<asp:ListItem>Default</asp:ListItem>
				<asp:ListItem Value="dd-MMM-yy">dd-MMM-yy</asp:ListItem>
				<asp:ListItem Value="dd-MMM-yyyy">dd-MMM-yyyy</asp:ListItem>
				<asp:ListItem Value="MM\/dd\/yyyy">MM/dd/yyyy</asp:ListItem>
				<asp:ListItem Value="yyyy-MM-dd">yyyy-MM-dd</asp:ListItem>
				<asp:ListItem Value="yyyyMMdd">yyyyMMdd</asp:ListItem>
				<asp:ListItem Value="yyyy">yyyy</asp:ListItem>
			</asp:dropdownlist><asp:checkbox id="cbHasHeader" runat="server" Text="Has data header"></asp:checkbox><br>
			<asp:textbox id="tbCSVData" runat="server" Height="185px" Width="728px" TextMode="MultiLine"></asp:textbox><br>
			<asp:button id="btnMerge" runat="server" Text="Import Data"></asp:button>&nbsp;
			<asp:button id="btnExport" runat="server" Text="Export Data"></asp:button>
			<hr>
			From:
			<tl:datepicker id="dpStart" runat="server"></tl:datepicker>To:
			<tl:datepicker id="dpEnd" runat="server"></tl:datepicker><br>
			<asp:button id="btnDelete" runat="server" Text="Delete Data"></asp:button>
			<hr>
			<asp:button id="btnClear" runat="server" Text="Delete Symbol &amp; all historical data"></asp:button>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
