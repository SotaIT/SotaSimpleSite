using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Text;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Базовая страница.
	/// </summary>
	public class BasePage : System.Web.UI.Page
	{
		public System.Web.UI.Control MainPlaceHolder
		{
			get { return FindPlaceHolder(Keys.ControlPHContent); }
		}

		public virtual System.Web.UI.Control FindPlaceHolder(string id)
		{
			return FindControl(id);
		}

		bool _lastModidifiedSet = false;
		static Regex regexControl = new Regex(@"<img[^>]*src=[""']{1}[^""']*?control.ashx\?c=([^""']+)[""']{1}[^>]*>", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.IgnoreCase);
		public virtual void LoadContent()
		{
            if (Request.QueryString[Keys.QueryStringLogout] != null)
            {
                UserInfo.LogOut();
                Response.Redirect(Path.Full.Split('?')[0]);
            }

            if (Request.QueryString[Keys.QueryStringAutoLogin] != null)
            {
                string token = Request.QueryString[Keys.QueryStringAutoLogin];
                UserInfo.LoginNew(token);
                Response.Redirect(Path.Full.Split('?')[0]);
            }

			PageInfo pi = PageInfo.Current;
			if (pi == null)
			{
				return;
			}
			if(pi.SecurityPart != -1)
			{
				if (!UserInfo.Current.CanAccess(pi.SecurityPart)
					&& !IsLoginPage)
				{
					Response.Redirect(LoginPage + Keys.UrlParamPageDelimiter + Keys.QueryStringUrl + Keys.UrlParamValueDelimiter + Server.UrlEncode(Path.Full));
				}
			}
			switch(pi.GenerateLastModified)
			{
				case GenerateLastModified.FromFiles:
					SetLastModified(System.IO.File.GetLastWriteTime(pi.DataFileName));
					SetLastModified(System.IO.File.GetLastWriteTime(Request.PhysicalPath));
					break;
				case GenerateLastModified.Now:
					SetLastModified(DateTime.Now);
					break;
			}
			if (pi.Cache > 0)
			{
				Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
				Response.Cache.SetVaryByCustom(Keys.CustomPath);
				Response.Cache.SetExpires(DateTime.Now.AddSeconds(pi.Cache));
			}
			if (pi.Header.Length > 0)
			{
				string[] headers = pi.Header.Split('\n');
				for(int i=0;i<headers.Length;i++)
				{
					Response.AppendHeader(
						headers[i].Substring(0,headers[i].IndexOf(":")),
						headers[i].Substring(headers[i].IndexOf(":")+1));
				}
			}
			if(pi.StatusCode > 0)
			{
				Response.StatusCode = pi.StatusCode;
			}
			System.Web.UI.Control mc = this.MainPlaceHolder;
			if (mc != null)
			{
				if (pi.CodeFile.Length > 0)
				{
					mc.Controls.Add(this.LoadControl(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.CodeFile));
					if(pi.GenerateLastModified == GenerateLastModified.FromFiles)
					{
						SetLastModified(System.IO.File.GetLastWriteTime(Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.CodeFile)));
					}
				}
				else
				{
					if (!Config.IsBlank(pi.Body))
					{
						MatchCollection col = regexControl.Matches(pi.Body);
						if(col.Count==0)
						{
							Literal l = new Literal();
							l.Text = pi.Body;//this.ParseContent()
							mc.Controls.Add(l);
						}
						else
						{
							int start = 0;
							System.Web.UI.Control ctrl;
							Literal lt;
							foreach(Match m in col)
							{
								lt = new Literal();
								lt.Text = pi.Body.Substring(start, m.Index-start);//this.ParseContent()
								start = m.Index+m.Length;
								mc.Controls.Add(lt);
								string[] arrC = m.Groups[1].Value.Split(Keys.UrlParamDelimiter[0]);
								ctrl = LoadControl(Keys.ServerRoot + Keys.UrlPathDelimiter + arrC[0]);
								if(ctrl != null)
								{
									if(arrC.Length > 1)
									{
										PropertyInfo[] prop = ctrl.GetType().GetProperties();
										for(int i=1;i<arrC.Length;i++)
										{
											int j = arrC[i].IndexOf(Keys.UrlParamValueDelimiter);
											string pr = arrC[i].Substring(0,j);
											string val = arrC[i].Substring(j+1);
											for(int p=0;p<prop.Length;p++)
											{
												if(prop[p].Name==pr)
												{
													prop[p].SetValue(ctrl,val,null);
												}
											}
										}
									}
									mc.Controls.Add(ctrl);
								}
							}
							if(start<pi.Body.Length)
							{
								lt = new Literal();
								lt.Text = pi.Body.Substring(start);
								mc.Controls.Add(lt);
							}
						}
					}
					if(pi.GenerateLastModified == GenerateLastModified.Standard)
					{
						SetLastModified(System.IO.File.GetLastWriteTime(pi.DataFileName));
						SetLastModified(System.IO.File.GetLastWriteTime(Request.PhysicalPath));
					}
				}
			}
			System.Web.UI.Control c;
			DataTable t = Config.GetConfigTable("controls.config", "load");
			if (t.Rows.Count > 0)
			{
				DateTime dtControls = System.IO.File.GetLastWriteTime(Request.MapPath(Config.ConfigFolderPath + "/controls.config"));
				DataRow[] rows = t.Select("(page='" + pi.FileName + "') OR (template='" + pi.Template + "')", "order ASC");
				for (int i = 0; i < rows.Length; i++)
				{
					if(rows[i]["allow"].ToString()=="0"
						|| ( Util.IsBlank(rows[i]["page"])
						&& t.Select("(page='" + pi.FileName + "') AND (control='" + rows[i]["control"].ToString() + "') AND (placeholder='" + rows[i]["placeholder"].ToString() + "') AND (allow='0')").Length>0))
					{
						continue;
					}
					c = FindPlaceHolder(rows[i]["placeholder"].ToString());
					if (c != null)
					{
						SetLastModifiedIfSet(dtControls);
						SetLastModifiedIfSet(System.IO.File.GetLastWriteTime(Request.MapPath(Keys.ServerRoot + Keys.UrlPathDelimiter + rows[i]["control"].ToString())));
						c.Controls.Add(this.LoadControl(Keys.ServerRoot + Keys.UrlPathDelimiter + rows[i]["control"].ToString()));
					}
				}
			}
		}
		/// <summary>
		/// Используется для установки даты изменения у статических страниц из блоков
		/// </summary>
		/// <param name="dateTime">Дата изменения</param>
		public void SetLastModifiedIfSet(DateTime dateTime)
		{
			if(_lastModidifiedSet)
			{
				if(dateTime < DateTime.Now)
				{
					Response.Cache.SetLastModified(dateTime);
				}
			}
		}
		/// <summary>
		/// Используется для установки даты изменения у страницы
		/// и устанавливает флаг для метода SetLastModifiedIfSet
		/// </summary>
		/// <param name="dateTime">Дата изменения</param>
		public void SetLastModified(DateTime dateTime)
		{
			SetLastModified(dateTime, true);
		}
		/// <summary>
		/// Используется для установки даты изменения у страницы
		/// </summary>
		/// <param name="dateTime">Дата изменения</param>
		/// <param name="setFlag">Установить флаг изменения для метода SetLastModifiedIfSet</param>
		public void SetLastModified(DateTime dateTime, bool setFlag)
		{
			if(setFlag)
			{
				_lastModidifiedSet = true;
			}
			if(dateTime < DateTime.Now)
			{
				Response.Cache.SetLastModified(dateTime);
			}
		}
		protected override void OnLoad(EventArgs e)
		{
			LoadContent();
			base.OnLoad(e);
		}


		#region Config & other properties

		public virtual string LoginPage
		{
			get { return ResolveUrl(Config.Main.LoginPage); }
		}

		public bool IsLoginPage
		{
			get { return Path.Full.Split(Keys.UrlParamPageDelimiter[0])[0].ToLower().EndsWith(LoginPage.ToLower()); }
		}

		public string Images
		{
			get { return ResolveUrl(Sota.Web.SimpleSite.Config.Main.Images); }
		}

		public string Css
		{
			get { return ResolveUrl(Sota.Web.SimpleSite.Config.Main.Css); }
		}

		public string Files
		{
			get { return ResolveUrl(Sota.Web.SimpleSite.Config.Main.Files); }
		}

		public string Extention
		{
			get { return Sota.Web.SimpleSite.Config.Main.Extension; }
		}

		public string Script
		{
			get { return ResolveUrl(Sota.Web.SimpleSite.Config.Main.Script); }
		}

		public string RedirectPage
		{
			get { return ResolveUrl(Sota.Web.SimpleSite.Config.Main.RedirectPage); }
		}

		public string PagePath
		{
			get { return Sota.Web.SimpleSite.Path.Page; }
		}

		public string Domain
		{
			get { return Sota.Web.SimpleSite.Path.Domain; }
		}

		public string FullPath
		{
			get { return Sota.Web.SimpleSite.Path.Full; }
		}

