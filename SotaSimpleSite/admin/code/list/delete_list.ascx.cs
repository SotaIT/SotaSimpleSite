using System.Data;

namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;

	/// <summary>
	///		Summary description for delete_list.
	/// </summary>
	public class delete_list : System.Web.UI.UserControl
	{
		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Request.QueryString["list"] != null)
			{
				DataTable tb = Config.GetConfigTable("lists.config", "list").Copy();
				if (tb.Columns.Contains("name"))
				{
					DataRow[] r = tb.Select("name='" + Request.QueryString["list"] + "'");
					if (r.Length > 0)
						tb.Rows.Remove(r[0]);
				}
				Config.WriteConfigTable("lists.config", tb, "root");
				Response.Redirect("~/admin/lists" + Config.Main.Extension);
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
	}
}