using System.Web;

namespace Sota.Web.SimpleSite.Utils
{

	public class RobotsTxtHandler : IHttpHandler
	{

		public void ProcessRequest(HttpContext context)
		{
			context.Response.ContentType = "text/plain";

			if (context.Request.FilePath
				.Substring(context.Request.ApplicationPath.Length)
				.TrimStart('/')
				.ToLower()
				== "robots.txt")
			{

				string domain = context.Request.Url.Host;
				if (domain.StartsWith("www."))
				{
					domain = domain.Substring("www.".Length);
				}
				string file = context.Request.MapPath("~/" + domain + "_robots.txt");
				if (System.IO.File.Exists(file))
				{
					context.Response.WriteFile(file);
				}
				else
				{
					context.Response.WriteFile(context.Request.MapPath("~/robots.txt"));
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
