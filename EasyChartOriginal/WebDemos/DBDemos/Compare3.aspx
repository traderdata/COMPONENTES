<%@ Page language="c#" Codebehind="Compare3.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.Compare3" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="SelectDateRange" src="~/SelectDateRange.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<META http-equiv="Content-Type" content="text/html; charset=gb2312">
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfCustomChart" method="post" runat="server">
			<asp:Literal ID="lSymbols" Runat="server" Visible="False" Text="^IRX,^OIX,^XOI,^ OSX,^OIH,^SPX,^RUT,^NDX,^DJI,ZN_F,ZT_N,ZB_F,ZF_F,^SOX,^XAL,^DRG,^XBD,^BTK,^IIX,^DFI,^HUI,^DTX,^DUX,^DJR,^INX,^GOX,^NBI,^XEX"></asp:Literal>
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><br>
			<b>Compare&nbsp;Historical Data:</b><br>
			<asp:DropDownList id="ddlSymbol1" runat="server" DataTextField="QuoteName" DataValueField="QuoteCode"
				Width="200px"></asp:DropDownList>
			<asp:Label id="lDefault1" runat="server" Visible="False">CPUSAM</asp:Label>
			<asp:DropDownList id="ddlSymbol2" runat="server" DataTextField="QuoteName" DataValueField="QuoteCode"
				Width="200px"></asp:DropDownList>
			<asp:DropDownList id="ddlSymbol3" runat="server" DataTextField="QuoteName" DataValueField="QuoteCode"
				Width="200px"></asp:DropDownList>
			<asp:button id="btnUpdate" runat="server" Text="Update Chart" CssClass="ChartButton"></asp:button>
			<br>
			<br>
			<asp:ImageButton id="ibChart" runat="server"></asp:ImageButton>
			<table>
				<tr>
					<td>
						<tl:SelectDateRange id="SelectDateRange" runat="server"></tl:SelectDateRange><br>
						<B>Scale:</B>
						<asp:radiobuttonlist id="rblScale" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True">
							<asp:ListItem Value="0" Selected="True">Linear</asp:ListItem>
							<asp:ListItem Value="1">Log</asp:ListItem>
						</asp:radiobuttonlist>
						<asp:button id="btnUpdate2" runat="server" CssClass="ChartButton" Text="Update Chart"></asp:button>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer>
		</form>
	</body>
</HTML>
