<%@Import Namespace="Sota.Web.SimpleSite"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="create_list.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.list.create_list" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<script src="<%=sScript%>common.js" type="text/javascript"></script>
<script type="text/javascript">
<!--
function ShowFileExplorer()
{
	var res = OpenModalDialog('<%=ResolveUrl(Config.Main.FileManagerPage)%>?field=<%=txtFile.ClientID%>&filter=config',null,650,550);
	if(res)
		getE('<%=txtFile.ClientID%>').value = res;
}
function CheckListName(el)
{
	var pPattern = "[^0-9a-z_/]+";
	var s = el.value;
	if(s)
	{
		var ar = s.match(pPattern);
		while(ar)
		{
			var n = ar.length;
			for(var i=0;i<n;i++)
			{
				while(s.indexOf(ar[i])!=-1)
				{
					s = s.replace(ar[i],"");
				}
			}
			ar = s.match(pPattern);
		}
	}
	el.value = s;
}
function ProviderChange(v)
{
	var n = getE('spanNewProvider');
	if(v=='0')
	{
		n.style.display = '';
	}
	else
	{
		n.style.display = 'none';
		getE('txtProvider').value = '';
	}
}
//-->
</script>
<script for="window" event="onload">
<!--
<%if(sError.Length>0){%>
alert('Ошибка: <%=sError.Replace("'","\\'")%>');
<%}%>
//-->
</script>
<table width="100%" height="100%">
	<tr>
		<td align="center" valign="middle">
			<table align="center">
				<tr>
					<td>Идентификатор:</td>
				</tr>
				<tr>
					<td>
						<input style="WIDTH:200px" onchange="CheckListName(this);" type="text" id="txtName" runat="server">
						</td>
				</tr>
				<tr>
					<td>Название:</td>
				</tr>
				<tr>
					<td>
						<input style="WIDTH:200px" type="text" id="txtCaption" runat="server"></td>
				</tr>
				<tr>
					<td>Тип:</td>
				</tr>
				<tr>
					<td>
						<select name="cmbProvider" onchange="ProviderChange(this.value);">
						<option value="0">[Новый]</option>
						<%for(int i=0;i<tbProvider.Rows.Count;i++)
						{
							Response.Write(string.Format("<option value='{0}'{2}>{1}</option>", 
							tbProvider.Rows[i]["type"], 
							tbProvider.Rows[i]["name"],
							tbProvider.Rows[i]["type"].ToString()==List.LIST_TYPE ? " selected" : ""
							));
						}%>
						</select>
						<span id="spanNewProvider" style="display:none">Тип, Сборка: <input type="text" name="txtProvider"></span>
						</td>
				</tr>
				<tr>
					<td>Путь к файлу конфигурации:</td>
				</tr>
				<tr>
					<td>
						<input style="WIDTH:180px" type="text" id="txtFile" runat="server"><input onclick="ShowFileExplorer();" type="button" value="...">
					</td>
				</tr>
				<tr>
					<td align="center">
						<br>
						<asp:button id="cmdNext" text="Далее >>" runat="server"></asp:button>
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
