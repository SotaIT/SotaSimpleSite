<%@ Control Language="c#" AutoEventWireup="false" Codebehind="actions.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.actions" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('txtAction').form.submit();
}
//-->
</script>
<a href='?new='>Добавить действие</a><br><br>
<asp:datagrid id="dgActions" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE">
	</alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
	</headerstyle>
	<columns>
		<asp:templatecolumn headertext="Действие">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.actionname") %>
			</itemtemplate>
			<edititemtemplate>
				<input type="text" id="txtAction" name="txtAction" value='<%# DataBinder.Eval(Container, "DataItem.actionname") %>'>
				<input type="hidden" name="hId" value='<%# DataBinder.Eval(Container, "DataItem.actionid") %>'>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn headertext="Переменная">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.actionvar") %>
			</itemtemplate>
			<edititemtemplate>
				<input type="text" id="txtVar" name="txtVar" value='<%# DataBinder.Eval(Container, "DataItem.actionvar") %>'>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.actionid"))<0 ? "&nbsp;" : "<a href='?edit="+Container.ItemIndex+"' title='Редактировать'><img src='"+img+"/edit.gif'></a>" %>
			</itemtemplate>
			<edititemtemplate>
				<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
				<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.actionid"))<1 ? "&nbsp;" : "<a href='?delete="+DataBinder.Eval(Container, "DataItem.actionid")+"' onclick=\"return confirm('Вы уверены, что хотите удалить действие \\\'"+DataBinder.Eval(Container, "DataItem.actionname")+"\\\'?');\" title='Удалить'><img src='"+img+"/remove.gif'></a>" %>
			</itemtemplate>
		</asp:templatecolumn>
	</columns>
</asp:datagrid>
