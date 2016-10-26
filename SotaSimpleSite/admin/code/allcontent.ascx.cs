using System;
using System.Data;
using System.IO;
using System.Text;
using Sota.Web.SimpleSite.Map;
using Sota.Web.SimpleSite.Security;
using Sota.Web.SimpleSite.WebControls;

namespace Sota.Web.SimpleSite.Code.Admin
{
	/// <summary>
	///		Summary description for allcontent.
	/// </summary>
	public class AllContentEditor : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlSelect cmbParent;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbIndex;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbType;
		protected System.Web.UI.HtmlControls.HtmlInputText txtUrl;
		protected System.Web.UI.HtmlControls.HtmlInputText txtText;
		protected System.Web.UI.HtmlControls.HtmlInputText txtPath;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox chkShow;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hId;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hIndex;
		public string icon1 = string.Empty;
		public string icon2 = string.Empty;
		public string icon3 = string.Empty;
		public SiteMap sm = SiteMap.Current;

		private DataRow rRequest = null;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfItem;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfCommon;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfHeader;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfContent;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfDict;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfTemplate;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfCode;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfPagex;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbPart;
		protected System.Web.UI.HtmlControls.HtmlInputRadioButton chkSecure1;
		protected System.Web.UI.HtmlControls.HtmlInputRadioButton chkSecure2;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbTemplate;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbLM;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCodeFile;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtHeader;
		protected System.Web.UI.HtmlControls.HtmlInputText txtTitle;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCache;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox chkDeleted;
		protected System.Web.UI.HtmlControls.HtmlInputText txtKeyWords;
		protected System.Web.UI.HtmlControls.HtmlInputText txtDescription;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtHead;
		protected System.Web.UI.HtmlControls.HtmlSelect cmbStatus;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtBody;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hFieldAction;
		protected System.Web.UI.HtmlControls.HtmlSelect lstFields;
		protected System.Web.UI.HtmlControls.HtmlInputText txtFieldName;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtFieldValue;
		protected System.Web.UI.HtmlControls.HtmlInputText txtTemplatePath;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtTemplateBody;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCodePath;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtCodeBody;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hPagexAction;
		protected System.Web.UI.HtmlControls.HtmlInputText txtPagex;

		protected Sota.Web.SimpleSite.WebControls.XmlForm xfDelete;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfTree;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfTrash;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfSynchr;


		private int nItemId = -2;

