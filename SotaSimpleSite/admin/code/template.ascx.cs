using System;
using System.Data;
using System.Text;
using System.IO;
using System.Web;

namespace Sota.Web.SimpleSite.Code.Admin
{
	/// <summary>
	///		Summary description for template.
	/// </summary>
	public class template : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgTemplate;
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
				dgControls_UpdateCommand();
			}
		}

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
			this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion

		public string GetTemplates(string template)
		{
			return GetFiles("*.aspx", template);
		}

		public static string GetFiles(string pattern, string file)
		{
			string root = HttpContext.Current.Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter);
			StringBuilder sb = new StringBuilder();
			FillFiles(sb, root, root, pattern, file);
			return sb.ToString();
		}

		private static void FillFiles(StringBuilder sb, string root, string cur, string pattern, string file)
		{
			if(cur.ToLower()==HttpContext.Current.Request.MapPath("~/admin").ToLower())
				return;
			string[] ard = Directory.GetDirectories(cur);
			
			for (int i = 0; i < ard.Length; i++)
			{
				FillFiles(sb, root, ard[i], pattern, file);
			}
			string[] arf = Directory.GetFiles(cur, pattern);
			for (int i = 0; i < arf.Length; i++)
			{
				string f = arf[i].Substring(root.Length).Replace("\\", "/");
				sb.AppendFormat("<option value=\"{0}\"{1}>{0}</option>", f, f == file ? " selected" : "");
			}
		}


		private DataTable GetTable()
		{
			DataTable tb = Config.GetConfigTable("template.config", "template").Copy();
			if (!tb.Columns.Contains("name"))
				tb.Columns.Add("name");
			if (!tb.Columns.Contains("file"))
				tb.Columns.Add("file");
			tb.DefaultView.Sort = "name ASC";
			return tb;
		}

		private void BindGrid()
		{
			this.dgTemplate.DataSource = GetTable();
			this.dgTemplate.DataBind();
		}

		private void dgControls_AddCommand()
		{
			DataTable tb = GetTable();
			tb.Rows.InsertAt(tb.NewRow(), 0);
			this.dgTemplate.EditItemIndex = 0;
			this.dgTemplate.DataSource = tb;
			this.dgTemplate.DataBind();
		}

		private void dgControls_EditCommand(int itemIndex)
		{
			this.dgTemplate.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void Save(DataTable tb)
		{
			DataSet ds = new DataSet("root");
			ds.Tables.Add(tb);
			ds.WriteXml(Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + "template.config"));
		}

		private void Redirect()
		{
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}

		private void dgControls_DeleteCommand()
		{
			DataTable tb = GetTable();
			string where = string.Format("file='{0}' AND name='{1}'",
			                             Request.QueryString["file"],
			                             Request.QueryString["name"]);
			DataRow[] rows = tb.Select(where);
			if (rows.Length > 0)
			{
				tb.Rows.Remove(rows[0]);
				Save(tb);
			}
			Redirect();
		}

		private void dgControls_UpdateCommand()
		{
			DataTable tb = GetTable();
			DataRow row = tb.NewRow();
			row["file"] = Request.Form["cmbFile"];
			row["name"] = Request.Form["txtName"];
			tb.Rows.Add(row);
			Save(tb);
			Redirect();
		}
	}
}