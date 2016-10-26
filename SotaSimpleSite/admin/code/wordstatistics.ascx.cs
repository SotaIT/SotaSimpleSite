namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.Text.RegularExpressions;
	using System.Data;

	/// <summary>
	///		Summary description for wordstatistics.
	/// </summary>
	public class WordStatistics : System.Web.UI.UserControl
	{
		protected System.Web.UI.WebControls.Repeater rptWords;
		private DataTable tbWords = new DataTable();

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Request.QueryString["path"] != null)
			{
				string path = Request.QueryString["path"];
				tbWords.Columns.Add("word", typeof (string));
				tbWords.Columns.Add("count", typeof (int)).DefaultValue = 0;
				Regex rex = new Regex("<[^>]*>");

				PageInfo pi = new PageInfo(path);
				AddContent(rex, pi.Body);
				foreach (string key in pi.Fields.Keys)
				{
					AddContent(rex, pi.Fields[key].ToString());
				}

				this.tbWords.DefaultView.Sort = "count DESC";
				this.rptWords.DataSource = this.tbWords;
				this.rptWords.DataBind();
			}
		}

		private void AddContent(Regex rex, string s)
		{
			if (s == null || s == string.Empty)
				return;
			string[] arWords = rex.Replace(BasePage.ClearMetaLanguage(s), " ").Split(Keys.ContentSeparators.ToCharArray());
			int n = arWords.Length;
			for (int i = 0; i < n; i++)
			{
				string w = arWords[i].Trim();
				if (w.Length > 2)
					AddWord(w);
			}
		}

		public int iRowNumber = 0;

		private void AddWord(string word)
		{
			if (word == null || word == string.Empty)
				return;
			try
			{
				DataRow[] rows = this.tbWords.Select("word='" + word + "'");
				if (rows.Length > 0)
				{
					rows[0]["count"] = Convert.ToInt32(rows[0]["count"]) + 1;
				}
				else
				{
					this.tbWords.Rows.Add(new object[] {word, 1});
				}
			}
			catch
			{
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
	}
}