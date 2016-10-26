<%@ Control Language="c#" AutoEventWireup="false" Codebehind="text_editor.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.WebTextEditor" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<script src="<%=sScript%>common.js" type="text/javascript"></script>
<script type="text/javascript">
<!--
ctrlSEvent = 'getE(\'<%=cmdSave.ClientID%>\').click();';
function RefreshXmlForm()
{	
	location.href = '?path='+getE('<%=txtPath.ClientID%>').value;
}
function ShowFileExplorer()
{
	ShowFileExplorerF('*');
}
function ShowFileExplorerF(f)
{
	var res = OpenModalDialog('<%=FileManagerPage%>?field=<%=txtPath.ClientID%>&filter='+f,null,650,550);
	if(res)
		getE('<%=txtPath.ClientID%>').value = res;
}
function txtBody_OnScroll()
{
	var t = getE('<%=txtBody.ClientID%>');
	var n = getE('txtLineNumber');
	var p = getE('hScrollPos');
	p.value = t.scrollTop;
	n.scrollTop = t.scrollTop;
}
function txtLineNumber_OnScroll()
{
	var t = getE('<%=txtBody.ClientID%>');
	var n = getE('txtLineNumber');
	var p = getE('hScrollPos');
	p.value = n.scrollTop;
	t.scrollTop = n.scrollTop;
}
function txtBody_OnChange()
{
	var t = getE('<%=txtBody.ClientID%>');
	var n = getE('txtLineNumber');
	var arr = t.value.split('\n');
	var arr2 = n.value.split('\n');
	if(arr.length==(arr2.length-1))
	{
		return;
	}
	var s = '';
	for(var i=1;i<arr.length+1;i++)
	{
		s+= i;
		if(i<arr.length+1)
			s+='\n';
	}
	n.value = s;
	n.scrollTop = t.scrollTop;
}
function txtLineNumber_OnClick()
{
	//TODO - сделать выделение строки
}
function OpenFile(f)
{
	location.href = '?path='+f;
}
window.onload = function()
{
	txtBody_OnChange();
	var t = getE('<%=txtBody.ClientID%>');
	var n = getE('txtLineNumber');
	var p = getE('hScrollPos');
	if(p.value)
	{
		t.scrollTop = parseInt(p.value);
		n.scrollTop = parseInt(p.value);
	}

}
//-->
</script>
<style>
.tbEditor { BORDER-RIGHT: #cccccc 1px solid; BORDER-TOP: #cccccc 1px solid; BORDER-LEFT: #cccccc 1px solid; BORDER-BOTTOM: #cccccc 1px solid; BACKGROUND-COLOR: #eeeeee }
</style>
<table width="100%" border="0" height="100%" class="tbEditor">
	<tr>
		<td>Путь:</td>
		<td width="100%" nowrap>
			<input id="txtPath" onpropertychange="RefreshXmlForm()" type="text" runat="server" readonly
				size="70"> <input onclick="ShowFileExplorer();" type="button" value="...">
				&nbsp;
				 <a href="javascript:OpenFile('templates/default/css/style.css');">[Css]</a> 
				 <a href="javascript:OpenFile('<%=Sota.Web.SimpleSite.Config.Main.DefaultTemplate%>');">[Default Template]</a> 
				 <a href="javascript:OpenFile('web.config');">[Web.config]</a> 
				 <a href="javascript:OpenFile('robots.txt');">[Robots.txt]</a> 
				</td>
	</tr>
	<tr>
		<td><textarea onclick="txtLineNumber_OnClick()" onscroll="txtLineNumber_OnScroll()" readonly id="txtLineNumber" style="padding-bottom:17px;WIDTH:60px;HEIGHT:100%;background-color:#eee;"></textarea>
		<input type="hidden" name="hScrollPos" id="hScrollPos" value='<%=Request.QueryString["pos"]%>' />
		</td>
		<td height="100%"><textarea onkeyup="txtBody_OnChange()" onpaste="txtBody_OnChange()" onafterupdate="txtBody_OnChange()" onchange="txtBody_OnChange()" onscroll="txtBody_OnScroll()" class="allow_tab_key" id="txtBody" style="WIDTH: 100%; HEIGHT: 100%" wrap="off" runat="server"></textarea></td>
	</tr>
	<tr>
		<td>&nbsp;</td>
		<td><asp:button id="cmdSave" runat="server" text="Сохранить" /></td>
	</tr>
</table>