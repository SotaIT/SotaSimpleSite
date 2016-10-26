using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Text;
using System.Drawing.Imaging;
using System.IO;

namespace Sota.Web.UI.WebControls
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ImageText runat=server></{0}:ImageText>")]
    public class ImageText : System.Web.UI.WebControls.Image
    {
        Font textFont = new Font("Arial", 10, FontStyle.Regular);
        Color textColor = Color.Black;
        Color textBackColor = Color.White;
        string text = string.Empty;
        TextRenderingHint textRenderingHint = TextRenderingHint.ClearTypeGridFit;
        int maxWidth = 0;
        int imageCache = 60;

        public Font TextFont
        {
            get
            {
                return textFont;
            }
            set
            {
                textFont = value;
            }
        }
        public string Text
        {
            get
            {
                return text;
            }
            set
            {
                text = value;
            }
        }
        public Color TextColor
        {
            get
            {
                return textColor;
            }
            set
            {
                textColor = value;
            }
        }
        public Color TextBackColor
        {
            get
            {
                return textBackColor;
            }
            set
            {
                textBackColor = value;
            }
        }
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return textRenderingHint;
            }
            set
            {
                textRenderingHint = value;
            }
        }
        public int MaxWidth
        {
            get
            {
                return maxWidth;
            }
            set
            {
                maxWidth = value;
            }
        }
        public int ImageCache
        {
            get
            {
                return imageCache;
            }
            set
            {
                imageCache = value;
            }
        }

        public void GenerateImage()
        {
            GenerateImage(ImageUrl, Text);
        }

        public void GenerateImage(string imageUrl, string imageText)
        {
            if (imageUrl.Length == 0 || imageText.Length == 0)
            {
                return;
            }
            string file = Context.Request.MapPath(imageUrl);

            if (!File.Exists(file) || ((DateTime.UtcNow - File.GetLastWriteTimeUtc(file)).TotalMinutes > imageCache))
            {
                ImageFormat imageFormat = ImageFormat.Png;
                string ext = Path.GetExtension(file).ToLower();
                switch (ext)
                {
                    case ".gif":
                        imageFormat = ImageFormat.Gif;
                        break;
                    case ".jpg":
                    case ".jpeg":
                        imageFormat = ImageFormat.Jpeg;
                        break;
                    case ".bmp":
                        imageFormat = ImageFormat.Bmp;
                        break;
                }

                Size size = new Size(100, 100);

                Bitmap bmp = null;
                Graphics gr1 = null;
                try
                {
                    bmp = new Bitmap(size.Width, size.Height);
                    gr1 = Graphics.FromImage(bmp);
                    size = (maxWidth == 0 ? gr1.MeasureString(imageText, textFont) : gr1.MeasureString(imageText, textFont, maxWidth)).ToSize();
                    gr1.Dispose();
                }
                finally
                {
                    bmp.Dispose();
                }

                Bitmap img = null;
                Graphics gr = null;
                try
                {
                    img = new Bitmap(size.Width, size.Height);
                    gr = Graphics.FromImage(img);
                    gr.TextRenderingHint = textRenderingHint;
                    gr.Clear(textBackColor);
                    Brush brush = new SolidBrush(textColor);
                    gr.DrawString(imageText, textFont, brush, 0, 0);
                    gr.Dispose();
                    img.Save(file, imageFormat);
                }
                finally
                {
                    img.Dispose();
                }

            }

        }

        protected override void RenderContents(HtmlTextWriter writer)
        {
            GenerateImage();
            base.RenderContents(writer);
        }
    }
}
