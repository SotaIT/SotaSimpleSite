using System;
using System.Collections;
using System.Web;

namespace Sota.Web.SimpleSite.Utils
{
	/// <summary>
	/// Осуществляет редирект на указанный URL.
	/// </summary>
	public class RedirectPage : IHttpHandler
	{
		public void ProcessRequest(HttpContext context)
		{
			HttpRequest Request = context.Request;
			HttpResponse Response = context.Response;

			if (Request.QueryString.Count > 0)
			{
				if (Request.UrlReferrer == null || !string.Equals(Request.UrlReferrer.Host, Request.Url.Host, StringComparison.InvariantCultureIgnoreCase))
				{
					Util.NotFound();
				}
				else
				{

					if (Config.Main.LogRedirect && Request.QueryString["nolog"] == null)
					{
						Hashtable h = Util.GetClientInfo(Request);
						h.Remove("path");
						foreach (string key in Request.QueryString.AllKeys)
						{
							h.Add(key, key == "url" ? Util.CreateHtmlLink(Request.QueryString[key]) : Request.QueryString[key]);
						}
						Log.Write("Redirect", h);
					}


					Uri uri = new Uri("http://" + Request.Url.Host);
					try
					{
						string url = Request.QueryString[0];
						if (url.StartsWith("http://")
							|| url.StartsWith("https://")
							|| url.StartsWith("ftp://"))
						{
							uri = new Uri(url);
						}
						else if (url.StartsWith("//"))
						{
							uri = new Uri("http:" + url);
						}
						else
						{
							uri = new Uri("http://" + Request.Url.Host + "/" + Request.QueryString[0].TrimStart('/'));
						}

					}
					catch (Exception ex)
					{
						Config.ReportError(ex);
					}

					Response.Redirect(uri.ToString());
				}
			}

		}
		public bool IsReusable
		{
			get
			{
				return false;
			}
		}
	}

}