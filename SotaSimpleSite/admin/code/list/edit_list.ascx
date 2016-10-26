<%@ Register TagPrefix="sota" Assembly="SotaSimpleSite" Namespace="Sota.Web.SimpleSite.WebControls"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="edit_list.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.list.edit_list" TargetSchema="http://schemas.microsoft.com/intellisense/ie5" %>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<%string sImages = ResolveUrl("~/admin.ashx?img=");%>
<link href="<%=ResolveUrl("~/admincss.ashx?css=")%>admin.pages.css" type="text/css" rel="stylesheet">
<script src="<%=sScript%>treeview.js" type="text/javascript"></script>
<script src="<%=sScript%>xmlform.js" type="text/javascript"></script>
<script src="<%=sScript%>common.js" type="text/javascript"></script>
<style> <!-- #aPreviewItem { color: #ffffff; }
	#aPreviewItem:hover { text-decoration: none; }
	#aPreviewItem img { border: 0; vertical-align: middle; }
	--> 
	</style>
<script type="text/javascript">
<!--
function OnLoad()
{
	FillComboBoxes();
	<%if(Request.QueryString["id"]!=null){%>
	OpenItem(<%=Request.QueryString["id"]%>);<%}else{%>
	CreateItem(0,-1);<%}%>
}
function CloseAll()
{
	treeview.closeAll();
}
function OpenAll()
{
	treeview.openAll();
}
var arComboBoxes = new Array();
function FillComboBoxes()
{
	var frm = getE('<%=xfItem.ClientID%>');
	var n = arComboBoxes.length;
	for(var i=0;i<n;i++)
	{
		XmlForm_FillListGet(frm, getE(arComboBoxes[i][0]), arComboBoxes[i][1]);
	}
}
var bItemParentIdChanged	= false;
var bItemDeletedChanged		= false;
var bItemOrderChanged		= false;

function ResetTheForm()
{
	bItemParentIdChanged			= false;
	bItemDeletedChanged				= false;
	bItemOrderChanged				= false;
	getE("cmbParent").disabled		= true;
	getE("aPreviewItem").href		= "javascript:Nothing();";
	getE("aPreviewItem").disabled	= true;
	getE("aPreviewItem").target		= '';
	var frm = getE("<%=xfItem.ClientID%>");
	frm.reset();
	return frm;
}
function CreateItem(level, pid)
{
	var frm = ResetTheForm();
	getE("field_<%=FIELD_ID%>").value = -1;
	var p = level+'#'+pid;
	XmlForm_SetSelectValue(getE("cmbParent"), p);
	getE("field_<%=FIELD_PARENT_ID%>").value = p;
	ShowSuitableFields();
	FillMulties();
}
function OpenItem(id)
{
	var frm = ResetTheForm();
	getE("field_<%=FIELD_ID%>").value = id;
	XmlForm_Refresh(frm, frm.action.split('?')[0]+'?id='+id);
	XmlForm_SetSelectValue(getE("cmbParent"), getE("field_<%=FIELD_PARENT_ID%>").value);
	treeview.openTo(id+<%=list.Data.Rows.Count+2%>,true);
	ShowSuitableFields();
	SetItemPreview();
	FillMulties();
}
function SetItemPreview()
{
	var sUrl  = GetLevelUrl(GetCurrentItemLevel());
	var frm = getE('<%=xfItem.ClientID%>');
	var n = frm.length;
	for(var i=0;i<n;i++)
	{
		var e = frm.elements[i];
		if(!e.disabled)
		{
			if(e.id)
			{
				var j = e.id.indexOf('field_');
				if(j!=-1)
				{
					var parName = '[@' + e.id.substring('field_'.length)+']';
					var val = XmlForm_GetElementValue(e);
					while(sUrl.indexOf(parName)!=-1)
					{
						sUrl = sUrl.replace(parName, val);
					}
				}
			}
		}
	}
	getE("aPreviewItem").disabled	= false;
	getE("aPreviewItem").target		= '_blank';
	getE("aPreviewItem").href		= sUrl;
}
function GetCurrentItemLevel()
{
	return parseInt(getE('cmbParent').value.split('#')[0]);
}
function GetLevelUrl(level)
{
	switch(level){<%System.Data.DataTable tbLU = list.LevelViewTable;
	string sDefaultUrl = "";
	foreach(System.Data.DataRow row in tbLU.Rows){
	if(row["level"].ToString()=="default")
		sDefaultUrl = row["url"].ToString();
	else
	{%>
	case <%=row["level"]%>:
		return '<%=row["url"]%>';<%}}%>
	default:
		return '<%=sDefaultUrl%>';
	}
}

