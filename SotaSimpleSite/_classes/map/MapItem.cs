using System;

namespace Sota.Web.SimpleSite.Map
{
	/// <summary>
	/// Ёлемент карты сайта.
	/// </summary>
	public class MapItem
	{
		internal MapItem()
		{
		}

		internal string _url = string.Empty;
		internal string _text = string.Empty;
		internal string _path = string.Empty;
		internal MapItemType _type = MapItemType.Root;
		internal MapItem _parent = null;
		internal MapItemCollection _items = new MapItemCollection();
		internal int _id = -1;
		internal bool _hidden = false;


		internal void ChangeIndexOf(int index, MapItem item)
		{
			this._items.Remove(item);
			this._items.Insert(index, item);
		}

		internal virtual void Remove()
		{
			this._parent._items.Remove(this);
		}

		internal int Index
		{
			get
			{
				if (IsRoot)
					return -1;
				return this._parent._items.IndexOf(this);
			}
		}

		public int AddChild(string text, string url, string path, MapItemType type, bool hidden)
		{
			MapItem m = new MapItem();
			m._url = url;
			m._text = text;
			m._type = type;
			m._path = path;
			m._hidden = hidden;
			m._parent = this;
			return this._items.Add(m);
		}

		public int ID
		{
			get { return _id; }
		}

		public bool Hidden
		{
			get
			{
				return _hidden;
			}
		}

		public bool IsHidden
		{
			get
			{
				if (IsRoot)
					return false;
				return _hidden || this._parent.IsHidden;
			}
		}

		public string Url
		{
			get { return _url; }
		}

		public string Text
		{
			get { return _text; }
		}

		public string Path
		{
			get { return _path; }
		}

		public MapItemType Type
		{
			get { return _type; }
		}

		internal MapItemType SetType(string type)
		{
			return _type = (MapItemType) Enum.Parse(typeof (MapItemType), type, true);
		}

		public MapItem Parent
		{
			get { return _parent; }
		}

		public MapItemCollection Items
		{
			get { return _items; }
		}

		/// <summary>
		/// явл€етс€ ли корневым элементом
		/// </summary>
		public bool IsRoot
		{
			get { return this._parent == null; }
		}

		/// <summary>
		/// ”ровень вложенности
		/// ƒл€ корневого элемента '-1'
		/// </summary>
		public int Level
		{
			get
			{
				if (IsRoot)
					return -1;
				return this._parent.Level + 1;
			}
		}

		/// <summary>
		///  оллекци€ всех родителей
		/// от корн€ до текущего
		/// </summary>
		public MapItemCollection ItemPath
		{
			get
			{
				MapItemCollection col = new MapItemCollection();
				if (!IsRoot)
				{
					col = this._parent.ItemPath;
				}
				col.Add(this);
				return col;
			}
		}

		public MapItem this[int index]
		{
			get { return this._items[index]; }
		}

		public int Count
		{
			get { return this._items.Count; }
		}

		public bool Selected
		{
			get { return SiteMap.Current.IsSelected(this); }
		}

	}
}