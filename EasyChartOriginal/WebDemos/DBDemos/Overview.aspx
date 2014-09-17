<%@ Page language="c#" Codebehind="Overview.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.DBDemos.Overview" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="ScanResultList" src="ScanResultList.ascx" %>
<HTML>
	<HEAD>
		<title>Easy Stock Chart - Overview</title>
		<LINK href="<%=WebDemos.Config.Path%>m.css" type=text/css rel=stylesheet>
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="wfOverview" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><br>
			<table>
				<tr>
					<td>
						<tl:ScanResultList id="ScanResultList" FormulaName="UP" Exchange="" runat="server"></tl:ScanResultList>
						
						<B>Daily View:</B><br>
						
						
						<asp:HyperLink id="hlDaily" runat="server" 
							NavigateUrl="CustomChart.aspx?(Symbol)" 
							ImageUrl="~/Chart.aspx?Provider=DB&Size=600*600&(Symbol)&Cycle=DAY1&MA=50;200&IND=PPO{U};CMF&Span=7&Layout=2Line;Default;Price;HisDate&Skin=GreenRed">
						Daily View</asp:HyperLink><br>
						<br><br>
						<b>Weekly View:</b><br>
						<asp:HyperLink id="hlWeekly" runat="server"  
						NavigateUrl="CustomChart.aspx?(Symbol)" 
						ImageUrl="~/Chart.aspx?Provider=DB&Size=600*600&(Symbol)&Cycle=WEEK1&Span=25&MA=50;200&IND=PPO{U};CMF&Layout=2Line;Default;Price;HisDate&Skin=GreenRed">Weekly View</asp:HyperLink><br>
					</td>
				</tr>
			</table>
			<br>
			<asp:HyperLink id="hlChart" runat="server"></asp:HyperLink><br>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
