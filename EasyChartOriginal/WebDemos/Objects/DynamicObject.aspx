<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="~/DemoHeader.ascx" %>
<%@ Page language="c#" Codebehind="DynamicObject.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Objects.DynamicObject" %>
<HTML>
	<HEAD>
		<title>Easy Stock Object</title>
		<LINK href="../m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginheight="0"
		marginwidth="0">
		<form id="DynamicObject" method="post" encType="multipart/form-data" runat="server">
			<tl:header id="Header" runat="server"></tl:header><tl:demoheader id="DemoHeader" runat="server"></tl:demoheader>
			<table width="800">
				<tr>
					<td>
						<b><font color="red">All objects and all parameters can be bind at runtime. here is 
								only some simple samples.</font></b>
						<table width=800 border="1" style="color:Black;background-color:#ffffee;border-color:black;border-width:1px;border-style:solid;border-collapse:collapse;">
							<tr>
								<td>Stock Object Name</td>
								<td>Parameter</td>
							</tr>
							<tr>
								<td>
									<asp:checkbox id="cbPriceLabel" runat="server" Text="Price Label  " Checked="True"></asp:checkbox>
								</td>
								<td>
									Date:
									<asp:textbox id="tbPriceDate" runat="server">2003-11-3</asp:textbox>Price:
									<asp:textbox id="tbLablePrice" runat="server">25</asp:textbox>
								</td>
							</tr>
							<tr>
								<td>
									<asp:checkbox id="cbArrowLine" runat="server" Text="Arrow Line" Checked="True"></asp:checkbox>
								</td>
								<td>
									Start Date :
									<asp:textbox id="tbArrowStartDate" runat="server">2003-6-1</asp:textbox>
									Start Price :
									<asp:textbox id="tbArrowStartPrice" runat="server">25</asp:textbox>
									<br>
									Stop Date :
									<asp:textbox id="tbArrowStopDate" runat="server">2003-11-2</asp:textbox>
									Stop Price :
									<asp:textbox id="tbArrowStopPrice" runat="server">28</asp:textbox>
								</td>
							</tr>
							<tr>
								<td>
									<asp:checkbox id="cbRegression" runat="server" Text="Regression Line" Checked="True"></asp:checkbox>
								</td>
								<td>
									Start Date :
									<asp:textbox id="tbRegStartDate" runat="server">2003-7-1</asp:textbox>
									Stop Date :
									<asp:textbox id="tbRegStopDate" runat="server">2003-9-10</asp:textbox>
								</td>
							</tr>
							<tr>
								<td colspan="2">
									<asp:Button id="btnDraw" runat="server" Text="Draw Object"></asp:Button>
								</td>
							</tr>
						</table>
						<asp:imagebutton id="ibChart" runat="server"></asp:imagebutton><br>
					</td>
				</tr>
			</table>
			<tl:footer id="Footer" runat="server"></tl:footer></form>
	</body>
</HTML>
