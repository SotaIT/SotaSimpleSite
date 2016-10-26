using System;
using System.Collections;
using System.Data;
using System.Text;
using sql = System.Data.SqlClient;
using System.Data.SqlClient;

/* Все данные хранятся в одной таблице
 * Если используется многоязычность - то данные по другим языкам хранятся в другой таблице, 
 * причем другая таблица будет иметь только те поля, которые разные для разных языков
 * 
 * */
namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// List настроенный на оптимальную работу с БД MS SQL Server
	/// </summary>
	public sealed class SqlList: List
	{
		public SqlList()
		{
		}

		public SqlList(string name)
		{
			Init(name, false);
		}

		public SqlList(string name, bool full)
		{
			Init(name, full);
		}

		public override void VerifyDataStructure()
		{
			string tableName = TableName;
			sql.SqlConnection conn = createConnection();
			sql.SqlCommand cmd = new SqlCommand(

@"IF EXISTS(SELECT * FROM sysobjects 
WHERE id = object_id(N'[dbo].[" 
				+ tableName + 
@"]') 
AND (OBJECTPROPERTY(id, N'IsUserTable') = 1))
SELECT '1'
ELSE
SELECT '0'",
				conn);
			bool tableExists = false;
			try
			{
				conn.Open();
				tableExists = cmd.ExecuteScalar().ToString()=="1";
				if(tableExists)
				{
					#region alter table

					cmd.CommandText = "SELECT * FROM [" + tableName + "]";
					sql.SqlDataReader r = null;
					DataTable tbSchema = null;
					try
					{
						r = cmd.ExecuteReader();
						if(r.HasRows)
						{
							tbSchema = r.GetSchemaTable();
						}
					}
					finally
					{
						if(r!=null)
						{
							r.Close();
						}
					}
					if(tbSchema==null)
					{
						cmd.CommandText = "DROP TABLE [" + tableName + "];" + this.getCreateSql(tableName);
						cmd.ExecuteNonQuery();
					}
					else
					{
						//сначала сравним и только если разные - тогда перезапись таблицы
						bool hasChanged = false;
						for(int i=0;i<this.Data.Columns.Count;i++)
						{
							DataRow[] rs = tbSchema.Select("ColumnName='" + this.Data.Columns[i].ColumnName + "'");
							if(rs.Length > 0)
							{
								if(rs[0]["DataType"].ToString() != this.Data.Columns[i].DataType.ToString())
								{
									hasChanged = true;
									break;
								}
							}
							else
							{
								hasChanged = true;
								break;
							}
						}
						for(int i=0;i<tbSchema.Rows.Count;i++)
						{
							if(!this.Data.Columns.Contains(tbSchema.Rows[i]["ColumnName"].ToString()))
							{
								hasChanged = true;
								break;
							}
						}
						if(hasChanged)
						{
							StringBuilder sb = new StringBuilder();
							sb.Append(
								@"BEGIN TRANSACTION
SET QUOTED_IDENTIFIER ON
SET ARITHABORT ON
SET NUMERIC_ROUNDABORT OFF
SET CONCAT_NULL_YIELDS_NULL ON
SET ANSI_NULLS ON
SET ANSI_PADDING ON
SET ANSI_WARNINGS ON
COMMIT
BEGIN TRANSACTION;");
							sb.Append(this.getCreateSql("Tmp_" + tableName)); 
							sb.Append(";SET IDENTITY_INSERT [Tmp_");
							sb.Append(tableName);
							sb.Append("] ON;INSERT INTO [Tmp_");
							sb.Append(tableName);
							sb.Append("](");
							for(int i=0;i<this.Data.Columns.Count;i++)
							{
								if(tbSchema.Select("ColumnName='" + this.Data.Columns[i].ColumnName + "'").Length > 0)
								{
									if(i > 0)
									{
										sb.Append(",");
									}
									sb.Append("[");
									sb.Append(this.Data.Columns[i].ColumnName);
									sb.Append("]");
								}
							}
							sb.Append(") SELECT ");
							for(int i=0;i<this.Data.Columns.Count;i++)
							{
								DataRow[] rs = tbSchema.Select("ColumnName='" + this.Data.Columns[i].ColumnName + "'");
								if(rs.Length > 0)
								{
									if(i > 0)
									{
										sb.Append(",");
									}
									if(rs[0]["DataType"].ToString() != this.Data.Columns[i].DataType.ToString())
									{
										sb.Append("CONVERT(");
										sb.Append(getSqlDataType(this.Data.Columns[i].DataType,this.Data.Columns[i].ExtendedProperties["inputtype"].ToString()));
										sb.Append(",");

										if(rs[0]["DataType"].ToString()=="System.String" || this.Data.Columns[i].DataType.ToString()=="System.String")
										{
											sb.Append("CONVERT(nvarchar(255),");
										}
									}
									sb.Append("[");
									sb.Append(this.Data.Columns[i].ColumnName);
									sb.Append("]");
									if(rs[0]["DataType"].ToString() != this.Data.Columns[i].DataType.ToString())
									{
										sb.Append(")");

										if(rs[0]["DataType"].ToString()=="System.String" || this.Data.Columns[i].DataType.ToString()=="System.String")
										{
											sb.Append(")");
										}
									}
								}
							}
							sb.Append(" FROM [");
							sb.Append(tableName);
							sb.Append("] WITH (HOLDLOCK TABLOCKX);SET IDENTITY_INSERT [Tmp_");
							sb.Append(tableName);
							sb.Append("] OFF;DROP TABLE [");
							sb.Append(tableName);
							sb.Append("];EXECUTE sp_rename N'Tmp_");
							sb.Append(tableName);
							sb.Append("', N'");
							sb.Append(tableName);
							sb.Append("', 'OBJECT';COMMIT;");
							cmd.CommandText = sb.ToString();
							cmd.ExecuteNonQuery();
						}
					}
					#endregion
				}
				else
				{
					#region create table
					cmd.CommandText = getCreateSql(tableName);
					cmd.ExecuteNonQuery();
					#endregion
				}
			}
			finally
			{
				conn.Close();
			}

		}
		private string getCreateSql(string tableName)
		{
			StringBuilder sb = new StringBuilder();
			sb.Append("CREATE TABLE [");
			sb.Append(tableName);
			sb.Append("]([");
			sb.Append(FIELD_ID);
			sb.Append("] [int] IDENTITY(1,1) NOT NULL,[");
			sb.Append(FIELD_PARENT_ID);
			sb.Append("] [int] NULL,[");
			sb.Append(FIELD_DELETED);
			sb.Append("] [int] NULL,[");
			sb.Append(FIELD_LEVEL);
			sb.Append("] [int] NULL,[");
			sb.Append(FIELD_GUID);
			sb.Append("] [nvarchar](50) NULL");
			for(int i=0;i<this.Data.Columns.Count;i++)
			{
				if(List.IsCustomField(this.Data.Columns[i].ColumnName))
				{
					sb.Append(",[");
					sb.Append(this.Data.Columns[i].ColumnName);
					sb.Append("] ");

					sb.Append(getSqlDataType(this.Data.Columns[i].DataType,this.Data.Columns[i].ExtendedProperties["inputtype"].ToString()));
					sb.Append(" NULL");
				}
			}

			sb.Append(") ON [PRIMARY]");
			return sb.ToString();
		}
		private SqlConnection createConnection()
		{
			return new SqlConnection(this._connectionString);
		}
		private SqlCommand createCommand()
		{
			return createConnection().CreateCommand();
		}
		private string getSqlDataType(Type dt, string inputType)
		{
			if(dt==typeof(string))
			{
				/*if(inputType=="textarea"  
					|| inputType=="html")
				{
						return "ntext";
				}
				else
				{*/
					return "nvarchar(max)";
				/*}*/
			}
			else if(dt==typeof(int))
			{
				return "int";
			}
			else if (dt==typeof(bool))
			{
				return "bit";
			}
			else if (dt==typeof(double))
			{
				return "float";
			}
			else if (dt==typeof(DateTime))
			{
				return "datetime";
			}
			return "nvarchar(max)"; //"ntext";
		}
		private SqlDbType getSqlDbType(Type dt, string inputType)
		{
			if(dt==typeof(string))
			{
				/*if(inputType=="textarea"  
					|| inputType=="html")
				{
					return SqlDbType.NText;
				}
				else
				{*/
					return SqlDbType.NVarChar;
				/*}*/
			}
			else if(dt==typeof(int))
			{
				return SqlDbType.Int;
			}
			else if (dt==typeof(bool))
			{
				return SqlDbType.Bit;
			}
			else if (dt==typeof(double))
			{
				return SqlDbType.Float;
			}
			else if (dt==typeof(DateTime))
			{
				return SqlDbType.DateTime;
			}
			return SqlDbType.NVarChar;//SqlDbType.NText;
		}

		private SqlDbType getSqlDbType(DbType dt)
		{
			if(dt==DbType.Int32)
			{
				return SqlDbType.Int;
			}
			else if (dt==DbType.Boolean)
			{
				return SqlDbType.Bit;
			}
			else if (dt==DbType.Double)
			{
				return SqlDbType.Float;
			}
			else if (dt==DbType.DateTime)
			{
				return SqlDbType.DateTime;
			}
			return SqlDbType.NVarChar;//SqlDbType.NText;
		}

		#region read
		private void ReadItemRowsFromCommand(string commandText)
		{
			SqlCommand cmd = createCommand();
			cmd.CommandText = commandText;
			ReadItemRowsFromCommand(cmd);
		}
		private void ReadItemRowsFromCommand(SqlCommand cmd)
		{
			try
			{
				cmd.Connection.Open();
				ReadItemRowsFromReader(cmd.ExecuteReader());
			}
			finally
			{
				cmd.Connection.Close();
			}
		}
		private void ReadItemRowsFromReader(SqlDataReader r)
		{
			try
			{
				while (r.Read())
				{
					DataRow row = this.tbData.NewRow();
					for(int i=0;i<this.tbData.Columns.Count;i++)
					{
						row[this.tbData.Columns[i].ColumnName] = r[this.tbData.Columns[i].ColumnName];
					}
					this.tbData.Rows.Add(row);
				}
			}
			finally
			{
				if (r != null)
					r.Close();
			}
		}

		public override ListDataTable ReadAllFull()
		{
			return Read("SELECT * FROM [" + TableName + "]");
		}

		public override ListDataTable Read(string selectSql)
		{
			ReadItemRowsFromCommand(selectSql);
			return this.tbData;
		}

		public override ListDataTable Read(string selectSql, params Sota.Data.Simple.DbParameter[] par)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = selectSql;
			for(int i=0;i<par.Length;i++)
			{
				cmd.Parameters.Add(par[i].ParameterName, getSqlDbType(par[i].DbType)).Value = par[i].Value;
			}
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable Read(System.Data.IDbCommand cmd)
		{
			return Read((SqlCommand)cmd);
		}

		public ListDataTable Read(SqlCommand cmd)
		{
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadWhere(string where, params Sota.Data.Simple.DbParameter[] par)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = string.Format("SELECT * FROM [{0}] WHERE {1}", TableName, where);
			for(int i = 0; i < par.Length; i++)
			{
				cmd.Parameters.Add(par[i].ParameterName, getSqlDbType(par[i].DbType)).Value = par[i].Value;
			}
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadByGuid(string guid)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + FIELD_GUID + "]=@guid";
			cmd.Parameters.Add("@guid", SqlDbType.NVarChar).Value = guid;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}
				
		public override ListDataTable ReadChildren(int parent_id)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + FIELD_PARENT_ID + "]=@pid";
			cmd.Parameters.Add("@pid", SqlDbType.Int).Value = parent_id;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}
		
		public override ListDataTable ReadItem(int itemId)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + FIELD_ID + "]=@id";
			cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemId;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadParentItems(int parent_id)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + FIELD_PARENT_ID + "]=@pid AND [" + FIELD_ID + "] IN (SELECT [" + FIELD_PARENT_ID + "] FROM [" + TableName + "])";
			cmd.Parameters.Add("@pid", SqlDbType.Int).Value = parent_id;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadNextItem(int itemId)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT TOP 1 * FROM [" + TableName + "] WHERE [" + FIELD_ID + "]>@id ORDER BY [" + FIELD_ID + "] ASC";
			cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemId;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadNextItem(int itemId, string sort)
		{
			throw new NotImplementedException();
		}
		
		public override ListDataTable ReadPrevItem(int itemId)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT TOP 1 * FROM [" + TableName + "] WHERE [" + FIELD_ID + "]<@id ORDER BY [" + FIELD_ID + "] DESC";
			cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemId;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable ReadPrevItem(int itemId, string sort)
		{
			throw new NotImplementedException();
		}

		public override ListDataTable FindByField(string field, object value)
		{
			SqlCommand cmd = this.createCommand();
			SqlDbType dbType = getSqlDbType(this.tbData.Columns[field].DataType,this.tbData.Columns[field].ExtendedProperties["inputtype"].ToString());
			/*if(dbType==SqlDbType.NText)
			{
					cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + field + "] LIKE @field";
			}
			else
			{*/
				cmd.CommandText = "SELECT * FROM [" + TableName + "] WHERE [" + field + "]=@field";
			/*}*/
			cmd.Parameters.Add("@field", dbType).Value = value;
			ReadItemRowsFromCommand(cmd);
			return this.tbData;
		}

		public override ListDataTable Select(string sql, params object[] par)
		{
			ReadItemRowsFromCommand(createExecuteCommand(sql, par));
			return this.tbData;
		}


		#endregion


		#region insert
		public override int Insert(int parentId, int deleted, System.Collections.Hashtable fields)
		{
			if(parentId==-1)
			{
				return Insert(-1, deleted, 0, fields);
			}
			sql.SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT [" + FIELD_LEVEL + "] FROM [" + TableName + "] WHERE [" + FIELD_ID + "]=@pid";
			cmd.Parameters.Add("@pid", SqlDbType.Int).Value = parentId;
			try
			{
				cmd.Connection.Open();
				object res = cmd.ExecuteScalar();
				if(!Util.IsBlank(res))
				{
					return Insert(parentId, deleted, Convert.ToInt32(res)+1, fields);
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}
		public override int Insert(int parentId, int deleted, int level, System.Collections.Hashtable fields)
		{
			string sGuid = Guid.NewGuid().ToString();
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT [" + FIELD_ID + "] FROM [" + TableName + "] WHERE [" + FIELD_GUID + "]=@" + FIELD_GUID;
			cmd.Parameters.Add("@" + FIELD_GUID, SqlDbType.NVarChar).Value = sGuid;
			try
			{
				cmd.Connection.Open();
				object res = cmd.ExecuteScalar();
				while (res != null)
				{
					sGuid = Guid.NewGuid().ToString();
					cmd.Parameters["@" + FIELD_GUID].Value = sGuid;
					res = cmd.ExecuteScalar();
				}

				StringBuilder sb1 = new StringBuilder();
				StringBuilder sb2 = new StringBuilder("@");
				sb1.Append(FIELD_GUID);
				sb2.Append(FIELD_GUID);
				
				sb1.Append(",");
				sb2.Append(",@");
				sb1.Append(FIELD_DELETED);
				sb2.Append(FIELD_DELETED);
				cmd.Parameters.Add("@" + FIELD_DELETED, SqlDbType.Int).Value = deleted;

				sb1.Append(",");
				sb2.Append(",@");
				sb1.Append(FIELD_PARENT_ID);
				sb2.Append(FIELD_PARENT_ID);
				cmd.Parameters.Add("@" + FIELD_PARENT_ID, SqlDbType.Int).Value = parentId;

				sb1.Append(",");
				sb2.Append(",@");
				sb1.Append(FIELD_LEVEL);
				sb2.Append(FIELD_LEVEL);
				cmd.Parameters.Add("@" + FIELD_LEVEL, SqlDbType.Int).Value = level;

				if (fields != null)
				{
					foreach(string key in fields.Keys)
					{
						if (this.Data.Columns.Contains(key))
						{
							if(!Util.IsBlank(fields[key]))
							{
								cmd.Parameters.Add("@" + key, getSqlDbType(this.Data.Columns[key].DataType,this.Data.Columns[key].ExtendedProperties["inputtype"].ToString())).Value = fields[key];
								sb1.Append(",[");
								sb1.Append(key);
								sb1.Append("]");
								sb2.Append(",@");
								sb2.Append(key);
							}
						}
					}
				}
				cmd.CommandText = "INSERT INTO [" + TableName + "](" + sb1 + ")VALUES(" + sb2 + ")";
				cmd.ExecuteNonQuery();

				cmd.CommandText = "SELECT [" + FIELD_ID + "] FROM [" + TableName + "] WHERE [" + FIELD_GUID + "]=@" + FIELD_GUID;
				cmd.Parameters.Clear();
				cmd.Parameters.Add("@" + FIELD_GUID, SqlDbType.NVarChar).Value = sGuid;
				res = cmd.ExecuteScalar();
                if(!Util.IsBlank(res))
				{
					return Convert.ToInt32(res);
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}
		#endregion 


		#region update

		public override void Update(int itemId, Hashtable fields)
		{
            Update(itemId, -2, -2, -2, fields);
        }

		public override void Update(int itemId, int deleted, Hashtable fields)
		{
		    Update(itemId, -2, deleted, -2, fields);
		}

		public override void Update(int itemId, int parentId, int deleted, Hashtable fields)
		{
			if(parentId == -1)
			{
				Update(itemId, -1, deleted, 0, fields);
				return;
			}
			sql.SqlCommand cmd = this.createCommand();
			cmd.CommandText = "SELECT [" + FIELD_LEVEL + "] FROM [" + TableName + "] WHERE [" + FIELD_ID + "]=@pid";
			cmd.Parameters.Add("@pid", SqlDbType.Int).Value = parentId;
			try
			{
				cmd.Connection.Open();
				object res = cmd.ExecuteScalar();
				if(!Util.IsBlank(res))
				{
					Update(itemId, parentId, deleted, Convert.ToInt32(res)+1, fields);
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		public override void Update(int itemId, int parentId, int deleted, int level, Hashtable fields)
		{
			SqlCommand cmd = this.createCommand();
			cmd.Parameters.Add("@" + FIELD_ID, SqlDbType.Int).Value = itemId;
			
			StringBuilder sb = new StringBuilder();

            if (deleted != -2)
            {
                sb.Append(FIELD_DELETED);
                sb.Append("=@");
                sb.Append(FIELD_DELETED);
                cmd.Parameters.Add("@" + FIELD_DELETED, SqlDbType.Int).Value = deleted;
                sb.Append(",");

                if (parentId != -2)
                {
                    sb.Append(FIELD_PARENT_ID);
                    sb.Append("=@");
                    sb.Append(FIELD_PARENT_ID);
                    cmd.Parameters.Add("@" + FIELD_PARENT_ID, SqlDbType.Int).Value = parentId;
                    sb.Append(",");

                    sb.Append(FIELD_LEVEL);
                    sb.Append("=@");
                    sb.Append(FIELD_LEVEL);
                    cmd.Parameters.Add("@" + FIELD_LEVEL, SqlDbType.Int).Value = level;
                    sb.Append(",");
                }
            }

			if (fields != null)
			{
				foreach(string key in fields.Keys)
				{
					if (this.Data.Columns.Contains(key))
					{
						cmd.Parameters.Add("@" + key, getSqlDbType(this.Data.Columns[key].DataType,this.Data.Columns[key].ExtendedProperties["inputtype"].ToString())).Value = fields[key];
						sb.Append("[");
						sb.Append(key);
						sb.Append("]");
						sb.Append("=@");
						sb.Append(key);
                        sb.Append(",");
                    }
				}
			}
			try
			{
				cmd.Connection.Open();
				cmd.CommandText = "UPDATE [" + TableName + "] SET " + sb.ToString().TrimEnd(',') + " WHERE " + FIELD_ID + "=@" + FIELD_ID;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
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

            int id = Convert.ToInt32(row[List.FIELD_ID]);
            int deleted = Convert.ToInt32(row[List.FIELD_DELETED]);

            // int parent_id = Convert.ToInt32(row[List.FIELD_PARENT_ID]);
            // int level = Convert.ToInt32(row[List.FIELD_LEVEL]);

            Update(id, deleted, h);

        }

        public override void UpdateField(int itemId, string field, object value)
		{
			SqlCommand cmd = this.createCommand();
			cmd.Parameters.Add("@" + FIELD_ID, SqlDbType.Int).Value = itemId;
            if (field == List.FIELD_DELETED)
            {
                cmd.Parameters.Add("@" + field, SqlDbType.Int).Value = value;
            }
            else
            {
                cmd.Parameters.Add("@" + field, getSqlDbType(this.Data.Columns[field].DataType, this.Data.Columns[field].ExtendedProperties["inputtype"].ToString())).Value = value;
            }
            try
			{
				cmd.Connection.Open();
				cmd.CommandText = "UPDATE [" + TableName + "] SET [" + field + "]=@" + field + " WHERE [" + FIELD_ID + "]=@" + FIELD_ID;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
			}		
		}
		#endregion 


		#region delete
		public override bool Delete(int itemId)
		{
			bool retVal = false;
			sql.SqlCommand cmd = this.createCommand();
			cmd.CommandText = "DELETE FROM [" + TableName + "] WHERE " + List.FIELD_ID + "=@id";
			cmd.Parameters.Add("@id", SqlDbType.Int).Value = itemId;
			try
			{
				cmd.Connection.Open();
				if(cmd.ExecuteNonQuery()>0)
				{
					retVal = true;
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return retVal;
		}
		#endregion


		#region search
		protected override ListDataTable ReadSearch(string text)
		{
			SqlCommand cmd = this.createCommand();
			cmd.Parameters.Add("@text", SqlDbType.NVarChar).Value = text;
			StringBuilder sb = new StringBuilder();
			for(int i=0;i<this.tbData.Columns.Count;i++)
			{
				if(this.tbData.Columns[i].DataType==typeof(string))
				{
					if(sb.Length>0)
					{
						sb.Append(" OR ");
					}
					sb.Append("[");
					sb.Append(this.tbData.Columns[i].ColumnName);
					sb.Append("] LIKE '%'+@text+'%'");
				}
			}
			if(sb.Length>0)
			{
				sb.Insert(0, " WHERE ");
			}
			cmd.CommandText = "SELECT * FROM [" + TableName + "]" + sb;
			return Read(cmd);
		}
		#endregion


		#region execute
		/*
		 			SqlCommand cmd = this.createCommand();
			StringBuilder sql = new StringBuilder();
			sql.AppendFormat("SELECT * FROM [{0}] {1}", TableName, sqlFormat);
			for (int i = 0; i < parameterArgs.Length; i++)
			{
				SqlParameter par = new SqlParameter("@par" + i, parameterArgs[i]);
				sql.Replace("{" + i + "}", "@par" + i);
				cmd.Parameters.Add(par);
			}

			cmd.CommandText = sql.ToString();
		 */
		SqlCommand createExecuteCommand(string sqlFormat, object[] parameterArgs)
		{
			SqlCommand cmd = this.createCommand();
			cmd.CommandText = sqlFormat.Replace("@table", "[" + TableName + "]");
			if (parameterArgs != null)
			{
				for (int i = 0; i < parameterArgs.Length; i++)
				{
					SqlParameter par = new SqlParameter("@parameter" + i, parameterArgs[i]);
					cmd.Parameters.Add(par);
				}
			}
			return cmd;
		}

		public override int Execute(string sql)
		{
			return Execute(sql, (object[])null);
		}
		public override int Execute(string sql, params object[] par)
		{
			SqlCommand cmd = createExecuteCommand(sql, par);
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}
		public override object ExecuteScalar(string sql)
		{
			return ExecuteScalar(sql, (object[])null);
		}
		public override object ExecuteScalar(string sql, params object[] par)
		{
			SqlCommand cmd = createExecuteCommand(sql, par);
			try
			{
				cmd.Connection.Open();
				return cmd.ExecuteScalar();
			}
			finally
			{
				cmd.Connection.Close();
			}
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
