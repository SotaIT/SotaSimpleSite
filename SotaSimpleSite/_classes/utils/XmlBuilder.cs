using System.Text;
using System.Web;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Облегчает построение XML.
	/// </summary>
	public sealed class XmlBuilder
	{
		private XmlBuilder()
		{
		}

		public static void WriteField(HttpResponse r, string key, string value)
		{
			r.Write("<");
			r.Write(key);
			r.Write("><![CDATA[");
			r.Write(value.Replace("]]>", "] ]>"));
			r.Write("]]></");
			r.Write(key);
			r.Write(">");
		}

		public static void WriteListItem(HttpResponse r, string value, string text)
		{
			r.Write("<option ");
			r.Write("value=\"");
			r.Write(Util.EncodeXmlAttributeValue(value));
			r.Write("\" text=\"");
			r.Write(Util.EncodeXmlAttributeValue(text));
			r.Write("\"/>");
		}

		public static void AppendField(StringBuilder sb, string key, string value)
		{
			sb.Append("<");
			sb.Append(key);
			sb.Append("><![CDATA[");
			sb.Append(value.Replace("]]>", "] ]>"));
			sb.Append("]]></");
			sb.Append(key);
			sb.Append(">");
		}

		public static void AppendList(StringBuilder sb, string value, string text)
		{
			sb.Append("<option ");
			sb.Append("value=\"");
			sb.Append(Util.EncodeXmlAttributeValue(value));
			sb.Append("\" text=\"");
			sb.Append(Util.EncodeXmlAttributeValue(text));
			sb.Append("\"/>");
		}
	}
/// <summary>
/// TODO
/// </summary>
	public sealed class JsonBuilder
	{
		private JsonBuilder()
		{
		}

		public static void WriteField(HttpResponse r, string key, string value)
		{
			r.Write("<");
			r.Write(key);
			r.Write("><![CDATA[");
			r.Write(value.Replace("]]>", "] ]>"));
			r.Write("]]></");
			r.Write(key);
			r.Write(">");
		}

		public static void WriteListItem(HttpResponse r, string value, string text)
		{
			r.Write("<option ");
			r.Write("value=\"");
			r.Write(Util.EncodeXmlAttributeValue(value));
			r.Write("\" text=\"");
			r.Write(Util.EncodeXmlAttributeValue(text));
			r.Write("\"/>");
		}

		public static void AppendField(StringBuilder sb, string key, string value)
		{
			sb.Append("<");
			sb.Append(key);
			sb.Append("><![CDATA[");
			sb.Append(value.Replace("]]>", "] ]>"));
			sb.Append("]]></");
			sb.Append(key);
			sb.Append(">");
		}

		public static void AppendList(StringBuilder sb, string value, string text)
		{
			sb.Append("<option ");
			sb.Append("value=\"");
			sb.Append(Util.EncodeXmlAttributeValue(value));
			sb.Append("\" text=\"");
			sb.Append(Util.EncodeXmlAttributeValue(text));
			sb.Append("\"/>");
		}
	}

}