//		public string SkinImages
//		{
//			get
//			{
//				Skin s = Skin.Current;
//				if (s == null)
//					return null;
//				return s.Images;
//			}
//		}

		public Config.MainConfig MainConfig
		{
			get { return Config.Main; }
		}

//		public string SkinCss
//		{
//			get
//			{
//				Skin s = Skin.Current;
//				if (s == null)
//					return null;
//				return s.Css;
//			}
//		}

		public PageInfo PageInfo
		{
			get { return PageInfo.Current; }
		}

		public SiteMap SiteMap
		{
			get { return SiteMap.Current; }
		}

		public string VRoot
		{
			get { return Path.VRoot; }
		}

		public string ARoot
		{
			get { return Path.ARoot; }
		}

		#endregion

		#region labels
		public string ParseContent(string str)
		{
			return str;
			/*
			if (Config.IsBlank(str))
			{
				return string.Empty;
			}

			StringBuilder sb = new StringBuilder(str);
			sb.Replace(ExtensionLabelTag, Config.Main.Extension);
			sb.Replace(ImgLabelTag, ResolveUrl(Config.Main.Images));
			sb.Replace(CssLabelTag, ResolveUrl(Config.Main.Css));
			//sb.Replace(SkinImgLabelTag, ResolveUrl(Skin.Current.Images));
			//sb.Replace(SkinCssLabelTag, ResolveUrl(Skin.Current.Css));
			sb.Replace(ScriptLabelTag, ResolveUrl(Config.Main.Script));
			sb.Replace(FilesLabelTag, ResolveUrl(Config.Main.Files));
			sb.Replace(RedirectPageLabelTag, ResolveUrl(Config.Main.RedirectPage));
			sb.Replace(RootLabelTag, Sota.Web.SimpleSite.Path.Domain);
			sb.Replace(PageLabelTag, Sota.Web.SimpleSite.Path.Page);
			sb.Replace(PathLabelTag, Sota.Web.SimpleSite.Path.Full);
			sb.Replace(NowLabelTag, Config.Main.Now().ToString());
			sb.Replace(DateLabelTag, Config.Main.Now().ToShortDateString());
			sb.Replace(TimeLabelTag, Config.Main.Now().ToShortTimeString());
			sb.Replace(LDateLabelTag, Config.Main.Now().ToLongDateString());
			sb.Replace(LTimeLabelTag, Config.Main.Now().ToLongTimeString());
			sb.Replace(DownloadLabelTag, ResolveUrl(Config.Main.DownloadPage));
			sb.Replace(ImagePageLabelTag, ResolveUrl(Config.Main.ImagePage));
			return sb.ToString();*/
		}

		public static string ClearMetaLanguage(string str)
		{
			return str;
			/*
			StringBuilder sb = new StringBuilder(str);
			sb.Replace(ExtensionLabelTag, string.Empty);
			sb.Replace(ImgLabelTag, string.Empty);
			sb.Replace(CssLabelTag, string.Empty);
			sb.Replace(SkinImgLabelTag, string.Empty);
			sb.Replace(SkinCssLabelTag, string.Empty);
			sb.Replace(ScriptLabelTag, string.Empty);
			sb.Replace(FilesLabelTag, string.Empty);
			sb.Replace(RedirectPageLabelTag, string.Empty);
			sb.Replace(RootLabelTag, string.Empty);
			sb.Replace(PageLabelTag, string.Empty);
			sb.Replace(PathLabelTag, string.Empty);
			sb.Replace(NowLabelTag, string.Empty);
			sb.Replace(DateLabelTag, string.Empty);
			sb.Replace(TimeLabelTag, string.Empty);
			sb.Replace(LDateLabelTag, string.Empty);
			sb.Replace(LTimeLabelTag, string.Empty);
			sb.Replace(DownloadLabelTag, string.Empty);
			sb.Replace(ImagePageLabelTag, string.Empty);
			return sb.ToString();*/
		}
