<%@ Page language="c#" Codebehind="DataFeeds.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Admin.DataFeeds" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="AdmHeader" src="AdmHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.0 Transitional//EN" >
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Data Feeds</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader><tl:admheader id="Admheader" runat="server"></tl:admheader>
			<table cellSpacing="1" cellPadding="4" bgColor="gray" width="650">
				<tr bgcolor="whitesmoke" runat="server" id="trMsg">
					<td>
						<asp:label id="lFunction" runat="server" ForeColor="Red"></asp:label><br>
						<asp:label id="lMsg" runat="server" ForeColor="#C04000"></asp:label>
						<asp:Button id="btnRefresh" runat="server" Text="Refresh"></asp:Button>
						<asp:Button id="btnStop" runat="server" Text="Stop"></asp:Button>
					</td>
				</tr>
				<tr bgColor="whitesmoke" runat="server" id="trFunction">
					<td>
						Data feed:<asp:dropdownlist id="ddlDataFeed" runat="server" AutoPostBack="True" EnableViewState="False"></asp:dropdownlist>
						<asp:dropdownlist id="ddlExchange" runat="server" EnableViewState="False"></asp:dropdownlist><BR>
						<asp:label id="lUsername" runat="server" EnableViewState="False">Username:</asp:label><asp:textbox id="tbUsername" runat="server" EnableViewState="False" Width="80px"></asp:textbox>
						<asp:label id="lPassword" runat="server" EnableViewState="False">Password:</asp:label><asp:textbox id="tbPassword" runat="server" TextMode="Password" EnableViewState="False" Width="80px"></asp:textbox>
						<br>
						<asp:literal id="lDescription" runat="server" EnableViewState="False"></asp:literal><BR>
						<asp:HyperLink id="hlUrl" runat="server" EnableViewState="False"></asp:HyperLink>
						<hr>
						<asp:button id="btnImportSymbol" runat="server" Text="Import Symbols"></asp:button>
						<hr>
						Start Date:
						<asp:TextBox id="tbStartDate" runat="server" Width="96px"></asp:TextBox>
						End Date:
						<asp:TextBox id="tbEndDate" runat="server" Width="96px"></asp:TextBox>
						<asp:button id="btnImportEod" runat="server" Text="Import End of Day Data"></asp:button>
						<hr>
						Start Date:
						<asp:TextBox id="tbHistoryStart" runat="server" Width="96px">1990-01-01</asp:TextBox>
						<asp:DropDownList id="ddlDownloadMode" runat="server"></asp:DropDownList>
						<asp:button id="btnImportHistorical" runat="server" Text="Import Historical Data"></asp:button>
						<br>
						<hr>
						<asp:Button id="btnDeleteExchange" runat="server" Text="Delete selected exchange"></asp:Button>
						<hr>
						Auto End of Day Service String :
						<asp:Label id="lAutoUpdateEodString" runat="server" ForeColor="Green"></asp:Label>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
