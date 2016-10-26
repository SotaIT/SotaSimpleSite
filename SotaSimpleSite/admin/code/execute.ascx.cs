using System;
using System.Data;
using System.Web.UI.WebControls;
using Sota.Data.Simple;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.Code.Admin
{

	/// <summary>
	///		Summary description for execute.
	/// </summary>
	public class execute : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txtCmd;
		protected System.Web.UI.WebControls.Label lblError;
		protected System.Web.UI.WebControls.PlaceHolder phGrid;
		protected System.Web.UI.WebControls.Label lblAffected;
		protected System.Web.UI.WebControls.Button cmdOk;
		protected System.Web.UI.WebControls.DropDownList cmbConnector;
		string connr = "";
		string list = "";
		private void Page_Load(object sender, System.EventArgs e)
		{
			cmbConnector.Items.Add(new ListItem("Log","log"));
			cmbConnector.Items.Add(new ListItem("Security","security"));
			DataTable lists = List.GetLists();
			foreach(DataRow r in lists.Rows)
			{
				cmbConnector.Items.Add(new ListItem("List["+r["name"]+"]","list:"+r["name"]));
			}
			
			if(Request.Form[cmbConnector.UniqueID]!=null)
			{
				connr = Request.Form[cmbConnector.UniqueID];
				if(connr.IndexOf(":")!=-1)
				{
					list = connr.Split(':')[1];
					connr = connr.Split(':')[0];
				}
			}
		}

		#region Web Form Designer generated code
		override protected void OnInit(EventArgs e)
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
			this.cmdOk.Click += new System.EventHandler(this.cmdOk_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

		private void cmdOk_Click(object sender, System.EventArgs e)
		{
			try
			{
				Connector conn = null;
				switch(connr)
				{
					case "log":
						conn = Log.Connector;
						break;
					case "security":
						conn = SecurityManager.Connector;
						break;
					case "list":
						conn = (List.Create(list)).Connector;
						break;
				}
				if(conn != null)
				{
					DbCommand cmd = conn.CreateCommand(this.txtCmd.Text);
					IDataReader reader = null;
					try
					{
						cmd.Connection.Open();
						reader = cmd.ExecuteReader();
						if(reader.RecordsAffected>0)
						{
							this.lblAffected.Text = string.Format("Обновлено {0} записей.", reader.RecordsAffected);
						}
						DataGrid dg = new DataGrid();
						this.phGrid.Controls.Add(dg);
						dg.DataSource = reader;
						dg.DataBind();
					
						while(reader.NextResult())
						{
							dg = new DataGrid();					
							this.phGrid.Controls.Add(ParseControl("<br />"));
							this.phGrid.Controls.Add(dg);
							dg.DataSource = reader;
							dg.DataBind();
						}
					}
					finally
					{
						if(reader!=null)
						{
							reader.Close();
						}
						cmd.Connection.Close();
					}
				}
			}
			catch(Exception ex)
			{
				this.lblError.Text = ex.Message;
			}

		}
	}
}
