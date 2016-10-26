<%@ Control Language="c#" %>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<%@ Import Namespace="Sota.Data.Simple"%>
<%@ Import Namespace="System.Data"%>
<%@ Import Namespace="System.Text.RegularExpressions"%>
<%@ Register TagPrefix="sota" Namespace="Sota.Web.UI.WebControls" Assembly="SotaSimpleSite"%>
<%@ Register TagPrefix="sss" Namespace="Sota.Web.SimpleSite" Assembly="SotaSimpleSite"%>
<script runat="server">
	List list = null;
	void Page_Load(object sender, EventArgs e)
	{
		Sota.Web.SimpleSite.Security.UserInfo ui = Sota.Web.SimpleSite.Security.UserInfo.Current;
		bool restrictedUser = (string)ui.Fields[Keys.UserIsRestricted] == "yes";
		
		if (Request.QueryString["list"] != null)
		{
			if ((restrictedUser && ui.DataFields["list[" + Request.QueryString["list"] + "][*]"] != null)
				||(!restrictedUser))
			{

				try
				{
					list = List.Create(Request.QueryString["list"], true);
					list.ReadAllFull();
				}
				catch { }
			}
		}
		if (list == null)
		{
			DataTable tb = List.GetLists().Copy();
			if (restrictedUser)
			{
				for (int i = tb.Rows.Count - 1; i > -1; i--)
				{
					if (ui.DataFields["list[" + tb.Rows[i]["name"] + "][*]"] == null)
					{
						tb.Rows.RemoveAt(i);
					}
					else if (ui.DataFields["list[" + tb.Rows[i]["name"] + "][*]"].ToString().IndexOf("allow=full") != 0)
					{
						tb.Rows.RemoveAt(i);
					}
				}
			}
			if (tb.Rows.Count == 0)
			{
				Response.Redirect("~/admin/lists.aspx");
			}
			tb.Columns.Add("caption1").Expression = "caption + ' [' + name + ']'";
			cmbList.DataSource = tb;
			cmbList.DataTextField = "caption1";
			cmbList.DataValueField = "name";
			cmbList.DataBind();
		}
		else
		{
			if (!IsPostBack)
			{
				StringBuilder sbF = new StringBuilder();
				foreach (DataColumn col in list.Data.Columns)
				{
					if (List.IsCustomField(col.ColumnName))
					{
						if (sbF.Length > 0)
						{
							sbF.Append(",");
						}
						sbF.Append(col.ColumnName);
					}
				}
				txtFields.Value = sbF.ToString();
			}
		}
	}
	void Choose(object sender, EventArgs e)
	{
		Response.Redirect(Path.Full.Split('?')[0] + "?list=" + Request.Form[cmbList.UniqueID]);
	}
	void BuildItemsTree(int parentid, StringBuilder sb)
	{
		DataRow[] rows = list.Data.Select(List.FIELD_PARENT_ID + "=" + parentid, list.TreeSortExpression);
		for (int i = 0; i < rows.Length; i++)
		{
			int level = Convert.ToInt32(rows[i][List.FIELD_LEVEL]);
			sb.Append("<option value=\"" + (level + 1) + "#" + rows[i][List.FIELD_ID] + "\">" +(new string('`', level + 1)).Replace("`","` ")+ list.GetTreeCaption(rows[i]) + "</option>");
			if(list.CanBeParent(level))
			{
				BuildItemsTree(Convert.ToInt32(rows[i][List.FIELD_ID]), sb);
			}
		}
	}
	string sResult = "";
	void Import(object sender, EventArgs e)
	{
		try
		{
			ArrayList arrID = new ArrayList();
			string levelup = txtLevelUp.Value;
			string leveldown = txtLevelDown.Value;
			string[] p = Request.Form["cmbParent"].Split('#');
			int pid = int.Parse(p[1]);
			int level = int.Parse(p[0]);
			int deleted = chkDeleted.Checked ? 1 : 0;
			int lastID = pid;
			string[] rows = Regex.Split(txtData.Value, txtRowSeparator.Value);
			int imported = 0;
			for (int i = 0; i < rows.Length; i++)
			{
                if (Util.IsBlank(rows[i]))
                {
                    continue;
                }
                if (rows[i] == leveldown)
				{
					arrID.Add(pid);
					level++;
					pid = lastID;
					continue;
				}
				if (rows[i] == levelup)
				{
					level--;
					pid = Convert.ToInt32(arrID[arrID.Count-1]);
					arrID.RemoveAt(arrID.Count-1);
					continue;
				}
				Hashtable h = new Hashtable();
				string[] values = Regex.Split(rows[i], txtFieldSeparator.Value);
				string[] fields = txtFields.Value.Split(',');
				int n = Math.Min(fields.Length, values.Length);
				for (int j = 0; j < n; j++)
				{
					h[fields[j]] = values[j];
				}
				lastID = list.Insert(pid, deleted, level, h);
				imported++;
			}
			sResult = string.Format("<span style=\"color:green;\">Успешно импортировано {0} записей</span>", imported);
			txtData.Value = "";
		}
		catch (Exception ex)
		{
			sResult = string.Format("<span style=\"color:red;\">Произошла ошибка во время импорта!<br />{0}</span>", ex);
		}
	}