/*
		public const string ImgLabelTag = "[#img#]";
		public const string CssLabelTag = "[#css#]";
		public const string SkinImgLabelTag = "[#skinimg#]";
		public const string SkinCssLabelTag = "[#skincss#]";
		public const string ScriptLabelTag = "[#script#]";
		public const string FilesLabelTag = "[#files#]";
		public const string RedirectPageLabelTag = "[#redir#]";
		public const string ExtensionLabelTag = "[#ext#]";
		public const string RootLabelTag = "[#root#]";
		public const string PageLabelTag = "[#page#]";
		public const string PathLabelTag = "[#path#]";
		public const string NowLabelTag = "[#now#]";
		public const string DateLabelTag = "[#date#]";
		public const string TimeLabelTag = "[#time#]";
		public const string LDateLabelTag = "[#ldate#]";
		public const string LTimeLabelTag = "[#ltime#]";
		public const string DownloadLabelTag = "[#dload#]";
		public const string ImagePageLabelTag = "[#imgrs#]";
*/
		#endregion

		#region utils

		#region control content
		public bool SetControlContent(string id, string s, bool append)
		{
			Control c = FindControl(id);
			if(c!=null)
			{
				if(!append)
				{
					c.Controls.Clear();
				}
				Literal l = new Literal();
				l.Text = s;
				c.Controls.Add(l);
				return true;
			}
			return false;
		}
		public bool AppendControlContent(string id, string s)
		{
			return SetControlContent(id, s, true);
		}
		public bool ReplaceControlContent(string id, string s)
		{
			return SetControlContent(id, s, false);
		}
		public bool SetControlContent(string id, Control c, bool append, bool hideAfter, bool showBefore)
		{
			if(showBefore)
			{
				c.Visible = true;
			}
			StringWriter s = new StringWriter();
			c.RenderControl(new HtmlTextWriter(s));
			if(hideAfter)
			{
				c.Visible = false;
			}
			return SetControlContent(id, s.ToString(),append);
		}
		public bool SetControlContent(string id, Control c, bool append)
		{
			return SetControlContent(id, c, append, true, false);
		}
		public bool AppendControlContent(string id, Control c)
		{
			return SetControlContent(id, c, true);
		}
		public bool ReplaceControlContent(string id, Control c)
		{
			return SetControlContent(id, c, false);
		}
		
		#endregion

		#endregion
	}
}