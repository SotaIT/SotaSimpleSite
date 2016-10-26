using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;
	using System.Data;

	/// <summary>
	///	Главная страница списков
	/// </summary>
	public class lists : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Repeater rptLists;
		protected bool IsAdmin = false;

		private void Page_Load(object sender, System.EventArgs e)
		{
			UserInfo ui = UserInfo.Current;
			IsAdmin = ui.IsInGroup(-3);
			bool restrictedUser = false;
			if ((string)ui.Fields[Keys.UserIsRestricted] == "yes")
			{
				restrictedUser = true;
			}

	
			DataTable dt = List.GetLists();
			if (!dt.Columns.Contains("access"))
			{ 
				dt.Columns.Add("access", typeof(int));
			}
			foreach (DataRow r in dt.Rows)
			{
				r["access"] = 0;
				if (restrictedUser)
				{
					bool restrictedList = true;
					foreach (string key in ui.DataFields.Keys)
					{
						if (key.StartsWith("list[" + r["name"] + "]"))
						{
							restrictedList = false;
						}
					}
					if (restrictedList)
					{
						continue;
					}
				}
				try
				{
					List l = List.Create(r["name"].ToString(), true);
					if (ui.CanAccess(l.SecurityPart))
					{
						r["access"] = 1;
					}
				}
				catch (Exception ex)
				{
					Config.ReportError(ex);
				}
			}
			dt.DefaultView.RowFilter = "access=1";
			this.rptLists.DataSource = dt;
			this.rptLists.DataBind();
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