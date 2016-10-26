using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Web;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite.Utils
{
	/// <summary>
	/// Обеспечивает скачивание файла.
	/// </summary>
	public class FileDownloadPage : System.Web.UI.Page
	{
		protected override void OnLoad(EventArgs e)
		{
			Response.Clear();
			Response.ContentType = "application/octet-stream";
			string fileName = "notfound.txt";
			if (Request.QueryString.Count > 0)
			{
				string file = Request.QueryString[0].Trim();
				if (file.Length > 0)
				{
					if (Request.QueryString["enc"] != null)
					{
						file = Uri.UnescapeDataString(file);
					}
					if (!file.StartsWith("\\\\") && file.IndexOf(System.IO.Path.VolumeSeparatorChar) == -1)
					{
						file = Request.MapPath(file);
					}
					if (File.Exists(file))
					{
						file = file.ToLower();
						bool allowed = false;
						if (UserInfo.Current != null && UserInfo.Current.IsAdmin)
						{
							allowed = true;
						}
						else
						{
							DataTable tb = Config.GetConfigTable("download.config", "folder");
							for (int i = 0; i < tb.Rows.Count; i++)
							{
								string folder = Request.MapPath(tb.Rows[i]["name"].ToString()).ToLower();
								if (file.StartsWith(folder))
								{
									allowed = tb.Rows[i]["allow"].ToString() == "1";
									if (!allowed)
									{
										break;
									}
								}
							}
						}
						if (allowed)
						{

							fileName = HttpUtility.UrlEncode(System.IO.Path.GetFileName(file));
							Response.WriteFile(file);
						}
					}
				}
			}
			Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
			if (fileName == "notfound.txt")
			{
				Response.Write("!!!The file is not found!!!");
			}
			if (Config.Main.LogDownload)
			{
				Hashtable h = Util.GetClientInfo(Request);
				h["file"] = fileName;
				Log.Write("Download", h);
			}
			Response.End();
		}
	}
}