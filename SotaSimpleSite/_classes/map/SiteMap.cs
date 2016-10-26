using System.Collections;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Xml;
using Sota.Web.SimpleSite.Map;
using System.IO;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Карта сайта.
	/// </summary>
	public sealed class SiteMap : MapItem
	{
		public static SiteMap Current
		{
			get
			{
				SiteMap sm = null;
				Cache c = HttpContext.Current.Cache;
				if (c[Keys.CacheSiteMapObject] != null)
				{
					sm = (SiteMap) c[Keys.CacheSiteMapObject];
				}
				else
				{
					string file = HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + "/map.config");
					sm = new SiteMap(file);
					c.Insert(Keys.CacheSiteMapObject, sm, new CacheDependency(file));
				}
				return sm;
			}
		}

		internal override void Remove()
		{
			this._items.Clear();
			this._text = Sota.Web.SimpleSite.Path.GetDefaultSiteName();
			this._url = Sota.Web.SimpleSite.Path.VRoot;
		}
		private string _file = string.Empty;

		internal SiteMap()
		{
			ReadXml(HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + "/map.config"));
		}

		public SiteMap(string file)
		{
			ReadXml(file);
		}

		internal void ReadXml(string file)
		{
			this._text	= Sota.Web.SimpleSite.Path.GetDefaultSiteName();
			this._url	= Sota.Web.SimpleSite.Path.VRoot;
			this._file	= file;
			XmlTextReader oReader = null;
			try
			{
				oReader = new XmlTextReader(_file);
				oReader.WhitespaceHandling = WhitespaceHandling.None;
				string curNodeName = "";
				MapItem curItem = this;
				while (oReader.Read())
				{
					switch (oReader.NodeType)
					{
						case XmlNodeType.Element:
							switch (oReader.Name)
							{
								case "item":
									MapItem newItem = new MapItem();
									newItem._parent = curItem;
									curItem.Items.Add(newItem);
									newItem._id = this._all.Add(newItem);
									curItem = newItem;
									break;
								case "hidden":
									curItem._hidden = true;
									break;
							}
							curNodeName = oReader.Name;
							break;
						case XmlNodeType.Text:
							switch (curNodeName)
							{
								case "text":
									curItem._text = oReader.Value;
									break;
								case "url":
									curItem._url = oReader.Value;
									break;
								case "type":
									curItem.SetType(oReader.Value);
									break;
								case "path":
									curItem._path = oReader.Value;
									this._itemsByPath[curItem._path] = curItem;
									break;
							}
							break;
						case XmlNodeType.EndElement:
							if (oReader.Name == "item")
								curItem = curItem.Parent;
							break;
					}
				}
			}
			finally
			{
				if (oReader != null)
					oReader.Close();
			}
		}


		public void Save()
		{
			Save(this._file);
		}

		public void Save(string file)
		{
			XmlTextWriter w = null;
			try
			{
				w = new XmlTextWriter(file, Encoding.UTF8);
				w.Formatting = Formatting.Indented;

				w.WriteStartDocument();
				WriteXml(this, w);
			}
			finally
			{
				if (w != null)
					w.Close();
			}
		}

		private void WriteXml(MapItem item, XmlTextWriter w)
		{
			if (item._type == MapItemType.Root)
				w.WriteStartElement("root");
			else
				w.WriteStartElement("item");
			if (item._url != string.Empty)
				w.WriteElementString("url", item._url);
			if (item._text != string.Empty)
				w.WriteElementString("text", item._text);
			if (item._path != string.Empty)
				w.WriteElementString("path", item._path);
			if (item._type != MapItemType.Root)
				w.WriteElementString("type", item._type.ToString());
			if (item._hidden)
				w.WriteElementString("hidden", null);
			foreach (MapItem i in item.Items)
				WriteXml(i, w);
			w.WriteEndElement();
		}

		public MapItem GetItemById(int id)
		{
			if (id == -1)
				return this;
			if (id < this._all.Count)
				return this._all[id];
			return null;
		}

		private MapItemCollection _all = new MapItemCollection();

		public MapItemCollection All
		{
			get { return _all; }
		}


		private Hashtable _itemsByPath = new Hashtable();

		public MapItem GetItemByPath(string path)
		{
			if (path == string.Empty)
			{
				if (_itemsByPath["/"] != null)
				{
					return (MapItem) _itemsByPath["/"];
				}
				return this;
			}
			return (MapItem) _itemsByPath[path];
		}

		public MapItem GetCurrentItem()
		{
			return GetItemByPath(PageInfo.Current.FileName);
		}

		public MapItemCollection GetCurrentItemPath()
		{
			return GetCurrentItem().ItemPath;
		}

		public bool IsSelected(MapItem item)
		{
			return this.GetCurrentItemPath().Contains(item);
		}

		public string Title
		{
			get
			{
				string title = this._text;
				string separator = Config.Main.TitleSeparator;
				MapItemCollection mc = GetCurrentItemPath();
				int n = mc.Count;
				for (int i = 1; i < n; i++)
				{
					title += separator + mc[i]._text;
				}
				return title;
			}
		}

		public string Title1
		{
			get
			{
				string title = this._text;
				string separator = Config.Main.TitleSeparator;
				MapItemCollection mc = GetCurrentItemPath();
				int n = mc.Count;
				for (int i = 1; i < n-1; i++)
				{
					title += separator + mc[i]._text;
				}
				return title + separator + PageInfo.Current.Title;
			}
		}
		public string Title2
		{
			get
			{
				string title = this._text;
				string separator = Config.Main.TitleSeparator;
				MapItemCollection mc = GetCurrentItemPath();
				int n = mc.Count;
				for (int i = 1; i < n; i++)
				{
					title += separator + mc[i]._text;
				}
				return PageInfo.Current.Title + separator + title;
			}
		}

		#region utils

		public static string RemoveBadChars(string text)
		{
			if (text == null || text == string.Empty)
				return string.Empty;
			return text.Replace("'", "").Replace("\"", "").Replace("\\", "");
		}

		internal static void RemoveFromSiteMap(string path)
		{
			SiteMap sm = SiteMap.Current;
			MapItem m = sm.GetItemByPath(path);
			if (m != null)
				m.Remove();
			sm.Save();
		}

		public void SearchForNewPages()
		{
			DirectoryInfo di = new DirectoryInfo(HttpContext.Current.Request.MapPath(Config.Main.Data));
			FileInfo[] afi = di.GetFiles("*" + Keys.ConfigExtension);
			int afil = afi.Length;
			for (int j = 0; j < afil; j++)
			{
				if (!afi[j].Name.StartsWith("admin."))
				{
					string path = PageInfo.DotToSlash(afi[j].Name.Substring(0, afi[j].Name.Length - afi[j].Extension.Length));
					MapItem m = GetItemByPath(path);
					if (m == null || m.Type == MapItemType.Link)
						AddToSiteMap(path);
				}
			}
			Save();
		}

		internal void AddToSiteMap(string path)
		{
			PageInfo pi = new PageInfo(path);
			MapItem m = this.GetItemByPath(path);
			if (m == null)
			{
				MapItem p = this;
				string[] parts = path.Split('/');
				if (parts.Length > 1)
				{
					string folder = string.Empty;
					for (int i = 0; i < parts.Length - 1; i++)
					{
						folder += "/" + parts[i];
						folder = folder.TrimStart('/');
						if (this.GetItemByPath(folder) == null)
						{
							MapItem mp = new MapItem();
							mp._text = RemoveBadChars(PageInfo.CreateName(folder));
							mp._path = folder;
							mp._type = MapItemType.Link;
							mp._url = Sota.Web.SimpleSite.Path.VRoot + folder;
							mp._hidden = pi.Deleted;
							p.Items.Add(mp);
							this._all.Insert(p.ID + 1, mp);
							this._itemsByPath[folder] = mp;
							p = mp;
						}
						else
						{
							p = this.GetItemByPath(folder);
						}
					}
				}
				else
				{
					p = this.GetItemByPath(string.Empty);
				}
				m = new MapItem();
				p.Items.Add(m);
				this._all.Insert(p.ID + 1, m);
				this._itemsByPath[pi.FileName] = m;
			}
			m._text = RemoveBadChars(pi.Title);
			m._path = pi.FileName;
			m._type = MapItemType.Page;
			m._url = pi.VUrl;
			if (pi.Deleted)
				m._hidden = true;
		}

		internal void SaveToSiteMap(string path)
		{
			AddToSiteMap(path);
			Save();
		}
		
		public void CorrectUrls()
		{
			this._url	= Sota.Web.SimpleSite.Path.VRoot;
			for(int i=0;i<this.All.Count;i++)
			{
				if(this.All[i].Type==MapItemType.Page)
				{
					PageInfo pi = new PageInfo(this.All[i].Path);
	
					this.All[i]._url = pi.VUrl;
				}
			}
			Save();
		}

		#endregion

	}
}