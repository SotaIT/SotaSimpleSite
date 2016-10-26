<%@ Page Language="c#" Inherits="Sota.Web.SimpleSite.BasePage"%><!DOCTYPE html>
<%@ Import Namespace="Sota.Web.SimpleSite" %>
<%@ Import Namespace="Sota.Web.SimpleSite.Map" %>
<script runat="server">
	Sota.Web.SimpleSite.PageInfo pageInfo = Sota.Web.SimpleSite.PageInfo.Current;
	Sota.Web.SimpleSite.SiteMap siteMap = Sota.Web.SimpleSite.SiteMap.Current;
	void Page_Load(object o, EventArgs e)
	{
	}

	void Page_LoadComplete(object sender, EventArgs e)
	{
		if (pageInfo.FileName != "default")
		{
			phJumbotron.Visible = false;
			ltH1.Text = string.Format("<h1>{0}</h1>", pageInfo.Title);

			StringBuilder sbPath = new StringBuilder();
			sbPath.Append("<h5><a href=\"/\">Главная</a>");
			Sota.Web.SimpleSite.Map.MapItemCollection mc = siteMap.GetCurrentItemPath();
			for (int i = 1; i < mc.Count - 1; i++)
			{
				sbPath.AppendFormat(" &rarr; <a href=\"{0}\">{1}</a>", mc[i].Url, mc[i].Text);
			}
			ArrayList arrReplaceCurrentCrumbs = SiteUtil.GetReplaceCurrentCrumbs();
			if (arrReplaceCurrentCrumbs != null)
			{
				for (int i = 0; i < arrReplaceCurrentCrumbs.Count; i += 2)
				{
					sbPath.AppendFormat(" &rarr; <a href=\"{0}\">{1}</a>"
						, arrReplaceCurrentCrumbs[i]
						, arrReplaceCurrentCrumbs[i + 1]);
				}

			}
			sbPath.AppendFormat(" &rarr; {0}</h5>", pageInfo.Title);

			ltPath.Text = sbPath.ToString();
		}
	}

	string BuildMenu()
	{
		StringBuilder sb = new StringBuilder();
		RenderMenuItem(siteMap, sb);
		return sb.ToString();
	}

	void RenderMenuItem(MapItem m, StringBuilder sb)
	{
		if (m.Level == -1)
		{
			for (int i = 0; i < m.Items.Count; i++)
				if (!m.Items[i].Hidden)
				{
					RenderMenuItem(m.Items[i], sb);
				}
		}
		else
		{
			bool hasChildren = false;
			if (m.Level == 0)
			{
				for (int i = 0; i < m.Items.Count; i++)
				{
					if (!m.Items[i].Hidden)
					{
						hasChildren = true;
						break;
					}
				}
			}

			if (hasChildren)
			{
				sb.AppendFormat(
					"<li class=\"dropdown{2}\"><a href=\"{0}\" class=\"dropdown-toggle\" data-toggle=\"dropdown\">{1} <b class=\"caret\"></b></a><ul class=\"dropdown-menu\">"
					, m.Url
					, m.Text
					, m.Selected ? " active" : "");
				for (int i = 0; i < m.Items.Count; i++)
					if (!m.Items[i].Hidden)
					{
						RenderMenuItem(m.Items[i], sb);
					}
				sb.Append("</ul></li>");
			}
			else
			{
				sb.AppendFormat(
					"<li{2}><a href=\"{0}\">{1}</a><li>"
					, m.Url
					, m.Text
					, m.Selected ? " class=\"active\"" : "");
			}
		}
	}
</script>
<html>
<head>
	<title><%=pageInfo.Title%></title>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1">
	<meta name="keywords" content="<%=pageInfo.KeyWords%>" />
	<meta name="description" content="<%=pageInfo.Description%>" />
	<link href="/templates/bootstrap3/css/bootstrap.min.css" rel="stylesheet">
	<link rel="stylesheet" type="text/css" href="/templates/bootstrap3/custom.css" />
	<link rel="stylesheet" type="text/css" href="/css/style.css" />
	<!-- HTML5 shim and Respond.js IE8 support of HTML5 elements and media queries -->
	<!--[if lt IE 9]>
	<script src="/templates/bootstrap3/js/html5shiv.js"></script>
	<script src="/templates/bootstrap3/js/respond.min.js"></script>
	<![endif]-->
	<asp:PlaceHolder ID="head" runat="server"></asp:PlaceHolder>
	<%=pageInfo.Head%>
</head>
<body role="document">
<asp:PlaceHolder ID="navigation" runat="server">
    <div class="navbar navbar-inverse navbar-fixed-top" role="navigation">
      <div class="container">
        <div class="navbar-header">
          <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
            <span class="sr-only">Навигация</span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
            <span class="icon-bar"></span>
          </button>
          <a class="navbar-brand" href="/"><%=siteMap.Text %></a>
        </div>
        <div class="navbar-collapse collapse">
          <ul class="nav navbar-nav">
			  <%=BuildMenu()%>
          </ul>
          <form class="navbar-form navbar-right" role="form" action="/login/">
            <div class="form-group">
              <input type="text" placeholder="E-mail" class="form-control">
            </div>
            <div class="form-group">
              <input type="password" placeholder="Пароль" class="form-control">
            </div>
            <button type="submit" class="btn btn-success">Войти</button>
          </form>
        </div>
      </div>
    </div>
</asp:PlaceHolder>
<asp:PlaceHolder runat="server" ID="phJumbotron">
<div class="jumbotron">
    <div class="container">
    <h1>Добро пожаловать!</h1>
    <p>Наш замечательный сайт приветствует Вас!</p>
    <!--<p><a class="btn btn-primary btn-lg" role="button">Learn more &raquo;</a></p>-->
    </div>
</div>
</asp:PlaceHolder>
<div class="container" role="main">
<asp:Literal runat="server" ID="ltPath"></asp:Literal>
<asp:Literal runat="server" ID="ltH1"></asp:Literal>
<asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
</div>
<script src="/templates/bootstrap3/js/jquery-1.11.1.min.js"></script>
<script src="/templates/bootstrap3/js/bootstrap.min.js"></script>
<asp:PlaceHolder ID="scripts" runat="server"></asp:PlaceHolder>
</body>
</html>