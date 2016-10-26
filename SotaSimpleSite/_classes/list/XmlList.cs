using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.Text;
using sql = System.Data.SqlClient;
using System.Data.SqlClient;
using System.Web;


/* Все данные хранятся в одном файе
 * */
namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// List настроенный на оптимальную работу с XML файлом
	/// </summary>
	public sealed class XmlList: List
	{
		public XmlList()
		{
		}

		public XmlList(string name)
		{
			Init(name, false);
		}

		public XmlList(string name, bool full)
		{
			Init(name, full);
		}

		public override void VerifyDataStructure()
		{
			Save();
		}
		private DataSet ReadData(string file)
		{
			DataSet ds = new DataSet();
			ds.Locale = CultureInfo.InvariantCulture;
			ds.ReadXml(file);
			return ds;
		}
		#region read
		private void ReadItemsFromRows(DataRow[] rows, int from, int count)
		{
			CultureInfo ci = CultureInfo.InvariantCulture;
			int n = from;
			int m = count == 0 ? rows.Length : count + from;
			for(int j = n; j < m; j++)
			{
				if(this.tbData.FindRow(List.FIELD_ID + "=" + rows[j][List.FIELD_ID ]) == null)
				{

					DataRow row = this.tbData.NewRow();
					for(int i = 0; i < this.tbData.Columns.Count; i++)
					{
						string col = this.tbData.Columns[i].ColumnName;
						if(rows[j].Table.Columns.Contains(col))
						{
							if(this.tbData.Columns[i].DataType==typeof(DateTime))
							{
								row[col] = Convert.ToDateTime(rows[j][col], ci);
							}
							else if(this.tbData.Columns[i].DataType==typeof(int))
							{
								row[col] = Convert.ToInt32(rows[j][col], ci);
							}
							else if(this.tbData.Columns[i].DataType==typeof(bool))
							{
								row[col] = Convert.ToBoolean(rows[j][col], ci);
							}
							else if(this.tbData.Columns[i].DataType==typeof(double))
							{
								row[col] = Convert.ToDouble(rows[j][col], ci);
							}
							else
							{
								row[col] = rows[j][col];
							}
						}
					}
					this.tbData.Rows.Add(row);
				}
			}
		}

		public override ListDataTable ReadAllFull()
		{
			return Read("");
		}

		public override ListDataTable Read(string selectSql)
		{
			return Read(selectSql, "");
		}

		private ListDataTable Read(string select, string sort)
		{
			return Read(select, sort, 0, 0);
		}
		private ListDataTable Read(string select, string sort, int count)
		{
			return Read(select, sort, 0, count);
		}
		private ListDataTable Read(string select, string sort, int from, int count)
		{
			DataSet ds = ReadData(HttpContext.Current.Request.MapPath(this._connectionString));
			if(ds.Tables.Count > 0)
			{
				ReadItemsFromRows(ds.Tables[0].Select(select, sort), from, count);
			}
			ds.Dispose();
			return this.tbData;
		}


		public override ListDataTable Read(string selectSql, params Sota.Data.Simple.DbParameter[] par)
		{
			throw new NotImplementedException();
		}

		public override ListDataTable Read(System.Data.IDbCommand cmd)
		{
			throw new NotImplementedException();
		}

		public ListDataTable Read(SqlCommand cmd)
		{
			throw new NotImplementedException();
		}



		public override ListDataTable ReadByGuid(string guid)
		{
			return Read(List.FIELD_GUID + "='" + guid + "'");
		}
				
		public override ListDataTable ReadChildren(int parent_id)
		{
			return Read(List.FIELD_PARENT_ID + "='" + parent_id + "'");
		}
		
		public override ListDataTable ReadItem(int itemId)
		{
			return Read(List.FIELD_ID + "='" + itemId + "'");
		}

		public override ListDataTable ReadParentItems(int parent_id)
		{
			throw new NotImplementedException();
		}

		public override ListDataTable ReadNextItem(int itemId)
		{
			return Read(List.FIELD_ID + ">" + itemId, List.FIELD_ID + " ASC", 1);
		}

		public override ListDataTable ReadNextItem(int itemId, string sort)
		{
			throw new NotImplementedException();
		}
		
		public override ListDataTable ReadPrevItem(int itemId)
		{
			return Read(List.FIELD_ID + "<" + itemId, List.FIELD_ID + " DESC", 1);
		}

		public override ListDataTable ReadPrevItem(int itemId, string sort)
		{
			throw new NotImplementedException();
		}

		public override ListDataTable FindByField(string field, object value)
		{
			return Read(field + "='" + value.ToString().Replace("'","''") + "'");
		}
		#endregion


		#region insert
		private void Save()
		{
			DataSet ds = new DataSet("List_" + this.tbData.TableName);
			ds.Locale = CultureInfo.InvariantCulture;
			ds.Tables.Add(this.tbData);

			ds.WriteXml(HttpContext.Current.Request.MapPath(this._connectionString));
			ds.Tables.Remove(this.tbData);
			ds.Dispose();
		}
		private void SetRowValues(DataRow row, Hashtable fields)
		{
			if (fields != null)
			{
				foreach(string key in fields.Keys)
				{
					if (this.Data.Columns.Contains(key))
					{
						row[key] = fields[key];
					}
				}
			}
		}
		public override int Insert(int parentId, int deleted, System.Collections.Hashtable fields)
		{
			if(parentId==-1)
			{
				return Insert(-1, deleted, 0, fields);
			}
			return Insert(parentId, deleted, -1, fields);
		}
		public override int Insert(int parentId, int deleted, int level, System.Collections.Hashtable fields)
		{
			this.ReadAllFull();
			string sGuid = Guid.NewGuid().ToString();
	
			DataRow row = this.Data.FindRow(List.FIELD_GUID + "='" + sGuid + "'");
			while(row != null)
			{
				sGuid = Guid.NewGuid().ToString();
				row = this.Data.FindRow(List.FIELD_GUID + "='" + sGuid + "'");
			}
			
			if(level == -1)
			{
				if(parentId == -1)
				{
					level = 0;
				}
				else
				{
					row = this.Data.FindRow(List.FIELD_ID + "=" + parentId);
					if(row != null)
					{
						level = Convert.ToInt32(row[List.FIELD_LEVEL]) + 1;
					}
					else
					{
						level = 0;
						parentId = -1;
					}
				}
			}
			int newID = 1;
			if(this.Data.Count > 0)
			{
				string maxid = "maxid";
				this.Data.Columns.Add(maxid, typeof(int)).Expression = "MAX(" + List.FIELD_ID + ")";
				newID = this.Data.GetInt32(0, maxid) + 1;
				this.Data.Columns.Remove(maxid);
			}

			row = this.Data.NewRow();
			this.Data.Rows.Add(row);
			row[List.FIELD_PARENT_ID] = parentId;
			row[List.FIELD_ID] = newID;
			row[List.FIELD_GUID] = sGuid;
			row[List.FIELD_LEVEL] = level;
			row[List.FIELD_DELETED] = deleted;

			SetRowValues(row, fields);
			Save();
			return newID;
		}
		#endregion 


		#region update
		public override void Update(int itemId, Hashtable fields)
		{
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
			if(row != null)
			{
				SetRowValues(row, fields);
				Save();
			}
		}

		public override void Update(int itemId, int deleted, Hashtable fields)
		{
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
			if(row != null)
			{
				row[List.FIELD_DELETED] = deleted;
				SetRowValues(row, fields);
				Save();
			}
		}

		public override void Update(int itemId, int parentId, int deleted, Hashtable fields)
		{
			if(parentId == -1)
			{
				Update(itemId, -1, deleted, 0, fields);
				return;
			}
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + parentId);
			if(row != null)
			{
				int level = Convert.ToInt32(row[List.FIELD_LEVEL]) + 1;
				row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
				if(row != null)
				{
					row[List.FIELD_DELETED] = deleted;
					row[List.FIELD_PARENT_ID] = parentId;
					row[List.FIELD_LEVEL] = level;
					SetRowValues(row, fields);
					Save();
				}			
			}
		}

		public override void Update(int itemId, int parentId, int deleted, int level, Hashtable fields)
		{
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
			if(row != null)
			{
				row[List.FIELD_DELETED] = deleted;
				row[List.FIELD_PARENT_ID] = parentId;
				row[List.FIELD_LEVEL] = level;
				SetRowValues(row, fields);
				Save();
			}
		}
		public override void Update(DataRow row)
		{
			Hashtable h = new Hashtable();
			foreach (DataColumn col in this.Data.Columns)
			{
				if (List.IsCustomField(col.ColumnName))
				{
					h[col.ColumnName] = row[col.ColumnName];
				}
			}
			Update(
				Convert.ToInt32(row[List.FIELD_ID]),
				Convert.ToInt32(row[List.FIELD_PARENT_ID]),
				Convert.ToInt32(row[List.FIELD_DELETED]),
				Convert.ToInt32(row[List.FIELD_LEVEL]),
				h);
		}
		public override void UpdateField(int itemId, string field, object value)
		{
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
			if(row != null)
			{
				row[field] = value;
				Save();
			}
		}
		#endregion 


		#region delete
		public override bool Delete(int itemId)
		{
			this.ReadAllFull();
			DataRow row = this.Data.FindRow(List.FIELD_ID + "=" + itemId);
			if(row != null)
			{
				this.Data.Rows.Remove(row);
				Save();
				return true;
			}
			return false;
		}
		#endregion


		#region search
		protected override ListDataTable ReadSearch(string text)
		{
			StringBuilder sb = new StringBuilder();
			for(int i=0;i<this.tbData.Columns.Count;i++)
			{
				if(this.tbData.Columns[i].DataType==typeof(string))
				{
					if(sb.Length > 0)
					{
						sb.Append(" OR ");
					}
					sb.Append(this.tbData.Columns[i].ColumnName);
					sb.Append(" LIKE '*" + text.Replace("'","''") + "*'");
				}
			}
			return Read(sb.ToString());
		}
		#endregion


		#region not to implement
		public override ListDataTable ReadShort(string selectSql)
		{
			throw new NotImplementedException();
		}
		public override void DeleteField(int itemId, string field)
		{
			throw new NotImplementedException();
		}
		public override void InsertField(int itemId, string field, string value)
		{
			throw new NotImplementedException();
		}
		
		public override ListDataTable ReadAll()
		{
			throw new NotImplementedException();
		}
		public override ListDataTable ReadShort(IDbCommand cmd)
		{
			throw new NotImplementedException();
		}
		
		public override ListDataTable ReadShort(string selectSql, params Sota.Data.Simple.DbParameter[] par)
		{
			throw new NotImplementedException();
		}
		#endregion

	}
}
