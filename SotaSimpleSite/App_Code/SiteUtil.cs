using Sota.Web.SimpleSite;
using System.Collections;
using System.Web;

/// <summary>
/// Вспомогательные функции
/// </summary>
public sealed class SiteUtil
{
	const string ReplaceCurrentCrumbsKey = "ReplaceCurrentCrumbs";
	public static void ReplaceCurrentCrumbs(ArrayList arr)
	{
		HttpContext.Current.Items[ReplaceCurrentCrumbsKey] = arr;
	}
	public static ArrayList GetReplaceCurrentCrumbs()
	{
		return HttpContext.Current.Items.Contains(ReplaceCurrentCrumbsKey)
			? (ArrayList)HttpContext.Current.Items[ReplaceCurrentCrumbsKey] 
			: null;
	}
}
