using System.ComponentModel;
using Sota.Web.UI.WebControls;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Repeater для вывода данных их List.
	/// </summary>
	public class ListRepeater : RichRepeater
	{
		private List _list = null;

		[Description("The List object used for reading data."), Category("List"), DefaultValue(null)]
		public List List
		{
			get { return _list; }
			set { _list = value; }
		}
		private string _sort = "";

		[Description("Sort"), Category("List"), DefaultValue("")]
		public string Sort
		{
			get { return _sort; }
			set { _sort = value; }
		}

		[Description("RowFilter"), Category("List"), DefaultValue("")]
		public string Filter
		{
			get { return _filter; }
			set { _filter = value; }
		}

		private string _filter = "";



		private int _argumentItemId = -1;
		[Description("The ID used for reading data when Read is set to ReadType.Item, ReadType.NextItem, ReadType.PrevItem, ReadType.Children."), Category("List"), DefaultValue(-1)]
		public int ArgumentItemID
		{
			get { return _argumentItemId; }
			set { _argumentItemId = value; }
		}

		private string _argumentField = "";
		[Description("The field name used for reading data when Read is set to ReadType.ByField."), Category("List"), DefaultValue("")]
		public string ArgumentField
		{
			get { return _argumentField; }
			set { _argumentField = value; }
		}

		private string _argumentValue = "";
		[Description("The field value used for reading data when Read is set to ReadType.ByField."), Category("List"), DefaultValue("")]
		public string ArgumentValue
		{
			get { return _argumentValue; }
			set { _argumentValue = value; }
		}
		
		private string _argumentSql = "";
		[Description("The sql text used for reading data when Read is set to ReadType.Custom."), Category("List"), DefaultValue("")]
		public string ArgumentSql
		{
			get { return _argumentSql; }
			set { _argumentSql = value; }
		}

		public enum ReadType
		{
			None,
			AllFull,
			RootParentItems,
			ParentItems,
			RootItems,
			Children,
			Item,
			NextItem,
			PrevItem,
			ByField,
			Custom
		}
		private ReadType _readType = ReadType.AllFull;
		[Description("Indicates which method of List to use for reading data."), Category("List"), DefaultValue(ReadType.AllFull)]
		public ReadType Read
		{
			get { return _readType; }
			set { _readType = value; }
		}

		[Description("The name of the List."), Category("List"), DefaultValue(null)]
		public string ListName
		{
			get
			{
				if (_list == null)
				{
					return null;
				}
				return _list.Name;
			}
			set { _list = List.Create(value); }
		}

		public override void DataBind()
		{
			if (_list != null)
			{
				switch(_readType)
				{
					case ReadType.AllFull:
						_list.ReadAllFull();
						break; 
					case ReadType.RootParentItems:
						_list.ReadRootParentItems();
						break; 
					case ReadType.ParentItems:
						_list.ReadParentItems(this._argumentItemId);
						break; 
					case ReadType.RootItems:
						_list.ReadRootItems();
						break; 
					case
					ReadType.Children:
						_list.ReadChildren(this._argumentItemId);
						break; 
					case
					ReadType.Item:
						_list.ReadItem(this._argumentItemId);
						break; 
					case
					ReadType.NextItem:
						_list.ReadNextItem(this._argumentItemId);
						break; 
					case
					ReadType.PrevItem:
						_list.ReadPrevItem(this._argumentItemId);
						break; 
					case
					ReadType.ByField:
						_list.FindByField(this._argumentField, this._argumentValue);
						break; 
					case
					ReadType.Custom:
						_list.Read(this._argumentSql);
						break;
				}
				if(_filter.Length > 0)
				{
					_list.Data.DefaultView.RowFilter = _filter;
				}
				if(_sort.Length > 0)
				{
					_list.Data.DefaultView.Sort = _sort;
				}
				DataSource = _list.Data;
				base.DataBind();
			}
		}
		public new void DataBind(object dataSource)
		{
			this.DataSource = dataSource==null ? this.List.Data : dataSource;
			base.DataBind();
		}
	}
}
