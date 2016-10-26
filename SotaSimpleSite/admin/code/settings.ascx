<%@ Control Language="c#" AutoEventWireup="false" Codebehind="settings.ascx.cs" Inherits="Sota.Web.SimpleSite.admin.code.settings" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<style type="text/css">
.form { BORDER-RIGHT: 0px; BORDER-TOP: 0px; BORDER-LEFT: 0px; WIDTH: 100%; BORDER-BOTTOM: 0px; BORDER-COLLAPSE: collapse }
.form TD { BORDER-RIGHT: #00f 1px solid; PADDING-RIGHT: 3px; BORDER-TOP: #00f 1px solid; PADDING-LEFT: 3px; PADDING-BOTTOM: 3px; BORDER-LEFT: #00f 1px solid; PADDING-TOP: 3px; BORDER-BOTTOM: #00f 1px solid }
.form TD INPUT { BORDER-RIGHT: #ccc 1px solid; BORDER-TOP: #ccc 1px solid; BORDER-LEFT: #ccc 1px solid; WIDTH: 100%; BORDER-BOTTOM: #ccc 1px solid }
.form TD.check INPUT { BORDER-RIGHT: #000 0px solid; PADDING-RIGHT: 0px; BORDER-TOP: #000 0px solid; PADDING-LEFT: 0px; PADDING-BOTTOM: 0px; BORDER-LEFT: #000 0px solid; WIDTH: 15px; PADDING-TOP: 0px; BORDER-BOTTOM: #000 0px solid }
.form TH { FONT-SIZE: 11px; COLOR: #fff; BORDER-BOTTOM: #fff 1px solid; WHITE-SPACE: nowrap; BACKGROUND-COLOR: #00f; TEXT-ALIGN: left }
button{border: 1px solid #000;width:20px;heigth:20px;}
</style>
<%string sScript = ResolveUrl("~/adminscript.ashx?script=");%>
<script src="<%=sScript%>common.js" type="text/javascript"></script>
<script type="text/javascript">
ctrlSEvent = 'getE(\'<%=cmdSave.ClientID%>\').click();';

function AddSetting(btn)
{
	var name = document.getElementById('txtNewSetting').value;
	if(name)
	{
		var tr = btn.parentNode.parentNode;
		var newTr = document.createElement('tr');
		var newTh = document.createElement('th');
		var newTd = document.createElement('td');
		newTr.appendChild(newTh);
		newTr.appendChild(newTd);
		
		newTh.innerHTML = "<button title='������� ����������' onclick='RemoveSetting(this)'>�</button> " + name + ":";
		newTd.innerHTML = "<input type='text' name='custom_" + name + "' />";

		tr.parentNode.insertBefore(newTr, tr);
		document.getElementById('txtNewSetting').value = '';
	}
	else
	{
		alert('������� �������� ����������!');
	}
}
function RemoveSetting(btn)
{
	var tr = btn.parentNode.parentNode;
	var txt = tr.getElementsByTagName('input')[0];
	var name = txt.name.replace('custom_',' ');
	if(confirm('�� �������, ��� ������ ������� ��������� "' + name + '"?'))
	{
		tr.parentNode.removeChild(tr);
	}
}
</script>
<table class="form">
	<tr>
		<th>
			��������� ����:</th>
		<td class="check"><asp:checkbox id="chkOff" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			�������� ��������� (~/underconstruction.aspx):</th>
		<td><asp:TextBox Runat="server" ID="txtRedirectAll"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			SEO ������:</th>
		<td class="check"><asp:checkbox id="chkSeoError" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			������ ����������� � ��:</th>
		<td width="100%" style='background-color:<%=txtConStr.Text.Trim().Length > 0 ? (Sota.Web.SimpleSite.Util.TestDBConnection(txtConStr.Text) ? "" : "red") : "" %>'
			><asp:TextBox Runat="server" id="txtConStr"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			�������� �����������:</th>
		<td class="check"><asp:checkbox id="chkEnableAuth" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			���������� ���������:</th>
		<td class="check"><asp:checkbox id="chkLogRedirect" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			���������� ����������:</th>
		<td class="check"><asp:checkbox id="chkLogDownload" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			���������� ������:</th>
		<td class="check"><asp:checkbox id="chkLogError" runat="server"></asp:checkbox></td>
	</tr>
	<tr>
		<th>
			������� ����:</th>
		<td><asp:dropdownlist runat="server" id="cmbTimeZone"></asp:dropdownlist></td>
	</tr>
	<tr>
		<th>
			����������:</th>
		<td><asp:TextBox id="txtExt" runat="server"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			���������� ��������:</th>
		<td><asp:TextBox Runat="server" ID="txtPageCache"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			������� ��-����:</th>
		<td><asp:TextBox Runat="server" ID="txtOnline"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			����������� ����������:</th>
		<td><asp:TextBox Runat="server" ID="txtSeparator"></asp:TextBox></td>
	</tr>
	<tr>
		<th valign="top">
			������ ��������������:</th>
		<td><asp:TextBox Runat="server" ID="txtAdminPass"></asp:TextBox>
		<b>�������:</b> <asp:TextBox Runat="server" ID="txtNewAdminPass"></asp:TextBox></td>
	</tr>
	<tr>
		<th valign="top">
			������ ���������:</th>
		<td><asp:TextBox Runat="server" ID="txtManagerPass"></asp:TextBox> 
		<b>�������:</b> <asp:TextBox Runat="server" ID="txtNewManagerPass"></asp:TextBox></td>
	</tr>
	<tr>
		<th valign="top">
			������:</th>
		<td>
		<b>����:</b>
		<asp:TextBox Runat="server" ID="txtEncKey"></asp:TextBox>
		<b>������ �������������:</b>
		<asp:TextBox Runat="server" ID="txtEncVI"></asp:TextBox>
		<b>������� ������:</b><br/>
		<asp:dropdownlist runat="server" id="cmbEncLevel">
		<asp:ListItem Value="0">0 - ������� ����������</asp:ListItem>
		<asp:ListItem Value="1">1 - � ���������� ��������</asp:ListItem>
		<asp:ListItem Value="2">2 - � ����������� IP</asp:ListItem>
		</asp:dropdownlist></td>
	</tr>

	<tr>
		<th>
			�������� ������ (~/login.aspx):</th>
		<td><asp:TextBox Runat="server" ID="txtLoginPath"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			����� ����������� (~/img):</th>
		<td><asp:TextBox Runat="server" ID="txtImg"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			����� ������ (~/css):</th>
		<td><asp:TextBox Runat="server" ID="txtCss"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			����� �������� (~/script):</th>
		<td><asp:TextBox Runat="server" ID="txtScript"></asp:TextBox></td>
	</tr>
	<tr>
		<th>
			����� ������ (~/files):</th>
		<td><asp:TextBox Runat="server" ID="txtFiles"></asp:TextBox></td>
	</tr>
	<%foreach(string key in custom_settings.Keys){%>
		<tr><th><button title='������� ����������' onclick='RemoveSetting(this)'>�</button>
		<%=key%>:
		</th>
		<td><input type='text' name='custom_<%=key%>' value='<%=custom_settings[key]%>' /></td></tr>
	<%}%>
	<tr>
		<th><button title="�������� ����������" onclick="AddSetting(this)">+</button> ����������:
		</th>
		<td><input type="text" id='txtNewSetting' /></td>
	</tr>

	<tr>
		<td>&nbsp;</td>
		<td><asp:Button Runat="server" ID="cmdSave" Text="���������" Width="100px" BorderWidth="1px" BorderStyle="Solid"></asp:Button></td>
	</tr>
</table>
<br>
<asp:button runat="server" id="cmdClearCache" text="�������� ��� �������" Width="150px" borderwidth="1px" borderstyle="Solid"></asp:button>