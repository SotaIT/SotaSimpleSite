using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Отображает содержимое в плейсхолдере шаблона, указанном в PlaceHolderID
	/// </summary>
	public class Content: PlaceHolder
	{
		private string _placeHolderID = null;

		public string PlaceHolderID
		{
			get { return _placeHolderID; }
			set { _placeHolderID = value; }
		}

		protected override void CreateChildControls()
		{
			//base.CreateChildControls();
			PlaceHolder content = new PlaceHolder();
			Control placeholder = Page.FindControl(PlaceHolderID);
			for (int i = Controls.Count; i > 0; i--)
			{
				content.Controls.AddAt(0, Controls[i-1]);
			}
			placeholder.Controls.Add(content);
		}

	}
}
