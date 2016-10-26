using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.Data;

	/// <summary>
	///	¬ход в контрольную панель.
	/// </summary>
	public class AdminLoginPage : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlInputHidden hRef;
		protected System.Web.UI.HtmlControls.HtmlInputText txtLogin;
		protected System.Web.UI.HtmlControls.HtmlInputCheckBox chkSave;
		protected System.Web.UI.HtmlControls.HtmlInputText txtPass;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfLogin;
		//protected bool isCompitable = false;
		private void Page_Load(object sender, System.EventArgs e)
		{
			/*if(Request.Browser.Browser.ToUpper().IndexOf("IE") > -1 
				&& (Request.Browser.MajorVersion>=6 || 
				(Request.Browser.MajorVersion==5 && Request.Browser.MinorVersion>=5)))
			{
				isCompitable = true;*/
			
				if (Request.RequestType.ToLower() == "get")
				{
					if(UserInfo.Current.IsAuthorized && UserInfo.Current.CanAccess(-2))
					{
						Response.Redirect(Config.Main.AdminDefault);
					}
					this.xfLogin.Action = Path.Full;
					if (Request.QueryString.Count > 0)
					{
						this.hRef.Value = Server.UrlEncode(Request.QueryString[0]);
					}
					else
					{
						this.hRef.Value = Server.UrlEncode(ResolveUrl(Config.Main.AdminDefault));
					}
				}
				else
				{
					Response.Clear();
					if (Request.ContentType == "text/xml")
					{
						Response.ContentEncoding = System.Text.Encoding.UTF8;
						DataSet ds = new DataSet();
						ds.ReadXml(Request.InputStream);
						if (ds.Tables.Count > 0)
						{
							DataRow r = ds.Tables[0].Rows[0];
							try
							{
								if (!UserInfo.LoginNew(r[this.txtLogin.ClientID].ToString(), r[this.txtPass.ClientID].ToString(), r[this.chkSave.ClientID].ToString()=="1"))
								{
									Response.Write("1");
								}
							}
							catch(Exception ex)
							{
								Config.ReportError(ex);
								Response.Write("1");
							}
						}
					}
					Response.End();
				}
			//}
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