<%@ Control Language="c#" AutoEventWireup="false" Codebehind="rules.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.rules" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('cmbPart').form.submit();
}
//-->
</script>
<a href='?new='>Добавить разрешение</a><br><br>
<asp:datagrid id="dgRules" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE">
	</alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
	</headerstyle>
	<columns>
		<asp:templatecolumn headertext="Группа">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.groupname") %>
			</itemtemplate>
			<edititemtemplate>
				<select id="cmbGroup" name="cmbGroup">
					<%# GetGroupList(DataBinder.Eval(Container, "DataItem.groupid").ToString())%>
				</select>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn headertext="Раздел">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.partname") %>
			</itemtemplate>
			<edititemtemplate>
				<select id="cmbPart" name="cmbPart">
					<%# GetPartList(DataBinder.Eval(Container, "DataItem.partid").ToString())%>
				</select>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn headertext="Действие">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.actionname") %>
				[<%# DataBinder.Eval(Container, "DataItem.actionvar") %>]
			</itemtemplate>
			<edititemtemplate>
				<select id="cmbAction" name="cmbAction">
					<%# GetActionList(DataBinder.Eval(Container, "DataItem.actionid").ToString())%>
				</select>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# IsInternalRule(Convert.ToInt32(DataBinder.Eval(Container, "DataItem.partid")),Convert.ToInt32(DataBinder.Eval(Container, "DataItem.groupid")),Convert.ToInt32(DataBinder.Eval(Container, "DataItem.actionid"))) ? "&nbsp;" : "<a href='?delete=&part="+ DataBinder.Eval(Container, "DataItem.partid")+"&group="+ DataBinder.Eval(Container, "DataItem.groupid")+"&action="+ DataBinder.Eval(Container, "DataItem.actionid")+"' onclick=\"return confirm('Вы уверены, что хотите удалить разрешение?');\" title='Удалить'><img src='"+img+"/remove.gif'></a>" %>
			</itemtemplate>
			<edititemtemplate>
				<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
				<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>
			</edititemtemplate>
		</asp:templatecolumn>
	</columns>
</asp:datagrid>