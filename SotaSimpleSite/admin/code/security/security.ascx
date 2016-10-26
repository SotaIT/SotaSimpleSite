<%@ Control Language="c#" AutoEventWireup="false" Codebehind="security.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.SecurityEditor" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<%string img = ResolveUrl("~/admin.ashx?img=security");%>
<script type="text/javascript" src="<%=sScript%>common.js"></script>
<style type="text/css">
TR.tr_head TD.selected { COLOR: #0000ff; BACKGROUND-COLOR: #ffffff }
TR.tr_head TD.selected A { COLOR: #0000ff }
TR.tr_head TD A { FONT-SIZE: 11px; COLOR: #ffffff }
TR.tr_head TD { FONT-WEIGHT: bold; BORDER-LEFT: #0000cc 1px solid; BACKGROUND-COLOR: #0000ff; TEXT-ALIGN: center }
a img{border:0px;vertical-align:middle;}
</style>
<table cellspacing="0" cellpadding="0" border="0" width="100%" height="100%" rules="groups" style="BORDER-RIGHT: #0000ff 1px solid; BORDER-TOP: #0000ff 1px solid; BORDER-LEFT: #0000ff 1px solid; BORDER-BOTTOM: #0000ff 1px solid">
	<tr><td>
			<table width="100%" cellspacing="0" cellpadding="4" border="0">
				<tr class="tr_head">
					<td width="2%">&nbsp;</td>
					<td width="10%" class='<%if(sTab==""){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/default.aspx'>Главная</a></td>
					<td width="10%" class='<%if(sTab=="users"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/users.aspx'>Пользователи</a></td>
					<td width="10%" class='<%if(sTab=="groups"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/groups.aspx'>Группы</a></td>
					<td width="10%" class='<%if(sTab=="parts"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/parts.aspx'>Разделы</a></td>
					<td width="10%" class='<%if(sTab=="actions"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/actions.aspx'>Действия</a></td>
					<td width="10%" class='<%if(sTab=="rules"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/rules.aspx'>Разрешения</a></td>
					<%--<td width="15%" nowrap class='<%if(sTab=="cache"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/cache.aspx'>Кэш 
							разрешений</a></td>--%>
					<td width="10%" class='<%if(sTab=="online"){%>selected<%}%>'><a href='<%=ResolveUrl("~/")%>admin/security/online.aspx'>Он-лайн</a></td>
					<td>&nbsp;</td>
				</tr>
			</table>
		</td>
	</tr>
	<tr>
		<td width="100%" height="100%">
			<table width="100%" height="100%" cellspacing="0" cellpadding="4" border="0">
				<tr><td valign="top">
						<%if(sTab==""){%>
						<br>
						<table> 
							<tr><td><img src="<%=img%>/users.gif"></td><td> <a href='<%=ResolveUrl("~/")%>admin/security/users.aspx'>Управление пользователями</a></td></tr> 
							<tr><td><img src="<%=img%>/groups.gif"></td><td><a href='<%=ResolveUrl("~/")%>admin/security/groups.aspx'>Управление группами</a></td></tr> 
							<tr><td><img src="<%=img%>/parts.gif"></td><td><a href='<%=ResolveUrl("~/")%>admin/security/parts.aspx'>Управление разделами безопасности</a></td></tr> 
							<tr><td><img src="<%=img%>/actions.gif"></td><td><a href='<%=ResolveUrl("~/")%>admin/security/actions.aspx'>Управление действиями</a></td></tr> 
							<tr><td><img src="<%=img%>/rules.gif"></td><td><a href='<%=ResolveUrl("~/")%>admin/security/rules.aspx'>Управление разрешениями действий</a></td></tr> 
							<%--<tr><td><img src="<%=img%>/cache.gif"></td><td><a href='<%=ResolveUrl("~/")%>admin/security/cache.aspx'>Просмотр и обновление кэша разрешений</a></td></tr> --%>
						</table>
						<%}%>
						<asp:placeholder runat="server" id="phItem"></asp:placeholder>
					</td></tr></table>
		</td>
	</tr>
</table>