		public bool IsAdmin = false;
		private string sPagePath = string.Empty;

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			IsAdmin = UserInfo.Current.IsInGroup(-3);
			icon1 = ResolveUrl("~/admin.ashx?img=checked_w.gif");
			icon2 = ResolveUrl("~/admin.ashx?img=unchecked_w.gif");
			icon3 = ResolveUrl("~/admin.ashx?img=item.gif");
			if (Request.QueryString["id"] != null)
				nItemId = int.Parse(Request.QueryString["id"]);
			if (Request.RequestType.ToLower() == "post" && Request.ContentType.ToLower() == "text/xml")
			{
				if (nItemId > -1 && nItemId < sm.All.Count)
				{
					MapItem m = sm.All[nItemId];
					if (m != null)
						sPagePath = m._path;
				}

				Response.Clear();
				DataSet ds = new DataSet();
				ds.ReadXml(Request.InputStream);
				if (ds.Tables.Count > 0)
				{
					rRequest = ds.Tables[0].Rows[0];
					XmlFormPostBack(rRequest[XmlForm.ElementForm].ToString(), rRequest[XmlForm.ElementAction].ToString());
				}
				Response.End();
			}
			else
			{
				this.xfItem.Action = Path.Full;
				InitCombo();
				//Response.Cache.SetLastModified(new DateTime(2006,9,4,1,1,1));
			}

		}

		#region init combo


		private void InitCombo()
		{
			DataTable tb = SecurityManager.GetParts();
			tb.DefaultView.Sort = "partname ASC";
			if (!IsAdmin)
			{
				tb.DefaultView.RowFilter = "partid<>-3";
			}
			this.cmbPart.DataSource = tb;
			this.cmbPart.DataTextField = "partname";
			this.cmbPart.DataValueField = "partid";
			this.cmbPart.DataBind();

			tb = Config.GetConfigTable("template.config", "template").Copy();
			tb.DefaultView.Sort = "name ASC";
			if (!IsAdmin)
			{
				tb.DefaultView.RowFilter = "file NOT LIKE 'admin/templates/*'";
			}
			this.cmbTemplate.DataSource = tb;
			this.cmbTemplate.DataTextField = "name";
			this.cmbTemplate.DataValueField = "file";
			this.cmbTemplate.DataBind();
		}


		#endregion

		#region define method

		private void XmlFormPostBack(string form, string action)
		{
			string name = form.Substring(form.LastIndexOf("_") + 1);
			switch (action)
			{
				case XmlForm.ActionRefresh:

					#region refresh

					Response.ContentType = "text/xml";
					Response.ContentEncoding = Encoding.UTF8;
					Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					Response.Write("<response>");
					switch (name)
					{
						case "xfItem":
							RefreshItem();
							break;
						case "xfHeader":
							HeaderRefresh();
							break;
						case "xfContent":
							ContentRefresh();
							break;
						case "xfDict":
							DictRefresh();
							break;
						case "xfTemplate":
							TemplateRefresh();
							break;
						case "xfCode":
							CodeRefresh();
							break;
						case "xfPagex":
							PagexRefresh();
							break;
					}
					Response.Write("</response>");

					#endregion

					break;
				case XmlForm.ActionSubmit:

					#region submit

					switch (name)
					{
						case "xfItem":
							SaveItem();
							break;
						case "xfDelete":
							DeleteItem();
							break;
						case "xfTree":
							RefreshTree();
							break;
						case "xfSynchr":
							SearchForNewPages();
							CorrectUrls();
							break;
						case "xfTrash":
							CheckTrash();
							break;
						case "xfHeader":
							HeaderSubmit();
							break;
						case "xfContent":
							ContentSubmit();
							break;
						case "xfDict":
							DictSubmit();
							break;
						case "xfTemplate":
							TemplateSubmit();
							break;
						case "xfCode":
							CodeSubmit();
							break;
						case "xfPagex":
							PagexSubmit();
							break;
					}

					#endregion

					break;
				case XmlForm.ActionList:

					#region list

					Response.ContentType = "text/xml";
					Response.ContentEncoding = Encoding.UTF8;
					string lst	= rRequest[XmlForm.ElementList].ToString();
					string list	= lst.Substring(lst.LastIndexOf("_")+1);
					Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
					Response.Write("<response>");
					switch (name)
					{
						case "xfItem":
							switch(list)
							{
								case "cmbParent":
									RefreshParents();
									break;
								case "cmbTemplate":
									RefreshTemplates();
									break;
								case "cmbPart":
									RefreshParts();
									break;
							}
							break;
						case "xfDict":
							DictRefreshList();
							break;
					}
					Response.Write("</response>");

					#endregion

					break;
			}
		}

		private void CheckTrash()
		{
			if (!PageInfo.TrashIsEmpty())
				Response.Write("1");
		}

		#endregion

		private void SaveItem()
		{
			MapItem m = sm;
			string title = SiteMap.RemoveBadChars(rRequest[this.txtText.ClientID].ToString());
			string url = rRequest[this.txtUrl.ClientID].ToString();
			if (nItemId != -1)
			{
				sPagePath = rRequest[this.txtPath.ClientID].ToString().Replace("admin/", "admin1/");
				if (nItemId == -2)
				{
					if(sPagePath!="/")
					{
						sPagePath = sPagePath.Trim('/');
					}
					if (rRequest[this.cmbType.ClientID].ToString() == "Page" && PageInfo.Exists(sPagePath))
					{
						return;
					}
					m = new MapItem();
					m._parent = sm;
				}
				else
				{
					m = sm.All[nItemId];
				}
				m.SetType(rRequest[this.cmbType.ClientID].ToString());
				if (m._type == MapItemType.Page)
				{
					PageInfo pi = new PageInfo(sPagePath);
					if(m._path != sPagePath && m._path.Length>0)
					{
						if(PageInfo.Exists(sPagePath))
						{
							sPagePath = m._path;
							pi = new PageInfo(sPagePath);
						}
						else
						{
							pi = new PageInfo(m._path);
							PageInfo.Delete(m._path);
						}
					}
					if (rRequest[this.cmbPart.ClientID].ToString() == string.Empty)
					{
						rRequest[this.cmbPart.ClientID] = -1;
					}
					title = title == string.Empty ? PageInfo.CreateName(sPagePath) : title;
					PageInfo.SavePage(
						sPagePath,
						rRequest[this.txtCodeFile.ClientID].ToString(),
						rRequest[this.cmbTemplate.ClientID].ToString(),
						pi.Body,
						pi.Description,
						pi.Title.Length == 0 || pi.Title==m.Text ? title : pi.Title,
						pi.KeyWords,
						pi.Head,
						pi.Header,
						rRequest[this.chkSecure2.ClientID].ToString() == "1",
						rRequest[this.chkDeleted.ClientID].ToString() == "1",
						pi.Cache,
						Convert.ToInt32(rRequest[this.cmbPart.ClientID]),
						pi.StatusCode,
						pi.GenerateLastModified,
						pi.Fields);
					if (url == string.Empty || url == null)
						url = Path.VRoot + sPagePath + Config.Main.Extension;
				}
				m._hidden = rRequest[this.chkShow.ClientID].ToString() == "0";
				int pid = int.Parse(rRequest[this.cmbParent.ClientID].ToString().Split('#')[0]);
				int index = int.Parse(rRequest[this.cmbIndex.ClientID].ToString());
				if (pid != m.Parent.ID || index != m.Index)
				{
					MapItem p = pid == -1 ? sm : sm.All[pid];
					m.Remove();
					if (p.Items.Count > index)
						p.Items.Insert(index, m);
					else
						p.Items.Add(m);
				}
			}
			m._text = title;
			m._url = url;
			m._path = sPagePath;
			sm.Save();

			sm = new SiteMap();
			Response.Write(sm.GetItemByPath(m._path).ID);
		}

		private void DeleteItem()
		{
			if (nItemId > -1)
			{
				MapItem m = sm.All[nItemId];
				m.Remove();
				if (m._type == MapItemType.Page)
					PageInfo.Delete(sPagePath);
			}
			else if (nItemId == -1)
				sm.Remove();
			sm.Save();
		}

		private void RefreshItem()
		{
			MapItem m = null;
			if (nItemId > -1)
			{
				m = sm.All[nItemId];
				XmlBuilder.WriteField(Response, this.cmbParent.ClientID, m.Parent.ID.ToString() + "#" + m.Parent.Items.Count.ToString());
				XmlBuilder.WriteField(Response, this.cmbIndex.ClientID, m.Index.ToString());
				XmlBuilder.WriteField(Response, this.cmbType.ClientID, m._type.ToString());
				XmlBuilder.WriteField(Response, this.txtPath.ClientID, m._path);
				XmlBuilder.WriteField(Response, this.chkShow.ClientID, m._hidden ? "0" : "1");
				XmlBuilder.WriteField(Response, this.hIndex.ClientID, m.Index.ToString());
				if (m._type == MapItemType.Page)
				{
					PageInfo pi = new PageInfo(m._path);
					XmlBuilder.WriteField(Response, this.chkSecure1.ClientID, pi.IsSecure ? "0" : "1");
					XmlBuilder.WriteField(Response, this.chkSecure2.ClientID, pi.IsSecure ? "1" : "0");
					XmlBuilder.WriteField(Response, this.cmbTemplate.ClientID, pi.Template);
					XmlBuilder.WriteField(Response, this.txtCodeFile.ClientID, pi.CodeFile);
					XmlBuilder.WriteField(Response, this.cmbPart.ClientID, pi.SecurityPart.ToString());
					XmlBuilder.WriteField(Response, this.chkDeleted.ClientID, pi.Deleted ? "1" : "0");
				}
			}
			else if (nItemId == -1)
			{
				m = sm;
			}
			else
			{
				return;
			}			
			XmlBuilder.WriteField(Response, this.txtUrl.ClientID, m._url);
			XmlBuilder.WriteField(Response, this.txtText.ClientID, m._text);
		}

		private void RefreshParents()
		{
			XmlBuilder.WriteListItem(Response, "-1#" + sm.Items.Count.ToString(), sm._text);
			foreach (MapItem m in sm.All)
			{
				XmlBuilder.WriteListItem(Response, m.ID.ToString() + "#" + m.Items.Count.ToString(), new string('\"', m.Level + 1).Replace("\"", "` ") + m._text);
			}
		}
		private void RefreshTemplates()
		{
			DataTable tb = Config.GetConfigTable("template.config", "template").Copy();
			tb.DefaultView.Sort = "name ASC";
			if (!IsAdmin)
			{
				tb.DefaultView.RowFilter = "file NOT LIKE 'admin/templates/*'";
			}

			foreach (DataRowView r in tb.DefaultView)
			{
				XmlBuilder.WriteListItem(Response, r["file"].ToString(), r["name"].ToString());
			}
		}
		private void RefreshParts()
		{
			DataTable tb = SecurityManager.GetParts();
			tb.DefaultView.Sort = "partname ASC";
			if (!IsAdmin)
			{
				tb.DefaultView.RowFilter = "partid<>-3";
			}

			foreach (DataRowView r in tb.DefaultView)
			{
				XmlBuilder.WriteListItem(Response, r["partid"].ToString(), r["partname"].ToString());
			}
		}

		private void RefreshTree()
		{
			int addId = sm.All.Count + 1;
			Response.Write("treeview.add(0, -1, '" + sm.Text.Replace("'", "\\'") + "','javascript:OpenItem(-1);');");
			Response.Write("treeview.add(" + addId + ", 0, '[Новый элемент]','javascript:CreateItem(-1," + sm.Items.Count + ");','Добавить новый элемент',null,'" + icon3 + "');");
			foreach (MapItem m in sm.All)
			{
				Response.Write("treeview.add(" + (m.ID + 1) + ", " + (m.Parent.ID + 1) + ", '<img src=\\'" + (m.Hidden ? icon2 : icon1) + "\\'>" + m.Text.Replace("'", "\\'") + "','javascript:OpenItem(" + m.ID + ");','" + m.Url + "');");
				if (m._items.Count > 0)
				{
					addId++;
					Response.Write("treeview.add(" + addId + ", " + (m.ID + 1) + ", '[Новый элемент]','javascript:CreateItem(" + m.ID + "," + m.Items.Count + ",\\'" + m._path + "\\');','Добавить новый элемент в папку «" + m.Text.Replace("'", "\\'") + "»',null,'" + icon3 + "');");
				}
			}
		}

		private void SearchForNewPages()
		{
			SiteMap.Current.SearchForNewPages();
		}
		private void CorrectUrls()
		{
			SiteMap.Current.CorrectUrls();
		}
		#region xfHeader

		private void HeaderSubmit()
		{
			if (sPagePath == "")
				return;
			DataRow r = rRequest;
			PageInfo pi = new PageInfo(sPagePath);
			PageInfo.SavePage(pi.Path,
				pi.CodeFile,
				pi.Template,
				pi.Body,
				r[this.txtDescription.ClientID].ToString(),
				r[this.txtTitle.ClientID].ToString(),
				r[this.txtKeyWords.ClientID].ToString(),
				r[this.txtHead.ClientID].ToString(),
				r[this.txtHeader.ClientID].ToString(),
				pi.IsSecure,
				pi.Deleted,
				Convert.ToInt32(r[this.txtCache.ClientID]),
				pi.SecurityPart,
				Convert.ToInt32(r[this.cmbStatus.ClientID]),
				(GenerateLastModified)int.Parse(r[this.cmbLM.ClientID].ToString()),
				pi.Fields);
		}

		private void HeaderRefresh()
		{
			PageInfo pi = new PageInfo(sPagePath);
			XmlBuilder.WriteField(Response, this.txtTitle.ClientID, pi.Title);
			XmlBuilder.WriteField(Response, this.txtDescription.ClientID, pi.Description);
			XmlBuilder.WriteField(Response, this.txtKeyWords.ClientID, pi.KeyWords);
			XmlBuilder.WriteField(Response, this.txtHead.ClientID, pi.Head);
			XmlBuilder.WriteField(Response, this.txtHeader.ClientID, pi.Header);
			XmlBuilder.WriteField(Response, this.txtCache.ClientID, pi.Cache.ToString());
			XmlBuilder.WriteField(Response, this.cmbStatus.ClientID, pi.StatusCode.ToString());
			XmlBuilder.WriteField(Response, this.cmbLM.ClientID, ((int)pi.GenerateLastModified).ToString());
		}

		#endregion

		#region xfContent

		private void ContentSubmit()
		{
			if (sPagePath == "")
				return;
			DataRow r = rRequest;
			PageInfo pi = new PageInfo(sPagePath);
			PageInfo.SavePage(pi.Path,
			            pi.CodeFile,
			            pi.Template,
			            r[this.txtBody.ClientID].ToString(),
			            pi.Description,
			            pi.Title,
			            pi.KeyWords,
			            pi.Head,
			            pi.Header,
			            pi.IsSecure,
			            pi.Deleted,
			            pi.Cache,
			            pi.SecurityPart,
						pi.StatusCode,
						pi.GenerateLastModified,
			            pi.Fields);
		}

		private void ContentRefresh()
		{
			PageInfo pi = new PageInfo(sPagePath);
			XmlBuilder.WriteField(Response, this.txtBody.ClientID, pi.Body);
		}

		#endregion

		#region xfDict

		private void DictSubmit()
		{
			if (sPagePath == "")
				return;
			DataRow r = rRequest;
			PageInfo pi = new PageInfo(sPagePath);
			if (r[this.hFieldAction.ClientID].ToString() == "0")
			{
				if (r[this.txtFieldName.ClientID].ToString().Length > 0)
				{
					pi.Fields[r[this.txtFieldName.ClientID].ToString()] = r[this.txtFieldValue.ClientID];
				}
			}
			else
			{
				if (pi.Fields.ContainsKey(r[this.txtFieldName.ClientID].ToString()))
				{
					pi.Fields.Remove(r[this.txtFieldName.ClientID].ToString());
				}
			}
			PageInfo.SavePage(pi.Path,
				pi.CodeFile,
				pi.Template,
				pi.Body,
				pi.Description,
				pi.Title,
				pi.KeyWords,
				pi.Head,
				pi.Header,
				pi.IsSecure,
				pi.Deleted,
				pi.Cache,
				pi.SecurityPart,
				pi.StatusCode,
				pi.GenerateLastModified,
				pi.Fields);
		}

		private void DictRefresh()
		{
			string field = Request.QueryString["field"];
			if (field == null)
				return;
			PageInfo pi = new PageInfo(sPagePath);
			if (pi.Fields.ContainsKey(field))
			{
				XmlBuilder.WriteField(Response, this.txtFieldName.ClientID, field.ToLower());
				XmlBuilder.WriteField(Response, this.txtFieldValue.ClientID, pi.Fields[field].ToString());
			}
		}

		private void DictRefreshList()
		{
			if (sPagePath == "")
				return;
			PageInfo pi = new PageInfo(sPagePath);
			foreach (string s in pi.Fields.Keys)
			{
				XmlBuilder.WriteListItem(Response, s, s);
			}
		}

		#endregion

		#region xfTemplate

		private void TemplateSubmit()
		{
			if (sPagePath == "")
				return;
			DataRow r = rRequest;
			PageInfo pi = new PageInfo(sPagePath);
			if (pi.Template.Length > 0)
			{
				string file = Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.Template);
				StreamWriter sw = null;
				try
				{
					sw = new StreamWriter(file, false, Encoding.Default);
					sw.Write(r[this.txtTemplateBody.ClientID]);
				}
				finally
				{
					if (sw != null)
						sw.Close();
				}
			}
		}

		private void TemplateRefresh()
		{
			if (sPagePath == "")
				return;
			PageInfo pi = new PageInfo(sPagePath);
			if (pi.Template.Length > 0)
			{
				XmlBuilder.WriteField(Response, this.txtTemplatePath.ClientID, pi.Template);
				StreamReader sr = null;
				try
				{
					sr = new StreamReader(Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.Template), Encoding.Default);
					XmlBuilder.WriteField(Response, this.txtTemplateBody.ClientID, sr.ReadToEnd());
				}
				finally
				{
					if (sr != null)
						sr.Close();
				}
			}
			else
			{
				XmlBuilder.WriteField(Response, this.txtTemplatePath.ClientID, "");
				XmlBuilder.WriteField(Response, this.txtTemplateBody.ClientID, "");
			}
		}

		#endregion

		#region xfCode

		private void CodeSubmit()
		{
			if (sPagePath == "")
				return;
			DataRow r = rRequest;
			PageInfo pi = new PageInfo(sPagePath);
			if (pi.CodeFile.Length > 0)
			{
				string file = Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.CodeFile);
				StreamWriter sw = null;
				try
				{
					sw = new StreamWriter(file, false, Encoding.Default);
					sw.Write(r[this.txtCodeBody.ClientID]);
				}
				finally
				{
					if (sw != null)
						sw.Close();
				}
			}
		}

		private void CodeRefresh()
		{
			if (sPagePath == "")
				return;
			PageInfo pi = new PageInfo(sPagePath);
			XmlBuilder.WriteField(Response, this.txtCodePath.ClientID, pi.CodeFile);
			if (pi.CodeFile.Length > 0)
			{
				StreamReader sr = null;
				try
				{
					sr = new StreamReader(Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.CodeFile), Encoding.Default);
					XmlBuilder.WriteField(Response, this.txtCodeBody.ClientID, sr.ReadToEnd());
				}
				catch(Exception ex)
				{
					Config.ReportError(ex);
				}
				finally
				{
					if (sr != null)
						sr.Close();
				}
			}
			else
			{
				XmlBuilder.WriteField(Response, this.txtCodeBody.ClientID, "");
			}
		}

		#endregion

		#region xfPagex

		private void PagexSubmit()
		{
			if (sPagePath.Length == 0)
				return;
			string pagex = rRequest[this.txtPagex.ClientID].ToString().Trim();
			DataTable tbPagex = Config.GetPagex().Copy();
			DataRow[] rs = tbPagex.Select("path='" + sPagePath + "'");
			for (int i = 0; i < rs.Length; i++)
			{
				tbPagex.Rows.Remove(rs[i]);
			}
			if (pagex.Length != 0)
			{
				string[] ps = pagex.Split(',');
				DataRow r = null;
				for (int i = 0; i < ps.Length; i++)
				{
					if (ps[i].Trim().Length > 0)
					{
						r = tbPagex.NewRow();
						r["path"] = sPagePath;
						r["value"] = ps[i].Trim();
						tbPagex.Rows.Add(r);
					}
				}
			}
			Config.WriteConfigTable("pagex.config", tbPagex, "pagexes");
		}

		private void PagexRefresh()
		{
			if (sPagePath == "")
				return;
			DataRow[] rows = Config.GetPagex().Select("path='" + sPagePath + "'");
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < rows.Length; i++)
			{
				if (i > 0)
					sb.Append(",");
				sb.Append(rows[i]["value"].ToString());
			}
			XmlBuilder.WriteField(Response, this.txtPagex.ClientID, sb.ToString());
		}

		#endregion
	}
}