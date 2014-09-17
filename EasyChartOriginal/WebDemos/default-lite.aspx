<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="Default.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Default" %>
<HTML>
	<HEAD>
		<title>Easy financial chart.NET - Demos home</title>
		<LINK href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="Default" method="post" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<br>
			<table cellSpacing="1" cellPadding="4" width="680" bgColor="gray">
				<tr bgColor="whitesmoke">
					<td>
						1.<A class="SiteLink" href="webchart.aspx">Web chart</A>
						<ul>
							This sample shows a rich function web stock chart.You can custom the stock 
							chart as you wish.The stock data is downloaded from yahoo finance and cached in 
							the web server.
						</ul>
						2.<A class="SiteLink" href="Formula.aspx">Formula on the fly</A>
						<ul>
							You can try the indicator script system using this sample.And it shows how to 
							compile the indicator script on the fly.
						</ul>
						3.<A class="SiteLink" href="Explore.aspx">Formula Explore</A>
						<ul>
							You can store all stock indicator in one or more separate assemblies, and Load 
							it on runtime. Stock indicator script will be stored in a xml file . "formula 
							compiler" can create and modify the xml file easily. also it can compile the 
							script into assembly.
							<br>
							This sample need IE TreeView control.
						</ul>
						4.<A class="SiteLink" href="Compiler.aspx">Tools:Formula Compiler</A>
						<ul>
							This sample shows some screen spot of our "formula compiler".
						</ul>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
