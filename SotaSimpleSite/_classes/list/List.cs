using System;
using System.Collections;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Caching;
using Sota.Data.Simple;
using Sota.Web.SimpleSite.Search;
using Sota.Web.SimpleSite.Security;

namespace Sota.Web.SimpleSite
{
	/// <summary>
	/// Список
	/// </summary>
	public class List : ISupportsSearch
	{
		#region const

		public const string FIELD_ID		= "_id";
		public const string FIELD_GUID		= "_guid";
		public const string FIELD_PARENT_ID	= "_pid";
		public const string FIELD_DELETED	= "_deleted";
		public const string FIELD_LEVEL		= "_level";
		public const string LIST_TYPE		= "Sota.Web.SimpleSite.List,SotaSimpleSite";

		#endregion

		#region constructor

		public List()
		{
		}

		public List(string name)
		{
			Init(name, false);
		}

		public List(string name, bool full)
		{
			Init(name, full);
		}

		protected void Init(string name, bool full)
		{
			DataRow[] rows = GetLists().Select("name='" + name + "'");
			if (rows.Length > 0)
			{
				this._name = name;
				this._caption = rows[0]["caption"].ToString();
				ReadSchema(HttpContext.Current.Request.MapPath(rows[0]["file"].ToString()), full);
				if (this._provider == "xml")
				{
					this.tbXmlPattern = Config.GetConfigTable("list.config", "xml_pattern");
				}
				else
				{
					this.tbValuesTable = Config.GetConfigTable("list.config", "value_table");
					this.tbListTable = Config.GetConfigTable("list.config", "list_table");
					this.tbSqls = Config.GetConfigTable("list.config", "sql");
					this.tbParameters = Config.GetConfigTable("list.config", "sql_parameter");
				}
			}
		}
		public static List Create(string name)
		{
			return Create(name, false);
		}
		public static List Create(string name, bool full)
		{
			DataRow[] r = GetLists().Select("name='" + name + "'");
			if(r.Length>0)
			{
				string[] t = r[0]["provider"].ToString().Split(',');
				List l = (List)Activator.CreateInstance(t[1], t[0]).Unwrap();
				l.Init(name, full);
				return l;
			}
			throw new IndexOutOfRangeException("Список не создан!");
		}

		#endregion

		#region Create Cached

		public static void ClearCache(string name, CreateType type, bool full)
		{
			Cache cache = HttpContext.Current.Cache;
			string key = Keys.CachedList+"[" + Path.Site + "]["+name+"]["+type.ToString()+"]["+full.ToString()+"]";
			cache.Remove(key);
		}
		public static void ClearCache(string name, CreateType type)
		{
			ClearCache(name, type, false);
		}
		public static void ClearCache(string name)
		{
			ClearCache(name, CreateType.AllFull, false);
		}
		public static List CreateCached(string name)
		{
			return CreateCached(name, CreateType.AllFull, false, 30);
		}
		public static List CreateCached(string name, int seconds)
		{
			return CreateCached(name, CreateType.AllFull, false, seconds);
		}
		public static List CreateCached(string name, CreateType type)
		{
			return CreateCached(name, type, false, 30);
		}
		public static List CreateCached(string name, CreateType type, bool full, int seconds)
		{
			Cache cache = HttpContext.Current.Cache;
			string key = Keys.CachedList + "[" + Path.Site + "][" + name + "]["+type.ToString()+"]["+full.ToString()+"]";
			if(cache[key] != null)
			{
				return (List)cache[key];
			}
			List l = Create(name, full);
			switch(type)
			{
				case CreateType.AllFull:
					l.ReadAllFull();
					break;
				case CreateType.RootParentItems:
					l.ReadRootParentItems();
					break;
				case CreateType.RootItems:
					l.ReadRootItems();
					break;
			}
			cache.Insert(key, l, null, DateTime.Now.AddSeconds(seconds), TimeSpan.Zero);
			return l;
		}
		public enum CreateType
		{
			Empty,
			AllFull,
			RootParentItems,
			RootItems
		}
		#endregion

		#region schema
		public virtual void VerifyDataStructure()
		{
			
		}
        protected string _tableName = string.Empty;

		public string TableName
		{
			get
			{
                return _tableName.Length == 0 
                    ? "sss_List_" + this.DBName 
                    : _tableName;
			}
		}

		protected void ReadSchema(string file, bool full)
		{
			this._file = file;
			tbData = new ListDataTable(Name, this);
			tbData.Columns.Add(FIELD_ID, typeof (int));
			tbData.Columns.Add(FIELD_PARENT_ID, typeof (int));
			tbData.Columns.Add(FIELD_GUID, typeof (string));
			tbData.Columns.Add(FIELD_DELETED, typeof (int));
			tbData.Columns.Add(FIELD_LEVEL, typeof (int));
			DataSet ds = GetSchema(file);
			DataTable tbList = ds.Tables["list"];
			if (full)
			{
				tbTreeCaption = ds.Tables["tree_caption"];
				tbEditor = ds.Tables["editor"];
				tbLevelIcon = ds.Tables["level_tree_icon"];
				tbLevelView = ds.Tables["level_view"];
				tbDeleteQuestion = ds.Tables["delete_question"];
			}
			this._connectionString = tbList.Rows[0]["connectionstring"].ToString().Trim().Length==0 ? Config.Main.ConnectionString : tbList.Rows[0]["connectionstring"].ToString();
			this._provider = tbList.Rows[0]["providertype"].ToString();
			this._searchIn = tbList.Rows[0]["searchin"].ToString() == "1";
            if (tbList.Columns.Contains("table"))
            {
                this._tableName = tbList.Rows[0]["table"].ToString();
            }

			if (this._provider == "xml")
				this._directory = new DirectoryInfo(HttpContext.Current.Request.MapPath(this.ConnectionString));
			else
				this._connector = new Connector(this.ConnectionString, this.Provider);
			DataTable tbColumns = ds.Tables["column"];
			int n = tbColumns.Rows.Count;
			for (int i = 0; i < n; i++)
			{
				DataColumn col = this.tbData.Columns.Add();
				col.ColumnName = tbColumns.Rows[i]["fieldname"].ToString();
				col.DataType = Type.GetType("System." + tbColumns.Rows[i]["datatype"].ToString());
				if (tbColumns.Columns.Contains("expression"))
				{
					if (tbColumns.Rows[i]["expression"] != DBNull.Value)
					{
						col.Expression = tbColumns.Rows[i]["expression"].ToString();
					}
				}
				col.Caption = tbColumns.Rows[i]["caption"].ToString();
				if (tbColumns.Columns.Contains("defaultvalue"))
				{
					if (tbColumns.Rows[i]["defaultvalue"] != DBNull.Value)
					{
						string val = tbColumns.Rows[i]["defaultvalue"].ToString();
						if(!Util.IsBlank(val))
						{
							if (col.DataType == typeof (DateTime))
							{
								string s = val.Trim();
								if (s == "now")
								{
									col.DefaultValue = Config.Main.Now();
								}
								else
								{
									col.DefaultValue = DateTime.Parse(s, CultureInfo.InvariantCulture);
								}
							}
							else if(val=="[#userid#]")
							{
								col.DefaultValue = UserInfo.Current.UserId;
							}
							else if (col.DataType == typeof (int))
							{
								col.DefaultValue = int.Parse(val);
							}
							else if (col.DataType == typeof (double))
							{
								col.DefaultValue = double.Parse(val, CultureInfo.InvariantCulture);
							}
							else if (col.DataType == typeof (bool))
							{
								col.DefaultValue = bool.Parse(val);
							}
							else
							{
								col.DefaultValue = val;
							}
						}
					}
				}
				foreach (DataColumn c in tbColumns.Columns)
				{
					if (IsCustomColumnAttribute(c.ColumnName))
					{
						col.ExtendedProperties.Add(c.ColumnName, tbColumns.Rows[i][c]);
					}
				}
			}
		}

