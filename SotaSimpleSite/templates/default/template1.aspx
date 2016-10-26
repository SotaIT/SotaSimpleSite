<%@ Page Language="c#" Inherits="Sota.Web.SimpleSite.BasePage" %>

<!DOCTYPE html>
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
    string RenderMenu(Sota.Web.SimpleSite.Map.MapItem parent)
    {
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < parent.Items.Count; i++) if (!parent[i].Hidden)
            {
                sb.AppendFormat("<li{2}><a href=\"{0}\">{1}</a></li>"
                    , parent[i].Url
                    , parent[i].Text
                    , parent[i].Selected ? "  class=\"active\"" : "");
            }
        return sb.ToString();
    }
</script>
<html>
<head>
    <title><%=pageInfo.Title%></title>
    <meta charset="utf-8">
    <meta name="keywords" content="<%=pageInfo.KeyWords%>" />
    <meta name="description" content="<%=pageInfo.Description%>" />
    <link rel="stylesheet" type="text/css" href="/templates/default/css/style.css" />
    <asp:PlaceHolder ID="head" runat="server"></asp:PlaceHolder>
    <%=pageInfo.Head%>
</head>
<body>
    <div id="page">
        <div id="logo"><a href="/"><%=siteMap.Text %></a></div>
        <asp:PlaceHolder ID="navigation" runat="server">
            <div id="menu">
                <ul><%=RenderMenu(siteMap) %></ul>
            </div>
        </asp:PlaceHolder>
        <div id="content">
            <asp:Literal runat="server" ID="ltPath"></asp:Literal>
            <asp:Literal runat="server" ID="ltH1"></asp:Literal>
            <asp:PlaceHolder ID="phContent" runat="server"></asp:PlaceHolder>
        </div>
    </div>
    <asp:PlaceHolder ID="scripts" runat="server"></asp:PlaceHolder>
</body>
</html>
