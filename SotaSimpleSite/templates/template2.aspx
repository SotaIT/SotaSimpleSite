<%@ Page Language="C#" MasterPageFile="~/templates/template2.master" Inherits="Sota.Web.SimpleSite.BaseTemplatedPage" %>
<script runat="server">
	void Page_PreInit(object sender, EventArgs e)
	{
		Page.MasterPageFile = "~/templates/template3.master";
	}
</script>
<asp:Content ContentPlaceHolderID="phContent" runat="server">
</asp:Content>
