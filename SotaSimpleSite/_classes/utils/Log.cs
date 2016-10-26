using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using Sota.Data.Simple;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Работает с логами
	/// </summary>
	public sealed class Log
	{
		private Log()
		{
		}

		#region utils


		/// <summary>
		/// Коннектор для подключения к БД
		/// </summary>
		public static Sota.Data.Simple.Connector Connector
		{
			get
			{
				Sota.Data.Simple.Connector connector = (Sota.Data.Simple.Connector)HttpContext.Current.Cache[Keys.KeyLogConnector];
				if (connector == null)
				{
					DataTable tb = Config.GetConfigTableNoCache(Keys.ConfigLog, "common");
					connector = new Sota.Data.Simple.Connector(tb.Rows[0]["connectionstring"].ToString().Trim().Length == 0 ? Config.Main.ConnectionString : tb.Rows[0]["connectionstring"].ToString(), tb.Rows[0]["provider"].ToString());
					HttpContext.Current.Cache.Insert(Keys.KeyLogConnector, connector, new CacheDependency(
						new string[]{
										HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigLog),
										HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigMain)
									}));
				}
				return connector;
			}
		}

		/// <summary>
		/// Получает текст выражения SQL
		/// </summary>
		/// <param name="key">идентификатор выражения SQL</param>
		/// <returns>Текст выражения SQL</returns>
		public static string GetSql(string key)
		{
			DataTable tb = Config.GetConfigTable("log.config", "sql");
			return tb.Select("key='" + key + "'")[0]["value"].ToString();
		}

		/// <summary>
		/// Получает список параметров указанного выражения
		/// </summary>
		/// <param name="key">идентификатор выражения SQL</param>
		/// <returns>Массив DbParameter[]</returns>
		public static DbParameter[] GetSqlParameters(string key)
		{
			DbParameter[] ar = null;
			DataTable tb = Config.GetConfigTable("log.config", "sql_parameter");
			DataRow[] arr = tb.Select("sqlkey='" + key + "'", "[order] ASC");
			if (arr.Length > 0)
			{
				int n = arr.Length;
				ar = new DbParameter[n];
				for (int i = 0; i < n; i++)
				{
					ar[i] = ParameterFromRow(arr[i]);
				}
			}
			return ar;
		}

		/// <summary>
		/// Получает имя параметра для использования в выражении
		/// </summary>
		/// <param name="key">идентификатор параметра</param>
		/// <param name="sqlKey">идентификатор выражения SQl</param>
		/// <returns>Имя указанного параметра</returns>
		public static string GetParameterName(string key, string sqlKey)
		{
			DataTable tb = Config.GetConfigTable("log.config", "sql_parameter");
			return tb.Select("(key='" + key + "') AND (sqlkey='" + sqlKey + "')")[0]["parametername"].ToString();
		}

		/// <summary>
		/// Создает экземпляр Sota.Data.Simple.DbParameter на основе информации из DataRow
		/// </summary>
		/// <param name="row">Строка, содержащая информацию о параметре</param>
		/// <returns>Экземпляр DbParameter</returns>
		public static DbParameter ParameterFromRow(DataRow row)
		{
			return new DbParameter(row["parametername"].ToString(), DbParameter.ParseDbType(row["dbtype"].ToString()));
		}

		#endregion

		public static int Write(string type, Hashtable par)
		{
			StringBuilder sb = new StringBuilder();
			if (par != null)
			{
				foreach (string key in par.Keys)
				{
					if(!Util.IsBlank(par[key]))
					{
						sb.AppendFormat("<b>{0}</b>: {1}<br />", key, par[key]);
					}
				}
			}
			IDbCommand cmd = Connector.CreateCommand(GetSql("writelog"), GetSqlParameters("writelog"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("datetime", "writelog")]).Value = Config.Main.Now();
			((IDbDataParameter) cmd.Parameters[GetParameterName("type", "writelog")]).Value = type;
			((IDbDataParameter) cmd.Parameters[GetParameterName("params", "writelog")]).Value = sb.ToString();
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "writelog")]).Value = Path.Site;
			try
			{
				return Connector.ExecuteNonQuery(cmd);
			}
			catch
			{
				HttpContext cntx = HttpContext.Current;
				string file = cntx.Request.MapPath("~/error.log.config");
				StreamWriter w = null;
				try
				{
					w = new StreamWriter(file, true, Encoding.UTF8);
					w.WriteLine(Config.Main.Now());
					w.WriteLine(type);
					w.WriteLine(sb.ToString());
					w.WriteLine("");
				}
				finally
				{
					if (w != null)
						w.Close();
				}
			}
			return -1;
		}
		public static int Delete(int id)
		{
			IDbCommand cmd = Connector.CreateCommand(GetSql("dellog"), GetSqlParameters("dellog"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("id", "dellog")]).Value = id;
			return Connector.ExecuteNonQuery(cmd);
		}

		public static int GetCount(string type)
		{
			IDbCommand cmd = Connector.CreateCommand(GetSql("getcount"), GetSqlParameters("getcount"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("type", "getcount")]).Value = type;
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "getcount")]).Value = Path.Site;
			return Convert.ToInt32(Connector.ExecuteScalar(cmd));
		}

		public static int GetCountAt(string type, DateTime day)
		{
			return GetCountByPeriod(type, day.Date, day.Date.AddDays(1).AddSeconds(-1));
		}
		public static int GetCountByPeriod(string type, DateTime begin, DateTime end)
		{
			IDbCommand cmd = Connector.CreateCommand(GetSql("getcountat"), GetSqlParameters("getcountat"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("type", "getcountat")]).Value = type;
			((IDbDataParameter) cmd.Parameters[GetParameterName("after", "getcountat")]).Value = end.AddSeconds(1);
			((IDbDataParameter) cmd.Parameters[GetParameterName("before", "getcountat")]).Value = begin.AddSeconds(-1);
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "getcountat")]).Value = Path.Site;
			return Convert.ToInt32(Connector.ExecuteScalar(cmd));
		}

		public static int GetCountToday(string type)
		{
			return GetCountAt(type, Config.Main.Now());
		}

		public static DataTable GetAll()
		{
			DataRow rLog = Config.GetConfigTable("log.config", "table").Rows[0];
			DataTable tb = new DataTable();
			tb.Columns.Add("id", typeof (int));
			tb.Columns.Add("datetime", typeof (DateTime));
			tb.Columns.Add("type", typeof (string));
			tb.Columns.Add("params", typeof (string));
			tb.Columns.Add("site", typeof (string));
			IDataReader r = null;
			IDbCommand cmd = Connector.CreateCommand(GetSql("getall"), GetSqlParameters("getall"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "getall")]).Value = Path.Site;
			try
			{
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				while (r.Read())
				{
					tb.Rows.Add(new object[] {r[rLog["id"].ToString()], r[rLog["datetime"].ToString()], r[rLog["type"].ToString()], r[rLog["params"].ToString()], r[rLog["site"].ToString()]});
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
			return tb;
		}

		public static DataTable GetByParams(string type, DateTime after, DateTime before, string param)
		{
			DataRow rLog = Config.GetConfigTable("log.config", "table").Rows[0];
			DataTable tb = new DataTable();
			tb.Columns.Add("id", typeof (int));
			tb.Columns.Add("datetime", typeof (DateTime));
			tb.Columns.Add("type", typeof (string));
			tb.Columns.Add("params", typeof (string));
			tb.Columns.Add("site", typeof (string));
			IDataReader r = null;
			IDbCommand cmd = Connector.CreateCommand(GetSql("getbyparams"), GetSqlParameters("getbyparams"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("type", "getbyparams")]).Value = type==null ? DBNull.Value : (object)type;
			((IDbDataParameter) cmd.Parameters[GetParameterName("after", "getbyparams")]).Value =  after==DateTime.MaxValue ? DBNull.Value : (object)after;
			((IDbDataParameter) cmd.Parameters[GetParameterName("before", "getbyparams")]).Value =  before==DateTime.MinValue ? DBNull.Value : (object)before;
			((IDbDataParameter) cmd.Parameters[GetParameterName("params", "getbyparams")]).Value =  param==null ? DBNull.Value : (object)param;
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "getbyparams")]).Value = Path.Site;
			try
			{
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				while (r.Read())
				{
					tb.Rows.Add(new object[] {r[rLog["id"].ToString()], r[rLog["datetime"].ToString()], r[rLog["type"].ToString()], r[rLog["params"].ToString()], r[rLog["site"].ToString()]});
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
			return tb;
		}


		public static ArrayList GetTypes()
		{
			ArrayList arr = new ArrayList();
			IDataReader r = null;
			IDbCommand cmd = Connector.CreateCommand(GetSql("gettypes"), GetSqlParameters("gettypes"));
			((IDbDataParameter) cmd.Parameters[GetParameterName("site", "gettypes")]).Value = Path.Site;
			try
			{
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				while (r.Read())
				{
					arr.Add(r[0].ToString());
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
			return arr;
		}
		public static void Request()
		{
			Hashtable h = Util.GetClientInfo();
			Request(h);
		}
		public static void Request(Hashtable h)
		{
			HttpContext context = HttpContext.Current;
			if (Config.GetConfigTable("se.config","se").Select("'" + h["browser"] + "' LIKE browser OR '" + h["ip"].ToString().Trim() + "' LIKE ip").Length > 0)
			{
				Log.Write("SearchEngine", h);
			}  
			else if(context.Request.Browser.Cookies
				&& context.Request.Cookies[Keys.CookieCounted] == null
				&& context.Session[Keys.CookieCounted] == null
				&& IsNewUser(h))
			{
				context.Session[Keys.CookieCounted] = true;
				Util.SetCookie(
					Keys.CookieCounted,
					Config.Main.Now().ToLongTimeString(),
					Config.Main.MyTomorrow(),
					context);
				Log.Write("Unique", h);
				RememberUser(h);
			}
			else
			{
				Log.Write("Hit", h);
			}
		}
		static void RememberUser(Hashtable h)
		{
			Cache cache = HttpContext.Current.Cache;
			string key = string.Format("[{0}]-[{1}]-[{2}]", Path.Site, h["ip"], h["browser"]);
			cache.Insert(key,1,null,DateTime.MaxValue,new TimeSpan(0,5,0));
		}
		static bool IsNewUser(Hashtable h)
		{
			Cache cache = HttpContext.Current.Cache;
			string key = string.Format("[{0}]-[{1}]-[{2}]", Path.Site, h["ip"], h["browser"]);
			return cache[key]==null;
		}
	}
}