<%@ Register TagPrefix="sota" Assembly="SotaSimpleSite" Namespace="Sota.Web.SimpleSite.WebControls"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="trash.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.PageTrash" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<%string sImages = ResolveUrl("~/admin.ashx?img=");%>
<script src="<%=sScript%>xmlform.js" type=text/javascript></script>

<script type="text/javascript">
<!--
function getE(id)
{
	return document.getElementById(id);
}
function OnLoad()
{
	RefreshPagesList();
}
function RefreshPagesList()
{
	var frm = getE('<%=xfPages.ClientID%>');
	var lst = getE('<%=lstPages.ClientID%>');
	lst.options.length = 0;
	XmlForm_FillList(frm, lst, frm.action);
}
function GotResponse()
{
	var frm = getE('<%=xfPages.ClientID%>');
	var r = frm.ResponseText;
	if(r.length>0)
	{
		alert(r);
	}
	else
	{
		RefreshPagesList();
	}
}
function RestorePage()
{
	if(getE('<%=lstPages.ClientID%>').selectedIndex==-1)
		return;
	SubmitPagesForm("0");
}
function DeletePage()
{
	if(getE('<%=lstPages.ClientID%>').selectedIndex==-1)
		return;
	if(confirm('Вы уверены, что хотите удалить файл без возможности восстановления?'))
	{
		SubmitPagesForm("1");
	}
}
function SubmitPagesForm(act)
{
	var frm = getE('<%=xfPages.ClientID%>');
	getE('<%=hAction.ClientID%>').value = act;
	XmlForm_OnSubmit(frm, frm.action);
	GotResponse();
	if(opener)
	{
		try
		{
			if(act!="1")
			{
				opener.Synchronize();
			}
			opener.CheckTrash();
		}
		catch(ex){}
	}
}
//-->
</script>
<style>
td.but a
{
	text-decoration:none;
	color:#00f;
}
td.but img
{
	vertical-align:middle;
}
</style>
<sota:xmlform id="xfPages" runat="server"><input id="hAction" type="hidden" value="0" runat="server">
	<table align="center" width="100%">
		<tr>
			<td>Удаленные файлы:</td></tr>
		<tr>
			<td align="center">
			<select id="lstPages" size="6" runat="server" style="width:100%"></select></td></tr>
		<tr>
			<td nowrap align="center" class="but">
			<a title="Восстановить" href="javascript:RestorePage();"><img src="<%=ResolveUrl("~/admin.ashx?img=")%>trash_restore.gif" border="0"> Восстановить</a> 
			&nbsp;&nbsp; 
			<a title="Удалить" href="javascript:DeletePage();"><img src="<%=ResolveUrl("~/admin.ashx?img=")%>delete_red.gif" border="0"> Удалить</a> 
			&nbsp;&nbsp; 
			<a title="Обновить список" href="javascript:RefreshPagesList();"><img src="<%=ResolveUrl("~/admin.ashx?img=")%>trash_refresh.gif" border="0"> Обновить</a> 
			</td></tr></table></sota:xmlform>
<script type="text/javascript">
<!--
	OnLoad();
//-->
</script>

