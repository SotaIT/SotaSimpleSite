<%@ Control Language="C#" %>
<%@ Import Namespace="Sota.Web.SimpleSite" %>
<script runat="server">
	void Page_Load(object o, EventArgs e)
	{
		if (Request.QueryString["new-page"] == "1")
		{ 
		}
		else if (Request.QueryString["edit-page"] == "1")
		{
		}
		else
		{
			this.Visible = false; 
		}
	}
</script>

<style type="text/css">
#page-content-editor
{
	margin: 10px auto;
	padding: 0;
	border: 0;
	border-collapse:collapse;
	width:auto;
}
#page-content-editor td,
#page-content-editor th
{
	padding: 5px;
}
</style>
<script type="text/javascript">
	var page_content_editor_ru2en = {
		ru_str: 'АБВГДЕЁЖЗИЙКЛМНОПРСТУФХЦЧШЩЪЫЬЭЮЯабвгдеёжзийклмнопрстуфхцчшщъыьэюяQWERTYUIOPASDFGHJKLZXCVBNMqwertyuiopasdfghjklzxcvbnm1234567890"',
		en_str: ['A', 'B', 'V', 'G', 'D', 'E', 'YO', 'ZH', 'Z', 'I', 'Y', 'K', 'L', 'M', 'N', 'O', 'P', 'R', 'S', 'T',
 'U', 'F', 'KH', 'C', 'CH', 'SH', 'SHH', '', 'I', '', 'E', 'YU',
 'YA', 'a', 'b', 'v', 'g', 'd', 'e', 'yo', 'zh', 'z', 'i', 'y', 'k', 'l', 'm', 'n', 'o', 'p', 'r', 's', 't', 'u', 'f',
 'kh', 'c', 'ch', 'sh', 'shh', '', 'i', '', 'e', 'yu', 'ya',
 'Q', 'W', 'E', 'R', 'T', 'Y', 'U', 'I', 'O', 'P', 'A', 'S', 'D', 'F', 'G', 'H', 'J', 'K', 'L', 'Z', 'X', 'C', 'V', 'B', 'N', 'M',
 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm',
 '1', '2', '3', '4', '5', '6', '7', '8', '9', '0'
 ],
		translit: function (org_str) {
			var tmp_str = "";
			for (var i = 0, l = org_str.length; i < l; i++) {
				var s = org_str.charAt(i), n = this.ru_str.indexOf(s);
				if (n >= 0) { tmp_str += this.en_str[n]; }
				else { tmp_str += '-'; }
			}
			tmp_str = tmp_str.replace(/-+/g, '-');

			if (tmp_str.charAt(0) == '-') {
				tmp_str = tmp_str.substr(1);
			}
			if (tmp_str.charAt(tmp_str.length - 1) == '-') {
				tmp_str = tmp_str.substr(0, tmp_str.length - 1);
			}

			return tmp_str;
		}
	}

	function page_content_editor_title_change() {
		var tt = document.getElementById('<%=ClientID %>_txtTitle');
		var tu = document.getElementById('<%=ClientID %>_txtUrl');
		if (tu.value == '') {
			tu.value = page_content_editor_ru2en.translit(tt.value);
		}
	}

</script>

<form method="post" action="_self" enctype="multipart/form-data">
<table id="page-content-editor">
<tr>
<th>Заголовок:</th>
</tr>
 <tr><td><input type="text" name="<%=ClientID %>_txtTitle" id="<%=ClientID %>_txtTitle" size="50" value="<%=page_title %>" onchange="page_content_editor_title_change();" /></td></tr>
<tr>
<th>Адрес:</th>
</tr>
 <tr><td><%=Path.ARoot %><input type="text" name="<%=ClientID %>_txtUrl" id="<%=ClientID %>_txtUrl" size="50" value="<%=page_path %>" /></td></tr>

<tr>
<th>Текст:</th>
</tr>
 <tr><td><%=Path.ARoot %><textarea" name="<%=ClientID %>_txtText" id="<%=ClientID %>_txtText" size="50" value="<%=page_body %>"></textareatype></td></tr>
 


</table>
</form>