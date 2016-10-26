using System.Data;

namespace Sota.Data.Simple
{
	/// <summary>
	/// ���������� SQL-��������� �� xml-������.
	/// </summary>
	public sealed class StoredSqlManager
	{
		/// <summary>
		/// ������� StoredSqlManager �� ������ XML-�����,
		/// ����������� �������� ���������
		/// </summary>
		/// <param name="filename">������ ���� � �����</param>
		public StoredSqlManager(string filename)
		{
			ReadFile(filename);
		}

		/// <summary>
		/// ������� StoredSqlManager �� ������ ��� 
		/// ����������� ������ �������� ���������
		/// </summary>
		/// <param name="sql">������� �������� SQL-���������</param>
		/// <param name="parameter">������� �������� ���������� ���������</param>
		public StoredSqlManager(DataTable sql, DataTable parameter)
		{
			_sql = sql;
			_parameter = parameter;
		}

		/// <summary>
		/// ������� StoredSqlManager �� ������ XML-�����,
		/// ����������� �������� ���������
		/// </summary>
		/// <param name="filename">������ ���� � �����</param>
		/// <param name="providerType">��� ����������</param>
		public StoredSqlManager(string filename, ProviderType providerType)
		{
			ReadFile(filename);
			_provider = providerType.ToString();
		}

		/// <summary>
		/// ������� StoredSqlManager �� ������ ��� 
		/// ����������� ������ �������� ���������
		/// </summary>
		/// <param name="sql">������� �������� SQL-���������</param>
		/// <param name="parameter">������� �������� ���������� ���������</param>
		/// <param name="providerType">��� ����������</param>
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
		/// �������� ����� ��������� SQL
		/// </summary>
		/// <param name="key">������������� ��������� SQL</param>
		/// <returns>����� ��������� SQL</returns>
		public string GetSql(string key)
		{
			return _sql.Select("key='" + key + "'")[0]["value"].ToString();
		}

		/// <summary>
		/// �������� ������ ���������� ���������� ���������
		/// </summary>
		/// <param name="key">������������� ��������� SQL</param>
		/// <returns>������ DbParameter[]</returns>
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
		/// �������� ��� ��������� ��� ������������� � ���������
		/// </summary>
		/// <param name="key">������������� ���������</param>
		/// <param name="sqlKey">������������� ��������� SQl</param>
		/// <returns>��� ���������� ���������</returns>
		public string GetParameterName(string key, string sqlKey)
		{
			return _parameter.Select("(key='" + key + "') AND (sqlkey='" + sqlKey + "')" + (_provider == string.Empty ? "" : " AND (provider='" + _provider + "')"))[0]["parametername"].ToString();
		}

		/// <summary>
		/// ������� ��������� Sota.Data.Simple.DbParameter �� ������ ���������� �� DataRow
		/// </summary>
		/// <param name="row">������, ���������� ���������� � ���������</param>
		/// <returns>��������� DbParameter</returns>
		public DbParameter ParameterFromRow(DataRow row)
		{
			return new DbParameter(row["parametername"].ToString(), DbParameter.ParseDbType(row["dbtype"].ToString()));
		}
	}
}