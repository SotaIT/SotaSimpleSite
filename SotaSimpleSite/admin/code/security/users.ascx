<%@ Control Language="c#" AutoEventWireup="false" Codebehind="users.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.users" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('hsave').value = '1';
	getE('txtLogin').form.submit();
}
function ChangeGroup()
{
	var res = OpenModalDialog('change_group.aspx',getE('cmbGroup'),400,200);
	if(res)
	{
		var lst		= getE('cmbGroup');
		var h		= getE('hGroupIds');
		var n		= res.length;
		var s = ",";
		lst.options.length = 0;
		for(var i=0;i<n;i++)
		{
			var o	= new Option();
			o.text	= res[i][1];
			o.value	= res[i][0];
			lst.options[lst.options.length] = o;
			s+=o.value+",";
		}
		h.value = s;
	}
}
function EditFields(id)
{
	OpenModalDialog('user_fields.aspx?id='+id,null,400,220);
}
function FindUser() {
	location = '?ts_login='
	+ escape(document.getElementById('ts_login').value)
	+ '&ts_email='
	+ escape(document.getElementById('ts_email').value);
}
//-->
</script>
Логин: <input type="text" id="ts_login" value="<%=Request.QueryString["ts_login"]!=null ? Request.QueryString["ts_login"].Replace("\"","&quot;") : ""  %>" />
E-mail: <input type="text" id="ts_email" value="<%=Request.QueryString["ts_email"]!=null ? Request.QueryString["ts_email"].Replace("\"","&quot;") : ""  %>" />
<input type="button" value="Найти" onclick="FindUser();" /><br /><br />
<input type="hidden" name="hsave" id="hsave" value="" />
<input type="hidden" name="hpage" id="hpage" value="<%=dgUsers.PageNumber%>" />
<a href='?new='>Добавить пользователя</a><br /><br />
<style type="text/css">
table.users 
{
	width:100%;
	border:1px solid #00f;
	border-collapse:collapse;
}
table.users td
{
	border: 1px solid #ccc;
	padding: 5px;
}
tr.header td
{
	color:#fff;background-color:#00f;text-align:center;font-weight:bold;
}
</style>
<sota:richrepeater id="dgUsers" runat="server" DisplayPager="Both" PageSize="20">
<HeaderTemplate>
<table class="users">
<tr class="header">
	<td>Логин</td>
	<td>Пароль</td>
	<td>E-mail</td>
	<td>Поля</td>
	<td>Активен</td>
	<td>Группы</td>
	<td>&nbsp;</td>
	<td>&nbsp;</td>
</tr>
</HeaderTemplate>
	<ItemTemplate><tr></ItemTemplate>
	<AlternatingItemTemplate><tr style="background-color:#eee"></AlternatingItemTemplate>
	<EditItemTemplate>
	<tr style="<%# Container.ItemIndex%2==0 ? "" : "background-color:#eee" %>">
	<td>
		<input type="text" id="txtLogin" name="txtLogin" value='<%# Eval("login") %>' />
		<input type="hidden" name="hId" value='<%# Eval("userid") %>' />
	</td>
	<td>
		<input type="password" name="txtPassword" />
	</td>
	<td>
		<input name="txtEmail" type="text" value='<%# Eval("email") %>' />
	</td>
	<td style="text-align:center">
		<a href='javascript:EditFields(<%# Eval("userid") %>);'>...</a>
	</td>
	<td style="text-align:center">
		<input name="chkEnabled" type="checkbox<%#(Convert.ToBoolean(Eval("enabled")) ? "\" checked=\"checked\"" : "")%>" />
	</td>
	<td>
			<nobr><select name="cmbGroup" id="cmbGroup">
						<%# GetUserGroupsList(Eval("userid").ToString())%>
					</select>
					<input type="hidden" name="hGroupIds" id="hGroupIds" value='<%# GetUserGroupIds(Eval("userid").ToString())%>'>
					<input type="button" value="..." title="Изменить" onclick="ChangeGroup();">
				</nobr>
	</td>
	<td style="text-align:center">
		<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif" alt="Сохранить" /></a>
		<a href='?page=<%# dgUsers.PageNumber + searchQS%>' title="Отменить"><img src="<%=img%>/cancel.gif" alt="Отмена" /></a>
	</td>
	<td style="text-align:center">
		<%# Convert.ToInt32(Eval("userid")) < 1 ? "&nbsp;" : "<a href='?delete=" + Eval("userid") + "&page=" + dgUsers.PageNumber + searchQS + "' onclick=\"return confirm('Вы уверены, что хотите удалить пользователя \\\'" + Eval("login") + "\\\'?');\" title='Удалить'><img src='" + img + "/remove.gif' alt='Удалить' /></a>"%>
	</td>
	</tr>
	</EditItemTemplate>
	<AfterAnyItemTemplate>
		<td><%# Eval("login") %></td>
		<td style="text-align:center">***</td>
		<td><%# Eval("email", "<a href=\"mailto:{0}\">{0}</a>") %></td>
		<td style="text-align:center"><a href='javascript:EditFields(<%# Eval("userid") %>);'>...</a></td>
		<td style="text-align:center"><%# (bool)Eval("enabled") ? "Да" : "Нет"  %></td>
		<td><%# GetUserGroups(Eval("userid").ToString())%></td>
		<td style="text-align:center"><%# (int)Eval("userid") < -1 ? "&nbsp;" : "<a href='?edit=" + Eval("userid") + "&index=" + Container.ItemIndex + "&page=" + dgUsers.PageNumber + searchQS + "' title='Редактировать'><img src='" + img + "/edit.gif'></a>"%></td>
		<td style="text-align:center"><%# (int)Eval("userid") < 1 ? "&nbsp;" : "<a href='?delete=" + Eval("userid") + "&page=" + dgUsers.PageNumber + searchQS + "' onclick=\"return confirm('Вы уверены, что хотите удалить пользователя \\\'" + Eval("login") + "\\\'?');\" title='Удалить'><img src='" + img + "/remove.gif' alt='Удалить' /></a>"%></td>
	</tr>
	</AfterAnyItemTemplate>
	<FooterTemplate></table></FooterTemplate>
	<OnePageTemplate></OnePageTemplate>
</sota:richrepeater>