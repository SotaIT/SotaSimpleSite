<%@ Control Language="c#" AutoEventWireup="false" Codebehind="modules.ascx.cs" Inherits="Sota.Web.SimpleSite.Code.Admin.modules" TargetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<asp:datagrid id="dgModules" runat="server" autogeneratecolumns="False" cellpadding="4" borderwidth="3px"
	borderstyle="Double" bordercolor="#0000C0" width="100%">
	<alternatingitemstyle backcolor="#EEEEEE"></alternatingitemstyle>
	<headerstyle font-bold="True" forecolor="White" backcolor="Blue"></headerstyle>
	<columns>
		<asp:hyperlinkcolumn datanavigateurlfield="path" datatextfield="name" headertext="��������"></asp:hyperlinkcolumn>
		<asp:boundcolumn datafield="path" headertext="����"></asp:boundcolumn>
		<asp:buttoncolumn text="�������" commandname="Delete"></asp:buttoncolumn>
	</columns>
</asp:datagrid>
<%if(IsAdmin){%>
<br>
<asp:textbox id="txtName" runat="server">��������</asp:textbox>
<asp:textbox id="txtPath" runat="server">����</asp:textbox>
<asp:linkbutton id="lnkAdd" runat="server">��������</asp:linkbutton>
<%}%>
