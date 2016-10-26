using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace Sota.Web.SimpleSite.Code.Admin
{
	/// <summary>
	///		Summary description for controls.
	/// </summary>
	public class controls : System.Web.UI.UserControl
	{
		#region Web Form Designer generated code

		protected override void OnInit(EventArgs e)
		{
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}

		/// <summary>
		///		Required method for Designer support - do not modify
		///		the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.Load+=new EventHandler(Page_Load);
		}

		#endregion

		protected System.Web.UI.WebControls.DataGrid dgControls;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgControls_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgControls_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgControls_DeleteCommand();
				else
					BindGrid();
			}
			else
			{
				if (Request.ContentType == "text/xml")
				{
					XmlFormRefreshPHList();
				}
				else
					dgControls_UpdateCommand();
			}
		}


		private void XmlFormRefreshPHList()
		{
			Response.Clear();
			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentType = "text/xml";
			Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
			Response.Write("<response>");
			string template = Request.QueryString["template"];
			if (template == null)
			{
				string page = Request.QueryString["page"];
				PageInfo pi = new PageInfo(page);
				template = pi.Template;
			}
			string file = Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + template);
			if (File.Exists(file))
			{
				string text = "";
				StreamReader sr = null;
				try
				{
					sr = new StreamReader(file, Encoding.Default);
					text = sr.ReadToEnd();
				}
				finally
				{
					if (sr != null)
						sr.Close();
				}
				Regex rex = new Regex(@"<asp:placeholder[^>]*id=""([^""]+)""[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				MatchCollection mc = rex.Matches(text);
				foreach (Match m in mc)
				{
					if (m.Groups.Count > 1)
					{
						Response.Write(String.Format("<option value=\"{0}\" text=\"{0}\"/>", m.Groups[1].Value));
					}
				}
				rex = new Regex(@"<asp:content[^>]*contentplaceholderid=""([^""]+)""[^>]*>", RegexOptions.IgnoreCase | RegexOptions.Singleline);
				mc = rex.Matches(text);
				foreach (Match m in mc)
				{
					if (m.Groups.Count > 1)
					{
						Response.Write(String.Format("<option value=\"{0}\" text=\"{0}\"/>", m.Groups[1].Value));
					}
				}
				/*rex = new Regex("<asp:placeholder[^>]*>[^>]*</asp:placeholder>", RegexOptions.IgnoreCase);
				mc = rex.Matches(text);
				foreach (Match m in mc)
				{
					string s = m.Value;
					int i = s.ToLower().IndexOf("id=\"") + "id=\"".Length;
					string c = s.Substring(i, s.IndexOf("\"", i) - i);
					Response.Write(String.Format("<option value=\"{0}\" text=\"{0}\"/>", c));
				}*/
			}
			Response.Write("</response>");
			Response.End();
		}

		public string GetTemplateName(string template)
		{
			return GetItemName("template.config", "template", template);
		}

		public string GetControlName(string control)
		{
			return GetItemName("control.config", "control", control);
		}

		public string GetItemName(string fileName, string tableName, string item)
		{
			DataTable tb = GetConfigTable(fileName, tableName);
			DataRow[] rows = tb.Select("file='" + item + "'");
			if (rows.Length > 0)
			{
				return rows[0]["name"].ToString();
			}
			return item;
		}

		public string GetTemplates(string template)
		{
			return GetItems("template.config", "template", template);
		}

		public string GetControls(string control)
		{
			return GetItems("control.config", "control", control);
		}

		private string GetItems(string fileName, string tableName, string selected)
		{
			DataTable tb = GetConfigTable(fileName, tableName);
			DataRow[] rows = tb.Select("", "name ASC");
			StringBuilder sb = new StringBuilder();
			foreach (DataRow row in rows)
			{
				sb.AppendFormat("<option value=\"{0}\"{1}>{2}</option>", row["file"], selected == row["file"].ToString() ? " selected" : "", row["name"]);
			}
			return sb.ToString();
		}

		private DataTable GetConfigTable(string fileName, string tableName)
		{
			DataTable tb = Config.GetConfigTable(fileName, tableName);
			if (!tb.Columns.Contains("name"))
				tb.Columns.Add("name");
			if (!tb.Columns.Contains("file"))
				tb.Columns.Add("file");
			return tb;
		}

		public string GetPages(string page)
		{
			string root = Request.MapPath(Config.Main.Data);
			string[] ar = Directory.GetFiles(root, "*" + Keys.ConfigExtension);
			StringBuilder sb = new StringBuilder();
			for (int i = 0; i < ar.Length; i++)
			{
				string p = PageInfo.DotToSlash(ar[i].Remove(ar[i].Length - Keys.ConfigExtension.Length, Keys.ConfigExtension.Length).Substring(root.Length + 1));
				if (!p.StartsWith(Config.Main.AdminFolderName + Keys.UrlPathDelimiter))
					sb.AppendFormat("<option value=\"{0}\"{1}>{0}</option>", p, p == page ? " selected" : "");
			}
			return sb.ToString();
		}


		private DataTable GetTable()
		{
			DataTable tb = Config.GetConfigTable("controls.config", "load").Copy();
			if (!tb.Columns.Contains("template"))
				tb.Columns.Add("template");
			if (!tb.Columns.Contains("page"))
				tb.Columns.Add("page");
			if (!tb.Columns.Contains("placeholder"))
				tb.Columns.Add("placeholder");
			if (!tb.Columns.Contains("control"))
				tb.Columns.Add("control");
			if (!tb.Columns.Contains("allow"))
				tb.Columns.Add("allow");
			if (!tb.Columns.Contains("order"))
				tb.Columns.Add("order");
			tb.Columns["order"].DefaultValue = 1;
			tb.DefaultView.Sort = "template ASC, page ASC, placeholder ASC, order ASC";
			return tb;
		}

		private void BindGrid()
		{
			this.dgControls.DataSource = GetTable();
			this.dgControls.DataBind();
		}

		private void dgControls_AddCommand()
		{
			DataTable tb = GetTable();
			tb.Rows.InsertAt(tb.NewRow(), 0);
			this.dgControls.EditItemIndex = 0;
			this.dgControls.DataSource = tb;
			this.dgControls.DataBind();
		}

		private void dgControls_EditCommand(int itemIndex)
		{
			this.dgControls.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void Save(DataTable tb)
		{
			DataSet ds = new DataSet("root");
			ds.Tables.Add(tb);
			ds.WriteXml(Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + "controls.config"));
		}

		private void Redirect()
		{
			string path = Sota.Web.SimpleSite.Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);

		}

		private void dgControls_DeleteCommand()
		{
			if (Request.QueryString["control"].Length > 0)
			{
				DataTable tb = GetTable();
				string where = string.Format("page='{0}' AND template='{1}' AND control='{2}' AND placeholder='{3}'",
				                             Request.QueryString["page"],
				                             Request.QueryString["template"],
				                             Request.QueryString["control"],
				                             Request.QueryString["placeholder"]);
				DataRow[] rows = tb.Select(where);
				if (rows.Length > 0)
				{
					tb.Rows.Remove(rows[0]);
					Save(tb);
				}
			}
			Redirect();
		}

		private void dgControls_UpdateCommand()
		{
			if (Request.Form["cmbPH"] == null || Request.Form["cmbControl"] == null)
				Redirect();
			if (Request.Form["cmbPH"].Trim().Length == 0 || Request.Form["cmbControl"].Trim().Length == 0)
				Redirect();
			DataTable tb = GetTable();
			string where = string.Format("page='{0}' AND template='{1}' AND control='{2}' AND placeholder='{3}'",
				Request.Form["cmbObject"] == "page" ? Request.Form["cmbPage"] : "",
				Request.Form["cmbObject"] == "page" ? "" : Request.Form["cmbTemplate"],
				Request.Form["cmbControl"],
				Request.Form["cmbPH"]);
			DataRow[] rows = tb.Select(where);
			DataRow row = null;
			if (rows.Length > 0)
			{
				row = rows[0];
			}
			else
			{
				row = tb.NewRow();
				tb.Rows.Add(row);
			}
			if (Request.Form["cmbObject"] == "page")
			{
				if (Util.IsBlank(Request.Form["cmbPage"]))
					Redirect();
				row["page"] = Request.Form["cmbPage"];
				row["template"] = string.Empty;
			}
			else
			{
				if (Util.IsBlank(Request.Form["cmbTemplate"]))
					Redirect();
				row["page"] = string.Empty;
				row["template"] = Request.Form["cmbTemplate"];
			}
			row["placeholder"] = Request.Form["cmbPH"];
			row["control"] = Request.Form["cmbControl"];
			row["allow"] =  Request.Form["chkDisallow"] == null ? "1" : "0";
			int order = 1;
			try
			{
				order = int.Parse(Request.Form["txtOrder"]);
			}
			catch(Exception ex)
			{
				Config.ReportError(ex);
			}
			row["order"] = order;
			Save(tb);

			Redirect();
		}
	}
}