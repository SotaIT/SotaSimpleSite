using System.Drawing;
using System.Web;
using System.IO;
using System.Web.UI;
using System.ComponentModel;
using System.Drawing.Design;
using System;
using System.Text;
using System.Drawing.Drawing2D;

namespace Sota.Web.UI.WebControls
{
    [DefaultProperty("ImageUrl")]
    [ToolboxData("<{0}:Imager runat=\"server\"></{0}:Imager>")]
    public class Imager : System.Web.UI.WebControls.Image
    {
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
                return "~/image.ashx";
            }
            set
            {
                this.ViewState["HandlerUrl"] = value;
            }
        }
        
        [Category("Behavior")]
        [DefaultValue(0)]
        [Description("The time in seconds to cache the generated image")]
        public int Cache
        {
            get
            {

                object val = this.ViewState["Cache"];
                if (val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(val);
            }
            set
            {
                this.ViewState["Cache"] = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        [Description("Left offset in pixels if a background image is used")]
        public int DrawLeft
        {
            get
            {

                object val = this.ViewState["DrawLeft"];
                if (val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(val);
            }
            set
            {
                this.ViewState["DrawLeft"] = value;
                this.ViewState["HorizontalAlignment"] = HorizontalAlignment.NotSet;
            }
        }

        [Category("Appearance")]
        [DefaultValue(0)]
        [Description("Top offset in pixels if a background image is used")]
        public int DrawTop
        {
            get
            {

                object val = this.ViewState["DrawTop"];
                if (val == null)
                {
                    return 0;
                }
                return Convert.ToInt32(val);
            }
            set
            {
                this.ViewState["DrawTop"] = value;
                this.ViewState["VerticalAlignment"] = VerticalAlignment.NotSet;
            }
        }

        [Category("Appearance")]
        [DefaultValue(WebImageFormat.Auto)]
        [Description("The format of generated image")]
        public WebImageFormat WebImageFormat
        {
            get
            {
                object val = this.ViewState["WebImageFormat"];
                if (val == null)
                {
                    return WebImageFormat.Auto;
                }
                return (WebImageFormat)val;
            }
            set
            {
                this.ViewState["WebImageFormat"] = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(VerticalAlignment.NotSet)]
        [Description("Vertical alignment if a background image is used")]
        public VerticalAlignment VerticalAlignment
        {
            get
            {
                object val = this.ViewState["VerticalAlignment"];
                if (val == null)
                {
                    return VerticalAlignment.NotSet;
                }
                return (VerticalAlignment)val;
            }
            set
            {
                this.ViewState["VerticalAlignment"] = value;
                this.ViewState["DrawTop"] = 0;
            }
        }

        [Category("Appearance")]
        [DefaultValue(HorizontalAlignment.NotSet)]
        [Description("Horizontal alignment if a background image is used")]
        public HorizontalAlignment HorizontalAlignment
        {
            get
            {
                object val = this.ViewState["HorizontalAlignment"];
                if (val == null)
                {
                    return HorizontalAlignment.NotSet;
                }
                return (HorizontalAlignment)val;
            }
            set
            {
                this.ViewState["HorizontalAlignment"] = value;
                this.ViewState["DrawLeft"] = 0;
            }
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        [Description("If true indicates that the image musn't be cached")]
        public bool NoCache
        {
            get
            {

                object val = this.ViewState["NoCache"];
                if (val == null)
                {
                    return false;
                }
                return Convert.ToBoolean(val);
            }
            set
            {
                this.ViewState["NoCache"] = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(typeof(Size),"0,0")]
        [Description("The size of the generated image")]
        public Size DrawSize
        {
            get
            {
                object val = this.ViewState["DrawSize"];
                if (val == null)
                {
                    return Size.Empty;
                }
                return (Size)val;
            }
            set
            {
                this.ViewState["DrawSize"] = value;
            }
        }

		[Category("Appearance")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		[Description("The url of the image used as a background")]
		public string BackgroundImageUrl
		{
			get
			{
				string val = (string)this.ViewState["BackgroundImageUrl"];
				if (val != null)
				{
					return val;
				}
				return string.Empty;
			}
			set
			{
				this.ViewState["BackgroundImageUrl"] = value;
			}
		}

		[Category("Appearance")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		[Description("The url of the image used as a foreground")]
		public string ForegroundImageUrl
		{
			get
			{
				string val = (string)this.ViewState["ForegroundImageUrl"];
				if (val != null)
				{
					return val;
				}
				return string.Empty;
			}
			set
			{
				this.ViewState["ForegroundImageUrl"] = value;
			}
		}
		
		[Category("Appearance")]
		[Editor("System.Web.UI.Design.ImageUrlEditor, System.Design", typeof(UITypeEditor))]
		[Description("The url of the image displayed when the")]
		public string NotFoundImageUrl
		{
			get
			{
				string val = (string)this.ViewState["NotFoundImageUrl"];
				if (val != null)
				{
					return val;
				}
				return string.Empty;
			}
			set
			{
				this.ViewState["NotFoundImageUrl"] = value;
			}
		}

        [Category("Appearance")]
        [DefaultValue(SmoothingMode.Default)]
        [Description("Specifies the quality of the generated image")]
        public SmoothingMode SmoothingMode
        {
            get
            {
                object val = this.ViewState["SmoothingMode"];
                if (val == null)
                {
                    return SmoothingMode.Default;
                }
                return (SmoothingMode)val;
            }
            set
            {
                this.ViewState["SmoothingMode"] = value;
            }
        }

        [Category("Appearance")]
        [DefaultValue(CompositingQuality.Default)]
        [Description("Specifies the quality of the generated image")]
        public CompositingQuality CompositingQuality
        {
            get
            {
                object val = this.ViewState["CompositingQuality"];
                if (val == null)
                {
                    return CompositingQuality.Default;
                }
                return (CompositingQuality)val;
            }
            set
            {
                this.ViewState["CompositingQuality"] = value;
            }
        }
		private static bool IsNullOrEmpty(string s)
		{
			if (s != null)
			{
				return s.Length == 0;
			}
			return true;
		}
        protected override void Render(HtmlTextWriter writer)
        {
            if (this.Page==null 
                || this.Page.Site==null 
                || !this.Page.Site.DesignMode)
            {
                StringBuilder sb = new StringBuilder();
                sb.Append(ResolveUrl(HandlerUrl));
                sb.Append("?url=");
                sb.Append(ResolveUrl(ImageUrl));
				if (!IsNullOrEmpty(BackgroundImageUrl))
				{
					sb.Append("&bg=");
					sb.Append(ResolveUrl(BackgroundImageUrl));
				}
				if (!IsNullOrEmpty(ForegroundImageUrl))
				{
					sb.Append("&fg=");
					sb.Append(ResolveUrl(ForegroundImageUrl));
				}
				if (!IsNullOrEmpty(NotFoundImageUrl))
				{
					sb.Append("&nf=");
					sb.Append(ResolveUrl(NotFoundImageUrl));
				}
				if (DrawSize.Height > 0)
                {
                    sb.Append("&h=");
                    sb.Append(DrawSize.Height);
                }
                if (DrawSize.Width > 0)
                {
                    sb.Append("&w=");
                    sb.Append(DrawSize.Width);
                }
                if (VerticalAlignment != VerticalAlignment.NotSet)
                {
                    sb.Append("&y=");
                    sb.Append(VerticalAlignment.ToString().Substring(0, 1).ToLower());
                }
                else if (DrawTop > 0)
                {
                    sb.Append("&y=");
                    sb.Append(DrawTop);
                }
                if (HorizontalAlignment != HorizontalAlignment.NotSet)
                {
                    sb.Append("&x=");
                    sb.Append(HorizontalAlignment.ToString().Substring(0, 1).ToLower());
                }
                else if (DrawLeft > 0)
                {
                    sb.Append("&x=");
                    sb.Append(DrawLeft);
                }
                if (NoCache)
                {
                    sb.Append("&nocache=");
                }
                if (Cache>0)
                {
                    sb.Append("&cache=");
                    sb.Append(Cache);
                }
                if (SmoothingMode != SmoothingMode.Default)
                {
                    sb.Append("&sm=");
                    sb.Append(SmoothingMode);
                }
				if (CompositingQuality != CompositingQuality.Default)
				{
					sb.Append("&cq=");
					sb.Append(CompositingQuality);
				}
				if (WebImageFormat != WebImageFormat.Auto)
				{
					sb.Append("&f=");
					sb.Append(WebImageFormat.ToString());
				}
				ImageUrl = sb.ToString();
            }
            base.Render(writer);
        }
       
		/// <summary>
        /// Gets the size of the image from file
        /// </summary>
        /// <param name="file">The path of the file containing image</param>
        /// <returns></returns>
        public static Size GetImageSize(string file)
        {			
            return ImagerBuilder.GetImageSize(file);
        }
    }
}
