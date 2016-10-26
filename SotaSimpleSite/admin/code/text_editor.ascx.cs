namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.Text;
	using System.IO;

	/// <summary>
	///	Текстовый редактор.
	/// </summary>
	public class WebTextEditor : System.Web.UI.UserControl
	{
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
			this.cmdSave.Click += new System.EventHandler(this.cmdSave_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion

		protected System.Web.UI.HtmlControls.HtmlInputText txtPath;
		protected System.Web.UI.HtmlControls.HtmlTextArea txtBody;
		protected System.Web.UI.WebControls.Button cmdSave;
		private Encoding enc = Encoding.Default;

		private void Page_Load(object sender, EventArgs e)
		{
			if (!this.IsPostBack)
			{
				Refresh();
			}
		}

		private void Refresh()
		{
			string sPath = Request.QueryString["path"];
			if (sPath == null || sPath == string.Empty)
				return;
			string file = Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + sPath);
			if (IsXmlFile(file))
				enc = Encoding.UTF8;
			if (File.Exists(file))
			{
				StreamReader sr = null;
				try
				{
					sr = new StreamReader(file, enc);
					//if(Util.GetRuntimeVersion().IndexOf("2.0.")==0)
					//{
						this.txtBody.Value = sr.ReadToEnd();
					//}
					//else
					//{
					//	this.txtBody.Value = Server.HtmlEncode(sr.ReadToEnd());
					//}
					this.txtPath.Value = sPath;
				}
				finally
				{
					if (sr != null)
						sr.Close();
				}
			}

		}

		private void cmdSave_Click(object sender, System.EventArgs e)
		{
			if (this.txtPath.Value.Length == 0)
				return;
			string file = Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + this.txtPath.Value);
			if (IsXmlFile(file))
				enc = Encoding.UTF8;
			StreamWriter sw = null;
			try
			{
				sw = new StreamWriter(file, false, enc);
				//if(Util.GetRuntimeVersion().IndexOf("2")==0)
				//{
                    sw.Write(this.txtBody.Value);
                //}
				//else
				//{
                //    sw.Write(Server.HtmlDecode(this.txtBody.Value));
                //}
			}
			finally
			{
				if (sw != null)
					sw.Close();
			}
			Response.Redirect(Sota.Web.SimpleSite.Path.Full.Split('?')[0]+"?path="+Request.QueryString["path"]+"&pos="+Request.Form["hScrollPos"]);
		}

		private bool IsXmlFile(string fileName)
		{
			string ext = GetExtention(fileName).ToLower();
			return ext == "xml" || ext == "config";
		}

		private string GetExtention(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf(".") + 1);
		}

		public string FileManagerPage
		{
			get { return ResolveUrl(Config.Main.FileManagerPage); }
		}

	}
}