<%@ Control Language="c#" AutoEventWireup="false" Codebehind="arcache.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.arcache" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<a href='?refresh=1'>Обновить</a><br>
<br>
<asp:datagrid id="dgRules" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE">
	</alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
	</headerstyle>
	<columns>
		<asp:templatecolumn headertext="Пользователь">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.userid") %>
			</itemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn headertext="Раздел">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.partid") %>
			</itemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn headertext="Действие">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.actionvar") %>
			</itemtemplate>
		</asp:templatecolumn>
	</columns>
</asp:datagrid>
