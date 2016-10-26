namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for user_fields.
	/// </summary>
	public class user_fields : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgFields;
		public int iUserId;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Request.QueryString["id"] != null)
				iUserId = int.Parse(Request.QueryString["id"]);
			else
				Response.End();
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgFields_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgFields_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgFields_DeleteCommand(Request.QueryString["delete"]);
				else BindGrid();
			}
			else
			{
				dgFields_UpdateCommand();
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
			this.dgFields.DataSource = SecurityManager.GetAllUserFieldsTable(iUserId);
			this.dgFields.DataBind();
		}

		private void dgFields_AddCommand()
		{
			DataTable tb = SecurityManager.GetAllUserFieldsTable(iUserId);
			tb.Rows.InsertAt(tb.NewRow(), 0);
			this.dgFields.EditItemIndex = 0;
			this.dgFields.DataSource = tb;
			this.dgFields.DataBind();
		}

		private void dgFields_EditCommand(int itemIndex)
		{
			this.dgFields.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgFields_DeleteCommand(string field)
		{
			SecurityManager.RemoveUserField(iUserId, field);
			BindGrid();
		}

		private void dgFields_UpdateCommand()
		{
			SecurityManager.SetUserField(iUserId, Request.Form["txtField"], Request.Form["txtValue"]);
			//Редирект
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path + "?id=" + iUserId);
		}
	}
}