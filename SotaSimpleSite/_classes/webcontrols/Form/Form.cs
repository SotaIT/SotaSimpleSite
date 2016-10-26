using System.ComponentModel;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Форма HTML
	/// </summary>
	public class Form : System.Web.UI.HtmlControls.HtmlForm
	{
		protected override void RenderAttributes(System.Web.UI.HtmlTextWriter writer)
		{
			string s = string.Empty;
			string action = Sota.Web.SimpleSite.Path.Full;
			StringWriter w = new StringWriter();
			try
			{
				base.RenderAttributes(new HtmlTextWriter(w));
				s = w.ToString();
			}
			finally
			{
				if (w != null)
				{
					w.Close();
				}
			}
			s = Regex.Replace(s, "action=\\\"[^\\\"]+\\\"", "action=\"" + action + "\"");
			writer.Write(s);
		}
	}
}