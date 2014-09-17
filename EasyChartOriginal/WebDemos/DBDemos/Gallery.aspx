<%@ Import NameSpace="WebDemos" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="Gallery.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.Gallery" %>
<HTML>
	<HEAD>
		<title>Easy financial chart - gallery</title>
		<LINK href="<%=WebDemos.Config.Path%>m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfHelp" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><br>
			<table>
				<tr>
					<td><b>Gallery group:</b>&nbsp;&nbsp;<asp:dropdownlist id="ddlType" runat="server" AutoPostBack="True">
							<asp:ListItem Value="Index.xml">Index Charts</asp:ListItem>
							<asp:ListItem Value="Commodity.xml">Commodities</asp:ListItem>
							<asp:ListItem Value="Economic.xml">Economic Indicators</asp:ListItem>
						</asp:dropdownlist>&nbsp;Click on charts to see full page view.
						<BR>
						<br>
					</td>
				</tr>
				<tr>
					<td><asp:datalist id="dlGallery" runat="server" RepeatDirection="Horizontal" CellPadding="0" Width="700px"
							RepeatColumns="3">
							<ItemTemplate>
								<table cellpadding="0" cellspacing="0" width="120">
									<tr>
										<td>
											<center>
												<a href='CustomChart.aspx?<%=Config.SymbolParameterName%>=<%#DataBinder.Eval(Container,"DataItem.Symbol")%><%#GetParam("Start","StartDate",Container)%><%#GetParam("Span","Span",Container)%>&End=<%#DateTime.Today.ToString("yyyyMMdd")%>&Cycle=<%#DataBinder.Eval(Container,"DataItem.BigCycle")%>&RType=1&Scale=1'>
													<img src='../Chart.aspx?Provider=<%=Config.GalleryDataManager%>&Code=<%#DataBinder.Eval(Container,"DataItem.Symbol")%>&Main=<%#DataBinder.Eval(Container,"DataItem.Main")%>&Type=3&Scale=1<%#GetParam("Start","StartDate",Container)%><%#GetParam("Span","Span",Container)%>&Skin=RedWhite&size=250*200&XFormat=<%#DataBinder.Eval(Container,"DataItem.XFormat")%>&XCycle=<%#DataBinder.Eval(Container,"DataItem.XCycle")%>&Cycle=<%#DataBinder.Eval(Container,"DataItem.Cycle")%>&His=0&SV=0&YFormat=<%#DataBinder.Eval(Container,"DataItem.YFormat")%>&BMargin=10&Layout=Small&End=<%#DateTime.Today.ToString("yyyyMMdd")%>' border=0>
												</a>
												<%#ChartFooter(Container)%>
										</td>
										<td width="20">&nbsp;&nbsp;&nbsp;</td>
									</tr>
								</table>
								<br>
								<br>
							</ItemTemplate>
						</asp:datalist><b>Symbols</b>
						<table>
							<tr>
								<td id="tdSymbols" runat="server"></td>
							</tr>
							<tr>
								<td>Duration:
									<asp:dropdownlist id="ddlDuration" runat="server">
										<asp:ListItem Value="Month2">Two Months</asp:ListItem>
										<asp:ListItem Value="Month6">Six Months</asp:ListItem>
										<asp:ListItem Value="Year1">One Year</asp:ListItem>
									</asp:dropdownlist></td>
							</tr>
							<tr>
								<td><asp:button id="btnGo" runat="server" Text="Show Gallery" CssClass="ChartButton"></asp:button></td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
