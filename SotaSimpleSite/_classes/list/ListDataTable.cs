using System;
using System.Collections;
using System.Data;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// ListDataTable - List.Data
	/// </summary>
	public class ListDataTable: DataTable, IEnumerable
	{
		public ListDataTable(string name, List list):base(name)
		{
			_list = list;
		}

		private List _list = null;
		public List List
		{
			get
			{
				return _list;
			}
			set
			{
				_list = value;
			}
		}
		public DataRow FirstRow
		{
			get
			{
				if(this.Rows.Count>0)
				{
					return this.Rows[0];
				}
				return null;
			}
		}

		public DataRow FindRow(string filter)
		{
			return FindRow(filter, "");
		}
		public DataRow FindRow(string filter, string sort)
		{
			DataRow[] rows = this.Select(filter, sort);
			if(rows.Length>0)
			{
				return rows[0];
			}
			return null;
		}
		public void ImportRows(params DataRow[] rows)
		{
			for(int i=0;i<rows.Length;i++)
			{
				this.ImportRow(rows[i]);
			}
		}
		
		public DataRow[] GetRows()
		{
			return this.Select(List.FIELD_DELETED + "=0", this._list.TreeSortExpression);
		}
		public string Extended(string columnName, string property)
		{
			return this.Columns[columnName].ExtendedProperties[property].ToString();
		}
		public string Extended(string columnName)
		{
			return Extended(columnName, "extended");
		}
		public string Uploads(string columnName)
		{
			 return Extended(columnName, "uploads");
		 }
		public string Levels(string columnName)
		{
			return Extended(columnName, "levels");
		}
		public string InputType(string columnName)
		{
			return Extended(columnName, "inputtype");
		}
		public string ListUrl(string columnName)
		{
			return Extended(columnName, "listurl");
		}
		public string FileFilter(string columnName)
		{
			return Extended(columnName, "filter");
		}
		public string Regex(string columnName)
		{
			return Extended(columnName, "regex");
		}
		public string OnChange(string columnName)
		{
			return Extended(columnName, "onchange");
		}

		public IEnumerator GetEnumerator()
		{
			return this.Rows.GetEnumerator();
		}

		public int Count
		{
			get
			{
				return Rows.Count;
			}
		}

		public object GetValue(int rowID, string colName)
		{
			return Rows[rowID][colName];
		}

		public int GetInt32(int rowID, string colName)
		{
			return Convert.ToInt32(Rows[rowID][colName]);
		}

		public string GetString(int rowID, string colName)
		{
			return Rows[rowID][colName].ToString();
		}

		public DateTime GetDateTime(int rowID, string colName)
		{
			return Convert.ToDateTime(Rows[rowID][colName]);
		}

		public double GetDouble(int rowID, string colName)
		{
			return Convert.ToDouble(Rows[rowID][colName]);
		}

		public bool GetBoolean(int rowID, string colName)
		{
			return Convert.ToBoolean(Rows[rowID][colName]);
		}
	}
}
