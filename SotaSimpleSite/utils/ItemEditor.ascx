<%@ Control Language="C#" AutoEventWireup="true"  %>
<%@ Import Namespace="Sota.Web.SimpleSite" %>
<%@ Import Namespace="Sota.Web.SimpleSite.Security" %>
<%@ Import Namespace="System.Data" %>
<script runat="server">
	ObjectTypes objectType = ObjectTypes.Empty;
	public ObjectTypes ObjectType { get {return objectType;} set {objectType=value;} }
	string listName = "";
	public string ListName { get {return listName;} set {listName=value;} }
	string itemID = "";
	public string ItemID { get {return itemID;} set {itemID=value;} }
	string userIDField = "";
	public string UserIDField { get {return userIDField;} set {userIDField=value;} }
	string returnUrl = "";
	public string ReturnUrl { get {return returnUrl;} set {returnUrl=value;} }

	public enum ObjectTypes
	{
		Empty, User, Page, List
	}
	
	string prefixID = "_Prop_";
	UserInfo userInfo = UserInfo.Current;
	void Page_Load(object o, EventArgs e)
	{
		if(!userInfo.IsAuthorized)
		{
			return;
		}
		
		prefixID = UniqueID + "_Prop_";
		if (Request.HttpMethod.ToUpper() == "GET")
		{
			if (ObjectType == ObjectTypes.Empty)
			{
				if(Request.QueryString["type"] == null)
				{
					ObjectType = ObjectTypes.List;
				}
				else
				{
					ObjectType = (ObjectTypes)Enum.Parse(typeof(ObjectTypes), Request.QueryString["type"]);
				}
			}
			if (Util.IsBlank(ListName) && Request.QueryString["list"] != null)
			{
				ListName = Request.QueryString["list"];
			}
			if (Util.IsBlank(ItemID) && Request.QueryString["id"] != null)
			{
				ItemID = Request.QueryString["id"];
			}
			if (Util.IsBlank(UserIDField) && Request.QueryString["uid"] != null)
			{
				UserIDField = Request.QueryString["uid"];
			}

			if (Util.IsBlank(ReturnUrl))
			{
				if (Request.QueryString["url"] != null)
				{
					ReturnUrl = Request.QueryString["url"];
				}
				else if (Request.UrlReferrer != null)
				{
					ReturnUrl = Request.UrlReferrer.ToString();
				}
			}
			if (Util.IsBlank(ReturnUrl))
			{
				if (Request.QueryString["url"] != null)
				{
					ReturnUrl = Request.QueryString["url"];
				}
				else if (Request.UrlReferrer != null)
				{
					ReturnUrl = Request.UrlReferrer.ToString();
				}
			}

		}
		else //POST
		{
			ObjectType = (ObjectTypes)Enum.Parse(typeof(ObjectTypes), Request.Form[prefixID + "ObjectType"]);
			ListName = Request.Form[prefixID + "ListName"];
			ItemID = Request.Form[prefixID + "ItemID"];
			UserIDField = Request.Form[prefixID + "UserIDField"];
			ReturnUrl = Request.Form[prefixID + "ReturnUrl"];
		}

		switch (ObjectType)
		{
			case ObjectTypes.User:
				UserItem();
				break;
			case ObjectTypes.Page:
				PageItem();
				break;
			case ObjectTypes.List:
				ListItem();
				break;
		}
	}

	void UserItem()
	{
	}

	void PageItem()
	{
	}

	List list = null;
	DataRow listItem = null;
	void ListItem()
	{
		if(Util.IsBlank(UserIDField) && !(userInfo.IsAdmin || userInfo.IsManager))
		{
			//пользователь не имеет доступа на редактирование
			return;
		}

		list = List.Create(ListName);
		list.Data.Columns["_pid"].DefaultValue = -1;
		
		if (Util.IsBlank(ItemID) && Util.IsBlank(UserIDField))
		{
			listItem = list.Data.NewRow();
		}
		else if(Util.IsBlank(ItemID))
		{
			listItem = list.FindByField(UserIDField, userInfo.UserId).FirstRow;
			if(listItem == null)
			{
				listItem = list.Data.NewRow();
			}
		}
		else if (Util.IsBlank(UserIDField))
		{
			listItem = list.ReadItem(int.Parse(ItemID)).FirstRow;
		}
		else
		{
			listItem = list.ReadItem(int.Parse(ItemID)).FindRow("[" + UserIDField + "]=" + userInfo.UserId);
		}
		
		if(listItem==null)
		{
			//строка не найдена или пользователь не имеет к ней доступа
			return;
		}
		
		mv.SetActiveView(vList);
		
		if(Request.HttpMethod.ToUpper()=="POST")
		{
			int _deleted = Request.Form[prefixID + "_deleted"] == "1" ? 1 : 0;
			int _pid = int.Parse(Request.Form[prefixID + "_pid"]);
			Hashtable ht = new Hashtable();
			if(!Util.IsBlank(UserIDField))
			{
				ht[UserIDField] = userInfo.UserId;
			}
			for (int i = 0; i < list.Data.Columns.Count; i++)
			{
				DataColumn col = list.Data.Columns[i];
				if (List.IsCustomField(col.ColumnName))
				{
					if (col.ExtendedProperties["levels"] != null && !col.ExtendedProperties["levels"].ToString().Contains(listItem["_level"].ToString()))
					{
						continue;
					}

					if (Util.IsBlank(Request.Form[prefixID + col.ColumnName])
						/*&&
						(
						col.DataType == typeof(DateTime)
						|| col.DataType == typeof(double)
						|| col.DataType == typeof(int)
						)*/)
					{
						ht[col.ColumnName] = DBNull.Value;
					}
					else
					{
						string inputType = col.ExtendedProperties["inputtype"].ToString().ToLower();
						switch (inputType)
						{
							case "datetime":
								ht[col.ColumnName] = DateTime.Parse(Request.Form[prefixID + col.ColumnName]);
								break;
							case "checkbox":
								ht[col.ColumnName] = Request.Form[prefixID + col.ColumnName] == "1" ? "1" : "0";
								break;
							default:
								ht[col.ColumnName] = Request.Form[prefixID + col.ColumnName];
								break;
						}
					}
				}
			}

			try
			{
				if (Util.IsBlank(itemID))
				{
					int id = list.Insert(_pid, _deleted, ht);
					/*if(Util.IsBlank(ReturnUrl))
					{
						ReturnUrl = "?list=" + ListName + "&id=" + id + (Util.IsBlank(UserIDField) ? "" : "&uid=" + UserIDField + "&url=");
					}*/
				}
				else
				{
					list.Update((int)listItem["_id"], _pid, _deleted, ht);
				}

				ltError.Text = string.Format("<p style=\"color:green\">Изменения успешно сохранены</p>{0}",
					Util.IsBlank(ReturnUrl)
					? ""
					: "<scr" + "ipt>setTimeout('location.href=\"" + ReturnUrl + "\"', 1000);</scr" + "ipt>"
					);
			}
			catch(Exception ex)
			{
				ltError.Text = string.Format("<p style=\"color:red\">{0}</p>", ex);
			}
		}
	}
	
