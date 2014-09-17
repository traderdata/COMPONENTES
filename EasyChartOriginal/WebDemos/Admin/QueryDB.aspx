<%@ Page language="c#" Codebehind="QueryDB.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.QueryDB" enableViewState="False" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Query Database</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<tl:Admheader id="Admheader" runat="server"></tl:Admheader>
			<br>
			<table cellSpacing="1" cellPadding="4" width="680" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td>
						<asp:Label id="lQueryString" runat="server">Query String:</asp:Label><br>
						<asp:TextBox id="tbSql" runat="server" TextMode="MultiLine" Width="776px" Height="288px">select * from</asp:TextBox>
						<asp:Button id="tnOK" runat="server" Text="OK"></asp:Button>
						<asp:CheckBox id="cbExecute" runat="server" Text="Execute"></asp:CheckBox>
						<asp:DataGrid id="dgResult" runat="server" BorderColor="#999999" BorderStyle="Solid" BorderWidth="1px"
							BackColor="White" CellPadding="3" GridLines="Vertical" ForeColor="Black">
							<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#000099"></SelectedItemStyle>
							<AlternatingItemStyle BackColor="#CCCCCC"></AlternatingItemStyle>
							<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="Black"></HeaderStyle>
							<FooterStyle BackColor="#CCCCCC"></FooterStyle>
							<PagerStyle HorizontalAlign="Center" ForeColor="Black" BackColor="#999999"></PagerStyle>
						</asp:DataGrid>
						<br>
						<asp:Literal id="lMsg" runat="server"></asp:Literal>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
