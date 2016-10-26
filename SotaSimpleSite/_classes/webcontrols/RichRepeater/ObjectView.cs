using System;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// Renders an object.
	/// </summary>
	[ParseChildren(true), PersistChildren(false), DefaultProperty("DataSource")]
	public class ObjectView: Control, INamingContainer
	{
		private ITemplate _editItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate EditItemTemplate
		{
			get { return _editItemTemplate; }
			set { _editItemTemplate = value; }
		}

		private ITemplate _itemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate ItemTemplate
		{
			get { return _itemTemplate; }
			set { _itemTemplate = value; }
		}

		private ITemplate _alternatingItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate AlternatingItemTemplate
		{
			get { return _alternatingItemTemplate; }
			set { _alternatingItemTemplate = value; }
		}

		private ITemplate _selectedItemTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate SelectedItemTemplate
		{
			get { return _selectedItemTemplate; }
			set { _selectedItemTemplate = value; }
		}

		private ITemplate _footerTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)]
		public ITemplate FooterTemplate
		{
			get { return _footerTemplate; }
			set { _footerTemplate = value; }
		}

		private ITemplate _headerTemplate;

		[TemplateContainer(typeof(RichRepeaterItem)), Browsable(false),
		PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null)] 
		public ITemplate HeaderTemplate
		{
			get { return _headerTemplate; }
			set { _headerTemplate = value; }
		}

		RichRepeaterItem _item = null;

		public RichRepeaterItem Item
		{
			get { return _item; }
		}

		public override void DataBind()
		{
			this.OnDataBinding(EventArgs.Empty);
		}
		public void DataBind(object dataSource)
		{
			this._dataSource = dataSource;
			DataBind();
		}
		private object _dataSource = null;
		public object DataSource
		{
			get
			{
				return _dataSource;
			}
			set { _dataSource = value;	 }
		}
		protected override void OnDataBinding(EventArgs e)
		{
			base.OnDataBinding(e);
			this.Controls.Clear();
			base.ClearChildViewState();
			this.CreateControl();
			base.ChildControlsCreated = true;
		}

		public void CreateControl()
		{
			RichRepeaterItem item = new RichRepeaterItem(0, _itemType);
			item.DataItem = this.DataSource;
			if(this._headerTemplate != null)
			{
				this._headerTemplate.InstantiateIn(item);
				this.Controls.Add(item);
				item.DataBind();
			}
			switch(_itemType)
			{
				case ListItemType.AlternatingItem:
					this._alternatingItemTemplate.InstantiateIn(item);				
					break;
				case ListItemType.Item:
					this._itemTemplate.InstantiateIn(item);
					break;
				case ListItemType.EditItem:
					this._editItemTemplate.InstantiateIn(item);
					break;
				case ListItemType.SelectedItem:
					this._selectedItemTemplate.InstantiateIn(item);
					break;
			}
			this.Controls.Add(item);
			item.DataBind();
			if(this._footerTemplate != null)
			{
				this._footerTemplate.InstantiateIn(item);
				this.Controls.Add(item);
				item.DataBind();
			}

		}

		private ListItemType _itemType = ListItemType.Item;

		public ListItemType ItemType
		{
			get { return _itemType; }
			set { _itemType = value; }
		}
	}
}
