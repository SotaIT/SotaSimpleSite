<%@ Control Language="c#" AutoEventWireup="false" Codebehind="file_manager.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.SotaWebFileExplorer" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%string Img = ResolveUrl("~/admin.ashx?img=explorer");%>
<%string downloadPage = ResolveUrl(Sota.Web.SimpleSite.Config.Main.DownloadPage);%>
<style type="text/css"> 
<!-- 
	.divExplorer { background-color:#ffffff; OVERFLOW:auto; HEIGHT:100%; WIDTH:100%; border: 1px solid #cccccc;}
	.divExplorer img { border: 0px; vertical-align: middle; }
	.divExplorer a { color: #0000ff; text-decoration:none; }
	.divExplorer a:hover { text-decoration:underline; }
	.divImgPreview{background-color:#ffffff; OVERFLOW:auto; HEIGHT:180px; WIDTH:200px; border: 1px solid #cccccc; }
	.divImgPreview img {border: 1px solid #cccccc; }
	.file_size{font-size:smaller; color: #bbbbbb;}
	.tbFileManager { background-color:#eeeeee; border: 1px solid #cccccc;}
	td.toolbar a {	color: #0000ff;	}
	form{display:inline;padding:0;margin:0;}
	--> 
</style>
<script type="text/javascript">
<!--
function getE(id)
{
	return document.getElementById(id);
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
		if(s)
		{
			var m = parseInt(s);
			if(m)
				el.value = m;
			else
				el.value = 1;
		}
		else
			el.value = 1;
	}
}
function endsWith(str1, str2)
{
	var i = str1.lastIndexOf(str2);
	if(i>-1)
	{
		if(str1.length==(i+str2.length))
			return true;
	}
	return false;
}
function isImage(f)
{
	var arE = new Array(<%for(int i=0;i<arImgTypes.Length;i++){if(i>0) Response.Write(","); Response.Write("'"+arImgTypes[i]+"'");}%>);
	var n = arE.length;
	var s = f.toLowerCase();
	for(var i=0;i<n;i++)
	{
		if(endsWith(s,arE[i]))
			return true;
	}
	return false;
}
function ChangePrW()
{
	ChangePrS(1);
}
function ChangePrH()
{
    ChangePrS(2);
}
function ChangePrS(wOrh)
{
	var w = getE('imgPreview').width;
	var h = getE('imgPreview').height;
	if(getE('chkConstraint').checked)
	{
		var wNew = getE('txtPreviewW').value;
		var hNew = getE('txtPreviewH').value;
		if(wOrh==1)
		{
			h	= Math.round((wNew/w)*h);
			w	= wNew;
		}
		else
		{
			w	= Math.round((hNew/h)*w);
			h	= hNew;
		}
	}
	else
	{
		if(wOrh==1)
			w = getE('txtPreviewW').value;
		else
			h = getE('txtPreviewH').value;
	}
	getE('imgPreview').width	= w;
	getE('imgPreview').height	= h;
	getE('txtPreviewW').value	= w;
	getE('txtPreviewH').value	= h;
	WriteNewName(w,h);
}
function ChooseFile(p)
{<%if(IsDialog){%>
	getE("txtChoice").value = p;<%}else{%>
	var arp		= p.split('/');
	var f		= arp[arp.length-1];
	var sUrl	= '<%=sCurPath%>'+f;
	<%}%>
	if(isImage(p))
	{	<%if(IsDialog){%>
		var arp		= p.split('/');
		var f		= arp[arp.length-1];
		var sUrl	= '<%=sCurPath%>'+f;<%}%>
		previewImage(sUrl);
	}
	else
	{
		previewImage();
		<%if(!IsDialog){
		%>window.open(sUrl);<%}%>
	}
}
function previewImage(f)
{
	var oImg	= new Image();
	if(f)
		oImg.src	= f;
	else
		oImg.src	= '<%=GetFileImage("_place_holder_")%>';
	var imgP	= getE("imgPreview");
	imgP.src	= oImg.src;
	if(oImg.width)
	{
		imgP.width	= oImg.width;
		imgP.height	= oImg.height;
	}
	if(f)
	{
		getE('frmPreviewTools').style.visibility = '';
		getE("txtPreviewW").value	= imgP.width;
		getE("txtPreviewH").value	= imgP.height;
		var ap =  imgP.src.split('/');
		var s = ap[ap.length-1];
		getE("txtOldName").value	= s;
		WriteNewName(imgP.width,imgP.height);
	}
	else
	{
		getE('frmPreviewTools').style.visibility = 'hidden';
		getE("txtPreviewW").value	= '';
		getE("txtPreviewH").value	= '';
		getE("txtNewName").value	= '';
	}
}
function WriteNewName(w,h)
{
	var s = getE("txtOldName").value;
	var i = s.lastIndexOf(".");
	getE("txtNewName").value	= s.substring(0,i)+'['+w+'x'+h+']'+s.substring(i);
}
function DownloadFile(file)
{
	//location.href = '<%=downloadPage%>?file='+file;
	window.open('<%=downloadPage%>?enc='+escape(file));
}
function HideTools()
{
	getE('tbTools').style.display='none';
	getE('tbWait').style.display='';
}
function DeleteFile(n, file)
{
	var arp		= file.split('/');
	var f		= arp[arp.length-1];
	if(confirm('Вы действительно хотите удалить файл \'' + n + '\'?'))
	{
		getE('hFile').value = file;
		getE('frmDeleteFile').submit();
	}
}
function DeleteFolder(n, folder)
{
	var arp		= folder.split('/');
	var f		= arp[arp.length-1];
	if(confirm('Вы действительно хотите удалить папку \'' + n + '\'?'))
	{
		getE('hFolder').value = folder;
		getE('frmDeleteFolder').submit();
	}
}
function CheckFileExists(frm, mode)
{
	var f = '';
	if(mode==1)
	{
		var arr = frm.elements['<%=idFile.UniqueID%>'].value.split('\\');
		f = arr[arr.length-1];	
	}
	else
	{
		f = frm.txtFileName.value;
		if(frm.cmbFileType.value != '*')
		{
			f+='.'+frm.cmbFileType.value.split('-')[0];
		}
	}
	f = f.toLowerCase();
	var arrTD = getE('tbExplorer').getElementsByTagName('a');
	for(var i=0;i<arrTD.length;i++)
	{
		if(arrTD[i].href)
		{
			var href = arrTD[i].href;
			var l = href.indexOf('ChooseFile(\'')
			if(l > -1)
			{
				var arrFN = href.substring('javascript:ChooseFile(\''.length, href.indexOf('\');')).split('/');
				if(f==arrFN[arrFN.length-1].toLowerCase())
				{
					if(confirm('Файл с именем "' + f + '" уже существует!\nЗаменить файл?'))
					{
						frm.hReplace.value = '1';
					}
					break;
				}
			}
		}
	}	
}
//-->
</script>
<%if(IsDialog){
switch(sHtmlEditor)
{ 

case "tinymce":
%>
<script language="javascript" type="text/javascript" src="tinymce/jscripts/tiny_mce/tiny_mce_popup.js">
</script>

<script language="javascript" type="text/javascript">

	var FileBrowserDialogue = {
		init: function() {
			// Here goes your code for setting your custom things onLoad.
		},
		mySubmit: function() {
		var URL = '<%=sRootPath%>' + document.getElementById("txtChoice").value;
			var win = tinyMCEPopup.getWindowArg("window");

			// insert information now
			win.document.getElementById(tinyMCEPopup.getWindowArg("input")).value = URL;

			// are we an image browser
			if (typeof (win.ImageDialog) != "undefined") {
				// we are, so update image dimensions and preview if necessary
				if (win.ImageDialog.getImageData) win.ImageDialog.getImageData();
				if (win.ImageDialog.showPreviewImage) win.ImageDialog.showPreviewImage(URL);
			}

			// close popup window
			tinyMCEPopup.close();
		}
	}

	tinyMCEPopup.onInit.add(FileBrowserDialogue.init, FileBrowserDialogue);


	function Ok_Click() {
		FileBrowserDialogue.mySubmit() ;
	}
	
	function Cancel_Click() {
		tinyMCEPopup.close();
	}
</script>
<%
break;//case "tinymce"

default:
%>
<script type="text/javascript">
function Ok_Click()
{
	if(opener)
	{
		if(<%=sOpenerField.Length%>)
		{
			opener.document.getElementById('<%=sOpenerField%>').value = document.getElementById("txtChoice").value;
		}
	}
	else
	{
		window.returnValue = document.getElementById("txtChoice").value;
	}
	window.close();
}
function Cancel_Click()
{
	window.close();
}
</script>
<%
break;//default
}//switch
}//if(IsDialog)
%>


<form  method="post" id="frmDeleteFile">
<input type="hidden" value="delete" name="act">
<input type="hidden" value="" name="hFile" id="hFile">
</form>
<form  method="post" id="frmDeleteFolder">
<input type="hidden" value="deletefolder" name="act">
<input type="hidden" value="" name="hFolder" id="hFolder">
</form>
<table height="100%" width="100%" border="0" class="tbFileManager">
	<%if(IsDialog){%><tr>
		<td class="toolbar">
			<a href="<%=Sota.Web.SimpleSite.Path.Full%>">Обновить</a>
		</td>
		<td>&nbsp;</td></tr><%}%>
	<tr height="100%">
		<td width="100%" height="100%">
			<div class="divExplorer">
				<table border="0" cellpadding="2" cellspacing="0" width="100%" id="tbExplorer">
					<tr>
						<td><a href="?field=<%=sOpenerField%>&filter=<%=sFilter%>&path=<%=Server.UrlEncode(sCurUpPath)%>&root=<%=Server.UrlEncode(sRootPath)%>&editor=<%=sHtmlEditor %>"><img src='<%=GetFileImage("_folder_up_")%>'>..</a> </td></tr>
					<%
int start = pageSize * pageNumber;
int end = start + pageSize;						
int nd = arDirs.Length;
int VisibleFolderCount = 0;
int FoldersShown = 0;
for (int i = 0; i < nd; i++)
{
	if (!HideAdminFolders || !IsHidden(arDirs[i].Name))
	{
		VisibleFolderCount++;
	}
}						
for (int i = start; i < nd && i < end; i++)
{
	if (!HideAdminFolders || !IsHidden(arDirs[i].Name))
	{
		FoldersShown++;
%>
					<tr>
						<td nowrap><a href="?field=<%=sOpenerField%>&filter=<%=sFilter%>&path=<%=Server.UrlEncode(GetWebPath(arDirs[i].FullName))%>&root=<%=Server.UrlEncode(sRootPath)%>&editor=<%=sHtmlEditor %>"><img src='<%=GetFileImage("_folder_")%>'><%=arDirs[i].Name%></a>
						&nbsp;&nbsp;<a title="Удалить папку '<%=arDirs[i].Name%>'" href="javascript:DeleteFolder('<%=arDirs[i].Name%>', '<%=GetWebPath(arDirs[i].FullName)%>');"><img src="<%=Img%>/delete_file.gif"></a>
						</td></tr>
					<%}
}
start = start - VisibleFolderCount + FoldersShown;
end = start + pageSize - FoldersShown;
if (start > -1)
{
	int n = arFiles.Length;
	for (int i = start; i < n && i < end; i++)
	{
		string sPath = GetWebPath(arFiles[i].FullName).Substring(sRootPath.Length).TrimStart('/');
%>
					<tr>
						<td nowrap><a href="javascript:ChooseFile('<%=sPath%>');"><img src="<%=GetFileImage(arFiles[i].Name)%>"><%=arFiles[i].Name%></a>
							<span class="file_size"><%=FormatBytes(arFiles[i].Length)%></span>
							<a title="Скачать" href="javascript:DownloadFile('<%=GetWebPath(arFiles[i].FullName)%>');"><img src="<%=Img%>/download.gif"></a>
							&nbsp;&nbsp;<a title="Удалить файл '<%=arFiles[i].Name%>'" href="javascript:DeleteFile('<%=arFiles[i].Name%>','<%=GetWebPath(arFiles[i].FullName)%>');"><img src="<%=Img%>/delete_file.gif"></a>
						</td></tr>
					<%}
}%>
				</table><%int AllItemsCount = arFiles.Length + VisibleFolderCount;
	  int pagesCount = (int)Math.Ceiling(AllItemsCount / (double)pageSize);        	
	  if(pagesCount > 1){ %>
<div style="padding: 5px 2px 10px 2px;margin-top:5px;border-top:1px dashed #eee;">
<%for (int i = 0; i < pagesCount; i++)
  {
	  int from = i * pageSize + 1;
	  int to = Math.Min(from + pageSize - 1, AllItemsCount);
	  string t = "[" + (from == to ? to.ToString() : from + "-" + to ) + "]";
	  if (i == pageNumber)
	  { %><b><%=t %></b> <%}
	  else
  {%>
  <a href="?root=<%=Server.UrlEncode(sRootPath) %>&path=<%=Server.UrlEncode(sCurPath) %>&filter=<%=sFilter %>&field=<%=sOpenerField %>&page=<%=i %>&psize=<%=pageSize %>&editor=<%=sHtmlEditor %>"
  ><%=t %></a>
<%}} %>
</div>
<%} %></div></td>
		<td valign="top" align="center">
			<table cellpadding="2" width="100">
				<tr>
					<td>
						<div class="divImgPreview">
							<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
								<tr>
									<td valign="middle" align="center"><img id="imgPreview" src='<%=GetFileImage("_place_holder_")%>'>
									</td></tr></table></div></td></tr>
				<tr>
					<td>
						<form method="post" id="frmPreviewTools" style="VISIBILITY:hidden;" onsubmit="if(this.txtNewName.value.length==0)return false;">
							<table cellspacing="0" cellpadding="1">
								<tr>
									<td style="font-size:9px">Ширина:</td>
									<td width="100%"><input name="txtPreviewW" id="txtPreviewW" type="text" onchange="CheckNumber(this);" size="5" style="font-size:9px"><input style="font-size:9px" onclick="ChangePrW();" type="button" value="ok"></td></tr>
								<tr>
									<td style="font-size:9px">Высота:</td>
									<td><input style="font-size:9px" name="txtPreviewH" id="txtPreviewH" type="text" onchange="CheckNumber(this);" size="5"><input style="font-size:9px" onclick="ChangePrH();" type="button" value="ok"></td></tr>
								<tr>
									<td colspan="2"><input style="vertical-align:middle" id="chkConstraint" type="checkbox" checked><label for="chkConstraint" style="font-size:9px">Пропорции</label></td></tr>
								
								<tr>
									<td style="font-size:9px">SM:</td><td>
									<select name='cmbSmoothingMode' style="font-size:9px">
									<%string[] arr = Enum.GetNames(typeof(System.Drawing.Drawing2D.SmoothingMode));
									for(int i=0;i<arr.Length;i++)
									{if(arr[i]!="Invalid"){%>
									<option value="<%=arr[i]%>"><%=arr[i]%></option>
									<%}}%>
									</select>
									</td></tr>
								<tr>
									<td style="font-size:9px">CQ:</td><td>
									<select name='cmbCompositingQuality' style="font-size:9px">
									<%arr = Enum.GetNames(typeof(System.Drawing.Drawing2D.CompositingQuality));
									for(int i=0;i<arr.Length;i++)
									{if(arr[i]!="Invalid"){%>
									<option value="<%=arr[i]%>"><%=arr[i]%></option>
									<%}}%>
									</select>
									</td></tr>
								<tr>
									<td colspan="2" nowrap style="font-size:9px">
										Сохранить как:<br>
										<input style="font-size:9px" type="text" name="txtNewName" id="txtNewName">
										<input type="hidden" name="txtOldName" id="txtOldName">
										<input style="font-size:9px" type="submit" value="ОК">
									</td>
								</tr>
							</table></form>
					</td></tr>
			</table>
		</td></tr>
	<tr>
		<td><%if(sError.Length>0){%>
			<script type="text/javascript" for="window" event="onload">
			<!--
				alert('Ошибка: <%=sError.Replace("'","\\'").Replace("\n","\\n").Replace("\r","\\r")%>');
			//-->
			</script><%}%>
			<table width="100%">
				<%if(IsDialog){%><tr>
					<td>Файл:</td>
					<td width="100%"><input id="txtChoice" style="WIDTH: 100%" type="text"> </td>
					<td nowrap><input onclick="Ok_Click();" type="button" value="ОК"> <input onclick="Cancel_Click();" type="button" value="Отмена"></td></tr>
				<tr><td colspan="3">&nbsp;</td></tr>
				<%}%>
			</table>
			<table width="100%" id="tbTools">
				<form method="post" enctype="multipart/form-data" onsubmit="if(this.elements['<%=idFile.UniqueID%>'].value.length==0)return false;CheckFileExists(this,1);HideTools();">
					<tr>
						<td nowrap>Загрузить файл:</td>
						<td width="100%"><input style="WIDTH: 100%" type="file" runat="server" id="idFile"></td>
						<td><input type="submit" value="Загрузить"><input type="hidden" name="hReplace" value="0" /></td></tr></form>
				<form method="post" enctype="multipart/form-data" onsubmit="if(this.elements['<%=idZipFile.UniqueID%>'].value.length==0)return false;HideTools();">
					<tr>
						<td nowrap>Загрузить ZIP:</td>
						<td width="100%"><input style="WIDTH: 100%" type="file" runat="server" id="idZipFile"></td>
						<td><input type="submit" value="Загрузить"></td></tr></form>

				
				<form method="post" onsubmit="if(this.txtFileName.value.length==0)return false;CheckFileExists(this);HideTools();">
					<tr>
						<td>Создать файл:</td>
						<td width="100%">
							<table cellpadding="1" cellspacing="0" border="0" width="100%">
								<tr>
									<td width="100%"><input style="WIDTH: 100%" type="text" name="txtFileName"></td>
									<td><select name="cmbFileType">
											<option value="*" selected></option>
											<option value="ascx">.ascx</option>
											<option value="aspx">.aspx</option>
											<option value="aspx-sota">.aspx(sota)</option>
											<option value="html">.html</option>
											<option value="xml">.xml</option>
											<option value="config">.config</option>
											<option value="config-list">.config(list)</option>
										</select><input type="hidden" name="hReplace" value="0" />
									</td>
								</tr>
							</table>
						</td>
						<td><input type="submit" value="Создать"></td></tr></form>
				<form method="post" onsubmit="if(this.txtFolderName.value.length==0)return false;HideTools();">
					<tr>
						<td>Создать папку:</td>
						<td><input style="WIDTH: 100%" type="text" name="txtFolderName"></td>
						<td><input type="submit" value="Создать"></td></tr></form>

			</table>
			<table width="100%" style="display:none" id="tbWait">
			<tr><td>&nbsp;</td></tr>
			<tr><td align="center" valign="middle">
			Пожалуйста, подождите...
			</td></tr>
			<tr><td>&nbsp;</td></tr>
			</table>
			</td>
		<td align="right" valign="bottom">SotaWebFileExplorer v1.0</td></tr></table>