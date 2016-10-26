using System.Data;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;

	/// <summary>
	///		Summary description for modules.
	/// </summary>
	public class modules : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.LinkButton lnkAdd;
		protected System.Web.UI.WebControls.TextBox txtName;
		protected System.Web.UI.WebControls.TextBox txtPath;
		protected System.Web.UI.WebControls.DataGrid dgModules;
		protected bool IsAdmin = false;

		private void Page_Load(object sender, System.EventArgs e)
		{
			IsAdmin = SecurityManager.IsUserInGroup(UserInfo.Current.UserId, -3);
			if (!IsAdmin)
			{
				this.dgModules.Columns[1].Visible = false;
				this.dgModules.Columns[2].Visible = false;
			}
			this.dgModules.DataSource = Config.GetConfigTable("modules.config", "module");
			this.dgModules.DataBind();
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
			this.dgModules.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgModules_DeleteCommand);
			this.lnkAdd.Click += new System.EventHandler(this.lnkAdd_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion

		private void dgModules_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			if (IsAdmin)
			{
				DataTable tb = Config.GetConfigTable("modules.config", "module");
				tb.Rows.RemoveAt(e.Item.ItemIndex);
				Config.WriteConfigTable("modules.config", tb, "root");
			}
			Response.Redirect(Path.Full);
		}

		private void lnkAdd_Click(object sender, System.EventArgs e)
		{
			if (IsAdmin)
			{
				DataTable tb = Config.GetConfigTable("modules.config", "module");
				if (!tb.Columns.Contains("name"))
					tb.Columns.Add("name");
				if (!tb.Columns.Contains("path"))
					tb.Columns.Add("path");
				if (!tb.Columns.Contains("name"))
					tb.Columns.Add("name");
				tb.Rows.Add(new object[] {this.txtName.Text.Trim(), this.txtPath.Text.Trim()});
				Config.WriteConfigTable("modules.config", tb, "root");
			}
			Response.Redirect(Path.Full);
		}
	}
}