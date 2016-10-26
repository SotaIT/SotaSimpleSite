<%@ Control Language="c#" AutoEventWireup="false" Codebehind="control.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.control" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	document.getElementById('txtName').form.submit();
}
//-->
</script>
&nbsp;<a href="?new=">Добавить блок</a><br><br>
<form method="post" action="<%=Path.Full%>">
	<asp:datagrid id="dgControl" width="100%" bordercolor="#0000C0" borderstyle="Double" borderwidth="3px" cellpadding="4" runat="server" autogeneratecolumns="False">
		<alternatingitemstyle backcolor="#EEEEEE">
		</alternatingitemstyle>

		<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
		</headerstyle>

		<columns>
			<asp:templatecolumn headertext="Название">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "name")%>

				</itemtemplate>

				<edititemtemplate>
					<input name="txtName" type="text" value='<%# DataBinder.Eval(Container.DataItem, "name")%>'>
				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn headertext="Файл">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "file") %>
				</itemtemplate>

				<edititemtemplate>
					<select name="cmbFile"><%#GetControls(DataBinder.Eval(Container.DataItem, "file").ToString())%></select>
				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn>
				<itemstyle horizontalalign="Center">
				</itemstyle>

				<itemtemplate>
					<a href='?delete=1&<%# "name="+DataBinder.Eval(Container.DataItem, "name")+"&file="+DataBinder.Eval(Container.DataItem, "file")%>' title="Удалить" onclick="return confirm('Вы действительно хотите удалить строку?');"><img src="<%=img%>/remove.gif"></a>


				</itemtemplate>

				<edititemtemplate>
					<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
					<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>


				</edititemtemplate>
			</asp:templatecolumn>
		</columns>
	</asp:datagrid>
</form>
