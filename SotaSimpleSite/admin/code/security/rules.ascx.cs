namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///		Summary description for rules.
	/// </summary>
	public class rules : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgRules;
		public string img = "";

		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
					dgRules_AddCommand();
				else if (Request.QueryString["edit"] != null)
					dgRules_EditCommand(int.Parse(Request.QueryString["edit"]));
				else if (Request.QueryString["delete"] != null)
					dgRules_DeleteCommand();
				else BindGrid();
			}
			else
			{
				dgRules_UpdateCommand();
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
			this.dgRules.DataSource = SecurityManager.GetAccessRuleList();
			this.dgRules.DataBind();
		}

		private void dgRules_AddCommand()
		{
			DataTable tb = SecurityManager.GetAccessRuleList();
			DataRow row = tb.NewRow();
			row["partid"] = 0;
			row["groupid"] = 0;
			row["actionid"] = 0;
			tb.Rows.InsertAt(row, 0);
			this.dgRules.EditItemIndex = 0;
			this.dgRules.DataSource = tb;
			this.dgRules.DataBind();
		}

		private void dgRules_EditCommand(int itemIndex)
		{
			this.dgRules.EditItemIndex = itemIndex;
			BindGrid();
		}

		private void dgRules_DeleteCommand()
		{
			SecurityManager.RemoveAccessRule(int.Parse(Request.QueryString["group"]),
			                                 int.Parse(Request.QueryString["part"]),
			                                 int.Parse(Request.QueryString["action"]));
			BindGrid();
		}

		private void dgRules_UpdateCommand()
		{
			int partId = int.Parse(Request.Form["cmbPart"]);
			int groupId = int.Parse(Request.Form["cmbGroup"]);
			int actionId = int.Parse(Request.Form["cmbAction"]);
			if (!IsInternalRule(partId, groupId, actionId))
			{
				SecurityManager.RemoveAccessRule(groupId, partId, actionId);
				SecurityManager.AddAccessRule(groupId, partId, actionId);
			}
			//Редирект
			string path = Path.Full;
			path = path.Substring(0, path.IndexOf(Keys.UrlParamPageDelimiter[0]));
			Response.Redirect(path);
		}

		public string GetPartList(string partId)
		{
			DataRowCollection rows = SecurityManager.GetParts().Rows;
			string res = "";
			for (int i = 0; i < rows.Count; i++)
			{
				res += "<option value=\"" + rows[i]["partid"].ToString() + "\"";
				if (rows[i]["partid"].ToString() == partId)
					res += " selected";
				res += ">" + rows[i]["partname"].ToString() + "</option>";
			}
			return res;
		}

		public string GetGroupList(string groupId)
		{
			DataRowCollection rows = SecurityManager.GetGroups().Rows;
			string res = "";
			for (int i = 0; i < rows.Count; i++)
			{
				res += "<option value=\"" + rows[i]["groupid"].ToString() + "\"";
				if (rows[i]["groupid"].ToString() == groupId)
					res += " selected";
				res += ">" + rows[i]["groupname"].ToString() + "</option>";
			}
			return res;
		}

		public string GetActionList(string actionId)
		{
			DataRowCollection rows = SecurityManager.GetActions().Rows;
			string res = "";
			for (int i = 0; i < rows.Count; i++)
			{
				res += "<option value=\"" + rows[i]["actionid"].ToString() + "\"";
				if (rows[i]["actionid"].ToString() == actionId)
					res += " selected";
				res += ">" + rows[i]["actionname"].ToString() + "</option>";
			}
			return res;
		}

		public bool IsInternalRule(int partId, int groupId, int actionId)
		{
			return partId < 1 && groupId < 1 && actionId < 1;
		}

	}
}