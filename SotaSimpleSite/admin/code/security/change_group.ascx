<%@ Control Language="c#" AutoEventWireup="false" Codebehind="change_group.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.Security.change_group" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script event="onload" for="window" type="text/javascript">
<!--
if(window.dialogArguments)
{
	var lst		= window.dialogArguments;
	var lst2	= document.getElementById('cmbSelectedGroups');
	var lst3	= document.getElementById('<%=cmbGroups.ClientID%>');
	for(var i=0;i<lst.options.length;i++)
	{
		var o	= new Option();
		o.text	= lst.options[i].text;
		o.value	= lst.options[i].value;
		lst2.options[lst2.options.length] = o;
		var j = IndexOfOption(lst3, lst.options[i].value);
		if(j!=-1)
		{
			lst3.options[j] = null;
		}
	}
}
//-->
</script>
<script type="text/javascript">
<!--
function Add_Group()
{
	ChangeList(document.getElementById('<%=cmbGroups.ClientID%>'),document.getElementById('cmbSelectedGroups'));
}
function Remove_Group()
{
	ChangeList(document.getElementById('cmbSelectedGroups'),document.getElementById('<%=cmbGroups.ClientID%>'));
}
function ChangeList(lst1,lst2)
{
	var i	= lst1.selectedIndex;
	if(i==-1)
		return;
	var o	= new Option();
	o.text	= lst1.options[i].text;
	o.value	= lst1.options[i].value;
	lst2.options[lst2.options.length] = o;
	lst1.options[i] = null;		
}
function IndexOfOption(lst, value)
{
	for(var i=0;i<lst.options.length;i++)
	{
		if(lst.options[i].value==value)
			return i;
	}
	return -1;
}
function Ok_Click()
{
	if(window.dialogArguments)
	{
		var arr = new Array();
		var lst	= document.getElementById('cmbSelectedGroups');
		for(var i=0;i<lst.options.length;i++)
		{
			arr[arr.length] = new Array(lst.options[i].value, lst.options[i].text);
		}
		window.returnValue = arr;
		window.close();
	}
}
//-->
</script>

<table cellspacing="0" cellpadding="2" width="100%" border="0">
	<tr>
		<td width="45%"><select id="cmbSelectedGroups" style="WIDTH: 100%" size="10" name="cmbSelectedGroups"></select></td>
		<td align="center">
		<input type="button" value="  >  " onclick="Remove_Group();"><br><br>
		<input type="button" value="  <  " onclick="Add_Group();">
		</td>
		<td width="45%"><select id="cmbGroups" style="WIDTH: 100%" size="10" runat="server"></select></td></tr>
	<tr>
		<td align="center" colspan="3">
			<input type="button" value="  ОК  " onclick="Ok_Click();">
			<input type="button" value="Отмена" onclick="window.close();">
		</td></tr></table>
