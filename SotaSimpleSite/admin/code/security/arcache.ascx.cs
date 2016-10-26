namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for arcache.
	/// </summary>
	public class arcache : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgRules;

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Request.QueryString["refresh"] != null)
			{
				SecurityManager.ClearAccessRuleCache();
				Response.Redirect(Path.Full.Split('?')[0]);
			}
			this.dgRules.DataSource = SecurityManager.GetAccessRuleCache();
			this.dgRules.DataBind();
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