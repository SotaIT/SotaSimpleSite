<%@ Control Language="c#" AutoEventWireup="false" Codebehind="admin_login.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.AdminLoginPage" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%@ Register TagPrefix="sota" Assembly="SotaSimpleSite" Namespace="Sota.Web.SimpleSite.WebControls"%>
<script src="<%=ResolveUrl("~/adminscript.ashx?script=")%>xmlform.js" type="text/javascript"></script>
<script type="text/javascript">
<!--
function GotResponse(frm)
{
	var r = frm.ResponseText;
	if(r)
	{
		alert('Неправильные имя пользователя и пароль!');
	}
	else
	{
		location.href = decodeURIComponent(document.getElementById('<%=hRef.ClientID%>').value);
	}
}
//-->
</script>
<table width="100%" height="100%">
	<tr>
		<td align="center" valign="middle">
			<sota:xmlform id="xfLogin" runat="server" aftersubmit="GotResponse(this);">
			<input id="hRef" type="hidden" runat="server">
				<table>
					<tr>
						<td>Логин:</td>
						<td><input id="txtLogin" type="text" runat="server"></td></tr>
					<tr>
						<td>Пароль:</td>
						<td><input id="txtPass" type="password" runat="server"></td></tr>
					<tr>
						<td>&nbsp;</td>
						<td><input type="checkbox" id="chkSave" runat="server" style="vertical-align:middle;"><label for='<%=chkSave.ClientID%>'>Запомнить</label>
						</td></tr>
					<tr>
						<td>&nbsp;</td>
						<td><input type="submit" value="Войти>>">
						</td></tr></table>
			</sota:xmlform>
			<%--if(isCompitable){%><%}else{%>
			Приносим свои извинения!<br>
			Система управления на данный момент поддерживает только браузер Microsoft Internet Explorer 5.5 и выше.<br>
			В следующих версиях планируется поддержка других браузеров.
			
			<%}--%>
			</td>
	</tr>
</table>