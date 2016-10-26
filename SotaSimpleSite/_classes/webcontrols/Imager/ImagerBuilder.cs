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
    public sealed class ImagerBuilder
    {

        public static byte[] GenerateImage(
            System.Web.HttpContext context,
            string profile,
            string file)
        {
            return GenerateImage(context, profile, file, ImagerConfig.Empty(), null);
        }

        public static byte[] GenerateImage(
            System.Web.HttpContext context, 
            string profile, 
            string file,
            ImagerConfig config,
            ImageFormat imageFormat)
        {
            if (config.IsEmpty)
            {
                config = GetImagerConfig(context, profile);
                if (config.IsEmpty)
                {
                    return null;
                }
            }

            //существует ли файл
            bool fileExists = false;
            Bitmap fromUrl = null;
            if (file.StartsWith("http://"))
            {
                try
                {
                    HttpWebRequest req = (HttpWebRequest)WebRequest.Create(file);
                    HttpWebResponse res = (HttpWebResponse)req.GetResponse();
                    Stream receiveStream = res.GetResponseStream();
                    fromUrl = new Bitmap(receiveStream);
                    receiveStream.Close();
                    res.Close();
                    fileExists = true;
                }
                catch
                { }
            }
            else if (File.Exists(file))
            {
                fileExists = true;
            }



            if (!fileExists)
            {
                if (!Util.IsBlank(config.NotFoundImage))
                {
                    file = context.Request.MapPath(config.NotFoundImage);
                }
                else
                {
                    return null;
                }
            }

            if (imageFormat == null)
            {
                imageFormat = GetImageFormat(file, config);
            }


            int w1 = -1;
            int h1 = -1;
            if (!Util.IsBlank(config.Width))
            {
                w1 = int.Parse(config.Width);
            }
            if (!Util.IsBlank(config.Height))
            {
                h1 = int.Parse(config.Height);
            }
            string fg = string.Empty;
            string bg = string.Empty;
            string y = string.Empty;
            string x = string.Empty;
            string fy = string.Empty;
            string fx = string.Empty;
            if (config.BackgroundImage != null)
            {
                bg = context.Request.MapPath(config.BackgroundImage);
                if (!Util.IsBlank(config.AlignX))
                {
                    x = config.AlignX;
                }
                if (!Util.IsBlank(config.AlignY))
                {
                    y = config.AlignY;
                }
            }
            if (!Util.IsBlank(config.FrontImage))
            {
                fg = context.Request.MapPath(config.FrontImage);
                if (!Util.IsBlank(config.FrontAlignX))
                {
                    fx = config.FrontAlignX;
                }
                if (!Util.IsBlank(config.FrontAlignY))
                {
                    fy = config.FrontAlignY;
                }
            }

            CompositingQuality cq = CompositingQuality.Default;
            if (!Util.IsBlank(config.CompositingQuality))
            {
                cq = (CompositingQuality)Enum.Parse(typeof(CompositingQuality), config.CompositingQuality, true);
            }
            SmoothingMode sm = SmoothingMode.Default;
            if (!Util.IsBlank(config.SmoothingMode))
            {
                sm = (SmoothingMode)Enum.Parse(typeof(SmoothingMode), config.SmoothingMode, true);
            }

            InterpolationMode im = InterpolationMode.Default;
            if (!Util.IsBlank(config.InterpolationMode))
            {
                im = (InterpolationMode)Enum.Parse(typeof(InterpolationMode), config.InterpolationMode, true);
            }


            Bitmap bitmap = null;
            Bitmap image = null;
            try
            {
                if (fromUrl != null)
                {
                    bitmap = fromUrl;
                }
                else if (File.Exists(file))
                {
                    bitmap = new Bitmap(file);
                }
                else
                {
                    bitmap = new Bitmap(1, 1);
                }
                //определяем необходимый размер

                if (w1 == -1)
                    w1 = bitmap.Width;
                if (h1 == -1)
                    h1 = bitmap.Height;

                SizeF maxSize = new Size(w1, h1);
                SizeF newSize = bitmap.Size;
                int cx = 0;
                int cy = 0;

                //определяем метод уменьшения
                if (Util.IsBlank(config.Crop))//уменьшение пропорциональное
                {
                    if (newSize.Height > maxSize.Height)
                    {
                        newSize.Width = newSize.Width * (maxSize.Height / newSize.Height);
                        newSize.Height = maxSize.Height;
                    }
                    if (newSize.Width > maxSize.Width)
                    {
                        newSize.Height = newSize.Height * (maxSize.Width / newSize.Width);
                        newSize.Width = maxSize.Width;
                    }
                    image = new Bitmap(newSize.ToSize().Width, newSize.ToSize().Height);
                }
                else//вырезаем картинку нужного размера
                {
                    if (maxSize.Width > newSize.Width)
                    {
                        maxSize.Width = newSize.Width;
                    }
                    if (maxSize.Height > newSize.Height)
                    {
                        maxSize.Height = newSize.Height;
                    }
                    string[] crop = config.Crop.Split(',');

                    image = new Bitmap(maxSize.ToSize().Width, maxSize.ToSize().Height);
                    if (crop[0] == "1")//не масштабируем оригинал
                    {
                        if (crop.Length > 1)
                        {
                            switch (crop[1])
                            {
                                case "l": //left
                                    break;
                                case "r": //right
                                    cx = bitmap.Width - image.Width;
                                    break;
                                case "c": //center
                                    cx = (bitmap.Width - image.Width) / 2;
                                    break;
                                default:
                                    cx = int.Parse(crop[1]);
                                    break;
                            }
                            if (crop.Length > 2)
                            {
                                switch (crop[2])
                                {
                                    case "t": //top
                                        break;
                                    case "b": //bottom
                                        cy = bitmap.Height - image.Height;
                                        break;
                                    case "m": //middle
                                        cy = (bitmap.Height - image.Height) / 2;
                                        break;
                                    default:
                                        cy = int.Parse(crop[2]);
                                        break;
                                }
                            }
                            else
                            {
                                cy = (bitmap.Height - image.Height) / 2;
                            }
                        }
                        else
                        {
                            cx = (bitmap.Width - image.Width) / 2;
                            cy = (bitmap.Height - image.Height) / 2;
                        }
                    }
                    else//уменьшаем насколько возможно и выбрасываем ненужную часть
                    {
                        bool isW = true;
                        double s1 = maxSize.Height / maxSize.Width;
                        double s2 = newSize.Height / newSize.Width;
                        if (s1 > s2)
                        {
                            isW = false;
                        }
                        if (isW)
                        {
                            newSize.Height = newSize.Height * (maxSize.Width / newSize.Width);
                            newSize.Width = maxSize.Width;
                        }
                        else
                        {
                            newSize.Width = newSize.Width * (maxSize.Height / newSize.Height);
                            newSize.Height = maxSize.Height;
                        }
                        if (newSize.Width > maxSize.Width)
                        {
                            if (crop.Length > 1)
                            {
                                switch (crop[1])
                                {
                                    case "l": //left
                                        break;
                                    case "r": //right
                                        cx = (int)Math.Round(newSize.Width - maxSize.Width);
                                        break;
                                    case "c": //center
                                        cx = (int)Math.Round((newSize.Width - maxSize.Width) / 2);
                                        break;
                                    default:
                                        cx = int.Parse(crop[1]);
                                        break;
                                }
                            }
                            else
                            {
                                cx = (int)Math.Round((newSize.Width - maxSize.Width) / 2);
                            }
                        }
                        else
                        {
                            if (crop.Length > 2)
                            {
                                switch (crop[2])
                                {
                                    case "t": //top
                                        break;
                                    case "b": //bottom
                                        cy = (int)Math.Round(newSize.Height - maxSize.Height);
                                        break;
                                    case "m": //middle
                                        cy = (int)Math.Round((newSize.Height - maxSize.Height) / 2);
                                        break;
                                    default:
                                        cy = int.Parse(crop[2]);
                                        break;
                                }
                            }
                            else
                            {
                                cy = (int)Math.Round((newSize.Height - maxSize.Height) / 2);
                            }
                        }
                    }
                }
                Graphics gr = Graphics.FromImage(image);
                gr.SmoothingMode = sm;
                gr.CompositingQuality = cq;
                gr.InterpolationMode = im;
                if (!Util.IsBlank(config.BackgroundColor))
                {
                    string rgb = config.BackgroundColor;
                    if (rgb.Length == 6)
                    {
                        System.Globalization.NumberStyles nstyle = System.Globalization.NumberStyles.AllowHexSpecifier;
                        Color c = Color.FromArgb(int.Parse(rgb.Substring(0, 2), nstyle), int.Parse(rgb.Substring(2, 2), nstyle), int.Parse(rgb.Substring(4, 2), nstyle));
                        gr.Clear(c);
                    }
                }
                gr.DrawImage(bitmap, -1 * cx, -1 * cy, newSize.ToSize().Width, newSize.ToSize().Height);
                gr.Dispose();

                //Фоновое изображение
                if (File.Exists(bg))
                {
                    Bitmap bg1 = new Bitmap(bg);
                    Bitmap bgB = new Bitmap(bg1.Width, bg1.Height);
                    Graphics g = Graphics.FromImage(bgB);
                    g.SmoothingMode = sm;
                    g.CompositingQuality = cq;
                    g.InterpolationMode = im;
                    g.DrawImage(bg1, 0, 0, bg1.Width, bg1.Height);
                    bg1.Dispose();
                    float y1 = 0;
                    float x1 = 0;
                    if (x != string.Empty)
                        switch (x)
                        {
                            case "l": //left
                                break;
                            case "r": //right
                                x1 = bgB.Width - image.Width;
                                break;
                            case "c": //center
                                x1 = (bgB.Width - image.Width) / 2;
                                break;
                            default:
                                try
                                {
                                    x1 = int.Parse(x);
                                }
                                catch { }
                                break;
                        }
                    if (y != string.Empty)
                        switch (y)
                        {
                            case "t": //top
                                break;
                            case "b": //bottom
                                y1 = bgB.Height - image.Height;
                                break;
                            case "m": //middle
                                y1 = (bgB.Height - image.Height) / 2;
                                break;
                            default:
                                try
                                {
                                    y1 = int.Parse(y);
                                }
                                catch { }
                                break;
                        }
                    g.DrawImage(image, x1, y1, image.Width, image.Height);
                    g.Dispose();
                    image.Dispose();
                    image = bgB;
                }
                //Рамка
                if (File.Exists(fg))
                {
                    Bitmap fg1 = new Bitmap(fg);
                    float y1 = 0;
                    float x1 = 0;
                    if (fx != string.Empty)
                        switch (fx)
                        {
                            case "l": //left
                                break;
                            case "r": //right
                                x1 = image.Width - fg1.Width;
                                break;
                            case "c": //center
                                x1 = (image.Width - fg1.Width) / 2;
                                break;
                            default:
                                try
                                {
                                    x1 = int.Parse(fx);
                                }
                                catch { }
                                break;
                        }
                    if (fy != string.Empty)
                        switch (fy)
                        {
                            case "t": //top
                                break;
                            case "b": //bottom
                                y1 = image.Height - fg1.Height;
                                break;
                            case "m": //middle
                                y1 = (image.Height - fg1.Height) / 2;
                                break;
                            default:
                                try
                                {
                                    y1 = int.Parse(fy);
                                }
                                catch { }
                                break;
                        }

                    Graphics g = Graphics.FromImage(image);
                    g.SmoothingMode = sm;
                    g.CompositingQuality = cq;
                    g.InterpolationMode = im;
                    g.DrawImage(fg1, x1, y1, fg1.Width, fg1.Height);
                    g.Dispose();
                    fg1.Dispose();
                }

                //коррекция GIF////////////////
                if (imageFormat == ImageFormat.Gif)
                {
                    ImageManipulation.OctreeQuantizer quantizer = new ImageManipulation.OctreeQuantizer(255, 8);
                    image = quantizer.Quantize(image);
                }
                /////////////////////////////

                MemoryStream s = new MemoryStream();

                //коррекция JPG////////////////
                if (imageFormat == ImageFormat.Jpeg)
                {
                    int quality = 90;
                    if (!Util.IsBlank(config.JpegQuality))
                    {
                        quality = int.Parse(config.JpegQuality);
                    }
                    Sota.Web.SimpleSite.JpegQuality.Save(image, s, quality);
                }
                /////////////////////////////
                else
                {
                    image.Save(s, imageFormat);
                }
                byte[] imageBytes = s.ToArray();
                s.Close();

                //коррекция PNG////////////////
                if (imageFormat == ImageFormat.Png)
                {
                    imageBytes = Sota.Web.SimpleSite.PNGFix.RemoveImageGamma(imageBytes);
                }
                /////////////////////////////

                return imageBytes;

            }
            finally
            {
                if (bitmap != null)
                    bitmap.Dispose();
                if (image != null)
                    image.Dispose();
            }
        }


        public static ImagerConfig GetImagerConfig(System.Web.HttpContext context, string profile)
        {
            ImagerConfig config = ImagerConfig.Empty();

            try
            {
                System.Data.DataTable tbProfile = Sota.Web.SimpleSite.Config.GetConfigTable("imager.config", "profile");
                System.Data.DataRow rConfigMain = Sota.Web.SimpleSite.Config.GetConfigTable("imager.config", "imager").Rows[0];

                if (Util.IsBlank(profile))
                {
                    if (rConfigMain["allowqs"].ToString() == "0")
                    {
                        config = ImagerConfig.FromDataRow(tbProfile.Select("id='" + rConfigMain["default"] + "'")[0]);
                    }
                    else
                    {
                        config = ImagerConfig.FromQueryString(context.Request.QueryString);
                    }
                }
                else
                {
                    DataRow[] rows = tbProfile.Select("id='" + profile + "'");
                    if (rows.Length > 0)
                    {
                        config = ImagerConfig.FromDataRow(rows[0]);
                    }
                }
            }
            catch (FileNotFoundException ex)//на случай если файл конфигурации не существует
            {
                Config.ReportError(ex);

                System.Data.DataTable tbMain = new System.Data.DataTable("imager");
                tbMain.Columns.Add("allowqs");
                tbMain.Columns.Add("default");
                tbMain.Rows.Add(new object[] { "1", "1" });
                Sota.Web.SimpleSite.Config.WriteConfigTable("imager.config", tbMain, "root");

                System.Data.DataTable tbProfile = new System.Data.DataTable("profile");
                tbProfile.Columns.Add("id");
                tbProfile.Rows.Add(new object[] { "1" });
                Sota.Web.SimpleSite.Config.WriteConfigTable("imager.config", tbProfile, "root");
            }

            return config;
        }
        public static ImageFormat GetImageFormat(string file, ImagerConfig config)
        {
            //определяем формат
            string ext = string.Empty;
            if (!Util.IsBlank(config.Format))
            {
                ext = "." + config.Format.Trim().ToLower();
            }
            ImageFormat imageFormat = ImageFormat.Png;
            if (ext == string.Empty)
            {
                ext = Path.GetExtension(file).ToLower();
            }
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

            return imageFormat;
        }

        /// <summary>
        /// Gets the size of the image from file
        /// </summary>
        /// <param name="file">The path of the file containing image</param>
        /// <returns></returns>
        public static Size GetImageSize(string file)
        {
            if (!Path.IsPathRooted(file))
            {
                file = System.Web.HttpContext.Current.Request.MapPath(file);

            }
            if (File.Exists(file))
            {
                Bitmap b = null;
                try
                {
                    b = new Bitmap(file);
                    return b.Size;
                }
                finally
                {
                    if (b != null)
                    {
                        b.Dispose();
                    }
                }
            }
            return Size.Empty;
        }

    }
}
