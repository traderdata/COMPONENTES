<%@ Control Language="c#" AutoEventWireup="false" Codebehind="ScanResultList.ascx.cs" Inherits="WebDemos.DBDemos.ScanResultList" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
	<asp:DataList id="dlSymbols" runat="server" BorderColor="#DEDFDE" BorderStyle="Solid" BackColor="White"
		CellPadding="4" GridLines="Both" BorderWidth="1px" RepeatColumns="10" ForeColor="Black">
		<SelectedItemStyle Font-Bold="True" ForeColor="White" BackColor="#CE5D5A"></SelectedItemStyle>
		<AlternatingItemStyle BackColor="White"></AlternatingItemStyle>
		<FooterStyle BackColor="#CCCC99"></FooterStyle>
		<ItemStyle BackColor="#F7F7DE"></ItemStyle>
		<ItemTemplate>
			<a href='CustomChart.aspx?Symbol=<%#DataBinder.Eval(Container,"DataItem.QuoteCode")%>'>
				<%#DataBinder.Eval(Container,"DataItem.QuoteCode")%>
			</a>
		</ItemTemplate>
		<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="#6B696B"></HeaderStyle>
	</asp:DataList>
