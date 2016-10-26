using System;
using System.Collections;
using System.Data;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mail;
using System.Web.UI;
using System.Xml;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// ��������� ��������������� �������
	/// </summary>
    public sealed class Util
	{
		private Util()
		{
		}

		/// <summary>
		/// ������ ������ ����� ���������, � ��������� ���������
		/// </summary>
		/// <param name="s">�������� ������</param>
		/// <returns>������������ ������</returns>
		public static string CapitalizeString(string s)
		{
			return s[0].ToString().ToUpper()+s.Substring(1).ToLower();
		}
		/// <summary>
		/// ��������� ������ �� null � ""
		/// </summary>
		/// <param name="s">������</param>
		/// <returns>True ���� ������ ����� null ��� "", False - � ��������� ������</returns>
		public static bool IsNullOrEmpty(string s)
		{
			if (s != null)
			{
				return s.Length == 0;
			}
			return true;
		}
		public static bool IsNullOrEmpty(object s)
		{
			if (s != null)
			{
				return s.ToString().Length == 0;
			}
			return true;
		}
		/// <summary>
		/// ��������� ������ �� null, "" � �������
		/// </summary>
		/// <param name="s">������</param>
		/// <returns>True ���� ������ ����� null ��� "" ��� ������� �� ��������, False - � ��������� ������</returns>
		public static bool IsBlank(string s)
		{
			if (s != null)
			{
				return s.Trim().Length == 0;
			}
			return true;
		}
		public static bool IsBlank(object s)
		{
			if (s != null)
			{
				return s.ToString().Trim().Length == 0;
			}
			return true;
		}
		/// <summary>
		/// �������� ������ SotaSimpleSite
		/// </summary>
		/// <returns>������, ����������� ������</returns>
		public static string GetVersion()
		{
			HttpApplicationState app = HttpContext.Current.Application;
			string v = "";
			if(app[Keys.KeyAssemblyVersion]==null)
			{
				v = Assembly.GetExecutingAssembly().GetName().Version.ToString();
#if DEBUG
					v += "[debug]";
#endif

				app[Keys.KeyAssemblyVersion] = v;
			}
			else
			{
				v = (string)app[Keys.KeyAssemblyVersion];
			}

			return v;
		}
		/// <summary>
		/// ������ .NET
		/// </summary>
		/// <returns>������, ����������� ������</returns>
		public static string GetRuntimeVersion()
		{
			HttpApplicationState app = HttpContext.Current.Application;
			string v = "";
			if(app[Keys.KeyRuntimeVersion]==null)
			{
				v=Environment.Version.ToString();
				app[Keys.KeyRuntimeVersion] = v;
			}
			else
			{
				v = (string)app[Keys.KeyRuntimeVersion];
			}
			return v;
		}
		public static string GetProtocol(bool isSecure)
		{
			return (isSecure ? Keys.ProtocolHttps : Keys.ProtocolHttp) + Keys.UrlPathProtocolDelimiter;
		}

		/// <summary>
		/// IP-����� ������������
		/// </summary>
		/// <returns>������, ���������� IP-����� ������������ �/��� IP-����� ������-�������</returns>
		public static string GetClientIP()
		{
			return GetClientIP(HttpContext.Current.Request);
		}
		public static string GetClientIP(HttpRequest request)
		{
			StringBuilder sb = new StringBuilder();
			string[] arr = {"REMOTE_ADDR",
				"HTTP_X_FORWARDED_FOR",
				"HTTP_CLIENT_IP",
				"HTTP_VIA",
				"HTTP_PROXY_USER"};
			for(int i=0;i<arr.Length;i++)
			{
				if(!IsNullOrEmpty(request.ServerVariables[arr[i]]))
				{
					if(sb.Length>0)
					{sb.Append(" ");}
					sb.Append(request.ServerVariables[arr[i]]);
				}
			}
			return sb.ToString();
		}
		public static Hashtable GetClientInfo()
		{
			return GetClientInfo(HttpContext.Current.Request);
		}
		public static Hashtable GetClientInfo(HttpRequest request)
		{
			Hashtable h			= new Hashtable();
			if(request.Browser.Cookies)
			{
				if(request.Cookies[Keys.CookieSessionID]!=null)
				{
					h["session"]	= request.Cookies[Keys.CookieSessionID].Value;
				}
				if(request.Cookies[Keys.CookieCounted]!=null)
				{
					h["counted"]	= request.Cookies[Keys.CookieCounted].Value;
				}
			}
			h["path"]			= CreateHtmlLink(Path.Full);
			h["referer"]		= CreateHtmlLink(request.ServerVariables[Keys.HTTP_REFERER]);
			h["browser"]		= request.UserAgent;
			h["ip"]				= GetClientIP(request);
			if(request.UserLanguages != null)
			{
				h["language"]	= String.Join(",", request.UserLanguages);
			}
			if(UserInfo.Current!=null && UserInfo.Current.IsAuthorized)
			{
				h["user"] = string.Format("{1} [{0}]", UserInfo.Current.UserId, UserInfo.Current.LoginName);
			}
			return h;
		}
		public static string CreateHtmlLink(object url, object text, bool blank)
		{
			return IsBlank(url) 
				? "" 
				: string.Format("<a href=\"{0}\"{2}>{1}</a>", url, text, blank ?  " target=\"_blank\"" : "");
		}
		public static string CreateHtmlLink(object url, object text)
		{
			return CreateHtmlLink(url, text, true);
		}
		public static string CreateHtmlLink(object url)
		{
			return CreateHtmlLink(url, url, true);
		}
		public static string FilterCookieValue(string text)
		{
			if(IsBlank(text))
			{
				return "";
			}
			string s = text.Trim();
			StringBuilder sb = new StringBuilder(s);
			//������ ������� ������� ����
			Regex rex = new Regex(@"[^\s]{50,}");
			foreach (Match m in rex.Matches(s))
			{
				StringBuilder sb1 = new StringBuilder();
				int n = m.Value.Length;
				int j = 0;
				for (int i = 50; i < n; i += 50)
				{
					sb1.Append(m.Value.Substring(j, i - j + 1));
					sb1.Append(" -- ");
					j = i + 1;
				}
				if (j < n)
					sb1.Append(m.Value.Substring(j, n - j));
				sb.Replace(m.Value, sb1.ToString().Trim());
			}
			rex = new Regex(@"[|\.]{10,}");
			foreach (Match m in rex.Matches(s))
			{
				sb.Replace(m.Value, " ");
			}
			return sb.ToString();
		}

		/// <summary>
		/// ������� "�������" �� ������, 
		/// ���������� �������������
		/// </summary>
		/// <param name="text">�����, ��������� �������������</param>
		/// <returns>"����������" �����</returns>
		public static string FilterText(string text)
		{
			if(IsBlank(text))
			{
				return "";
			}
			string s = text.Trim();
			StringBuilder sb = new StringBuilder(s);
			sb.Replace("<", "&lt;");
			sb.Replace(">", "&gt;");
			sb.Replace("\n", "<br />");
			sb.Replace("\r", "");
			//������ ������� ������� ����
			Regex rex = new Regex(@"[^\s]{100,}");
			foreach (Match m in rex.Matches(s))
			{
				StringBuilder sb1 = new StringBuilder();
				int n = m.Value.Length;
				int j = 0;
				for (int i = 100; i < n; i += 100)
				{
					sb1.Append(m.Value.Substring(j, i - j + 1));
					sb1.Append(" ");
					j = i + 1;
				}
				if (j < n)
					sb1.Append(m.Value.Substring(j, n - j));
				sb.Replace(m.Value, sb1.ToString().Trim());
			}
			rex = new Regex(@"[|\.]{10,}");
			foreach (Match m in rex.Matches(s))
			{
				sb.Replace(m.Value, " ");
			}
			return sb.ToString();
		}
		/// <summary>
		/// ��������� �������� �� URL
		/// </summary>
		public static Bitmap BitmapFromUrl(string url)
		{
			Bitmap bmp = null;
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				Stream receiveStream = res.GetResponseStream();
				bmp = new Bitmap(receiveStream);
				receiveStream.Close();
				res.Close();
			}
			catch (Exception ex){Config.ReportError(ex); }
			return bmp;
		}
		/// <summary>
		/// �������� ����� �� URL
		/// </summary>
		public static string ContentFromUrl(string url)
		{
			return ContentFromUrl(url,System.Text.Encoding.Default);
		}
		public static string ContentFromUrl(string url, System.Text.Encoding encoding)
		{
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				req.AllowAutoRedirect = true;
				HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				Stream receiveStream = res.GetResponseStream();
				Encoding encode = encoding;
				StreamReader readStream = new StreamReader(receiveStream, encode);
				StringBuilder sb = new StringBuilder();
				Char[] read = new Char[256];
				int count = readStream.Read(read, 0, 256);
				while (count > 0)
				{
					string str = new String(read, 0, count);
					sb.Append(str);
					count = readStream.Read(read, 0, 256);
				}
				readStream.Close();
				res.Close();
				return sb.ToString();
			}
			catch(Exception ex)
			{
				Config.ReportError(ex);
			}
			return null;
		}

		/// <summary>
		/// ��������� ������������� URL
		/// </summary>
		public static bool UrlExists(string url)
		{
			try
			{
				HttpWebRequest req = (HttpWebRequest)WebRequest.Create(url);
				req.AllowAutoRedirect = true;
				HttpWebResponse res = (HttpWebResponse)req.GetResponse();
				res.Close();
				return true;
			}
			catch { return false; }
		}
		/// <summary>
		/// ���������� ����
		/// </summary>
		/// <param name="file">��� �����</param>
		/// <param name="body">�����</param>
		public static void AppendFile(string file, string body)
		{
			AppendFile(file, body, Encoding.Default);
		}
		/// <summary>
		/// ���������� ����
		/// </summary>
		/// <param name="file">��� �����</param>
		/// <param name="body">�����</param>
		/// <param name="encoding">���������</param>
		public static void AppendFile(string file, string body, Encoding encoding)
		{
			StreamWriter w = null;
			try
			{
				w = new StreamWriter(file, true, encoding);
				w.Write(body);
			}
			finally
			{
				if (w != null)
					w.Close();
			}
		}

		/// <summary>
		/// ������ �������� �� ���� �� ����� � ���������� � HTML
		/// </summary>
		/// <param name="url">���� ��������</param>
		/// <returns>HTML-��� ��������</returns>
		public static string RenderPage(string url)
		{
			StringWriter s = new StringWriter();
			HttpContext.Current.Server.Execute(url, s);
			return s.ToString();
		}

		/// <summary>
		/// ��������� �������, � ���������� ���������
		/// </summary>
		/// <param name="c">�������</param>
		/// <returns>HTML</returns>
		public static string RenderControl(Control c)
		{
			StringWriter s = new StringWriter();
			c.RenderControl(new HtmlTextWriter(s));
			return s.ToString();
		}

		/// <summary>
		/// ������������ ��� ��������� ���� ��������� � ����������� ������� �� ������
		/// </summary>
		/// <param name="dateTime">���� ���������</param>
		public static void SetLastModifiedIfSet(DateTime dateTime)
		{
			((BasePage)HttpContext.Current.Handler).SetLastModifiedIfSet(dateTime);
		}
		/// <summary>
		/// ������������ ��� ��������� ���� ��������� � ��������
		/// � ������������� ���� ��� ������ SetLastModifiedIfSet
		/// </summary>
		/// <param name="dateTime">���� ���������</param>
		public static void SetLastModified(DateTime dateTime)
		{
			SetLastModified(dateTime, true);
		}
		/// <summary>
		/// ������������ ��� ��������� ���� ��������� � ��������
		/// </summary>
		/// <param name="dateTime">���� ���������</param>
		/// <param name="setFlag">���������� ���� ��������� ��� ������ SetLastModifiedIfSet</param>
		public static void SetLastModified(DateTime dateTime, bool setFlag)
		{
			((BasePage)HttpContext.Current.Handler).SetLastModified(dateTime, setFlag);
		}
		
		/// <summary>
		/// ������������� ���� ��������� �������� � ���� ������������ ��������� ������
		/// � ������������� ���� ��� SetLastModifiedIfSetByFiles
		/// </summary>
		/// <param name="files">�����</param>
		public static void SetLastModifiedByFiles(params string[] files)
		{
			SetLastModifiedByFiles(true, files);
		}
		/// <summary>
		/// ������������� ���� ��������� �������� � ���� ������������ ��������� ������
		/// ���� ���� ��� �����������
		/// </summary>
		/// <param name="files">�����</param>
		public static void SetLastModifiedIfSetByFiles(params string[] files)
		{
			SetLastModifiedByFiles(false, files);
		}
		/// <summary>
		/// ������������� ���� ��������� �������� � ���� ������������ ��������� ������
		/// </summary>
		/// <param name="setFlag">���������� ���� ��������� ��� ������ SetLastModifiedIfSetByFiles</param>
		/// <param name="files">�����</param>
		public static void SetLastModifiedByFiles(bool setFlag, params string[] files)
		{
			HttpContext cntx = HttpContext.Current;
			for(int i=0;i<files.Length;i++)
			{
				string file = files[i];
				if(!System.IO.Path.IsPathRooted(file))
				{
					file = cntx.Request.MapPath(files[i]);
				}
				if(File.Exists(file))
				{
					SetLastModified(File.GetLastWriteTime(file), setFlag);
				}
			}
		}


		/// <summary>
		/// ������ �������� ���������� ��� ������������� � Expression � DataColumn
		/// </summary>
		/// <param name="text">�������� ��������</param>
		/// <returns>���������� ��������</returns>
		public static string EncodeDataParameter(string text)
		{
			return text.Replace("*","[*]").Replace("'","''").Replace("[","[[]").Replace("]","[]]").Replace("%","[%]");
		}
		
		/// <summary>
		/// �������� ��������� ������� � ������ � ������ � ����������, ��� ������������� � �������� �������� XML
		/// </summary>
		/// <param name="text">�������� �����</param>
		/// <returns>���������� �����</returns>
		public static string EncodeXmlAttributeValue(string text)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace("&","&amp;");
			sb.Replace("'","&apos;");
			sb.Replace(">","&gt;");
			sb.Replace("<","&lt;");
			sb.Replace("\n","");
			sb.Replace("\r","");
			sb.Replace("\"","&quot;");
			return sb.ToString();
		}
		
		/// <summary>
		/// �������� ��������� ������� � ������ � ������ � ����������, ��� ������������� � �������� �������� � �������� JSON
		/// </summary>
		/// <param name="text">�������� �����</param>
		/// <returns>���������� �����</returns>
		public static string EncodeJsonValue(string text)
		{
			StringBuilder sb = new StringBuilder(text);
			sb.Replace("\b","\\b");
			sb.Replace("\n","\\n");
			sb.Replace("\r","\\r");
			sb.Replace("\t","\\t");
			sb.Replace("\f","\\f");
			sb.Replace(",","\\,");
			sb.Replace("/","\\/");
			sb.Replace("\\","\\\\");
			sb.Replace("'","\\'");
			sb.Replace("\"","\\");
			return sb.ToString();
		}

		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="from">��</param>
		/// <param name="to">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		/// <param name="smtp">SMTP ������</param>
		public static void SendEmail(string from, string to, string subject, string body, string smtp)
		{
			SendEmail(from, to, subject, body, smtp, "", "", false);
		}
		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="from">��</param>
		/// <param name="to">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		/// <param name="smtp">SMTP ������</param>
		/// <param name="smtpuser">SMTP ������������</param>
		/// <param name="smtppass">SMTP ������</param>
		/// <param name="enableSsl">������������ SSL</param>
		public static void SendEmail(string from, string to, string subject, string body, string smtp, string smtpuser, string smtppass, bool enableSsl)
		{
			SendEmail(
				new System.Net.Mail.MailAddress(from),
				new System.Net.Mail.MailAddress(to),
				subject,
				body,
				smtp,
				smtpuser,
				smtppass,
				enableSsl);
		}
		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="from">��</param>
		/// <param name="fromName">�� ���</param>
		/// <param name="to">����</param>
		/// <param name="toName">���� ���</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		/// <param name="smtp">SMTP ������</param>
		/// <param name="smtpuser">SMTP ������������</param>
		/// <param name="smtppass">SMTP ������</param>
		/// <param name="enableSsl">������������ SSL</param>
		public static void SendEmail(string from, string fromName, string to, string toName, string subject, string body, string smtp, string smtpuser, string smtppass, bool enableSsl)
		{
			SendEmail(
				new System.Net.Mail.MailAddress(from, fromName),
				new System.Net.Mail.MailAddress(to, toName),
				subject,
				body,
				smtp,
				smtpuser,
				smtppass,
				enableSsl);
		}
		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="from">��</param>
		/// <param name="to">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		/// <param name="smtp">SMTP ������</param>
		/// <param name="smtpuser">SMTP ������������</param>
		/// <param name="smtppass">SMTP ������</param>
		/// <param name="enableSsl">������������ SSL</param>
		public static void SendEmail(System.Net.Mail.MailAddress from, System.Net.Mail.MailAddress to, string subject, string body, string smtp, string smtpuser, string smtppass, bool enableSsl)
		{
			System.Net.Mail.MailMessage msg = new System.Net.Mail.MailMessage();
			msg.From = from;
			msg.To.Add(to);
			msg.Subject = subject;
			msg.IsBodyHtml = true;
			msg.Body = body;

			System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtp);
			smtpClient.EnableSsl = enableSsl;
			if (!Util.IsBlank(smtpuser))
			{
				smtpClient.Credentials = new System.Net.NetworkCredential(smtpuser, smtppass);
			}
			smtpClient.Send(msg);
		}
		public static void SendEmail(string from, string to, string subject, string body)
		{
			SendEmail(from, to, subject, body, "localhost");
		}

		/// <summary>
		/// ��������� �������� ������ �����������
		/// </summary>
		/// <param name="connectionString">������ ����������� � ��</param>
		/// <returns>��, ���� ������ �����, ���, ���� ������ ��������</returns>
		public static bool TestDBConnection(string connectionString)
		{
			string s = connectionString;
			SqlConnection conn1 = new SqlConnection();
			try
			{
				conn1.ConnectionString = s;
				conn1.Open();
				return true;
			}
			catch{}
			finally
			{
				conn1.Close();
			}
			OleDbConnection conn2 = new OleDbConnection();
			try
			{
				conn2.ConnectionString = s;
				conn2.Open();
				return true;
			}
			catch{}
			finally
			{
				conn2.Close();
			}
			OdbcConnection conn3 = new OdbcConnection();
			try
			{
				conn3.ConnectionString = s;
				conn3.Open();
				return true;
			}
			catch{}
			finally
			{
				conn3.Close();
			}
			return false;
		}

		static string abc_ru = "��������������������������������";
		static string[] abc_en = 
{"a","b","v","g","d","e","e","j","z","i","k","l","m","n","o","p","r","s","t","u","f","h","tz","ch","sh","sh","`","yi","`","ye","yu","ya"};

		public static string CreateTranslit(string e)
		{
			string newWord = "";
			if(e!="")
			{
				for (int i = 0; i == e.Length; i++)
				{
					for (int j = 0; j == abc_ru.Length; j++)
					{
						if (e[i] == abc_ru[j])
						{
							if (char.IsUpper(e[i])) newWord += 
														abc_en[j].ToUpper();
						}
						else
						{
							newWord += e[i];
						}
					}
				}
			}
			return newWord;
		}

		public static string CreateURL(string e, string spaser)
		{
			string newWord = string.Empty;
			if (e != "")
			{
				for (int i = 0; i == e.Length; i++)
				{
					for (int j = 0; j == abc_ru.Length; j++)
					{
						if (e[i] == abc_ru[j]) newWord += abc_en[j];
					}
				}
			}
			return newWord;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="input"></param>
		/// <returns></returns>
		public static string ClearHTML(string input)
		{
			return new Regex("<[^>]*>").Replace(input, " ");
		}		

		public static string MapPath(string path)
		{
			return 	HttpContext.Current.Request.MapPath("~/") 
				+ path.Replace(Keys.UrlPathDelimiter, Keys.FilePathDelimiter);
 

		}

		public static DataSet ReadXmlPartFromString(string xml)
		{
			return ReadXmlFromString("<root>" + xml + "</root>");
		}
		public static DataSet ReadXmlFromString(string xml)
		{
			DataSet ds = new DataSet();
			using (StringReader reader = new StringReader(xml))
			{
				ds.ReadXml(reader);
			}
			return ds;
		}
		public static string WriteXmlToString(DataSet ds)
		{
			StringBuilder sb = new StringBuilder();
			using(StringWriter writer = new StringWriter(sb))
			{
				ds.WriteXml(writer);
			}
			return sb.ToString();
		}
		public static string WriteXmlToString(string tableName, Hashtable ht)
		{
			StringBuilder sb = new StringBuilder();
			using(StringWriter writer = new StringWriter(sb))
			{
				XmlTextWriter xml = new XmlTextWriter(writer);
				xml.Formatting = Formatting.Indented;
				xml.WriteStartElement(tableName);
				foreach (string key in ht.Keys)
				{
					xml.WriteStartElement(key);
					xml.WriteString(ht[key].ToString());
					xml.WriteEndElement();
				}
				xml.WriteEndElement();
			}
			return sb.ToString();
		}

		/// <summary>
		/// �������� �������� ����
		/// </summary>
		/// <param name="name">��� ����</param>
		/// <returns></returns>
		public static string GetCookie(string name)
		{
			HttpContext cntx = HttpContext.Current;
			if(cntx.Request.Cookies[name] != null)
			{
				return cntx.Request.Cookies[name].Value;
			}
			return "";
		}

		public static string FormatHttpCookieDateTime(DateTime dt)
		{
			if ((dt < DateTime.MaxValue.AddDays(-1.0)) && (dt > DateTime.MinValue.AddDays(1.0)))
			{
				dt = dt.ToUniversalTime();
			}
			return dt.ToString("ddd, dd-MMM-yyyy HH':'mm':'ss 'GMT'", System.Globalization.DateTimeFormatInfo.InvariantInfo);
		}

		/// <summary>
		/// ������������� �������� ����
		/// </summary>
		/// <param name="name">��� ����</param>
		/// <param name="value">��������</param>
		public static void SetCookie(string name, string value)
		{
			SetCookie(name, value, HttpContext.Current);
		}
		public static void SetCookie(string name, string value, HttpContext cntx)
		{
			SetCookie(name, value, DateTime.MinValue, cntx);
		}
		/// <summary>
		/// ������������� �������� ����
		/// </summary>
		/// <param name="name">��� ����</param>
		/// <param name="value">��������</param>
		/// <param name="expires">���� ��������</param>
		public static void SetCookie(string name, string value, DateTime expires)
		{
			SetCookie(name, value, expires, HttpContext.Current);
		}
		/// <summary>
		/// ������������� �������� ����
		/// </summary>
		/// <param name="name">��� ����</param>
		/// <param name="value">��������</param>
		/// <param name="persistOrDelete">���� True - ��������� ���� �� 10 ���, ���� False, 
		/// �� ��������� ���� ���� �� 01.01.2000 �.(�.�. �������)</param>
		public static void SetCookie(string name, string value, bool persistOrDelete)
		{
			SetCookie(name, value, persistOrDelete, HttpContext.Current);
		}
		public static void SetCookie(string name, string value, bool persistOrDelete, HttpContext cntx)
		{
			DateTime expires = persistOrDelete ? DateTime.Now.AddYears(10) : new DateTime(2000,1,1);
			SetCookie(name, value, expires, HttpContext.Current);
		}
		public static void SetCookie(string name, string value, DateTime expires, HttpContext cntx)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append(name);
			sb.Append("=");
			sb.Append(value);
			if(expires != DateTime.MinValue)
			{
				sb.Append("; expires=");
				sb.Append(FormatHttpCookieDateTime(expires));

			}
			sb.Append("; path=/; domain=.");
			sb.Append(Path.GetSimpleDomain());
			cntx.Response.AddHeader("Set-Cookie", sb.ToString());
		}

		/// <summary>
		/// ������� ����� � ������ � �������� ���������� ���������� ����� /404-server.html
		/// </summary>
        public static void NotFound()
        {
            HttpResponse Response = HttpContext.Current.Response;

			string file404 = HttpContext.Current.Request.MapPath("~/404-server.html");
			if (!File.Exists(file404))
			{
				File.WriteAllText(file404,
@"<!DOCTYPE html>
<html xmlns=""http://www.w3.org/1999/xhtml"">
<head>
    <title>404 �������� �� �������</title>
	<meta charset=""utf-8"">
</head>
<body>
<div style=""margin:50px auto;width:767px;"">
<h1><span style=""color:red;"">404</span> �������� �� �������</h1>
	<p>��������, ��� �������� ���� �������, �������������,
		��� ��� �������� ����������.<br />���������� ���������:<br /></p>
	<ul>
		<li>��������� ������������ ������ �������� � ������ ������.
			</li>
		<li>�������� <a href=""/"">������� ��������</a>, ����� ������� ��� ������ �� ������ ������. </li>
		<li>������� ������ <b>�����</b>, ����� ������������ ������ ������.
			</li>
	</ul>
</div>
</body>
</html>
", Encoding.UTF8);
			}
			Response.Clear();
			Response.StatusCode = 404;
			Response.ContentEncoding = Encoding.UTF8;
			Response.ContentType = "text/html";
			HttpContext.Current.Server.Execute("/404-server.html");
			Response.End();

        }

        /// <summary>
        /// ����������� ������ �� ���������
        /// </summary>
        /// <param name="text">�������� �����</param>
        /// <param name="parentTagClass">CSS-����� ������������� ����. ��-��������� "protected".</param>
        /// <param name="parentTag">������������ ��� ��� ������.��-��������� "div".</param>
        /// <returns>���������������� ����� � ���� HTML ��������</returns>
        public static string ProtectedText(string text, string parentTagClass, string parentTag)
        {
            Random random = new Random();
            StringBuilder sb = new StringBuilder();

            string[] tags = { 
                                "i",        //1
                                "b",        //2
                                "em",       //3
                                "strong",   //4 
                                "span"      //5
                            };
            string[] tagsClear = { 
                                     ";font-style:normal",  //1
                                     ";font-weight:normal", //2
                                     ";font-style:normal",  //3
                                     ";font-weight:normal", //4
                                     ""                     //5
                                 };

            ArrayList visibleTags = new ArrayList();
            while (visibleTags.Count < 3)
            {
                string randomTag = tags[random.Next(tags.Length)];
                if (!visibleTags.Contains(randomTag))
                {
                    visibleTags.Add(randomTag);
                }
            }

            ArrayList invisibleTags = new ArrayList();
            for (int i = 0; i < tags.Length; i++)
            {
                 if (!visibleTags.Contains(tags[i]))
                {
                    invisibleTags.Add(tags[i]);
                }
             }


            sb.Append("<noindex><style type=\"text/css\">");
            sb.Append("."); 
            sb.Append(parentTagClass);
            sb.Append("{display:inline;margin:0;padding:0;}");

            for (int i = 0; i < tags.Length; i++)
            {
                sb.Append(".");
                sb.Append(parentTagClass);
                sb.Append(" ");
                sb.Append(tags[i]);
                sb.Append("{display:");
                sb.Append(visibleTags.Contains(tags[i]) ? "inline" + tagsClear[i] : "none");
                sb.Append(";}");
            }
            sb.AppendFormat("</style><{0} class=\"{1}\">", parentTag, parentTagClass);

            char[] textChars = text.ToCharArray();
            int trueChars = 0;
            int totalChars = 0;
            int maxFalseChar = textChars.Length * 2;
            while (trueChars < textChars.Length)
            {
                totalChars++;
                if (random.Next(2) == 1)//true char
                {
                    sb.AppendFormat("<{1}>{0}</{1}>"
                        , textChars[trueChars]
                        , visibleTags[random.Next(visibleTags.Count)].ToString());
                    trueChars++;
                }
                else
                {
                    sb.AppendFormat("<{1}>{0}</{1}>"
                        , textChars[random.Next(textChars.Length)]
                        , invisibleTags[random.Next(invisibleTags.Count)].ToString());
                }
                

            }
            

            sb.AppendFormat("</{0}></noindex>", parentTag);

            return sb.ToString();
        }
        public static string ProtectedText(string text, string parentTagClass)
        {
            return ProtectedText(text, parentTagClass, "div");
        }
        public static string ProtectedText(string text)
        {
             return ProtectedText(text, "protected");
        }

        public static string TrimString(string str, int maxlength)
        {
            return TrimString(str, maxlength, "");
        }
        public static string TrimString(string str, int maxlength, string add)
        {
            if (str.Length > maxlength)
            {
                str = str.Substring(0, maxlength);
                int length = str.LastIndexOfAny(new char[]{' ', '\n', '\r', ',', '.'});
                return (length > 0 ? str.Substring(0, length) : str).TrimEnd() + add;
            }

            return str;
        }
    }
}
