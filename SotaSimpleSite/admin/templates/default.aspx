<%@ Page language="c#" Inherits="Sota.Web.SimpleSite.AdminBasePage" %>
<%@ Register TagPrefix="suc" TagName="Top" Src="top.ascx"%>
<%@ Register TagPrefix="suc" TagName="Bottom" Src="bottom.ascx"%>
<suc:Top runat="server"></suc:Top>
<!--Контент---------------------------------------------->
	<asp:placeholder id="phContent" runat="server"></asp:placeholder>
<!--/Контент---------------------------------------------->
<suc:Bottom runat="server"></suc:Bottom>