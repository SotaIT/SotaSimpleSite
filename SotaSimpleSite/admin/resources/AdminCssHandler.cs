using System;

namespace Sota.Web.SimpleSite.Resources 
{
	/// <summary>
	/// Summary description for AdminCss.
	/// </summary>
	public class AdminCssHandler: System.Web.IHttpHandler
	{
		public AdminCssHandler()
		{
		}
		public bool IsReusable
		{
			get { return true; }
		}

		public void ProcessRequest(System.Web.HttpContext context)
		{
			context.Response.Cache.SetCacheability(System.Web.HttpCacheability.Public);
			context.Response.ContentType = "text/css";
			switch(context.Request.QueryString["css"])
			{
				case "admin.css":
					context.Response.Cache.SetLastModified(new DateTime(2006,8,15));
					context.Response.Write(
@"body, td, p, div, input, select
{
	font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
	font-size: 11px;
}
h1,h2,h3,h4,h5,h6
{
	color:#0000ff;
}
body
{
	background-color:#ffffff;
}
textarea
{
	font-size: 13px;
	font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
	width:200px;
	height:100px;
}
a
{
	color: #0000ff;
	text-decoration: none;
}
a img
{
	border:0;
	vertical-align:middle;
}
form
{
	padding:0;
	margin:0;
	display:inline;
}
a:hover
{
	text-decoration: underline;
}
a.logo_sota:hover
{
	text-decoration: none;
}
.bottomt
{
	background-color: #0000ff;
}
.bottomt td, .bottomt a
{
	font-size: 9px;
	color: #ffffff;
}
.topt
{
	background-color: #0000ff;
}
.topt td
{
	font-size: 1px;
}
td.white
{
	background-color: #ffffff;
}
.logo1
{
	padding-right: 5px;
	padding-left: 5px;
	font-weight: bold;
	font-size: 25px;
	padding-bottom: 5px;
	color: #0000ff;
	padding-top: 5px;
}
.logo2 a
{
	color:#ffffff;
}
.logo1 a:hover, .logo2 a:hover
{
	text-decoration:none;
}
.logo2
{
	padding-right: 5px;
	padding-left: 5px;
	font-weight: bold;
	font-size: 18px;
	padding-bottom: 5px;
	color: #ffffff;
	padding-top: 5px;
}
.logo3
{
	padding-right: 5px;
	padding-left: 5px;
	font-size: 9px;
	padding-bottom: 5px;
	padding-top: 5px;
}
.logo3 a, .logo3 a:hover
{
	color: #cccccc;
	text-decoration:none;
}
td.menu
{
	padding: 5px 0px 5px 0px;
	font-weight:bold;
	color:#aaa;	
	vertival-align:middle;
}
"
						);
					break;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "admin.dialog.css":
					context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
					context.Response.Write(
@"body, td, p, div, input, select, th
{
	font-family: Verdana, Geneva, Arial, Helvetica, sans-serif;
	font-size: 11px;
}"
						);
					break;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "admin.htmleditor.css":
					context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
					context.Response.Write(
@".SotaHtmlEditor_all
{
	background-color: buttonface;
	border: 1px solid #999999;
}
.SotaHtmlEditor_toolbar
{
	padding: 2px;
	height: 25px;
}
.SotaHtmlEditor_toolbar a
{
	border: buttonface 1px solid;
	padding: 3px;
	font-size: 12px;
	margin: 0px;
	vertical-align: middle;
	width: 25px;
	color: #000000;
	font-family: 'Times New Roman';
	height: 20px;
	text-align: center;
	text-decoration: none;
}
.SotaHtmlEditor_toolbar a:hover
{
	border: 1px solid #0000aa;
	background-color: #ddddff;
	filter: Alpha(opacity=80);
}
.SotaHtmlEditor_toolbar a.selected
{
}
.SotaHtmlEditor_toolbar a img
{
	border: 0;
	vertical-align: middle;
	margin: 0;
}
.SotaHtmlEditor_toolbar select
{
	border: 0;
	margin: 0;
	font-family: Verdana,Arial;
	font-size: 10px;
	vertical-align: middle;
}
.SotaHtmlEditor_toolbar span
{
	border-right: 1px inset;
	border-top: 1px inset;
	border-left: 1px inset;
	border-bottom: 1px inset;
	margin-right: 3px;
	margin-top: 0px;
	margin-left: 3px;
	margin-bottom: 0px;
	width: 1px;
	vertical-align: middle;
	height: 17px;
}
.SotaHtmlEditor_toolbar div
{
	border-right: 1px inset;
	border-top: 1px inset;
	border-left: 1px inset;
	border-bottom: 1px inset;
	margin-right: 3px;
	margin-top: 3px;
	margin-left: 3px;
	margin-bottom: 3px;
	height: 0px;
	width:100%;
	line-height:0;
	font-size: 1;
}
.SotaHtmlEditor_view
{
	margin-top: 1px;
	margin-bottom: 1px;
	background-color: window;
	border: 2px inset;
}
.SotaHtmlEditor_txt,
.SotaHtmlEditor_view
{
	overflow: auto;
	font-family: Verdana, Arial;
	font-size: 12px;
	padding: 2px;
}
.SotaHtmlEditor_mode
{
	padding: 5px;
}
.SotaHtmlEditor_mode div
{
	border: 1px solid buttonface;
	padding: 2px;
	margin: 0px;
	display: inline;
	cursor: default;
	font-family: Verdana, Arial;
	font-size: 11px;
	width: 70px;
}
.SotaHtmlEditor_mode div.selected
{
	border: 1px solid #000077;
	background-color: #ffffff;
} 
.SotaHtmlEditor_path
{
	white-space: nowrap;
}
.SotaHtmlEditor_path a
{
	margin: 0px;
	border: 1px solid #333333;
	text-decoration: none;
	padding: 1px;
	color: #333333;
}
.SotaHtmlEditor_path a:hover
{
	border-color: #ffffff;
	background-color: #777777;
	color: #ffffff;
}"
						);
					break;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "admin.map.css":
					context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
					context.Response.Write(
@"form
{
	padding:0;
	margin:0;
}
a img
{
	vertical-align:middle;
	border:0;
}
.tbform
{
	background-color: #0000ff;
}
.tbform th
{
	color: #ffffff;
	font-size: 11px;
}
.tdform
{
	background-color: #ffffff;
}
.pnTdOpen
{
	background-color: #ffffff;
}
.pnTdOpen a, .pnTdOpen a:hover
{
	font-weight: bold;
	text-decoration: none;
	cursor: default;
}
.pnTdClosed
{
	border-left: #000099 1px solid;
	background-color: #0000ff;
}
.pnTdClosed a
{
	color: #ffffff;
	font-weight: bold;
}
.editable
{
	padding: 6px;
	width: 100%;
}
.lnkButton
{
	border-right: #cccccc 1px solid;
	padding-right: 2px;
	border-top: #cccccc 1px solid;
	display: block;
	padding-left: 2px;
	padding-bottom: 2px;
	margin: 2px;
	border-left: #cccccc 1px solid;
	padding-top: 2px;
	border-bottom: #cccccc 1px solid;
	background-color: #eeeeee;
}
.lnkButton:hover
{
	text-decoration: none;
}
.lnkButton img
{
	vertical-align:middle;
	border:0px;
}
#headPagePath
{
	border-right: #cccccc 1px solid;
	padding-right: 3px;
	border-top: #cccccc 1px solid;
	padding-left: 3px;
	padding-bottom: 3px;
	margin: 0px;
	border-left: #cccccc 1px solid;
	color: #0000ff;
	padding-top: 3px;
	border-bottom: #cccccc 1px solid;
	background-color: #eeeeee;
}
#headPagePath a
{
	text-decoration: none;
	white-space: nowrap;
}

.menu_p
{
	border-right: #cccccc 1px solid;
	padding-right: 3px;
	border-top: #cccccc 1px solid;
	padding-left: 3px;
	padding-bottom: 3px;
	margin: 0px;
	border-left: #cccccc 1px solid;
	color: #0000ff;
	padding-top: 3px;
	border-bottom: #cccccc 1px solid;
	background-color: #eeeeee;
}
.menu_p a, .menu_p a:hover
{
	text-decoration: none;
	white-space: nowrap;
}
.labels a
{
	font-weight: bold;
}
.treeview
{
	/*font-family: Verdana, Geneva, Arial, Helvetica, sans-serif; 	font-size: 11px; 	color: #666;*/
	white-space: nowrap;
	overflow: auto;
	width: 200px;
	height: 100%;
	color: #0000ff;
}
.treeview img
{
	border: 0px;
	vertical-align: middle;
}
/*.treeview a
{
	color: #333;
	text-decoration: none;
}*/
.treeview a.node, .treeview a.nodeSel
{
	white-space: nowrap;
	padding: 1px 2px 1px 2px;
}
/*.treeview a.node:hover, .treeview a.nodeSel:hover
{
	color: #333;
	text-decoration: underline;
}*/
.treeview a.nodeSel
{
	background-color: #0000ff;
	color: #ffffff;
}
.treeview .clip
{
	overflow: hidden;
}
.table.item
{
	border: solid 1px #888888;
}
.table.item th
{
	background-color:#bbbbbb;
	text-align:left;
	vertical-align:middle;
	cursor:hand;
}
.table.item th img
{
	vertical-align:middle;
}
.table.item td.form
{
	border-top: solid 1px #8888888;
	background-color:#eeeeee;
}
img.refresh_button
{
	MARGIN-TOP: 2px; 
	VERTICAL-ALIGN: top;
	border:0;
}
div.content
{
	overflow: auto;
	width: 100%;
	height: 100%;
	padding:3px;
}"
						);
					break;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				case "admin.pages.css":
					context.Response.Cache.SetLastModified(new DateTime(2006,7,8));
					context.Response.Write(
@".tbform
{
	background-color: #0000ff;
}
.tbform th
{
	color: #ffffff;
	font-size: 11px;
}
.tdform
{
	background-color: #ffffff;
}
.pnTdOpen
{
	background-color: #ffffff;
}
.pnTdOpen a, .pnTdOpen a:hover
{
	font-weight: bold;
	text-decoration: none;
	cursor: default;
}
.pnTdClosed
{
	border-left: #000099 1px solid;
	background-color: #0000ff;
}
.pnTdClosed a
{
	color: #ffffff;
	font-weight: bold;
}
.editable
{
	padding: 6px;
	width: 100%;
}
.lnkButton
{
	border-right: #cccccc 1px solid;
	padding-right: 2px;
	border-top: #cccccc 1px solid;
	display: block;
	padding-left: 2px;
	padding-bottom: 2px;
	margin: 2px;
	border-left: #cccccc 1px solid;
	padding-top: 2px;
	border-bottom: #cccccc 1px solid;
	background-color: #eeeeee;
}
.lnkButton:hover
{
	text-decoration: none;
}
.lnkButton img
{
	vertical-align:middle;
	border:0px;
}
#headPagePath
{
	border-right: #cccccc 1px solid;
	padding-right: 3px;
	border-top: #cccccc 1px solid;
	padding-left: 3px;
	padding-bottom: 3px;
	margin: 0px;
	border-left: #cccccc 1px solid;
	color: #0000ff;
	padding-top: 3px;
	border-bottom: #cccccc 1px solid;
	background-color: #eeeeee;
	font-weight: bold;
}
#headPagePath a
{
	text-decoration: none;
	white-space: nowrap;
}
#headPagePath a img
{
	vertical-align:middle;
}
.labels a
{
	font-weight: bold;
}
.treeview
{
	/*font-family: Verdana, Geneva, Arial, Helvetica, sans-serif; 	font-size: 11px; 	color: #666;*/
	white-space: nowrap;
	overflow: auto;
	width: 200px;
	height: 100%;
	color: #0000ff;
}
.treeview img
{
	border: 0px;
	vertical-align: middle;
}
/*.treeview a
{
	color: #333;
	text-decoration: none;
}*/
.treeview a.node, .treeview a.nodeSel
{
	white-space: nowrap;
	padding: 1px 2px 1px 2px;
}
/*.treeview a.node:hover, .treeview a.nodeSel:hover
{
	color: #333;
	text-decoration: underline;
}*/
.treeview a.nodeSel
{
	background-color: #0000ff;
	color: #ffffff;
}
.treeview .clip
{
	overflow: hidden;
}
"
						);
					break;
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			}
		}
	}
}