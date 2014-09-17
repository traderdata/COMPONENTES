<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="FixWrongData.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin.FixWrongData" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Fix wrong data</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><tl:admheader id="Admheader" runat="server"></tl:admheader>
			<table cellSpacing="1" cellPadding="4" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td>This will fix wrong data from yahoo finance<br>
						<asp:button id="btnFixWrongData" runat="server" Text="Fix wrong data"></asp:button><asp:label id="lFixProgress" runat="server"></asp:label><asp:datagrid id="dgWrongList" runat="server" DataKeyField="ConditionId" AutoGenerateColumns="False"
							CellPadding="3" BackColor="White" BorderWidth="1px" BorderStyle="None" BorderColor="#E7E7FF">
							<FooterStyle ForeColor="#4A3C8C" BackColor="#B5C7DE"></FooterStyle>
							<SelectedItemStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#738A9C"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#F7F7F7"></AlternatingItemStyle>
							<ItemStyle ForeColor="#4A3C8C" BackColor="#E7E7FF"></ItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="#F7F7F7" BackColor="#4A3C8C"></HeaderStyle>
							<Columns>
								<asp:HyperLinkColumn Text="List" DataNavigateUrlField="ConditionId" DataNavigateUrlFormatString="../DBDemos/StockList.aspx?ConditionId={0}"
									HeaderText="List"></asp:HyperLinkColumn>
								<asp:ButtonColumn Text="Delete" HeaderText="Delete" CommandName="Delete"></asp:ButtonColumn>
								<asp:BoundColumn DataField="Description" HeaderText="Description"></asp:BoundColumn>
								<asp:BoundColumn DataField="Count" HeaderText="Count"></asp:BoundColumn>
							</Columns>
							<PagerStyle HorizontalAlign="Right" ForeColor="#4A3C8C" BackColor="#E7E7FF" Mode="NumericPages"></PagerStyle>
						</asp:datagrid></td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
