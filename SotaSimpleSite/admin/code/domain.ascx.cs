using System.Data;

namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;

	/// <summary>
	///		Summary description for domain.
	/// </summary>
	public class DomainEditor : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgDomains;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgDomains_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgDomains_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgDomains_DeleteCommand(Request.QueryString["delete"]);
				else BindGrid();
			}
			else
			{
				dgDomains_UpdateCommand();
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

		private DataTable GetTable()
		{
			return Config.GetDomains().Copy();
		}

		private void BindGrid()
		{
			this.dgDomains.DataSource = GetTable();
			this.dgDomains.DataBind();
		}

		private void dgDomains_AddCommand()
		{
			DataTable tb = GetTable();
			DataRow row = tb.NewRow();
			tb.Rows.InsertAt(row, 0);
			this.dgDomains.EditItemIndex = 0;
			this.dgDomains.DataSource = tb;
			this.dgDomains.DataBind();
		}


		private void dgDomains_EditCommand(int itemIndex)
		{
			this.dgDomains.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgDomains_DeleteCommand(string domain)
		{
			DataTable tb = GetTable();
			DataRow[] r = tb.Select("name='" + domain + "'");
			if (r.Length > 0)
			{
				tb.Rows.Remove(r[0]);
				Config.WriteConfigTable("domain.config", tb, "domains");
			}
			Redirect();
		}

		private void dgDomains_UpdateCommand()
		{
			string domain = Request.Form["txtName"];
			if (domain == null)
				return;
			domain = domain.Trim();
			if (domain == string.Empty)
				return;
			DataRow row = null;
			DataTable tb = GetTable();
			DataRow[] r = tb.Select("name='" + domain + "'");
			if (r.Length > 0)
			{
				row = r[0];
			}
			else
			{
				row = tb.NewRow();
				tb.Rows.Add(row);
				row["name"] = domain;
			}
			row["addpath"] = Request.Form["txtAddPath"];
			row["site"] = Request.Form["txtSite"];
/*			
			string folder = Request.MapPath(Config.ConfigFolderPath+"/"+Request.Form["txtSite"]);
			if(!Directory.Exists(folder))
			{
				Directory.CreateDirectory(folder);
				Directory.CreateDirectory(Request.MapPath("~data/"+Request.Form["txtSite"]));
				string cf = Request.MapPath(Config.ConfigFolderPath);
				string[] fs = Directory.GetFiles(cf);
				foreach(string f in fs)
				{
					File.Copy(f,f.Replace(cf,folder));
				}
				
			}*/
			Config.WriteConfigTable("domain.config", tb, "domains");
			Redirect();
		}

		private void Redirect()
		{
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}
	}
}