		protected bool IsCustomColumnAttribute(string name)
		{
			switch (name)
			{
				case "fieldname":
				case "caption":
				case "datatype":
				case "expression":
				case "defaultvalue":
					return false;
			}
			return true;
		}

		protected DataSet GetSchema(string file)
		{
			DataSet ds = new DataSet();
			ds.ReadXml(file);
			return ds;
		}

		#endregion

		#region editor

		public string GetTreeCaptionExpression(int level)
		{
			DataRow[] rows = this.tbTreeCaption.Select("[level]='" + level.ToString() + "'");
			if (rows.Length > 0)
			{
				return rows[0]["expression"].ToString();
			}
			else
			{
				rows = this.tbTreeCaption.Select("[level]='default'");
				if (rows.Length > 0)
				{
					return rows[0]["expression"].ToString();
				}
			}
			return null;
		}

		public string GetTreeCaption(DataRow row)
		{
			string expression = this.GetTreeCaptionExpression(Convert.ToInt32(row[FIELD_LEVEL]));
			if (expression == null || expression == string.Empty)
				return row[FIELD_ID].ToString();
			return ParseExpression(row, expression)
				.Replace("\n"," ")
				.Replace("\r"," ")
				.Replace("\\","&#92;")
				.Replace("'","&#39;")
				.Replace("\"","&#34;");
		}

		public DataTable DeleteQuestionTable
		{
			get { return this.tbDeleteQuestion; }
		}

		public string[] GetLevelIcons(int level)
		{
			string[] li = getLevelIcons(level.ToString());
			if (li != null)
				return li;
			else
				return getLevelIcons("default");
		}

		protected string[] getLevelIcons(string level)
		{
			DataRow[] row = this.tbLevelIcon.Select("[level]='" + level + "'");
			if (row.Length > 0)
			{
				return new string[] {row[0]["icon"].ToString(), row[0]["icon_deleted"].ToString()};
			}
			return null;
		}

		public string[] GetNewItemIconCaption(int level)
		{
			string[] ic = getNewItemIconCaption(level.ToString());
			if (ic != null)
				return ic;
			else
				return getNewItemIconCaption("default");
		}

		protected string[] getNewItemIconCaption(string level)
		{
			DataRow[] row = this.tbLevelIcon.Select("[level]='" + level + "'");
			if (row.Length > 0)
			{
				return new string[] {row[0]["new_item_icon"].ToString(), row[0]["new_item_caption"].ToString()};
			}
			return null;
		}

		public Hashtable GetLevelColumns()
		{
			Hashtable h = new Hashtable();
			foreach (DataColumn col in this.Data.Columns)
			{
				string s = (string) col.ExtendedProperties["levels"];
				if (s != null && s != string.Empty)
				{
					string[] levels = s.Split(',');
					foreach (string level in levels)
					{
						if (h.ContainsKey(level))
						{
							h[level] = h[level].ToString() + "," + col.ColumnName;
						}
						else
						{
							h[level] = col.ColumnName;
						}
					}
				}
				else if (IsCustomField(col.ColumnName))
				{
					if (h.ContainsKey("-1"))
					{
						h["-1"] = h["-1"].ToString() + "," + col.ColumnName;
					}
					else
					{
						h["-1"] = col.ColumnName;
					}
				}
			}
			return h;
		}

		public static bool IsCustomField(string fieldName)
		{
			switch (fieldName)
			{
				case List.FIELD_ID:
				case List.FIELD_GUID:
				case List.FIELD_PARENT_ID:
				case List.FIELD_DELETED:
				case List.FIELD_LEVEL:
					return false;
			}
			return true;
		}

		public bool CanBeParent(int level)
		{
			return level < LevelCount || LevelCount == -1;
		}

		public DataTable LevelViewTable
		{
			get { return this.tbLevelView; }
		}

		public string GetLevelViewUrl(int level)
		{
			string lv = getLevelViewUrl(level.ToString());
			if (lv != null)
				return lv;
			else
				return getLevelViewUrl("default");

		}

		protected string getLevelViewUrl(string level)
		{
			DataRow[] row = this.tbLevelIcon.Select("[level]='" + level + "'");
			if (row.Length > 0)
			{
				return row[0]["url"].ToString();
			}
			return null;
		}

		#endregion

		#region utils
		
		public static string DateTimeToString(DateTime dt)
		{
			return dt.ToString(CultureInfo.InvariantCulture);
		}

