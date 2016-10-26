using System.Web.UI;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Отображает метку из словаря.
	/// </summary>
	public class ContentPlaceHolder: Control
	{
		private string _field = null;

		public string Field
		{
			get { return _field; }
			set { _field = value; }
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if(!Util.IsNullOrEmpty(_field))
			{
				writer.Write(PageInfo.Current.Fields[_field]);
			}
		}

	}
}
