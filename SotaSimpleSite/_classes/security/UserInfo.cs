using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.SessionState;

namespace Sota.Web.SimpleSite.Security
{
	/// <summary>
	/// Содержит информацию о пользователь.
	/// </summary>
	public sealed class UserInfo
	{
		#region static members
//		private static void InsertToCache(HttpContext cntx, UserInfo ui)
//		{
//			cntx.Cache.Insert(Keys.SessionUserInfo + "[" + ui.UserId + "]", ui);
//		}
//		private static UserInfo GetFromCache(HttpContext cntx, int id)
//		{
//			return (UserInfo)cntx.Cache[Keys.SessionUserInfo + "[" + id + "]"];
//		}

		public static void Init()
		{
			Init(HttpContext.Current);
		}
		public static void Init(HttpContext cntx)
		{
			int id = GetUserIdFromCookie();
			if(id == -1)
			{
				cntx.Session[Keys.SessionUserInfo] = new UserInfo();
			}
			else
			{
				LoginNew(id);
			}
		}
		
		public static UserInfo Current
		{
			get
			{
				HttpContext cntx = HttpContext.Current;
				HttpSessionState session = cntx.Session;
				if (session == null)
					return null;
				if (session[Keys.SessionUserInfo] == null)//
				{
					Init(cntx);
				}
//				else
//				{
//					string ck = Keys.SessionUserInfo + "_cookieChecked";
//					if(cntx.Items[ck] == null)
//					{
//						UserInfo u = (UserInfo) session[Keys.SessionUserInfo];//
//						int id = GetUserIdFromCookie();
//						if(id != -1 
//							&& id != u.UserId)
//						{
//							SetUserIdToCookie(u, false);
//						}
//						cntx.Items[ck] = 1;
//						return u;
//					}
//				}
				UserInfo ui = (UserInfo) session[Keys.SessionUserInfo];//
				return ui;
			}
		}

		public static void LogOut()
		{
			RemoveUserIdFromCookie();
			HttpContext.Current.Session[Keys.SessionUserInfo] = new UserInfo();//
		}

		public static bool LoginNew(string login, string password)
		{
			return LoginNew(login, password, false);
		}
		public static bool LoginNew(string login, string password, bool persist)
		{
			UserInfo ui = new UserInfo(login, password);
			if(LoginNew(ui))
			{
				SetUserIdToCookie(ui, persist);
				return true;
			}
			return false;
		}

        public static bool LoginNew(string token)
        {
            return LoginNew(token, false);
        }

        public static bool LoginNew(string token, bool persist)
        {
            try
            {
                int userId = DecryptUserIdToken(token);
                if (userId != -1)
                {
                   UserInfo ui = new UserInfo(userId);
                   if (ui.IsAuthorized)
                    {
                        SetUserIdToCookie(ui, persist);
                        return true;
                    }
                }
            }
            catch
            { }

            return false;
        }

        public static bool LoginNew(int userId)
        {
            return LoginNew(new UserInfo(userId), false);
        }
        public static bool LoginNew(int userId, bool persist)
        {
            return LoginNew(new UserInfo(userId), persist);
        }

        private static bool LoginNew(UserInfo ui, bool persist)
        {
            HttpContext.Current.Session[Keys.SessionUserInfo] = ui;//
            if (ui.IsAuthorized)
            {
                SetUserIdToCookie(ui, persist);
                return true;
            }
            return false;
        }
		private static bool LoginNew(UserInfo ui)
		{
            return LoginNew(ui, false);
		}
		

		private static string cookieName = "Sota.Cookie";
		private const string cookieValueSeparator = "#";
		
		private static void SetResponseCookie(HttpContext cntx, bool persist, bool delete, string value)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(cookieName);
			sb.Append("=");
			sb.Append(value);
			if(persist || delete)
			{
				DateTime expires = persist ? DateTime.Now.AddYears(10) : new DateTime(2000,1,1);
				sb.Append("; expires=");
				sb.Append(Util.FormatHttpCookieDateTime(expires));

			}
			sb.Append("; path=/; domain=.");
			sb.Append(Path.GetSimpleDomain());
			cntx.Response.AddHeader("Set-Cookie", sb.ToString());
		}

