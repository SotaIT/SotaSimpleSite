using System.Data;
using System.Text.RegularExpressions;

namespace Sota.Web.SimpleSite
{

    public sealed class BBParser
    {

		private static DataTable GetBBTable()
        {
            return Config.GetConfigTable("bbparser.config", "bb");
        }

        private static DataTable GetIconsTable()
        {
            return Config.GetConfigTable("bbparser.config", "icon");
        }

        public static string Parse(string text)
        {
			if(Util.IsBlank(text))
			{
				return string.Empty;
			}
			RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Compiled;
			//сначала смайлы
			DataTable tbSmile = GetIconsTable();
			for(int i=0;i<tbSmile.Rows.Count;i++)
			{
				text = Regex.Replace(
					text,
					tbSmile.Rows[i]["code"].ToString(),
					string.Format("<img src=\"{2}{0}\" alt=\"{1}\" style=\"display:inline;border:0;\" />", tbSmile.Rows[i]["src"], tbSmile.Rows[i]["alt"], Path.ARoot),
					options);
			}
			DataTable tbBB = GetBBTable();
			for(int i=0;i<tbBB.Rows.Count;i++)
			{
				string temp = "";
				while(text!=temp)
				{
					temp = text;
					text = Regex.Replace(
						temp,
						tbBB.Rows[i]["pattern"].ToString(),
						tbBB.Rows[i]["replacement"].ToString(),
						options);
				}
			}
			return text;
        }
    }
}

