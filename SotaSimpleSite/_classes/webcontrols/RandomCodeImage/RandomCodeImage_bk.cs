/*
 
using System;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Drawing.Design;
using System.Drawing;
using System.Drawing.Text;

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

        protected override void Render(HtmlTextWriter writer)
        {
            if (((this.Page != null) && (this.Page.Site != null)) && this.Page.Site.DesignMode)
            {
                base.ImageUrl = BackgroundImageUrl;
            }
            else
            {
				string guid =  Guid.NewGuid().ToString();
				_codeImageInfo.Code = GetRandom(SymbolSet, Digits);
				Page.Cache.Insert(GetSessionKey(guid), _codeImageInfo);
				base.ImageUrl = ResolveUrl(HandlerUrl) + "?key=" + SessionKey + "&id=" + this.ClientID + "&page=" + Page.Request.CurrentExecutionFilePath + "&g=" + guid;
				writer.Write(string.Format("<input type=\"hidden\" name=\"{0}\" value=\"{1}\"/>", HiddenID, guid));

//                Page.Cache.Insert(GetSessionKey(), _codeImageInfo);
//                base.ImageUrl = ResolveUrl(HandlerUrl) + "?key=" + SessionKey + "&id=" + this.ClientID + "&page=" + Page.Request.CurrentExecutionFilePath + "&g=" + Guid.NewGuid().ToString();
//                _codeImageInfo.Code = GetRandom(SymbolSet, Digits);

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

        /// <summary>
        /// Determines if the given string is equal to 
        /// the one rendered in the image
        /// <param name="code">The string to compare</param>
        /// </summary>
        public bool IsValid(string code)
        {
            CodeImageInfo codeImageInfo = (CodeImageInfo)HttpContext.Current.Cache[GetSessionKey(this.Page.Request.Form[HiddenID])];
            if (codeImageInfo == null)
            {
                return false;
            }
            return codeImageInfo.Code.ToUpper() == code.ToUpper();

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

*/