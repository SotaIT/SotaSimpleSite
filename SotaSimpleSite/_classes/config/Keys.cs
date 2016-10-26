namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Поля данного класса содержат строковые
	/// названия ключей
	/// </summary>
	public sealed class Keys
	{
		private Keys()
		{
		}

		public const string ContentSeparators = "\n\t -.,:{}[]()*+!\'\"/\\_=|?@#$%^&~`<>;№«»";
		public const string CacheTableConfigDelimiter = ":";
		public const string ServerRoot = "~";
		public const string FileNameDot = ".";
		public const string UrlPathProtocolDelimiter = "://";
		public const string UrlPathDelimiter = "/";
		public const string FilePathDelimiter = "\\";
		public const string UrlPortDelimiter = ":";
		public const string UrlParamPageDelimiter = "?";
		public const string UrlParamDelimiter = "&";
		public const string UrlParamValueDelimiter = "=";
		public const string ProtocolHttp = "http";
		public const string ProtocolHttps = "https";
		public const string HTTP_X_FORWARDED_FOR = "HTTP_X_FORWARDED_FOR";
		public const string HTTP_REFERER = "HTTP_REFERER";
		public const string ControlPHContent = "phContent";
		public const string ConfigExtension = ".config";
		public const string ContextPathFull = "Path_full";
		//public const string ContextPathDomain = "Path_domain";
		//public const string ContextPathPage = "Path_page";
		public const string ContextPageInfo = "pageInfo";
		public const string ContextPageInfoTitle = "pageInfoTitle";
		public const string ConfigSkin = "skin.config";
		public const string TableNameSkin = "skin";
		public const string ConfigDomain = "domain.config";
		public const string TableNameDomain = "domain";
		public const string CustomPath = "path";
		public const string CustomPage = "page";
		public const string CustomFileName = "filename";
		public const string CustomDomain = "domain";
		public const string ConfigPagex = "pagex.config";
		public const string TableNamePagex = "pagex";
		public const string ConfigNoRewrite = "norewrite.config";
		public const string TableNameNoRewrite = "item";
		public const string ConfigMain = "main.config";
		public const string TableNameMain = "main";
		public const string KeyConfigFolderPath = "configFolderPath";
		public const string ConfigSearch = "search.config";
		public const string TableNameSearch = "search";
		public const string KeyMainRedirectPage = "redirectpage";
		public const string KeyMainImagePage = "imagepage";
		public const string KeyMainExtension = "ext";
		public const string KeyMainDefaultSkin = "defaultskin";
		public const string KeyMainFileManagerPage = "filemanagerpage";
		public const string KeyMainHtmlEditorPage = "htmleditorpage";
		public const string KeyMainDownloadPage = "downloadpage";
//		public const string KeyMainOpenAnyDomain = "openanydomain";
		public const string KeyMainTimePickerPage = "timepickerpage";
		public const string KeyMainDatePickerPage = "datepickerpage";
		public const string KeyMainAdminDefault = "admindefault";
		//public const string KeyMainAdminFolderName = "adminfoldername";
		public const string KeyMainAdminLoginPage = "adminloginpage";
		public const string KeyMainLoginPage = "loginpage";
		public const string KeyMainAdminPassword = "adminpass";
		public const string KeyMainManagerPassword = "managerpass";
		public const string KeyMainEnableAuthorization = "enableauth";
		public const string KeyMainSeoError = "seoerror";
		public const string KeyMainImages = "img";
		public const string KeyMainCss = "css";
		public const string KeyMainScript = "script";
		public const string KeyMainFiles = "files";
		public const string KeyMainHashMode = "hashmode";
		//public const string KeyMainData = "data";
		public const string KeyMainDefaultTemplate = "deftemplate";
		public const string KeyMainConnectionString = "constr";
		public const string KeyMainRedirectAll = "redirectall";
		public const string KeyMainCustom = "custom";
		public const string KeySearchResults = "results";
		public const string KeySearchSort = "sort";
		public const string KeySearchBeginTag = "begintag";
		public const string KeySearchEndTag = "endtag";
		public const string CookieSkinId = "skinId";
		public const string SessionSkinInfo = "skinInfo";
		public const string QueryStringUrl = "url";
		public const string FolderDefaultPage = "default";
		public const string QueryStringAspxErrorPath = "aspxerrorpath";
		public const string SessionUserInfo = "userInfo";
		public const string AccessRuleTableCache = "tbAccessRule";
		public const string KeyMainTitleSeparator = "titleseparator";
		public const string ContextRequestId = "request_id";
		public const string CacheSiteMapObject = "CacheSiteMapObject";
		public const string KeyMainTimeZone = "timezone";
		public const string QueryStringLogout = "logout";
		public const string CachedOnlineUsersListObject = "CachedOnlineUsersListObject";
		public const string KeyMainOnlineTimeOut = "onlinetimeout";
		public const string KeyAssemblyVersion = "AssemblyVersion";
		public const string KeyRuntimeVersion = "RuntimeVersion";
		public const string KeyMainLogRedirect = "logredirect";
		public const string KeyMainOff = "off";
		public const string KeyMainLogError		= "logerror";
		public const string CookieSessionID		= "ASP.NET_SessionId";
		public const string CookieCounted		= "sota_counted";
		public const string KeyMainLogDownload	= "logdownload";
		public const string CachedList			= "CachedList";
		internal static string UrlOk			= "http://www.sotait.net/license.aspx?name=";
		internal static string HostOk			= "localhost";
		public const string KeySecurityConnector	= "SotaSecurityConnector";
		public const string KeyLogConnector	= "SotaLogConnector";
		public const string ConfigSecurity = "security.config";
		public const string ConfigLog = "log.config";
		public const string KeySecurityStoredSqlManager = "SotaSecurityStoredSqlManager";
		public const string KeyLogStoredSqlManager = "SotaLogStoredSqlManager";
		public const string KeyTimeZoneInfo = "SotaTimeZoneInfo";
		public const string ConfigTimeZone = "timezone.config";
		public const string KeyEncryptionKey = "encryptionkey";
		public const string KeyEncryptionVI = "encryptionvi";
		public const string KeyEncryptionLevel = "enclevel";
		public const int DefaultSearchResultBodyLength = 100;
		public const string DefaultSearchPrefix = "...";
		public const string DefaultSearchPostfix = "...";
		public const string UserIsRestricted = "restricted";
        public const string QueryStringAutoLogin = "autologin";
        public const string KeyMainPageCache = "pagecache";
    }
}