namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for groups.
	/// </summary>
	public class groups : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgGroups;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgGroups_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgGroups_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgGroups_DeleteCommand(int.Parse(Request.QueryString["delete"]));
				else BindGrid();
			}
			else
			{
				dgGroups_UpdateCommand();
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

		private void BindGrid()
		{
			this.dgGroups.DataSource = SecurityManager.GetGroups();
			this.dgGroups.DataBind();
		}

		private void dgGroups_AddCommand()
		{
			DataTable tb = SecurityManager.GetGroups();
			DataRow row = tb.NewRow();
			row["groupid"] = 0;
			tb.Rows.InsertAt(row, 0);
			this.dgGroups.EditItemIndex = 0;
			this.dgGroups.DataSource = tb;
			this.dgGroups.DataBind();
		}

		private void dgGroups_EditCommand(int itemIndex)
		{
			this.dgGroups.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgGroups_DeleteCommand(int groupId)
		{
			SecurityManager.DeleteGroup(groupId);
			BindGrid();
		}

		private void dgGroups_UpdateCommand()
		{
			int nId = int.Parse(Request.Form["hId"]);
			if (nId == 0)
			{
				nId = SecurityManager.CreateGroup(Request.Form["txtGroup"]);
			}
			else
			{
				SecurityManager.UpdateGroup(nId, Request.Form["txtGroup"]);
			}
			//Редирект
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}
	}
}