		public static DateTime DateTimeFromString(string s)
		{
			return DateTime.Parse(s, CultureInfo.InvariantCulture);
		}

		public static string DateTimeParameter(DateTime dt)
		{
				return "#" + DateTimeToString(dt) + "#";
		}

		public void Clear()
		{
			this.Data.Rows.Clear();
		}

		public static string ParseExpression(DataRow row, string expression)
		{
			if (expression == null || expression == string.Empty)
				return string.Empty;
			StringBuilder sb = new StringBuilder(expression);
			foreach (DataColumn col in row.Table.Columns)
			{
				if (Util.IsNullOrEmpty(row[col]))
				{
					sb.Replace("[@" + col.ColumnName + "]", "");
				}
				else
				{
					if (col.DataType == typeof (DateTime))
					{
						switch(col.ExtendedProperties["inputtype"].ToString())
						{
							case "datetime":
								sb.Replace("[@" + col.ColumnName + "]", Convert.ToDateTime(row[col]).ToString("yyyy-MM-dd HH:mm:ss"));
								break;
							case "date":
								sb.Replace("[@" + col.ColumnName + "]", Convert.ToDateTime(row[col]).ToString("yyyy-MM-dd"));
								break;
							case "time":
								sb.Replace("[@" + col.ColumnName + "]", Convert.ToDateTime(row[col]).ToLongTimeString());
								break;
						}
					}
					if (col.DataType == typeof (double))
					{
						sb.Replace("[@" + col.ColumnName + "]", Convert.ToDouble(row[col]).ToString(CultureInfo.InvariantCulture));
					}
					else
					{
						sb.Replace("[@" + col.ColumnName + "]", row[col].ToString());
					}
				}
			}
			return sb.ToString();
		}

		public static DataTable GetLists()
		{
			return Config.GetConfigTable("lists.config", "list");
		}

		protected string GetSql(string key)
		{
			return this.tbSqls.Select("(key='" + key + "') AND (provider='" + this.Provider + "')")[0]["value"].ToString();
		}

		protected DataRow[] GetSqlParameters(string key)
		{
			return this.tbParameters.Select("(sqlkey='" + key + "') AND (provider='" + this.Provider + "')", "[order] ASC");
		}

		protected string GetParameterName(string key, string sqlKey)
		{
			return this.tbParameters.Select("(key='" + key + "') AND (sqlkey='" + sqlKey + "') AND (provider='" + this.Provider + "')")[0]["parametername"].ToString();
		}

		protected DbParameter ParameterFromRow(DataRow row)
		{
			return new DbParameter(row["parametername"].ToString(), DbParameter.ParseDbType(row["dbtype"].ToString()));
		}

		protected string GetXmlPattern(string name)
		{
			DataRow[] rows = this.tbXmlPattern.Select("name='" + name + "'");
			if (rows.Length > 0)
				return rows[0]["pattern"].ToString();
			return null;
		}
		protected string GetFieldValue(string field, object value)
		{
			if(!Util.IsBlank(value))
			{
				Type dt = this.Data.Columns[field].DataType;
				if(dt == typeof(DateTime) && this.Data.Columns[field].ExtendedProperties["inputtype"].ToString() == "datetime")
				{
					return Convert.ToDateTime(value).ToString(CultureInfo.InvariantCulture);
				}
				else if(dt == typeof(double))
				{
					return Convert.ToDouble(value).ToString(CultureInfo.InvariantCulture);
				}
			}
			return value.ToString();
		}

		public static DateTime CombineDateTime(DateTime date, DateTime time)
		{
			return date.Date.AddHours(time.Hour).AddMinutes(time.Minute).AddSeconds(time.Second);
		}
		public static DateTime CombineDateTime(object oDate, object oTime)
		{
			return CombineDateTime(Convert.ToDateTime(oDate), Convert.ToDateTime(oTime));
		}
		public static string DateTimeToString(DateTime date, DateTime time, string format)
		{
			return CombineDateTime(date, time).ToString(format);
		}
		public static string DateTimeToString(object oDate, object oTime, string format)
		{
			return DateTimeToString(Convert.ToDateTime(oDate), Convert.ToDateTime(oTime), format);
		}
		public static string DateTimeToString(object oDate, object oTime)
		{
			return DateTimeToString(Convert.ToDateTime(oDate), Convert.ToDateTime(oTime), "yyyy-MM-dd HH:mm");
		}
		#endregion

		#region read

		#region common sql

		#region full
		/// <summary>
		/// Считывает все данные из БД
		/// </summary>
		/// <returns>ListDataTable со всеми данными</returns>
		public virtual ListDataTable Read()
		{
			return ReadAllFull();
		}

		public virtual ListDataTable Read(string selectSql)
		{
			return Read(this._connector.CreateCommand(selectSql));
		}

		public virtual ListDataTable Read(string selectSql, params DbParameter[] par)
		{
			return Read(this._connector.CreateCommand(selectSql, par));
		}

		public virtual ListDataTable Read(IDbCommand cmd)
		{
			ReadItemRowsFromReader(cmd);
			return this.Data;
		}

