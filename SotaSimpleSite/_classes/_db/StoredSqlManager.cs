using System.Data;

namespace Sota.Data.Simple
{
	/// <summary>
	/// Считывание SQL-выражений из xml-файлов.
	/// </summary>
	public sealed class StoredSqlManager
	{
		/// <summary>
		/// Создает StoredSqlManager на основе XML-файла,
		/// содержащего описания выражений
		/// </summary>
		/// <param name="filename">полный путь к файлу</param>
		public StoredSqlManager(string filename)
		{
			ReadFile(filename);
		}

		/// <summary>
		/// Создает StoredSqlManager на основе уже 
		/// заполненных таблиц описания выражений
		/// </summary>
		/// <param name="sql">Таблица описания SQL-выражений</param>
		/// <param name="parameter">Таблица описания параметров выражений</param>
		public StoredSqlManager(DataTable sql, DataTable parameter)
		{
			_sql = sql;
			_parameter = parameter;
		}

		/// <summary>
		/// Создает StoredSqlManager на основе XML-файла,
		/// содержащего описания выражений
		/// </summary>
		/// <param name="filename">полный путь к файлу</param>
		/// <param name="providerType">Тип провайдера</param>
		public StoredSqlManager(string filename, ProviderType providerType)
		{
			ReadFile(filename);
			_provider = providerType.ToString();
		}

		/// <summary>
		/// Создает StoredSqlManager на основе уже 
		/// заполненных таблиц описания выражений
		/// </summary>
		/// <param name="sql">Таблица описания SQL-выражений</param>
		/// <param name="parameter">Таблица описания параметров выражений</param>
		/// <param name="providerType">Тип провайдера</param>
		public StoredSqlManager(DataTable sql, DataTable parameter, ProviderType providerType)
		{
			_sql = sql;
			_parameter = parameter;
			_provider = providerType.ToString();
		}

		private void ReadFile(string filename)
		{
			DataSet ds = new DataSet();
			ds.ReadXml(filename);
			_sql = ds.Tables["sql"];
			_parameter = ds.Tables["sql_parameter"];
		}

		private DataTable _parameter;
		private DataTable _sql;
		private string _provider = string.Empty;

		/// <summary>
		/// Получает текст выражения SQL
		/// </summary>
		/// <param name="key">идентификатор выражения SQL</param>
		/// <returns>Текст выражения SQL</returns>
		public string GetSql(string key)
		{
			return _sql.Select("key='" + key + "'")[0]["value"].ToString();
		}

		/// <summary>
		/// Получает список параметров указанного выражения
		/// </summary>
		/// <param name="key">идентификатор выражения SQL</param>
		/// <returns>Массив DbParameter[]</returns>
		public DbParameter[] GetSqlParameters(string key)
		{
			DbParameter[] ar = null;
			DataRow[] arr = _parameter.Select("(sqlkey='" + key + "')" + (_provider == string.Empty ? "" : " AND (provider='" + _provider + "')"), "[order] ASC");
			if (arr.Length > 0)
			{
				int n = arr.Length;
				ar = new DbParameter[n];
				for (int i = 0; i < n; i++)
				{
					ar[i] = ParameterFromRow(arr[i]);
				}
			}
			return ar;
		}

		/// <summary>
		/// Получает имя параметра для использования в выражении
		/// </summary>
		/// <param name="key">идентификатор параметра</param>
		/// <param name="sqlKey">идентификатор выражения SQl</param>
		/// <returns>Имя указанного параметра</returns>
		public string GetParameterName(string key, string sqlKey)
		{
			return _parameter.Select("(key='" + key + "') AND (sqlkey='" + sqlKey + "')" + (_provider == string.Empty ? "" : " AND (provider='" + _provider + "')"))[0]["parametername"].ToString();
		}

		/// <summary>
		/// Создает экземпляр Sota.Data.Simple.DbParameter на основе информации из DataRow
		/// </summary>
		/// <param name="row">Строка, содержащая информацию о параметре</param>
		/// <returns>Экземпляр DbParameter</returns>
		public DbParameter ParameterFromRow(DataRow row)
		{
			return new DbParameter(row["parametername"].ToString(), DbParameter.ParseDbType(row["dbtype"].ToString()));
		}
	}
}