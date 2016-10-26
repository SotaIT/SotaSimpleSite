namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for actions.
	/// </summary>
	public class actions : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgActions;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgActions_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgActions_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgActions_DeleteCommand(int.Parse(Request.QueryString["delete"]));
				else BindGrid();
			}
			else
			{
				dgActions_UpdateCommand();
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
			this.dgActions.DataSource = SecurityManager.GetActions();
			this.dgActions.DataBind();
		}

		private void dgActions_AddCommand()
		{
			DataTable tb = SecurityManager.GetActions();
			DataRow row = tb.NewRow();
			row["actionid"] = 0;
			tb.Rows.InsertAt(row, 0);
			this.dgActions.EditItemIndex = 0;
			this.dgActions.DataSource = tb;
			this.dgActions.DataBind();
		}

		private void dgActions_EditCommand(int itemIndex)
		{
			this.dgActions.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgActions_DeleteCommand(int ActionId)
		{
			SecurityManager.DeleteAction(ActionId);
			BindGrid();
		}

		private void dgActions_UpdateCommand()
		{
			int nId = int.Parse(Request.Form["hId"]);
			if (nId == 0)
			{
				nId = SecurityManager.CreateAction(Request.Form["txtVar"], Request.Form["txtAction"]);
			}
			else
			{
				SecurityManager.UpdateAction(nId, Request.Form["txtVar"], Request.Form["txtAction"]);
			}
			//Редирект
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}

	}
}