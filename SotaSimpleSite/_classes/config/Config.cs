using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Specialized;
using System.Configuration;
using System.Data;
using System.IO;
using System.Web.Caching;
using System.Web;
using System;
using System.Text;
using System.Web.UI.WebControls;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Осуществляет доступ к различным настройкам сайта
	/// </summary>
	public sealed class Config
	{
		private Config()
		{
		}
		
		#region errors
		public static void ReportError(string error)
		{
			ReportError(error, Path.Full);
		}
		public static void ReportError(string error, string path)
		{
			if(Main.LogError)
			{
				Hashtable h	= new Hashtable();
				h["path"]	= path;
				h["error"]	= error;
				try
				{
					Log.Write("Exception", h);
					return;
				}
				catch{
                    HttpContext cntx = HttpContext.Current;
                    string file = cntx.Request.MapPath("~/error.log.config");
                    StreamWriter w = null;
                    try
                    {
                        w = new StreamWriter(file, true, Encoding.UTF8);
                        w.WriteLine(Main.Now());
                        w.WriteLine(path);
                        w.WriteLine(error);
                        w.WriteLine("");
                    }
                    finally
                    {
                        if (w != null)
                            w.Close();
                    }
                }
			}
		}
		public static void ReportError(Exception ex)
		{
			ReportError(ex, Path.Full);
		}
		public static void ReportError(Exception ex, string path)
		{
			if(Main.LogError)
			{
				Hashtable h		= Util.GetClientInfo(HttpContext.Current.Request);
				if(ex.GetType()==typeof(HttpCompileException))
				{
					CompilerErrorCollection arr = ((HttpCompileException)ex).Results.Errors;
					if(arr.HasErrors)
					{
						StringBuilder sb = new StringBuilder();
						for(int i=0;i<arr.Count;i++)
						{
							sb.AppendFormat("<br />{0}, Line {1}, Column {2}: {3}: {4}",arr[i].FileName, arr[i].Line, arr[i].Column, arr[i].ErrorNumber, arr[i].ErrorText);
						}
						h["error"] = sb.ToString();
					}
				}
				else
				{
					h["error"]		= ex.ToString();
				}
				Log.Write(ex.GetType().Name, h);
			}
			else
			{
				ReportError(ex.ToString(), path);
			}
		}
		#endregion

		#region misc
		public static string ConfigFolderPath
		{
			get { return App[Keys.KeyConfigFolderPath]; }
		}
		/*public static string ConfigFolderPath
		{
			get { return App[Keys.KeyConfigFolderPath]; }
		}
		public static string SiteConfigFolderPath
		{
			get { return ConfigFolderPath+(Path.Site.Length>0 ? "/"+Path.Site : ""); }
		}*/
		/// <summary>
		/// Заполняет список часовыми поясами
		/// </summary>
		/// <param name="cmbTimeZone">Список, который нужно заполнить</param>
		public static void FillDropDownList(DropDownList cmbTimeZone)
		{
			cmbTimeZone.Items.Clear();
			DataTable tb = Config.GetConfigTable("timezone.config","timezone");
			foreach(DataRow r in tb.Rows)
			{
				cmbTimeZone.Items.Add(new ListItem(r["name"].ToString(), r["id"].ToString()));
			}

		}
		/// <summary>
		/// Делает первую букву заглавное, а остальные строчными
		/// </summary>
		/// <param name="s">Исходная строка</param>
		/// <returns>Исправленная строка</returns>
		public static string CapitalizeString(string s)
		{
			return Util.CapitalizeString(s);
		}
		/// <summary>
		/// Проверяет строку на null и ""
		/// </summary>
		/// <param name="s">Строка</param>
		/// <returns>True если строка равна null или "", False - в противном случае</returns>
		public static bool IsNullOrEmpty(string s)
		{
			return Util.IsNullOrEmpty(s);
		}
		public static bool IsNullOrEmpty(object s)
		{
			return Util.IsNullOrEmpty(s);
		}
		/// <summary>
		/// Проверяет строку на null, "" и пробелы
		/// </summary>
		/// <param name="s">Строка</param>
		/// <returns>True если строка равна null или "" или состоит из пробелов, False - в противном случае</returns>
		public static bool IsBlank(string s)
		{
			return Util.IsBlank(s);
		}
		public static bool IsBlank(object s)
		{
			return Util.IsBlank(s);
		}
		/// <summary>
		/// Проверка версии SotaSimpleSite
		/// </summary>
		/// <returns>Строка, содеражащая версию</returns>
		public static string GetVersion()
		{
			return Util.GetVersion();
		}
		/// <summary>
		/// Версия .NET
		/// </summary>
		/// <returns>Строка, содеражащая версию</returns>
		public static string GetRuntimeVersion()
		{
			return Util.GetRuntimeVersion();
		}
		public static string GetProtocol(bool isSecure)
		{
			return Util.GetProtocol(isSecure);
		}
		/// <summary>
		/// IP-адрес пользователя
		/// </summary>
		/// <returns>Строка, содержащая IP-адрес пользователя и/или IP-адрес прокси-сервера</returns>
		public static string GetClientIP()
		{
			return Util.GetClientIP();
		}
		public static string GetClientIP(HttpRequest request)
		{
			return Util.GetClientIP(request);
		}
		public static Hashtable GetClientInfo()
		{
			return Util.GetClientInfo();
		}
		public static Hashtable GetClientInfo(HttpRequest request)
		{
			return Util.GetClientInfo(request);
		}
		public static string CreateHtmlLink(object url, object text, bool blank)
		{
			return Util.CreateHtmlLink(url,text,blank);
		}
		public static string CreateHtmlLink(object url, object text)
		{
			return Util.CreateHtmlLink(url, text, true);
		}
		public static string CreateHtmlLink(object url)
		{
			return Util.CreateHtmlLink(url, url, true);
		}
		public static string FilterCookieValue(string text)
		{
			return Util.FilterCookieValue(text);
		}

		/// <summary>
		/// Убирает "опасное" из текста, 
		/// введенного пользователем
		/// </summary>
		/// <param name="text">Текст, введенный пользователем</param>
		/// <returns>"Безопасный" текст</returns>
		public static string FilterText(string text)
		{
			return Util.FilterText(text);
		}

		#endregion

		#region appsett		
		public static NameValueCollection App
		{
			get
			{
                return ConfigurationManager.AppSettings;//ConfigurationSettings.AppSettings;
			}
		}
		/*
		private static string _ok = "ok";
		internal static bool ok
		{
			get
			{
				if(_ok != "ok")
				{
					Ok();
				}
				return _ok == "ok";
			}
		}
		internal static void Ok()
		{
			return;
			string name = HttpContext.Current.Request.Url.Host.ToLower();
			if(name==Keys.HostOk)
			{
				_ok = "ok";
				return;
			}
			string ok = Util.ContentFromUrl(Keys.UrlOk + name);
			if(ok == null || ok == "ok")
			{
				_ok = "ok";
			}
			else
			{
				_ok = null;
			}
		}
		*/
		#endregion

		#region xml.config

		public static DataTable GetSkins()
		{
			return GetConfigTable(Keys.ConfigSkin, Keys.TableNameSkin);
		}

		public static DataTable GetDomains()
		{
			DataTable tb = GetConfigTable(Keys.ConfigDomain, Keys.TableNameDomain);
			if (tb.Columns.Count == 0)
			{
				tb.Columns.Add("name");
				tb.Columns.Add("addpath");
				tb.Columns.Add("site");
			}
			return tb;
		}

		public static DataTable GetPagex()
		{
			return GetConfigTable(Keys.ConfigPagex, Keys.TableNamePagex);
		}

		public static DataTable GetNoRewrite()
		{
			return GetConfigTable(Keys.ConfigNoRewrite, Keys.TableNameNoRewrite);
		}

		public static DataTable GetMain()
		{
			string configFileName = Keys.ConfigMain;
			string tableName = Keys.TableNameMain;
			string cacheKey = tableName + Keys.CacheTableConfigDelimiter + configFileName;
			DataTable tb = null;
			Cache c = HttpContext.Current.Cache;
			if (c[cacheKey] != null)
			{
				tb = (DataTable) c[cacheKey];
			}
			else
			{
				string file = HttpContext.Current.Request.MapPath(ConfigFolderPath + Keys.UrlPathDelimiter + configFileName);
				DataSet ds = new DataSet();
				ds.ReadXml(file);
				tb = ds.Tables[tableName];
				c.Insert(cacheKey, tb, new CacheDependency(file));
				Main._LastUpdate = DateTime.Now;
			}
			return tb;
		}

		public static DataTable GetSearch()
		{
			return GetConfigTable(Keys.ConfigSearch, Keys.TableNameSearch);
		}

		/// <summary>
		/// Метод считывает информацию из xml-файла конфигурации
		/// и сохраняет её в кэш, а при последующих обращениях достает оттуда.
		/// При этом кэш зависит от файла и сбрасывается после редактирования исходного файла.
		/// </summary>
		/// <param name="configFileName">имя config-файла из папки /config</param>
		/// <param name="tableName">имя таблицы в config-файле</param>
		/// <returns>DataTable</returns>
		public static DataTable GetConfigTable(string configFileName, string tableName)
		{
			DataTable tb = null;
			string cacheKey = GetCachedTableCacheKey(configFileName, tableName);
			Cache c = HttpContext.Current.Cache;
			if (c[cacheKey] != null)
			{
				tb = (DataTable) c[cacheKey];
			}
			else
			{
				string file = HttpContext.Current.Request.MapPath(ConfigFolderPath + Keys.UrlPathDelimiter + configFileName);
				DataSet ds = new DataSet();
				ds.ReadXml(file);
				tb = ds.Tables[tableName];
				if (tb == null)
					return new DataTable(tableName);
				c.Insert(cacheKey, tb, new CacheDependency(file));
			}
			return tb;
		}
		public static DataTable GetConfigTableNoCache(string configFileName, string tableName)
		{
			string file = HttpContext.Current.Request.MapPath(ConfigFolderPath + Keys.UrlPathDelimiter + configFileName);
			DataSet ds = new DataSet();
			ds.ReadXml(file);
			DataTable tb = ds.Tables[tableName];
			if (tb == null)
				return new DataTable(tableName);
			return tb;
		}

		public static string GetCachedTableCacheKey(string configFileName, string tableName)
		{
			return tableName + Keys.CacheTableConfigDelimiter + configFileName;
		}

		internal static void WriteConfigTable(string configFileName, DataTable tb, string root)
		{
			string file = HttpContext.Current.Request.MapPath(ConfigFolderPath + Keys.UrlPathDelimiter + configFileName);
			DataSet ds = new DataSet(root);
			try
			{
				ds.ReadXml(file);
			}
			catch(Exception ex)
			{
				Config.ReportError(ex);
			}
			if (ds.Tables.Contains(tb.TableName))
			{
				DataTable tbOld = ds.Tables[tb.TableName];
				tbOld.Rows.Clear();
				for (int i = 0; i < tb.Rows.Count; i++)
				{
					tbOld.ImportRow(tb.Rows[i]);
				}
			}
			else
			{
				ds.Tables.Add(tb);
			}
			ds.WriteXml(file);
			HttpContext.Current.Cache.Remove(GetCachedTableCacheKey(configFileName, tb.TableName));
		}

		#endregion

		#region main.config

		private static MainConfig _main = new MainConfig();

		public static MainConfig Main
		{
			get { return _main; }
		}

		public class MainConfig
		{
			public MainConfig()
			{
			}

			public int TimeZone
			{
				get { return int.Parse(GetConfig(Keys.KeyMainTimeZone)); }
			}

			public long OnlineTimeOut
			{
				get { return long.Parse(GetConfig(Keys.KeyMainOnlineTimeOut)); }
			}
			/// <summary>
			/// Возвращает дату начала завтрашнего дня (время 00:00:00)
			/// во времени в соответсвии с TimeZone приведенную 
			/// ко времени сервера
			/// </summary>
			public DateTime MyTomorrow()
			{
				DateTime now = Now();
				return now.AddDays(1).Date;// +TimeOffset();//.AddHours(DateTime.Now.Hour-now.Hour);
			}
			/// <summary>
			/// Текущая дата и время сайта в UTC 0
			/// </summary>
			public DateTime Now()
			{
				return TimeZoneInfo.Current.Now();
				//return DateTime.UtcNow;
			}
			/// <summary>
			/// Разница между временем сайта и временем на сервере
			/// </summary>
			public TimeSpan TimeOffset()
			{
				return TimeZoneInfo.Current.Offset;
				//return TimeSpan.FromMinutes(Math.Round((Now() - DateTime.Now).TotalMinutes));
			}
			/// <summary>
			/// Привести серверную дату/время в дату/время сайта
			/// </summary>	
			public DateTime ServerToSite(DateTime dateTime)
			{
				return dateTime + TimeOffset();
			}
			/// <summary>
			/// Привести дату/время сайта в дату/время сервера
			/// </summary>	
			public DateTime SiteToServer(DateTime dateTime)
			{
				return dateTime - TimeOffset();
			}
			public string RedirectPage
			{
				get { return GetConfig(Keys.KeyMainRedirectPage); }
			}
			public int HashMode
			{
				get { return int.Parse(GetConfig(Keys.KeyMainHashMode)); }
			}

			public string ImagePage
			{
				get { return GetConfig(Keys.KeyMainImagePage); }
			}

			public bool LogRedirect
			{
				get { return GetConfig(Keys.KeyMainLogRedirect)=="1"; }
			}
			public bool Off
			{
				get { return GetConfig(Keys.KeyMainOff)=="1"; }
			}
			public bool LogError
			{
				get { return GetConfig(Keys.KeyMainLogError)=="1"; }
			}
			public bool LogDownload
			{
				get { return GetConfig(Keys.KeyMainLogDownload)=="1"; }
			}

			public string Extension
			{
				get { return GetConfig(Keys.KeyMainExtension); }
			}

			public byte[] EncryptionKey
			{
				get
				{
					string key = GetConfig(Keys.KeyEncryptionKey);
					if(Util.IsBlank(key))
					{
						key							= Guid.NewGuid().ToString("N").Substring(0,16);
						DataTable tb				= Config.GetMain();
						DataRow r					= tb.Rows[0];
						r[Keys.KeyEncryptionKey]	= key;
						Config.WriteConfigTable("main.config",tb,"main");
					}
					return Encoding.UTF8.GetBytes(key);
				}
			}

			public byte[] EncryptionVI
			{
				get
				{
					string vi = GetConfig(Keys.KeyEncryptionVI);
					if(Util.IsBlank(vi))
					{
						vi							= Guid.NewGuid().ToString("N").Substring(0,16);
						DataTable tb				= Config.GetMain();
						DataRow r					= tb.Rows[0];
						r[Keys.KeyEncryptionVI]		= vi;
						Config.WriteConfigTable("main.config",tb,"main");
					}
					return Encoding.UTF8.GetBytes(vi);
				}
			}
			public EncryptionLevel EncryptionLevel
			{
				get
				{
					return (EncryptionLevel)int.Parse(GetConfig(Keys.KeyEncryptionLevel));
				}
			}

			public string TitleSeparator
			{
				get { return GetConfig(Keys.KeyMainTitleSeparator); }
			}

			public string DefaultSkin
			{
				get { return GetConfig(Keys.KeyMainDefaultSkin); }
			}

			public string AdminDefault
			{
				get { return GetConfig(Keys.KeyMainAdminDefault); }
			}

			public string DefaultTemplate
			{
				get { return GetConfig(Keys.KeyMainDefaultTemplate); }
			}

			public string FileManagerPage
			{
				get { return GetConfig(Keys.KeyMainFileManagerPage); }
			}

			public string HtmlEditorPage
			{
				get { return GetConfig(Keys.KeyMainHtmlEditorPage); }
			}

			public string DownloadPage
			{
				get { return GetConfig(Keys.KeyMainDownloadPage); }
			}

			public string DatePickerPage
			{
				get { return GetConfig(Keys.KeyMainDatePickerPage); }
			}

			public string TimePickerPage
			{
				get { return GetConfig(Keys.KeyMainTimePickerPage); }
			}

			public string AdminFolderName
			{
				get { return "admin";}//GetConfig(Keys.KeyMainAdminFolderName); }
			}

			public string AdminLoginPage
			{
				get { return GetConfig(Keys.KeyMainAdminLoginPage); }
			}

			public string LoginPage
			{
				get { return GetConfig(Keys.KeyMainLoginPage); }
			}

			public bool AuthorizationEnabled
			{
				get { return GetConfig(Keys.KeyMainEnableAuthorization) == "1"; }
			}
			public bool SeoError
			{
				get { return GetConfig(Keys.KeyMainSeoError) == "1"; }
			}

			public string AdminPassword
			{
				get { return GetConfig(Keys.KeyMainAdminPassword); }
			}

			public string ManagerPassword
			{
				get { return GetConfig(Keys.KeyMainManagerPassword); }
			}

			public DateTime LastUpdate
			{
				get { return _LastUpdate; }
			}

			internal DateTime _LastUpdate = DateTime.Now;
			private DataTable tbMain = null;

			public virtual string GetConfig(string key)
			{
				try
				{
					tbMain = GetMain();
					if (tbMain.Columns.Contains(key))
						return (string) (tbMain.Rows[0][key]);
				}
				catch(Exception ex)
				{
					Config.ReportError(ex);
				}
				return string.Empty;
			}

			public string Images
			{
				get { return GetConfig(Keys.KeyMainImages); }
			}

			public string Css
			{
				get { return GetConfig(Keys.KeyMainCss); }
			}

			public string Script
			{
				get { return GetConfig(Keys.KeyMainScript); }
			}

			public string Files
			{
				get { return GetConfig(Keys.KeyMainFiles); }
			}

			public string Data
			{
				get { return "~/data";}
			}

			public string ConnectionString
			{
				get { return GetConfig(Keys.KeyMainConnectionString); }
			}

			public string RedirectAll
			{
				get { return GetConfig(Keys.KeyMainRedirectAll); }
			}

			public string GetCustom(string name)
			{
				string[] custom = GetConfig(Keys.KeyMainCustom).Split('&');
				foreach(string key in custom)
				{
					string[] values = key.Split('=');
					if(values[0] == name)
					{
						return values[1];
					}
				}
				return null;
			}
            public int PageCache
            {
                get { return int.Parse(GetConfig(Keys.KeyMainPageCache)); }
            }

		}

		#endregion

		#region search.config

		private static SearchConfig _search = new SearchConfig();

		public static SearchConfig Search
		{
			get { return _search; }
		}

		public class SearchConfig
		{
			public SearchConfig()
			{
			}

			public int Results
			{
				get
				{
					string s = GetConfig(Keys.KeySearchResults);
					if (s == null)
						return 0;
					return int.Parse(s);
				}
			}

			public int Sort
			{
				get
				{
					string s = GetConfig(Keys.KeySearchSort);
					if (s == null)
						return 0;
					return int.Parse(s);
				}
			}

			public string BeginTag
			{
				get { return GetConfig(Keys.KeySearchBeginTag); }
			}

			public string EndTag
			{
				get { return GetConfig(Keys.KeySearchEndTag); }
			}

			public string GetConfig(string key)
			{
				DataTable _tb = GetSearch();
				if (_tb.Columns.Contains(key))
					return (string) (_tb.Rows[0][key]);
				else
					return null;
			}
		}

		#endregion
	}

	public enum EncryptionLevel
	{
		/// <summary>
		/// Простое шифрование
		/// </summary>
		Simple = 0,
		/// <summary>
		/// С добавлением браузера
		/// </summary>
		Browser = 1,
		/// <summary>
		/// С добавление IP-адреса
		/// </summary>
		Double = 2
	}
}