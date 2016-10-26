<%@ Page language="c#" Inherits="Sota.Web.SimpleSite.AdminBasePage" %>
<%@ Register TagPrefix="sws" Namespace="Sota.Web.SimpleSite" Assembly="SotaSimpleSite"%>
<%@ Register TagPrefix="suc" TagName="Top" Src="top.ascx"%>
<%@ Register TagPrefix="suc" TagName="Bottom" Src="bottom.ascx"%>
<suc:Top runat="server"></suc:Top>
<sws:form runat="server" id="frmMain">
<!--Контент---------------------------------------------->
	<asp:placeholder id="phContent" runat="server"></asp:placeholder>
<!--/Контент---------------------------------------------->
</sws:form>
<suc:Bottom runat="server"></suc:Bottom>				