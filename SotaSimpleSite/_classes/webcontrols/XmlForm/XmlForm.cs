using System.Web.UI;
using System.ComponentModel;

namespace Sota.Web.SimpleSite.WebControls
{
	/// <summary>
	/// Форма с клинтскими вызовами через XmlHttp.
	/// </summary>
	[DefaultProperty("Action"),
		ToolboxData("<{0}:XmlForm runat=server></{0}:XmlForm>")]
	public class XmlForm : System.Web.UI.HtmlControls.HtmlContainerControl
	{
		#region const

		public const string ElementRoot = "root";
		public const string ElementForm = "form";
		public const string ElementAction = "action";
		public const string ElementList = "list";
		public const string ActionSubmit = "submit";
		public const string ActionRefresh = "refresh";
		public const string ActionList = "list";

		#endregion

		#region props & fields

		private string _action = "";

		public string Action
		{
			get { return _action; }
			set { _action = value; }
		}

		private string _beforeSubmit = "";

		public string BeforeSubmit
		{
			get { return _beforeSubmit; }
			set { _beforeSubmit = value; }
		}

		private string _afterSubmit = "";

		public string AfterSubmit
		{
			get { return _afterSubmit; }
			set { _afterSubmit = value; }
		}

		private string _onReadyStateChanged = "";

		public string OnReadyStateChanged
		{
			get { return _onReadyStateChanged; }
			set { _onReadyStateChanged = value; }
		}

		[Browsable(false)]
		public override string TagName
		{
			get { return "form"; }
		}

		public static string SubmitFunction
		{
			get { return "XmlForm_OnSubmit"; }
		}

		public static string RefreshFunction
		{
			get { return "XmlForm_Refresh"; }
		}

		public static string FillListFunction
		{
			get { return "XmlForm_FillList"; }
		}

		public static string SetSelectValueFunction
		{
			get { return "XmlForm_SetSelectValue"; }
		}

		[Browsable(false)]
		public override bool EnableViewState
		{
			get { return base.EnableViewState; }
			set { base.EnableViewState = value; }
		}

		#endregion

		#region render

		protected override void RenderAttributes(HtmlTextWriter writer)
		{
			string readyState = this.OnReadyStateChanged.Length > 0 ? "," + this.OnReadyStateChanged : "";
			writer.WriteAttribute("onsubmit", this.BeforeSubmit + SubmitFunction + "(this, this.action" + readyState + ");" + this.AfterSubmit + "return false;");
			base.Attributes.Remove("onsubmit");

			writer.WriteAttribute("name", this.UniqueID);
			base.Attributes.Remove("name");

			writer.WriteAttribute("action", this.Action);
			base.Attributes.Remove("action");

			writer.WriteAttribute("method", "post");
			base.Attributes.Remove("method");

			base.RenderAttributes(writer);
		}

		#endregion

		#region Xml Functionality

		/*
		 	
		private bool _useXml = false;
		public bool UseXml
		{get{return _useXml;}set{_useXml = value;}}
		protected override void RenderAttributes(HtmlTextWriter writer)
		{
			string onsubmit = this.Attributes["onsubmit"]!=null ? this.Attributes["onsubmit"] : "";
			writer.WriteAttribute("onsubmit", this.SubmitFunction+"(this,"+UseXml.ToString().ToLower()+");"+this.OnSubmit+"return false;");
			base.Attributes.Remove("onsubmit");
			//etc
		}
		private string GetSubmitFunction()
		{
			//etc
			sb.Append("\tfunction "+SubmitFunction+"(frm,useXml)\n\t{\n");	
			sb.Append("\t\tvar n = frm.length;\n");
			sb.Append("\n");
			sb.Append("\t\tif(useXml)\n");
			sb.Append("\t\t\tvar s = \"<?xml version=\"1.0\" encoding=\"utf-8\" ?><xmlform><form>\"+frm.name+\"</form>\";\n");
			sb.Append("\t\telse\n");
			sb.Append("\t\t\tvar s = \"form=\"+frm.name;\n");
			sb.Append("\t\tfor(i=0;i<n;i++)\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\tvar e = frm.elements[i];\n");
			sb.Append("\t\t\tif(e.name)\n");
			sb.Append("\t\t\t{\n");
			sb.Append("\t\t\t\tif(useXml)\n");
			sb.Append("\t\t\t\t\ts+= \"<\"+e.name+\">\"+encodeURIComponent(e.value)+\"</\"+e.name+\">\";\n");
			sb.Append("\t\t\t\telse\n");
			sb.Append("\t\t\t\t\ts+= \"&\"+e.name+\"=\"+encodeURIComponent(e.value);\n");
			sb.Append("\t\t\t}\n");
			sb.Append("\t\t}\n");
			sb.Append("\t\tif(useXml)\n");
			sb.Append("\t\t\ts += \"</xmlform>\";\n");
			sb.Append("\t\tif(document.all)\n");
			sb.Append("\t\t\tvar xmlhttp = new ActiveXObject(\"Msxml2.XMLHTTP\");\n");
			sb.Append("\t\telse\n");
			sb.Append("\t\t\tvar xmlhttp = new XMLHttpRequest();\n");
			sb.Append("\t\tif(xmlhttp)\n");
			sb.Append("\t\t{\n");
			sb.Append("\t\t\txmlhttp.open(\"POST\", frm.action, false);\n");
			sb.Append("\t\t\tif(useXml)\n");
			sb.Append("\t\t\t\txmlhttp.setRequestHeader(\"Content-Type\", \"text/xml\");\n");
			sb.Append("\t\t\telse\n");
			sb.Append("\t\t\t\txmlhttp.setRequestHeader(\"Content-Type\", frm.enctype);\n");
			sb.Append("\t\t\txmlhttp.send(s);\n");
			sb.Append("\t\t\tfrm.ResponseText = xmlhttp.ResponseText;\n");
			sb.Append("\t\t}\n");
			sb.Append("\t}\n");
			//etc
		}
		*/

