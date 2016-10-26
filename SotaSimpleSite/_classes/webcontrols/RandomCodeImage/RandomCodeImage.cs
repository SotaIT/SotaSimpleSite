using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;


namespace Sota.Web.UI.WebControls
{
    [DefaultProperty("BackgroundImageUrl")]
    [ToolboxData("<{0}:RandomCodeImage runat=server></{0}:RandomCodeImage>")]
    public class RandomCodeImage : System.Web.UI.WebControls.Image
    {
        CodeImageInfo _codeImageInfo = new CodeImageInfo();
        
        [Category("Appearance")]
        [Editor("System.Web.UI.Design.UrlEditor, System.Design", typeof(UITypeEditor))]
        [Description("The path to the HttpHandler which generates the image")]
		public string HandlerUrl
        {
            get
            {
                
                string val = (string)this.ViewState["HandlerUrl"];
                if (val != null)
                {
                    return val;
                }
                return "~/rci.ashx";
            }
            set
            {
                this.ViewState["HandlerUrl"] = value;
            }
        }
        
        [Category("Appearance")]
        [Editor("System.Web.UI.Design.ImageUrlEditor, System.Design, Version=2.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a", typeof(UITypeEditor))]
        [DefaultValue("")]
        [Description("The url of the image used as a background")]
        public string BackgroundImageUrl
        {
            get
            {
                return _codeImageInfo.ImageUrl;
            }
            set
            {
                _codeImageInfo.ImageUrl = value;
            }
        }

        [Category("Behavior")]
        [Description("The name of the string key used to store the random string in the Cache object")]
        public string SessionKey
        {
            get
            {
                string val = (string)this.ViewState["SessionKey"];
                if (val != null)
                {
                    return val;
                }
                return "RandomCodeImage";
            }
            set
            {
                this.ViewState["SessionKey"] = value;
            }
        }
       
        [Category("Behavior")]
        [DefaultValue(6)]
        [Description("The length of the random string")]
        public int Digits
        {
            get
            {
                object val = this.ViewState["Digits"];
                if (val != null)
                {
                    return Convert.ToInt32(val);
                }
                return 6;
            }
            set
            {
                if (value > 0 && value < 33)
                {
                    this.ViewState["Digits"] = value;
                }
            }
        }
        
        [Category("Appearance")]
        [DefaultValue(typeof(Font), "Arial, 12pt, style=Bold")]
        [Description("The font used to render the random string")]
        public Font DrawFont
        {
            get
            {
                return _codeImageInfo.Font;
            }
            set
            { 
                _codeImageInfo.Font = value; 
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Size), "100,40")]
        [Description("The size of the image generated as a background if the BackgroundImageUrl is not set")]
        public Size DrawSize
        {
            get
            {
                return _codeImageInfo.Size;
            }
            set
            {
                _codeImageInfo.Size = value;
            }
        }
        
        [Category("Appearance")]
        [DefaultValue(15)]
        [Description("The angle of rotation of the symbols in the random string")]
        public int RotateAngle
        {
            get
            {
                return _codeImageInfo.Angle;
            }
            set
            {
                _codeImageInfo.Angle = value;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Browsable(false)]
        public override string ImageUrl
        {
            get
            {
                return base.ImageUrl;
            }
            set
            {
                base.ImageUrl = value;
            }
        }

        [DefaultValue(typeof(Color),"Black")]
        [Description("The color of the rendered symbols")]
        public override System.Drawing.Color ForeColor
        {
            get
            {
                return this._codeImageInfo.ForeColor;
            }
            set
            {
                this._codeImageInfo.ForeColor = value;
            }
        }

        /// <summary>
        /// Which symbol set to use to generate random number
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(SymbolSet.AlphaNumeric)]
        [Description("Which symbol set to use to generate the random string")]
        public SymbolSet SymbolSet
        {
            get
            {
                object val = this.ViewState["SymbolSet"];
                if (val == null)
                {
                    return SymbolSet.AlphaNumeric;
                }
                return (SymbolSet)val;
            }
            set
            {
                this.ViewState["SymbolSet"] = value;
            }
        }
        
        [Category("Appearance")]
        [DefaultValue(TextRenderingHint.SystemDefault)]
        [Description("Specifies the quality of the symbols")]
        public TextRenderingHint TextRenderingHint
        {
            get
            {
                return this._codeImageInfo.TextRenderingHint;
            }
            set
            {
                this._codeImageInfo.TextRenderingHint = value;
            }
        }


        /// <summary>
        /// The folder to hold generated images
        /// </summary>
        [Category("Behavior")]
        [DefaultValue("~/files/rcitemp/")]
        [Description("Where to hold genereated images")]
        public string ImagesTempFolder
        {
            get
            {
                return imagesTempFolder;
            }
            set
            {
                imagesTempFolder = value;
            }
        }
        string imagesTempFolder = "~/files/rcitemp/";

        /// <summary>
        /// Use a date-based salt or not
        /// </summary>
        [Category("Behavior")]
        [DefaultValue(true)]
        [Description("Use a date-based salt or not")]
        public bool UseDateSalt
        {
            get
            {
                return useDateSalt;
            }
            set
            {
                useDateSalt = value;
            }
        }
        bool useDateSalt = true;
  

