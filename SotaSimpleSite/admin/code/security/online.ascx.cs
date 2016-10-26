using System.Data;

namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;

	/// <summary>
	///		Summary description for online.
	/// </summary>
	public class online : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgOnline;
		DataTable tb = Sota.Web.SimpleSite.Security.UserInfo.WhoIsOnline().Copy();

		private void Page_Load(object sender, System.EventArgs e)
		{
			dgOnline.DataSource = tb;
			dgOnline.DataBind();
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
			this.dgOnline.SortCommand += new System.Web.UI.WebControls.DataGridSortCommandEventHandler(this.dgOnline_SortCommand);
			this.Load += new System.EventHandler(this.Page_Load);
		}

		#endregion

		private void dgOnline_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
		{
			tb.DefaultView.Sort = e.SortExpression;
			dgOnline.DataSource = tb;
			dgOnline.DataBind();
		}
	}
}