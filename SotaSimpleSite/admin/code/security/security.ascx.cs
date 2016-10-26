namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;

	/// <summary>
	///	Редактирование безопасности.
	/// </summary>
	public class SecurityEditor : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.PlaceHolder phItem;
		public string sTab = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			string pagex = PageInfo.Current.Pagex;
			sTab = PageInfo.Current.Path;
			if (pagex.Length > 0)
			{
				sTab = sTab.Substring(pagex.Length - 1);
			}
			switch (sTab)
			{
				case "users":
					this.phItem.Controls.Add(this.LoadControl("users.ascx"));
					break;
				case "parts":
					this.phItem.Controls.Add(this.LoadControl("parts.ascx"));
					break;
				case "actions":
					this.phItem.Controls.Add(this.LoadControl("actions.ascx"));
					break;
				case "groups":
					this.phItem.Controls.Add(this.LoadControl("groups.ascx"));
					break;
				case "rules":
					this.phItem.Controls.Add(this.LoadControl("rules.ascx"));
					break;
				case "cache":
					this.phItem.Controls.Add(this.LoadControl("arcache.ascx"));
					break;
				case "online":
					this.phItem.Controls.Add(this.LoadControl("online.ascx"));
					break;
				default:
					sTab = "";
					break;
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