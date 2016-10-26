<%@ Page language="c#" Inherits="Sota.Web.SimpleSite.AdminBasePage" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
	<head>
		<title><%=PageInfo.Title%></title>
		<meta name=vs_targetSchema content="http://schemas.microsoft.com/intellisense/ie5">
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1251">
		<meta name="robots" content="NOINDEX">
		<meta http-equiv="expires" content="0">
		<base target="_self">
		<link rel="stylesheet" href="<%=ResolveUrl("~/admincss.ashx?css=")%>admin.dialog.css" type="text/css">
	</head>
	<body bottommargin="0" topmargin="0" leftmargin="0" rightmargin="0">
    <asp:placeholder id="phContent" runat="server"></asp:placeholder>
	</body>
</html>