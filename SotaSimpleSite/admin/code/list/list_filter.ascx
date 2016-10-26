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
<ItemTemplate><option value="[<%# Eval("ColumnName")%>] <%#((Type)Eval("DataType")==typeof(string) ? "LIKE" : "=") %>" title="<%# Eval("Caption").ToString().Replace("\"","&quot;") %>"><%# Eval("Caption") %></option></ItemTemplate>
</asp:Repeater>
</select>

<input type="text" id="txtValue" />

</td>
<td><input type="button" value="Добавить" onclick="AddField()" id="btnAdd" />
</td>
</tr>
<tr>
<td><select id="selAll" multiple="multiple" size="10" style="width:321px" onclick="CheckAllFields()"></select>
</td>
<td style="vertical-align:middle"><input type="button" value="Удалить" id="btnDel" disabled="disabled" onclick="DeleteFields()" />
</td>
</tr>
<tr>
<td><input type="button" value="Отфильтровать" onclick="Save()" /> 
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
	var txtValue = document.getElementById('txtValue');
	var selAll = document.getElementById('selAll');
	var btnAdd = document.getElementById('btnAdd');
	var btnDel = document.getElementById('btnDel');


	
	function AddField() {
		var i = selFields.selectedIndex;
		if (i == -1) {
			return;
		}
		var opt = selFields.options[i];

		var newOpt = document.createElement('option');
		newOpt.innerHTML = opt.text + '=' + txtValue.value;
		newOpt.value = opt.value + " '" + txtValue.value + "'";
		selAll.appendChild(newOpt);

		txtValue.value = '';
	}

	function CheckAllFields() {
		btnDel.disabled = selAll.selectedIndex == -1;
	}

	function DeleteFields() {
		for (var i = selAll.options.length; i > 0; i--) {
			var o = selAll.options[i - 1];
			if (o.selected) {
				selAll.options[i - 1] = null;
			}
		}
	}

	function Save() {
		var s = '';
		for (var i = 0; i < selAll.options.length; i++) {
			if (i > 0)
				s += ' AND ';
			s += selAll.options[i].value;
		}
		window.returnValue = s;
		window.close();
	}
	function Cancel() {
		window.returnValue = null;
		window.close();
	}
</script>