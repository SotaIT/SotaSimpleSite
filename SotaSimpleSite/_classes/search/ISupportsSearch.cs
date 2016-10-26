namespace Sota.Web.SimpleSite.Search
{
	/// <summary>
	/// ”казывает, что класс поддерживает поиск
	/// </summary>
	public interface ISupportsSearch
	{
		System.Data.DataTable Search(string text);
		System.Data.DataTable Search(string text, int resultBodyLength);
		System.Data.DataTable Search(string text, int resultBodyLength, string prefix, string postfix);
	}
}