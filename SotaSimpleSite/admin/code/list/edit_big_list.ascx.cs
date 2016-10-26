using System.Globalization;

namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;
	using System.Data;
	using System.Text;
	using System.Collections;
	using Sota.Web.SimpleSite.WebControls;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///	Отображение дерева списка
	/// </summary>
	public class edit_big_list : System.Web.UI.UserControl
	{
		private DataTable tbRequest = null;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfItem;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xf;
		protected System.Web.UI.HtmlControls.HtmlTableCell tdFieldControl;
		public List list;
		protected int pageSize = 20;
		protected string sortExpression = "";
		protected string filterExpression = "";
		bool restrictedUser = false;
		UserInfo ui = UserInfo.Current;
		Hashtable restrictedFields = null;
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			string pagex = PageInfo.Current.Pagex;
			string sList = PageInfo.Current.Path;
			if (pagex.Length > 0)
			{
				sList = sList.Substring(pagex.Length - 1);
				list = List.Create(sList, true);

				if ((string)ui.Fields[Keys.UserIsRestricted] == "yes")
				{
					restrictedUser = true;
					bool restrictedList = true;
					foreach (string key in ui.DataFields.Keys)
					{
						if (key.StartsWith("list[" + list.Name + "]"))
						{
							restrictedList = false;
						}
					}
					if (restrictedList)
					{
						Response.Redirect("~/admin/lists.aspx");
					}
				}

				if (!ui.CanAccess(list.SecurityPart))
				{
					Response.Redirect("~/admin/lists.aspx");
				}


				if (restrictedUser)
				{
					restrictedFields = new Hashtable();
					if (ui["list[" + list.Name + "]"] != null)
					{
						string[] rf = ui["list[" + list.Name + "]"].ToString().Split('&');
						for (int i = 0; i < rf.Length; i++)
						{
							int j = rf[i].IndexOf("=");
							string r_field = rf[i].Substring(0, j);
							string r_value = ParseUserExpression(rf[i].Substring(j + 1));
							restrictedFields.Add(
								r_field,
								r_value
								);
							list.Data.Columns[r_field].DefaultValue = r_value;
						}
					}

					foreach (DataColumn col in list.Data.Columns)
					{
						if (col.ExtendedProperties["uploads"] != null)
						{
							col.ExtendedProperties["uploads"] = 
								col.ExtendedProperties["uploads"].ToString().TrimEnd('/')
								+ "/" + ui.LoginName;
						}
					}
				}


				if (!Util.IsBlank(Request.Cookies["ListSort_" + list.Name]))
				{
					try {
						sortExpression = "[_level] ASC, " + Server.UrlDecode(Request.Cookies["ListSort_" + list.Name].Value);
						DataRow[] rs = list.Data.Select("", sortExpression); 
					}
					catch {
						Util.SetCookie("ListSort_" + list.Name, "0", false);
						sortExpression = list.TreeSortExpression;
					}
				}
				else
				{
					sortExpression = list.TreeSortExpression;
				}
				if (!Util.IsBlank(Request.Cookies["ListFilter_" + list.Name]))
				{
					try
					{
						filterExpression = Server.UrlDecode(Request.Cookies["ListFilter_" + list.Name].Value);
						DataRow[] rs = list.Data.Select(filterExpression);
					}
					catch
					{
						//Hashtable ht = new Hashtable();
						//ht["cv"] = Request.Cookies["ListFilter_" + list.Name].Value;
						//ht["fe"] = filterExpression;
						//Log.Write("error", ht);
						Util.SetCookie("ListFilter_" + list.Name, "0", false);
						filterExpression = "";
					}
				}


				if(!Util.IsBlank(Request.Cookies["listPageSize_" + list.Name]))
				{
					pageSize = int.Parse(Request.Cookies["listPageSize_" + list.Name].Value);
				}
			}
			else
			{
				Response.Redirect("~/admin/lists.aspx");
			}
			if (Request.RequestType.ToLower() == "get")
			{
				xfItem.Action = Path.Full;
				list.ReadRootItems();
			}
			else
			{
				Response.Clear();
				if (Request.ContentType == "text/xml")
				{
					Response.ContentEncoding = Encoding.UTF8;
					DataSet ds = new DataSet();
					ds.ReadXml(Request.InputStream);
					if (ds.Tables.Count > 0)
					{
						tbRequest = ds.Tables[0];
						if (tbRequest.Columns.Contains(XmlForm.ElementForm)
							&& tbRequest.Columns.Contains(XmlForm.ElementAction))
						{
							if(tbRequest.Rows[0][XmlForm.ElementForm].ToString()==xf.ClientID)
							{
								XmlFormPostBack();
							}
							else
							{
								XmlFormPostBack(tbRequest.Rows[0][XmlForm.ElementAction].ToString());
							}
						}
					}
				}
				Response.End();
			}
		}

		string ParseUserExpression(string s)
		{
			return s;
			//StringBuilder sb = new StringBuilder();
			//sb.Append(s);
			//sb.Replace("[@login]", ui.LoginName);
			//sb.Replace("[@email]", ui.Email);
			//sb.Replace("[@userid]", ui.UserId.ToString());
			//foreach (string key in ui.DataFields.Keys)
			//{
			//    sb.Replace("[@" + key + "]", ui.DataFields[key].ToString());
			//}
			//return sb.ToString();
		}
		
		public string FIELD_ID
		{
			get { return List.FIELD_ID; }
		}

		public string FIELD_GUID
		{
			get { return List.FIELD_GUID; }
		}

		public string FIELD_PARENT_ID
		{
			get { return List.FIELD_PARENT_ID; }
		}

		public string FIELD_DELETED
		{
			get { return List.FIELD_DELETED; }
		}

		public string FIELD_LEVEL
		{
			get { return List.FIELD_LEVEL; }
		}

		public string FileManagerPage
		{
			get { return ResolveUrl(Config.Main.FileManagerPage); }
		}

		public string DownloadPage
		{
			get { return ResolveUrl(Config.Main.DownloadPage); }
		}

		public string DatePickerPage
		{
			get { return ResolveUrl(Config.Main.DatePickerPage); }
		}

		public string TimePickerPage
		{
			get { return ResolveUrl(Config.Main.TimePickerPage); }
		}

		public bool IsCustomField(string fieldName)
		{
			return List.IsCustomField(fieldName);
		}
		private void XmlFormPostBack()
		{
			string action = tbRequest.Rows[0]["hact"].ToString();
			switch (action)
			{
				case "tree":
					string[] pid = tbRequest.Rows[0]["hid"].ToString().Split('_');
					int parent_id = int.Parse(pid[0]);
					int node_id = parent_id;
					if(parent_id==0)
					{
						parent_id = -1;
					}
					if(node_id==-1)
					{
						node_id=0;
					}
					int level = -1;
					if(parent_id > 0)
					{
						list.ReadItem(parent_id);
						level = Convert.ToInt32(list.Data.FirstRow[List.FIELD_LEVEL]);
						list.Clear();
					}
					level++;
					list.ReadChildren(parent_id);

					string[] arNewItemIC = list.GetNewItemIconCaption(level);
					string[] arIcons = list.GetLevelIcons(level);
					bool bUseIcon = !list.TreeNoIcon;

					DataRow[] rows = null;
					string restrictedExpression = "";
					string allow = "full";
					if (restrictedUser)
					{
						string settings = "";
						if(ui["list[" + list.Name + "][" + level + "]"] != null)
						{
							settings = ui["list[" + list.Name + "][" + level + "]"].ToString();
						}
						else if (ui["list[" + list.Name + "][*]"] != null)
						{
							settings = ui["list[" + list.Name + "][*]"].ToString();
						}

						int k = settings.IndexOf("&filter=");
						if (k > -1)
						{
							restrictedExpression = settings.Substring(k + "&filter=".Length);
							allow = settings.Substring("allow=".Length, k - "allow=".Length);
						}
						else
						{
							allow = settings.Substring("allow=".Length);
						}
						
					}
					try
					{
						rows = list.Data.Select(
							restrictedExpression.Length==0
								? filterExpression
								: (filterExpression.Length==0 
									? restrictedExpression 
									:"(" + restrictedExpression + ") AND (" + filterExpression + ")"), 
							sortExpression);
					}
					catch (Exception ex)
					{
						Hashtable ht = new Hashtable();
						ht["filter"] = filterExpression;
						ht["sort"] = sortExpression;
						ht["exception"] = ex.ToString();
						Log.Write("ListExpressionException", ht);

						rows = list.Data.Select(restrictedExpression, "");
					}
					int start = 0;
					int end = rows.Length;
					int page = 0;
					
					Response.Write("[");

					if(pid.Length > 1)
					{
						page = int.Parse(pid[1]);
						start = (page - 1) * pageSize;
						end = Math.Min(end, start + pageSize);
					}
					else if(pageSize>0 && end > pageSize)
					{
						int res;
						int pageCount = Math.DivRem(end, pageSize, out res);
						if (res > 0)
							pageCount++;
						Response.Write(string.Format(
							"new SotaTreeNode('{0}_0','{0}','{1}','javascript:{2};'{3})",
							node_id,
							arNewItemIC[1].Replace("\n"," ").Replace("\r"," ").Replace("'","\\'"),
							allow == "full" 
								? string.Format("CreateItem({0},{1})", level, parent_id) 
								: "alert(\\'Вы не можете добавлять элементы этого уровня!\\')",
							bUseIcon ? ",null,null,'" + ResolveUrl(arNewItemIC[0])+"'" : ""
							));
						for(int i = 0; i < pageCount; i++)
						{
							Response.Write(string.Format(
								",new SotaTreeNode('{0}_{1}','{0}','Страница {1}','javascript:treeview.open(\\'{0}_{1}\\');',null,null,'',null,true,true)",
								node_id, 
								i+1));
						}
						Response.Write("]");
						Response.End();
					}
					else
					{
						Response.Write(string.Format(
							"new SotaTreeNode('{0}_0','{0}','{1}','javascript:{2};'{3})",
							node_id,
							arNewItemIC[1],
							allow == "full"
								? string.Format("CreateItem({0},{1})", level, parent_id)
								: "alert(\\'Вы не можете добавлять элементы этого уровня!\\')",
							bUseIcon ? ",null,null,'" + ResolveUrl(arNewItemIC[0]) + "'" : ""));
						if(end > 0)
						{
							Response.Write(",");
						}
					}
					for(int i = start; i < end; i++)
					{
						if(i > start)
						{
							Response.Write(",");
						}
						DataRow r = rows[i];
						Response.Write(string.Format(
							"new SotaTreeNode('{0}','{1}{2}','{3}','javascript:OpenItem({0});',null,null,'{4}'{5})",
							r[List.FIELD_ID], 
							node_id, 
							page==0 ? "" : "_" + page,
							list.GetTreeCaption(r),
							bUseIcon ? (r[FIELD_DELETED].ToString()=="0" ? ResolveUrl(arIcons[0]) : ResolveUrl(arIcons[1])) : "",
							list.CanBeParent(Convert.ToInt32(r[List.FIELD_LEVEL])) ? ",null, true" : ""));
					}
					Response.Write("]");
				break;
			}
		}

		private void XmlFormPostBack(string action)
		{
			switch (action)
			{
				case XmlForm.ActionRefresh:
				{
					#region refresh

					Response.ContentType = "text/xml";
					Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					Response.Write("<response>");
					ItemRefresh();
					Response.Write("</response>");

					#endregion

					break;
				}
				case XmlForm.ActionSubmit:
				{
					#region submit

					ItemSubmit();

					#endregion

					break;
				}
				case XmlForm.ActionList:

					#region list

				{
					Response.ContentType = "text/xml";
					Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					Response.Write("<response>");
					//string lst	= this.tbRequest.Rows[0]["list"].ToString();
					//string list	= lst.Substring(lst.LastIndexOf("_")+1);
					Response.Write("</response>");
				}

					#endregion

					break;
			}

		}

		private void ItemSubmit()
		{
			string submitAction = tbRequest.Rows[0]["hAction"].ToString();
			int itemId = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_ID]);
			switch (submitAction)
			{
				case "save":

					#region save

					int parentId = -1;
					int deleted = 0;
					int level = 0;
					Hashtable h = new Hashtable();
					foreach (DataColumn col in list.Data.Columns)
					{
						if (IsCustomField(col.ColumnName))
						{
							string field = "field_" + col.ColumnName;

							if (tbRequest.Columns.Contains(field))
							{
								string val = tbRequest.Rows[0][field]==null ? "" : tbRequest.Rows[0][field].ToString();
								if(col.DataType==typeof(DateTime) && col.ExtendedProperties["inputtype"].ToString()=="datetime")
								{
									if(val.Length >0)
									{
										try
										{
											h[col.ColumnName] = DateTime.Parse(tbRequest.Rows[0][field].ToString(), CultureInfo.InvariantCulture);
										}
										catch(Exception ex)
										{
											Config.ReportError(ex);
										}
									}
									else
									{
										h[col.ColumnName] = DBNull.Value;
									}
								}
								else if(col.DataType==typeof(double))
								{
									if(val.Length >0)
									{
										try
										{
											h[col.ColumnName] = double.Parse(tbRequest.Rows[0][field].ToString(), CultureInfo.InvariantCulture);
										}
										catch(Exception ex)
										{
											Config.ReportError(ex);
										}
									}
									else
									{
										h[col.ColumnName] = DBNull.Value;
									}
								}
								else if(col.DataType==typeof(int))
								{
									if(val.Length >0)
									{
										try
										{
											h[col.ColumnName] = int.Parse(tbRequest.Rows[0][field].ToString(), CultureInfo.InvariantCulture);
										}
										catch(Exception ex)
										{
											Config.ReportError(ex);
										}
									}
									else
									{
										h[col.ColumnName] = DBNull.Value;
									}
								}
								else
								{
									if (restrictedUser)
									{
										//заменяем значения у файловых полей
										if (col.ExtendedProperties["inputtype"].ToString() == "file"
											&& !Util.IsBlank(val))
										{
											val = ui.LoginName + "/" + val; 
										}
									}

									h[col.ColumnName] = val;
								}
							}
							/*else
							{
								h[col.ColumnName] = string.Empty;
							}*/
						}
						else
						{
							switch (col.ColumnName)
							{
								case List.FIELD_PARENT_ID:
									parentId = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_PARENT_ID].ToString().Split('#')[1]);
									break;
								case List.FIELD_DELETED:
									deleted = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_DELETED]);
									break;
								case List.FIELD_LEVEL:
									level = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_PARENT_ID].ToString().Split('#')[0]);
									break;
							}
						}
					}

					string allow = "full";
					if (restrictedUser)
					{
						int ilevel = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_PARENT_ID].ToString().Split('#')[0]);
						string settings = "";
						if (ui["list[" + list.Name + "][" + ilevel + "]"] != null)
						{
							settings = ui["list[" + list.Name + "][" + ilevel + "]"].ToString();
						}
						else if (ui["list[" + list.Name + "][*]"] != null)
						{
							settings = ui["list[" + list.Name + "][*]"].ToString();
						}

						int k = settings.IndexOf("&filter=");
						if (k > -1)
						{
							allow = settings.Substring("allow=".Length, k - "allow=".Length);
						}
						else
						{
							allow = settings.Substring("allow=".Length);
						}


						//заменяем форсированные значения
						foreach (string key in restrictedFields.Keys)
						{
							h[key] = restrictedFields[key];
						}

					}
					if (allow != "view")
					{
						if (itemId == -1)
						{
							Response.Write(list.Insert(parentId, deleted, h));
						}
						else
						{
							list.Update(itemId, parentId, deleted, h);
							if (!Util.IsBlank(tbRequest.Rows[0]["txtAnotherParentID"]))
							{
								try
								{
									int newPid = Convert.ToInt32(tbRequest.Rows[0]["txtAnotherParentID"]);
									if (newPid == 0)
									{
										newPid = -1;
									}
									if (newPid != parentId)
									{
										int plevel = 0;
										bool parentExists = false;
										if (newPid == -1)
										{
											parentExists = true;
										}
										else
										{
											List l = List.Create(list.Name);
											l.ReadItem(newPid);
											if (l.Data.Rows.Count > 0)
											{
												plevel = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]);
												if (list.CanBeParent(plevel))
												{
													parentExists = true;
													plevel++;
												}
											}
										}
										if (parentExists)
										{
											if (plevel == level)
											{
												list.Update(itemId, newPid, deleted);
											}
											else
											{
												list.ChangeParent(itemId, newPid);
											}
										}

									}
								}
								catch (Exception ex)
								{
									Config.ReportError(ex);
								}
							}
						}
					}

					#endregion

					break;
				case "delete":

					#region delete

					if (itemId != -1)
					{
						list.ReadItem(itemId);

						string dallow = "full";
						if (restrictedUser)
						{
							int ilevel = Convert.ToInt32(tbRequest.Rows[0]["field_" + List.FIELD_PARENT_ID].ToString().Split('#')[0]);
							string settings = "";
							if (ui["list[" + list.Name + "][" + ilevel + "]"] != null)
							{
								settings = ui["list[" + list.Name + "][" + ilevel + "]"].ToString();
							}
							else if (ui["list[" + list.Name + "][*]"] != null)
							{
								settings = ui["list[" + list.Name + "][*]"].ToString();
							}

							int k = settings.IndexOf("&filter=");
							if (k > -1)
							{
								dallow = settings.Substring("allow=".Length, k - "allow=".Length);
							}
							else
							{
								dallow = settings.Substring("allow=".Length);
							}
						}

						if (dallow == "full")
						{
							if (list.DeleteTree(itemId))
								Response.Write("-1");
						}
					}

					#endregion

					break;
			}
		}



		private void ItemRefresh()
		{
			if (Request.QueryString["id"] == null)
				return;
			list.ReadItem(Int32.Parse(Request.QueryString["id"]));
			StringBuilder sb = new StringBuilder();
			XmlBuilder.AppendField(sb, "field_" + FIELD_PARENT_ID, list.Data.Rows[0][FIELD_LEVEL].ToString() + "#" + list.Data.Rows[0][FIELD_PARENT_ID].ToString());
			foreach (DataColumn col in list.Data.Columns)
			{
				if (col.ColumnName != FIELD_PARENT_ID
					&& col.ColumnName != FIELD_LEVEL)
				{
					string val = list.Data.Rows[0][col].ToString();
					if (col.ExtendedProperties.ContainsKey("inputtype"))
					{
						if (val != string.Empty)
						{
							switch (col.ExtendedProperties["inputtype"].ToString())
							{
								case "date":
									val = Convert.ToDateTime(list.Data.Rows[0][col]).ToString("yyyy-MM-dd");
									break;
								case "time":
									val = Convert.ToDateTime(list.Data.Rows[0][col]).ToLongTimeString();
									break;
								case "datetime":
									val = Convert.ToDateTime(list.Data.Rows[0][col]).ToString(CultureInfo.InvariantCulture);
									break;
								case "file":
									if (restrictedUser)
									{
										if (val.StartsWith(ui.LoginName + "/"))
										{
											val = val.Substring((ui.LoginName + "/").Length);
										}
									}
									break;
							}
							if(col.DataType==typeof(double))
							{
								val = Convert.ToDouble(list.Data.Rows[0][col]).ToString(CultureInfo.InvariantCulture);
							}
						}
					}
					XmlBuilder.AppendField(sb, "field_" + col.ColumnName, val);
				}
			}
			Response.Write(sb);
		}
	}
}