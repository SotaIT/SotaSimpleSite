using System.Data;
using System.IO;

namespace Sota.Web.SimpleSite.Code.Admin.list
{
	using System;

	/// <summary>
	///		Summary description for create_list.
	/// </summary>
	public class create_list : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlInputText txtName;
		protected System.Web.UI.HtmlControls.HtmlInputText txtFile;
		protected System.Web.UI.WebControls.Button cmdNext;
		protected System.Web.UI.HtmlControls.HtmlInputText txtCaption;
		protected string sError = string.Empty;
		protected DataTable tbProvider = null;
		private void Page_Load(object sender, System.EventArgs e)
		{
			tbProvider = Config.GetConfigTable("lists.config", "provider").Copy();
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
			this.cmdNext.Click += new System.EventHandler(this.cmdNext_Click);
			this.Load += new System.EventHandler(this.Page_Load);

		}

		#endregion

		private void cmdNext_Click(object sender, System.EventArgs e)
		{
			DataTable tb = Config.GetConfigTable("lists.config", "list").Copy();
			if (tb.Rows.Count > 0)
				if (tb.Select("name='" + this.txtName.Value + "'").Length > 0)
				{
					sError = "Идентификатор должен быть уникальным!\\n Список с таким идентификатором уже создан!";
					return;
				}
			string file = Request.MapPath("~/" + txtFile.Value.Trim());
			if (!File.Exists(file))
			{
				sError = "Указанный файл не существует!";
				return;
			}
			string provider = List.LIST_TYPE;
			if(!Util.IsBlank(Request.Form["txtProvider"]))
			{
				provider = Request.Form["txtProvider"].Trim();
			}
			else if(!Util.IsBlank(Request.Form["cmbProvider"]) && Request.Form["cmbProvider"]!="0")
			{
				provider = Request.Form["cmbProvider"].Trim();
			}

			if (!tb.Columns.Contains("name"))
				tb.Columns.Add("name");
			if (!tb.Columns.Contains("file"))
				tb.Columns.Add("file");
			if (!tb.Columns.Contains("caption"))
				tb.Columns.Add("caption");
			if (!tb.Columns.Contains("provider"))
				tb.Columns.Add("provider");
			DataRow r = tb.NewRow();
			r["name"] = txtName.Value.Trim();
			r["caption"] = txtCaption.Value.Trim();
			r["file"] = "~/" + txtFile.Value.Trim();
			r["provider"] = provider;
			tb.Rows.Add(r);
			Config.WriteConfigTable("lists.config", tb, "root");
			Response.Redirect("~/admin/config/list/" + txtName.Value.Trim() + Config.Main.Extension);
		}
	}
}