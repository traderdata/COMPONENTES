<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="Help.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Help" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfHelp" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>
			<br>
			<ul>
				<table>
					<tr>
						<td>
							<li>
								<a href="http://finance.easychart.net/Readme.htm">Readme.</a></li><br>
							<br>
							<li>
								<a href="http://finance.easychart.net/Help/WindowsDemo.htm">How to use Windows demo.</a></li><br>
							<br>
							<li>
								<a href="http://finance.easychart.net/Help/YourOwnFormula.htm">Write you own formulas for easy financial 
									chart.</a></li><br>
							<br>
							<li>
								<a href="../Help/ChartReadme.htm">Parameters of chart.aspx </a>
							</li>
							<br><br>
							<li>
								<a href="http://finance.easychart.net/WebDemos/FormulaHelp.aspx">Formula script language guide of easy financial 
									chart.</a></li><br>
							<br>
							<br>
						</td>
					</tr>
				</table>
			</ul>
			<br>
			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>