</script>
<%if(list==null){ %>
<table width="100%" height="100%">
	<tr>
		<td align="center" valign="middle">
			<table align="center" cellpadding="5">
				<tr>
					<td>
						<select runat="server" id="cmbList">
						</select>
					</td>
				</tr>
				<tr>
					<td>
						<asp:Button runat="server" ID="cmbLsit" Text="Далее >>" OnClick="Choose" />
					</td>
				</tr>
			</table>
		</td>
	</tr>
</table>
<%} else{%>
<style type="text/css">
table.settings th{font-size:11px;text-align:left;background:#00f;color:#fff;}
table.settings td,table.settings th{border: 1px solid #00f;padding:2px;}
</style>
<script src='<%=ResolveUrl("~/adminscript.ashx?script=common.js")%>' type="text/javascript"></script>
<script type="text/javascript">
function InsertText(s)
{
	paste(getE(s).value.replace('\\n','\n').replace('\\r','\r').replace('\\t','\t'),'',getE('<%=txtData.ClientID %>'));
}
</script>
<table class="settings">
	<tr>
		<th>
			Родительский элемент:</th>
		<td><select name="cmbParent"> 
                <option value="0#-1">..</option> 
                <%
                StringBuilder sb = new StringBuilder();
				BuildItemsTree(-1, sb);
				Response.Write(sb);
                %>
           </select></td>
	</tr>
	<tr>
		<th>Не показывать:</th>
		<td><asp:CheckBox runat="server" ID="chkDeleted" /></td>
	</tr>
	<tr>
		<th>
			Разделитель полей:</th>
		<td><input id="txtFieldSeparator" type="text" runat="server" value="##"/><input type="button" value="+" onclick="InsertText('<%=txtFieldSeparator.ClientID %>')" /></td>
	</tr>
	<tr>
		<th>
			Разделитель строк:</th>
		<td><input id="txtRowSeparator" type="text" runat="server" value="\r\n" /><input type="button" value="+" onclick="InsertText('<%=txtRowSeparator.ClientID %>')" /></td>
	</tr>
	<tr>
		<th>
			Уровень выше:</th>
		<td><input id="txtLevelUp" type="text" runat="server" value="[levelup]" /><input type="button" value="+" onclick="InsertText('<%=txtLevelUp.ClientID %>')" /></td>
	</tr>
	<tr>
		<th>
			Уровень ниже:</th>
		<td><input id="txtLevelDown" type="text" runat="server" value="[leveldown]" /><input type="button" value="+" onclick="InsertText('<%=txtLevelDown.ClientID %>')" /></td>
	</tr>
</table>
<br />
 <table style="width:100%">
 <tr>
 <td><%=sResult %>&nbsp;</td>
 </tr>
 <tr>
 <td>Поля (через запятую):</td>
 </tr>
  <tr>
 <td><input id="txtFields" type="text" runat="server" style="width:100%" /></td>
 </tr>
  <tr>
 <td>Данные:</td>
 </tr>
  <tr>
 <td><textarea runat="server" id="txtData" style="width:100%"></textarea></td>
 </tr>
 <tr>
 <td>
 <asp:Button runat="server" id="cmdImport" Text="Импортировать" OnClick="Import" />
 </td>
 </tr>
 </table>
<%} %>