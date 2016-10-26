using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Summary description for JpegQuality.
	/// </summary>
	public class JpegQuality
	{
		public JpegQuality()
		{
		}
		
		public static void Save(Image image, Stream stream, int quality)
		{
			Save(image, null, stream, quality);
		}

		public static void Save(Image image, string filename, int quality)
		{
			Save(image, filename, null, quality);
		}

		private static void Save(Image image, string filename, Stream stream, int quality)
		{
			ImageCodecInfo ici = GetEncoderInfo("image/jpeg");
			Encoder enc = Encoder.Quality;
			EncoderParameter par = new EncoderParameter(enc, quality);
			EncoderParameters pars = new EncoderParameters(1);
			pars.Param[0] = par;
			if(filename != null)
			{
				image.Save(filename, ici, pars);
			}
			if(stream != null)
			{
				image.Save(stream, ici, pars);
			}
		}

		public static ImageCodecInfo GetEncoderInfo(String mimeType)
		{
			int j;
			ImageCodecInfo[] encoders;
			encoders = ImageCodecInfo.GetImageEncoders();
			for(j = 0; j < encoders.Length; ++j)
			{
				if(encoders[j].MimeType == mimeType)
					return encoders[j];
			}
			return null;
		}

	}
}
