using System.Web.UI;

namespace Sota.Web.UI.WebControls
{
	public class PagerItem : Control, INamingContainer
	{
		public PagerItem(RichRepeater repeater, int pageNumber)
		{
			_repeater = repeater;
			_pageNumber = pageNumber;
			_text = pageNumber.ToString();
		}

		public PagerItem(RichRepeater repeater, int pageNumber, string text)
		{
			_repeater = repeater;
			_pageNumber = pageNumber;
			_text = text;
		}

		private RichRepeater _repeater;

		public int PageSize
		{
			get { return _repeater.PageSize; }
		}

		public int PageCount
		{
			get { return _repeater.PageCount; }
		}

		public int CurrentPageNumber
		{
			get { return _repeater.PageNumber; }
		}

		public bool Selected
		{
			get { return _repeater.PageNumber == _pageNumber; }
		}

		public bool IsAllLabel
		{
			get { return 0 == _pageNumber; }
		}

		private int _pageNumber;

		public int PageNumber
		{
			get { return _pageNumber; }
		}

		private string _text;

		public string Text
		{
			get { return _text; }
		}

		public override string ToString()
		{
			return _text;
		}


	}

	public enum PagerItemType
	{
		Item,
		SelectedItem,
		Header,
		Footer,
		OnePage,
		PrevPage,
		PrevInactivePage,
		NextPage,
		NextInactivePage,
		FirstPage,
		FirstInactivePage,
		LastPage,
		LastInactivePage
	}
}
