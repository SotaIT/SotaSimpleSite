<%@ Control Language="c#" AutoEventWireup="false" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import namespace="Sota.Web.SimpleSite"%>
<%@ Import namespace="Sota.Web.SimpleSite.Security"%>
<%string img = ResolveUrl("~/admin.ashx?img=main");%>
<%string root = ResolveUrl("~/admin");%>
<%bool isadmin = SecurityManager.IsUserInGroup(UserInfo.Current.UserId,-3);%>
<table><%if(isadmin){%>
	<tr>
		<td><img src="<%=img%>/settings.gif"></td><td><a href="<%=root%>/settings.aspx">���������</a></td>
	</tr>
	<%if(Sota.Web.SimpleSite.Config.Main.AuthorizationEnabled){%>
	<tr>
		<td><img src="<%=img%>/security.gif"></td><td><a href="<%=root%>/security/default.aspx">������������</a></td>
	</tr><%}}%>
	<tr>
		<td><img src="<%=img%>/map.gif"></td><td><a href="<%=root%>/all.aspx">��������� � ������� �����</a></td>
	</tr>
	<%if(isadmin){%>
	<tr>
		<td><img src="<%=img%>/web.gif"></td><td><a href="<%=root%>/domain.aspx">������</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/template.gif"></td><td><a href="<%=root%>/template.aspx">�������</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/control.gif"></td><td><a href="<%=root%>/control.aspx">�����</a></td>
	</tr><%}%>
	<tr>
		<td><img src="<%=img%>/controls.gif"></td><td><a href="<%=root%>/controls.aspx">�������� ������</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/explorer.gif"></td><td><a href="<%=root%>/filemanager.aspx">��������
				��������</a> </td>
	</tr>
	<tr>
		<td><img src="<%=img%>/texteditor.gif"></td><td><a href="<%=root%>/texteditor.aspx">���������
				��������</a></td>
	</tr>
	<%if(isadmin){%>
	<tr>
		<td><img src="<%=img%>/texteditor1.gif"></td><td><a href="<%=root%>/execute.aspx">������� SQL</a></td>
	</tr><%}%>
	<tr>
		<td><img src="<%=img%>/lists.gif"></td><td><a href="<%=root%>/lists.aspx">������</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/aspx.gif"></td><td><a href="<%=root%>/modules.aspx">������</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/stats.gif"></td><td><a href="<%=root%>/stats.aspx">����������</a></td>
	</tr>
	<%--
	<tr>
		<td><img src="<%=img%>/search.gif"></td><td><a href="<%=root%>/search.aspx">�����</a></td>
	</tr>
	<tr>
		<td><img src="<%=img%>/help.gif"></td><td><a href="http://support.sotait.net/?site=<%=Path.Domain%>" target="_blank">������</a></td>
	</tr>
	--%>
</table>
