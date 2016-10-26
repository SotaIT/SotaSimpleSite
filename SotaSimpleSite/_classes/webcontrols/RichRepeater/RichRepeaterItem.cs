using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Sota.Web.UI.WebControls
{
	/// <summary>
	/// RichRepeaterItem.
	/// </summary>
	public class RichRepeaterItem: RepeaterItem
	{
		public RichRepeaterItem(int itemIndex, ListItemType itemType):base(itemIndex, itemType)
		{
			
		}
		
		public DataRow Row
		{
			get{return (DataRow)this.DataItem;}
		}
		
		public DataRowView RowView
		{
			get{return (DataRowView)this.DataItem;}
		}
		
		public object this[string expression]
		{
			get
			{
				return DataBinder.Eval(this.DataItem, expression);
			}
		}

	}
}