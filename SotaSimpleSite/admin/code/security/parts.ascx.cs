namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for parts.
	/// </summary>
	public class parts : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgParts;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgParts_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgParts_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgParts_DeleteCommand(int.Parse(Request.QueryString["delete"]));
				else BindGrid();
			}
			else
			{
				dgParts_UpdateCommand();
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
			this.dgParts.DataSource = SecurityManager.GetParts();
			this.dgParts.DataBind();
		}

		private void dgParts_AddCommand()
		{
			DataTable tb = SecurityManager.GetParts();
			DataRow row = tb.NewRow();
			row["partid"] = 0;
			tb.Rows.InsertAt(row, 0);
			this.dgParts.EditItemIndex = 0;
			this.dgParts.DataSource = tb;
			this.dgParts.DataBind();
		}

		private void dgParts_EditCommand(int itemIndex)
		{
			this.dgParts.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgParts_DeleteCommand(int partId)
		{
			SecurityManager.DeletePart(partId);
			BindGrid();
		}

		private void dgParts_UpdateCommand()
		{
			int nId = int.Parse(Request.Form["hId"]);
			if (nId == 0)
			{
				nId = SecurityManager.CreatePart(Request.Form["txtPart"]);
			}
			else
			{
				SecurityManager.UpdatePart(nId, Request.Form["txtPart"]);
			}
			//Редирект
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}

	}
}