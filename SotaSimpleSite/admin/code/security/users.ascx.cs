namespace Sota.Web.SimpleSite.Code.Admin.Security
{
	using System;
	using System.Data;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///	Редактирование информации о пользователе.
	/// </summary>
	public class users : System.Web.UI.UserControl
	{
		protected Sota.Web.UI.WebControls.RichRepeater dgUsers;
		protected System.Data.DataRow rEdit = null;
		public string img = "";
		DataTable tb = SecurityManager.GetUsers();
		protected string searchQS = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
			img = ResolveUrl("~/admin.ashx?img=security/buttons");
			if(!Page.IsPostBack && Request.QueryString["page"] != null)
			{
				this.dgUsers.PageNumber = int.Parse(Request.QueryString["page"]);
			}
			BindGrid();
			if (Request.RequestType.ToLower() == "get")
			{
				if (Request.QueryString["new"] != null)
				{
					dgUsers_AddCommand();
				}
				else if (Request.QueryString["edit"] != null)
				{
					dgUsers_EditCommand(int.Parse(Request.QueryString["edit"]), int.Parse(Request.QueryString["index"]));
				}
				else if (Request.QueryString["delete"] != null)
				{
					dgUsers_DeleteCommand(int.Parse(Request.QueryString["delete"]));
				}
			}
			else
			{
				if(Request.Form["hsave"]=="1")
				{
					dgUsers.PageNumber = int.Parse(Request.Form["hpage"]);
					dgUsers_UpdateCommand();
				}
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
			tb.DefaultView.RowFilter = "";
			searchQS = "";
			if (!Util.IsBlank(Request.QueryString["ts_login"]))
			{
				if (tb.DefaultView.RowFilter.Length > 0)
				{
					tb.DefaultView.RowFilter += " AND ";
				}
				tb.DefaultView.RowFilter += "login LIKE '" + Request.QueryString["ts_login"].Replace("'", "''") + "'";
				searchQS += "&ts_login=" + Request.QueryString["ts_login"].Replace("\"", "&quot;");
			}
			if (!Util.IsBlank(Request.QueryString["ts_email"]))
			{
				if (tb.DefaultView.RowFilter.Length > 0)
				{
					tb.DefaultView.RowFilter += " AND ";
				}
				tb.DefaultView.RowFilter += "email LIKE '" + Request.QueryString["ts_email"].Replace("'", "''") + "'";
				searchQS += "&ts_email=" + Request.QueryString["ts_email"].Replace("\"", "&quot;");
			}
			this.dgUsers.DataSource = tb;
			this.dgUsers.DataBind();
		}

		public string GetUserGroups(string userId)
		{
			DataRow[] rows = SecurityManager.GetUserGroupList(int.Parse(userId)).Select("");
			string res = "";
			for (int i = 0; i < rows.Length; i++)
			{
				res += rows[i]["groupname"].ToString() + ", ";
			}
			return res.TrimEnd(new char[] {',', ' '});
		}

		public string GetUserGroupIds(string userId)
		{
			DataRow[] rows = SecurityManager.GetUserGroupList(int.Parse(userId)).Select("");
			string res = ",";
			for (int i = 0; i < rows.Length; i++)
			{
				res += rows[i]["groupid"].ToString() + ",";
			}
			return res;
		}

		public string GetUserGroupsList(string userId)
		{
			DataRow[] rows = SecurityManager.GetUserGroupList(int.Parse(userId)).Select("");
			string res = "";
			for (int i = 0; i < rows.Length; i++)
			{
				res += "<option value=\"" + rows[i]["groupid"].ToString() + "\">" + rows[i]["groupname"].ToString() + "</option>";
			}
			return res;
		}

		private void dgUsers_AddCommand()
		{
			DataRow r = tb.NewRow();
			r["userid"] = 0;
			r["enabled"] = true;
			tb.Rows.InsertAt(r, 0);
			dgUsers.EditItemIndex = 0;
			BindGrid();
		}

		private void dgUsers_EditCommand(int userId, int itemIndex)
		{
			DataRow[] rs = tb.Select("userid=" + userId);
			if (rs.Length > 0)
			{
				dgUsers.EditItemIndex = itemIndex;
				BindGrid();
			}
		}

		private void dgUsers_DeleteCommand(int userId)
		{
			DataRow[] rs = tb.Select("userid=" + userId);
			if (rs.Length > 0)
			{
				SecurityManager.DeleteUser(userId);
				tb.Rows.Remove(rs[0]);
				BindGrid();
			}
		}

		private void dgUsers_UpdateCommand()
		{
			int nId = int.Parse(Request.Form["hId"]);
			if (nId == 0)
			{
				nId = SecurityManager.CreateUser(Request.Form["txtLogin"],
				                                 Request.Form["txtPassword"],
				                                 Request.Form["txtEmail"],
				                                 Request.Form["chkEnabled"] != null);
			}
			else
			{
				SecurityManager.UpdateUser(nId,
				                           Request.Form["txtLogin"],
				                           Request.Form["txtEmail"],
				                           Request.Form["chkEnabled"] != null);
				if (Request.Form["txtPassword"].Length > 0)
					SecurityManager.ChangeUserPassword(nId, Request.Form["txtPassword"]);
				SecurityManager.RemoveUserFromAllGroups(nId);
			}
			string[] ag = Request.Form["hGroupIds"].Trim(',').Split(',');
			foreach (string g in ag)
			{
				if (g.Length > 0)
				{
					int gi = int.Parse(g);
					if (gi > 0 || nId > 0 || gi != nId)
						SecurityManager.AddUserToGroup(nId, gi);
				}
			}
			//Редирект
			Response.Redirect(Path.Full.Split('?')[0]+"?page=" + this.dgUsers.PageNumber + searchQS);
		}
	}
}