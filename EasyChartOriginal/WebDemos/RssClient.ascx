<%@ Control Language="c#" AutoEventWireup="false" Codebehind="RssClient.ascx.cs" Inherits="WebDemos.RssClient" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" EnableViewState="false" %>
<P>
<table cellpadding="0" cellspacing="0"><tr><td valign="center">
<asp:Label id="lTitle" runat="server" CssClass="Headline"></asp:Label>
</td><td width="20"></td> <td valign="center">
<asp:HyperLink id="hlTitle" runat="server"></asp:HyperLink>
</td></tr></table>
	
	<br>
	<asp:DataGrid id="dgNews" runat="server" ShowHeader="False" GridLines="None" AutoGenerateColumns="False">
		<ItemStyle Height="28px"></ItemStyle>
		<Columns>
			<asp:TemplateColumn>
				<ItemTemplate>
					<a class='News' href='<%# DataBinder.Eval(Container, "DataItem.link") %>' target="_blank">
						<%# DataBinder.Eval(Container, "DataItem.title") %>
					</a>-
					<%# DataBinder.Eval(Container, "DataItem.pubdate") %>
				</ItemTemplate>
			</asp:TemplateColumn>
		</Columns>
	</asp:DataGrid></P>
<!--
title='<%# DataBinder.Eval(Container, "DataItem.description") %>' 
//-->