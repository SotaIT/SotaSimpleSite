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
            //����
            LanguageInfo.Init(Context);

            //���������� � ��������� ������ ����
            string fullpath = Request.Url.AbsoluteUri.ToLower();
            Path.InitFull(fullpath);
            //���������� ����� � ��������������� ����
            //��������
            string protocol = Util.GetProtocol(Request.IsSecureConnection);
            //������ ���� ��� ���������
            string path = fullpath.Remove(0, protocol.Length).Split(Keys.UrlParamPageDelimiter[0])[0];

            //����� ����, ������� ����������� � ���� �������� � ����������� �� ������
            string addPath = string.Empty;

            string site = string.Empty;
            string domain = path.Split('/')[0] + Request.ApplicationPath.TrimEnd('/');
            int domainLength = domain.Length;

            //���� ���������� �����
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


            //���� ���������� �� .aspx � �� ��, ��� ������� � ����������, �� �������
            //����������
            string ext = Config.Main.Extension;
            if (path.EndsWith(".aspx"))
            {
                ext = ".aspx";
            }
            else if (!path.EndsWith(ext))
            {
                return;
            }


            //���������� ��������
            //���� �������� ��� ������, ���������� � ������ �������
            string pathWithoutDomain = path.Remove(0, domainLength + 1);
            string page = pathWithoutDomain.Length == 0 ? "default" : pathWithoutDomain.Remove(pathWithoutDomain.Length - ext.Length, ext.Length);
            //��������� ������������� ����������
            if (Config.GetNoRewrite().Select("'" + page.Replace("'", "''") + "' LIKE path").Length > 0)
            {
                return;
            }
            page = addPath + page;
            //��������� ��������
            PageInfo pi = PageInfo.Init(page);
            //���� �������� �� ����������� ��� ������� �������
            if (pi == null || pi.Deleted)
            {
                return;
            }
            //��������� ���������� �� ��������
            //��������, ������� ������������� ������ ��������
            if (pi.Protocol != protocol)
            {
                Response.Redirect(pi.Protocol + path);
            }
            //������ ����������
            //������ ��������
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
