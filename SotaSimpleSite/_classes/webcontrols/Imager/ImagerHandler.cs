using System.Data;
using System.Drawing;
using System.IO;
using System;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Net;
using Sota.Web.SimpleSite;
using Path = System.IO.Path;

namespace Sota.Web.UI.WebControls
{
    public class ImagerHandler : System.Web.IHttpHandler
    {

        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            string profile = context.Request.QueryString["profile"];

            string file = "";
            if (context.Request.QueryString["url"] != null)
            {
                if (context.Request.QueryString["url"].StartsWith("http://"))
                {
                    file = context.Request.QueryString["url"];
                }
                else
                {
                    file = context.Request.MapPath(context.Request.QueryString["url"]);
                }
            }
            else
            {
                string[] parts = context.Request.Url.ToString()
                    .Substring(Sota.Web.SimpleSite.Path.ARoot.Length)
                    .Split(Sota.Web.SimpleSite.Keys.UrlParamPageDelimiter[0])[0]
                    .Split(Sota.Web.SimpleSite.Keys.UrlPathDelimiter[0]);
                if (parts.Length == 5)
                {
                    profile = parts[3];
                    parts = new string[] { parts[0], parts[1], parts[2], parts[4] };
                }
                if (parts.Length == 4)
                {
                    Sota.Web.SimpleSite.List l = Sota.Web.SimpleSite.List.Create(parts[1]);
                    l.ReadItem(int.Parse(parts[3].Substring(0, parts[3].IndexOf("."))));
                    if (l.Data.Count > 0)
                    {
						string fileName = l.Data.FirstRow[parts[2]].ToString();
						if (fileName.StartsWith("http://"))
						{
							file = fileName;
						}
						else
						{
							file = context.Request.MapPath(l.Data.Uploads(parts[2])
								.TrimEnd(Sota.Web.SimpleSite.Keys.UrlPathDelimiter[0])
								+ Sota.Web.SimpleSite.Keys.UrlPathDelimiter
								+ fileName);
						}
                    }
                }
            }

 
            ImagerConfig config = ImagerBuilder.GetImagerConfig(context, profile);

            ImageFormat imageFormat = ImagerBuilder.GetImageFormat(file, config);
            string conttype = "image/png";
            if (imageFormat != ImageFormat.Png)
            {
                if (imageFormat == ImageFormat.Jpeg)
                {
                    conttype = "image/jpeg";
                }
                else if (imageFormat == ImageFormat.Gif)
                {
                    conttype = "image/gif";
                }
                else if (imageFormat == ImageFormat.Bmp)
                {
                    conttype = "image/bmp";
                }
            }


            byte[] imageBytes = ImagerBuilder.GenerateImage(
                    context
                    , profile
                    , file
                    , config
                    , imageFormat
                );


            if (imageBytes == null)
            {
                context.Response.StatusCode = 404;
            }
            else
            {
                //кэширование
                context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
                if (!Util.IsBlank(config.Cache))
                {
                    context.Response.Cache.SetExpires(DateTime.Now.AddSeconds(int.Parse(config.Cache)));
                }
                else if (config.NoCache != null)
                {
                    context.Response.Cache.SetCacheability(System.Web.HttpCacheability.NoCache);
                    context.Response.Cache.SetExpires(DateTime.MinValue);
                }


                context.Response.ContentType = conttype;


                //запись файла в поток
                context.Response.BinaryWrite(imageBytes);
            }
        }

    }
}