		public static void SetUserIdToCookie(UserInfo ui, bool persist)
		{
			HttpContext cntx = HttpContext.Current;
			EncryptionLevel level = ui.IsAdmin || ui.IsManager ? EncryptionLevel.Double : Config.Main.EncryptionLevel;
			string hash = GetCookieHash(level, ui.UserId);
			SetResponseCookie(
				cntx,
				persist, 
				false,
				EncryptData(((int)level).ToString() + cookieValueSeparator + ui.UserId.ToString() + cookieValueSeparator + hash));
		}

        public static string GetUserTokenFromCookie()
        {
            HttpContext cntx = HttpContext.Current;
            if (cntx.Request.Cookies[cookieName] != null)
            {
                return cntx.Request.Cookies[cookieName].Value;
            }
            return null;
        }

        public static int GetUserIdFromCookie()
        {
            try
            {
                string token = GetUserTokenFromCookie();
                if (token != null)
                {
                    return DecryptUserIdToken(token);
                }
            }
            catch
            {
                RemoveUserIdFromCookie();
            }

            return -1;
        }

		public static int DecryptUserIdToken(string token)
		{
			string v = DecryptData(token).Trim();				
			int i1 = v.IndexOf(cookieValueSeparator);
			int i2 = v.IndexOf(cookieValueSeparator, i1+1);

			EncryptionLevel level = (EncryptionLevel)int.Parse(v.Substring(0, i1));	
				
			int userID = int.Parse(v.Substring(i1 + 1, i2 - i1 - 1));

			string hash = v.Substring(i2 + 1).Trim();	
			string hash1 = GetCookieHash(level, userID);

			if(string.Compare(hash,hash1,false,CultureInfo.InvariantCulture)==0)
			{
				return userID;
			}

			return -1;
		}

		private static void RemoveUserIdFromCookie()
		{
			SetResponseCookie(
				HttpContext.Current,
				false,
				true, 
				EncryptData("0" + cookieValueSeparator + "-1" + cookieValueSeparator));
		}
		private static string GetCookieHash(EncryptionLevel level, int userID)
		{
			HttpContext cntx = HttpContext.Current;
			string add = "";
			switch(level)
			{
				case EncryptionLevel.Browser:
					add = SecurityManager.EncryptPassword(cntx.Request.UserAgent);
					break;
				case EncryptionLevel.Double:
					add = SecurityManager.EncryptPassword(cntx.Request.UserAgent + cntx.Request.UserHostAddress);
					break;
			}
			return SecurityManager.EncryptPassword(((int)level).ToString() + userID.ToString() + add);
		}
		private static byte[] encKey
		{
			get
			{
				return Config.Main.EncryptionKey;
			}
		}
		private static byte[] encIV 
		{
			get
			{
				return Config.Main.EncryptionVI;
			}
		}
		public static string EncryptData(string original)
		{
			RijndaelManaged rm = new RijndaelManaged();
			ICryptoTransform encryptor = rm.CreateEncryptor(encKey, encIV);
			MemoryStream msEncrypt = new MemoryStream();
			CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);

			byte[] toEncrypt = System.Text.Encoding.UTF8.GetBytes(original);

			csEncrypt.Write(toEncrypt, 0, toEncrypt.Length);
			csEncrypt.FlushFinalBlock();

			byte[] encrypted = msEncrypt.ToArray();
			return Convert.ToBase64String(encrypted);
		}
		public static string DecryptData(string encrypted)
		{
			try
			{
				RijndaelManaged rm = new RijndaelManaged();
				ICryptoTransform decryptor = rm.CreateDecryptor(encKey, encIV);

				MemoryStream msDecrypt = new MemoryStream(Convert.FromBase64String(encrypted));
				CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);

				byte[] fromEncrypt = new byte[encrypted.Length];

				csDecrypt.Read(fromEncrypt, 0, fromEncrypt.Length);

				return System.Text.Encoding.UTF8.GetString(fromEncrypt);
			}
			catch
			{
				//Config.ReportError(ex);
				return "";
			}
		}

		#region on-line
		
