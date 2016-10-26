using System.Web.UI;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// Used to display pager
	/// </summary>
	public class Pager: Control
	{
		private ISupportsPager _parent = null;
		public ISupportsPager PagedControl
		{
			get
			{
				return _parent;
			}
			set
			{
				_parent = value;
			}
		}
		string _pagedControlID = "";
		public string PagedControlID
		{
			get{return _pagedControlID;}
			set{_pagedControlID = value;}
		}

		protected override void Render(HtmlTextWriter writer)
		{
			if(_parent == null)
			{
				 _parent = (ISupportsPager)Parent.FindControl(_pagedControlID);
			}
			if(_parent != null)
			{
				_parent.CreatePager().RenderControl(writer);
			}
		}

	}

	
	public interface ISupportsPager
	{
		Control CreatePager();
	}
}