var arNotNullFields = new Array();
<%foreach(System.Data.DataColumn col in list.Data.Columns){
if(col.ExtendedProperties["allownull"]!=null && col.ExtendedProperties["allownull"].ToString()!="1"){
%>arNotNullFields[arNotNullFields.length] = new Array('<%=col.ColumnName%>','<%=col.Caption%>');
<%}}%>
function ValidateFormFields()
{
	var arEmptyFields = new Array();
	var n = arNotNullFields.length;
	for(var i=0;i<n;i++)
	{
		if(ElValIsEmpty(getE('field_'+arNotNullFields[i][0])))
		{
			arEmptyFields[arEmptyFields.length] = arNotNullFields[i][1];
		}
	}
	if(arEmptyFields.length>0)
	{
		var s = "Не заполнены следующие поля:\n";
		var n = arEmptyFields.length;
		for(var i=0;i<n;i++)
		{
			s+="'"+arEmptyFields[i]+"'\n";
		}
		alert(s);
		return false;
	}
	return true;
}
function ElValIsEmpty(el)
{
	if(el.disabled)
		return false;
	if(el.value)
		return false;
	return true;
}
function SaveItem()
{
	getE("hAction").value = "save";
	if(ValidateFormFields())
		SubmitForm();
}
function GetDeleteItemQuestion(level)
{
	switch(level){<%System.Data.DataTable tbDQT = list.DeleteQuestionTable;
	string sDefaultQ = "";
	foreach(System.Data.DataRow row in tbDQT.Rows){
	if(row["level"].ToString()=="default")
		sDefaultQ = row["question"].ToString();
	else
	{%>
	case <%=row["level"]%>:
		return '<%=row["question"]%>';<%}}%>
	default:
		return '<%=sDefaultQ%>';
	}
}
function DeleteItem()
{
	if(getE("field_<%=FIELD_ID%>").value=="-1")
		return;
	var quest = GetDeleteItemQuestion(GetCurrentItemLevel());

	if(confirm(quest))
	{
		getE("hAction").value = "delete";
		HideAllCustoms();
		SubmitForm();
	}
}
function SubmitForm()
{
	var frm = getE("<%=xfItem.ClientID%>");
	XmlForm_OnSubmit(frm, frm.action);
	AfterSaveItem(frm);
}
function AfterSaveItem(frm)
{
	var id = frm.ResponseText;
	if(id)
	{
		if(id=="-1")
			location.href = GetClearLocation();
		else
			location.href = GetClearLocation()+"?id="+id;
	}
	else
	{
		if(bItemParentIdChanged	| bItemDeletedChanged | bItemOrderChanged)
		{
			location.href = GetClearLocation()+"?id="+getE("field_<%=FIELD_ID%>").value;
		}
	}
}
function GetClearLocation()
{
	return location.href.replace("#","").split('?')[0];
}
function DateFieldChanged(el, dv)
{
	RegexCorrectValue(el, /[0-9]{4}\-[0-9]{2}\-[0-9]{2}/, dv);
}
function TimeFieldChanged(el, dv)
{
	RegexCorrectValue(el, /[0-9]{2}:[0-9]{2}:[0-9]{2}/, dv);
}
function RegexCorrectValue(el, regex, dv)
{
	var value = el.value;
	if(RegexTrim(value).length == 0)
        return;        
    var rx = new RegExp(regex);
    var matches = rx.exec(value);
    if(matches == null)
	{
		el.value = dv;
	}
	else
	{
		el.value = matches[0];
	}
}
function RegexTrim(s) {
    var m = s.match(/^\s*(\S+(\s+\S+)*)\s*$/);
    return (m == null) ? "" : m[1];
}
function OpenFileUpload(id, root, filter)
{
	var res = OpenModalDialog('<%=FileManagerPage%>?field='+id+'&filter='+filter+'&root='+root,null,650,550);
	if(res)
		getE(id).value = res;
}
function OpenDatePicker(id)
{
	var res = OpenModalDialog('<%=DatePickerPage%>?field='+id+'&date='+getE(id).value,null,270,305);
	if(res)
		getE(id).value = res;
}
function OpenTimePicker(id)
{
	var res = OpenModalDialog('<%=TimePickerPage%>?field='+id+'&time='+getE(id).value,null,180,50);
	if(res)
		getE(id).value = res;
}
function OpenHtmlEditor(id)
{
	OpenModalDialog("<%=ResolveUrl(Sota.Web.SimpleSite.Config.Main.HtmlEditorPage)%>", getE(id),document.body.clientWidth,document.body.clientHeight);
}
function ShowFileField(id, root)
{
	var path = getE(id).value;
	if(path)
	{
		OpenDialog(root+"/"+path,null,640,480);
	}
}
function DownloadFileField(id, root)
{
	var path = getE(id).value;
	if(path)
	{
		location.href = '<%=DownloadPage%>?file=' + escape(root + '/' + path);
	}
}
function ShowSuitableFields()
{
	ShowSuitableFieldsByLevel(GetCurrentItemLevel());
}
function HideAllCustoms()
{
	var arAllFields = '<%foreach(System.Data.DataColumn col in list.Data.Columns){if(IsCustomField(col.ColumnName))Response.Write(","+col.ColumnName);}%>'.split(',');
	var n = arAllFields.length;
	for(var i=1;i<n;i++)
	{
		getE('tr_field_'+arAllFields[i]).style.display = 'none';
		getE('field_'+arAllFields[i]).disabled = true;
	}
}
function ShowSuitableFieldsByLevel(level)
{
	HideAllCustoms();
	var arSFields = null;
	switch(level)<%System.Collections.Hashtable hl = list.GetLevelColumns();%>
	{<%foreach(string key in hl.Keys)if(key!="-1"){%>
		case <%=key%>:
			arSFields = '<%=hl[key]+(hl["-1"]==null ? "" : ","+hl["-1"])%>'.split(',');
			break;<%}%>
		default:
			arSFields = '<%=hl["-1"]%>'.split(',');
			break;			
	}
	if(arSFields)
	{
		n = arSFields.length;
		for(var i=0;i<n;i++)
		{
			getE('tr_field_'+arSFields[i]).style.display = '';
			getE('field_'+arSFields[i]).disabled = false;
		}
	}
}
function cmbParentChange(cmb)
{
	var pid = cmb.value.split('#')[1];
	if(pid!='-1')
	{
		if(pid==getE("field_<%=FIELD_ID%>").value)
		{
			cmb.selectedIndex = 0;
		}
	}
	getE("field_<%=FIELD_PARENT_ID%>").value = getE("cmbParent").value; 
	ShowSuitableFields();
	bItemParentIdChanged = true;
}
var arrParentItemCount = new Array();
function MoveOption(lst1, lst2, i)
{
	try
	{
		if(lst1.options[i])
		{
			var o	= new Option();
			o.value	= lst1.options[i].value;
			if(lst1.options[i].parentNode.tagName.toLowerCase()=='optgroup')
			{
				var label = lst1.options[i].parentNode.label;
				var oGroup = GetOptGroup(lst2, label);
				if(oGroup==null)
				{
					oGroup		= document.createElement('OPTGROUP');
					oGroup.label	= label;
					lst2.appendChild(oGroup);
				} 
				o.innerText = lst1.options[i].text;
				oGroup.appendChild(o);
				if(lst1.options[i].parentNode.children.length==1)
				{
					lst1.removeChild(lst1.options[i].parentNode);
				}
				else
				{
					lst1.options[i] = null;
				} 	
			}
			else
			{
				o.text	= lst1.options[i].text;
				lst2.options[lst2.options.length] = o;
				lst1.options[i] = null;
			}	
		}
	}
	catch(ex){}
}
function ChangeList(id1,id2,id)
{
	var lst1 = getE(id1+id);
	var lst2 = getE(id2+id);
	var i	= lst1.selectedIndex;
	if(i==-1)
	{
		if(lst1.options.length)
			i=0;
		else
			return;
	}
	MoveOption(lst1, lst2, i);
	if(lst1.options.length)
	{
		if(i>0)
		{
			lst1.selectedIndex = i-1;
		}
		else
		{
			lst1.selectedIndex = 0;
		}
		lst1.setActive();
	}
	LstSelectedChanged(id);		
}
function ChangeListAll(id1,id2,id)
{
	var lst1 = getE(id1+id);
	var lst2 = getE(id2+id);
	for(var i=lst1.length-1;i>-1;i--)
	{
		MoveOption(lst1, lst2, i);
	}
	LstSelectedChanged(id);
}
function LstSelectedChanged(id)
{
	var lst = getE('lst_selected_field_'+id);
	var h = getE('field_'+id);
	var s = '';
	if(lst.options.length)
	{
		s = '|';
		for(var i=0;i<lst.options.length;i++)
		{
			s+= lst.options[i].value+'|';
		}
	}
	h.value = s;
}
var arMultiFields = new Array();
function FillMulties()
{
	for(var i=0;i<arMultiFields.length;i++)
	{
		MultiChanged(arMultiFields[i]);
	}
}
function MultiChanged(id)
{
	var s = getE('field_'+id).value;
	ChangeListAll('lst_selected_field_','lst_all_field_',id);
	getE('field_'+id).value = s;
	if(s)
	{
		var arr		= s.substring(1,s.length-1).split('|');
		var lst1	= getE('lst_selected_field_'+id);
		var lst2	= getE('lst_all_field_'+id);
		for(var i=0;i<arr.length;i++)
		{
			var j = IndexOfOption(lst2, arr[i]);
			if(j!=-1)
			{
				MoveOption(lst2, lst1, j);
			}
		}
	}
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
function GetOptGroup(lst, label)
{
	var arr = lst.getElementsByTagName('optgroup');
	for(var i=0;i<arr.length;i++)
	{
		if(arr[i].label==label)
			return arr[i];
	}
	return null;
}

//-->
</script>
<%bool bUseIcon = !list.TreeNoIcon;%>
<table height="100%" cellspacing="1" cellpadding="2" width="100%" border="0">
	<tr valign="top">
		<td nowrap>
			<table class="tbform" height="100%" cellspacing="1" cellpadding="3" border="0">
				<tr><th><%=list.Caption%></th></tr>
				<tr><td valign="top" class="tdform">
						<a href="javascript: OpenAll();" title="Открыть все ветки" class="lnkButton"><img border="0" src="<%if(bUseIcon){%><%=ResolveUrl(list.OpenAllIcon)%><%}else{%><%=sImages+"closeall.gif"%><%}%>">
							Открыть все</a>
						<a href="javascript: CloseAll();" title="Закрыть все ветки" class="lnkButton"><img border="0" src="<%if(bUseIcon){%><%=ResolveUrl(list.CloseAllIcon)%><%}else{%><%=sImages+"openall.gif"%><%}%>">
							Закрыть все</a>
					</td></tr>
				<tr height="100%"><td valign="top" class="tdform">
						<!--Tree-->
						<div class="treeview" style="WIDTH:250px">
							<script type="text/javascript">
				<!--
				treeview = new dTree('treeview','<%if(bUseIcon){%><%=ResolveUrl(list.TreeRootIcon)%><%}else{%><%=sImages+"1x1.gif"%><%}%>');
				treeview.config.inOrder		= true;
				treeview.config.folderLinks = true;
				treeview.config.useCookie	= false;
				treeview.add(1, -1, '<%=list.Caption.Replace("'","\\'")%>');
				<%string[] arNewItemIC = list.GetNewItemIconCaption(0);%>
				treeview.add(2, 1, '<%=arNewItemIC[1].Replace("'","\\'")%>','javascript:CreateItem(0,-1);',null,null,'<%if(bUseIcon){%><%=ResolveUrl(arNewItemIC[0])%><%}%>');
<%
System.Data.DataRow[] rows = list.Data.Select("", list.TreeSortExpression);
int addId	= rows.Length+2;
int smallId = 3;
foreach(System.Data.DataRow row in rows)
{
	int level = Convert.ToInt32(row[FIELD_LEVEL]);
	string[] arIcons = list.GetLevelIcons(level);
	string sIcon = bUseIcon ? (row[FIELD_DELETED].ToString()=="0" ? ResolveUrl(arIcons[0]) : ResolveUrl(arIcons[1])) : "";
	int id	= Convert.ToInt32(row[FIELD_ID])+addId;
	int pid	= Convert.ToInt32(row[FIELD_PARENT_ID]);
	pid = pid==-1 ? 1 : pid+addId;
	
	Response.Write("\t\t\t\ttreeview.add("+id+","+pid+",'"+
		list.GetTreeCaption(row).Replace("\n","").Replace("\r","")+"','javascript:OpenItem("+
		row[FIELD_ID]+");',null,null,'"+sIcon+"','"+sIcon+"');\n");
	if(list.CanBeParent(level))
	{
		arNewItemIC = list.GetNewItemIconCaption(level+1);
		Response.Write("\t\t\t\ttreeview.add("+smallId+","+id+",'"+
		arNewItemIC[1].Replace("'","\\'")+"','javascript:CreateItem("+(level+1)+","+
		row[FIELD_ID]+");',null,null,'"+( bUseIcon ? ResolveUrl(arNewItemIC[0]) : "")+"');\n");
		smallId++;
	}				
}
%>
				document.write(treeview);
				treeview.closeAll();
				treeview.openTo(2, true);
				
				//-->
							</script>
						</div>		
						<!--/Tree-->
					</td></tr></table>
		</td>
		<td width="100%" height="100%">
			<table class="tbform" height="100%" cellspacing="1" cellpadding="3" width="100%" border="0">
				<tr><th align="left"><a id="aPreviewItem" href=""><img src="<%=sImages%>preview.gif"> Просмотр</a></th>
				</tr>
				<tr height="100%"><td valign="top" class="tdform">
						<!--Content-->
						<sota:xmlform runat="server" id="xfItem">
						<input id="hAction" type="hidden" value="save"> 
						<input id="field_<%=FIELD_ID%>" type="hidden" value="-1"> 
						<input id="field_<%=FIELD_PARENT_ID%>" type="hidden" value="0#-1"> 
            <table border="0" width="100%">
              <tr>
                <td>Папка:</td>
                <td width="100%"><select id="cmbParent" disabled onchange="cmbParentChange(this);"> 
                <option value="0#-1" selected>..</option> 
                <%
                System.Data.DataRow[] rows = list.Data.Select("", FIELD_ID+" ASC");
				foreach(System.Data.DataRow row in rows)
				{
					int level = Convert.ToInt32(row[FIELD_LEVEL]);
					if(list.CanBeParent(level))
						Response.Write("<option value=\""+(level+1)+"#"+row[FIELD_ID]+"\">"+list.GetTreeCaption(row)+"</option>");
				}
                %>
                </select>
                <%
                System.Collections.Hashtable hParentItemCount = new System.Collections.Hashtable();
				foreach(System.Data.DataRow row in rows)
				{
					int level = Convert.ToInt32(row[FIELD_LEVEL]);
					if(list.CanBeParent(level))
					{
						if(hParentItemCount[row[FIELD_ID].ToString()]==null)
						{
							hParentItemCount[row[FIELD_ID].ToString()] = 0;
						}
					}
					string pid = row[FIELD_PARENT_ID].ToString();
					if(pid=="-1")
					{
						pid="0";
					}
					hParentItemCount[pid] = hParentItemCount[pid]==null ? 1 : Convert.ToInt32(hParentItemCount[pid])+1;
				}%>
				<script type="text/javascript">
				<!--
				<%
				foreach(string pid in hParentItemCount.Keys)
				{
					Response.Write("\n arrParentItemCount["+pid+"]="+hParentItemCount[pid]+";");
				}
                %>
				//-->
				</script>
                <input onclick="getE('cmbParent').disabled=!this.checked;" type="checkbox"></td></tr>
                <%foreach(System.Data.DataColumn col in list.Data.Columns){
				if(IsCustomField(col.ColumnName)){
				string intyp =  col.ExtendedProperties["inputtype"].ToString().ToLower();
				%>
              <tr id="tr_field_<%=col.ColumnName%>" style="DISPLAY: none">
                <td nowrap valign="<%if(intyp=="textarea" || intyp=="html" || intyp=="multi"){%>top<%}else{%>middle<%}%>"><%=col.Caption+":"%></td>
                <td><%			object defval = "";
								switch(intyp)
								{
									case "hidden":
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\""+intyp+"\" value=\""+col.DefaultValue+"\">"+col.DefaultValue);
									break;
									case "text":
									case "password":
										Response.Write("<input style=\"width:100%\" id=\"field_"+col.ColumnName+"\" type=\""+intyp+"\" value=\""+col.DefaultValue+"\">");
									break;
									case "regex":
										Response.Write("<input style=\"width:100%\" id=\"field_"+col.ColumnName+"\" type=\"text\" value=\""+col.DefaultValue+"\" onchange=\"RegexCorrectValue(this,/"+col.ExtendedProperties["regex"]+"/,'"+col.DefaultValue+"');\">");
									break;
									case "textarea":
										Response.Write("<textarea style=\"width:100%\" id=\"field_"+col.ColumnName+"\">"+col.DefaultValue+"</textarea>");
									break;
									case "checkbox":
										string sChecked = col.DefaultValue.ToString()=="1" ? " checked" : "";
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\"checkbox\""+sChecked+">");
									break;
									case "combobox":
										Response.Write("<select id=\"field_"+col.ColumnName+"\">\n");
										Response.Write("<option value=\"\" selected></option>\n");
										Response.Write("</select>\n");
										Response.Write("<script type='text/javascript'><!--");
										Response.Write("\n arComboBoxes[arComboBoxes.length] = new Array('field_"+col.ColumnName+"','"+ResolveUrl(col.ExtendedProperties["listurl"].ToString())+"');\n");
										Response.Write("//--></script>\n");
									break;
									case "file":
										string uploads	= ResolveUrl(col.ExtendedProperties["uploads"].ToString());
										string filter	= col.ExtendedProperties["filter"].ToString();
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\"text\" value=\""+col.DefaultValue+"\">");
										Response.Write("<input type=\"button\" value=\"...\" onclick=\"OpenFileUpload('field_"+col.ColumnName+"','"+uploads+"','"+filter+"');\">");
										Response.Write(" <a href=\"javascript:ShowFileField('field_"+col.ColumnName+"','"+uploads+"');\">Просмотр</a>");
										Response.Write(" <a href=\"javascript:DownloadFileField('field_"+col.ColumnName+"','"+uploads+"');\">Скачать</a>");
									break;
									case "date":
										defval = col.DataType==typeof(DateTime) ? (col.DefaultValue!=DBNull.Value ? Convert.ToDateTime(col.DefaultValue).ToString("yyyy-MM-dd") : "") : col.DefaultValue;
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\"text\" value=\""+defval+"\" onchange=\"DateFieldChanged(this,'"+defval+"');\">");
										Response.Write("<input type=\"button\" value=\"...\" onclick=\"OpenDatePicker('field_"+col.ColumnName+"');\">");
									break;
									case "time":
										defval = col.DataType==typeof(DateTime) ? (col.DefaultValue!=DBNull.Value ? Convert.ToDateTime(col.DefaultValue).ToLongTimeString() : "") : col.DefaultValue;
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\"text\" value=\""+defval+"\" onchange=\"TimeFieldChanged(this,'"+defval+"');\">");
										Response.Write("<input type=\"button\" value=\"...\" onclick=\"OpenTimePicker('field_"+col.ColumnName+"');\">");
									break;
									case "html":
										Response.Write("<textarea style=\"width:100%\" id=\"field_"+col.ColumnName+"\">"+col.DefaultValue+"</textarea>");
										Response.Write("<br><input type=\"button\" value=\"Редактировать...\" onclick=\"OpenHtmlEditor('field_"+col.ColumnName+"');\">");
									break;
									case "multi":
										Response.Write("<input id=\"field_"+col.ColumnName+"\" type=\"hidden\" value=\""+col.DefaultValue+"\">\n");
										Response.Write("<table cellspacing=\"0\" cellpadding=\"0\" width=\"100%\" border=\"0\">");
										Response.Write("<tr>");
										Response.Write("	<td width=\"45%\"><select id=\"lst_selected_field_" + col.ColumnName + "\" style=\"WIDTH: 100%\" size=\"10\"></select></td>");
										Response.Write("	<td align=\"center\">");
										Response.Write("	<input type=\"button\" value=\" << \" onclick=\"ChangeListAll('lst_all_field_','lst_selected_field_','"+col.ColumnName+"');\"><br><br>");
										Response.Write("	<input type=\"button\" value=\"  <  \" onclick=\"ChangeList('lst_all_field_','lst_selected_field_','"+col.ColumnName+"');\"><br><br>");
										Response.Write("	<input type=\"button\" value=\"  >  \" onclick=\"ChangeList('lst_selected_field_','lst_all_field_','"+col.ColumnName+"');\"><br><br>");
										Response.Write("	<input type=\"button\" value=\" >> \" onclick=\"ChangeListAll('lst_selected_field_','lst_all_field_','"+col.ColumnName+"');\">");
										Response.Write("	</td>");
										Response.Write("	<td width=\"45%\"><select id=\"lst_all_field_" + col.ColumnName + "\" style=\"WIDTH: 100%\" size=\"10\"></select></td></tr>");
										Response.Write("</table>");

										Response.Write("<script type='text/javascript'><!--");
										Response.Write("\n arMultiFields[arMultiFields.length] = '"+col.ColumnName+"';\n");
										Response.Write("\n arComboBoxes[arComboBoxes.length] = new Array('lst_all_field_"+col.ColumnName+"','"+ResolveUrl(col.ExtendedProperties["listurl"].ToString())+"');\n");
										Response.Write("//--></script>\n");
									break;
								}
								%></td></tr><%}}%>
              <tr>
                <td nowrap>Не показывать:</td>
                <td><input id="field_<%=FIELD_DELETED%>" 
                  onclick="bItemDeletedChanged=true;" type="checkbox"></td></tr>
              <tr>
                <td align=center colSpan=2>
                <input onclick="SaveItem();" type="button" value="Сохранить"> 
				<input onclick="DeleteItem();" type="button" value="Удалить">
				</td></tr></table>
						</sota:xmlform>
						<!--/Content-->
					</td></tr></table></td></tr></table>
<script type="text/javascript">
<!--
OnLoad();
//-->
</script>