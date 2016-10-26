namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;
	using System.Data;
	using System.Text;
	using System.Collections;
	using Sota.Web.SimpleSite.WebControls;

	/// <summary>
	///	Отображение дерева списка
	/// </summary>
	public class edit_list : System.Web.UI.UserControl
	{
		private DataTable tbRequest = null;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfItem;
		protected System.Web.UI.HtmlControls.HtmlTableCell tdFieldControl;
		public List list;
		
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad (e);
			string pagex = PageInfo.Current.Pagex;
			string sList = PageInfo.Current.Path;
			if (pagex.Length > 0)
			{
				sList = sList.Substring(pagex.Length - 1);
				list = List.Create(sList, true);
			}
			else
			{
				Response.End();
			}
			if (Request.RequestType.ToLower() == "get")
			{
				xfItem.Action = Path.Full;
				list.ReadAllFull();
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
							XmlFormPostBack(tbRequest.Rows[0][XmlForm.ElementAction].ToString());
						}
					}
				}
				Response.End();
			}
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
								h[col.ColumnName] = tbRequest.Rows[0][field];
							}
							else
							{
								h[col.ColumnName] = string.Empty;
							}
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
					if (itemId == -1)
					{
						Response.Write(list.Insert(parentId, deleted, level, h));
					}
					else
					{
						list.Update(itemId, parentId, deleted, level, h);
					}

					#endregion

					break;
				case "delete":

					#region delete

					if (itemId != -1)
					{
						if (list.Delete(itemId))
							Response.Write("-1");
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