		private void ReadItemRowsFromReader(IDbCommand cmd)
		{
			IDataReader r = null;
			try
			{
				int nLastId = -1;
				DataRow rowTable = this.tbListTable.Select("provider='" + this.Provider + "'")[0];
				string sId = rowTable["id"].ToString();
				string sGuId = rowTable["guid"].ToString();
				string sParentId = rowTable["pid"].ToString();
				string sDeleted = rowTable["deleted"].ToString();
				string sLevel = rowTable["level"].ToString();
				rowTable = this.tbValuesTable.Select("provider='" + this.Provider + "'")[0];
				string sFieldName = rowTable["fieldname"].ToString();
				string sValue = rowTable["value"].ToString();
				DataRow row = null;
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				while (r.Read())
				{
					int nId = r.GetInt32(r.GetOrdinal(sId));
					if (nLastId != nId)
					{
						if (row != null)
						{
							this.tbData.Rows.Add(row);
						}
						row = this.tbData.NewRow();
						row[FIELD_ID] = nLastId = nId;
						row[FIELD_GUID] = r[sGuId];
						row[FIELD_PARENT_ID] = r[sParentId];
						row[FIELD_DELETED] = r[sDeleted];
						row[FIELD_LEVEL] = r[sLevel];
					}
					string sFieldNameVal = r[sFieldName].ToString();
					if (sFieldNameVal.Length > 0)
					{
						if (row.Table.Columns.Contains(sFieldNameVal))
						{
							try
							{
								Type dt = row.Table.Columns[sFieldNameVal].DataType;
								string val = r[sValue].ToString();
								if(dt==typeof(DateTime))
								{
									string it = row.Table.Columns[sFieldNameVal].ExtendedProperties["inputtype"].ToString();
									switch(it)
									{
										case "datetime":
											row[sFieldNameVal] = DateTime.Parse(val, CultureInfo.InvariantCulture);
											break;
										case "date":
											row[sFieldNameVal] = DateTime.Parse(val);
											break;
										case "time":
											row[sFieldNameVal] = DateTime.Parse(val);
											break;
									}
								}
								else if(dt==typeof(double))
								{
									row[sFieldNameVal] = double.Parse(val, CultureInfo.InvariantCulture);
								}
								else
								{
									row[sFieldNameVal] = val;
								}
								
							}
							catch(Exception ex)
							{
								Config.ReportError(ex);
								DeleteField(nId, sFieldNameVal);
							}
						}

					}
				}
				if (row != null)
				{
					this.tbData.Rows.Add(row);
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
		}

		#endregion

		#region short

		public virtual ListDataTable ReadShort(string selectSql)
		{
			return ReadShort(this._connector.CreateCommand(selectSql));
		}

		public virtual ListDataTable ReadShort(string selectSql, params DbParameter[] par)
		{
			return ReadShort(this._connector.CreateCommand(selectSql, par));
		}

		public virtual ListDataTable ReadShort(IDbCommand cmd)
		{
			ReadShortItemRowsFromReader(cmd);
			return this.Data;
		}

		protected void ReadShortItemRowsFromReader(IDbCommand cmd)
		{
			IDataReader r = null;
			try
			{
				DataRow rowTable = this.tbListTable.Select("provider='" + this.Provider + "'")[0];
				string sId = rowTable["id"].ToString();
				string sGuid = rowTable["guid"].ToString();
				string sParentId = rowTable["pid"].ToString();
				string sDeleted = rowTable["deleted"].ToString();
				string sLevel = rowTable["level"].ToString();
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				while (r.Read())
				{
					DataRow row = this.tbData.NewRow();
					row[FIELD_ID] = r[sId];
					row[FIELD_GUID] = r[sGuid];
					row[FIELD_PARENT_ID] = r[sParentId];
					row[FIELD_DELETED] = r[sDeleted];
					row[FIELD_LEVEL] = r[sLevel];
					this.tbData.Rows.Add(row);
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
		}

		#endregion

		#endregion

		#region common xml

		public ListDataTable ReadXml(params FileInfo[] files)
		{
			DataSet ds = new DataSet();
			int n = files.Length;
			for (int i = 0; i < n; i++)
			{
				string[] ids = files[0].Name.Split(new char[] {'_', '.'});
				ds.Clear();
				ds.ReadXml(files[i].FullName);
				int m = ds.Tables[0].Rows.Count;
				int k = ds.Tables[0].Columns.Count;
				for (int j = 0; j < m; j++)
				{
					DataRow r = this.Data.NewRow();
					r[FIELD_ID] = ids[0];
					r[FIELD_PARENT_ID] = ids[1].ToLower() == "n" ? "-1" : ids[1];
					for (int l = 0; l < k; l++)
					{
						if (this.Data.Columns.Contains(ds.Tables[0].Columns[l].ColumnName))
						{
							r[ds.Tables[0].Columns[l].ColumnName] = ds.Tables[0].Rows[j][ds.Tables[0].Columns[l]];
						}
					}
					this.Data.Rows.Add(r);
				}
			}
			return this.Data;
		}

		#endregion

		public virtual ListDataTable ReadAll()
		{
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(GetXmlPattern("selectall")));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectall");
				DbParameter par1 = ParameterFromRow(prows[0]);
				par1.Value = this.DBName;
				return ReadShort(GetSql("selectall"), par1);
			}
		}

		public virtual ListDataTable ReadAllFull()
		{
			if (this.Provider == "xml")
			{
				return this.ReadAll();
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectallfull");
				DbParameter par1 = ParameterFromRow(prows[0]);
				par1.Value = this.DBName;
				return Read(GetSql("selectallfull"), par1);
			}
		}

		public virtual ListDataTable ReadWhere(string where, params DbParameter[] par)
		{
			throw new NotImplementedException("Метод не определен для данного провайдера!");
		}
		public virtual ListDataTable ReadWhere(string where)
		{
			return ReadWhere(where, null);
		}


		public ListDataTable ReadRootParentItems()
		{
			return ReadParentItems(-1);
		}

		public virtual ListDataTable ReadParentItems(int parent_id)
		{
			DataRow[] prows = GetSqlParameters("selectparentitems");
			DbParameter par1 = ParameterFromRow(prows[0]);
			DbParameter par2 = ParameterFromRow(prows[1]);
			if (prows[0]["key"].ToString() == "parent_id")
			{
				par1.Value = parent_id;
				par2.Value = this.DBName;
			}
			else
			{
				par1.Value = this.DBName;
				par2.Value = parent_id;
			}
			return Read(GetSql("selectparentitems"), par1, par2);
		}

		public ListDataTable ReadRootItems()
		{
			return ReadChildren(-1);
/*
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(String.Format(GetXmlPattern("selectchildren"), -1)));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectrootitems");
				DbParameter par1 = ParameterFromRow(prows[0]);
				par1.Value = this.DBName;
				return Read(GetSql("selectrootitems"), par1);
			}*/
		}

		public virtual ListDataTable ReadChildren(int parent_id)
		{
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(String.Format(GetXmlPattern("selectchildren"), parent_id)));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectchildren");
				DbParameter par1 = ParameterFromRow(prows[0]);
				DbParameter par2 = ParameterFromRow(prows[1]);
				if (prows[0]["key"].ToString() == "parent_id")
				{
					par1.Value = parent_id;
					par2.Value = this.DBName;
				}
				else
				{
					par1.Value = this.DBName;
					par2.Value = parent_id;
				}
				return Read(GetSql("selectchildren"), par1, par2);
			}
		}

