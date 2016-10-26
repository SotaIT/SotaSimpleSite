<%@ Control Language="c#" AutoEventWireup="false" Codebehind="online.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.online" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<br>
<asp:datagrid id="dgOnline" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px"
	borderstyle="Double" bordercolor="#0000C0" width="100%" allowsorting="True">
	<AlternatingItemStyle BackColor="#EEEEEE"></AlternatingItemStyle>
	<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="Blue"></HeaderStyle>
	<Columns>
		<asp:BoundColumn DataField="datetime" SortExpression="datetime DESC" HeaderText="Последнее обновление"></asp:BoundColumn>
		<asp:BoundColumn DataField="uid" SortExpression="uid ASC" HeaderText="Код"></asp:BoundColumn>
		<asp:BoundColumn DataField="login" SortExpression="login ASC" HeaderText="Логин"></asp:BoundColumn>
		<asp:HyperLinkColumn Target="_blank" DataNavigateUrlField="sid" DataNavigateUrlFormatString="~/admin/log_view.aspx?sort=1&filter={0}"
			DataTextField="sid" SortExpression="sid ASC" HeaderText="Сессия"></asp:HyperLinkColumn>
		<asp:HyperLinkColumn Target="_blank" DataNavigateUrlField="ip" DataNavigateUrlFormatString="~/admin/log_view.aspx?sort=1&filter={0}"
			DataTextField="ip" SortExpression="ip ASC" HeaderText="IP"></asp:HyperLinkColumn>
		<asp:HyperLinkColumn Target="_blank" DataNavigateUrlField="url" DataTextField="url" SortExpression="url ASC"
			HeaderText="Страница"></asp:HyperLinkColumn>
	</Columns>
</asp:datagrid>
