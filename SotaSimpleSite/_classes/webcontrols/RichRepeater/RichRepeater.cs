using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// A rich repeater control wich extends the System.Web.UI.WebControls.Repeater
	/// </summary>
	public class RichRepeater : System.Web.UI.WebControls.Repeater, ISupportsPager
	{
		#region eval helpers
		public static object Eval(object container, string expression)
		{
			return DataBinder.Eval(container, "DataItem."+expression);
		}
		public static object Eval(object container, string expression, string format)
		{
			return DataBinder.Eval(container, "DataItem."+expression, format);
		}
		public static int EvalInt(object container, string expression)
		{
			return EvalInt(container, expression, -1);
		}
		public static int EvalInt(object container, string expression, int nullValue)
		{
			object val = Eval(container, expression);
			if(val==null)
			{
				return nullValue;
			}
			return Convert.ToInt32(val);		
		}
		public static string EvalString(object container, string expression)
		{
			object val = Eval(container, expression);
			if(val==null)
			{
				return "";
			}
			return val.ToString();
		}
		public static DateTime EvalDateTime(object container, string expression)
		{
			object val = Eval(container, expression);
			if(val==null)
			{
				return DateTime.MinValue;
			}
			return Convert.ToDateTime(val);
		}
		#endregion

		private RichRepeaterItem CreateItem(int itemIndex, ListItemType itemType, object dataItem)
		{
			RichRepeaterItem item1 = new RichRepeaterItem(itemIndex, itemType);
			RepeaterItemEventArgs args1 = new RepeaterItemEventArgs(item1);
			this.InitializeItem(item1);
			item1.DataItem = dataItem;
			this.OnItemCreated(args1);
			this.Controls.Add(item1);
			item1.DataBind();
			this.OnItemDataBound(args1);
			item1.DataItem = null;
			return item1;
		}

		protected override void InitializeItem(RepeaterItem item)
		{
			if(item.ItemType == ListItemType.EditItem && this._editItemTemplate != null)
			{
				this._editItemTemplate.InstantiateIn(item);
				return;
			}
			if(this._beforeAnyItemTemplate != null && (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem))
			{
				this._beforeAnyItemTemplate.InstantiateIn(item);
			}
			base.InitializeItem (item);
			if(this._afterAnyItemTemplate!=null && (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem))
			{
				this._afterAnyItemTemplate.InstantiateIn(item);
			}
		}

		private bool _autoHidePager = true;

		[Description("Indicates whethere to hide pager when there is no item."), Category("Pager"), DefaultValue(true)]
		public bool AutoHidePager
		{
			get { return _autoHidePager; }
			set { _autoHidePager = value; }
		}

		private DisplayPager _showPager = DisplayPager.None;

		[Description("Indicates where to show pager."), Category("Pager"), DefaultValue(DisplayPager.None)]
		public DisplayPager DisplayPager
		{
			get { return _showPager; }
			set { _showPager = value; }
		}

		private string _allText = "Все";

		[Description("All label text."), Category("Pager"), DefaultValue("Все")]
		public string AllText
		{
			get { return _allText; }
			set { _allText = value; }
		}

		private string _prevPageText = "&laquo;";

		[Description("Prev page text."), Category("Pager"), DefaultValue("&laquo;")]
		public string PrevPageText
		{
			get { return _prevPageText; }
			set { _prevPageText = value; }
		}

		private string _nextPageText = "&raquo;";

		[Description("Next page text."), Category("Pager"), DefaultValue("&raquo;")]
		public string NextPageText
		{
			get { return _nextPageText; }
			set { _nextPageText = value; }
		}

		private string _firstPageText = "&laquo;&laquo;";

		[Description("First page text."), Category("Pager"), DefaultValue("&laquo;&laquo;")]
		public string FirstPageText
		{
			get { return _firstPageText; }
			set { _firstPageText = value; }
		}

		private string _lastPageText = "&raquo;&raquo;";

		[Description("Last page text."), Category("Pager"), DefaultValue("&raquo;&raquo;")]
		public string LastPageText
		{
			get { return _lastPageText; }
			set { _lastPageText = value; }
		}



		private DisplayPager _allLabel = DisplayPager.None;

		[Description("Indicates how to render an item with PageSize=0."), Category("Pager"), DefaultValue(DisplayPager.None)]
		public DisplayPager DisplayAllLabel
		{
			get { return _allLabel; }
			set { _allLabel = value; }
		}

		private int _pageSize = 0;

		[Description("The number of items on a page."), Category("Pager"), DefaultValue(0)]
		public int PageSize
		{
			get { return _pageSize; }
			set { _pageSize = value; }
		}
		
		private int _editItemIndex = -1;
		
		[Browsable(false)]
		public int EditItemIndex
		{
			get { return _editItemIndex; }
			set { _editItemIndex = value; }
		}

		private int _pageCount = 1;

		[Browsable(false)]
		public int PageCount
		{
			get { return _virtualPageCount==-1 ? _pageCount : _virtualPageCount; }
		}
		
		private int _virtualPageCount = -1;

		[Browsable(false)]
		public int VirtualPageCount
		{
			get { return _virtualPageCount; }
			set { _virtualPageCount = value; }
		}

		private int _virtualItemCount = -1;

		[Browsable(false)]
		public int VirtualItemCount
		{
			get { return _virtualItemCount; }
			set 
			{
				_virtualItemCount = value;
				VirtualPageCount = this.GetPageCount(_virtualItemCount);
			}
		}

		private int _pageNumber = 1;

		[Description("Current page number."), Category("Pager"), DefaultValue(1)]
		public int PageNumber
		{
			get { return _pageNumber; }
			set
			{
				_pageNumber = Math.Max(value, 1);
				if (value == 0)
					_pageSize = 0;
			}
		}
		
		private string _qs = "page";

		[Description("The name of the query string parameter containing page number."), Category("Pager"), DefaultValue("page")]
		public string QueryStringPageNumber
		{
			get { return _qs; }
			set
			{ 
				if(value!=null)
				{
					_qs = value.Trim();
				}
				else
				{
					_qs = "page";
				}
			}
		}

		private int _visiblePageCount = 0;

		[Description("Indicates how many maximum pages links are displayed on the pager."), Category("Pager"), DefaultValue(0)]
		public int VisiblePageCount
		{
			get { return _visiblePageCount; }
			set { _visiblePageCount = value; }
		}


		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public new ITemplate ItemTemplate
		{
			get { return base.ItemTemplate; }
			set { base.ItemTemplate = value; }
		}
		
		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public new ITemplate AlternatingItemTemplate
		{
			get { return base.AlternatingItemTemplate; }
			set { base.AlternatingItemTemplate = value; }
		}

		private ITemplate _pagerItemTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PagerItemTemplate
		{
			get { return _pagerItemTemplate; }
			set { _pagerItemTemplate = value; }
		}

		private ITemplate _pagerSelectedItemTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PagerSelectedItemTemplate
		{
			get { return _pagerSelectedItemTemplate; }
			set { _pagerSelectedItemTemplate = value; }
		}



		private ITemplate _prevPageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PrevPageTemplate
		{
			get { return _prevPageTemplate; }
			set { _prevPageTemplate = value; }
		}

		private ITemplate _prevInactivePageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PrevInactivePageTemplate
		{
			get { return _prevInactivePageTemplate; }
			set { _prevInactivePageTemplate = value; }
		}


		private ITemplate _nextInactivePageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate NextInactivePageTemplate
		{
			get { return _nextInactivePageTemplate; }
			set { _nextInactivePageTemplate = value; }
		}

		private ITemplate _nextPageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate NextPageTemplate
		{
			get { return _nextPageTemplate; }
			set { _nextPageTemplate = value; }
		}

		private ITemplate _firstInactivePageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate FirstInactivePageTemplate
		{
			get { return _firstInactivePageTemplate; }
			set { _firstInactivePageTemplate = value; }
		}

		private ITemplate _firstPageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate FirstPageTemplate
		{
			get { return _firstPageTemplate; }
			set { _firstPageTemplate = value; }
		}

		private ITemplate _lastInactivePageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate LastInactivePageTemplate
		{
			get { return _lastInactivePageTemplate; }
			set { _lastInactivePageTemplate = value; }
		}

		private ITemplate _lastPageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate LastPageTemplate
		{
			get { return _lastPageTemplate; }
			set { _lastPageTemplate = value; }
		}



		private ITemplate _pagerHeaderTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PagerHeaderTemplate
		{
			get { return _pagerHeaderTemplate; }
			set { _pagerHeaderTemplate = value; }
		}

		private ITemplate _pagerFooterTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate PagerFooterTemplate
		{
			get { return _pagerFooterTemplate; }
			set { _pagerFooterTemplate = value; }
		}

		private ITemplate _onePageTemplate;

		[TemplateContainer(typeof(PagerItem)), Browsable(false),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate OnePageTemplate
		{
			get { return _onePageTemplate; }
			set { _onePageTemplate = value; }
		}

		private ITemplate _noItemTemplate;

		[Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate NoItemTemplate
		{
			get { return _noItemTemplate; }
			set { _noItemTemplate = value; }
		}
		
		private ITemplate _editItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate EditItemTemplate
		{
			get { return _editItemTemplate; }
			set { _editItemTemplate = value; }
		}
		
		private ITemplate _afterAnyItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate AfterAnyItemTemplate
		{
			get { return _afterAnyItemTemplate; }
			set { _afterAnyItemTemplate = value; }
		}
		
		private ITemplate _beforeAnyItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate BeforeAnyItemTemplate
		{
			get { return _beforeAnyItemTemplate; }
			set { _beforeAnyItemTemplate = value; }
		}

		public override RepeaterItemCollection Items
		{
			get { return _items; }
		}

		private RepeaterItemCollection _items;

		private void CreateItems(bool useDataSource)
		{
			ArrayList ar = new ArrayList();
			if (useDataSource)
			{
				IEnumerable e = null;
				if (this.DataSource is IEnumerable)
				{
					e = (IEnumerable)this.DataSource;
				}
				else
				{
					if (this.DataSource is DataTable)
					{
						e = ((DataTable)DataSource).DefaultView;
					}
					else if (this.DataSource is DataSet)
					{
						if (this.DataMember == string.Empty)
							e = ((DataSet)DataSource).Tables[0].DefaultView;
						else
							e = ((DataSet)DataSource).Tables[this.DataMember].DefaultView;
					}
				}
				foreach (object obj in e)
				{
					ar.Add(obj);
				}
				this.ViewState["ItemsArrayList"] = ar;
			}
			else
			{
				ar = (ArrayList)this.ViewState["ItemsArrayList"];
			}
			if (ar.Count == 0)
			{
				_items = new RepeaterItemCollection(ar);
				return;
			}
			int i1 = 0;
			int i2 = ar.Count;
			int pages = 1;
			int page = this.PageNumber;
			int count = this.PageSize == 0 ? i2 : Math.Min(this.PageSize, i2);
			int res;
			pages = Math.DivRem(i2, count, out res);
			if (res > 0)
				pages++;
			page = Math.Min(Math.Max(page, 1), pages);

			this._pageCount = pages;

			if (_showPager == DisplayPager.Before
				|| _showPager == DisplayPager.Both)
			{
				this.Controls.Add(CreatePager());
			}

			if (this.HeaderTemplate != null)
			{
				this.CreateItem(-1, ListItemType.Header, null);
			}
			i1 = count * (page - 1);
			i2 = Math.Min(i1 + count, i2);
			ArrayList ar2 = new ArrayList();
			for (int i = i1; i < i2; i++)
			{
				if (this.SeparatorTemplate != null && (i > i1))
				{
					this.CreateItem(i - 1, ListItemType.Separator, null);
				}
				ListItemType itemType = ((i % 2) == 0) ? ListItemType.Item : ListItemType.AlternatingItem;
				if(i == this.EditItemIndex)
				{
					itemType = ListItemType.EditItem;
				}
				ar2.Add(this.CreateItem(i, itemType, ar[i]));
			}
			_items = new RepeaterItemCollection(ar2);
			if (this.FooterTemplate != null)
			{
				this.CreateItem(-1, ListItemType.Footer, null);
			}
			if (_showPager == DisplayPager.After 
				|| _showPager == DisplayPager.Both)
			{
				this.Controls.Add(CreatePager());
			}
		}
		protected override void CreateControlHierarchy(bool useDataSource)
		{
			if(_qs.Length>0)
			{
				try
				{
					if(Page.Request.QueryString[_qs]!=null)
					{
						this.PageNumber = int.Parse(Page.Request.QueryString[_qs]);
					}
				}
				catch{}
			}
			CreateItems(useDataSource);
			if (this.Items.Count == 0)
			{
				if (this._noItemTemplate != null)
				{
					this.Controls.Clear();
					if (!this._autoHidePager && (_showPager == DisplayPager.Before
						|| _showPager == DisplayPager.Both))
					{
						this.Controls.Add(CreatePager());
					}
					Control c = new Control();
					_noItemTemplate.InstantiateIn(c);
					this.Controls.Add(c);
					if (!this._autoHidePager && (_showPager == DisplayPager.After
						|| _showPager == DisplayPager.Both))
					{
						this.Controls.Add(CreatePager());
					}
				}
			}
		}
		public string PagerToString()
		{
			Control c = CreatePager();
			StringWriter s = new StringWriter();
			c.RenderControl(new HtmlTextWriter(s));
			return s.ToString();

		}
		public Control CreatePager()
		{
			Control pager = new Control();
			/*
			 * При отображении пейджера ДО репитера, эти проверки прячут пейджер, т.к. элементов Items ещё нет 
			 if(this.Items==null)
			{
				return pager;
			}
			if(this.Items.Count==0 && this._autoHidePager)
			{
				return pager;
			}*/
			//One Page
			if (this.PageCount == 1 && _onePageTemplate != null)
			{
				pager.Controls.Add(CreatePagerItem(PagerItemType.OnePage, -1, ""));
				return pager;
			}

			//Header
			if (_pagerHeaderTemplate != null)
			{
				pager.Controls.Add(CreatePagerItem(PagerItemType.Header, -1, ""));
			}

			//First Page
			if(this._firstPageTemplate != null)
			{
				if(this.PageNumber == 1)
				{
					pager.Controls.Add(CreatePagerItem(
						this._firstInactivePageTemplate == null
							? PagerItemType.FirstPage
							: PagerItemType.FirstInactivePage,
						1, 
						this.FirstPageText));
				}
				else
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.FirstPage, 1, this.FirstPageText));
				}
			}

			//Prev Page
			if(this._prevPageTemplate != null)
			{
				if(this.PageNumber == 1)
				{
					pager.Controls.Add(CreatePagerItem(
						this._prevInactivePageTemplate == null
							? PagerItemType.PrevPage
							: PagerItemType.PrevInactivePage, 
						1, 
						this.PrevPageText));
				}
				else
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.PrevPage, this.PageNumber - 1, this.PrevPageText));
				}
			}

			//Items
			int startingPage = 1;
			int endingPage = this.PageCount;
			if(this.VisiblePageCount > 0)
			{
				startingPage = this.PageNumber - (int)Math.Round(this.VisiblePageCount/2.0);
				if(startingPage < 1)
				{
					startingPage = 1;
				}
				endingPage = startingPage + this.VisiblePageCount - 1;
				if(endingPage > this.PageCount)
				{
					endingPage = this.PageCount;
				}
			}
			if (_pagerItemTemplate != null)
			{
				if (_allLabel == DisplayPager.Before 
					|| _allLabel == DisplayPager.Both)
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.Item, 0, this.AllText));
				}

				for (int i = startingPage; i < endingPage + 1; i++)
				{
					if (i == this.PageNumber && _pagerSelectedItemTemplate != null)
					{
						pager.Controls.Add(CreatePagerItem(PagerItemType.SelectedItem, i, i.ToString()));
					}
					else
					{
						pager.Controls.Add(CreatePagerItem(PagerItemType.Item, i, i.ToString()));
					}
				}

				if (_allLabel == DisplayPager.After
					|| _allLabel == DisplayPager.Both)
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.Item, 0, this.AllText));
				}
			}
			else
			{
				if(_qs.Length>0)
				{
					string format = " <a href=\"?"+this._qs+"={0}\">{1}</a> ";
					StringBuilder sb = new StringBuilder();
					sb.Append("<div>");
					if (_allLabel == DisplayPager.Before 
						|| _allLabel == DisplayPager.Both)
					{
						sb.AppendFormat(format, 0, this.AllText);
					}
					for (int i = startingPage; i < endingPage + 1; i++)
					{
						if (i == this.PageNumber)
						{
							sb.AppendFormat(" <b>{0}</b> ", i);
						}
						else
						{
							sb.AppendFormat(format, i, i);
						}
					}
					if (_allLabel == DisplayPager.After
						|| _allLabel == DisplayPager.Both)
					{
						sb.AppendFormat(format, 0, this.AllText);
					}
					sb.Append("</div>");
					Literal l = new Literal();
					l.Text = sb.ToString();
					pager.Controls.Add(l);
				}
				
			}
			
			//Next Page
			if(this._nextPageTemplate != null)
			{
				if(this.PageNumber == this.PageCount)
				{
					pager.Controls.Add(CreatePagerItem(
						this._nextInactivePageTemplate == null
							? PagerItemType.NextPage
							: PagerItemType.NextInactivePage,
						this.PageCount, 
						this.NextPageText));
				}
				else
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.NextPage, this.PageNumber + 1, this.NextPageText));
				}
			}
			//Last Page
			if(this._lastPageTemplate != null)
			{
				if(this.PageNumber == this.PageCount)
				{
					pager.Controls.Add(CreatePagerItem(
						this._lastInactivePageTemplate == null
							? PagerItemType.LastPage
							: PagerItemType.LastInactivePage, 
						this.PageCount, 
						this.LastPageText));
				}
				else
				{
					pager.Controls.Add(CreatePagerItem(PagerItemType.LastPage, this.PageCount, this.LastPageText));
				}
			}

			
			//Footer
			if (_pagerFooterTemplate != null)
			{
				pager.Controls.Add(CreatePagerItem(PagerItemType.Footer, -1, ""));
			}

			return pager;
		}
		private PagerItem CreatePagerItem(PagerItemType itemType, int pageNumber, string text)
		{
			PagerItem i = new PagerItem(this, pageNumber, text);

			switch (itemType)
			{
				case PagerItemType.Item:
					_pagerItemTemplate.InstantiateIn(i);
					break;
				case PagerItemType.SelectedItem:
					_pagerSelectedItemTemplate.InstantiateIn(i);
					break;
				case PagerItemType.Header:
					_pagerHeaderTemplate.InstantiateIn(i);
					break;
				case PagerItemType.Footer:
					_pagerFooterTemplate.InstantiateIn(i);
					break;
				case PagerItemType.OnePage:
					_onePageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.PrevPage:
					_prevPageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.PrevInactivePage:
					_prevInactivePageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.NextPage:
					_nextPageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.NextInactivePage:
					_nextInactivePageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.FirstPage:
					_firstPageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.FirstInactivePage:
					_firstInactivePageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.LastPage:
					_lastPageTemplate.InstantiateIn(i);
					break;
				case PagerItemType.LastInactivePage:
					_lastInactivePageTemplate.InstantiateIn(i);
					break;

			}
			if (pageNumber > -1)
			{
				i.DataBind();
			}
			return i;
		}
		public void DataBind(object dataSource)
		{
			this.DataSource = dataSource;
			base.DataBind();
		}

		public static int GetPageCount(int itemCount, int pageSize)
		{
			int pages = 1;
			int res;
			pages = Math.DivRem(itemCount, pageSize, out res);
			if (res > 0)
				pages++;
			return pages;
		}
		public int GetPageCount(int itemCount)
		{
			return GetPageCount(itemCount, this.PageSize);
		}
	}
}