		public virtual ListDataTable ReadItem(int itemId)
		{
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(String.Format(GetXmlPattern("selectitem"), itemId)));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectitem");
				DbParameter par1 = ParameterFromRow(prows[0]);
				DbParameter par2 = ParameterFromRow(prows[1]);
				if (prows[0]["key"].ToString() == "item_id")
				{
					par1.Value = itemId;
					par2.Value = this.DBName;
				}
				else
				{
					par1.Value = this.DBName;
					par2.Value = itemId;
				}
				return Read(GetSql("selectitem"), par1, par2);
			}
		}
		public virtual ListDataTable ReadByGuid(Guid guid)
		{
			return ReadByGuid(guid.ToString());
		}
		public virtual ListDataTable ReadByGuid(string guid)
		{
			if (this.Provider == "xml")
			{
			}
			else
			{
				IDbCommand cmd = this._connector.CreateCommand();
				DataRow[] prows = null;
				cmd.CommandText = GetSql("selectitemid");
				prows = GetSqlParameters("selectitemid");
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
				((IDbDataParameter) cmd.Parameters[GetParameterName("guid", "selectitemid")]).Value = guid;
				try
				{
					cmd.Connection.Open();
					object res = cmd.ExecuteScalar();
					if(res != null && res != DBNull.Value)
					{
						ReadItem(Convert.ToInt32(res));
					}
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
			return this.Data;
		}
		public virtual ListDataTable ReadNextItem(int itemId, string sort)
		{
			return ReadNextItem(itemId);
		}
		public virtual ListDataTable ReadNextItem(int itemId)
		{
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(String.Format(GetXmlPattern("selectitem"), itemId)));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectnextitem");
				DbParameter par1 = ParameterFromRow(prows[0]);
				par1.Value = itemId;
				return Read(GetSql("selectnextitem"), par1);
			}
		}
		public virtual ListDataTable ReadPrevItem(int itemId, string sort)
		{
			return ReadPrevItem(itemId);
		}
		public virtual ListDataTable ReadPrevItem(int itemId)
		{
			if (this.Provider == "xml")
			{
				return this.ReadXml(this._directory.GetFiles(String.Format(GetXmlPattern("selectitem"), itemId)));
			}
			else
			{
				DataRow[] prows = GetSqlParameters("selectprevitem");
				DbParameter par1 = ParameterFromRow(prows[0]);
				par1.Value = itemId;
				return Read(GetSql("selectprevitem"), par1);
			}
		}
		public virtual ListDataTable FindByField(string field, object value)
		{
			if (this.Provider == "xml")
			{
				return null;
			}
			else
			{
				DataRow[] prows = GetSqlParameters("findbyfield");
				ArrayList arr = new ArrayList();
				for(int i=0;i<prows.Length;i++)
				{
					DbParameter par = ParameterFromRow(prows[i]);
					switch(prows[i]["key"].ToString())
					{
						case "list_name":
							par.Value = DBName;
							break;
						case "field_name":
							par.Value = field;
							break;
						case "value":
							par.Value = GetFieldValue(field, value);
							break;
					}
					arr.Add(par);
				}
				return Read(GetSql("findbyfield"), (DbParameter[])arr.ToArray(typeof(DbParameter)));
			}
		}

		public virtual ListDataTable Select(string sqlFormat, params object[] parameterArgs)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region edit

		#region sql

		public virtual int Execute(string sql)
		{
			return Execute(this._connector.CreateCommand(sql));
		}

		public virtual int Execute(string sql, params DbParameter[] par)
		{
			return Execute(this._connector.CreateCommand(sql, par));
		}

		public virtual int Execute(IDbCommand cmd)
		{
			return Connector.ExecuteNonQuery(cmd);
		}
		public virtual int Execute(string sql, params object[] par)
		{
			throw new NotImplementedException();
		}

		public virtual object ExecuteScalar(string sql)
		{
			return ExecuteScalar(this._connector.CreateCommand(sql));
		}

		public virtual object ExecuteScalar(string sql, params DbParameter[] par)
		{
			return ExecuteScalar(this._connector.CreateCommand(sql, par));
		}

		public virtual object ExecuteScalar(IDbCommand cmd)
		{
			return Connector.ExecuteScalar(cmd);
		}

		public virtual object ExecuteScalar(string sql, params object[] par)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region xml

		//TODO доделать

		#endregion

		#region insert
		public int Insert(Hashtable fields)
		{
			return Insert(-1, 0, 0, fields);
		}
		public int Insert(int deleted, Hashtable fields)
		{
			return Insert(-1, deleted, 0, fields);
		}
		public virtual int Insert(int parentId, int deleted, Hashtable fields)
		{
			int level = 0;
			if(parentId != -1)
			{
				List l = List.Create(this.Name);
				l.ReadItem(parentId);
				level = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]) + 1;
			}
			return Insert(parentId, deleted, level, fields);
		}
		public virtual int Insert(int parentId, int deleted, int level, Hashtable fields)
		{
			int itemId = -1;
			if (this._provider == "xml")
			{
				//TODO
			}
			else
			{
				string sGuid = Guid.NewGuid().ToString();

				IDbCommand cmd = this._connector.CreateCommand();
				DataRow[] prows = null;

				cmd.CommandText = GetSql("selectitemid");
				prows = GetSqlParameters("selectitemid");
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
				((IDbDataParameter) cmd.Parameters[GetParameterName("guid", "selectitemid")]).Value = sGuid;
				try
				{
					cmd.Connection.Open();
					object res = cmd.ExecuteScalar();
					while (res != null)
					{
						sGuid = Guid.NewGuid().ToString();
						((IDbDataParameter) cmd.Parameters[GetParameterName("guid", "selectitemid")]).Value = sGuid;
						res = cmd.ExecuteScalar();
					}
					cmd.Parameters.Clear();
					cmd.CommandText = GetSql("insertitem");
					prows = GetSqlParameters("insertitem");
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[3])));
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[4])));
					((IDbDataParameter) cmd.Parameters[GetParameterName("parent_id", "insertitem")]).Value = parentId;
					((IDbDataParameter) cmd.Parameters[GetParameterName("guid", "insertitem")]).Value = sGuid;
					((IDbDataParameter) cmd.Parameters[GetParameterName("list_name", "insertitem")]).Value = this.DBName;
					((IDbDataParameter) cmd.Parameters[GetParameterName("deleted", "insertitem")]).Value = deleted;
					((IDbDataParameter) cmd.Parameters[GetParameterName("level", "insertitem")]).Value = level;

					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.Parameters.Clear();
						cmd.CommandText = GetSql("selectitemid");
						prows = GetSqlParameters("selectitemid");
						cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
						((IDbDataParameter) cmd.Parameters[GetParameterName("guid", "selectitemid")]).Value = sGuid;
						itemId = Convert.ToInt32(cmd.ExecuteScalar());
						if (fields != null)
						{
							cmd.Parameters.Clear();
							cmd.CommandText = GetSql("insertfield");
							prows = GetSqlParameters("insertfield");
							cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
							cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
							cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
							((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "insertfield")]).Value = itemId;
							string parFN = GetParameterName("field_name", "insertfield");
							string parFV = GetParameterName("value", "insertfield");
							foreach (string key in fields.Keys)
							{
								if (this.Data.Columns.Contains(key))
								{
									bool bLevel = false;
									string levels1 = (string) this.Data.Columns[key].ExtendedProperties["levels"];
									if (Util.IsNullOrEmpty(levels1))
									{
										bLevel = true;
									}
									else
									{
										string[] levels = levels1.Split(',');

										foreach (string s in levels)
										{
											if (int.Parse(s) == level)
											{
												bLevel = true;
												break;
											}
										}
									}
									if (bLevel)
									{
										if (!Util.IsNullOrEmpty(fields[key]))
										{
											((IDbDataParameter) cmd.Parameters[parFN]).Value = key;
											((IDbDataParameter) cmd.Parameters[parFV]).Value = GetFieldValue(key, fields[key]);
											cmd.ExecuteNonQuery();
										}

									}
								}
							}
						}
					}
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
			return itemId;
		}

		#endregion

		#region update
		public virtual void Update(int itemId, Hashtable fields)
		{
			List l = List.Create(this.Name);
			l.ReadItem(itemId);
			int level = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]);
			int parentId = Convert.ToInt32(l.Data.FirstRow[List.FIELD_PARENT_ID]);
			int deleted = Convert.ToInt32(l.Data.FirstRow[List.FIELD_DELETED]);
			Update(itemId, parentId, deleted, level, fields);
		}
		public void Update(int itemId, int deleted)
		{
			Update(itemId, deleted, null);
		}
		public virtual void Update(int itemId, int deleted, Hashtable fields)
		{
			List l = List.Create(this.Name);
			l.ReadItem(itemId);
			int level = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]);
			int parentId = Convert.ToInt32(l.Data.FirstRow[List.FIELD_PARENT_ID]);
			Update(itemId, parentId, deleted, level, fields);
		}
		public void Update(int itemId, int parentId, int deleted)
		{
			Update(itemId, parentId, deleted, null);
		}
		public virtual void Update(int itemId, int parentId, int deleted, Hashtable fields)
		{
			int level = 0;
			if(parentId != -1)
			{
				List l = List.Create(this.Name);
				l.ReadItem(parentId);
				level = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]) + 1;
			}
			Update(itemId, parentId, deleted, level, fields);
		}
		public virtual void Update(int itemId, int parentId, int deleted, int level, Hashtable fields)
		{
			if (this._provider == "xml")
			{
				//TODO
			}
			else
			{
				IDbCommand cmd = this._connector.CreateCommand();
				DataRow[] prows = null;

				cmd.CommandText = GetSql("updateitem");
				prows = GetSqlParameters("updateitem");
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[3])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[4])));
				((IDbDataParameter) cmd.Parameters[GetParameterName("parent_id", "updateitem")]).Value = parentId;
				((IDbDataParameter) cmd.Parameters[GetParameterName("list_name", "updateitem")]).Value = this.DBName;
				((IDbDataParameter) cmd.Parameters[GetParameterName("deleted", "updateitem")]).Value = deleted;
				((IDbDataParameter) cmd.Parameters[GetParameterName("level", "updateitem")]).Value = level;
				((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "updateitem")]).Value = itemId;

				try
				{
					cmd.Connection.Open();
					if (cmd.ExecuteNonQuery() > 0 && fields != null)
					{
						cmd.CommandText = GetSql("deleteallfields");
						prows = GetSqlParameters("deleteallfields");
						cmd.Parameters.Clear();
						cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
						((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "deleteallfields")]).Value = itemId;
						cmd.ExecuteNonQuery();

						cmd.CommandText = GetSql("insertfield");
						cmd.Parameters.Clear();
						prows = GetSqlParameters("insertfield");
						cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
						cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
						cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
						((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "insertfield")]).Value = itemId;
						string parFN = GetParameterName("field_name", "insertfield");
						string parFV = GetParameterName("value", "insertfield");
						foreach (string key in fields.Keys)
						{
							if (this.Data.Columns.Contains(key))
							{
								bool bLevel = false;
								string levels1 = (string) this.Data.Columns[key].ExtendedProperties["levels"];
								if (levels1 == string.Empty || levels1 == null)
								{
									bLevel = true;
								}
								else
								{
									string[] levels = levels1.Split(',');

									foreach (string s in levels)
									{
										if (int.Parse(s) == level)
										{
											bLevel = true;
											break;
										}
									}
								}
								if (bLevel)
								{
									if (!Util.IsNullOrEmpty(fields[key]))										
									{
										((IDbDataParameter) cmd.Parameters[parFN]).Value = key;
										((IDbDataParameter) cmd.Parameters[parFV]).Value = GetFieldValue(key, fields[key]);
										cmd.ExecuteNonQuery();
									}
								}
							}
						}
					}
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
		}

		public void Update(int itemId, int parentId, int deleted, int level)
		{
			Update(itemId, parentId, deleted, level, null);
		}

		public virtual void Update(DataRow row)
		{
			Hashtable h = new Hashtable();
			foreach(DataColumn col in this.Data.Columns)
			{
				if(List.IsCustomField(col.ColumnName))
				{
					string it = col.ExtendedProperties["inputtype"].ToString();
					if(it=="date")
					{
						h[col.ColumnName] = Convert.ToDateTime(row[col.ColumnName]).ToString("yyyy-MM-dd");
					}
					else if(it=="time")
					{
						h[col.ColumnName] = Convert.ToDateTime(row[col.ColumnName]).ToLongTimeString();
					}
					else
					{
						h[col.ColumnName] = row[col.ColumnName];
					}
				}
			}
			Update(
				Convert.ToInt32(row[List.FIELD_ID]), 
				Convert.ToInt32(row[List.FIELD_PARENT_ID]), 
				Convert.ToInt32(row[List.FIELD_DELETED]), 
				Convert.ToInt32(row[List.FIELD_LEVEL]), 
				h);
		}

		/// <summary>
		/// Используется при смене родителя, если новый родитель другого уровня
		/// </summary>
		/// <param name="itemID">Код элемента</param>
		/// <param name="newParentID">Код нового родителя</param>
		public virtual void ChangeParent(int itemID, int newParentID)
		{
			List l = List.Create(this.Name);
			int level = 0;
			if(newParentID != -1)
			{
				l.ReadItem(newParentID);
				level = Convert.ToInt32(l.Data.FirstRow[List.FIELD_LEVEL]) + 1;
				l.Clear();
			}
			l.ReadItem(itemID);
			int deleted = Convert.ToInt32(l.Data.FirstRow[List.FIELD_DELETED]);
			changeParentAndLevel(itemID, newParentID, deleted, level);
		}
		private void changeParentAndLevel(int itemID, int newParentID, int deleted, int level)
		{
			List l = List.Create(this.Name);
			Update(itemID, newParentID, deleted, level);
			l.Clear();
			l.ReadChildren(itemID);
			if(CanBeParent(level))
				for(int i=0;i<l.Data.Rows.Count;i++)
				{
					changeParentAndLevel(
						Convert.ToInt32(l.Data.Rows[i][List.FIELD_ID]),
						itemID,
						Convert.ToInt32(l.Data.Rows[i][List.FIELD_DELETED]),
						level + 1
						);
				}
			else
				for(int i=0;i<l.Data.Rows.Count;i++)
				{
					Delete(Convert.ToInt32(l.Data.Rows[i][List.FIELD_ID]));
				}
		}
		#endregion

		#region delete

		public virtual bool Delete(int itemId)
		{
			bool retVal = false;
			if (this._provider == "xml")
			{
				//TODO
			}
			else
			{
				IDbCommand cmd = this._connector.CreateCommand();
				DataRow[] prows = null;

				cmd.CommandText = GetSql("deleteallfields");
				prows = GetSqlParameters("deleteallfields");
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
				((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "deleteallfields")]).Value = itemId;

				try
				{
					cmd.Connection.Open();
					cmd.ExecuteNonQuery();

					cmd.Parameters.Clear();
					cmd.CommandText = GetSql("deleteitem");
					prows = GetSqlParameters("deleteitem");
					cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
					((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "deleteitem")]).Value = itemId;
					if (cmd.ExecuteNonQuery() > 0)
					{
						retVal = true;
					}
				}
				finally
				{
					cmd.Connection.Close();
				}
			}
			return retVal;
		}
		public virtual bool DeleteTree(int itemID)
		{
			List l = List.Create(this.Name);
			l.ReadChildren(itemID);
			for(int i=0;i<l.Data.Rows.Count;i++)
			{
				DeleteTree(Convert.ToInt32(l.Data.Rows[i][List.FIELD_ID]));
			}
			return Delete(itemID);
		}
		#endregion

		#region fields

		public virtual void InsertField(int itemId, string field, string value)
		{
			IDbCommand cmd = this._connector.CreateCommand(GetSql("insertfield"));
			DataRow[] prows = GetSqlParameters("insertfield");
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
			((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "insertfield")]).Value = itemId;
			((IDbDataParameter) cmd.Parameters[GetParameterName("field_name", "insertfield")]).Value = field;
			((IDbDataParameter) cmd.Parameters[GetParameterName("value", "insertfield")]).Value = value;
			Connector.ExecuteNonQuery(cmd);
		}

		public virtual void DeleteField(int itemId, string field)
		{
			IDbCommand cmd = this._connector.CreateCommand(GetSql("deletefield"));
			DataRow[] prows = GetSqlParameters("deletefield");
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
			((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "deletefield")]).Value = itemId;
			((IDbDataParameter) cmd.Parameters[GetParameterName("field_name", "deletefield")]).Value = field;
			Connector.ExecuteNonQuery(cmd);

		}

		public virtual void UpdateField(int itemId, string field, object value)
		{
			IDbCommand cmd = this._connector.CreateCommand(GetSql("deletefield"));
			DataRow[] prows = GetSqlParameters("deletefield");
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
			cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
			((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "deletefield")]).Value = itemId;
			((IDbDataParameter) cmd.Parameters[GetParameterName("field_name", "deletefield")]).Value = field;
			try
			{
				cmd.Connection.Open();
				cmd.ExecuteNonQuery();
				cmd.CommandText = GetSql("insertfield");
				cmd.Parameters.Clear();
				prows = GetSqlParameters("insertfield");
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[0])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[1])));
				cmd.Parameters.Add(this._connector.CreateParameter(ParameterFromRow(prows[2])));
				((IDbDataParameter) cmd.Parameters[GetParameterName("item_id", "insertfield")]).Value = itemId;
				((IDbDataParameter) cmd.Parameters[GetParameterName("field_name", "insertfield")]).Value = field;
				((IDbDataParameter) cmd.Parameters[GetParameterName("value", "insertfield")]).Value = value;
				cmd.ExecuteNonQuery();
			}
			finally
			{
				cmd.Connection.Close();
			}
		}

		#endregion

		#endregion

		#region fields & properties

		protected ListDataTable tbData;
		protected DataTable tbParameters;
		protected DataTable tbSqls;
		protected DataTable tbListTable;
		protected DataTable tbValuesTable;
		protected DataTable tbTreeCaption;
		protected DataTable tbEditor;
		protected DataTable tbLevelIcon;
		protected DataTable tbLevelView;
		protected DataTable tbDeleteQuestion;
		protected Connector _connector;
		protected DirectoryInfo _directory;
		protected DataTable tbXmlPattern;

		public ListDataTable Data
		{
			get { return tbData; }
		}

		protected string _name;

		public string Name
		{
			get { return _name; }
		}
		
		public string DBName
		{
			get
			{
				if(Path.Site.Length > 0)
				{
					return Path.Site + "__" + _name;
				}
				return _name;
			}
		}

		protected bool _searchIn;

		public bool SearchIn
		{
			get { return _searchIn; }
		}

		protected string _caption;

		public string Caption
		{
			get { return _caption; }
		}

		public string TreeSortExpression
		{
			get { return this.tbEditor.Rows[0]["sort_expression"].ToString(); }
		}

		public string TreeRootIcon
		{
			get { return this.tbEditor.Rows[0]["root_icon"].ToString(); }
		}

		public bool TreeNoIcon
		{
			get { return this.tbEditor.Rows[0]["no_icon"].ToString() == "1"; }
		}

		public string CloseAllIcon
		{
			get { return this.tbEditor.Rows[0]["closeall_icon"].ToString(); }
		}

		public string OpenAllIcon
		{
			get { return this.tbEditor.Rows[0]["openall_icon"].ToString(); }
		}

		public int LevelCount
		{
			get { return Convert.ToInt32(this.tbEditor.Rows[0]["levels"]); }
		}

		public int SecurityPart
		{
			get { return Convert.ToInt32(this.tbEditor.Rows[0]["part"]); }
		}

		protected string _file;

		public string File
		{
			get { return _file; }
		}

		protected string _provider;

		public string Provider
		{
			get { return _provider; }
		}

		protected string _connectionString;

		public string ConnectionString
		{
			get { return _connectionString; }
		}

		public Connector Connector
		{
			get { return _connector; }
		}

		#endregion

		#region search

		protected DataTable tbLevelSearchUrl;
		protected DataTable tbLevelSearchTitle;

		public DataTable LevelSearchUrl
		{
			get
			{
				if (tbLevelSearchUrl == null)
				{
					FillSearchConfig();
				}
				return tbLevelSearchUrl;
			}
		}

		protected void FillSearchConfig()
		{
			DataSet ds = GetSchema(this._file);
			tbLevelSearchUrl = ds.Tables["search_url"];
			tbLevelSearchTitle = ds.Tables["search_title"];
		}

		public DataTable LevelSearchTitle
		{
			get
			{
				if (tbLevelSearchTitle == null)
				{
					FillSearchConfig();
				}
				return tbLevelSearchTitle;
			}
		}

		public string GetLevelSearchUrl(int level)
		{
			string lv = getLevelSearchUrl(level.ToString());
			if (lv != null)
				return lv;
			else
				return getLevelSearchUrl("default");

		}

		protected string getLevelSearchUrl(string level)
		{
			DataRow[] row = LevelSearchUrl.Select("[level]='" + level + "'");
			if (row.Length > 0)
			{
				return row[0]["url"].ToString();
			}
			return null;
		}

		public string GetLevelSearchTitle(int level)
		{
			string lv = getLevelSearchTitle(level.ToString());
			if (lv != null)
				return lv;
			else
				return getLevelSearchTitle("default");

		}

		protected string getLevelSearchTitle(string level)
		{
			DataRow[] row = LevelSearchTitle.Select("[level]='" + level + "'");
			if (row.Length > 0)
			{
				return row[0]["title"].ToString();
			}
			return null;
		}

		protected virtual ListDataTable ReadSearch(string text)
		{
			DataRow[] prows = GetSqlParameters("search");
			DbParameter par1 = ParameterFromRow(prows[0]);
			DbParameter par2 = ParameterFromRow(prows[1]);
			if (prows[0]["key"].ToString().ToLower() == "text")
			{
				par1.Value = text;
				par2.Value = this.DBName;
			}
			else
			{
				par2.Value = text;
				par1.Value = this.DBName;
			}
			return Read(GetSql("search"), par1, par2);
		}

		public virtual DataTable SearchList(string text, int resultBodyLength, string prefix, string postfix)
		{
			DataTable tb = SearchUtil.CreateResultTable();
			if (!this._searchIn || text==null || text.Length==0)
				return tb;
			ReadSearch(text);
			foreach (DataRow row in this.Data.Rows)
			{
				DataRow r = tb.NewRow();
				r["href"] = ParseExpression(row, GetLevelSearchUrl(Convert.ToInt32(row[List.FIELD_LEVEL])));
				r["title"] = ParseExpression(row, GetLevelSearchTitle(Convert.ToInt32(row[List.FIELD_LEVEL])));
				string body = string.Empty;
				foreach (DataColumn col in this.Data.Columns)
				{
					if (IsCustomField(col.ColumnName))
					{
						string b = SearchUtil.SearchTheText(row[col.ColumnName].ToString(), text, resultBodyLength, prefix, postfix);
						if (b != null && b.Length > 0)
						{
							body = b;
							break;
						}
					}
				}
				r["body"] = body;
				tb.Rows.Add(r);
			}
			return tb;
		}

		#region ISupportsSearch Members

		public DataTable Search(string text)
		{
			return Search(text, Keys.DefaultSearchResultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public System.Data.DataTable Search(string text, int resultBodyLength)
		{
			return Search(text, resultBodyLength, Keys.DefaultSearchPrefix, Keys.DefaultSearchPostfix);
		}

		public System.Data.DataTable Search(string text, int resultBodyLength, string prefix, string postfix)
		{
			DataTable tb = SearchUtil.CreateResultTable();
			DataTable tbL = List.GetLists();
			foreach (DataRow row in tbL.Rows)
			{
				DataTable tb1 = List.Create(row["name"].ToString()).SearchList(text, resultBodyLength, prefix, postfix);
				int m = tb1.Rows.Count;
				for (int j = 0; j < m; j++)
				{
					SearchUtil.ImportRow(tb1.Rows[j],tb);
				}
			}
			return tb;
		}

		#endregion

		#endregion
	}
}