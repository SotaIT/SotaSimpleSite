using System;
using System.Data;

namespace Sota.Web.SimpleSite.Code.Admin
{
	/// <summary>
	///		Summary description for control.
	/// </summary>
	public class control : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgControl;
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

		public string GetControls(string control)
		{
			return template.GetFiles("*.ascx", control);
		}

		private DataTable GetTable()
		{
			DataTable tb = Config.GetConfigTable("control.config", "control").Copy();
			if (!tb.Columns.Contains("name"))
				tb.Columns.Add("name");
			if (!tb.Columns.Contains("file"))
				tb.Columns.Add("file");
			tb.DefaultView.Sort = "name ASC";
			return tb;
		}

		private void BindGrid()
		{
			this.dgControl.DataSource = GetTable();
			this.dgControl.DataBind();
		}

		private void dgControls_AddCommand()
		{
			DataTable tb = GetTable();
			tb.Rows.InsertAt(tb.NewRow(), 0);
			this.dgControl.EditItemIndex = 0;
			this.dgControl.DataSource = tb;
			this.dgControl.DataBind();
		}

		private void dgControls_EditCommand(int itemIndex)
		{
			this.dgControl.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void Save(DataTable tb)
		{
			DataSet ds = new DataSet("root");
			ds.Tables.Add(tb);
			ds.WriteXml(Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + "control.config"));
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