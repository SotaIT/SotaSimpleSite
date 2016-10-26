<%@ Control language="c#" targetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<script runat="server">
	Sota.Web.SimpleSite.Config.MainConfig MainConfig = Sota.Web.SimpleSite.Config.Main;
	Sota.Web.SimpleSite.PageInfo pi = Sota.Web.SimpleSite.PageInfo.Current;
	string Images = "";
	string[] arrMenu = null;
	protected override void OnLoad(EventArgs e)
	{
		Images = ResolveUrl("~/admin.ashx?img=");
		switch(pi.FileName)
		{
			case "admin/settings":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Настройки"}; 
				break;
			case "admin/security/default":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Безопасность"}; 
				break;
			case "admin/all":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Структура и контент сайта"}; 
				break;
			case "admin/domain":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Домены"}; 
				break;
			case "admin/template":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Шаблоны"}; 
				break;
			case "admin/control":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Блоки"}; 
				break;
			case "admin/controls":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Загрузка блоков"}; 
				break;
			case "admin/filemanager":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Управление файлами"}; 
				break;
			case "admin/texteditor":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Текстовый редактор"}; 
				break;
			case "admin/lists":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Списки"}; 
				break;
			case "admin/create_list":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","~/admin/lists.aspx","Списки","Создание списка"}; 
				break;
			case "admin/listimport":
                arrMenu = new string[] { "~/admin/default.aspx", "Главная", "~/admin/lists.aspx", "Списки", "Импорт данных в список" }; 
				break;
			case "admin/config_list":
                arrMenu = new string[] { "~/admin/default.aspx", "Главная", "~/admin/lists.aspx", "Списки", "Создание списка" }; 
				break;
			case "admin/list":
			case "admin/oldlist":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","~/admin/lists.aspx","Списки","Редактирование данных списка"}; 
				break;
			case "admin/modules":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Модули"}; 
				break;
			case "admin/log_view":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","~/admin/stats.aspx","Статистика","Просмотр логов"}; 
				break;
			case "admin/search":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Настройка поиска"}; 
				break;
			case "admin/execute":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Выполнение SQL"}; 
				break;
			case "admin/stats":
					arrMenu = new string[]{"~/admin/default.aspx","Главная","Статистика"}; 
				break;
		}
	}
</script>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN">
<html>
	<head>
		<title><%=pi.Title%></title>
		<meta http-equiv="Content-Type" content="text/html; charset=windows-1251">
		<meta http-equiv="expires" content="0">
		<meta content="http://schemas.microsoft.com/intellisense/ie5" name="vs_targetSchema">
		<link rel="stylesheet" href="<%=ResolveUrl("~/admincss.ashx?css=")%>admin.css" type="text/css">
	</head>
	<body bottommargin="5" topmargin="10">
		<table height="100%" cellspacing="0" cellpadding="0" width="100%" border="0">
			<tr>
				<td>
					<table class="topt" cellspacing="0" cellpadding="0" width="100%" border="0">
						<tr>
							<td><img height=10 src="<%=Images%>1x1.gif" width=10></td>
							<td><img height=1 src="<%=Images%>1x1.gif" width=180></td>
							<td width="100%">&nbsp;</td>
							<td>&nbsp;</td>
							<td class="white"><img height=10 src="<%=Images%>1x1.gif" width=10></td>
							<td><img height=10 src="<%=Images%>1x1.gif" width=10></td>
							<td><img height=1 src="<%=Images%>1x1.gif" width=100></td>
							<td><img height=10 src="<%=Images%>1x1.gif" width=10></td></tr>
						<tr>
							<td><img height=100 src="<%=Images%>1x1.gif" width=10></td>
							<td class="white">
								<p class="logo1"><a href="<%=ResolveUrl(MainConfig.AdminDefault)%>" title="На главную">Sota<br>Web<br>Admin</a></p></td>
							<td valign="bottom">
								<p class="logo2"><a href="http://www.sotait.net/" target="_blank">State-of-the-art<br>Information Technologies</a></p></td>
							<td>
							&nbsp;
							</td>
							<td class="white">&nbsp;</td>
							<td>&nbsp;</td>
							<td class="white" valign="top" align="right">
								<p class="logo3"><a href="http://www.sotait.net/" target="_blank">SOTA IT.NET</a></p></td>
							<td>&nbsp;</td></tr>
						<tr>
							<td><img src="<%=Images%>1x1.gif" width="10" height="10"></td>
							<td>&nbsp;</td>
							<td>&nbsp;</td>
							<td>&nbsp;</td>
							<td class="white">&nbsp;</td>
							<td>&nbsp;</td>
							<td>&nbsp;</td>
							<td><img src="<%=Images%>1x1.gif" width="10" height="10"></td></tr>
							</table>
							</td></tr>
			<%if(arrMenu!=null){%>
			<tr><td class="menu">
			<%for(int i=0;i<arrMenu.Length-1;i+=2){%>
			<a href='<%=ResolveUrl(arrMenu[i])%>'><%=arrMenu[i+1]%><img src="<%=Images%>arr.gif"></a>
			<%}%>
			<%=arrMenu[arrMenu.Length-1]%>
			</td></tr>
			<%}else{%>
			<tr><td>&nbsp;</td></tr>
			<%}%>
			<tr>
				<td width="100%" height="100%" valign="top">