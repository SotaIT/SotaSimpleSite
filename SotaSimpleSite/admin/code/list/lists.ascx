<%@ Control Language="c#" AutoEventWireup="false" Codebehind="lists.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.list.lists" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<style type="text/css">
tr.tbt td{border-top:1px solid #00f}
tr.tbb td{border-bottom:1px solid #00f}
</style>
<table border="0" cellpadding="3" cellspacing="0">
	<%if(IsAdmin){%>
	<tr class="<%if(rptLists.Items.Count > 0){%>tbb<%}%>">
		<td align="center">
			<a href='create_list.aspx'><img src='<%=ResolveUrl("~/admin.ashx?img=list/add.gif")%>'></a>
		</td>
		<td>
			<a href='create_list.aspx'>Создать</a>
		</td>
		<td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>
	<%}%>
	<asp:repeater id="rptLists" runat="server">
		<itemtemplate>
			<tr>
				<td>
					<a href='list/<%#DataBinder.Eval(Container.DataItem,"name")%>.aspx'>
						<img src='<%=ResolveUrl("~/admin.ashx?img=list/list.gif")%>' alt="" /></a>
				</td>
				<td>
					<a href='list/<%#DataBinder.Eval(Container.DataItem,"name")%>.aspx'><%#DataBinder.Eval(Container.DataItem,"caption")%></a>&nbsp;&nbsp;
				</td>
				<%if(IsAdmin){%>
				<td>
					<a title='Настройка списка &laquo;<%#DataBinder.Eval(Container.DataItem,"caption")%>&raquo;' href='config/list/<%#DataBinder.Eval(Container.DataItem,"name")%><%=Sota.Web.SimpleSite.Config.Main.Extension%>'><img src='<%=ResolveUrl("~/admin.ashx?img=list/config.gif")%>'></a>
				</td>
				<td>
					<a onclick='return confirm(<%# "\"Вы уверены, что хотите удалить список &laquo;"+DataBinder.Eval(Container.DataItem,"caption")+"&raquo;?\""%>);' title='Удалить список &laquo;<%#DataBinder.Eval(Container.DataItem,"caption")%>&raquo;' href='delete_list<%=Sota.Web.SimpleSite.Config.Main.Extension%>?list=<%#DataBinder.Eval(Container.DataItem,"name")%>'><img src='<%=ResolveUrl("~/admin.ashx?img=list/delete.gif")%>'></a>
				</td>
				<%}%>
			</tr>
		</itemtemplate>
	</asp:repeater>
	<%if(rptLists.Items.Count > 0){%>
	<tr class="tbt">
		<td align="center">
			<a href='listimport.aspx'><img src='<%=ResolveUrl("~/admin.ashx?img=list/import.gif")%>'></a>
		</td>
		<td>
			<a href='listimport.aspx'>Импорт данных</a>
		</td>
		<td>&nbsp;</td>
		<td>&nbsp;</td>
	</tr>
	<%}%>
</table>