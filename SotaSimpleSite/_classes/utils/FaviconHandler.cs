using System.Web;

namespace Sota.Web.SimpleSite.Utils
{

	public class FaviconHandler : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "image/x-icon";

			if (context.Request.FilePath
				.Substring(context.Request.ApplicationPath.Length)
				.TrimStart('/')
				.ToLower()
				== "favicon.ico")
			{

				string domain = context.Request.Url.Host;
				if (domain.StartsWith("www."))
				{
					domain = domain.Substring("www.".Length);
				}
				string file = context.Request.MapPath("~/" + domain + ".ico");
				if (System.IO.File.Exists(file))
				{
					context.Response.WriteFile(file);
				}
				else
				{
					context.Response.WriteFile(context.Request.MapPath("~/favicon.ico"));
				}
			}
			else
			{
				context.Response.WriteFile(context.Request.PhysicalPath);

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
