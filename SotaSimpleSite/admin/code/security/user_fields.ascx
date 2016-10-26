<%@ Control Language="c#" AutoEventWireup="false" Codebehind="user_fields.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.user_fields" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	document.getElementById('txtField').form.submit();
}
//-->
</script>
<style>
A IMG { BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; BORDER-BOTTOM: 0px }
A { COLOR: #0000ff }
</style>
<form method="post">
	<table width="100%" border="0" height="100%">
		<tr>
			<td><a href="?id=<%=iUserId%>&amp;new=">Добавить</a><br><br></td></tr>
		<tr>
			<td height="100%">
				<div style="OVERFLOW: auto; HEIGHT: 100%"><asp:datagrid id="dgFields" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
						<alternatingitemstyle backcolor="#EEEEEE">
						</alternatingitemstyle>

						<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
						</headerstyle>

						<columns>
							<asp:templatecolumn headertext="Поле">
								<headerstyle horizontalalign="Center">
								</headerstyle>

								<itemtemplate>
									<%# DataBinder.Eval(Container.DataItem, "field") %>


								</itemtemplate>

								<edititemtemplate>
									<input type="text" id="txtField" name="txtField" value='<%# DataBinder.Eval(Container.DataItem, "field") %>'>


								</edititemtemplate>
							</asp:templatecolumn>
							<asp:templatecolumn headertext="Значение">
								<headerstyle horizontalalign="Center">
								</headerstyle>

								<itemtemplate>
									<%# DataBinder.Eval(Container.DataItem, "value") %>


								</itemtemplate>

								<edititemtemplate>
									<input type="text" id="txtValue" name="txtValue" value='<%# DataBinder.Eval(Container.DataItem, "value") %>'>


								</edititemtemplate>
							</asp:templatecolumn>
							<asp:templatecolumn>
								<itemstyle horizontalalign="Center">
								</itemstyle>

								<itemtemplate>
									<a href='?id=<%=iUserId%>&edit=<%#Container.ItemIndex%>' title="Редактировать"><img src="<%=img%>/edit.gif"></a>


								</itemtemplate>

								<edititemtemplate>
									<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
									<a href='?id=<%=iUserId%>' title="Отменить"><img src="<%=img%>/cancel.gif"></a>


								</edititemtemplate>
							</asp:templatecolumn>
							<asp:templatecolumn>
								<itemstyle horizontalalign="Center">
								</itemstyle>

								<itemtemplate>
									<%# "<a href='?id="+iUserId+"&delete="+DataBinder.Eval(Container.DataItem, "field")+"' onclick=\"return confirm('Вы уверены, что хотите удалить поле \\'"+DataBinder.Eval(Container.DataItem, "field")+"\\'?');\" title='Удалить'><img src='"+img+"/remove.gif'></a>"%>

								</itemtemplate>
							</asp:templatecolumn>
						</columns>
					</asp:datagrid></div></td></tr>
		<tr>
			<td align="center"><input onclick="window.close();" type="button" value="Закрыть"></td></tr></table>
</form>
