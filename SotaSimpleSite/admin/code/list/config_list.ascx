<%@ Control Language="c#" AutoEventWireup="false" Codebehind="config_list.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.list.config_list" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<%string sImages = ResolveUrl("~/admin.ashx?img=");%>
<link href="<%=ResolveUrl("~/admincss.ashx?css=")%>admin.map.css" type="text/css" rel="stylesheet">
<script src="<%=sScript%>xmlform.js" type=text/javascript></script>
<script src="<%=sScript%>common.js" type=text/javascript></script>
<script event="onload" for="window" type="text/javascript">
var s	= GetCookie('OpenPanels');
if(s)
{
	RemoveCookie('OpenPanels');
	var ar	= s.split(/[\[\]]+/);
	for(var i=0;i<ar.length;i++)
	{
		OpenPanel(ar[i]);
	}
}
InputTypeChanged();
</script>
<script type="text/javascript">
function SubmitForm()
{
	getE('txtFieldName').form.submit();
}
function SetConnection(p,s)
{
	XmlForm_SetSelectValue(getE('<%=cmbProvider.ClientID%>'), p);
	getE('<%=txtConstr.ClientID%>').value = s;

}
function PanelClick(id)
{
	var tr	= getE('tr'+id);
	var i	= getE('img'+id);
	var b	= tr.style.display =='none';
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
	var tr	= getE('tr'+id);
	var i	= getE('img'+id);
	tr.style.display = '';
	i.src = '<%=sImages%>nolines_minus.gif';
	var s = GetCookie('OpenPanels');
	if(s.indexOf('['+id+']')==-1)
		s+='['+id+']';
	RemoveCookie('OpenPanels');
	SetCookie('OpenPanels',s);
}
function ClosePanel(id)
{
	var tr	= getE('tr'+id);
	var i	= getE('img'+id);
	tr.style.display = 'none';
	i.src = '<%=sImages%>nolines_plus.gif';
	var s = GetCookie('OpenPanels');
	RemoveCookie('OpenPanels');
	SetCookie('OpenPanels', s.replace('['+id+']',''));
}
function CheckNumber(el)
{
	var pPattern = "[^0-9]+";
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
		if(s.length==0)
			s = 0; 
		if(s<0)
			s = 0;
	}
	el.value = s;
}
function ShowFileExplorer(id)
{
	var res = OpenModalDialog('<%=ResolveUrl(Config.Main.FileManagerPage)%>?field='+id+'&filter=gif;jpg;jpeg;png',null,650,550);
	if(res)
		getE(id).value = '<%=Keys.ServerRoot+Keys.UrlPathDelimiter%>'+res;
}
var arCustomProps = new Array('txtFieldExpression','txtFieldRegex','txtFieldUploads','txtFieldFilter','txtFieldListUrl');
function HideAllCustomProps()
{
	for(var i=0;i<arCustomProps.length;i++)
	{
		getE(arCustomProps[i]).style.display = 'none';
	}
}
function ShowCustomProp(n)
{
	getE(n).style.display = '';
}
function InputTypeChanged()
{
	var t = getE('cmbFieldInputType');
	if(t)
	{
		HideAllCustomProps();
		switch(t.value)
		{
			case "none":
				ShowCustomProp('txtFieldExpression');
				break;
			case "regex":
				ShowCustomProp('txtFieldRegex');
				break;
			case "multi":
			case "combobox":
				ShowCustomProp('txtFieldListUrl');
				break;
			case "file":
				ShowCustomProp('txtFieldUploads');
				ShowCustomProp('txtFieldFilter');
				break;
		}
	}
}
</script>
<style>DIV.coment { FONT-SIZE: smaller; COLOR: #aaaaaa }
	DIV.coment LI { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px; PADDING-TOP: 0px; LIST-STYLE-TYPE: none }
	DIV.coment UL { PADDING-RIGHT: 0px; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; MARGIN: 0px 0px 0px 10px; PADDING-TOP: 0px }
	DIV.coment LI A { CURSOR: hand; COLOR: #aaaaaa }
	INPUT.small { FONT-SIZE: smaller; WIDTH: 60px }
	SELECT.small { FONT-SIZE: smaller }
</style>
<table class="tbform" height="100%" cellspacing="1" cellpadding="1" width="100%" border="0">
	<tr>
		<td class="tdform">
			<div class="content">
				<table class="item" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<th onclick="PanelClick('List');">
							<img id=imgList src="<%=sImages%>nolines_plus.gif"> Подключение к БД</th></tr>
					<tr id="trList" style="DISPLAY: none">
						<td class="form">
							<table border="0">
								<tr>
									<td>Тип провайдера:</td>
									<td><select id="cmbProvider" runat="server">
											<option value="sql" selected>SQL Server</option>
											<option value="oledb">OLE DB</option>
											<option value="odbc">ODBC</option>
											<option value="oracle">Oracle</option>
											<option value="mysql">MySQL</option>
										</select>
									</td>
								</tr>
								<tr>
									<td nowrap>Строка подключения:
									</td>
									<td width="100%"><input id="txtConstr" style="WIDTH: 100%" type="text" runat="server"></td>
								</tr>
								<tr>
									<td colspan="2">
										<div class="coment"><b>SQL Server:</b>
											<ul>
												<li>
													<a onclick="SetConnection('sql',this.innerText)">Persist Security Info=False;Integrated Security=SSPI;database=northwind;server=mySQLServer
													</a>
												<li>
													<a onclick="SetConnection('sql',this.innerText)">User ID=myUserName;pwd=password;database=northwind;server=mySQLServer</a>
												</li>
											</ul>
											<b>OLE DB:</b>
											<ul>
												<li>
													<a onclick="SetConnection('oledb',this.innerText)">Provider=MSDAORA; Data
														Source=ORACLE8i7;Persist Security Info=False;Integrated Security=yes </a>
												<li>
													<a onclick="SetConnection('oledb',this.innerText)">Provider=Microsoft.Jet.OLEDB.4.0;
														Data Source=c:\bin\LocalAccess40.mdb </a>
												<li>
													<a onclick="SetConnection('oledb',this.innerText)">Provider=SQLOLEDB;Data
														Source=MySQLServer;Integrated Security=SSPI</a>
												</li>
											</ul>
											<b>ODBC:</b>
											<ul>
												<li>
													<a onclick="SetConnection('odbc',this.innerText)">Driver={SQLServer};Server=MyServer;Trusted_Connection=yes;Database=Northwind;</a>
												<li>
													<a onclick="SetConnection('odbc',this.innerText)">Driver={Microsoft ODBC for
														Oracle};Server= ORACLE8i7;Persist SecurityInfo=False;Trusted_Connection=yes</a>
												<li>
													<a onclick="SetConnection('odbc',this.innerText)">Driver={Microsoft Access Driver
														(*.mdb)};DBQ=c:\bin\nwind.mdb</a>
												<li>
													<a onclick="SetConnection('odbc',this.innerText)">DSN=dsnname</a>
												</li>
											</ul>
											<b>Oracle:</b>
											<ul>
												<li>
													<a onclick="SetConnection('oracle',this.innerText)">Data Source=Oracle8i;Integrated
														Security=yes</a>
												<li>
													<a onclick="SetConnection('oracle',this.innerText)">User
														ID=myUserName;password=password;Data Source=Oracle8i</a>
												</li>
											</ul>
											<b>MySQL:</b>
											<ul>
												<li>
													<a onclick="SetConnection('mysql',this.innerText)">Persist Security
														Info=False;database=MyDB;server=MySqlServer;user id=myUser;Password=myPass</a>
												</li>
											</ul>
										</div>
									</td>
								</tr>
								<tr>
									<td nowrap>Имя таблицы:
									</td>
									<td width="100%"><input id="txtTableName" style="WIDTH: 100%" type="text" runat="server"></td>
								</tr>

								<tr>
									<td><asp:button id="cmdSaveCommon" runat="server" text="Сохранить"></asp:button></td>
									<td>&nbsp;</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<br>
				<table class="item" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<th onclick="PanelClick('Editor');">
							<img id=imgEditor src="<%=sImages%>nolines_plus.gif"> Настройки
						</th>
					</tr>
					<tr id="trEditor" style="DISPLAY: none">
						<td class="form">
							<table width="100%">
								<tr>
									<td>Название списка:</td>
									<td width="100%"><input id="txtListCaption" type="text" runat="server" style="WIDTH: 100%" /></td>
								</tr>
								<tr>
									<td>Раздел безопасности:</td>
									<td width="100%">
									<select id="cmbSecurityPart" runat="server">
									<%=GetSecurityParts()%>
									</select>
									</td>
								</tr>
								<tr>
									<td>Количество уровней:</td>
									<td width="100%"><input id="txtLevels" type="text" onchange="CheckNumber(this);" size="5" value="1" runat="server" /></td>
								</tr>
								<tr>
									<td>Сортировка дерева:</td>
									<td><input id="txtSort" style="WIDTH: 100%" type="text" runat="server" /></td>
								</tr>
								<tr>
									<td nowrap="nowrap">Использовать стандартные иконки:</td>
									<td><input id="chkNoIcon" type="checkbox" runat="server" /></td>
								</tr>
								<tr>
									<td>Иконка корневого элемента:</td>
									<td><input id="txtRootIcon" type="text" runat="server" /><input onclick="ShowFileExplorer('<%=txtRootIcon.ClientID%>');" type=button value=...></td>
								</tr>
								<tr>
									<td>Иконка "Открыть все"</td>
									<td><input id="txtOpenAllIcon" type="text" runat="server"><input onclick="ShowFileExplorer('<%=txtOpenAllIcon.ClientID%>');" type=button value=...></td>
								</tr>
								<tr>
									<td>Иконка "Закрыть все"</td>
									<td><input id="txtCloseAllIcon" type="text" runat="server"><input onclick="ShowFileExplorer('<%=txtCloseAllIcon.ClientID%>');" type=button value=...></td>
								</tr>
								<tr>
									<td><asp:button id="cmdSaveEditor" runat="server" text="Сохранить"></asp:button></td>
									<td>&nbsp;</td>
								</tr>
							</table>
						</td>
					</tr>
				</table>
				<br>
				<table class="item" cellspacing="0" cellpadding="2" width="100%">
					<tr>
						<th onclick="PanelClick('Columns');">
							<img id=imgColumns src="<%=sImages%>nolines_plus.gif" /> Поля списка
						</th>
					</tr>
					<tr id="trColumns" style="DISPLAY: none">
						<td class="form">
							<br><a href="?newcolumn=1">Добавить поле</a><br><br>
							<asp:datagrid id="dgColumns" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px" borderstyle="Double" bordercolor="#0000C0" width="100%">
								<alternatingitemstyle backcolor="#EEEEEE"></alternatingitemstyle>
								<itemstyle backcolor="White"></itemstyle>
								<headerstyle font-bold="True" forecolor="White" backcolor="Blue"></headerstyle>
								<columns>
									<asp:templatecolumn headertext="Имя">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "fieldname") %>
										</itemtemplate>
										<edititemtemplate>
											<input name="hOldFieldName" type="hidden" value='<%# DataBinder.Eval(Container.DataItem, "fieldname") %>'>
											<input class="small" name="txtFieldName" type="text" value='<%# DataBinder.Eval(Container.DataItem, "fieldname") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Надпись">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "caption") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" name="txtFieldCaption" type="text" value='<%# DataBinder.Eval(Container.DataItem, "caption") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Тип">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "datatype") %>
										</itemtemplate>
										<edititemtemplate>
											<select class="small" name="cmbFieldDataType">
												<%# GetDataTypes(DataBinder.Eval(Container.DataItem, "datatype").ToString()) %>
											</select>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Контрол">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "inputtype") %>
										</itemtemplate>
										<edititemtemplate>
											<select class="small" onchange="InputTypeChanged()" name="cmbFieldInputType" id="cmbFieldInputType">
												<%# GetInputTypes(DataBinder.Eval(Container.DataItem, "inputtype").ToString()) %>
											</select>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Default">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "defaultvalue") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" name="txtFieldDefaultValue" type="text" value='<%# DataBinder.Eval(Container.DataItem, "defaultvalue") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Null">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "allownull").ToString()=="1" ? "Да" : "Нет" %>
										</itemtemplate>
										<edititemtemplate>
											<%# "<input type=\"checkbox\" name=\"chkFieldAllowNull\""+(DataBinder.Eval(Container.DataItem, "allownull").ToString()=="1" ? " checked" : "")+">" %>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Уровни">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "levels") %>
										</itemtemplate>
										<edititemtemplate>
											<%# GetLevelsChecks(DataBinder.Eval(Container.DataItem, "levels").ToString()) %>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Exp">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "expression") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" id="txtFieldExpression" name="txtFieldExpression" type="text" value='<%# DataBinder.Eval(Container.DataItem, "expression") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Regex">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "regex") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" id="txtFieldRegex" name="txtFieldRegex" type="text" value='<%# DataBinder.Eval(Container.DataItem, "regex") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Папка">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "uploads") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" id="txtFieldUploads" name="txtFieldUploads" type="text" value='<%# DataBinder.Eval(Container.DataItem, "uploads") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Фильтр">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "filter") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" id="txtFieldFilter" name="txtFieldFilter" type="text" value='<%# DataBinder.Eval(Container.DataItem, "filter") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn headertext="Url списка">
										<itemtemplate>
											<%# DataBinder.Eval(Container.DataItem, "listurl") %>
										</itemtemplate>
										<edititemtemplate>
											<input class="small" id="txtFieldListUrl" name="txtFieldListUrl" type="text" value='<%# DataBinder.Eval(Container.DataItem, "listurl") %>'>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn itemstyle-horizontalalign="Center" itemstyle-width="45px">
										<itemtemplate>
										<%# Container.ItemIndex==0 
											? "<img src='"+ResolveUrl("~/admin.ashx?img=1x1.gif")+"' width='19px' height='19px' />" 
											: "<a href='?upcolumn="+Container.ItemIndex+"' title='Вверх'><img src='"+ResolveUrl("~/admin.ashx?img=smile/icon_arrowu.gif")+"'></a>" %>
										<%# Container.ItemIndex==((System.Data.DataTable)dgColumns.DataSource).Rows.Count-1
											? "<img src='"+ResolveUrl("~/admin.ashx?img=1x1.gif")+"' width='19px' height='19px' />" 
											: "<a href='?downcolumn="+Container.ItemIndex+"' title='Вниз'><img src='"+ResolveUrl("~/admin.ashx?img=smile/icon_arrowd.gif")+"'></a>" %>
										</itemtemplate>
										<edititemtemplate>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn itemstyle-horizontalalign="Center" itemstyle-width="20px">
										<itemtemplate>
											<a href='?editcolumn=<%#Container.ItemIndex%>' title='Редактировать'><img src='<%=img%>/edit.gif'></a>
										</itemtemplate>
										<edititemtemplate>
											<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
											<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>
										</edititemtemplate>
									</asp:templatecolumn>
									<asp:templatecolumn itemstyle-horizontalalign="Center" itemstyle-width="20px">
										<itemtemplate>
											<a href='?deletecolumn=<%#DataBinder.Eval(Container.DataItem, "fieldname")%>' onclick="return confirm('Вы уверены, что хотите удалить поле?');" title='Удалить'>
												<img src='<%=img%>/remove.gif'></a>
										</itemtemplate>
									</asp:templatecolumn>
								</columns>
							</asp:datagrid>
						</td>
					</tr>
				</table><br>
				<table cellpadding=0 cellspacing=0>
				<tr>
				<td><asp:Button Runat="server" ID="cmdVerify" Text="Актуализировать структуру базы данных"></asp:Button></td>
				</tr>
				</table>
			</div>
		</td>
	</tr>
</table>
