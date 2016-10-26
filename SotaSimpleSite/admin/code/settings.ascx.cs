using System.Collections;
using System.Text;
using System.Web.UI.WebControls;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.admin.code
{
	using System;
	using System.Data;

	/// <summary>
	///		Summary description for settings.
	/// </summary>
	public class settings : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.TextBox txtConStr;
		protected System.Web.UI.WebControls.TextBox txtRedirectAll;
		protected System.Web.UI.WebControls.CheckBox chkEnableAuth;
        protected System.Web.UI.WebControls.TextBox txtPageCache;
        protected System.Web.UI.WebControls.TextBox txtOnline;
        protected System.Web.UI.WebControls.TextBox txtSeparator;
		protected System.Web.UI.WebControls.TextBox txtExt;
		protected System.Web.UI.WebControls.TextBox txtAdminPass;
		protected System.Web.UI.WebControls.TextBox txtManagerPass;
		protected System.Web.UI.WebControls.TextBox txtNewAdminPass;
		protected System.Web.UI.WebControls.TextBox txtNewManagerPass;
		protected System.Web.UI.WebControls.TextBox txtLoginPath;
		protected System.Web.UI.WebControls.TextBox txtImg;
		protected System.Web.UI.WebControls.TextBox txtCss;
		protected System.Web.UI.WebControls.TextBox txtScript;
		protected System.Web.UI.WebControls.TextBox txtFiles;
		protected System.Web.UI.WebControls.CheckBox chkLogRedirect;
		protected System.Web.UI.WebControls.CheckBox chkLogError;
		protected System.Web.UI.WebControls.CheckBox chkLogDownload;
		protected System.Web.UI.WebControls.DropDownList cmbTimeZone;
		protected System.Web.UI.WebControls.Button cmdSave;
		protected System.Web.UI.WebControls.Button cmdClearCache;
		protected System.Web.UI.WebControls.TextBox txtEncKey;
		protected System.Web.UI.WebControls.TextBox txtEncVI;
		protected System.Web.UI.WebControls.DropDownList cmbEncLevel;
		protected System.Web.UI.WebControls.CheckBox chkOff;
		protected System.Web.UI.WebControls.CheckBox chkSeoError;
		protected Hashtable custom_settings = new Hashtable();

		private void Page_Load(object sender, System.EventArgs e)
		{
			if(!this.IsPostBack)
			{
				FillData();
			}
			FillCustom();
		}

		void FillCustom()
		{
			string[] custom = Config.Main.GetConfig(Keys.KeyMainCustom).Split('&');
			if(custom[0].Length > 0)
			{
				foreach(string key in custom)
				{
					string[] values = key.Split('=');
					custom_settings[values[0]] = values[1];
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
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			this.Load += new System.EventHandler(this.Page_Load);
			this.cmdClearCache.Click+=new EventHandler(cmdClearCache_Click);
		}
		#endregion

		private void FillData()
		{
			Config.FillDropDownList(this.cmbTimeZone);

			Config.MainConfig m				= Config.Main;
			this.txtConStr.Text				= m.ConnectionString;
			this.txtRedirectAll.Text		= m.RedirectAll;
			this.chkEnableAuth.Checked		= m.AuthorizationEnabled;
			this.txtExt.Text				= m.Extension;
            this.txtPageCache.Text          = m.PageCache.ToString();
            this.txtOnline.Text             = m.OnlineTimeOut.ToString();
            this.txtSeparator.Text          = m.TitleSeparator;
			this.txtAdminPass.Text			= m.AdminPassword;
			this.txtManagerPass.Text		= m.ManagerPassword;
			this.txtLoginPath.Text			= m.LoginPage;
			this.txtImg.Text				= m.Images;
			this.txtCss.Text				= m.Css;
			this.txtScript.Text				= m.Script;
			this.txtFiles.Text				= m.Files;
			this.chkLogError.Checked		= m.LogError;
			this.chkLogRedirect.Checked		= m.LogRedirect;
			this.chkLogDownload.Checked		= m.LogDownload;
			this.chkOff.Checked				= m.Off;
			this.chkSeoError.Checked		= m.SeoError;
			this.txtEncKey.Text				= m.GetConfig(Keys.KeyEncryptionKey);
			this.txtEncVI.Text				= m.GetConfig(Keys.KeyEncryptionVI);

			SetDDLValue(this.cmbTimeZone, m.TimeZone.ToString());
			SetDDLValue(this.cmbEncLevel, m.GetConfig(Keys.KeyEncryptionLevel));
		}
		private void SetDDLValue(DropDownList ddl, string value)
		{
			ListItem li = ddl.Items.FindByValue(value);
			if(li != null)
			{
				li.Selected = true;
			}
		}
		private void cmdSave_Click(object sender, System.EventArgs e)
		{
			DataTable tb = Config.GetMain();
			DataRow r = tb.Rows[0];
			r[Keys.KeyMainConnectionString]		= this.txtConStr.Text.Trim();
			r[Keys.KeyMainRedirectAll]			= this.txtRedirectAll.Text.Trim();
			r[Keys.KeyMainEnableAuthorization]	= this.chkEnableAuth.Checked ? "1" : "0";
			r[Keys.KeyMainTimeZone]				= int.Parse(Request.Form[this.cmbTimeZone.UniqueID]);
			r[Keys.KeyMainExtension]			= this.txtExt.Text;
            r[Keys.KeyMainPageCache]            = int.Parse(this.txtPageCache.Text);
            r[Keys.KeyMainOnlineTimeOut]        = int.Parse(this.txtOnline.Text);
            r[Keys.KeyMainTitleSeparator]       = this.txtSeparator.Text;
			if(!Util.IsBlank(this.txtNewAdminPass.Text))
			{
				r[Keys.KeyMainAdminPassword]	= SecurityManager.EncryptPassword(this.txtNewAdminPass.Text);
			}
			else
			{
				r[Keys.KeyMainAdminPassword]	= this.txtAdminPass.Text;
			}
			if(!Util.IsBlank(this.txtNewManagerPass.Text))
			{
				r[Keys.KeyMainManagerPassword]	= SecurityManager.EncryptPassword(this.txtNewManagerPass.Text);
			}
			else
			{
				r[Keys.KeyMainManagerPassword]	= this.txtManagerPass.Text;
			}
			r[Keys.KeyMainLoginPage]			= this.txtLoginPath.Text.Trim();
			r[Keys.KeyMainImages]				= this.txtImg.Text.Trim();
			r[Keys.KeyMainCss]					= this.txtCss.Text.Trim();
			r[Keys.KeyMainScript]				= this.txtScript.Text.Trim();
			r[Keys.KeyMainFiles]				= this.txtFiles.Text.Trim();
			r[Keys.KeyMainLogError]				= this.chkLogError.Checked ? "1" : "0";
			r[Keys.KeyMainOff]					= this.chkOff.Checked ? "1" : "0";
			r[Keys.KeyMainLogRedirect]			= this.chkLogRedirect.Checked ? "1" : "0";
			r[Keys.KeyMainLogDownload]			= this.chkLogDownload.Checked ? "1" : "0";
			r[Keys.KeyMainSeoError]				= this.chkSeoError.Checked ? "1" : "0";
			r[Keys.KeyEncryptionKey]			= this.txtEncKey.Text;
			r[Keys.KeyEncryptionVI]				= this.txtEncVI.Text;
			r[Keys.KeyEncryptionLevel]			= int.Parse(Request.Form[this.cmbEncLevel.UniqueID]);
			
			StringBuilder settings = new StringBuilder();
			foreach(string key in Request.Form.AllKeys)
			{
				if(key.StartsWith("custom_"))
				{
					if(settings.Length > 0)
					{
						settings.Append("&");
					}

					settings.Append(key.Substring("custom_".Length));
					settings.Append("=");
					settings.Append(Request.Form[key]);
				}
			}
			r[Keys.KeyMainCustom] = settings.ToString();

			Config.WriteConfigTable("main.config", tb, "main");
			
			Response.Redirect(Path.Full);
		}

		private void cmdClearCache_Click(object sender, EventArgs e)
		{
			PageInfo.ClearCache();
			Response.Redirect("~/admin/default.aspx");
		}
	}
}
