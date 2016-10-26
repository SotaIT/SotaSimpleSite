<%@ Control Language="c#" AutoEventWireup="false" Codebehind="parts.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.parts" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('txtPart').form.submit();
}
//-->
</script>
<a href='?new='>Добавить раздел</a><br><br>
<asp:datagrid id="dgParts" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE">
	</alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
	</headerstyle>
	<columns>
		<asp:templatecolumn headertext="Раздел">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.partname") %>
			</itemtemplate>
			<edititemtemplate>
				<input type="text" id="txtPart" name="txtPart" value='<%# DataBinder.Eval(Container, "DataItem.partname") %>'>
				<input type="hidden" name="hId" value='<%# DataBinder.Eval(Container, "DataItem.partid") %>'>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.partid"))<1 ? "&nbsp;" : "<a href='?edit="+Container.ItemIndex+"' title='Редактировать'><img src='"+img+"/edit.gif'></a>" %>
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
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.partid"))<1 ? "&nbsp;" : "<a href='?delete="+DataBinder.Eval(Container, "DataItem.partid")+"' onclick=\"return confirm('Вы уверены, что хотите удалить раздел \\\'"+DataBinder.Eval(Container, "DataItem.partname")+"\\\'?');\" title='Удалить'><img src='"+img+"/remove.gif'></a>" %>
			</itemtemplate>
		</asp:templatecolumn>
	</columns>
</asp:datagrid>
