using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using System.Web.SessionState;

namespace Sota.Web.UI.WebControls
{
    public class RandomCodeImageHandler : System.Web.IHttpHandler, IRequiresSessionState
    {
        public bool IsReusable
        {
            get { return true; }
        }

        public void ProcessRequest(System.Web.HttpContext context)
        {
            if (context.Request.QueryString["key"] == null
                || context.Request.QueryString["id"] == null 
                || context.Request.QueryString["page"] == null)
            {
                return;
            }
            CodeImageInfo codeImageInfo = (CodeImageInfo)context.Cache[RandomCodeImage.GetSessionKey(
				context.Request.QueryString["key"], 
				context.Request.QueryString["page"], 
				context.Request.QueryString["id"],
				context.Request.QueryString["g"])];
            if (codeImageInfo == null)
            {
                return;
            }
            Bitmap bg               = null;
            ImageFormat imageFormat = ImageFormat.Png;
            string conttype         = "image/png";
           
            string file = context.Request.MapPath(codeImageInfo.ImageUrl);
            if (File.Exists(file))
            {
               bg           = new Bitmap(file);
               string ext   = Path.GetExtension(file).ToLower();
               switch (ext)
               {
                   case ".gif":
                       imageFormat  = ImageFormat.Gif;
                       conttype     = "image/gif";
                       break;
                   case ".jpg":
                   case ".jpeg":
                       imageFormat  = ImageFormat.Jpeg;
                       conttype     = "image/jpeg";
                       break;
                   case ".bmp":
                       imageFormat  = ImageFormat.Bmp;
                       conttype     = "image/bmp";
                       break;
               }
            }
            else
            {
                bg = new Bitmap(codeImageInfo.Size.Width, codeImageInfo.Size.Height);
                Graphics gr1 = Graphics.FromImage(bg);
                gr1.Clear(Color.DarkGray);
                Pen pen = Pens.Gray;
                int h = bg.Height;
                int j = Math.Min(12, Math.Max(6, Convert.ToInt32(h / 10)));
                int count = Convert.ToInt32(bg.Width / j);
                for (int i = -2; i < count; i++)
                {
                    gr1.DrawLine(pen, new Point(i * j, 0), new Point(i * j + 2 * j, h));
                    gr1.DrawLine(pen, new Point(i * j + 2 * j, 0), new Point(i * j, h));
                }
                gr1.Dispose();
            }
            context.Response.ContentType    = conttype;
            Brush brush                     = new SolidBrush(codeImageInfo.ForeColor);
            Bitmap bitmap                   = new Bitmap(bg.Width, bg.Height);
            Graphics gr = Graphics.FromImage(bitmap);
            gr.DrawImage(bg, 0, 0);
            gr.TextRenderingHint = codeImageInfo.TextRenderingHint;
            float x = 2;
            Random rnd = new Random();
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < codeImageInfo.Code.Length; i++)
            {
                int angle = rnd.Next((-1) * codeImageInfo.Angle, codeImageInfo.Angle);
                if (codeImageInfo.Angle > 0)
                {
                    gr.RotateTransform(angle);
                }
                SizeF sizeT = gr.MeasureString(codeImageInfo.Code[i].ToString(), codeImageInfo.Font);
                gr.DrawString(codeImageInfo.Code[i].ToString(), codeImageInfo.Font, brush, new RectangleF(x, 0, sizeT.Width, bitmap.Height), format);
                x += sizeT.Width;
                if (codeImageInfo.Angle > 0)
                {
                    gr.RotateTransform(angle * (-1));
                }
            }
            gr.Dispose();
            bg.Dispose();
            MemoryStream s = new MemoryStream();
            bitmap.Save(s, imageFormat);
            context.Response.BinaryWrite(s.ToArray());
            s.Close();
            bitmap.Dispose();
            context.Response.End();

        }
    }
}
