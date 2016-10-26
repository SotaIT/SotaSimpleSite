using System.Data;
using System.Web;
using System.IO;

namespace Sota.Web.SimpleSite.Search
{
	/// <summary>
	/// Поиск по контенту.
	/// </summary>
	public class ContentSearch : Sota.Web.SimpleSite.Search.ISupportsSearch
	{
		public DataTable Search(string text)
		{
			return Search(text, Keys.DefaultSearchResultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public System.Data.DataTable Search(string text, int resultBodyLength)
		{
			return Search(text, resultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public System.Data.DataTable Search(string text, int resultBodyLength, string prefix, string postfix)
		{
			DataTable tb = SearchUtil.CreateResultTable();

			DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Request.MapPath(Config.Main.Data));
			FileInfo[] af = di.GetFiles("*" + Keys.ConfigExtension);
			int n = af.Length;
			for (int i = 0; i < n; i++)
			{
				PageInfo pi = new PageInfo(PageInfo.DotToSlash(af[i].Name.Substring(0, af[i].Name.Length - af[i].Extension.Length)));
				string body = SearchUtil.SearchTheText(pi.Body, text, resultBodyLength, prefix, postfix);
				if (body != null)
				{
					tb.Rows.Add(new object[] {pi.Url, pi.Title == string.Empty ? pi.Url : pi.Title, body});
				}
			}
			return tb;
		}

	}
}