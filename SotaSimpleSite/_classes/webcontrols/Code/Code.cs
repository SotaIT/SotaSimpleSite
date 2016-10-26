using System.ComponentModel;
using System.Web.UI;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// Контрол для осуществления подсветки кода средствами HTML.
	/// </summary>
	public class Code: Control
	{
		public enum CodeLanguage
		{
			HTML,
			XML,
			CS,
			VB,
			Delphi,
			C,
			CPP,
			PHP
		}
		public static string Colorize(string text)
		{
			return Colorize(text, CodeLanguage.HTML, true);
		}
		public static string Colorize(string text, CodeLanguage codeLanguage)
		{
			return Colorize(text, codeLanguage, true);
		}
		public static string Colorize(string text, CodeLanguage codeLanguage, bool usePre)
		{
			//TODO
			return text;
		}
		
		[Bindable(true), Category("Behavior"), DefaultValue("")]
		public string Text
		{
			get
			{
				string text1 = (string) this.ViewState["Text"];
				if (text1 == null)
				{
					return string.Empty;
				}
				return text1;
			}
			set
			{
				this.ViewState["Text"] = value;
			}
		}
		[Bindable(true), Category("Behavior"), DefaultValue(CodeLanguage.HTML)]
		public CodeLanguage Language
		{
			get
			{
				object obj =  this.ViewState["Language"];
				if (obj == null)
				{
					return CodeLanguage.HTML;
				}
				return (CodeLanguage)obj;
			}
			set
			{
				this.ViewState["Language"] = value;
			}
		}
		[Bindable(true), Category("Behavior"), DefaultValue(true)]
		public bool UsePRE
		{
			get
			{
				object obj =  this.ViewState["UsePRE"];
				if (obj == null)
				{
					return true;
				}
				return (bool)obj;
			}
			set
			{
				this.ViewState["UsePRE"] = value;
			}
		}
		
		protected override void Render(HtmlTextWriter writer)
		{
			string text = this.Text;
			if (text.Length != 0)
			{
				writer.Write(Colorize(text, this.Language, this.UsePRE));
			}
		}


	}
}
