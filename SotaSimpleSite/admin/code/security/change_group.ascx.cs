namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for add_group_d.
	/// </summary>
	public class change_group : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlSelect cmbGroups;

		private void Page_Load(object sender, System.EventArgs e)
		{
			this.cmbGroups.DataSource = SecurityManager.GetGroups();
			this.cmbGroups.DataTextField = "groupname";
			this.cmbGroups.DataValueField = "groupid";
			this.cmbGroups.DataBind();
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