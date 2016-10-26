namespace Sota.Web.SimpleSite.Code.Admin
{
	using System;
	using System.Data;
	using System.IO;
	using System.Text;
	using Sota.Web.SimpleSite.WebControls;
	using Sota.Web.SimpleSite.Security;

	/// <summary>
	///	Корзина страниц.
	/// </summary>
	public class PageTrash : System.Web.UI.UserControl
	{
		protected System.Web.UI.HtmlControls.HtmlSelect lstPages;
		protected System.Web.UI.HtmlControls.HtmlInputHidden hAction;
		protected Sota.Web.SimpleSite.WebControls.XmlForm xfPages;

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

		private void Page_Load(object sender, System.EventArgs e)
		{
			if (Request.RequestType.ToLower() == "get")
			{
				this.xfPages.Action = Sota.Web.SimpleSite.Path.Full;
			}
			else
			{
				Response.Clear();
				if (Request.ContentType == "text/xml")
				{
					Response.ContentEncoding = Encoding.UTF8;
					DataSet ds = new DataSet();
					ds.ReadXml(Request.InputStream);
					if (ds.Tables.Count > 0)
					{
						DataTable tbRequest = ds.Tables[0];
						if (tbRequest.Columns.Contains(XmlForm.ElementForm)
							&& tbRequest.Columns.Contains(XmlForm.ElementAction))
						{
							if (tbRequest.Rows[0][XmlForm.ElementAction].ToString() == XmlForm.ActionList)
							{
								Response.ContentType = "text/xml";
								Response.Write("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
								Response.Write("<response>");
								DirectoryInfo di = new DirectoryInfo(Request.MapPath(Config.Main.Data + "/trash"));
								FileInfo[] arFi = di.GetFiles();
								int n = arFi.Length;
								for (int i = 0; i < n; i++)
								{
									string s = arFi[i].Name.Substring(0, arFi[i].Name.Length - arFi[i].Extension.Length);
									PageInfo.DeletedFile df = PageInfo.GetFileNameFromTrash(s);
									Response.Write("<option ");
									Response.Write("value=\"");
									Response.Write(s);
									Response.Write("\" text=\"");
									Response.Write("[" + df.Deleted.ToString() + "] " + df.Name);
									Response.Write("\"/>");

								}
								Response.Write("</response>");
							}
							else
							{
								string s = tbRequest.Rows[0][this.lstPages.ClientID].ToString();
								if (tbRequest.Rows[0][this.hAction.ClientID].ToString() == "0")
								{
									PageInfo.Restore(s);
								}
								else
								{
									if (UserInfo.Current.IsInGroup(-3))
										PageInfo.FullDelete(s);
								}
							}
						}
					}
				}
				Response.End();
			}

		}

	}
}