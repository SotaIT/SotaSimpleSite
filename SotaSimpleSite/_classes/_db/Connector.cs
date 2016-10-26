using System;
using System.Data;
using sql	= System.Data.SqlClient;
using oledb	= System.Data.OleDb;
using odbc	= System.Data.Odbc;

namespace Sota.Data.Simple
{
	/// <summary>
	/// Класс для универсального доступа к БД
	/// </summary>
	public sealed class Connector
	{
		public static ProviderType ParseProviderType(string providerType)
		{
			return (ProviderType) Enum.Parse(typeof (ProviderType), providerType, true);
		}

		#region constructors

		public Connector(string connectionString)
		{
			Init(connectionString, ProviderType.Sql);
		}

		public Connector(string connectionString, ProviderType providerType)
		{
			Init(connectionString, providerType);
		}

		public Connector(string connectionString, string providerType)
		{
			Init(connectionString, ParseProviderType(providerType));
		}

		private void Init(string connectionString, ProviderType providerType)
		{
			this._providerType = providerType;
			this._connectionString = connectionString;
		}

		#endregion

		#region properties & fields

		private ProviderType _providerType;

		public ProviderType ProviderType
		{
			get { return this._providerType; }
			set { this._providerType = value; }
		}

		private string _connectionString;

		public string ConnectionString
		{
			get { return this._connectionString; }
			set { this._connectionString = value; }
		}

		#endregion

		#region get connection

		internal IDbConnection GetConnection()
		{
			switch (this.ProviderType)
			{
				case ProviderType.Odbc:
					return new odbc.OdbcConnection(this.ConnectionString);
				case ProviderType.OleDb:
					return new oledb.OleDbConnection(this.ConnectionString);
				case ProviderType.Sql:
					return new sql.SqlConnection(this.ConnectionString);
			}
			return null;
		}

		#endregion

		#region create parameter

		public IDbDataParameter CreateParameter(DbParameter par)
		{
			IDbDataParameter p = null;
			switch (this.ProviderType)
			{
//				case ProviderType.MySql:
//					p = new mysql.MySqlParameter();
//					break;
				case ProviderType.Odbc:
					p = new odbc.OdbcParameter();
					break;
				case ProviderType.OleDb:
					p = new oledb.OleDbParameter();
					break;
//				case ProviderType.Oracle:
//					p = new ora.OracleParameter();
//					break;
				case ProviderType.Sql:
					p = new sql.SqlParameter();
					break;
			}
			if (p != null)
			{
				p.ParameterName = par.ParameterName;
				p.Direction = par.Direction;
				p.Value = par.Value;
				p.DbType = par.DbType;
			}
			return p;
		}

		#endregion

		#region create command

		public DbCommand CreateCommand()
		{
			return new DbCommand(GetConnection().CreateCommand());
		}

		public DbCommand CreateCommand(string commandText, params DbParameter[] par)
		{
			DbCommand cmd = CreateCommand();
			cmd.CommandText = commandText;
			int n = par.Length;
			for (int i = 0; i < n; i++)
			{
				IDbDataParameter p = this.CreateParameter(par[i]);
				cmd.Parameters.Add(p);
			}
			return cmd;
		}

		public DbCommand CreateCommand(string commandText)
		{
			DbCommand cmd = CreateCommand();
			cmd.CommandText = commandText;
			return cmd;
		}

		public DbCommand CreateCommand(params DbParameter[] par)
		{
			return CreateCommand("", par);
		}

		#endregion

		#region execute

		public static IDataReader ExecuteReader(IDbCommand command)
		{
			command.Connection.Open();
			return command.ExecuteReader(CommandBehavior.CloseConnection);
		}

		public IDataReader ExecuteReader(string commandText, params DbParameter[] par)
		{
			return ExecuteReader(CreateCommand(commandText, par));
		}

		public IDataReader ExecuteReader(string commandText)
		{
			return ExecuteReader(CreateCommand(commandText));
		}

		public static object ExecuteScalar(IDbCommand command)
		{
			try
			{
				command.Connection.Open();
				return command.ExecuteScalar();
			}
			finally
			{
				command.Connection.Close();
			}
		}

		public object ExecuteScalar(string commandText, params DbParameter[] par)
		{
			return ExecuteScalar(CreateCommand(commandText, par));
		}

		public object ExecuteScalar(string commandText)
		{
			return ExecuteScalar(CreateCommand(commandText));
		}

		public static int ExecuteNonQuery(IDbCommand command)
		{
			try
			{
				command.Connection.Open();
				return command.ExecuteNonQuery();
			}
			finally
			{
				command.Connection.Close();
			}
		}

		public int ExecuteNonQuery(string commandText, params DbParameter[] par)
		{
			return ExecuteNonQuery(CreateCommand(commandText, par));
		}

		public int ExecuteNonQuery(string commandText)
		{
			return ExecuteNonQuery(CreateCommand(commandText));
		}

		#endregion

		#region data adapter methods

		private IDbDataAdapter GetAdapter()
		{
			IDbDataAdapter adapter = null;
			switch (this.ProviderType)
			{
//				case ProviderType.MySql:
//					adapter = new mysql.MySqlDataAdapter();
//					break;
				case ProviderType.Odbc:
					adapter = new odbc.OdbcDataAdapter();
					break;
				case ProviderType.OleDb:
					adapter = new oledb.OleDbDataAdapter();
					break;
//				case ProviderType.Oracle:
//					adapter = new ora.OracleDataAdapter();
//					break;
				case ProviderType.Sql:
					adapter = new sql.SqlDataAdapter();
					break;
			}
			return adapter;
		}

		private IDbDataAdapter GetAdapter(IDbCommand command)
		{
			IDbDataAdapter adapter = GetAdapter();
			if (typeof (DbCommand) == command.GetType())
				adapter.SelectCommand = ((DbCommand) command).InnerCommand;
			else
				adapter.SelectCommand = command;
			return adapter;
		}

		public void Fill(DataSet dataSet, IDbCommand command)
		{
			GetAdapter(command).Fill(dataSet);
		}

		public void Fill(DataSet dataSet, string commandText, params DbParameter[] par)
		{
			Fill(dataSet, CreateCommand(commandText, par));
		}

		public void Fill(DataSet dataSet, string commandText)
		{
			Fill(dataSet, CreateCommand(commandText));
		}

		public DataSet Fill(IDbCommand command)
		{
			DataSet ds = new DataSet();
			Fill(ds, command);
			return ds;
		}

		public DataSet Fill(string commandText, params DbParameter[] par)
		{
			return Fill(CreateCommand(commandText, par));
		}

		public DataSet Fill(string commandText)
		{
			return Fill(CreateCommand(commandText));
		}

		#endregion
	}
}