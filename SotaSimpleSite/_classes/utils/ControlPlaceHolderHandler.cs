using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Sota.Web.SimpleSite.Utils
{
	/// <summary>
	/// Показывает картинку с именем контрола.
	/// </summary>
	public class ControlPlaceHolderHandler: System.Web.IHttpHandler
	{
		public bool IsReusable
		{
			get { return false; }
		}


		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Response.ContentType = "image/png";
			string name = context.Request.QueryString["c"];
			try
			{
				DataRow[] r = Config.GetConfigTable("control.config","control").Select("file='"+name+"'");
				if(r.Length>0)
				{
					name = "{"+r[0]["name"]+"}";
				}
			}
			catch{}

			name = "БЛОК: "+name;
			Font font = new Font("Verdana", 10, FontStyle.Bold);
			Bitmap bmp = new Bitmap(1,1);
			Graphics gr = Graphics.FromImage(bmp);
			Size size = gr.MeasureString(name, font).ToSize();
			size.Width+=10;
			size.Height+=10;
			gr.Dispose();
			bmp.Dispose();
			bmp = new Bitmap(size.Width, size.Height);
			gr  = Graphics.FromImage(bmp);
			gr.FillRectangle(Brushes.White,0,0,size.Width,size.Height);
			gr.DrawRectangle(Pens.Blue,0,0,size.Width-1,size.Height-1);
			gr.DrawString(name,font,Brushes.Blue,5,5);
			gr.Dispose();

			MemoryStream s = new MemoryStream();
			bmp.Save(s, ImageFormat.Png);
			context.Response.BinaryWrite(s.ToArray());
			s.Close();
			bmp.Dispose();
		}
	}
}
