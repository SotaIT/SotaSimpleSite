<%@ Control Language="c#" AutoEventWireup="false" Codebehind="groups.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.groups" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('txtGroup').form.submit();
}
//-->
</script>
<a href='?new='>�������� ������</a><br><br>
<asp:datagrid id="dgGroups" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE">
	</alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
	</headerstyle>
	<columns>
		<asp:templatecolumn headertext="������">
			<headerstyle horizontalalign="Center">
			</headerstyle>
			<itemtemplate>
				<%# DataBinder.Eval(Container, "DataItem.groupname") %>
			</itemtemplate>
			<edititemtemplate>
				<input type="text" id="txtGroup" name="txtGroup" value='<%# DataBinder.Eval(Container, "DataItem.groupname") %>'>
				<input type="hidden" name="hId" value='<%# DataBinder.Eval(Container, "DataItem.groupid") %>'>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.groupid"))<1 ? "&nbsp;" : "<a href='?edit="+Container.ItemIndex+"' title='�������������'><img src='"+img+"/edit.gif'></a>" %>
			</itemtemplate>
			<edititemtemplate>
				<a href='javascript:SubmitForm();' title="���������"><img src="<%=img%>/save.gif"></a>
				<a href='?' title="��������"><img src="<%=img%>/cancel.gif"></a>
			</edititemtemplate>
		</asp:templatecolumn>
		<asp:templatecolumn>
			<itemstyle horizontalalign="Center">
			</itemstyle>
			<itemtemplate>
				<%# Convert.ToInt32(DataBinder.Eval(Container, "DataItem.groupid"))<1 ? "&nbsp;" : "<a href='?delete="+DataBinder.Eval(Container, "DataItem.groupid")+"' onclick=\"return confirm('�� �������, ��� ������ ������� ������ \\\'"+DataBinder.Eval(Container, "DataItem.groupname")+"\\\'?');\" title='�������'><img src='"+img+"/remove.gif'></a>" %>
			</itemtemplate>
		</asp:templatecolumn>
	</columns>
</asp:datagrid>