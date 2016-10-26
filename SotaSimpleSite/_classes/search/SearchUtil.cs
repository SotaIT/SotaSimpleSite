using System;
using System.Data;
using System.Text.RegularExpressions;

namespace Sota.Web.SimpleSite.Search
{
	/// <summary>
	/// Вспомогательные методы.
	/// </summary>
	public sealed class SearchUtil
	{
		private SearchUtil()
		{
		}
		public static void ImportRow(DataRow row, DataTable tb)
		{
			DataRow r = tb.NewRow();
			for(int i=0;i<tb.Columns.Count;i++)
			{
				r[tb.Columns[i].ColumnName] = row[tb.Columns[i].ColumnName];
			}
			tb.Rows.Add(r);
		}

		public static DataTable FullSearch(string text)
		{
			return FullSearch(text, Keys.DefaultSearchResultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public static DataTable FullSearch(string text, int resultBodyLength)
		{
			return FullSearch(text, resultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public static DataTable FullSearch(string text, int resultBodyLength, string prefix, string postfix)
		{
				DataTable tb = CreateResultTable();
			string[] arModules = Config.Search.GetConfig("modules").Split(';');
			int n = arModules.Length;
			for (int i = 0; i < n; i++)
			{
				string[] t = arModules[i].Split(',');
				DataTable tb1 = ((ISupportsSearch) (Activator.CreateInstance(t[0].Trim(), t[1].Trim()).Unwrap())).Search(text, resultBodyLength, prefix, postfix);
				int m = tb1.Rows.Count;
				for (int j = 0; j < m; j++)
				{
					ImportRow(tb1.Rows[j],tb);
				}
			}
			return tb;
		}

		public static string SearchTheText(string body, string text)
		{
			return SearchTheText(body, text, Keys.DefaultSearchResultBodyLength);
		}
		public static string SearchTheText(string body, string text, int resultBodyLength)
		{
			return SearchTheText(body, text, resultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}
		public static string SearchTheText(string body, string text, int resultBodyLength, string prefix, string postfix)
		{
			if (text == null || body == null)
				return null;
			text = text.Trim();
			body = body.Trim();
			if (text.Length == 0 || body.Length == 0)
				return null;
			Regex rex = new Regex("<[^>]+>");
			string newbody = rex.Replace(BasePage.ClearMetaLanguage(body), " ").Trim().Replace('\t',' ').Replace('\n',' ').Replace('\r',' ');
			if (newbody.Length == 0)
				return null;
			string lowbody = newbody.ToLower();
			int j = lowbody.IndexOf(text.ToLower());
			if (j != -1)
			{
				string notEmptyPrefix = "";
				string notEmptyPostfix = "";
				int before = resultBodyLength / 2;
				if(j > before)
				{
					before = newbody.LastIndexOf(' ', j - before);
					if(before == -1)
					{
						before = j;
					}
					else
					{
						before = j - before;
						notEmptyPrefix = prefix;
					}
				}
				else
				{
					before = j;
				}

				int after = resultBodyLength - before - text.Length;
				if(j + text.Length + after < newbody.Length)
				{
					after = newbody.IndexOf(' ', j + text.Length + after);
					if(after==-1)
					{
						after = newbody.Length - text.Length - j;
					}
					else
					{
						after = after - text.Length - j;
						notEmptyPostfix = postfix;
					}
				}
				else
				{
					after = newbody.Length - text.Length - j;
				}
				return notEmptyPrefix + newbody.Substring(j - before, after + text.Length + before) + notEmptyPostfix;
			}
			return null;
		}

		public static DataTable CreateResultTable()
		{
			DataTable tb = new DataTable();
			tb.Columns.Add("href");
			tb.Columns.Add("title");
			tb.Columns.Add("body");
			return tb;
		}
	}
}