using System;
using System.Data;

namespace Sota.Data.Simple
{
	/// <summary>
	/// Параметр для команды.
	/// </summary>
	public class DbParameter
	{
		public static DbType ParseDbType(string dbType)
		{
			return (DbType) Enum.Parse(typeof (DbType), dbType, true);
		}

		#region constructors

		public DbParameter(string parameterName, DbType dbType, object value, ParameterDirection direction)
		{
			this._parameterName = parameterName;
			this._dbType = dbType;
			this._value = value;
			this._direction = direction;
		}

		public DbParameter(string parameterName, DbType dbType, object value)
		{
			this._parameterName = parameterName;
			this._dbType = dbType;
			this._value = value;
		}

		public DbParameter(string parameterName, DbType dbType, ParameterDirection direction)
		{
			this._parameterName = parameterName;
			this._dbType = dbType;
			this._direction = direction;
		}

		public DbParameter(string parameterName, object value, ParameterDirection direction)
		{
			this._parameterName = parameterName;
			this._value = value;
			this._direction = direction;
		}

		public DbParameter(string parameterName, ParameterDirection direction)
		{
			this._parameterName = parameterName;
			this._direction = direction;
		}

		public DbParameter(string parameterName, object value)
		{
			this._parameterName = parameterName;
			this._value = value;
		}

		public DbParameter(string parameterName, DbType dbType)
		{
			this._parameterName = parameterName;
			this._dbType = dbType;
		}

		public DbParameter(string parameterName)
		{
			this._parameterName = parameterName;
		}

		#endregion

		private string _parameterName = "NoName";

		public string ParameterName
		{
			get { return _parameterName; }
			set { _parameterName = value; }
		}

		private object _value = null;

		public object Value
		{
			get { return _value; }
			set { _value = value; }
		}

		private DbType _dbType = DbType.String;

		public DbType DbType
		{
			get { return _dbType; }
			set { _dbType = value; }
		}

		private ParameterDirection _direction = ParameterDirection.Input;

		public ParameterDirection Direction
		{
			get { return _direction; }
			set { _direction = value; }
		}
	}
}