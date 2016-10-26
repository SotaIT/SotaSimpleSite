using System;
using System.Web;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Содержит текущий путь.
	/// </summary>
	public sealed class Path
	{
		private Path()
		{
		}
		private string _path = "";
		private string _domain = "";
		private string _page = "";
		private string _site = "";
		private string _add = "";

		private static Path FromContext
		{
			get
			{
				if(HttpContext.Current.Items[Keys.ContextPathFull]==null)
				{
					HttpContext.Current.Items[Keys.ContextPathFull] = new Path();
				}
				return (Path)HttpContext.Current.Items[Keys.ContextPathFull];
			}
		}
		public static void InitFull(string path)
		{
			FromContext._path = path;
		}

		public static void InitDomain(string domain)
		{
			FromContext._domain = domain;
		}

		public static void InitSite(string site)
		{
			FromContext._site = site;
		}

		public static void InitPage(string page)
		{
			FromContext._page = page;
		}

		public static void InitAddPath(string addPath)
		{
			FromContext._add = addPath;
		}

		/// <summary>
		/// Полный путь текущего запроса
		/// </summary>
		public static string Full
		{
			get { return FromContext._path; }
		}

		/// <summary>
		/// Полный путь текущего запроса, без строки запроса
		/// </summary>
		public static string PagePath
		{
			get { return FromContext._path.Split('?')[0]; }
		}

		/// <summary>
		/// Добавочный путь текущего домена
		/// </summary>
		public static string AddPath
		{
			get { return FromContext._add; }
		}

		/// <summary>
		/// Домен текущего запроса, без протокола
		/// </summary>
		public static string Domain
		{
			get { return FromContext._domain; }
		}

		public static string GetSimpleDomain()
		{
			
			string d = Domain;
			if(d.StartsWith("www.") && d.IndexOf('.', 5) > -1)
			{
				return d.Remove(0,4);
			}
			return d;
		}


		/// <summary>
		/// Сайт (код приложения) текущего домена
		/// </summary>
		public static string Site
		{
			get { return FromContext._site; }
		}

		/// <summary>
		/// Виртуальный путь текущей страницы
		/// </summary>
		public static string Page
		{
			get { return FromContext._page; }
		}
		

		/// <summary>
		/// Строка запроса 
		/// </summary>
		public static string QueryString
		{
			get { return HttpContext.Current.Request.QueryString.ToString(); }
		}

		/// <summary>
		/// Относительный путь к корню текущего приложения
		/// </summary>
		public static string VRoot
		{
			get
			{
				string r = HttpContext.Current.Request.ApplicationPath;
				return r == Keys.UrlPathDelimiter ? r : r + Keys.UrlPathDelimiter;
			}
		}
		
		/// <summary>
		/// Абсолютный путь к корню текущего приложения
		/// </summary>
		public static string ARoot
		{
			get
			{
				Uri url = HttpContext.Current.Request.Url;
				return url.Scheme+"://"+url.Host+(url.Port==80 ? "" : Keys.UrlPortDelimiter + url.Port.ToString())+VRoot;
			}
		}

		public static string GetDefaultSiteName()
		{
			return Util.CapitalizeString(Domain.Split(new char[]{'/','.'})[0]);
		}
	}
}