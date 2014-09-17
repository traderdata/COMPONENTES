<%@ Page language="c#" Codebehind="Formula.aspx.cs" AutoEventWireup="false" Inherits="WebDemos.Formula" ValidateRequest=false%>
<%@ Register TagPrefix="tl" TagName="DemoHeader" src="DemoHeader.ascx" %>
<%@ Register TagPrefix="tl" TagName="Footer" src="~/Footer.ascx" %>
<%@ Register TagPrefix="tl" TagName="Header" src="~/Header.ascx" %>
<HTML>
	<HEAD>
		<title>Easy financial chart </title>
		<meta name="vs_targetSchema" content="http://schemas.microsoft.com/intellisense/ie5">
		<link href="m.css" type="text/css" rel="stylesheet">
	</HEAD>
	<body vLink="#000080" aLink="#0000ff" link="#000080" leftMargin="0" topMargin="0" marginwidth="0"
		marginheight="0">
		<form id="wfFormula" method="post" runat="server">
			<tl:Header id="Header" runat="server"></tl:Header>
			<tl:DemoHeader id="DemoHeader" runat="server"></tl:DemoHeader>
			<B>This demo shows that Easy financial chart can compile Formula script on the fly.</B>
			<table cellSpacing="1" cellPadding="4" width="640" bgColor="gray">
				<tr bgcolor="whitesmoke">
					<td colspan="2">
						<B>Enter Sympol:</B>
						<asp:TextBox id="tbCode" runat="server">MSFT</asp:TextBox>
					</td>
				</tr>
				<tr bgColor="whitesmoke">
					<td>
						<b>Formula Name:</b>
						<asp:TextBox id="tbFormulaName" runat="server" Width="200px">Test</asp:TextBox>
						<br>
						<B>Formula Script:</B>(See online document for more detail)
						<asp:TextBox id="tbProgramScript" runat="server" TextMode="MultiLine" Width="488px" Height="200px">
DIFF:EMA(CLOSE,SHORT)-EMA(CLOSE,LONG);                   
DEA :EMA(DIFF,MID);                   
MACD:DIFF-DEA,COLORSTICK;
						</asp:TextBox>
						<br>
						<b>Parameter:</b>
						<table>
							<tr>
								<td>Name</td>
								<td>Default Value</td>
								<td>Minimum Value</td>
								<td>Maximum Value</td>
							</tr>
							<tr>
								<td>
									1.<asp:TextBox id="tbParamName1" runat="server" Width="80px">LONG</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbDefValue1" runat="server" Width="80px">26</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMinValue1" runat="server" Width="80px">20</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMaxValue1" runat="server" Width="80px">100</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									2.<asp:TextBox id="tbParamName2" runat="server" Width="80px">SHORT</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbDefValue2" runat="server" Width="80px">12</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMinValue2" runat="server" Width="80px">5</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMaxValue2" runat="server" Width="80px">19</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									3.<asp:TextBox id="tbParamName3" runat="server" Width="80px">MID</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbDefValue3" runat="server" Width="80px">9</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMinValue3" runat="server" Width="80px">2</asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMaxValue3" runat="server" Width="80px">20</asp:TextBox>
								</td>
							</tr>
							<tr>
								<td>
									4.<asp:TextBox id="tbParamName4" runat="server" Width="80px"></asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbDefValue4" runat="server" Width="80px"></asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMinValue4" runat="server" Width="80px"></asp:TextBox>
								</td>
								<td>
									<asp:TextBox id="tbMaxValue4" runat="server" Width="80px"></asp:TextBox>
								</td>
							</tr>
						</table>
						<asp:Button id="btnOK" runat="server" Text="OK"></asp:Button>
					</td>
					<td>
						<asp:literal id="lChart" runat="server"></asp:literal>
					</td>
				</tr>
			</table>
			<br>
			<tl:Footer id="Footer" runat="server"></tl:Footer>
		</form>
	</body>
</HTML>
