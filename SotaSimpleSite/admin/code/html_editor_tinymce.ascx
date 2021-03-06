<%@ Control Language="c#" targetSchema="http://schemas.microsoft.com/intellisense/ie5"%>
<%@ Import Namespace="Sota.Web.SimpleSite"%>
<script runat="server">
	string img = "";
	string root = "";
	string sCss = "";
	void Page_Load(object o, EventArgs e)
	{
		img = ResolveUrl("~/admin.ashx?img=editor");
		sCss = ResolveUrl(Config.Main.Css);
		
		Sota.Web.SimpleSite.Security.UserInfo ui = Sota.Web.SimpleSite.Security.UserInfo.Current;
		if ((string)ui.Fields[Keys.UserIsRestricted] == "yes")
		{
			root = ui["files"] == null 
				? ResolveUrl("~/files/" + ui.LoginName)
				: ResolveUrl(ui["files"].ToString());
		}
		else
		{
			root = Request.QueryString["root"] == null 
				? ResolveUrl(Config.Main.Files) 
				: Request.QueryString["root"];			 
		}
		root = root.TrimEnd('/') + "/";

		string rootFolder = Request.MapPath(root);
		if (!System.IO.Directory.Exists(rootFolder))
		{ 
			System.IO.Directory.CreateDirectory(rootFolder);
		}
	}
</script>



<%string sMce = ResolveUrl("~/admin/tinymce");%>
<%-- <script type="text/javascript" src="<%=sMce %>/jscripts/tiny_mce/tiny_mce.js"></script>--%>

<script type="text/javascript" src="<%=sMce %>/jscripts/tiny_mce/tiny_mce_gzip.js"></script>
<script type="text/javascript">
    tinyMCE_GZ.init({
        plugins: "safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,inlinepopups",
        themes: 'advanced',
        languages: 'ru',
        disk_cache: true,
        debug: false
    });
</script>

<script type="text/javascript">
	var field;
	if (window.dialogArguments) {
		field = window.dialogArguments;
	}

	tinyMCE.init({
		// General options
		mode : "textareas",
		theme : "advanced",
		plugins : "safari,pagebreak,style,layer,table,save,advhr,advimage,advlink,emotions,iespell,insertdatetime,preview,media,searchreplace,print,contextmenu,paste,directionality,fullscreen,noneditable,visualchars,nonbreaking,xhtmlxtras,template,inlinepopups",

		// Theme options
		theme_advanced_buttons1: "save,code,|,bold,italic,underline,strikethrough,|,justifyleft,justifycenter,justifyright,justifyfull,|,formatselect,fontselect,fontsizeselect",
		theme_advanced_buttons2 : "cut,copy,paste,pastetext,|,search,replace,|,bullist,numlist,|,outdent,indent,blockquote,|,undo,redo,|,link,unlink,anchor,image,cleanup,|,forecolor,backcolor",
		theme_advanced_buttons3 : "tablecontrols,|,hr,removeformat,visualaid,|,sub,sup,|,charmap,media,advhr,|,ltr,rtl",
		theme_advanced_buttons4 : "insertlayer,moveforward,movebackward,absolute,|,styleprops,|,cite,abbr,acronym,del,ins,attribs,|,nonbreaking",
		theme_advanced_toolbar_location : "top",
		theme_advanced_toolbar_align : "left",
		theme_advanced_statusbar_location: "bottom",
		theme_advanced_resizing : false,

		content_css : "<%=sCss %>/admin.htmleditor_inner.css",
	
		language : "ru",

		file_browser_callback: 'OpenFileBrowser',
		
		save_callback : 'Save',

		accessibility_warnings: false,
		relative_urls: false

	});
	function OpenFileBrowser(field_name, url, type, win) {
		
		tinyMCE.activeEditor.windowManager.open({
		file: '<%=ResolveUrl(Config.Main.FileManagerPage)%>?editor=tinymce&field=' + field_name
			+ (type == 'image' ? '&filter=gif;jpg;jpeg;bmp;png;ico' : '')
			+ ('<%=root %>' == '' ? '' : '&root=<%=root %>')
			, title: 'Выбор файла'
			, width: 650  // Your dimensions may differ - toy around with them!
			, height: 550
			, resizable: "no"
			, inline: "yes"  // This parameter only has an effect if you use the inlinepopups plugin!
			, close_previous: "no"
		}, {
			window: win,
			input: field_name
		});
		

		return false;
	}

	function Save() {

		if (field) 
		{
			field.value = tinyMCE.activeEditor.getContent();
		}
		window.close();
	}
</script>
<form>
<textarea id="txtBody" style="width:100%;height:100%;" name="txtBody">
</textarea>
</form>

<script type="text/javascript">
	if (field) {
		document.getElementById('txtBody').value = field.value;
	}

</script>