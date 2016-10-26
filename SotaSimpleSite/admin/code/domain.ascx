<%@ Control Language="c#" AutoEventWireup="false" Codebehind="domain.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.DomainEditor" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	document.getElementById('txtName').form.submit();
}
//-->
</script>
&nbsp;<a href="?new=">Добавить домен</a><br>
<br>
<asp:datagrid id="dgDomains" width="100%" bordercolor="#0000C0" borderstyle="Double" borderwidth="3px"
	cellpadding="4" autogeneratecolumns="False" runat="server">
	<AlternatingItemStyle BackColor="#EEEEEE"></AlternatingItemStyle>
	<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="Blue"></HeaderStyle>
	<Columns>
		<asp:TemplateColumn HeaderText="Домен">
			<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
			<ItemTemplate>
				<%# DataBinder.Eval(Container.DataItem, "name") %>
			</ItemTemplate>
			<EditItemTemplate>
				<input type="text" name="txtName" value='<%# DataBinder.Eval(Container.DataItem, "name") %>'>
			</EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Добавить путь">
			<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
			<ItemTemplate>
				<%# DataBinder.Eval(Container.DataItem, "addpath") %>
			</ItemTemplate>
			<EditItemTemplate>
				<input type="text" name="txtAddPath" value='<%# DataBinder.Eval(Container.DataItem, "addpath") %>' maxlength="20" />
			</EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn HeaderText="Сайт">
			<HeaderStyle HorizontalAlign="Center"></HeaderStyle>
			<ItemTemplate>
				<%# DataBinder.Eval(Container.DataItem, "site") %>
			</ItemTemplate>
			<EditItemTemplate>
				<input type="text" name="txtSite" value='<%# DataBinder.Eval(Container.DataItem, "site") %>' maxlength="20" />
			</EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemStyle Wrap="False" HorizontalAlign="Center"></ItemStyle>
			<ItemTemplate>
				<a href='?edit=<%#Container.ItemIndex%>' title='Редактировать'><img src='<%=img%>/edit.gif'></a>
			</ItemTemplate>
			<EditItemTemplate>
				<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
				<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>
			</EditItemTemplate>
		</asp:TemplateColumn>
		<asp:TemplateColumn>
			<ItemStyle HorizontalAlign="Center"></ItemStyle>
			<ItemTemplate>
				<a href='?delete=<%#DataBinder.Eval(Container.DataItem, "name")%>' onclick="return confirm('Вы уверены, что хотите удалить домен?');" title='Удалить'>
					<img src='<%=img%>/remove.gif'></a>
			</ItemTemplate>
		</asp:TemplateColumn>
	</Columns>
</asp:datagrid>
