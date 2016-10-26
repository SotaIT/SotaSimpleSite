using System;
using System.Data;
using System.Globalization;
using System.Threading;
using System.Web;

namespace Sota.Web.SimpleSite
{
    public sealed class LanguageInfo
    {
		private LanguageInfo()
		{ 

		}
		public override string ToString()
		{
			return this._id;
		}

		internal static void Init(HttpContext cntx)
		{ 
			DataRow rConfig = GetConfig().Rows[0];
			if(rConfig["on"].ToString()=="0")
			{
				return;
			}
			LanguageInfo li = new LanguageInfo();
			string by = "";
			switch(rConfig["by"].ToString())
			{
				case "domain":
					Uri url = cntx.Request.Url;
					by = (url.Host.StartsWith("www.") ? url.Host.Substring(4) : url.Host) + (url.Port==80 ? "" : url.Port.ToString()) + Sota.Web.SimpleSite.Path.VRoot;
					break;
				case "path":
					by = cntx.Request.Url.ToString().Replace(Sota.Web.SimpleSite.Path.ARoot,"").Split('/')[0];
					break;
				default:
					by = cntx.Request.QueryString[rConfig["by"].ToString().Split('=')[1]];
					break;
			}
			li._by = by;
			DataRow[] rs = GetLanguages().Select("by='" + by + "'");
			if(rs.Length>0)
			{
				li._id = rs[0]["id"].ToString();
				li._name = rs[0]["name"].ToString();
				li._sname = rs[0]["sname"].ToString();
				if(rs[0]["culture"].ToString()!="default")
				{
					li._culture = CultureInfo.CreateSpecificCulture(rs[0]["culture"].ToString());
					Thread.CurrentThread.CurrentCulture = li._culture;
					Thread.CurrentThread.CurrentUICulture =  li._culture;
				}
			}
			cntx.Items[context_key] = li;
		}
		public static DataTable GetLanguages()
		{
			return Config.GetConfigTable("language.config", "language");
		}
		public static DataTable GetConfig()
		{
			return Config.GetConfigTable("language.config", "config");
		}

		private string _id				= "ru";
		private string _by				= "ru";
		private string _name			= "Русский";
		private string _sname			= "рус";
		private CultureInfo _culture	= CultureInfo.CurrentCulture;
		const string context_key		= "SOTA_LANGUAGE";
		public static LanguageInfo Current
    	{
    		get
    		{
				HttpContext cntx = HttpContext.Current;
    			if(cntx.Items[context_key]==null)
    			{
    				Init(cntx);
    			}
				return (LanguageInfo)cntx.Items[context_key];
    		}
		}
		public string ID
		{
			get { return _id; }
		}
		public string By
		{
			get { return _by; }
		}

    	public string Name
    	{
    		get { return _name; }
    	}

    	public string ShortName
    	{
    		get { return _sname; }
    	}

		public CultureInfo Culture
    	{
    		get { return _culture; }
    	}
    }
}
