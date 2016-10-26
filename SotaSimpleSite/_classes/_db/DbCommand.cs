using System.Data;

namespace Sota.Data.Simple
{
	/// <summary>
	/// ”ниверсальный Command
	/// </summary>
	public class DbCommand : IDbCommand
	{
		public DbCommand(IDbCommand innerCommand)
		{
			_command = innerCommand;
		}

		private IDbCommand _command = null;

		public IDbCommand InnerCommand
		{
			get { return _command; }
		}

		public IDbDataParameter GetParameter(int index)
		{
			return (IDbDataParameter) _command.Parameters[index];
		}

		public IDbDataParameter GetParameter(string name)
		{
			return (IDbDataParameter) _command.Parameters[name];
		}

		#region IDbCommand Members

		public void Cancel()
		{
			_command.Cancel();
		}

		public void Prepare()
		{
			_command.Prepare();
		}

		public System.Data.CommandType CommandType
		{
			get { return _command.CommandType; }
			set { _command.CommandType = value; }
		}

		public IDataReader ExecuteReader(System.Data.CommandBehavior behavior)
		{
			return _command.ExecuteReader(behavior);
		}

		public IDataReader ExecuteReader()
		{
			return _command.ExecuteReader();
		}

		public object ExecuteScalar()
		{
			return _command.ExecuteScalar();
		}

		public int ExecuteNonQuery()
		{
			return _command.ExecuteNonQuery();
		}

		public int CommandTimeout
		{
			get { return _command.CommandTimeout; }
			set { _command.CommandTimeout = value; }
		}

		public IDbDataParameter CreateParameter()
		{
			return _command.CreateParameter();
		}

		public IDbConnection Connection
		{
			get { return _command.Connection; }
			set { _command.Connection = value; }
		}

		public System.Data.UpdateRowSource UpdatedRowSource
		{
			get { return _command.UpdatedRowSource; }
			set { _command.UpdatedRowSource = value; }
		}

		public string CommandText
		{
			get { return _command.CommandText; }
			set { _command.CommandText = value; }
		}

		public IDataParameterCollection Parameters
		{
			get { return _command.Parameters; }
		}

		public IDbTransaction Transaction
		{
			get { return _command.Transaction; }
			set { _command.Transaction = value; }
		}

		#endregion

		#region IDisposable Members

		public void Dispose()
		{
			_command.Dispose();
		}

		#endregion
	}
}