</script>
<style>
.table-itemeditor {border:0; border-collapse:collapse;width:100%;}
.table-itemeditor th, .table-itemeditor td {padding:5px;}
.table-itemeditor th {white-space:nowrap;}
.table-itemeditor td {width: 100%;}
.table-itemeditor tr.alt {background-color: #f5f5f5;}
.table-itemeditor tr.nolevel {display:none;}
.table-itemeditor td textarea {width:100%;}
</style>

<asp:Literal runat="server" ID="ltError"></asp:Literal>
<form method="post">
<input type="hidden" name="<%=prefixID %>ObjectType" value="<%=ObjectType %>" />
<input type="hidden" name="<%=prefixID %>ListName" value="<%=ListName %>" />
<input type="hidden" name="<%=prefixID %>ItemID" value="<%=ItemID %>" />
<input type="hidden" name="<%=prefixID %>UserIDField" value="<%=UserIDField %>" />
<input type="hidden" name="<%=prefixID %>ReturnUrl" value="<%=ReturnUrl %>" />

<asp:MultiView runat="server" ID="mv" ActiveViewIndex="0">
	<asp:View runat="server" ID="vError"><p style="color:red;">Произошла неизвестная ошибка!</p></asp:View>
	<asp:View runat="server" ID="vUser"></asp:View>
	<asp:View runat="server" ID="vPage"></asp:View>
	<asp:View runat="server" ID="vList">
		<table class="table-itemeditor">
			<tr>
				<th>ID:</th>
				<td><%=listItem["_id"]%></td>
			</tr>
			<tr class="alt">
				<th>GUID:</th>
				<td><%=listItem["_guid"]%></td>
			</tr>
			<tr>
				<th>LEVEL:</th>
				<td><%=listItem["_level"]%></td>
			</tr>
			<tr class="alt">
				<th>PID:</th>
				<td>
					<input name="<%=prefixID %>_pid" type="text" value="<%=listItem["_pid"]%>" /></td>
			</tr>
			<tr>
				<th>DELETED:</th>
				<td>
					<input name="<%=prefixID %>_deleted" type="checkbox" value="1<%if (listItem["_deleted"].ToString() == "1")
																	{%>" checked="checked<%}%>" /></td>
			</tr>
			<%int skiped = 0;
for(int i=0; i < list.Data.Columns.Count; i++)
{
	DataColumn col = list.Data.Columns[i];
	if(List.IsCustomField(col.ColumnName))
	{
		if (col.ExtendedProperties["levels"] != null && !col.ExtendedProperties["levels"].ToString().Contains(listItem["_level"].ToString()))
		{
			skiped++;
			continue;
		}
		Response.Write(string.Format("<tr class=\"{1}\"><th>{0}:{2}</th><td>"
		, col.Caption
		, (i+skiped)%2==0 ? "alt" : ""
		, col.ExtendedProperties["allownull"]!=null && col.ExtendedProperties["allownull"].ToString()!="1" ? "<span style=\"color:red;\">*</span>" : ""
		));
		string inputType =  col.ExtendedProperties["inputtype"].ToString().ToLower();
		switch(inputType)
		{
			case "hidden":
			case "text":
			case "password":
			case "regex":
				Response.Write(string.Format("<input type=\"text\" name=\"{0}{1}\" value=\"{2}\" />", prefixID, col.ColumnName, listItem[col.ColumnName].ToString().Replace("\"","&quote;")));
			break;
			case "datetime":
				Response.Write(string.Format("<input type=\"text\" name=\"{0}{1}\" value=\"{2:yyyy-MM-dd HH:mm}\" />", prefixID, col.ColumnName, listItem[col.ColumnName]));
			break;
			case "checkbox":
				Response.Write(string.Format("<input type=\"checkbox\" name=\"{0}{1}\" value=\"1\"{2}  />", prefixID, col.ColumnName, listItem[col.ColumnName].ToString()=="1" ? " checked=\"checked\"" : ""));
			break;
			default:
				Response.Write(string.Format("<textarea rows=\"5\" name=\"{0}{1}\">{2}</textarea>", prefixID, col.ColumnName, listItem[col.ColumnName]));
			break;
		}
		Response.Write("</td></tr>");
	}
	else { skiped++; }
}
%>
		<tr><th>&nbsp;</th><td><button type="submit">Сохранить</button></td></tr>
		</table>
	</asp:View>

</asp:MultiView>
</form>