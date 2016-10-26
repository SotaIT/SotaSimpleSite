<%@ Control Language="c#" AutoEventWireup="false" Codebehind="log_view.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.log_view" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Register TagPrefix="sss" Namespace="Sota.Web.SimpleSite" Assembly="SotaSimpleSite" %>
<style type="text/css">
table.options td
{
	border: solid 1px #ccf;
	padding: 2px;
}
textarea
{
	font-size:11px;
}
</style>
<form method="get">
<table class="options">
<tr>
	<td>
	Сортировка:</td>
	<td>
	<%
Response.Write("<select name=\"sort\">");
Response.Write("<option value=\"\"></option>");
Response.Write(string.Format("<option value=\"1\"{0}>по дате</option>", Request.QueryString["sort"]=="1" ? " selected" : ""));
Response.Write(string.Format("<option value=\"2\"{0}>по типу</option>", Request.QueryString["sort"]=="2" ? " selected" : ""));
Response.Write(string.Format("<option value=\"3\"{0}>по типу и дате</option>", Request.QueryString["sort"]=="3" ? " selected" : ""));
Response.Write(string.Format("<option value=\"4\"{0}>по дате и типу</option>", Request.QueryString["sort"]=="4" ? " selected" : ""));
Response.Write("</select>");
%>
	</td>
	<td valign="top" rowspan="5">
	<textarea wrap="off" style="height:130px;" readonly><%
	foreach(string t in arTypes)
	{
		Response.Write(string.Format("{0}:  {1}\n", t, htTypes[t]));
	}
	Response.Write(string.Format("--\nВсего:  {0}",htTypes["*"]));
	%></textarea>
	</td>
	<td rowspan="4"><textarea id="txtEncodedUrl" readonly></textarea></td>
</tr>
<tr>
	<td>Тип:</td>
	<td>
	<%
	Response.Write("<select name=\"type\">");
	Response.Write("<option value=\"\"></option>");
	foreach(string t in arTypes)
	{
		Response.Write(string.Format("<option value=\"{0}\"{1}>{0}</option>", t, type.ToLower()==t.ToLower() ? " selected" : ""));
	}
	Response.Write("</select>");
	%>
	</td>
</tr>
<tr>
	<td>Дата/Время:</td>
	<td><input type="text" name="date_f" value='<%=Request.QueryString["date_f"]%>' /> &mdash; <input type="text" name="date_t" value='<%=Request.QueryString["date_t"]%>'> <input type="button" value="Сегодня" onclick="document.all.date_f.value='<%=Sota.Web.SimpleSite.Config.Main.Now().ToString("yyyy-MM-dd")%>';document.all.date_t.value='';"/> <input type="button" value="Вчера" onclick="document.all.date_f.value='<%=Sota.Web.SimpleSite.Config.Main.Now().AddDays(-1).ToString("yyyy-MM-dd")%>';document.all.date_t.value='<%=Sota.Web.SimpleSite.Config.Main.Now().ToString("yyyy-MM-dd")%>';"/>
	</td>
</tr>
<tr>
	<td>Фильтр:</td>
	<td><input size="46" type="text" name="filter" value='<%=Request.QueryString["filter"]%>' />
	<input type="button" value="IP" onclick="document.all.filter.value='<%=Sota.Web.SimpleSite.Config.GetClientIP(Request)%>';" />
	<input type="button" value="SID" onclick="document.all.filter.value='<%=Session.SessionID%>';" /></td>
</tr>
<tr>
	<td>&nbsp;</td>
	<td><input type="submit" value="Применить" /></td>
	<td><input type="button" value="Декодировать" onclick="DecodeUrl();"></td>
</tr>
</table>
</form>
<script type="text/javascript">
function SelectAll(c)
{
	var dg = document.getElementById('<%=dgLogs.ClientID%>');
	var chk = dg.getElementsByTagName('INPUT');
	var v = c.checked;
	for(var i=0;i<chk.length;i++)
	{
		chk[i].checked = v;
	}
}
function SelectTR(c)
{
	var p = c.parentNode;
	while(p && p.tagName.toLowerCase()!='tr')
	{
		p = p.parentNode;
	}
	if(p)
	{
		if(p.oldBackgroundColor==undefined)
		{
			p.oldBackgroundColor = p.style.backgroundColor;
		}
		p.style.backgroundColor = c.checked ? '#ccc' : p.oldBackgroundColor;
	}
}
function DecodeUrl()
{
	if(document.selection)
	{
		if(document.selection.type.toLowerCase()=='control')
			return;	
		var oRange = document.selection.createRange();
	
		if(oRange.text.length > 0)
		{
			var s = oRange.text;
			try
			{
				s = decodeURI(s);
			}
			catch(ex)
			{
				try
				{
					s = decodeURIComponent(s);
				}
				catch(ex){s = 'не удалось декодировать';}
			}
			var txt = document.getElementById('txtEncodedUrl');
			txt.value = s;
		}
	}
}
</script>
<br>
<br>
<sss:form method="post" id="dgForm" runat="server">
<%if(Sota.Web.SimpleSite.Security.UserInfo.Current.IsInGroup(-3)){%><input type="submit" value="Удалить выделенное" onclick="return confirm('Вы уверены, что хотите удалить выделенные логи?');"><%}%>
<asp:datagrid id="dgLogs" runat="server" width="100%" autogeneratecolumns="False" cellpadding="4"
	borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" AllowPaging="true" PagerStyle-Mode="NumericPages" PageSize="50" PagerStyle-Position="TopAndBottom">
	<AlternatingItemStyle BackColor="#EEEEEE"></AlternatingItemStyle>
	<HeaderStyle Font-Bold="True" ForeColor="White" BackColor="Blue"></HeaderStyle>
	<Columns>
	<asp:templatecolumn headertext="&lt;input type='checkbox' onclick='SelectAll(this)'&gt;">
			<itemtemplate>
				<input onpropertychange="SelectTR(this)" type="checkbox" name="chk" value='<%# DataBinder.Eval(Container.DataItem, "id") %>'>
			</itemtemplate>
		</asp:templatecolumn>
		<asp:TemplateColumn HeaderText="#">
			<ItemTemplate>
				<%#Container.ItemIndex+1%>
			</ItemTemplate>
		</asp:TemplateColumn>
		<asp:BoundColumn DataField="datetime" HeaderText="Дата/Время">
		</asp:BoundColumn>
		<asp:BoundColumn DataField="type" HeaderText="Тип"></asp:BoundColumn>
		<asp:boundcolumn datafield="params" headertext="Параметры">
		</asp:boundcolumn>
	</Columns>
</asp:datagrid>
</sss:form>