		#endregion

		#region Script Generation

//		private string GetScripts()
//		{
//			StringBuilder sb = new StringBuilder();
//			sb.Append("<script language=\"javascript\" type=\"text/javascript\">\n");
//			sb.Append("\t<!--\n");
//			sb.Append("\t/*---------------------------------+\n");
//			sb.Append("\t|State-Of-The-Art ASP.NET Controls |\n");
//			sb.Append("\t|http://www.donhost.ru/            |\n");
//			sb.Append("\t+----------------------------------*/\n");
//			//filllist
//			sb.Append("\tfunction "+FillListFunction+"(frm, lst, url)\n");
//			sb.Append("\t{\n");
//			sb.Append("\t\tlst.options.length = 0;\n");
//			sb.Append("\t\tvar s = \"form=\"+frm.name+\"&action=list\";\n");
//			sb.Append("\t\tif(document.all)\n");
//			sb.Append("\t\t\tvar xmlhttp = new ActiveXObject(\"Msxml2.XMLHTTP\");\n");
//			sb.Append("\t\telse\n");
//			sb.Append("\t\t\tvar xmlhttp = new XMLHttpRequest();\n");
//			sb.Append("\t\tif(xmlhttp)\n");
//			sb.Append("\t\t{\n");
//			sb.Append("\t\t\txmlhttp.open(\"POST\", url, false);\n");
//			sb.Append("\t\t\txmlhttp.setRequestHeader(\"Content-Type\", \"application/x-www-form-urlencoded\");\n");
//			sb.Append("\t\t\txmlhttp.send(s);\n");
//			sb.Append("\t\t\tvar res = xmlhttp.ResponseText;\n");
//			sb.Append("\t\t\tif(res)\n");
//			sb.Append("\t\t\t{\n");
//			sb.Append("\t\t\t\tvar arOptions = res.split(\"&\");\n");
//			sb.Append("\t\t\t\tvar n = arOptions.length;\n");
//			sb.Append("\t\t\t\tfor(var i=0;i<n;i++)\n");
//			sb.Append("\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\ttry\n");
//			sb.Append("\t\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\t\tvar arOption = arOptions[i].split(\"=\");\n");
//			sb.Append("\t\t\t\t\t\tvar opt = document.createElement(\"OPTION\");\n");
//			sb.Append("\t\t\t\t\t\topt.value = arOption[0];\n");
//			sb.Append("\t\t\t\t\t\topt.text = arOption[1];\n");
//			sb.Append("\t\t\t\t\t\tlst.options[lst.options.length] = opt;\n");
//			sb.Append("\t\t\t\t\t}\n");
//			sb.Append("\t\t\t\t\tcatch(ex){}\n");
//			sb.Append("\t\t\t\t}\n");
//			sb.Append("\t\t\t}\n");
//			sb.Append("\t\t}\n");
//			sb.Append("\t}\n");
//			//submit
//			sb.Append("\tfunction "+SubmitFunction+"(frm, url)\n");
//			sb.Append("\t{\n");
//			sb.Append("\t\tvar n = frm.length;\n");
//			sb.Append("\t\tvar s = \"form=\"+frm.name+\"&action=submit\";\n");
//			sb.Append("\t\tfor(var i=0;i<n;i++)\n");
//			sb.Append("\t\t{\n");
//			sb.Append("\t\t\tvar e = frm.elements[i];\n");
//			sb.Append("\t\t\tif(e.name)\n");
//			sb.Append("\t\t\t{\n");
//			sb.Append("\t\t\t\ts+= \"&\"+e.name+\"=\"+encodeURIComponent(e.value);\n");
//			sb.Append("\t\t\t}\n");
//			sb.Append("\t\t}\n");
//			sb.Append("\t\tif(document.all)\n");
//			sb.Append("\t\t\tvar xmlhttp = new ActiveXObject(\"Msxml2.XMLHTTP\");\n");
//			sb.Append("\t\telse\n");
//			sb.Append("\t\t\tvar xmlhttp = new XMLHttpRequest();\n");
//			sb.Append("\t\tif(xmlhttp)\n");
//			sb.Append("\t\t{\n");
//			sb.Append("\t\t\txmlhttp.open(\"POST\", url, false);\n");
//			sb.Append("\t\t\txmlhttp.setRequestHeader(\"Content-Type\", \"application/x-www-form-urlencoded\");\n");
//			sb.Append("\t\t\txmlhttp.send(s);\n");
//			sb.Append("\t\t\tfrm.ResponseText = xmlhttp.ResponseText;\n");
//			sb.Append("\t\t}\n");
//			sb.Append("\t}\n");
//			//setselectvalue
//			sb.Append("\tfunction "+SetSelectValueFunction+"(lst,val)\n");
//			sb.Append("\t{\n");
//			sb.Append("\t\tvar n = lst.options.length;\n");
//			sb.Append("\t\tfor(var i=0;i<n;i++)\n");
//			sb.Append("\t\t{\n");
//			sb.Append("\t\t\tif(lst.options[i].value==val)\n");
//			sb.Append("\t\t\t{\n");
//			sb.Append("\t\t\t\tlst.selectedIndex = i;\n");
//			sb.Append("\t\t\t}\n");
//			sb.Append("\t\t}\n");
//			sb.Append("\t}\n");
//			//refresh
//			sb.Append("\tfunction "+RefreshFunction+"(frm, url)\n");
//			sb.Append("\t{\n");
//			sb.Append("\t\tvar s = \"form=\"+frm.name+\"&action=refresh\";\n");
//			sb.Append("\t\tif(document.all)\n");
//			sb.Append("\t\t\tvar xmlhttp = new ActiveXObject(\"Msxml2.XMLHTTP\");\n");
//			sb.Append("\t\telse\n");
//			sb.Append("\t\t\tvar xmlhttp = new XMLHttpRequest();\n");
//			sb.Append("\t\tif(xmlhttp)\n");
//			sb.Append("\t\t{\n");
//			sb.Append("\t\t\txmlhttp.open(\"POST\", url, false);\n");
//			sb.Append("\t\t\txmlhttp.setRequestHeader(\"Content-Type\", \"application/x-www-form-urlencoded\");\n");
//			sb.Append("\t\t\txmlhttp.send(s);\n");
//			sb.Append("\t\t\tvar res = xmlhttp.ResponseText;\n");
//			sb.Append("\t\t\tif(res)\n");
//			sb.Append("\t\t\t{\n");
//			sb.Append("\t\t\t\tvar arFields = res.split(\"&\");\n");
//			sb.Append("\t\t\t\tvar n = arFields.length;\n");
//			sb.Append("\t\t\t\tfor(var i=0;i<n;i++)\n");
//			sb.Append("\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\ttry\n");
//			sb.Append("\t\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\t\tvar arField = arFields[i].split(\"=\");\n");
//			sb.Append("\t\t\t\t\t\tvar el = frm.elements[arField[0]];\n");
//			sb.Append("\t\t\t\t\t\tvar val = decodeURIComponent(arField[1]);\n");
//			sb.Append("\t\t\t\t\t\tif(el.type)\n");
//			sb.Append("\t\t\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\t\t\tswitch(el.type.toLowerCase())\n");
//			sb.Append("\t\t\t\t\t\t\t{\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"text\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"password\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"submit\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"reset\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"button\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"hidden\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"textarea\":\n");
//			sb.Append("\t\t\t\t\t\t\t\t\tel.value = val;\n");
//			sb.Append("\t\t\t\t\t\t\t\t\tbreak;\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"checkbox\":\n");
//			sb.Append("\t\t\t\t\t\t\t\tcase \"radio\":\n");
//			sb.Append("\t\t\t\t\t\t\t\t\tel.checked = (val==1);\n");
//			sb.Append("\t\t\t\t\t\t\t\t\tbreak;\n");
//			sb.Append("\t\t\t\t\t\t\t}\n");
//			sb.Append("\t\t\t\t\t\t}\n");
//			sb.Append("\t\t\t\t\t\tif(el.tagName.toLowerCase()==\"select\")\n");
//			sb.Append("\t\t\t\t\t\t{XmlForm_SetSelectValue(el,val);}\n");
//			sb.Append("\t\t\t\t\t}\n");
//			sb.Append("\t\t\t\t\tcatch(ex){}\n");
//			sb.Append("\t\t\t\t}\n");
//			sb.Append("\t\t\t}\n");
//			sb.Append("\t\t}\n");
//			sb.Append("\t}\n");
//			//end
//			sb.Append("\t//-->\n");
//			sb.Append("\t</script>\n");
//			return sb.ToString();
//
//		}
//		private void RenderClientScript(HtmlTextWriter writer)
//		{
//			if(!this.Context.Items.Contains("XmlForm_Scripts"))
//			{
//				writer.Write(GetScripts());
//				this.Context.Items["XmlForm_Scripts"]=1;
//			}
//
//		}
//
//		protected override void Render(HtmlTextWriter writer)
//		{
//			if(this.GenerateScript)
//			{
//				RenderClientScript(writer);
//			}
//			base.Render (writer);
//		}
//
//		[DefaultValue(true)]
//		private bool _generateScript = true;
//		public bool GenerateScript
//		{get{return _generateScript;}set{_generateScript = value;}}

		#endregion
	}
}