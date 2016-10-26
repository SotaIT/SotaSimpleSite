<%@ Control Language="c#" AutoEventWireup="false" Codebehind="controls.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.controls" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<script type="text/javascript" src="<%=sScript%>common.js"></script>
<script type="text/javascript" src="<%=sScript%>xmlform.js"></script>
<script type="text/javascript">
<!--
function SubmitForm()
{
	getE('cmbControl').form.submit();
}
function ObjectChanged()
{	
	var s = getE('cmbObject').value;
	getE('cmbPage').style.display = s=='page' ? '' : 'none';
	getE('cmbTemplate').style.display = s=='template' ? '' : 'none';
	RefreshPH();
}
function RefreshPH()
{
	var q = getE('cmbObject').value=='template' ? '?template='+getE('cmbTemplate').value : '?page='+getE('cmbPage').value;
	var lst = getE('cmbPH');
	lst.options.length=0;
	XmlForm_FillList(lst.form, lst, '<%=Path.Full.Split('?')[0]%>'+q);
}
function TemplateChange()
{
	RefreshPH();
}
function PageChange()
{
	RefreshPH();
}
//-->
</script>
<script type="text/javascript" for="window" event="onload">
<!--
if(getE('cmbObject'))
	ObjectChanged();
//-->
</script>
&nbsp;<a href="?new=">Добавить</a><br><br>
<form method="post" action="<%=Path.Full%>">
	<asp:datagrid id="dgControls" width="100%" bordercolor="#0000C0" borderstyle="Double" borderwidth="3px" cellpadding="4" runat="server" autogeneratecolumns="False">
		<alternatingitemstyle backcolor="#EEEEEE">
		</alternatingitemstyle>

		<headerstyle font-bold="True" forecolor="White" backcolor="Blue">
		</headerstyle>

		<columns>
			<asp:templatecolumn headertext="Объект">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "template").ToString()==string.Empty ? "Страница" : "Шаблон" %>

				</itemtemplate>

				<edititemtemplate>
					<select name="cmbObject" id="cmbObject" onchange="ObjectChanged();">
						<%# "<option value=\"template\""+(DataBinder.Eval(Container.DataItem, "template").ToString()==string.Empty ? "" : " selected")+">Шаблон</option>" %>
						<%# "<option value=\"page\""+(DataBinder.Eval(Container.DataItem, "page").ToString()==string.Empty ? "" : " selected")+">Страница</option>" %>
					</select>

				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn headertext="Название">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "page").ToString()+GetTemplateName(DataBinder.Eval(Container.DataItem, "template").ToString()) %>
				</itemtemplate>

				<edititemtemplate>
					<nobr>
						<select onchange="TemplateChange();" name="cmbTemplate" id="cmbTemplate"><%# GetTemplates(DataBinder.Eval(Container.DataItem, "template").ToString())%></select>
						<select onchange="PageChange();" name="cmbPage" id="cmbPage"><%#GetPages(DataBinder.Eval(Container.DataItem, "page").ToString())%></select></nobr>
				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn headertext="Метка">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "placeholder") %>

				</itemtemplate>

				<edititemtemplate>
					<select name="cmbPH" id="cmbPH">
						<option value='<%# DataBinder.Eval(Container.DataItem, "placeholder") %>'><%# DataBinder.Eval(Container.DataItem, "placeholder") %></option>
					</select>

				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn headertext="Блок">
				<headerstyle horizontalalign="Center">
				</headerstyle>

				<itemtemplate>
					<%# GetControlName(DataBinder.Eval(Container.DataItem, "control").ToString()) %>

				</itemtemplate>

				<edititemtemplate>
					<select name="cmbControl" id="cmbControl"><%# GetControls(DataBinder.Eval(Container.DataItem, "control").ToString())%></select>

				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn headertext="Запретить" itemstyle-width="50">
				<headerstyle horizontalalign="Center">
				</headerstyle>
				<itemstyle horizontalalign="Center">
				</itemstyle>
				<itemtemplate>
				<%# DataBinder.Eval(Container.DataItem, "allow").ToString()=="0" ? "Да" : "Нет" %>
				</itemtemplate>
				<edititemtemplate>
								<%# "<input type='checkbox' name='chkDisallow'"+(DataBinder.Eval(Container.DataItem, "allow").ToString()=="0" ? " checked" : "")+"/>" %>
				</edititemtemplate>
				</asp:templatecolumn>
			<asp:templatecolumn headertext="Порядок" itemstyle-width="50">
				<headerstyle horizontalalign="Center">
				</headerstyle>
				<itemstyle horizontalalign="Center">
				</itemstyle>

				<itemtemplate>
					<%# DataBinder.Eval(Container.DataItem, "order") %>

				</itemtemplate>

				<edititemtemplate>
					<input style="width: 30px" type="text" name="txtOrder" id="txtOrder" value='<%# DataBinder.Eval(Container.DataItem, "order") %>'>

				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn>
				<itemstyle horizontalalign="Center">
				</itemstyle>

				<itemtemplate>
					<a href='?edit=<%#Container.ItemIndex%>' title='Редактировать'><img src='<%=img%>/edit.gif'></a>


				</itemtemplate>

				<edititemtemplate>
					<a href='javascript:SubmitForm();' title="Сохранить"><img src="<%=img%>/save.gif"></a>
					<a href='?' title="Отменить"><img src="<%=img%>/cancel.gif"></a>


				</edititemtemplate>
			</asp:templatecolumn>
			<asp:templatecolumn>
				<itemstyle horizontalalign="Center">
				</itemstyle>

				<itemtemplate>
					<a href='?delete=1&<%# "template="+DataBinder.Eval(Container.DataItem, "template")+"&page="+DataBinder.Eval(Container.DataItem, "page")+"&placeholder="+DataBinder.Eval(Container.DataItem, "placeholder")+"&control="+DataBinder.Eval(Container.DataItem, "control") %>' title="Удалить" onclick="return confirm('Вы действительно хотите удалить строку?');"><img src="<%=img%>/remove.gif"></a>

				</itemtemplate>
			</asp:templatecolumn>
		</columns>
	</asp:datagrid>
</form>