        protected override void Render(HtmlTextWriter writer)
        {
            if (((this.Page != null) && (this.Page.Site != null)) && this.Page.Site.DesignMode)
            {
                base.ImageUrl = BackgroundImageUrl;
            }
            else
            {
                string token = GenerateImage();
				base.ImageUrl = GetImageUrl(token);
				writer.Write(string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", HiddenID, token));


            }
            base.Render(writer);
        }
		
		public string HiddenID
		{
			get
			{
				return this.UniqueID + "_guid";
			}
		}

        private string GetImageUrl(string token)
        {
            return Page.ResolveUrl(imagesTempFolder) + GetEncodedFileName(token) + ".png";
        }
        private string GetImagePath(string token)
        {
            return Page.Request.MapPath(imagesTempFolder) + GetEncodedFileName(token) + ".png";
        }
        static Regex regexBadChars = new Regex("[^\\w]{1}", RegexOptions.Compiled | RegexOptions.IgnoreCase);
        private string GetEncodedFileName(string token)
        {
            return regexBadChars.Replace(token, "_");
        }


        public string GenerateImage()
        {
            return GenerateImage(GetRandom(SymbolSet, Digits));
        }

        public string GenerateImage(string code)
        {
            _codeImageInfo.Code = code;
            string token = GetImageToken();
            string imageFile = GetImagePath(token);

            if (File.Exists(imageFile)
                && ((!useDateSalt) 
                || ((DateTime.UtcNow.Date - File.GetLastWriteTimeUtc(imageFile).Date).Days == 0)))
            {
                 return token;
            }


            Bitmap bg = null;
            ImageFormat imageFormat = ImageFormat.Png;

            string file = Page.Request.MapPath(_codeImageInfo.ImageUrl);
            if (File.Exists(file))
            {
                bg = new Bitmap(file);
            }
            else
            {
                bg = new Bitmap(_codeImageInfo.Size.Width, _codeImageInfo.Size.Height);
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

            Brush brush = new SolidBrush(_codeImageInfo.ForeColor);
            Bitmap bitmap = new Bitmap(bg.Width, bg.Height);
            Graphics gr = Graphics.FromImage(bitmap);
            gr.DrawImage(bg, 0, 0);
            gr.TextRenderingHint = _codeImageInfo.TextRenderingHint;
            float x = 2;
            Random rnd = new Random();
            StringFormat format = new StringFormat(StringFormatFlags.NoWrap | StringFormatFlags.FitBlackBox);
            format.Alignment = StringAlignment.Center;
            format.LineAlignment = StringAlignment.Center;
            for (int i = 0; i < _codeImageInfo.Code.Length; i++)
            {
                int angle = rnd.Next((-1) * _codeImageInfo.Angle, _codeImageInfo.Angle);
                if (_codeImageInfo.Angle > 0)
                {
                    gr.RotateTransform(angle);
                }
                SizeF sizeT = gr.MeasureString(_codeImageInfo.Code[i].ToString(), _codeImageInfo.Font);
                gr.DrawString(_codeImageInfo.Code[i].ToString(), _codeImageInfo.Font, brush, new RectangleF(x, 0, sizeT.Width, bitmap.Height), format);
                x += sizeT.Width;
                if (_codeImageInfo.Angle > 0)
                {
                    gr.RotateTransform(angle * (-1));
                }
            }
            gr.Dispose();
            bg.Dispose();

            bitmap.Save(imageFile, imageFormat);

            bitmap.Dispose();

            return token;
        }


        /// <summary>
        /// Determines if the given string is equal to 
        /// the one rendered on the image
        /// <param name="code">The string to compare</param>
        /// </summary>
        public bool IsValid(string code)
        {
            string token = "";
            if (Page.Request.HttpMethod == "GET")
            {
                if (Page.Request.QueryString[HiddenID] != null)
                {
                    token = Page.Request.QueryString[HiddenID];
                }
            }
            else  if (Page.Request.Form[HiddenID] != null)
            {
                token = Page.Request.Form[HiddenID];
            }
            return IsValid(code, token);
        }
        public bool IsValid(string code, string token)
        {
            if (code == null)
                return false;

            code = code.ToLower();

            return GetImageToken(code, DateTime.UtcNow) == token
                || GetImageToken(code, DateTime.UtcNow.AddDays(-1)) == token;
        }

        private string GetImageToken()
        {
            return GetImageToken(_codeImageInfo.Code, DateTime.UtcNow);
        }
        private string GetImageToken(string code, DateTime dateTime)
        {
            return ComputeHash(string.Format("{0}{1}"
                , code
                , useDateSalt ? dateTime.ToString("yyyy-MM-dd") : ""));
        }


        public static string ComputeHash(string word)
        {
            return Convert.ToBase64String(
                   new System.Security.Cryptography.SHA1CryptoServiceProvider()
                   .ComputeHash(System.Text.Encoding.UTF8.GetBytes(word)));
        }


        private string GetSessionKey(string guid)
        {
            return GetSessionKey(SessionKey, Page.Request.CurrentExecutionFilePath, this.ClientID, guid);
        }
		internal static string GetSessionKey(string key, string page, string id, string guid)
		{
			return key + "[" + page + "][" + id + "][" + guid + "]" ;
		}
//		internal static string GetSessionKey(string key, string page, string id)
//		{
//			return key + "[" + page + "][" + id + "][" + HttpContext.Current.Session.SessionID + "]" ;
//		}
		internal static string GetRandom(SymbolSet symbolSet, int digits)
        {
            string symbols = "qwertyuiopasdfghjklzxcvbnm1234567890";
            if (symbolSet == SymbolSet.Alpha)
            {
                symbols = "qwertyuiopasdfghjklzxcvbnm";
            }
            else if (symbolSet == SymbolSet.Numeric)
            {
                symbols = "1234567890";
            }
            Random r = new Random();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < digits; i++)
            {
                sb.Append(symbols[r.Next(symbols.Length-1)]);
            }
            return sb.ToString().ToUpper();
        }

    }
}
