using System.Collections;

namespace Sota.Web.SimpleSite.Map
{
	/// <summary>
	/// Коллекция элементов карты сайта.
	/// </summary>
	public class MapItemCollection : ArrayList
	{
		new public MapItem this[int index]
		{
			get { return (MapItem) base[index]; }
			set { base[index] = value; }
		}

		public int Add(MapItem item)
		{
			return base.Add(item);
		}
	}
}