using System;
using System.Web;
using System.Collections;
using System.Data;
using System.Text;
using System.IO;
using System.Web.Caching;
using System.Xml;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Содержит всю информацию, которая доступна по 
	/// данному запросу(заголовок, ключевые слова, контент и т.д.)
	/// </summary>
	public sealed class PageInfo
	{
		#region static members
		public static bool IsAdmin(string path)
		{
			return path.StartsWith("admin/");
		}
		public static string Data(string path)
		{
			return IsAdmin(path) ? "~/admin/data" : "~/data";
		}
		public static void ClearCache()
		{
			System.Web.Caching.Cache cache = HttpContext.Current.Cache;
			ArrayList arr = new ArrayList();
			IDictionaryEnumerator CacheEnum = cache.GetEnumerator();
			while (CacheEnum.MoveNext())
			{
				if(CacheEnum.Key.ToString().StartsWith("CachedPageInfo['"))
				{
					arr.Add(CacheEnum.Key.ToString());
				}
			}
			foreach(string key in arr)
			{
				cache.Remove(key);
			}
		}
		public static PageInfo Init(string path)
		{
			PageInfo pi = null;
            if (Config.Main.PageCache == 0)
            { 
                pi = new PageInfo(path);
            }
            else
            {
                System.Web.Caching.Cache cache = HttpContext.Current.Cache;
				string cacheKey = "CachedPageInfo['" + Sota.Web.SimpleSite.Path.Full + "']";
                if (cache[cacheKey] == null)
                {
                    pi = new PageInfo(path);
                    if (pi.Path.Length > 0)
                    {
                        if (pi.IsPagex)
                        {
                            cache.Insert(cacheKey
                                , pi
                                , new CacheDependency(new string[] { pi.DataFileName, HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigPagex) })
                                , DateTime.Now.AddMinutes(Config.Main.PageCache)
                                , TimeSpan.Zero);
                        }
                        else
                        {
                            cache.Insert(cacheKey
                                , pi
                                , new CacheDependency(pi.DataFileName)
                                , DateTime.Now.AddMinutes(Config.Main.PageCache)
                                , TimeSpan.Zero);
                        }
                    }
                }
				else
				{
					pi = (PageInfo)cache[cacheKey];
				}
            }
			if (pi.Path.Length == 0)
			{
				return null;
			}

            HttpContext.Current.Items[Keys.ContextPageInfo] = pi;
            return pi;
		}

		public static PageInfo Current
		{
			get { return (PageInfo) HttpContext.Current.Items[Keys.ContextPageInfo]; }
		}


		#endregion

		#region constructors

		public PageInfo(string path)
		{
			OpenPage(path);
		}

		#endregion

		#region Open Page
	
		public static bool Exists(string path)
		{
			return File.Exists(GetFullPath(path));
		}

		public static string GetFullPath(string path)
		{
			return 
				HttpContext.Current.Request.MapPath(Data(path) + 
				Keys.UrlPathDelimiter) + 
				SlashToDot(path).Replace(Keys.UrlPathDelimiter, Keys.FilePathDelimiter) + 
				Keys.ConfigExtension;
		}
		public static string DotToSlash(string path)
		{
			return path.Replace(Keys.FileNameDot, Keys.UrlPathDelimiter);
		}

		public static string SlashToDot(string path)
		{
			return path.Replace(Keys.UrlPathDelimiter, Keys.FileNameDot);
		}
		public void OpenPage(string path)
		{
			string file = GetFullPath(path);
			if (!File.Exists(file))
			{
				DataTable tbPagex = Config.GetPagex();
				if(!tbPagex.Columns.Contains("order"))
				{
					tbPagex.Columns.Add("order", typeof(int)).Expression = "LEN([value])";
				}

				DataRow[] rows = tbPagex.Select("'" + path.Replace("'","''") + "' LIKE [value]", "order DESC");
				if (rows.Length == 0)
				{ 
					//no record found
					return;
				}
				else
				{
					file = GetFullPath(rows[0]["path"].ToString());
				}

				if (!File.Exists(file))
				{
					//record found, but no file
					return;
				}
				else
				{
					this._Pagex = rows[0]["value"].ToString();
					this._FileName = rows[0]["path"].ToString();
				}
			}
			_Path = path;
			XmlTextReader xr = new XmlTextReader(file);
			string nn = "";
			string fn = "";
			while(xr.Read())
			{
				if(xr.NodeType == XmlNodeType.Element)
				{
					nn = xr.Name;
					switch (nn)
					{
						case "secure":
							this._IsSecure = true;
							break;
						case "deleted":
							this._Deleted = true;
							break;
						case "field":
							fn = xr.GetAttribute("name");
							break;
					}
				}
				else if(xr.NodeType == XmlNodeType.Text || xr.NodeType == XmlNodeType.CDATA)
				{
					switch (nn)
					{
						case "cache":
							this._Cache = int.Parse(xr.Value);
							break;
						case "lm":
							this._generateLM = (GenerateLastModified)int.Parse(xr.Value);
							break;
						case "codefile":
							this._CodeFile = xr.Value;
							break;
						case "body":
							this._Body = xr.Value;
							break;
						case "description":
							this._Description = xr.Value;
							break;
						case "head":
							this._Head = xr.Value;
							break;
						case "keywords":
							this._KeyWords = xr.Value;
							break;
						case "template":
							this._Template = xr.Value;
							break;
						case "title":
							this._OriginalTitle = this._Title = xr.Value;
							break;
						case "header":
							this._Header = xr.Value;
							break;
						case "part":
							this._SecurityPart = int.Parse(xr.Value);
							break;
						case "statuscode":
							this._StatusCode = int.Parse(xr.Value);
							break;
						case "secure":
							this._IsSecure = (int.Parse(xr.Value) == 1);
							break;
						case "deleted":
							this._Deleted = (int.Parse(xr.Value) == 1);
							break;
						case "field":
							this._arFields.Add(fn, xr.Value);
							break;
						default:
							this._arFields.Add(nn, xr.Value);
							break;
					}
				}
			}
			xr.Close();
			
			/* 
			DataSet ds = new DataSet();
			ds.ReadXml(file);
			DataRow r = ds.Tables[0].Rows[0];
			DataColumnCollection cc = ds.Tables[0].Columns;
			foreach (DataColumn c in cc)
			{
				string cn = c.ColumnName;
				switch (cn)
				{
					case "cache":
						this._Cache = Convert.ToInt32(r[c.Ordinal]);
						break;
					case "lm":
						this._generateLM = (GenerateLastModified)Convert.ToInt32(r[c.Ordinal]);
						break;
					case "codefile":
						this._CodeFile = r[c.Ordinal].ToString();
						break;
					case "body":
						this._Body = r[c.Ordinal].ToString();
						break;
					case "description":
						this._Description = r[c.Ordinal].ToString();
						break;
					case "head":
						this._Head = r[c.Ordinal].ToString();
						break;
					case "secure":
						this._IsSecure = (Convert.ToInt32(r[c.Ordinal]) == 1);
						break;
					case "deleted":
						this._Deleted = (Convert.ToInt32(r[c.Ordinal]) == 1);
						break;
					case "keywords":
						this._KeyWords = r[c.Ordinal].ToString();
						break;
					case "template":
						this._Template = r[c.Ordinal].ToString();
						break;
					case "title":
						this._Title = r[c.Ordinal].ToString();
						break;
					case "header":
						this._Header = r[c.Ordinal].ToString();
						break;
					case "part":
						this._SecurityPart = Convert.ToInt32(r[c.Ordinal]);
						break;
					case "statuscode":
						this._StatusCode = Convert.ToInt32(r[c.Ordinal]);
						break;
					default:
						this._arFields.Add(cn, r[c.Ordinal].ToString());
						break;
				}
			}*/
		}

		#endregion

		#region Save Page

		internal static string CreateName(string path)
		{
			string p = path.Substring(path.LastIndexOf(Keys.UrlPathDelimiter[0]) + 1);
			return p.Substring(0, 1).ToUpper() + p.Substring(1).ToLower();
		}

		public static void SavePage(
			string path,
			string codeFile,
			string template,
			string body,
			string description,
			string title,
			string keyWords,
			string head,
			string header,
			bool isSecure,
			bool deleted,
			int cache,
			int part,
			int statusCode,
			GenerateLastModified generateLM,
			Hashtable fields)
		{
			string pt = path.Trim(' ', '/');
			if (pt.Length == 0)
				return;
			/*StringBuilder sb = new StringBuilder("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\n<page>");
			if(!Util.IsBlank(codeFile))
			{
				XmlBuilder.AppendField(sb, "codefile", codeFile);
			}
			if(isSecure)
			{
				XmlBuilder.AppendField(sb, "secure", "1");
			}
			if(deleted)
			{
				XmlBuilder.AppendField(sb, "deleted", "1");
			}
			XmlBuilder.AppendField(sb, "template", template);
			if(cache > 0)
			{
				XmlBuilder.AppendField(sb, "cache", cache.ToString());
			}
			if(generateLM != GenerateLastModified.None)
			{
				XmlBuilder.AppendField(sb, "lm", ((int)generateLM).ToString());
			}
			if(part != -1)
			{
				XmlBuilder.AppendField(sb, "part", part.ToString());
			}
			if(!Util.IsBlank(header))
			{
				XmlBuilder.AppendField(sb, "header", header);
			}
			if(statusCode > 0)
			{
				XmlBuilder.AppendField(sb, "statuscode", statusCode.ToString());
			}
			if(!Util.IsBlank(head))
			{
				XmlBuilder.AppendField(sb, "head", head);
			}
			if(!Util.IsBlank(description))
			{
				XmlBuilder.AppendField(sb, "description", description);
			}
			if(!Util.IsBlank(keyWords))
			{
				XmlBuilder.AppendField(sb, "keywords", keyWords);
			}
			XmlBuilder.AppendField(sb, "title", title == string.Empty ? CreateName(pt) : title);
			if(!Util.IsBlank(body))
			{
				XmlBuilder.AppendField(sb, "body", body);
			}
			foreach (string key in fields.Keys)
			{
				if(!Util.IsBlank(fields[key]))
				{
					XmlBuilder.AppendField(sb, key, fields[key].ToString());
				}
			}
			sb.Append("\n</page>");

			string file = GetFullPath(pt);
			StreamWriter w = null;
			try
			{
				w = new StreamWriter(file);
				w.Write(sb.ToString());
			}
			finally
			{
				if (w != null)
					w.Close();
			}*/
			
			string file = GetFullPath(pt);
			XmlTextWriter w = new XmlTextWriter(file, Encoding.UTF8);
			w.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
			w.WriteStartElement("page");
			if(!Util.IsBlank(codeFile))
			{
				w.WriteStartElement("codefile");
				w.WriteString(codeFile);
				w.WriteEndElement();
			}
			if(isSecure)
			{
				w.WriteStartElement("secure");
				w.WriteEndElement();
			}
			if(deleted)
			{
				w.WriteStartElement("deleted");
				w.WriteEndElement();
			}
			w.WriteStartElement("template");
			w.WriteString(template);
			w.WriteEndElement();
			if(cache > 0)
			{
				w.WriteStartElement("cache");
				w.WriteString(cache.ToString());
				w.WriteEndElement();
			}
			if(generateLM != GenerateLastModified.None)
			{
				w.WriteStartElement("lm");
				w.WriteString(((int)generateLM).ToString());
				w.WriteEndElement();
			}
			if(part != -1)
			{
				w.WriteStartElement("part");
				w.WriteString(part.ToString());
				w.WriteEndElement();
			}
			if(!Util.IsBlank(header))
			{
				w.WriteStartElement("header");
				w.WriteString(header);
				w.WriteEndElement();
			}
			if(statusCode > 0)
			{
				w.WriteStartElement("statuscode");
				w.WriteString(statusCode.ToString());
				w.WriteEndElement();
			}
			if(!Util.IsBlank(head))
			{
				w.WriteStartElement("head");
				w.WriteString(head);
				w.WriteEndElement();
			}
			if(!Util.IsBlank(description))
			{
				w.WriteStartElement("description");
				w.WriteString(description);
				w.WriteEndElement();
			}
			if(!Util.IsBlank(keyWords))
			{
				w.WriteStartElement("keywords");
				w.WriteString(keyWords);
				w.WriteEndElement();
			}
			w.WriteStartElement("title");
			w.WriteString(title == string.Empty ? CreateName(pt) : title);
			w.WriteEndElement();
			if(!Util.IsBlank(body))
			{
				w.WriteStartElement("body");
				w.WriteCData(body);
				w.WriteEndElement();
			}
			foreach (string key in fields.Keys)
			{
				if(!Util.IsBlank(fields[key]))
				{
					w.WriteStartElement("field");
					w.WriteAttributeString("name", key);
					w.WriteCData(fields[key].ToString());
					w.WriteEndElement();
				}
			}
			w.WriteEndElement();
			w.Close();
		}

		#endregion

		#region Delete & Restore Page
		static string GetTrashPath()
		{ 
			return Config.Main.Data + Keys.UrlPathDelimiter + "trash";
		}

		public static bool TrashIsEmpty()
		{
			return Directory.GetFiles(HttpContext.Current.Request.MapPath(GetTrashPath())).Length == 0;
		}

		public static string GetFullTrashPath(string deleted_name)
		{
			return HttpContext.Current.Request.MapPath(GetTrashPath() + Keys.UrlPathDelimiter + deleted_name + Keys.ConfigExtension);
		}

		public static void Delete(string path)
		{
			string trash_dir = HttpContext.Current.Request.MapPath(GetTrashPath());
			if(!Directory.Exists(trash_dir))
			{
				Directory.CreateDirectory(trash_dir);
			}
			string file = GetFullPath(path);
			string deleted_file = GetFullTrashPath(GetDeletedFileName(SlashToDot(path), Config.Main.Now()));
			File.Move(file, deleted_file);

			DataTable tbPagex = Config.GetPagex();
			DataRow[] rows = tbPagex.Select("path='" + path.Replace("'","''") + "'");
			for(int i=0;i<rows.Length;i++)
			{
				tbPagex.Rows.Remove(rows[i]);
			}
			Config.WriteConfigTable("pagex.config",tbPagex,"pagexes");

		}

		public static void FullDelete(string deleted_name)
		{
			string deleted_file = GetFullTrashPath(deleted_name);
			File.Delete(deleted_file);
		}

		public static void Restore(string deleted_name)
		{
			PageInfo.DeletedFile df = PageInfo.GetFileNameFromTrash(deleted_name);
			string file = GetFullPath(df.Name);
			if (!File.Exists(file))
			{
				string deleted_file = GetFullTrashPath(deleted_name);
				File.Move(deleted_file, file);
			}
		}

		public static string GetDeletedFileName(string file, DateTime date)
		{
			return string.Format(
				"{0}.{1}.{2}.{3}.{4}.{5}_{6}",
				date.Year,
				date.Month,
				date.Day,
				date.Hour,
				date.Minute,
				date.Second,
				file);
		}

		public static DeletedFile GetFileNameFromTrash(string deleted_file)
		{
			DeletedFile dp = new DeletedFile();
			string[] tn = deleted_file.Split('_');
			string[] dt = tn[0].Split('.');
			dp.Deleted = new DateTime(
				Int32.Parse(dt[0]),
				Int32.Parse(dt[1]),
				Int32.Parse(dt[2]),
				Int32.Parse(dt[3]),
				Int32.Parse(dt[4]),
				Int32.Parse(dt[5]));
			dp.Name = DotToSlash(tn[1]);
			return dp;
		}

		public struct DeletedFile
		{
			public DateTime Deleted;
			public string Name;
		}

		#endregion

		#region fields & properties

		public string DataFileName
		{
			get { return GetFullPath(FileName); }
		}

		private Hashtable _arFields = new Hashtable();

		public Hashtable Fields
		{
			get { return _arFields; }
		}

		private string _Path = string.Empty;

		public string Path
		{
			get { return _Path; }
		}
		
		private int _StatusCode = 0;

		public int StatusCode
		{
			get { return _StatusCode; }
		}

		private string _Pagex = string.Empty;

		public string Pagex
		{
			get { return _Pagex; }
		}

		public string PagexValue
		{
			get
			{
				if (_Pagex.Length > 0 && _Path != _FileName)
				{
					if (_Pagex.EndsWith("*"))
					{
						return _Path.Substring(_Pagex.Length - 1);
					}
					if (_Pagex.StartsWith("*"))
					{
						return _Path.Substring(0, _Path.Length - _Pagex.Length + 1);
					}
				}
				return string.Empty;
			}
		}
		public bool IsPagex
		{
			get{return _Pagex.Length > 0;}
		}
		private string _FileName = string.Empty;

		public string FileName
		{
			get { return _FileName == string.Empty ? _Path : _FileName; }
		}

		public string Url
		{
			get
			{
				return Sota.Web.SimpleSite.Path.ARoot + Path + Config.Main.Extension;
			}
		}

		public string VUrl
		{
			get { return Sota.Web.SimpleSite.Path.VRoot + Path + Config.Main.Extension; }
		}

		private bool _IsSecure = false;

		public bool IsSecure
		{
			get { return _IsSecure; }
		}

		public string Protocol
		{
			get { return Util.GetProtocol(_IsSecure); }
		}

		public string CurrentUrl
		{
			get { return Sota.Web.SimpleSite.Path.ARoot + Path + Config.Main.Extension; }
		}

		private bool _Deleted = false;

		public bool Deleted
		{
			get { return _Deleted; }
		}

		private string _Header = string.Empty;

		public string Header
		{
			get { return _Header; }
		}
		
		private string _CodeFile = string.Empty;

		public string CodeFile
		{
			get { return _CodeFile; }
		}

		private string _Template = string.Empty;

		public string Template
		{
			get { return _Template; }
		}

		private string _Head = string.Empty;

		public string Head
		{
			get { return _Head; }
			set { _Head = value; }
		}

		private string _Title = string.Empty;

		public string Title
		{
			get { return _Title; }
			set { _Title = value; }
		}

		private string _OriginalTitle = string.Empty;

		public string OriginalTitle
		{
			get { return _OriginalTitle; }
		}

		public void AppendTitle(string text)
		{
			_Title += Config.Main.TitleSeparator + text;
		}

		private string _Description = string.Empty;

		public string Description
		{
			get { return _Description; }
			set { _Description = value; }
		}

		private string _KeyWords = string.Empty;

		public string KeyWords
		{
			get { return _KeyWords; }
			set { _KeyWords = value; }
		}

		private string _Body = string.Empty;

		public string Body
		{
			get { return _Body; }
			set { _Body = value; }
		}

		private int _Cache = 0;

		public int Cache
		{
			get { return _Cache; }
		}

		private int _SecurityPart = -1;

		public int SecurityPart
		{
			get { return _SecurityPart; }
		}

		private GenerateLastModified _generateLM = GenerateLastModified.None;

		public GenerateLastModified GenerateLastModified
		{
			get { return _generateLM; }
		}

		#endregion

	}
	public enum GenerateLastModified
	{
		Standard = 0,
		Now = 1,
		FromFiles = 2,
		None = 3
		
	}
}