<%@ Register TagPrefix="sota" Assembly="SotaSimpleSite" Namespace="Sota.Web.SimpleSite.WebControls"%>
<%@Import Namespace="Sota.Web.SimpleSite"%>
<%@Import Namespace="Sota.Web.SimpleSite.Map"%>
<%@ Control Language="c#" AutoEventWireup="false" Codebehind="allcontent.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.AllContentEditor" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<%string sImages = ResolveUrl("~/admin.ashx?img=");%>
<link href="<%=ResolveUrl("~/admincss.ashx?css=")%>admin.map.css" type="text/css" rel="stylesheet">
<script src="<%=sScript%>treeview.js" type="text/javascript"></script>
<script src="<%=sScript%>xmlform.js" type="text/javascript"></script>
<script src="<%=sScript%>common.js" type="text/javascript"></script>
<script type="text/javascript">
<!--
var treeview;
var	mustRefreshTree		= false;
var	mustRefreshParents	= false;

function CloseAll()
{
	treeview.closeAll();
}
function OpenAll()
{
	treeview.openAll();
}
function CreateTheTree(items)
{
	treeview = new dTree('treeview','<%=sImages+"base.gif"%>');
	treeview.config.inOrder			= true;
	treeview.config.folderLinks		= true;
	treeview.config.useCookie		= false;
	eval(items);
	getE('divTree').innerHTML = treeview.toString();
}
function RefreshTheTree()
{
	SubmitForm("<%=xfTree.ClientID%>");
	var items = getE("<%=xfTree.ClientID%>").ResponseText;
	if(items)
	{
		CreateTheTree(items);
		var id = parseInt(getE("<%=hId.ClientID%>").value)+1;
		if(id>-1)
			treeview.openTo(id,true);
	}
}
function CreateItem(p,c,path)
{
	XmlForm_Reset(getE("<%=xfItem.ClientID%>"));
	CloseAllPagePanels();
	DisablePagePanels();
	getE('aUrl').href = '';
	getE("tOnlyChild").style.display = '';
	getE("<%=hId.ClientID%>").value = -2;
	getE("<%=xfItem.ClientID%>").reset();
	XmlForm_SetSelectValue(getE("<%=cmbParent.ClientID%>"),p+"#"+c);
	RefreshIndex();
	var cmb =  getE('<%=cmbIndex.ClientID%>');
	cmb.selectedIndex = cmb.options.length-1;
	TypeChanged();
	getE("<%=txtCache.ClientID%>").value	= 0;
	XmlForm_SetSelectValue(getE("<%=cmbPart.ClientID%>"),-1);
	XmlForm_SetSelectValue(getE("<%=cmbTemplate.ClientID%>"),"<%=Config.Main.DefaultTemplate%>");
	getE("<%=chkSecure1.ClientID%>").checked	= true;
	ChangeCaption();
	OpenPanel('Item');
	if(path)
	{
		getE('<%=txtPath.ClientID%>').value = path=='/' ? '' : path+'/';
	}
}
function OpenItem(id)
{
	CloseAllPagePanels();
	getE('aUrl').href = '';
	getE("<%=hId.ClientID%>").value = id;
	getE("tOnlyChild").style.display = id==-1 ? 'none' : '';
	OpenPanel('Item');
	treeview.openTo(id+1,true);
	RefreshIndex();
	XmlForm_SetSelectValue(getE("<%=cmbIndex.ClientID%>"),getE("<%=hIndex.ClientID%>").value);
	ChangeCaption();
	TypeChanged();
}
function SaveItem()
{
	if((getE('<%=cmbType.ClientID%>').value == 'Page')
		& (getE('<%=txtPath.ClientID%>').value.length == 0) 
		& (getE("<%=hId.ClientID%>").value != "-1"))
	{
		alert('Введите значение для поля "Путь"!');
		return;
	}
	SubmitForm("<%=xfItem.ClientID%>");
	var	id = getE("<%=xfItem.ClientID%>").ResponseText;
	if(getE("<%=hId.ClientID%>").value=="-2")
	{
		mustRefreshTree		= true;
		mustRefreshParents	= true;
	}	
	if(mustRefreshTree)
	{
		RefreshTheTree();
		mustRefreshTree = false;
	}
	if(mustRefreshParents)
	{
		RefreshParents();
		mustRefreshParents = false;
	}
	if(id)
	{
		OpenItem(parseInt(id));
	}
	else
	{
		alert('Не удалось сохранить страницу!\nВозможно в поле "Путь" указан путь уже существующей страницы!');
	}
}
function DeleteItem()
{
	if(parseInt(getE("<%=hId.ClientID%>").value)==-2)
		return;
	if(confirm("Вы действительно хотите удалить элемент?"))
	{
		SubmitForm("<%=xfDelete.ClientID%>");
		RefreshParents();
		RefreshTheTree();
		CheckTrash();
		OpenItem(0);
	}
}
function SubmitForm(id)
{
	XmlForm_OnSubmit(getE(id), GetActionUrl());
}
function RefreshParents()
{
	var cmb = getE("<%=cmbParent.ClientID%>");
	var val = cmb.value;
	cmb.options.length = 0;
	XmlForm_FillList(getE("<%=xfItem.ClientID%>"), cmb, GetClearLocation());
	XmlForm_SetSelectValue(cmb,val);
	RefreshIndex();
}
function RefreshIndex()
{
	if(getE('<%=cmbParent.ClientID%>').selectedIndex==-1)
		return;
	var n = parseInt(getE('<%=cmbParent.ClientID%>').value.split('#')[1])+2;
	var cmb = getE('<%=cmbIndex.ClientID%>');
	var val = cmb.value;
	cmb.options.length = 0;
	for(var i=1;i<n;i++)
	{
		var opt = document.createElement("OPTION");
		opt.value = i-1;
		opt.text = i;
		cmb.options[cmb.options.length] = opt;
	}
	if(getE("<%=hId.ClientID%>").value == "-2")
		cmb.selectedIndex = cmb.options.length-1;
	else
		XmlForm_SetSelectValue(cmb,val);
}
function GetActionUrl()
{
	return GetClearLocation()+'?id='+getE('<%=hId.ClientID%>').value;
}
function GetClearLocation()
{
	return location.href.split("#")[0].split('?')[0];
}
function Synchronize()
{
	SubmitForm("<%=xfSynchr.ClientID%>");
	RefreshTheTree();
	RefreshParents();
	var count = 0;
	var cp = getE("<%=cmbParent.ClientID%>");
	for(var i=0;i<cp.options.length;i++)
	{
		if(cp.options[i].value.indexOf('-1#')!=-1)
			count = parseInt(cp.options[i].value.substring(3));
	}
	CreateItem(-1,count);
	treeview.openTo(cp.options.length,true);
}
function CheckTrash()
{
	SubmitForm("<%=xfTrash.ClientID%>");
	if(getE("<%=xfTrash.ClientID%>").ResponseText)
	{
		getE('imgTrash').src = '<%=sImages%>trash_full.gif';
	}
	else
	{
		getE('imgTrash').src = '<%=sImages%>trash.gif';
	}
}
function PanelClick(id)
{
	var td	= getE('tr'+id);
	var i	= getE('img'+id);
	var b	= td.style.display =='none';
	if(b)
	{
		OpenPanel(id);
	}
	else
	{
		ClosePanel(id);
	}
}
function OpenPanel(id)
{
	var td	= getE('tr'+id);
	if(IsPanelDisabled(id))
		return;
	var i	= getE('img'+id);
	td.style.display = '';
	i.src = '<%=sImages%>nolines_minus.gif';
	var fid = "";
	switch(id)
	{
		case 'Item':
			fid = '<%=xfItem.ClientID%>';
			break;
		case 'Head':
			fid = '<%=xfHeader.ClientID%>';
			break;
		case 'Content':
			fid = '<%=xfContent.ClientID%>';
			break;
		case 'Field':
			fid = '<%=xfDict.ClientID%>';
			break;
		case 'Template':
			fid = '<%=xfTemplate.ClientID%>';
			break;
		case 'Code':
			fid = '<%=xfCode.ClientID%>';
			break;
		case 'Pagex':
			fid = '<%=xfPagex.ClientID%>';
			break;
	}
	if(fid)
		RefreshXmlForm(fid);
}
function ClosePanel(id)
{
	var tr	= getE('tr'+id);
	var i	= getE('img'+id);
	tr.style.display = 'none';
	i.src = '<%=sImages%>nolines_plus.gif';
}
var arPagePanels = new Array('Head','Content','Field'<%if(IsAdmin){%>,'Template','Code','Pagex'<%}%>);
function CloseAllPagePanels()
{
	for(var i=0;i<arPagePanels.length;i++)
		ClosePanel(arPagePanels[i]);
}
function EnablePagePanels()
{
	for(var i=0;i<arPagePanels.length;i++)
		EnablePanel(arPagePanels[i]);
}
function DisablePagePanels()
{	
	for(var i=0;i<arPagePanels.length;i++)
		DisablePanel(arPagePanels[i]);
}
function DisablePanel(id)
{
	var tr	= getE('tr'+id);
	tr.disabled = true;
}
function EnablePanel(id)
{
	var tr	= getE('tr'+id);
	tr.disabled = false;
}
function IsPanelDisabled(id)
{
	return getE('tr'+id).disabled;
}
function TypeChanged()
{
	if(getE('<%=cmbType.ClientID%>').value=="Page" && getE('<%=hId.ClientID%>').value!="-1")
	{
		getE("tOnlyPage").style.display = '';
		if(getE("<%=hId.ClientID%>").value!="-2")
			EnablePagePanels();
	}
	else
	{
		getE("tOnlyPage").style.display = 'none';
		DisablePagePanels();
	}
}
/////////////////////
function RefreshFieldsList()
{
	var frm = getE('<%=xfDict.ClientID%>');
	var lst = getE('<%=lstFields.ClientID%>');
	var url = GetActionUrl();
	lst.options.length = 0;
	var opt = document.createElement("OPTION");
	opt.value = "-";
	opt.text  = "[Новая]";
	lst.options[lst.options.length] = opt;
	XmlForm_FillList(frm, lst, url);
	OpenField("-");									
}
function OpenField(field)
{
	var frm = getE('<%=xfDict.ClientID%>');
	var lst = getE('<%=lstFields.ClientID%>');
	var url = GetActionUrl();
	if(field=="-")
	{
		XmlForm_Reset(frm);
		lst.selectedIndex = 0;
		frm.btnDelete.disabled = true;									
	}
	else
	{
		XmlForm_Refresh(frm, url+"&field="+field);
		frm.btnDelete.disabled = false;
	}
}
function SaveField()
{
	frmFieldsSubmit("0");
}
function DeleteField()
{
	if(confirm('Вы действительно хотите удалить метку?'))
	{
		frmFieldsSubmit("1");
		getE('btnDelete').disabled = true;
	}
}
function frmFieldsSubmit(act)
{
	var lst = getE('<%=lstFields.ClientID%>');
	if(lst.selectedIndex==-1)
		return;
	var field = lst.value;
	if(field=="-")
	{
		field = getE('<%=txtFieldName.ClientID%>').value;
	}
	var frm = getE('<%=xfDict.ClientID%>');
	var url = GetActionUrl();
	getE('<%=hFieldAction.ClientID%>').value = act;
	XmlForm_OnSubmit(frm, url);
	RefreshFieldsList();
	XmlForm_SetSelectValue(lst, field);
	OpenField(field);
}
function CheckFieldName(el)
{
	var pPattern = "[^0-9a-z_]+";
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
	if(s.substring(0,1).match("[0-9]+"))
		s = "_"+s;
	el.value = s;
}
/////////////////////
function RefreshXmlForm(id)
{	
	var url = GetActionUrl();
	var frm = getE(id);
	if(frm.id=="<%=xfDict.ClientID%>")
	{
		RefreshFieldsList(frm);
	}
	else
	{
		XmlForm_Refresh(frm, url);
		if(frm.id=="<%=xfTemplate.ClientID%>")
		{
			frm.style.display = getE("<%=txtTemplatePath.ClientID%>").value=="" ? 'none' : '';
		}
		else if(frm.id=="<%=xfCode.ClientID%>")
		{
			frm.style.display = getE("<%=txtCodePath.ClientID%>").value=="" ? 'none' : '';
		}
	}
}
function ShowWordsStat()
{
	var url = 'words.aspx?path='+getE('<%=txtPath.ClientID%>').value;
	OpenModelessDialog(url,window,250,200);
}
function InsertWord(w)
{
	var txt = getE('<%=txtKeyWords.ClientID%>');
	var s = txt.value;
	var arWords = s.split(',');
	var n = arWords.length;
	for(var i=0;i<n;i++)
	{
		if(arWords[i]==w)
		{
			return;
		}
	}
	if(s.length>0)
		s +=',';
	txt.value = s+w;
}
function CheckNumber(el)
{
	var s = el.value;
	var numPattern = "[^0-9]+";
	if(s)
	{
		var ar = s.match(numPattern);
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
			ar = s.match(numPattern);
		}
		if(s=="")
			s = "0";
	}
	else
	{
		s = "0";
	}
	el.value = parseInt(s);

}
function CheckPageName(el)
{
	var pPattern = "[^0-9a-z_/]+";
	var s = el.value.toLowerCase();
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
		if(s.substring(0,1)=='/')
		{
			s = s.substring(1);
		}
		if(s.substring(s.length-1, s.length)=='/')
		{
			s = s.substring(0, s.length-1);
		}
	}
	el.value = s;
}
function ShowFileExplorer(root, id, filter)
{
	var res = OpenModalDialog("<%=ResolveUrl(Config.Main.FileManagerPage)%>?field="+id+"&filter="+filter+"&root="+root,null,650,550);
	if(res)
	{
		getE(id).value = res;
	}
}
function DisplayTrash()
{
	var w = OpenDialog("trash.aspx",null,300,160);
}
function OpenHtmlEditor(id)
{
	OpenModalDialog("<%=ResolveUrl(Config.Main.HtmlEditorPage)%>", getE(id),document.body.clientWidth,document.body.clientHeight);
}
function ChangeCaption()
{
	if(getE("<%=hId.ClientID%>").value == "-1")
	{
			getE("headPagePathText").innerText = "http://<%=Path.Domain%>/";
			getE("lnkPreviewPage").href = "http://<%=Path.Domain%>/";
			getE("lnkPreviewPage").target = "_blank";
			return;
	}
	if(getE('<%=txtPath.ClientID%>').value)
	{
		if(getE('<%=cmbType.ClientID%>').value=='Page')
		{
			var p = getE("<%=chkSecure1.ClientID%>").checked ? "http" : "https";
			var s = p+"://<%=Path.Domain%>/"+getE('<%=txtPath.ClientID%>').value+"<%=Config.Main.Extension%>";
			getE("headPagePathText").innerText = s;
			getE("lnkPreviewPage").href = s;
			getE("lnkPreviewPage").target = "_blank";
			return;
		}
	}
	getE("headPagePathText").innerText = "[Новая страница]";
	getE("lnkPreviewPage").href = "javascript:Nothing();";
	getE("lnkPreviewPage").target = "";
}
function cmbParent_OnChange(cmb)
{
	var id = getE("<%=hId.ClientID%>").value;
	var pid = cmb.value.split('#')[0];
	if(id == pid)
	{
		cmb.selectedIndex = 0;
		return;
	}
	RefreshIndex();
	mustRefreshTree = true;
	mustRefreshParents = true;
}
//-->
</script>
<table height="100%" cellspacing="1" cellpadding="2" width="100%" border="0">
	<tr valign="top">
		<td nowrap>
			<table class="tbform" height="100%" cellspacing="1" cellpadding="3" border="0">
				<tr>
					<td class="tdform">
						<p class="menu_p">
						<a href="javascript: RefreshTheTree();" title="Обновить дерево">
								<img src='<%=sImages%>trash_refresh.gif'> Обновить</a>&nbsp; <a href="javascript: Synchronize();" title="Запустить синхронизацию">
								<img src='<%=sImages%>synchr.gif'> Синхронизация</a>
						</p>
					</td>
				</tr>
				<tr height="100%">
					<td valign="top" class="tdform" colspan="2">
						<!--Tree-->
						<div class="treeview" style="WIDTH:250px" id="divTree">
						</div>
						<!--/Tree-->
					</td>
				</tr>
			</table>
		</td>
		<td width="100%" height="100%">
			<table class="tbform" height="100%" cellspacing="1" cellpadding="3" width="100%" border="0">
				<tr>
					<td class="tdform">
						<p id="headPagePath">
							<a title="Просмотр страницы" id="lnkPreviewPage"><img src="<%=sImages%>preview.gif"> <span id="headPagePathText"></span></a>&nbsp;&nbsp;
							<a title="Удалить элемент" id="lnkDeletePage" href="javascript:DeleteItem();"><img border="0" src="<%=sImages%>delete_red.gif">
								Удалить</a>&nbsp;&nbsp; <a href="javascript: DisplayTrash();" title="Корзина"><img src='<%=sImages%>trash_old.gif' id="imgTrash">
								Корзина</a>
						</p>
					</td>
				</tr>
				<tr height="100%">
					<td valign="top" class="tdform" style="PADDING-RIGHT:0px;PADDING-LEFT:0px;PADDING-BOTTOM:0px;PADDING-TOP:0px">
						<!--Content-->
						<div class="content">
							<!--Action-->
							<sota:xmlform runat="server" id="xfTree">
							</sota:xmlform>
							<sota:xmlform runat="server" id="xfDelete">
							</sota:xmlform>
							<sota:xmlform runat="server" id="xfTrash">
							</sota:xmlform>
							<sota:xmlform runat="server" id="xfSynchr">
							</sota:xmlform>
							<!--/Action-->
							<!--Item-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Item');">
										<img src="<%=sImages%>nolines_minus.gif" id="imgItem"> Общие</th></tr>
								<tr id="trItem">
									<td class="form">
										<sota:xmlform runat="server" id="xfItem" onactivate="ctrlSEvent='SaveItem();';" ondeactivate="ctrlSEvent='';">
											<input id="hId" type="hidden" value="-2" runat="server"> <input id="hIndex" type="hidden" value="-1" runat="server">
											<table width="100%" border="0">
												<tr>
													<td width="150px" nowrap><a id="aUrl" target="_blank"><b>Ссылка в меню:</b></a></td>
													<td width="100%"><input id="txtUrl" style="WIDTH: 100%" onpropertychange="getE('aUrl').href=this.value;"
															type="text" onchange="getE('aUrl').href=this.value;" runat="server"></td>
												</tr>
												<tr>
													<td><b>Название:</b></td>
													<td><input id="txtText" style="WIDTH: 100%" type="text" 
															onchange="mustRefreshTree=true;mustRefreshParents=true;" 
															onkeydown="mustRefreshTree=true;mustRefreshParents=true;"
															runat="server"></td>
												</tr>
												<tbody id="tOnlyChild">
													<tr>
														<td><b>Путь:</b></td>
														<td><input id="txtPath" style="WIDTH: 100%" type="text" runat="server" maxlength="100" onchange="CheckPageName(this);"></td>
													</tr>
													<tr>
														<td>Родитель:</td>
														<td valign="top">
															<select id="cmbParent" onchange="cmbParent_OnChange(this);"
																runat="server">
															</select>
															<a title="Обновить список" href="javascript: RefreshParents();"><img class="refresh_button" src='<%=ResolveUrl("~/admin.ashx?img=trash_refresh.gif")%>'></a>
														</td>
													</tr>
													<tr>
														<td>Тип:</td>
														<td valign="top">
															<select id="cmbType" onchange="TypeChanged();mustRefreshTree=true;mustRefreshParents=true;"
																runat="server">
																<option value="Page" selected>Страница</option>
																<option value="Link">Ссылка</option>
															</select>
													<tr>
														<td>Показывать в меню:</td>
														<td><input id="chkShow" type="checkbox" checked onchange="mustRefreshTree=true;" runat="server"></td>
													</tr>
													<tr>
														<td>Порядок:</td>
														<td><select id="cmbIndex" onchange="mustRefreshTree=true;mustRefreshParents=true;" runat="server"></select></td>
													</tr>
												</tbody>
												<tbody id="tOnlyPage">
													<tr>
														<td>Раздел безопасности:</td>
														<td><select id="cmbPart" runat="server"></select></td>
													</tr>
													<tr>
														<td>Протокол:</td>
														<td>
															<input id="chkSecure1" onclick="ChangeCaption();" type="radio" value="0" name="chkSecure"
																runat="server"> <label for="<%=chkSecure1.ClientID%>">http</label> <input id="chkSecure2" onclick="ChangeCaption();" type="radio" value="1" name="chkSecure"
																runat="server"> <label for="<%=chkSecure2.ClientID%>">https</label>
														</td>
													</tr>
													<tr>
														<td>Шаблон:</td>
														<td><select id="cmbTemplate" runat="server"></select></td>
													</tr>
													<tr>
														<td>Программа:</td>
														<td><input id="txtCodeFile" type="text" runat="server"><input onclick='ShowFileExplorer("","<%=txtCodeFile.ClientID%>","ascx");' type="button" value="..."></td>
													</tr>
													<tr>
														<td>Недоступна:</td>
														<td><input id="chkDeleted" type="checkbox" runat="server"></td>
													</tr>
												</tbody>
												<tr>
													<td align="center" colspan="2"><input onclick="SaveItem();" type="button" value="Сохранить">
														<input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Item-->
							<br>
							<!--Content-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Content');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgContent"> Содержимое</th></tr>
								<tr id="trContent" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfContent" runat="server" onactivate="ctrlSEvent='SubmitForm(\''+this.id+'\');';" ondeactivate="ctrlSEvent='';">
											<table height="100%" width="100%" border="0">
												<tr>
													<td width="100%" height="100%"><textarea class="allow_tab_key" id="txtBody" style="WIDTH: 100%; HEIGHT: 200px; overflow:auto;" wrap="off" runat="server"></textarea>
													</td>
												</tr>
												<tr>
													<td align="center">
														<input onclick="SubmitForm(this.form.id);" type="button" value="Сохранить"> <input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
														<input onclick="OpenHtmlEditor('<%=txtBody.ClientID%>');" type="button" value="Редактировать...">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Content-->
							<br>
							<!--Head-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Head');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgHead"> Head</th></tr>
								<tr id="trHead" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfHeader" runat="server" onactivate="ctrlSEvent='SubmitForm(\''+this.id+'\');';" ondeactivate="ctrlSEvent='';">
											<table border="0" style="WIDTH: 100%">
												<tr>
													<td>Заголовок:</td>
													<td style="WIDTH: 100%"><input id="txtTitle" style="WIDTH: 100%" type="text" runat="server"></td>
												</tr>
											<tr>
													<td nowrap>Ключевые слова:</td>
													<td><input id="txtKeyWords" type="text" runat="server"> <input onclick="ShowWordsStat();" type="button" value="Анализ..."></td>
												</tr>
												<tr>
													<td>Описание:</td>
													<td><input id="txtDescription" style="WIDTH: 100%" type="text" runat="server"></td>
												</tr>
												<tr>
													<td>Head:</td>
													<td><textarea id="txtHead" style="WIDTH: 100%" runat="server"></textarea></td>
												</tr>
												<tr>
													<td>Header:</td>
													<td><textarea wrap="off" style="float:left;" id="txtHeader" runat="server"></textarea>
													<div style="float:left;font-weight:bold;padding-left:5px;">
														<a href="javascript:InsertHeader('Location','/');XmlForm_SetSelectValue(getE('<%=cmbStatus.ClientID%>'),301);">Редирект</a><br>
													 </div>
													 <script type="text/javascript">
														function InsertHeader(h,v)
														{
															var txt = getE('<%=txtHeader.ClientID%>');
															txt.setActive();
															if(txt.value)
															{
																txt.value += '\n';
															}
															txt.value += h+': '+v;
															txt.setActive();
														}
													 </script>
													 </td>
												</tr>
												<tr>
													<td>Статус:</td>
													<td><select runat="server" id="cmbStatus">
													<option value="0">&nbsp;</option>
													<option value="204">204 Нет содержимого</option>
													<option value="301">301 Постоянный редирект</option>
													<option value="302">302 Временный редирект</option>
													<option value="303">303 Редирект с методом GET</option>
													<option value="304">304 Нет изменений</option>
													<option value="305">305 Использовать прокси</option>
													<option value="307">307 Редирект с тем же методом</option>
													<option value="400">400 Ошибочный запрос</option>
													<option value="401">401 Требует аутентификации</option>
													<option value="403">403 Отказано в доступе</option>
													<option value="404">404 Не найдена</option>
													</select></td>
												</tr>
												<tr>
													<td>Last Modified:</td>
													<td><select runat="server" id="cmbLM">
													<option value="0">Стандартное</option>
													<option value="1">Текущее время</option>
													<option value="2">Из файлов</option>
													<option value="3">Не указывать</option>
													</select></td>
												</tr>
												<tr>
													<td>Кэшировать на:</td>
													<td><input id="txtCache" type="text" onchange="CheckNumber(this);" size="10" runat="server">&nbsp;сек.</td>
												</tr>
												<tr>
													<td align="center" colspan="2">
														<input onclick="SubmitForm(this.form.id);" type="button" value="Сохранить"> <input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Head-->
							<br>
							<!--Field-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Field');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgField"> Словарь</th></tr>
								<tr id="trField" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfDict" runat="server" onactivate="ctrlSEvent='SaveField();';" ondeactivate="ctrlSEvent='';">
											<input id="hFieldAction" type="hidden" value="0" runat="server">
											<table height="100%" width="100%" border="0">
												<tr>
													<td><select id="lstFields" onchange="OpenField(this.value);" runat="server"></select>
														<input onclick="RefreshFieldsList();" type="button" value="Обновить список">
													</td>
												</tr>
												<tr>
													<td>Название:</td>
												</tr>
												<tr>
													<td><input id="txtFieldName" style="WIDTH: 100%" type="text" onchange="CheckFieldName(this);"
															runat="server">
													</td>
												</tr>
												<tr>
													<td>Содержание:</td>
												</tr>
												<tr>
													<td width="100%" height="100%"><textarea class="allow_tab_key" id="txtFieldValue" style="WIDTH: 100%; HEIGHT: 150px; overflow:auto;" wrap="off" runat="server"></textarea>
													</td>
												</tr>
												<tr>
													<td nowrap>
														<input onclick="SaveField();" type="button" value="Сохранить метку"> <input id="btnDelete" onclick="DeleteField();" type="button" value="Удалить метку" name="btnDelete">
														<input onclick="OpenHtmlEditor('<%=txtFieldValue.ClientID%>');" type="button" value="Редактировать содержание...">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Field-->
							<%if(IsAdmin){%>
							<br>
							<!--Template-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Template');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgTemplate"> Шаблон</th></tr>
								<tr id="trTemplate" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfTemplate" runat="server" onactivate="ctrlSEvent='SubmitForm(\''+this.id+'\');';" ondeactivate="ctrlSEvent='';">
											<table height="100%" width="100%" border="0">
												<tr>
													<td><input id="txtTemplatePath" style="WIDTH: 100%" readonly type="text" runat="server"></td>
												</tr>
												<tr>
													<td width="100%" height="100%"><textarea class="allow_tab_key" id="txtTemplateBody" style="WIDTH: 100%; HEIGHT: 200px" wrap="off" runat="server"></textarea></td>
												</tr>
												<tr>
													<td>
														<input onclick="SubmitForm(this.form.id);" type="button" value="Сохранить"> <input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Template-->
							<br>
							<!--Code-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Code');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgCode"> Программа</th></tr>
								<tr id="trCode" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfCode" runat="server" onactivate="ctrlSEvent='SubmitForm(\''+this.id+'\');';" ondeactivate="ctrlSEvent='';">
											<table height="100%" width="100%" border="0">
												<tr>
													<td><input id="txtCodePath" style="WIDTH: 100%" readonly type="text" name="txtcodepath" runat="server"></td>
												</tr>
												<tr>
													<td width="100%" height="100%"><textarea class="allow_tab_key" id="txtCodeBody" style="WIDTH: 100%; HEIGHT: 200px" name="txtCodeBody" wrap="off"
															runat="server"></textarea></td>
												</tr>
												<tr>
													<td>
														<input onclick="SubmitForm(this.form.id);" type="button" value="Сохранить"> <input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Code-->
							<br>
							<!--Pagex-->
							<table width="100%" class="item" cellspacing="0" cellpadding="2">
								<tr>
									<th onclick="PanelClick('Pagex');">
										<img src="<%=sImages%>nolines_plus.gif" id="imgPagex"> Перезапись URL</th></tr>
								<tr id="trPagex" style="DISPLAY:none">
									<td class="form">
										<sota:xmlform id="xfPagex" runat="server" beforesubmit="ChangeAction(this);" onactivate="ctrlSEvent='SubmitForm(\''+this.id+'\');';" ondeactivate="ctrlSEvent='';">
											<input id="hPagexAction" type="hidden" value="0" name="hPagexAction" runat="server">
											<table width="100%" border="0">
												<tr>
													<td><input id="txtPagex" style="WIDTH: 100%" type="text" name="txtPagex" runat="server"></td>
												</tr>
												<tr>
													<td>
													<input onclick="SubmitForm(this.form.id);" type="button" value="Сохранить"> 
													<input onclick="RefreshXmlForm(this.form.id);" type="button" value="Обновить">
													</td>
												</tr>
											</table>
										</sota:xmlform>
									</td>
								</tr>
							</table>
							<!--/Pagex-->
							<%}%>
						</div>
						<!--/Content-->
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<script type="text/javascript">
<!--
window.onload = function()
{
	CheckTrash();
	RefreshTheTree();
	CloseAll();
	RefreshParents();
	CreateItem(-1,<%=sm.Items.Count%>);
	treeview.openTo(<%=sm.All.Count+1%>,true);
}
-->
</script>