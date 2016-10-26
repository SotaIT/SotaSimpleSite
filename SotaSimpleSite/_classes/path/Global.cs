using System.Web;
using System;
using System.Data;
using System.IO;
using System.Text;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Global.asax
	/// </summary>
    public class Global : System.Web.HttpApplication
	{
		public override void Init()
		{
			this.BeginRequest+=new EventHandler(Application_BeginRequest);
			this.Error += new EventHandler(Application_Error);
			base.Init();
		}
		
        public void Application_BeginRequest(Object sender, EventArgs e)
		{
            //язык
            LanguageInfo.Init(Context);

            //определяем и сохраняем полный путь
            string fullpath = Request.Url.AbsoluteUri.ToLower();
            Path.InitFull(fullpath);
            //определяем домен и соответствующий язык
            //протокол
            string protocol = Util.GetProtocol(Request.IsSecureConnection);
            //полный путь без протокола
            string path = fullpath.Remove(0, protocol.Length).Split(Keys.UrlParamPageDelimiter[0])[0];

            //кусок пути, который добавляется к пути страницы в зависимости от домена
            string addPath = string.Empty;

            string site = string.Empty;
            string domain = path.Split('/')[0] + Request.ApplicationPath.TrimEnd('/');
            int domainLength = domain.Length;

            //флаг отключения сайта
            if (Config.Main.Off)
            {
                string curPage = "~/" + path.Remove(0, domainLength + 1);
                if (!curPage.StartsWith("~/admin") && curPage != Config.Main.RedirectAll)
                {
                    Response.Redirect(Config.Main.RedirectAll);
                }
            }


            if (domain.StartsWith("www."))
            {
                domain = domain.Substring(4);
            }

            DataRow[] rs = Config.GetDomains().Select("'" + domain.Replace("'", "''") + "' LIKE name");
            if (rs.Length > 0)
            {
                addPath = rs[0]["addpath"].ToString();
                site = rs[0]["site"].ToString();
            }

            Path.InitDomain(domain);
            Path.InitSite(site);
            Path.InitAddPath(addPath);


            //если расширение не .aspx и не то, что указано в настройках, то выходим
            //расширение
            string ext = Config.Main.Extension;
            if (path.EndsWith(".aspx"))
            {
                ext = ".aspx";
            }
            else if (!path.EndsWith(ext))
            {
                return;
            }


            //определяем страницу
            //путь страницы без домена, расширения и строки запроса
            string pathWithoutDomain = path.Remove(0, domainLength + 1);
            string page = pathWithoutDomain.Length == 0 ? "default" : pathWithoutDomain.Remove(pathWithoutDomain.Length - ext.Length, ext.Length);
            //проверяем необходимость перезаписи
            if (Config.GetNoRewrite().Select("'" + page.Replace("'", "''") + "' LIKE path").Length > 0)
            {
                return;
            }
            page = addPath + page;
            //загружаем страницу
            PageInfo pi = PageInfo.Init(page);
            //если страница не загрузилась или удалена выходим
            if (pi == null || pi.Deleted)
            {
                return;
            }
            //проверяем правильный ли протокол
            //протокол, который соответствует данной странице
            if (pi.Protocol != protocol)
            {
                Response.Redirect(pi.Protocol + path);
            }
            //делаем перезапись
            //шаблон страницы
            if (pi.Template.Length > 0)
            {
                Path.InitPage(page);
                Context.RewritePath(Keys.ServerRoot + Keys.UrlPathDelimiter + pi.Template);
            }
		}

        protected void Application_Error(Object sender, EventArgs e)
        {
            Config.ReportError(Server.GetLastError().GetBaseException());
            if (Config.Main.SeoError)
            {
				Util.NotFound();
            }
        }

        public override string GetVaryByCustomString(HttpContext context, string custom)
        {
            switch (custom)
            {
                case Keys.CustomPath:
                    return Path.Full;
                case Keys.CustomDomain:
                    return Path.Domain;
                case Keys.CustomPage:
                    return Path.Page;
                case Keys.CustomFileName:
                    return PageInfo.Current.FileName;
            }
            return base.GetVaryByCustomString(context, custom);
        }


	}
}
