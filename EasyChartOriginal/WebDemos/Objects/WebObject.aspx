<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="WebObject.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Objects.WebObject" %>
<HTML>
	<HEAD>
		<title>Easy Stock Object</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="WebObject" method="post" encType="multipart/form-data" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<table width="800">
				<tr>
					<td>1.Select object file :
						<asp:dropdownlist id="ddlObject" runat="server"></asp:dropdownlist><asp:button id="btnShowObject" runat="server" Text="Show Object"></asp:button><br>
						2.Try our visual stock object designer, and save the objects to a file. (<a href="http://finance.easychart.net/DownloadForm.aspx">Dowload 
							Designer Here</a>) (<a href="http://finance.easychart.net/StockObjectDesigner.gif">Screen 
							Shot</a>)
						<br>
						3.Upload your object XML file and show it on the web: <INPUT id="File" type="file" runat="server">
						<asp:button id="btnUpload" runat="server" Text="Upload your object"></asp:button>
						<br>
						<asp:imagebutton id="ibChart" runat="server"></asp:imagebutton><br>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
