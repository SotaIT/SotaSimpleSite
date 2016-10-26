using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Web.Caching;

namespace Sota.Web.SimpleSite.Utils
{
	/// <summary>
	/// Рисует картинку с указанием количества посещений.
	/// </summary>
	public sealed class CounterDisplayHandler : System.Web.IHttpHandler, System.Web.SessionState.IRequiresSessionState
	{

		public bool IsReusable
		{
			get { return false; }
		}


		public void ProcessRequest(System.Web.HttpContext context)
		{
			DataRow row = Config.GetConfigTable("counter.config", "counter").Rows[0];
			if(context.Request.QueryString["id"]!=null)
			{
				row = Config.GetConfigTable("counter.config", "counter").Select("id=" + context.Request.QueryString["id"])[0];
			}

			if(context.Request.QueryString["nocount"]==null)
			{
				#region подсчитать
				Hashtable h		= Util.GetClientInfo(context.Request);
				if(context.Request.QueryString["path"]!=null)
				{
					h["path"]	= Util.CreateHtmlLink(context.Request.QueryString["path"]);
				}
				else
				{
					h["path"]	= h["referer"];
				}
				if(context.Request.QueryString["ref"]!=null)
				{
					h["referer"]	= Util.CreateHtmlLink(context.Request.QueryString["ref"]);
				}
				else
				{
					h["referer"]	= null;
				}
				Log.Request(h);
				#endregion
			}
			string cacheKey		= "CounterImageBytes";
			byte[] imageBytes = null;
			if (context.Cache[cacheKey] == null)
			{
				string file = context.Request.MapPath(row["img"].ToString());

				//существует ли файл
				if (!File.Exists(file))
					return;
				Font font = new Font(row["font"].ToString(), Convert.ToInt32(row["size"]));
				Brush brush = Brushes.Black;
				if (row.Table.Columns.Contains("rgb"))
				{
					string[] rgb = row["rgb"].ToString().Split(',');
					System.Globalization.NumberStyles nstyle = System.Globalization.NumberStyles.AllowHexSpecifier;
					brush = new SolidBrush(Color.FromArgb(int.Parse(rgb[0].Trim(), nstyle), int.Parse(rgb[1].Trim(), nstyle), int.Parse(rgb[2].Trim(), nstyle)));
				}
				int padding = Convert.ToInt32(row["padding"]);

				int all		= Log.GetCount("Unique") + Convert.ToInt32(row["start"]);
				int today	= Log.GetCountToday("Unique");

				//определяем формат			
				ImageFormat imageFormat = ImageFormat.Png;
				string conttype = "image/png";
				string ext = GetExtention(file).ToLower();
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
				context.Response.ContentType = conttype;

				Bitmap bitmap = null;
				Bitmap image = null;
				MemoryStream s = null;
				try
				{
					bitmap = new Bitmap(file);
					image = (Bitmap)bitmap.GetThumbnailImage(bitmap.Size.Width, bitmap.Size.Height, null, IntPtr.Zero);
					Graphics g = Graphics.FromImage(image);					

					float x1 = padding;
					float y1 = image.Size.Height - padding - g.MeasureString(all.ToString(), font).Height;
					g.DrawString(all.ToString(), font, brush, x1, y1);

					float x2 = image.Size.Width - padding - g.MeasureString("+" + today.ToString(), font).Width;
					float y2 = y1;
					g.DrawString("+" + today.ToString(), font, brush, x2, y2);
					g.Dispose();
					
					//коррекция GIF////////////////
					if(imageFormat==ImageFormat.Gif)
					{
						ImageManipulation.OctreeQuantizer	quantizer = new ImageManipulation.OctreeQuantizer ( 255 , 8 ) ;
						image = quantizer.Quantize ( image );
					}
					/////////////////////////////
					

					s = new MemoryStream();
					image.Save(s, imageFormat);
					

					imageBytes = s.ToArray();

					//коррекция PNG////////////////
					if(imageFormat==ImageFormat.Png)
					{
						imageBytes = PNGFix.RemoveImageGamma(imageBytes);
					}
					/////////////////////////////

				}
				finally
				{
					if (bitmap != null)
					{
						bitmap.Dispose();
					}
					if (image != null)
					{
						image.Dispose();
					}
					if (s != null)
					{
						s.Close();
					}
				}
				context.Cache.Insert(cacheKey, imageBytes, new CacheDependency(file), DateTime.Now.AddSeconds(30), TimeSpan.Zero);
			}
			else
			{
				imageBytes		= (byte[]) context.Cache[cacheKey];
			}
			//кэширование
			context.Response.Cache.SetExpires(DateTime.Now);
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
			context.Response.BinaryWrite(imageBytes);
		}

		private static string GetExtention(string fileName)
		{
			return fileName.Substring(fileName.LastIndexOf(".") + 1);
		}
	}
}