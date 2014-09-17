<%@ Register TagPrefix="tl" TagName="DatePicker" src="~/DatePicker.ascx" %>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="SelectDateRange.ascx.cs" Inherits="WebDemos.DBDemos.SelectDateRange" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<B><INPUT type="radio" value="0" name="RangeType" <%=IsCheck(0)%>>Predefined 
	Date Range:</B>
<asp:radiobuttonlist id="rblRange" runat="server" RepeatDirection="Horizontal" RepeatLayout="Flow" AutoPostBack="True">
	<asp:ListItem Value="1m">1m</asp:ListItem>
	<asp:ListItem Value="3m">3m</asp:ListItem>
	<asp:ListItem Value="6m" Selected="True">6m</asp:ListItem>
	<asp:ListItem Value="1y">1y</asp:ListItem>
	<asp:ListItem Value="2y">2y</asp:ListItem>
	<asp:ListItem Value="3y">3y</asp:ListItem>
	<asp:ListItem Value="5y">5y</asp:ListItem>
	<asp:ListItem Value="10y">10y</asp:ListItem>
	<asp:ListItem Value="max">max</asp:ListItem>
</asp:radiobuttonlist><BR>
<B><INPUT type="radio" value="1" name="RangeType" <%=IsCheck(1)%>>Custom 
	Dates: </B>
<br>
&nbsp;&nbsp;&nbsp;&nbsp;From:
<tl:datepicker id="dpStart" runat="server"></tl:datepicker>To:
<tl:datepicker id="dpEnd" runat="server"></tl:datepicker><BR>
&nbsp;&nbsp;&nbsp;&nbsp;Show detail in :
<asp:dropdownlist id="ddlCycle" runat="server">
	<asp:ListItem Value="DAY1">Day</asp:ListItem>
	<asp:ListItem Value="DAY3">3-Days</asp:ListItem>
	<asp:ListItem Value="WEEK1">Week</asp:ListItem>
	<asp:ListItem Value="WEEK2">2-Weeks</asp:ListItem>
	<asp:ListItem Value="MONTH1">Month</asp:ListItem>
	<asp:ListItem Value="MONTH3">3-Months</asp:ListItem>
	<asp:ListItem Value="QUARTER1">1-Quarter</asp:ListItem>
	<asp:ListItem Value="MONTH6">6-Months</asp:ListItem>
	<asp:ListItem Value="YEAR1">Year</asp:ListItem>
</asp:dropdownlist>
