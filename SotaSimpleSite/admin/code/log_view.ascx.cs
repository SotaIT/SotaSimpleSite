using System.Collections;

namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.Data;

	/// <summary>
	///		Summary description for log_view.
	/// </summary>
	public class log_view : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.DataGrid dgLogs;
		protected string type		= string.Empty;
		protected Hashtable htTypes	= new Hashtable();
		protected string[] arTypes	= new string[0];
		DataTable tb = null;
		private void Page_Load(object sender, System.EventArgs e)
		{
			if(Request.RequestType.ToUpper()=="POST" && Request.Form["chk"]!=null)
			{
				string[] arCheck = Request.Form["chk"].Split(',');
				for(int i=0;i<arCheck.Length;i++)
				{
					Log.Delete(int.Parse(arCheck[i]));
				}
				Response.Redirect(Path.Full);
			}

			DateTime from = DateTime.MinValue;
			DateTime to = DateTime.MaxValue;

			string filter = string.Empty;
			if (Request.QueryString["type"] != null)
			{
				type = Request.QueryString["type"];
				if(type.Length>0)
				{
					filter = "type='"+type+"'";
				}
			}
			if (Request.QueryString["filter"] != null)
			{
				if (Request.QueryString["filter"].Length > 0)
				{
					if (filter.Length > 0)
						filter += " AND ";
					filter += "params LIKE '*" + Request.QueryString["filter"] + "*'";
				}
			}
			if (Request.QueryString["date_f"] != null)
			{
				if (Request.QueryString["date_f"].Length > 0)
				{
//					if (filter.Length > 0)
//						filter += " AND ";
//					filter += "datetime>'" + Request.QueryString["date_f"] + "'";
					from = DateTime.Parse(Request.QueryString["date_f"]);
				}
			}
			if (Request.QueryString["date_t"] != null)
			{
				if (Request.QueryString["date_t"].Length > 0)
				{
//					if (filter.Length > 0)
//						filter += " AND ";
//					filter += "datetime<'" + Request.QueryString["date_t"] + "'";
					to = DateTime.Parse(Request.QueryString["date_t"]);
				}
			}
			tb = Log.GetByParams(null, to, from, null);

			if (Request.QueryString["sort"] != null)
			{
				switch (Request.QueryString["sort"])
				{
					case "1":
						tb.DefaultView.Sort = "datetime DESC";
						break;
					case "2":
						tb.DefaultView.Sort = "type ASC";
						break;
					case "3":
						tb.DefaultView.Sort = "type ASC, datetime DESC";
						break;
					case "4":
						tb.DefaultView.Sort = "datetime DESC, type ASC";
						break;
				}
			}

			tb.DefaultView.RowFilter = filter;
			
			foreach(DataRowView rv in tb.DefaultView)
			{
				int count = 0;
				if(htTypes.Contains(rv["type"]))
				{
					count = Convert.ToInt32(htTypes[rv["type"]]);
				}
				htTypes[rv["type"]] = count + 1;
			}
			ArrayList arr = Log.GetTypes();
			foreach(string item in arr)
			{
				if(!htTypes.Contains(item))
				{
					htTypes[item] = 0;
				}
			}
			arTypes = new string[htTypes.Count];
			htTypes.Keys.CopyTo(arTypes,0);
			Array.Sort(arTypes);
			htTypes["*"] = tb.DefaultView.Count;
			BindData();
		}

		void BindData()
		{
			this.dgLogs.DataSource = tb;
			this.dgLogs.DataBind();
			
		}
		private void dgLogs_PageIndexChanged(object source, System.Web.UI.WebControls.DataGridPageChangedEventArgs e)
		{
			this.dgLogs.CurrentPageIndex = e.NewPageIndex;
			BindData();
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
			this.Load+=new EventHandler(Page_Load);
			this.dgLogs.PageIndexChanged+=new System.Web.UI.WebControls.DataGridPageChangedEventHandler(dgLogs_PageIndexChanged);
		}

		#endregion

	}
}