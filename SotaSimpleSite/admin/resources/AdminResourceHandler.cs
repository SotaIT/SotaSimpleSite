using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Resources;

namespace Sota.Web.SimpleSite.Resources
{
	/// <summary>
	/// ƒостает ресурсы из DLL.
	/// </summary>
	public class AdminResourceHandler : System.Web.IHttpHandler
	{
		public AdminResourceHandler()
		{
		}

		public bool IsReusable
		{
			get { return false; }
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.Cache.SetExpires(DateTime.MaxValue);
			context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
			if (context.Request.QueryString["img"] != null)
			{
				string img = context.Request.QueryString["img"].Trim();
				Bitmap b = null;
				MemoryStream s = null;
				try
				{
					ResourceManager rm = new ResourceManager("SotaSimpleSite.admin.resources.AdminResources", GetType().Assembly);
					b = (Bitmap) rm.GetObject(img, System.Globalization.CultureInfo.InvariantCulture);
					s = new MemoryStream();

					//определ€ем формат
					ImageFormat imageFormat = ImageFormat.Png;
					string conttype = "image/png";
					string ext = GetExtention(img).ToLower();
					switch (ext)
					{
						case "gif":
							imageFormat = ImageFormat.Gif;
							conttype = "image/gif";
							break;
						case "jpg":
						case "jpeg":
							imageFormat = ImageFormat.Jpeg;
							conttype = "image/jpeg";
							break;
						case "bmp":
							imageFormat = ImageFormat.Bmp;
							conttype = "image/bmp";
							break;
					}
					b.Save(s, imageFormat);
					context.Response.ContentType = conttype;
					context.Response.BinaryWrite(s.ToArray());
				}
				catch(Exception ex)
				{
					Config.ReportError(ex);
				}
				finally
				{
					if (b != null)
					{
						b.Dispose();
					}
					if (s != null)
					{
						s.Close();
					}
				}
			}
		}

		private static string GetExtention(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf(".") + 1);
		}


	}
}