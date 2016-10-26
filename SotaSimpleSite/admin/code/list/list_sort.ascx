<%@ Control Language="c#" %>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%@ Import Namespace="Sota.Data.Simple"%>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.Text.RegularExpressions"%>
<%@ Register TagPrefix="sota" Namespace="Sota.Web.UI.WebControls" Assembly="SotaSimpleSite"%>
<%@ Register TagPrefix="sss" Namespace="Sota.Web.SimpleSite" Assembly="SotaSimpleSite"%>
<script runat="server">
	string list = "";
	void Page_Load(object o, EventArgs e)
	{
		list = Request.QueryString["list"];
		List l = List.Create(list);
		l.Data.Columns["_id"].Caption = "[ID]";
		l.Data.Columns["_deleted"].Caption = "[Не показывать]";
		string[] ignoreCols = { "_level", "_pid", "_guid" };
		ArrayList arr = new ArrayList();
		foreach (DataColumn col in l.Data.Columns)
		{
			bool ignore = false;
			for (int i = 0; i < ignoreCols.Length; i++)
			{
				if (col.ColumnName == ignoreCols[i])
				{
					ignore = true;
					break;
				} 
			}
			if (ignore)
			{
				continue; 
			}
			arr.Add(col);
		}
		rptFields.DataSource = arr;
		rptFields.DataBind();
	}
</script>
<style type="text/css">
table {margin:10px;}
table td {vertical-align:top;text-align:left;}
</style>

<form method="post">
<table>
<tr>
<td>
<select id="selFields" style="width:177px">
<asp:Repeater runat="server" ID="rptFields">
<ItemTemplate><option value="<%# Eval("ColumnName") %>" title="<%# Eval("Caption").ToString().Replace("\"","&quot;") %>"><%# Eval("Caption") %></option></ItemTemplate>
</asp:Repeater>
</select>

<select id="selAscDesc">
<option value="ASC">По возрастанию</option>
<option value="DESC">По убыванию</option>
</select>
</td>
<td><input type="button" value="Добавить" onclick="AddField()" id="btnAdd" />
</td>
</tr>
<tr>
<td><select id="selAll" multiple="multiple" size="10" style="width:300px" onclick="CheckAllFields()"></select>
</td>
<td style="vertical-align:middle"><input type="button" value="Удалить" id="btnDel" disabled="disabled" onclick="DeleteFields()" />
</td>
</tr>
<tr>
<td><input type="button" value="Сортировать" onclick="Save()" /> 
&nbsp;
&nbsp;
&nbsp;
&nbsp;
<input type="button" value="Отмена" onclick="Cancel()" /></td>
<td></td>
</tr>
</table>


</form>

<script type="text/javascript">
	var selFields = document.getElementById('selFields');
	var selAscDesc = document.getElementById('selAscDesc');
	var selAll = document.getElementById('selAll');
	var btnAdd = document.getElementById('btnAdd');
	var btnDel = document.getElementById('btnDel');

	function ParseCurrentValue() {
		var v = window.dialogArguments.value.toLowerCase();
		var arr = new Array();
		for (var i = 0; i < selFields.options.length; i++) {
			var opt = selFields.options[i];
			var j = v.indexOf('[' + opt.value.toLowerCase() + '] ');
			if (j != -1) {
				arr[j] = opt;
				//selFields.selectedIndex = i;
				//AddField();
			}
		}
		for (var i = 0; i < arr.length; i++) {
			if (arr[i]) {

				var sort = selAscDesc.options[
					v.indexOf(
						'[' + arr[i].value.toLowerCase() + '] asc'
						) == -1
					? 1
					: 0
				];
				
			
				var newOpt = document.createElement('option');
				newOpt.innerHTML = arr[i].text + ' [' + sort.text + ']';
				newOpt.value = arr[i].value + ' ' + sort.value;
				selAll.appendChild(newOpt);

				selFields.removeChild(arr[i]);
			}
		}

	}

	
	function AddField() {
		var i = selFields.selectedIndex;
		if (i == -1) {
			return;
		}
		var opt = selFields.options[i];

		var newOpt = document.createElement('option');
		newOpt.innerHTML = opt.text + ' [' + selAscDesc.options[selAscDesc.selectedIndex].text + ']';
		newOpt.value = opt.value + ' ' + selAscDesc.value;
		selAll.appendChild(newOpt);
		
		selFields.options[i] = null;
		
		if (selFields.options.length == 0) {
			btnAdd.disabled =
				selFields.disabled =
				selAscDesc.disabled = true;
		}
	}

	function CheckAllFields() {
		btnDel.disabled = selAll.selectedIndex == -1;
	}

	function DeleteFields() {
		for (var i = selAll.options.length; i > 0; i--) {
			var o = selAll.options[i - 1];
			if (o.selected) {
				var newO = document.createElement('option');
				newO.innerHTML = o.text.substring(0, o.text.lastIndexOf(' ['));
				newO.value = o.value.split(' ')[0];
				selFields.appendChild(newO);
				selAll.options[i - 1] = null;
			}
		}
		if (selFields.options.length > 0) {
			btnAdd.disabled =
				selFields.disabled =
				selAscDesc.disabled = false;
		}
	}

	function Save() {
		var s = '';
		for (var i = 0; i < selAll.options.length; i++) {
			if (i > 0)
				s += ', ';
			var o = selAll.options[i].value.split(' ');
			s += '[' + o[0] + '] ' + o[1];
		}
		window.returnValue = s;
		window.close();
	}
	function Cancel() {
		window.returnValue = null;
		window.close();
	}
	ParseCurrentValue();
</script>