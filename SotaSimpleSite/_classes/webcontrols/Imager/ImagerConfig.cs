using System.Collections.Specialized;
using System.Data;

namespace Sota.Web.UI.WebControls
{
	public struct ImagerConfig
	{
		public static ImagerConfig FromQueryString(NameValueCollection source)
		{
			ImagerConfig ic = new ImagerConfig();
            ic.IsEmpty = false;
            ic.NotFoundImage = source["nf"];
			ic.Width = source["w"];
			ic.Height = source["h"];
			ic.BackgroundImage = source["bg"];
			ic.AlignX = source["x"];
			ic.AlignY = source["y"];
			ic.FrontImage = source["fg"];
			ic.FrontAlignX = source["fx"];
			ic.FrontAlignY = source["fy"];
			ic.Format = source["f"];
			ic.CompositingQuality = source["cq"];
			ic.SmoothingMode = source["sm"];
			ic.InterpolationMode = source["im"];
			ic.Crop = source["c"];
			ic.Cache = source["cache"];
			ic.NoCache = source["nocache"];
			ic.BackgroundColor = source["bgc"];
            ic.JpegQuality = source["q"];
            return ic;
		}

        public static ImagerConfig Empty()
        {
            ImagerConfig ic = new ImagerConfig();
            ic.IsEmpty = true;
            return ic;
        }


		public static ImagerConfig FromDataRow(DataRow source)
		{
			ImagerConfig ic = new ImagerConfig();
            ic.IsEmpty = false;
			ic.NotFoundImage = GetString(source, "nf");
			ic.Width = GetString(source,"w");
			ic.Height = GetString(source,"h");
			ic.BackgroundImage = GetString(source,"bg");
			ic.AlignX = GetString(source,"x");
			ic.AlignY = GetString(source,"y");
			ic.FrontImage = GetString(source,"fg");
			ic.FrontAlignX = GetString(source,"fx");
			ic.FrontAlignY = GetString(source,"fy");
			ic.Format = GetString(source,"f");
			ic.CompositingQuality = GetString(source,"cq");
			ic.SmoothingMode = GetString(source,"sm");
			ic.InterpolationMode = GetString(source,"im");
			ic.Crop = GetString(source,"c");
			ic.Cache = GetString(source,"cache");
			ic.NoCache = GetString(source,"nocache");
			ic.BackgroundColor = GetString(source,"bgc");
            ic.JpegQuality = GetString(source, "q");
            return ic;
		}
		private static string GetString(DataRow row, string col)
		{
			if(row.Table.Columns.Contains(col))
			{
				return row[col].ToString();
			}
			return null;
		}

        public bool IsEmpty;

        public string NotFoundImage;//nf
        public string Width;//w
		public string Height;//h
		public string BackgroundImage;//bg
		public string AlignX;//x
		public string AlignY;//y
		public string FrontImage;//fg
		public string FrontAlignX;//fx
		public string FrontAlignY;//fy
		public string Format;//f
		public string CompositingQuality;//cq
		public string SmoothingMode;//sm
		public string InterpolationMode;//im
		public string Crop;//c
		public string Cache;//cache
		public string NoCache;//nocache
		public string BackgroundColor;//bgc
        public string JpegQuality;//q		
    }
}
