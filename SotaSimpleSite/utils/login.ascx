<%@ Control Language="C#"  %>
<%@ Import Namespace="Sota.Web.SimpleSite" %>
<%@ Import Namespace="Sota.Web.SimpleSite.Security" %>

<script runat="server">
    string url = "";
    string enteredLogin = "";



void Page_Load(object sender, EventArgs e)
{

}	

    protected override void OnLoad(EventArgs e)
    {

        base.OnLoad(e);

        if (Sota.Web.SimpleSite.Security.UserInfo.Current.IsAuthorized)
        {
            Response.Redirect("~/");
        }

        if (Request.HttpMethod.ToUpper() == "POST"
            && !Sota.Web.SimpleSite.Util.IsBlank(Request.Form["login"])
            && !Sota.Web.SimpleSite.Util.IsBlank(Request.Form["pass"])
            )
        {
            string login = Request.Form["login"];
            string password = Request.Form["pass"];

            Login(login, password);
        }
        if (Request.QueryString["url"] != null)
        {
            url = "?url=" + Request.QueryString["url"];
        }

    }

    void Login(string login, string password)
    {
        if (UserInfo.LoginNew(login, password, true))
        {
            if (Sota.Web.SimpleSite.Path.Page == "enter")
            {
                if (Request.QueryString["url"] != null)
                {
                    Response.Redirect(Request.QueryString["url"]);
                }
            }
			else if (Request.UrlReferrer != null 
				&& Request.UrlReferrer.Host.IndexOf(Path.Domain) > -1)
            {
                Response.Redirect(Request.UrlReferrer.ToString());
            }
            Response.Redirect("~/");
        }
        else
        {
            WrongLoginPassword(login);
        }
    }

    void WrongLoginPassword(string login)
    {
        enteredLogin = login;
        phError.Visible = true;
    }
</script>


<asp:PlaceHolder runat="server" ID="phError" Visible="false">
<p style="color:Red;">Неверные логин и пароль!</p>
</asp:PlaceHolder>

<form method="post" action="/enter/">
	<table>
		<tr>
			<th style="width: 200px">Логин:</th>
			<td><input type="text" name="login" tabindex="1" value="<%=enteredLogin %>" /></td>
		</tr>
		<tr>
			<th>Пароль:</th>
			<td><input type="password" name="pass" tabindex="2" /></td>
		</tr>
		<tr>
			<th style="border: none">&nbsp;</th>
			<td style="border: none"><input type="submit" value=" Войти "  tabindex="3" /></td>
		</tr>
	</table>
</form>