//		private static void UserActed(int id, string login)
//		{
//			HttpContext cntx	= HttpContext.Current;
//			string sid			= HttpUtility.UrlEncode(cntx.Session.SessionID);
//			string title		= PageInfo.Current == null ? Path.Full : PageInfo.Current.Title;
//			string ip			= HttpUtility.UrlEncode(Util.GetClientIP(cntx.Request));
//			DataTable tb		= WhoIsOnline();
//			DataRow[] rows		= tb.Select("sid='" + sid + "'");
//			if (rows.Length > 0)
//			{
//				rows[0]["datetime"]		= Config.Main.Now();
//				rows[0]["uid"]			= id;
//				rows[0]["login"]		= login;
//				rows[0]["url"]			= Path.Full;
//				rows[0]["url_title"]	= title;
//				rows[0]["ip"]			= ip;
//			}
//			else
//			{
//				tb.Rows.Add(new object[] {Config.Main.Now(), sid, id, login, Path.Full, title, ip});
//			}
//			cntx.Application.Lock();
//			cntx.Application[Keys.CachedOnlineUsersListObject] = tb;
//			cntx.Application.UnLock();
//		}
//		private static void UserActedLogout()
//		{
//			//			UserActed(-1, SecurityManager.GetInternalUserLogin(-1));
//			HttpContext cntx	= HttpContext.Current;
//			DataTable tb		= WhoIsOnline();
//			string title		= PageInfo.Current == null ? Path.Full : PageInfo.Current.Title;
//			string ip			= HttpUtility.UrlEncode(Util.GetClientIP(cntx.Request));
//			DataRow[] rows		= tb.Select("uid=" + Current.UserId + "");
//			for(int i=rows.Length-1; i > -1; i--)
//			{
//				rows[i]["datetime"]		= Config.Main.Now();
//				rows[i]["uid"]			= -1;
//				rows[i]["login"]		= SecurityManager.GetInternalUserLogin(-1);
//				rows[i]["url"]			= Path.Full;
//				rows[i]["url_title"]	= title;
//				rows[i]["ip"]			= ip;
//			}
//			cntx.Application.Lock();
//			cntx.Application[Keys.CachedOnlineUsersListObject] = tb;
//			cntx.Application.UnLock();
//
//		}
//		private static DateTime _lastUpdate = DateTime.Now;
//		public static DataTable WhoIsOnline()
//		{
//			HttpApplicationState app = HttpContext.Current.Application;
//			DataTable tb = null;
//			if (app[Keys.CachedOnlineUsersListObject] != null)
//			{
//				tb = ((DataTable) app[Keys.CachedOnlineUsersListObject]).Copy();
//				DateTime now = Config.Main.Now();
//				if(_lastUpdate.Minute!=now.Minute)
//				{
//					DataRow[] rows = tb.Select("datetime < #" + now.AddMinutes((-1)*Config.Main.OnlineTimeOut).ToString(CultureInfo.InvariantCulture) + "#");
//					if(rows.Length>0)
//					{
//						DataTable temp = tb.Copy();
//						tb.Rows.Clear();
//						rows = temp.Select("datetime > #" + now.AddMinutes(((-1)*Config.Main.OnlineTimeOut)-1).ToString(CultureInfo.InvariantCulture) + "#");
//						for (int i = 0; i < rows.Length; i++)
//						{
//							tb.ImportRow(rows[i]);
//						}
//					}
//					app.Lock();
//					app[Keys.CachedOnlineUsersListObject] = tb.Copy();
//					app.UnLock();
//				}
//			}
//			else
//			{
//				_lastUpdate = Config.Main.Now();
//				tb = new DataTable();
//				tb.Columns.Add("datetime", typeof (DateTime));
//				tb.Columns.Add("sid", typeof (string));
//				tb.Columns.Add("uid", typeof (int));
//				tb.Columns.Add("login", typeof (string));
//				tb.Columns.Add("url", typeof (string));
//				tb.Columns.Add("url_title", typeof (string));
//				tb.Columns.Add("ip", typeof (string));
//				app.Lock();
//				app[Keys.CachedOnlineUsersListObject] = tb.Copy();
//				app.UnLock();
//			}
//			return tb;
//		}
//		public static bool IsOnline(int userid)
//		{
//			return IsOnline(userid, "");
//		}
//		public static bool IsOnline(string login)
//		{
//			return IsOnline(0, login);
//		}
//		public static bool IsOnline(int userid, string login)
//		{
//			DataTable tb		= WhoIsOnline();
//			DataRow[] rows		= tb.Select("(uid=" + userid + ") OR (login='" + login + "')");
//			return rows.Length > 0;
//		}
		
		static Regex regexUser = new Regex("(?<login>.+?) \\[(?<uid>\\-?\\d+?)\\]", 
			RegexOptions.Compiled 
			| RegexOptions.IgnoreCase);
		static Regex regexParams = new Regex("<b>(?<name>.+?)</b>: (?<value>.+?)<br />", 
			RegexOptions.Compiled 
			| RegexOptions.IgnoreCase);
		public static DataTable WhoIsOnline()
		{
			Cache cache = HttpContext.Current.Cache;
			DataTable tb = null;
			if (cache[Keys.CachedOnlineUsersListObject] == null)
			{
				tb = new DataTable();
				tb.Columns.Add("datetime", typeof (DateTime));
				tb.Columns.Add("sid", typeof (string));
				tb.Columns.Add("uid", typeof (int)).DefaultValue = -1;
				tb.Columns.Add("login", typeof (string));
				tb.Columns.Add("url", typeof (string));
//				tb.Columns.Add("url_title", typeof (string));
				tb.Columns.Add("ip", typeof (string));
				
				if(Config.Main.OnlineTimeOut>0)
				{
					DataTable dtLogs = Log.GetByParams(null, DateTime.MaxValue, DateTime.Now.AddMinutes(-1 * Config.Main.OnlineTimeOut), null);
					DataRow[] dtr = dtLogs.Select("","datetime DESC");
					foreach(DataRow r in dtr)
					{
						DataRow rn = tb.NewRow();
						rn["datetime"] = r["datetime"];
						MatchCollection mc = regexParams.Matches(r["params"].ToString());
						foreach(Match m in mc)
						{
							if(m.Groups["name"] != null && m.Groups["value"] != null)
							{
								switch(m.Groups["name"].Value)
								{
									case "user":
										Match mu = regexUser.Match(m.Groups["value"].Value);
										if(mu.Groups["login"] != null && mu.Groups["uid"] != null)
										{
											rn["login"] = mu.Groups["login"].Value;
											rn["uid"] = mu.Groups["uid"].Value;
										}
										break;
									case "session":
										rn["sid"] = m.Groups["value"].Value;
										break;
									case "ip":
										rn["ip"] = HttpUtility.UrlEncode(m.Groups["value"].Value);
										break;
									case "path":
										rn["url"] = m.Groups["value"].Value;
										break;
								}
							}
						}
						if((int)rn["uid"] != -1)
						{
							if(tb.Select("uid=" + rn["uid"]).Length == 0)
							{
								tb.Rows.Add(rn);
							}
						}
					}
					cache.Insert(Keys.CachedOnlineUsersListObject, tb,null, DateTime.Now.AddMinutes(1), TimeSpan.Zero);
				}
			}
			else
			{
				tb = (DataTable) cache[Keys.CachedOnlineUsersListObject];
			}
			return tb;
		}

		public static bool IsOnline(int userid)
		{
			return IsOnline(userid, "");
		}
		public static bool IsOnline(string login)
		{
			return IsOnline(0, login);
		}
		public static bool IsOnline(int userid, string login)
		{
			DataTable tb		= WhoIsOnline();
			DataRow[] rows		= tb.Select("(uid=" + userid + ") OR (login='" + login + "')");
			return rows.Length > 0;
		}

		#endregion

		#endregion

		#region constructors

		private UserInfo()
		{
		}

		public UserInfo(int userId)
		{
			this.Select(userId);
		}

		public UserInfo(string login, string password)
		{
			this.Login(login, password);
		}

		#endregion

		#region SELECT methods
	
		public bool Login(string login, string password)
		{
			return ReadRow(SecurityManager.LoginUser(login, password));
		}

		public bool Select(int userId)
		{
			return ReadRow(SecurityManager.GetUser(userId));
		}

		private bool ReadRow(DataRow row)
		{
			if (row != null && (bool)row["enabled"])
			{
				this._UserId = Convert.ToInt32(row["userid"]);
				this._LoginName = row["login"].ToString();
				this._Email = row["email"].ToString();
				return true;
			}
			return false;
		}

		#endregion

		#region fields & properties

		private Hashtable _cache = new Hashtable();

		private Hashtable _Fields = new Hashtable();

		/// <summary>
		/// Поля для хранения сессионных переменных
		/// </summary>
		public Hashtable Fields
		{
			get { return _Fields; }
		}
		/// <summary>
		/// Поля для хранения в базе данных
		/// </summary>
		public Hashtable DataFields
		{
			get { return _DataFields; }
		}
		private Hashtable _DataFields = new Hashtable();
		
		public string this[string field]
		{
			get { return (string) this._DataFields[field]; }
			set { this._DataFields[field] = value; }
		}

		public bool IsGuest
		{
			get { return _UserId == -1; }
		}

		public bool IsAuthorized
		{
			get { return !IsGuest; }
		}

		private string _LoginName = "Guest";

		public string LoginName
		{
			get { return _LoginName; }
		}

		private string _Email = "";

		public string Email
		{
			get { return _Email; }
		}


		private int _UserId = -1;

		public int UserId
		{
			get { return _UserId; }
		}
		public override string ToString()
		{
			return LoginName;
		}
		#endregion

		#region group

		public bool IsInGroup(int groupId)
		{
			return SecurityManager.IsUserInGroup(this.UserId, groupId);
		}

		public bool IsAdmin
		{
			get { return IsInGroup(-3); }
		}

		public bool IsManager
		{
			get { return IsInGroup(-2); }
		}

		#endregion

		#region access methods

		/// <summary>
		/// Возвращает кэш разрешений для текущего пользователя
		/// </summary>
		/// <returns>DataTable</returns>
		public DataTable GetAccessRuleCache()
		{
			DataTable tb = null;
			Hashtable cache = _cache;
			if (cache[Keys.AccessRuleTableCache] != null)
			{
				tb = (DataTable) cache[Keys.AccessRuleTableCache];
			}
			else
			{
				tb = new DataTable("accessrules");
				tb.Columns.Add("partid", typeof (int));
				tb.Columns.Add("actionvar", typeof (string));
				DataTable tbR = SecurityManager.GetAccessRuleList();
				DataTable tbG = SecurityManager.GetUserGroupList(_UserId);
				foreach (DataRow r in tbG.Rows)
				{
					DataRow[] rows = tbR.Select("groupid=" + r["groupid"].ToString());
					foreach (DataRow row in rows)
					{
						tb.Rows.Add(new object[] {row["partid"], row["actionvar"]});
					}
				}
				cache.Add(Keys.AccessRuleTableCache, tb);
			}
			return tb;
		}

		public void ClearAccessRuleCache()
		{
			//HttpContext.Current.Session.Remove(Keys.AccessRuleTableCache);
			_cache = new Hashtable();
		}
		public bool Can(int partId, string actionVar)
		{

			if(partId == -1 && actionVar == SecurityManager.GetInternalActionVar(-1))
			{
				return true;
			}
			if (_UserId == -3)
			{
				return true;
			}
			if(_UserId == -2 && (_UserId <= partId) && actionVar == SecurityManager.GetInternalActionVar(-1))
			{
				return true;
			}
			DataTable tb = GetAccessRuleCache();
			return tb.Select("partid=" + partId.ToString() + " AND actionvar='" + actionVar + "'").Length > 0;
		}

		public bool CanAccess(int partId)
		{
			return Can(partId, SecurityManager.GetInternalActionVar(-1));
		}

		public bool CanCurrent(string actionVar)
		{
			return Can(PageInfo.Current.SecurityPart, actionVar);
		}

		public bool CanAccessCurrent()
		{
			return CanAccess(PageInfo.Current.SecurityPart);
		}

		#endregion

		#region dbfields

		public int RemoveAllFields()
		{
			return SecurityManager.RemoveAllUserFields(_UserId);
		}

		public DataTable GetAllFieldsTable()
		{
			return SecurityManager.GetAllUserFieldsTable(_UserId);
		}

		public Hashtable GetAllFields()
		{
			this._DataFields = SecurityManager.GetAllUserFields(_UserId);
			return this._DataFields;
		}

		public int RemoveField(string field)
		{
			return SecurityManager.RemoveUserField(_UserId, field);
		}

		public int SetField(string field, string value)
		{
			return SecurityManager.SetUserField(_UserId, field, value);
		}

		public string GetField(string field)
		{
			return SecurityManager.GetUserField(_UserId, field);
		}

		#endregion
	}
}