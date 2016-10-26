using System;
using System.Collections;
using System.IO;
using System.Text;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Базовая страница, для админовских страниц.
	/// </summary>
	public class AdminBasePage : BasePage
	{
		public override string LoginPage
		{
			get { return ResolveUrl(Config.Main.AdminLoginPage); }
		}

		protected override void OnLoad(EventArgs e)
		{
			ComputeFilesHash();
			UserInfo ui = UserInfo.Current;
			if (ui.IsAuthorized)
			{
				//читаем базу при необходимости
				if (ui.Fields[Keys.UserIsRestricted] == null)
				{
					ui.GetAllFields();
					ui.Fields[Keys.UserIsRestricted] = ui[Keys.UserIsRestricted] == null ? "no" : ui[Keys.UserIsRestricted];
				}
				
				//если это ограниченная учетная запись
				if ((string)ui.Fields[Keys.UserIsRestricted] == "yes")
				{
					string[] allowedPaths = { 
											"admin/filemanager_d" 
											, "admin/htmleditor" 
											, "admin/lists"
											, "admin/list"
											, "admin/listxml"
											, "admin/listfilter"
											, "admin/listsort"
											, "admin/listimport"
											, "admin/datepicker"
											, "admin/timepicker"
											};
					PageInfo pi = PageInfo.Current;
					if (Array.IndexOf(allowedPaths, pi.FileName) == -1)
					{
						Response.Redirect("~/admin/lists.aspx");
					}
				}

			}
			base.OnLoad(e);
		}

		static bool hashneedtocheck = true;
		private void ComputeFilesHash()
		{
			if(hashneedtocheck)
			{
				hashneedtocheck = false;
				Hashtable ht = new Hashtable();
				ht["bottom.ascx"]	= "mKSAS9Fg2oDeAknajDzTmLlnAoM=";
				ht["default.aspx"]	= "WaIM4Vin32ZhHUvsFt2WtHJ3Aps=";
				ht["dialog.aspx"]	= "jRbDcltROOCmxXhMZ3MTuhZ3Kz0=";
				ht["top.ascx"]		= "ddGTzurobLmjBOe9VIEZ1e0ebzc=";
				ht["webform.aspx"]	= "AURrg1toanXdg0pAbCm/gviA4I4=";
			
				string[] files  = Directory.GetFiles(Request.MapPath("~/admin/templates"));
				foreach(string f in files)
				{
					string hash = "";
					string name = System.IO.Path.GetFileName(f);
					using(StreamReader r = new StreamReader(f, Encoding.Default))
					{
						hash = Security.SecurityManager.EncryptPassword(r.ReadToEnd());
						r.Close();
					}
					if(ht.ContainsKey(name) && hash != ht[name].ToString())
					{
						//Response.Clear();
						//Response.End();
					}
				}
			}
		}
	}
}