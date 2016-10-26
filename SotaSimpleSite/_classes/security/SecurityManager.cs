using System;
using System.Collections;
using System.Data;
using System.Web;
using System.Web.Caching;
using Sota.Data.Simple;
using System.Text;

namespace Sota.Web.SimpleSite.Security
{
	/// <summary>
	/// ������������ ��� ���������������� �������.
	/// </summary>
	public sealed class SecurityManager
	{
		private SecurityManager()
		{
		}

		#region utils


		/// <summary>
		/// ��������� ��� ����������� � ��
		/// </summary>
		public static Sota.Data.Simple.Connector Connector
		{
			get
			{
				Sota.Data.Simple.Connector connector = (Sota.Data.Simple.Connector)HttpContext.Current.Cache[Keys.KeySecurityConnector];
				if (connector == null)
				{
					DataTable tb = Config.GetConfigTableNoCache(Keys.ConfigSecurity, "common");
					connector = new Sota.Data.Simple.Connector(tb.Rows[0]["connectionstring"].ToString().Trim().Length == 0 ? Config.Main.ConnectionString : tb.Rows[0]["connectionstring"].ToString(), tb.Rows[0]["provider"].ToString());
					HttpContext.Current.Cache.Insert(Keys.KeySecurityConnector, connector, new CacheDependency(
						new string[]{
										HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigSecurity),
										HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigMain)
							}));
				}
				return connector;
			}
		}


		/// <summary>
		/// �������� SQL-���������
		/// </summary>
		public static Sota.Data.Simple.StoredSqlManager Stored
		{
			get
			{
				Sota.Data.Simple.StoredSqlManager stored = (Sota.Data.Simple.StoredSqlManager)HttpContext.Current.Cache[Keys.KeySecurityStoredSqlManager];
				if (stored == null)
				{
					stored = new StoredSqlManager(Config.GetConfigTableNoCache(Keys.ConfigSecurity, "sql"), Config.GetConfigTable(Keys.ConfigSecurity, "sql_parameter"));
					HttpContext.Current.Cache.Insert(Keys.KeySecurityStoredSqlManager, stored, new CacheDependency(HttpContext.Current.Request.MapPath(Config.ConfigFolderPath + Keys.UrlPathDelimiter + Keys.ConfigSecurity)));
				}
				return stored;
			}
		}

		#endregion

		#region parts

		/// <summary>
		/// ������� ������ ������������
		/// </summary>
		/// <param name="name">�������� �������</param>
		/// <returns>��� ���������� �������</returns>
		public static int CreatePart(string name)
		{
			string ssql = Stored.GetSql("selectpartid");
			DbCommand cmd = Connector.CreateCommand();
			cmd.CommandText = ssql;
			DbParameter par1 = Stored.GetSqlParameters("selectpartid")[0];
			par1.Value = name;
			cmd.Parameters.Add(Connector.CreateParameter(par1));
			try
			{
				cmd.Connection.Open();
				if (cmd.ExecuteScalar() == null)
				{
					cmd.CommandText = Stored.GetSql("insertpart");
					cmd.Parameters.Clear();
					DbParameter par2 = Stored.GetSqlParameters("insertpart")[0];
					par2.Value = name;
					cmd.Parameters.Add(Connector.CreateParameter(par2));
					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.CommandText = ssql;
						cmd.Parameters.Clear();
						cmd.Parameters.Add(Connector.CreateParameter(par1));
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}

		/// <summary>
		/// ��������� ����� �������
		/// </summary>
		/// <param name="partId">��� �������</param>
		/// <param name="name">����� ��� �������</param>
		/// <returns>����� ����������� �������</returns>
		public static int UpdatePart(int partId, string name)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("updatepart"), Stored.GetSqlParameters("updatepart"));
			cmd.GetParameter(Stored.GetParameterName("part_id", "updatepart")).Value = partId;
			cmd.GetParameter(Stored.GetParameterName("part_name", "updatepart")).Value = name;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� �������
		/// </summary>
		/// <param name="partId">��� �������</param>
		/// <returns>����� ��������� �������</returns>
		public static int DeletePart(int partId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("deletepart"), Stored.GetSqlParameters("deletepart"));
			cmd.GetParameter(0).Value = partId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ������ �������� ������������
		/// </summary>
		/// <returns>������ DataTable, ���������� ������ ��������</returns>
		public static DataTable GetParts()
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "part_table").Rows[0];
			string sId = tableStructureRow["partid"].ToString();
			string sName = tableStructureRow["partname"].ToString();
			DataTable tb = new DataTable("parts");
			tb.Columns.Add("partid", typeof (int));
			tb.Columns.Add("partname", typeof (string));
			tb.Rows.Add(new object[] {-3, GetInternalPartName(-3)});
			tb.Rows.Add(new object[] {-2, GetInternalPartName(-2)});
			tb.Rows.Add(new object[] {0, GetInternalPartName(0)});
			tb.Rows.Add(new object[] {-1, GetInternalPartName(-1)});

			if (Config.Main.AuthorizationEnabled)
			{
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectparts"));
					while (r.Read())
					{
						tb.Rows.Add(new object[] {r[sId], r[sName]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;
		}

		#endregion

		#region group

		/// <summary>
		/// ������� ������ ������������
		/// </summary>
		/// <param name="name">�������� ������</param>
		/// <returns>��� ��������� ������</returns>
		public static int CreateGroup(string name)
		{
			string ssql = Stored.GetSql("selectgroupid");
			DbCommand cmd = Connector.CreateCommand();
			cmd.CommandText = ssql;
			DbParameter par1 = Stored.GetSqlParameters("selectgroupid")[0];
			par1.Value = name;
			cmd.Parameters.Add(Connector.CreateParameter(par1));
			try
			{
				cmd.Connection.Open();
				if (cmd.ExecuteScalar() == null)
				{
					cmd.CommandText = Stored.GetSql("insertgroup");
					cmd.Parameters.Clear();
					DbParameter par2 = Stored.GetSqlParameters("insertgroup")[0];
					par2.Value = name;
					cmd.Parameters.Add(Connector.CreateParameter(par2));
					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.CommandText = ssql;
						cmd.Parameters.Clear();
						cmd.Parameters.Add(Connector.CreateParameter(par1));
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}

		/// <summary>
		/// ��������� ����� ������
		/// </summary>
		/// <param name="groupId">��� ������</param>
		/// <param name="name">����� ��� ������</param>
		/// <returns>����� ����������� �������</returns>
		public static int UpdateGroup(int groupId, string name)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("updategroup"), Stored.GetSqlParameters("updategroup"));
			cmd.GetParameter(Stored.GetParameterName("group_id", "updategroup")).Value = groupId;
			cmd.GetParameter(Stored.GetParameterName("group_name", "updategroup")).Value = name;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ������
		/// </summary>
		/// <param name="groupId">��� ������</param>
		/// <returns>����� ��������� �������</returns>
		public static int DeleteGroup(int groupId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("deletegroup"), Stored.GetSqlParameters("deletegroup"));
			cmd.GetParameter(0).Value = groupId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ������ ����� ������������
		/// </summary>
		/// <returns>������ DataTable, ���������� ������ �����</returns>
		public static DataTable GetGroups()
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "group_table").Rows[0];
			string sId = tableStructureRow["groupid"].ToString();
			string sName = tableStructureRow["groupname"].ToString();
			DataTable tb = new DataTable("groups");
			tb.Columns.Add("groupid", typeof (int));
			tb.Columns.Add("groupname", typeof (string));
			tb.Rows.Add(new object[] {-3, GetInternalGroupName(-3)});
			tb.Rows.Add(new object[] {-2, GetInternalGroupName(-2)});
			tb.Rows.Add(new object[] {0, GetInternalGroupName(0)});
			tb.Rows.Add(new object[] {-1, GetInternalGroupName(-1)});
			if (Config.Main.AuthorizationEnabled)
			{
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectgroups"));
					while (r.Read())
					{
						tb.Rows.Add(new object[] {r[sId], r[sName]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;
		}

		#endregion

		#region action

		/// <summary>
		/// ������� ��������
		/// </summary>
		/// <param name="var">��� ���������� ��������</param>
		/// <param name="name">�������� ��������</param>
		/// <returns>��� ���������� ��������</returns>
		public static int CreateAction(string var, string name)
		{
			string ssql = Stored.GetSql("selectactionid");
			DbCommand cmd = Connector.CreateCommand();
			cmd.CommandText = ssql;
			DbParameter par1 = Stored.GetSqlParameters("selectactionid")[0];
			par1.Value = var;
			cmd.Parameters.Add(Connector.CreateParameter(par1));
			try
			{
				cmd.Connection.Open();
				if (cmd.ExecuteScalar() == null)
				{
					cmd.CommandText = Stored.GetSql("insertaction");
					cmd.Parameters.Clear();
					DbParameter[] par2 = Stored.GetSqlParameters("insertaction");
					cmd.Parameters.Add(Connector.CreateParameter(par2[0]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[1]));
					cmd.GetParameter(Stored.GetParameterName("action_var", "insertaction")).Value = var;
					cmd.GetParameter(Stored.GetParameterName("action_name", "insertaction")).Value = name;

					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.CommandText = ssql;
						cmd.Parameters.Clear();
						cmd.Parameters.Add(Connector.CreateParameter(par1));
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}

		/// <summary>
		/// ��������� ��������
		/// </summary>
		/// <param name="actionId">��� ��������</param>
		/// <param name="var">����� ���������� ��������</param>
		/// <param name="name">����� ��� ��������</param>
		/// <returns>����� ����������� �������</returns>
		public static int UpdateAction(int actionId, string var, string name)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("updateaction"), Stored.GetSqlParameters("updateaction"));
			cmd.GetParameter(Stored.GetParameterName("action_id", "updateaction")).Value = actionId;
			cmd.GetParameter(Stored.GetParameterName("action_var", "updateaction")).Value = var;
			cmd.GetParameter(Stored.GetParameterName("action_name", "updateaction")).Value = name;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ��������
		/// </summary>
		/// <param name="actionId">��� ��������</param>
		/// <returns>����� ��������� �������</returns>
		public static int DeleteAction(int actionId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("deleteaction"), Stored.GetSqlParameters("deleteaction"));
			cmd.GetParameter(0).Value = actionId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ������ ��������
		/// </summary>
		/// <returns>������ DataTable, ���������� ������ ��������</returns>
		public static DataTable GetActions()
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "action_table").Rows[0];
			string sId = tableStructureRow["actionid"].ToString();
			string sVar = tableStructureRow["actionvar"].ToString();
			string sName = tableStructureRow["actionname"].ToString();
			DataTable tb = new DataTable("actions");
			tb.Columns.Add("actionid", typeof (int));
			tb.Columns.Add("actionvar", typeof (string));
			tb.Columns.Add("actionname", typeof (string));
			tb.Rows.Add(new object[] {-1, GetInternalActionVar(-1), GetInternalActionName(-1)});
			if (Config.Main.AuthorizationEnabled)
			{
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectactions"));
					while (r.Read())
					{
						tb.Rows.Add(new object[] {r[sId], r[sVar], r[sName]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;
		}

		#endregion

		#region user

		/// <summary>
		/// ������� ������������ ��� 
		/// ���������� ��� ������������, 
		/// ���� ������������ � ����� ������� ��� ����
		/// </summary>
		/// <param name="login">���������� ����� ������������</param>
		/// <param name="password">������ ��� �����</param>
		/// <param name="email">����� ����������� �����, ��� �������� ������ � ����������</param>
		/// <param name="enabled">�������</param>
		/// <returns>��� ���������� ������������</returns>
		public static int CreateOrGetUser(string login, string password, string email, bool enabled)
		{
			for (int i = -3; i < 0; i++)
				if (GetInternalUserLogin(i) == login)
					return -1;
			string ssql = Stored.GetSql("selectuserid");
			DbCommand cmd = Connector.CreateCommand();
			cmd.CommandText = ssql;
			DbParameter par1 = Stored.GetSqlParameters("selectuserid")[0];
			par1.Value = login;
			cmd.Parameters.Add(Connector.CreateParameter(par1));
			try
			{
				cmd.Connection.Open();
				object ui = cmd.ExecuteScalar();
				if (ui == null)
				{
					cmd.CommandText = Stored.GetSql("insertuser");
					cmd.Parameters.Clear();
					DbParameter[] par2 = Stored.GetSqlParameters("insertuser");
					cmd.Parameters.Add(Connector.CreateParameter(par2[0]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[1]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[2]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[3]));
					cmd.GetParameter(Stored.GetParameterName("login", "insertuser")).Value = login;
					cmd.GetParameter(Stored.GetParameterName("password", "insertuser")).Value = EncryptPassword(password);
					cmd.GetParameter(Stored.GetParameterName("email", "insertuser")).Value = email;
					cmd.GetParameter(Stored.GetParameterName("enabled", "insertuser")).Value = enabled ? 1 : 0;

					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.CommandText = ssql;
						cmd.Parameters.Clear();
						cmd.Parameters.Add(Connector.CreateParameter(par1));
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
				else
				{
					return Convert.ToInt32(ui);
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}

		/// <summary>
		/// ������� ������������
		/// </summary>
		/// <param name="login">���������� ����� ������������</param>
		/// <param name="password">������ ��� �����</param>
		/// <param name="email">����� ����������� �����, ��� �������� ������ � ����������</param>
		/// <param name="enabled">�������</param>
		/// <returns>��� ���������� ������������</returns>
		public static int CreateUser(string login, string password, string email, bool enabled)
		{
			for (int i = -3; i < 0; i++)
				if (GetInternalUserLogin(i) == login)
					return -1;
			string ssql = Stored.GetSql("selectuserid");
			DbCommand cmd = Connector.CreateCommand();
			cmd.CommandText = ssql;
			DbParameter par1 = Stored.GetSqlParameters("selectuserid")[0];
			par1.Value = login;
			cmd.Parameters.Add(Connector.CreateParameter(par1));
			try
			{
				cmd.Connection.Open();
				if (cmd.ExecuteScalar() == null)
				{
					cmd.CommandText = Stored.GetSql("insertuser");
					cmd.Parameters.Clear();
					DbParameter[] par2 = Stored.GetSqlParameters("insertuser");
					cmd.Parameters.Add(Connector.CreateParameter(par2[0]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[1]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[2]));
					cmd.Parameters.Add(Connector.CreateParameter(par2[3]));
					cmd.GetParameter(Stored.GetParameterName("login", "insertuser")).Value = login;
					cmd.GetParameter(Stored.GetParameterName("password", "insertuser")).Value = EncryptPassword(password);
					cmd.GetParameter(Stored.GetParameterName("email", "insertuser")).Value = email;
					cmd.GetParameter(Stored.GetParameterName("enabled", "insertuser")).Value = enabled ? 1 : 0;

					if (cmd.ExecuteNonQuery() > 0)
					{
						cmd.CommandText = ssql;
						cmd.Parameters.Clear();
						cmd.Parameters.Add(Connector.CreateParameter(par1));
						return Convert.ToInt32(cmd.ExecuteScalar());
					}
				}
			}
			finally
			{
				cmd.Connection.Close();
			}
			return -1;
		}

		/// <summary>
		/// ��������� ���������� ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="login">�����</param>
		/// <param name="email">����������� �����</param>
		/// <param name="enabled">�������</param>
		/// <returns>���������� ���������� �������</returns>
		public static int UpdateUser(int userId, string login, string email, bool enabled)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("updateuser"), Stored.GetSqlParameters("updateuser"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "updateuser")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("login", "updateuser")).Value = login;
			cmd.GetParameter(Stored.GetParameterName("email", "updateuser")).Value = email;
			cmd.GetParameter(Stored.GetParameterName("enabled", "updateuser")).Value = enabled ? 1 : 0;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ��������� ������ ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="password">����� ������</param>
		/// <returns></returns>
		public static int ChangeUserPassword(int userId, string password)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("changeuserpassword"), Stored.GetSqlParameters("changeuserpassword"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "changeuserpassword")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("password", "changeuserpassword")).Value = EncryptPassword(password);
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>���������� ��������� �������</returns>
		public static int DeleteUser(int userId)
		{
			RemoveUserFromAllGroups(userId);
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("deleteuser"), Stored.GetSqlParameters("deleteuser"));
			cmd.GetParameter(0).Value = userId;
			return Connector.ExecuteNonQuery(cmd);
		}

		public static DataRow GetUser(int userId)
		{
			DataRow row = null;
			DataTable tb = new DataTable("users");
			tb.Columns.Add("userid", typeof (int));
			tb.Columns.Add("login", typeof (string));
			tb.Columns.Add("password", typeof (string));
			tb.Columns.Add("email", typeof (string));
			tb.Columns.Add("enabled", typeof (bool));
			tb.Columns.Add("code", typeof (string));
			if (userId == -3)
			{
				row = tb.Rows.Add(new object[] {-3, GetInternalUserLogin(-3), "", "", true, ""});
			}
			else if (userId == -2)
			{
				row = tb.Rows.Add(new object[] {-2, GetInternalUserLogin(-2), "", "", true, ""});
			}
			else if (userId == -1)
			{
				row = tb.Rows.Add(new object[] {-1, GetInternalUserLogin(-1), "", "", true, ""});
			}
			else if (Config.Main.AuthorizationEnabled)
			{
				DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
				string sId = tableStructureRow["userid"].ToString();
				string sLogin = tableStructureRow["login"].ToString();
				string sPassword = tableStructureRow["password"].ToString();
				string sEmail = tableStructureRow["email"].ToString();
				string sEnabled = tableStructureRow["enabled"].ToString();
				string sCode = tableStructureRow["code"].ToString();
				DbParameter par = Stored.GetSqlParameters("selectuser")[0];
				par.Value = userId;
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectuser"), par);
					if (r.Read())
					{
						row = tb.Rows.Add(new object[] {r[sId], r[sLogin], r[sPassword], r[sEmail], Convert.ToInt32(r[sEnabled]) == 1, r[sCode]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return row;
		}

		/// <summary>
		/// �������� ������������ �� ������, ���� �� �������
		/// </summary>
		/// <param name="login">����� ������������</param>
		/// <returns>������ ������������</returns>
		public static DataRow GetUser(string login)
		{
			DataRow row = null;
			DataTable tb = new DataTable("users");
			tb.Columns.Add("userid", typeof (int));
			tb.Columns.Add("login", typeof (string));
			tb.Columns.Add("password", typeof (string));
			tb.Columns.Add("email", typeof (string));
			tb.Columns.Add("enabled", typeof (bool));
			tb.Columns.Add("code", typeof (string));
			if (login == GetInternalUserLogin(-3))
			{
				row = tb.Rows.Add(new object[] {-3, GetInternalUserLogin(-3), "", "", true, ""});
			}
			else if (login == GetInternalUserLogin(-2))

			{
				row = tb.Rows.Add(new object[] {-2, GetInternalUserLogin(-2), "", "", true, ""});
			}
			else if (login == GetInternalUserLogin(-1))

			{
				row = tb.Rows.Add(new object[] {-1, GetInternalUserLogin(-1), "", "", true, ""});
			}
			else if (Config.Main.AuthorizationEnabled)
			{
				DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
				string sId = tableStructureRow["userid"].ToString();
				string sLogin = tableStructureRow["login"].ToString();
				string sPassword = tableStructureRow["password"].ToString();
				string sEmail = tableStructureRow["email"].ToString();
				string sEnabled = tableStructureRow["enabled"].ToString();
				string sCode = tableStructureRow["code"].ToString();
				DbParameter par = Stored.GetSqlParameters("loginuser")[0];
				par.Value = login;
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("loginuser"), par);
					if (r.Read())
					{
						row = tb.Rows.Add(new object[] {r[sId], r[sLogin], r[sPassword], r[sEmail], Convert.ToInt32(r[sEnabled]) == 1, r[sCode]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return row;
		}

		/// <summary>
		/// ���������� ���� � �������
		/// </summary>
		/// <param name="login">����� ������������</param>
		/// <param name="password">������ ������������</param>
		/// <returns>DataRow ���������� ���������� � ������������ ��� null</returns>
		public static DataRow LoginUser(string login, string password)
		{
			string pass = EncryptPassword(password);
			DataRow row = null;
			DataTable tb = new DataTable("user");
			tb.Columns.Add("userid", typeof (int));
			tb.Columns.Add("login", typeof (string));
			tb.Columns.Add("email", typeof(string));
			tb.Columns.Add("enabled", typeof(bool));
			if (login == GetInternalUserLogin(-3))
			{
				if (pass == Config.Main.AdminPassword)
					return tb.Rows.Add(new object[] {-3, GetInternalUserLogin(-3), "", true});
				else
					return null;
			}
			if (login == GetInternalUserLogin(-2))
			{
				if (pass == Config.Main.ManagerPassword)
					return tb.Rows.Add(new object[] { -2, GetInternalUserLogin(-2), "", true });
				else
					return null;
			}
			if (Config.Main.AuthorizationEnabled)
			{
				DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
				string sId = tableStructureRow["userid"].ToString();
				string sLogin = tableStructureRow["login"].ToString();
				string sPassword = tableStructureRow["password"].ToString();
				string sEmail = tableStructureRow["email"].ToString();
				DbParameter par = Stored.GetSqlParameters("loginuser")[0];
				par.Value = login;
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("loginuser"), par);
					if (r.Read())
					{
						if (r[sPassword].ToString() == pass)
							row = tb.Rows.Add(new object[] { r[sId], r[sLogin], r[sEmail], true });
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return row;
		}

		/// <summary>
		/// �������� ������ �������������
		/// </summary>
		/// <returns>������ DataTable, ���������� ������ �������������</returns>
		public static DataTable GetUsers()
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
			string sId = tableStructureRow["userid"].ToString();
			string sLogin = tableStructureRow["login"].ToString();
			string sPassword = tableStructureRow["password"].ToString();
			string sEmail = tableStructureRow["email"].ToString();
			string sEnabled = tableStructureRow["enabled"].ToString();
			string sCode = tableStructureRow["code"].ToString();
			DataTable tb = new DataTable("users");
			tb.Columns.Add("userid", typeof (int));
			tb.Columns.Add("login", typeof (string));
			tb.Columns.Add("password", typeof (string));
			tb.Columns.Add("email", typeof (string));
			tb.Columns.Add("enabled", typeof (bool));
			tb.Columns.Add("code", typeof (string));
			tb.Rows.Add(new object[] {-3, GetInternalUserLogin(-3), Config.Main.AdminPassword, "", true, ""});
			tb.Rows.Add(new object[] {-2, GetInternalUserLogin(-2), Config.Main.ManagerPassword, "", true, ""});
			tb.Rows.Add(new object[] {-1, GetInternalUserLogin(-1), "", "", true, ""});

			if (Config.Main.AuthorizationEnabled)
			{
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectusers"));
					while (r.Read())
					{
						tb.Rows.Add(new object[] {r[sId], r[sLogin], r[sPassword], r[sEmail], Convert.ToInt32(r[sEnabled]) == 1, r[sCode]});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;
		}

		/// <summary>
		/// ������� ��� ���� ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>���������� ��������� �������</returns>
		public static int RemoveAllUserFields(int userId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("removealluserfield"), Stored.GetSqlParameters("removealluserfield"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "removealluserfield")).Value = userId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� ��� ���� ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>DataTable �� ������� ����� � ��������</returns>
		public static DataTable GetAllUserFieldsTable(int userId)
		{
			DataTable tb = new DataTable();
			tb.Columns.Add("field", typeof (string));
			tb.Columns.Add("value", typeof (string));
			if (Config.Main.AuthorizationEnabled)
			{
				DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_field_table").Rows[0];
				string sField = tableStructureRow["field"].ToString();
				string sValue = tableStructureRow["value"].ToString();
				DbParameter par = Stored.GetSqlParameters("getalluserfield")[0];
				par.Value = userId;
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("getalluserfield"), par);
					while (r.Read())
					{
						tb.Rows.Add(new object[] { r[sField], r[sValue] });
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;
		}

		/// <summary>
		/// �������� ��� ���� ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>Hashtable �� ������� ����� � ��������</returns>
		public static Hashtable GetAllUserFields(int userId)
		{
			Hashtable h = new Hashtable();
			if (Config.Main.AuthorizationEnabled)
			{
				DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_field_table").Rows[0];
				string sField = tableStructureRow["field"].ToString();
				string sValue = tableStructureRow["value"].ToString();
				DbParameter par = Stored.GetSqlParameters("getalluserfield")[0];
				par.Value = userId;
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("getalluserfield"), par);
					while (r.Read())
					{
						h.Add(r[sField], r[sValue]);
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return h;
		}

		/// <summary>
		/// ������� ����
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="field">��� ����</param>
		/// <returns>���������� ��������� �������</returns>
		public static int RemoveUserField(int userId, string field)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("removeuserfield"), Stored.GetSqlParameters("removeuserfield"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "removeuserfield")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("field", "removeuserfield")).Value = field;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ������������� �������� ����
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="field">��� ����</param>
		/// <param name="value">�������� ����</param>
		/// <returns>���������� ����������� �������</returns>
		public static int SetUserField(int userId, string field, string value)
		{
			RemoveUserField(userId, field);
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("setuserfield"), Stored.GetSqlParameters("setuserfield"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "setuserfield")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("field", "setuserfield")).Value = field;
			cmd.GetParameter(Stored.GetParameterName("value", "setuserfield")).Value = value;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� �������� ����
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="field">��� ����</param>
		/// <returns>�������� ����</returns>
		public static string GetUserField(int userId, string field)
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_field_table").Rows[0];
			string sValue = tableStructureRow["value"].ToString();
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("getuserfield"), Stored.GetSqlParameters("getuserfield"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "getuserfield")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("field", "getuserfield")).Value = field;
			IDataReader r = null;
			try
			{
				cmd.Connection.Open();
				r = cmd.ExecuteReader();
				if (r.Read())
				{
					return r[sValue].ToString();
				}
			}
			finally
			{
				if (r != null)
					r.Close();
				cmd.Connection.Close();
			}
			return null;
		}

		/// <summary>
		/// ���������� ������
		/// </summary>
		/// <param name="password">�������� ������</param>
		/// <returns>������������� ������</returns>
		public static string EncryptPassword(string password)
		{
			int hashmode = Config.Main.HashMode;
			switch (hashmode)
			{
				case 2:
					byte[] arr = new System.Security.Cryptography.MD5CryptoServiceProvider()
						.ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
					StringBuilder sb = new StringBuilder();
					for (int i = 0; i < arr.Length; i++)
					{
						sb.AppendFormat("{0:x2}", arr[i]);
					}
					return sb.ToString();
				
				default:
					return Convert.ToBase64String(
						new System.Security.Cryptography.SHA1CryptoServiceProvider()
						.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
			}
		}

		/// <summary>
		/// �������� ������ ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="subject">���� ������</param>
		/// <param name="body">����� ������</param>
		public static void SendEmailToUser(int userId, string subject, string body)
		{
			DataRow ur = GetUser(userId);
			SendEmail(ur["email"].ToString(), subject, body);
		}

		/// <summary>
		/// �������� ������ ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="from">��</param>
		/// <param name="subject">���� ������</param>
		/// <param name="body">����� ������</param>
		public static void SendEmailToUser(int userId, string from, string subject, string body)
		{
			DataRow ur = GetUser(userId);
			SendEmail(from, ur["email"].ToString(), subject, body);
		}

		/// <summary>
		/// �������� ������ � ����� �������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		public static void SendRePassEmail(int userId)
		{
			DataRow row = Config.GetConfigTable("security.config", "password_recovery").Rows[0];
			DataRow ur = GetUser(userId);
			ur["password"] = Guid.NewGuid().ToString().Substring(0, 8);
			ChangeUserPassword(userId, ur["password"].ToString());
			string body = row["mail_body"].ToString().Replace("$login$", ur["login"].ToString()).Replace("$password$", ur["password"].ToString()).Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(ur["email"].ToString(), row["mail_subject"].ToString(), body);
		}

		/// <summary>
		/// �������� ������ � ����� �������  � ���������������� ����������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="from">��</param>
		/// <param name="mail_subject">���� ������</param>
		/// <param name="mail_body">������ ������</param>
		public static void SendRePassEmail(int userId, string from, string mail_subject, string mail_body)
		{
			DataRow ur = GetUser(userId);
			ur["password"] = Guid.NewGuid().ToString().Substring(0, 8);
			ChangeUserPassword(userId, ur["password"].ToString());
			string body = mail_body
				.Replace("$login$", ur["login"].ToString())
				.Replace("$password$", ur["password"].ToString())
				.Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(from, ur["email"].ToString(), mail_subject, body);
		}

		/// <summary>
		/// �������� ������ c ����� ��� ����� ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		public static void SendRePasswordConfirmEmail(int userId)
		{
			string code = Guid.NewGuid().ToString();
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("beginchangepassowrd"), Stored.GetSqlParameters("beginchangepassowrd"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "beginchangepassowrd")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("code", "beginchangepassowrd")).Value = code;
			Connector.ExecuteNonQuery(cmd);

			DataRow row = Config.GetConfigTable("security.config", "password_confirmation").Rows[0];
			DataRow ur = GetUser(userId);

			string body = row["mail_body"].ToString()
				.Replace("$code$", code)
				.Replace("$site$", "http://" + Path.Domain + "/")
				.Replace("$login$", ur["login"].ToString());
			SendEmail(ur["email"].ToString(), row["mail_subject"].ToString(), body);
		}

		/// <summary>
		/// �������� ������ c ����� ��� ����� ������ � ���������������� ����������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="from">��</param>
		/// <param name="mail_subject">���� ������</param>
		/// <param name="mail_body">������ ������</param>
		public static void SendRePasswordConfirmEmail(int userId, string from, string mail_subject, string mail_body)
		{
			string code = Guid.NewGuid().ToString();
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("beginchangepassowrd"), Stored.GetSqlParameters("beginchangepassowrd"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "beginchangepassowrd")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("code", "beginchangepassowrd")).Value = code;
			Connector.ExecuteNonQuery(cmd);

			DataRow ur = GetUser(userId);

			string body = mail_body
				.Replace("$code$", code)
				.Replace("$site$", "http://" + Path.Domain + "/")
				.Replace("$login$", ur["login"].ToString());
			SendEmail(from, ur["email"].ToString(), mail_subject, body);
		}


		/// <summary>
		/// ����� ������ � ��������������
		/// </summary>
		/// <param name="code">��� ���������</param>
		/// <param name="login">����� ������������</param>
		public static void ConfirmChangePassword(string code, string login)
		{

			DataRow ur = GetUser(login);
			ur["password"] = Guid.NewGuid().ToString().Substring(0, 8);

			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("endchangepassowrd"), Stored.GetSqlParameters("endchangepassowrd"));
			cmd.GetParameter(Stored.GetParameterName("password", "endchangepassowrd")).Value = EncryptPassword(ur["password"].ToString());
			cmd.GetParameter(Stored.GetParameterName("code", "endchangepassowrd")).Value = code;
			cmd.GetParameter(Stored.GetParameterName("login", "endchangepassowrd")).Value = login;
			Connector.ExecuteNonQuery(cmd);

			DataRow row = Config.GetConfigTable("security.config", "password_recovery").Rows[0];
			string body = row["mail_body"].ToString().Replace("$login$", ur["login"].ToString()).Replace("$password$", ur["password"].ToString()).Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(ur["email"].ToString(), row["mail_subject"].ToString(), body);
		}

		/// <summary>
		/// ����� ������ � �������������� � ���������������� ����������
		/// </summary>
		/// <param name="code">��� ���������</param>
		/// <param name="login">����� ������������</param>
		/// <param name="from">��</param>
		/// <param name="mail_subject">���� ������</param>
		/// <param name="mail_body">������ ������</param>
		public static void ConfirmChangePassword(string code, string login, string from, string mail_subject, string mail_body)
		{

			DataRow ur = GetUser(login);
			ur["password"] = Guid.NewGuid().ToString().Substring(0, 8);

			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("endchangepassowrd"), Stored.GetSqlParameters("endchangepassowrd"));
			cmd.GetParameter(Stored.GetParameterName("password", "endchangepassowrd")).Value = EncryptPassword(ur["password"].ToString());
			cmd.GetParameter(Stored.GetParameterName("code", "endchangepassowrd")).Value = code;
			cmd.GetParameter(Stored.GetParameterName("login", "endchangepassowrd")).Value = login;
			Connector.ExecuteNonQuery(cmd);

			string body = mail_body
				.Replace("$login$", ur["login"].ToString())
				.Replace("$password$", ur["password"].ToString())
				.Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(from, ur["email"].ToString(), mail_subject, body);
		}



		/// <summary>
		/// �������� ������ � ����� ���������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		public static void SendActivationEmail(int userId)
		{
			DataRow row = Config.GetConfigTable("security.config", "activation").Rows[0];
			DataRow ur = GetUser(userId);
			string code = Guid.NewGuid().ToString();
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("beginactivation"), Stored.GetSqlParameters("beginactivation"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "beginactivation")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("code", "beginactivation")).Value = code;
			Connector.ExecuteNonQuery(cmd);
			string body = row["mail_body"].ToString().Replace("$code$", code).Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(ur["email"].ToString(), row["mail_subject"].ToString(), body);
		}

		/// <summary>
		/// �������� ������ � ����� ��������� � ���������������� ����������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="from">��</param>
		/// <param name="mail_subject">���� ������</param>
		/// <param name="mail_body">������ ������</param>
		public static void SendActivationEmail(int userId, string from, string mail_subject, string mail_body)
		{
			DataRow ur = GetUser(userId);
			string code = Guid.NewGuid().ToString();
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("beginactivation"), Stored.GetSqlParameters("beginactivation"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "beginactivation")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("code", "beginactivation")).Value = code;
			Connector.ExecuteNonQuery(cmd);
			string body = mail_body.Replace("$code$", code).Replace("$site$", "http://" + Path.Domain + "/");
			SendEmail(from, ur["email"].ToString(), mail_subject, body);
		}

		/// <summary>
		/// ������������ ������������
		/// </summary>
		/// <param name="code">��� ���������</param>
		public static void ActivateUser(string code)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("endactivation"), Stored.GetSqlParameters("endactivation"));
			cmd.GetParameter(Stored.GetParameterName("code", "endactivation")).Value = code;
			Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="to">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		public static void SendEmail(string to, string subject, string body)
		{
			DataRow row = Config.GetConfigTable("security.config", "mail").Rows[0];
			Util.SendEmail(
				row["from"].ToString(),
				to,
				subject,
				body,
				row["smtp"].ToString(),
				row["login"].ToString(),
				row["password"].ToString(),
				row["ssl"].ToString() == "1");
		}

		/// <summary>
		/// �������� e-mail
		/// </summary>
		/// <param name="from">��</param>
		/// <param name="to">����</param>
		/// <param name="subject">����</param>
		/// <param name="body">����� ������</param>
		public static void SendEmail(string from, string to, string subject, string body)
		{
			DataRow row = Config.GetConfigTable("security.config", "mail").Rows[0];
			Util.SendEmail(
				from,
				to,
				subject,
				body,
				row["smtp"].ToString(),
				row["login"].ToString(),
				row["password"].ToString(),
				row["ssl"].ToString() == "1");
		}

		#endregion

		#region user & group

		/// <summary>
		/// ��������� ������������ � ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="groupId">��� ������</param>
		/// <returns>���������� ����������� �������</returns>
		public static int AddUserToGroup(int userId, int groupId)
		{
			//������� ������ �� ������ ������
			RemoveUserFromGroup(userId, groupId);
			//������ �������
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("usertogroup"), Stored.GetSqlParameters("usertogroup"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "usertogroup")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("group_id", "usertogroup")).Value = groupId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ������� ������������ �� ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="groupId">��� ������</param>
		/// <returns>���������� ��������� �������</returns>
		public static int RemoveUserFromGroup(int userId, int groupId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("userfromgroup"), Stored.GetSqlParameters("userfromgroup"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "userfromgroup")).Value = userId;
			cmd.GetParameter(Stored.GetParameterName("group_id", "userfromgroup")).Value = groupId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ������� ������������ �� ���� �����
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>���������� ��������� �������</returns>
		public static int RemoveUserFromAllGroups(int userId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("userfromallgroup"), Stored.GetSqlParameters("userfromallgroup"));
			cmd.GetParameter(Stored.GetParameterName("user_id", "userfromallgroup")).Value = userId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ���������� ������, ���������� ������������� ������������� � �����
		/// </summary>
		/// <returns>DataTable � ���������������</returns>
		public static DataTable GetUserInGroupList()
		{
			//TODO - �� ������������
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
			string sLogin = tableStructureRow["login"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "group_table").Rows[0];
			string sGroupName = tableStructureRow["groupname"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "useringroup_table").Rows[0];
			string sUserId = tableStructureRow["userid"].ToString();
			string sGroupId = tableStructureRow["groupid"].ToString();
			DataTable tb = new DataTable("useringroup");
			tb.Columns.Add("userid", typeof (int));
			tb.Columns.Add("login", typeof (string));
			tb.Columns.Add("groupid", typeof (int));
			tb.Columns.Add("groupname", typeof (string));
			tb.Rows.Add(new object[] {-3, GetInternalUserLogin(-3), -3, GetInternalGroupName(-3)});
			tb.Rows.Add(new object[] {-2, GetInternalUserLogin(-2), -2, GetInternalGroupName(-2)});
			tb.Rows.Add(new object[] {-1, GetInternalUserLogin(-1), -1, GetInternalGroupName(-1)});
			if (Config.Main.AuthorizationEnabled)
			{
				IDataReader r = null;
				try
				{
					r = Connector.ExecuteReader(Stored.GetSql("selectuseringroup"));
					while (r.Read())
					{
						string groupname = r[sGroupName] == DBNull.Value ? GetInternalGroupName(Convert.ToInt32(r[sGroupId])) : r[sGroupName].ToString();
						string login = r[sLogin] == DBNull.Value ? GetInternalUserLogin(Convert.ToInt32(r[sUserId])) : r[sLogin].ToString();
						tb.Rows.Add(new object[] {r[sUserId], login, r[sGroupId], groupname});
					}
				}
				finally
				{
					if (r != null)
						r.Close();
				}
			}
			return tb;

		}

		/// <summary>
		/// ���������� ������, ���������� ������ ������������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <returns>DataTable</returns>
		public static DataTable GetUserGroupList(int userId)
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "user_table").Rows[0];
			tableStructureRow = Config.GetConfigTable("security.config", "group_table").Rows[0];
			string sGroupName = tableStructureRow["groupname"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "useringroup_table").Rows[0];
			string sGroupId = tableStructureRow["groupid"].ToString();
			DataTable tb = new DataTable("useringroup");
			tb.Columns.Add("groupid", typeof (int));
			tb.Columns.Add("groupname", typeof (string));
			if(userId < 0)
			{
				tb.Rows.Add(new object[] {userId, GetInternalGroupName(userId)});
			}
			if (Config.Main.AuthorizationEnabled)
			{
				try
				{
					IDataReader r = null;
					try
					{
						DbParameter par = Stored.GetSqlParameters("selectusergroup")[0];
						par.Value = userId;
						r = Connector.ExecuteReader(Stored.GetSql("selectusergroup"),par);
						while (r.Read())
						{
							int groupid = Convert.ToInt32(r[sGroupId]);
							string groupname = groupid < 1 ? GetInternalGroupName(groupid) : r[sGroupName].ToString();
							tb.Rows.Add(new object[] {groupid, groupname});
						}
					}
					finally
					{
						if (r != null)
							r.Close();
					}
				}
				catch(Exception ex){Config.ReportError(ex);}
			}
			return tb;
		}

		/// <summary>
		/// ��������� ������ �� ������������ � ������ ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="groupId">��� ������</param>
		/// <returns>��, ���� ������, ��� - ���� �� ������</returns>
		public static bool IsUserInGroup(int userId, int groupId)
		{
			if (userId < 0 && userId == groupId)
			{
				return true;
			}
			return GetUserGroupList(userId).Select("groupid=" + groupId).Length > 0;
		}

		#endregion

		#region access rule

		/// <summary>
		/// �������� ������� �������
		/// </summary>
		/// <param name="groupId">��� ������</param>
		/// <param name="partId">��� �������</param>
		/// <param name="actionId">��� ��������</param>
		/// <returns>���������� ����������� �������</returns>
		public static int AddAccessRule(int groupId, int partId, int actionId)
		{
			//������� ������ �� ������ ������
			RemoveAccessRule(groupId, partId, actionId);
			//������ �������
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("insertaccessrule"), Stored.GetSqlParameters("insertaccessrule"));
			cmd.GetParameter(Stored.GetParameterName("group_id", "insertaccessrule")).Value = groupId;
			cmd.GetParameter(Stored.GetParameterName("part_id", "insertaccessrule")).Value = partId;
			cmd.GetParameter(Stored.GetParameterName("action_id", "insertaccessrule")).Value = actionId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ������� ������� �������
		/// </summary>
		/// <param name="groupId">��� ������</param>
		/// <param name="partId">��� �������</param>
		/// <param name="actionId">��� ��������</param>
		/// <returns>���������� ��������� �������</returns>
		public static int RemoveAccessRule(int groupId, int partId, int actionId)
		{
			DbCommand cmd = Connector.CreateCommand(Stored.GetSql("deleteaccessrule"), Stored.GetSqlParameters("deleteaccessrule"));
			cmd.GetParameter(Stored.GetParameterName("group_id", "deleteaccessrule")).Value = groupId;
			cmd.GetParameter(Stored.GetParameterName("part_id", "deleteaccessrule")).Value = partId;
			cmd.GetParameter(Stored.GetParameterName("action_id", "deleteaccessrule")).Value = actionId;
			return Connector.ExecuteNonQuery(cmd);
		}

		/// <summary>
		/// ���������� ������ ������������� ���������� �������
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetAccessRuleList()
		{
			DataRow tableStructureRow = Config.GetConfigTable("security.config", "part_table").Rows[0];
			string sPartName = tableStructureRow["partname"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "group_table").Rows[0];
			string sGroupName = tableStructureRow["groupname"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "action_table").Rows[0];
			string sActionName = tableStructureRow["actionname"].ToString();
			string sActionVar = tableStructureRow["actionvar"].ToString();
			tableStructureRow = Config.GetConfigTable("security.config", "accessrule_table").Rows[0];
			string sPartId = tableStructureRow["partid"].ToString();
			string sGroupId = tableStructureRow["groupid"].ToString();
			string sActionId = tableStructureRow["actionid"].ToString();
			DataTable tb = new DataTable("accessrule");
			tb.Columns.Add("partid", typeof (int));
			tb.Columns.Add("partname", typeof (string));
			tb.Columns.Add("groupid", typeof (int));
			tb.Columns.Add("groupname", typeof (string));
			tb.Columns.Add("actionid", typeof (int));
			tb.Columns.Add("actionname", typeof (string));
			tb.Columns.Add("actionvar", typeof (string));
			tb.Rows.Add(new object[] {-3, GetInternalPartName(-3), -3, GetInternalGroupName(-3), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-2, GetInternalPartName(-2), -3, GetInternalGroupName(-3), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {0, GetInternalPartName(0), -3, GetInternalGroupName(-3), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-1, GetInternalPartName(-1), -3, GetInternalGroupName(-3), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-2, GetInternalPartName(-2), -2, GetInternalGroupName(-2), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {0, GetInternalPartName(0), -2, GetInternalGroupName(-2), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-1, GetInternalPartName(-1), -2, GetInternalGroupName(-2), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {0, GetInternalPartName(0), 0, GetInternalGroupName(0), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-1, GetInternalPartName(-1), 0, GetInternalGroupName(0), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});
			tb.Rows.Add(new object[] {-1, GetInternalPartName(-1), -1, GetInternalGroupName(-1), -1, GetInternalActionName(-1), GetInternalActionVar(-1)});

			if (Config.Main.AuthorizationEnabled)
			{
				try
				{
					IDataReader r = null;
					try
					{
						r = Connector.ExecuteReader(Stored.GetSql("selectaccessrules"));
						while (r.Read())
						{
							string part = r[sPartName] == DBNull.Value ? GetInternalPartName(Convert.ToInt32(r[sPartId])) : r[sPartName].ToString();
							string action = r[sActionName] == DBNull.Value ? GetInternalActionName(Convert.ToInt32(r[sActionId])) : r[sActionName].ToString();
							string actionV = r[sActionVar] == DBNull.Value ? GetInternalActionVar(Convert.ToInt32(r[sActionId])) : r[sActionVar].ToString();
							string group = r[sGroupName] == DBNull.Value ? GetInternalGroupName(Convert.ToInt32(r[sGroupId])) : r[sGroupName].ToString();
							tb.Rows.Add(new object[] {r[sPartId], part, r[sGroupId], group, r[sActionId], action, actionV});
						}
					}
					finally
					{
						if (r != null)
							r.Close();
					}
				}
				catch(Exception ex)
				{
					Config.ReportError(ex);
				}
			}
			return tb;
		}

		#endregion

		#region check access

		/// <summary>
		/// ���������� ��� ����������
		/// </summary>
		/// <returns>DataTable</returns>
		public static DataTable GetAccessRuleCache()
		{
			//TODO - ������ ��������� ������ ���, ��� ������� � �������. �������� ���������� �� ���, � ����� ��������� �� ������ WhoIsOnline()
			DataTable tb = null;
			/*Cache cache = HttpContext.Current.Cache;
			if (cache[Keys.AccessRuleTableCache] != null)
			{
				tb = (DataTable) cache[Keys.AccessRuleTableCache];
			}
			else
			{*/
				tb = new DataTable("accessrules");
				tb.Columns.Add("userid", typeof (int));
				tb.Columns.Add("partid", typeof (int));
				tb.Columns.Add("actionvar", typeof (string));
				DataTable tbR = GetAccessRuleList();
				DataTable tbU = GetUserInGroupList();
				foreach (DataRow r in tbU.Rows)
				{
					DataRow[] rows = tbR.Select("groupid=" + r["groupid"].ToString());
					foreach (DataRow row in rows)
					{
						tb.Rows.Add(new object[] {r["userid"], row["partid"], row["actionvar"]});
					}
				}
				//cache.Insert(Keys.AccessRuleTableCache, tb);
			//}
			return tb;
		}

		public static void ClearAccessRuleCache()
		{
			//HttpContext.Current.Cache.Remove(Keys.AccessRuleTableCache);
		}

		/// <summary>
		/// ��������� ����� �� ������������ ���������� [������]
		/// �� ������ ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="partId">��� �������</param>
		/// <returns>��/���</returns>
		public static bool UserCanAccess(int userId, int partId)
		{
			return UserCan(userId, partId, GetInternalActionVar(-1));
		}

		/// <summary>
		/// ��������� ����� �� ������������ ������ ����������
		/// �� ������ ������
		/// </summary>
		/// <param name="userId">��� ������������</param>
		/// <param name="partId">��� �������</param>
		/// <param name="actionVar">���������� ����������</param>
		/// <returns>��/���</returns>
		public static bool UserCan(int userId, int partId, string actionVar)
		{
			if(partId == -1 && actionVar == GetInternalActionVar(-1))
			{
				return true;
			}
			if (userId == -3)
			{
				return true;
			}
			if(userId==-2 && (userId<=partId) && actionVar == GetInternalActionVar(-1))
			{
				return true;
			}
			DataTable tb = GetAccessRuleCache();
			return tb.Select("userid=" + userId.ToString() + " AND partid=" + partId.ToString() + " AND actionvar='" + actionVar + "'").Length > 0;
		}

		#endregion

		#region private

		public static string GetInternalUserLogin(int id)
		{
			switch (id)
			{
				case -3:
					return "Admin";
				case -2:
					return "Manager";
				case -1:
					return "Guest";
			}
			return "";
		}

		public static string GetInternalGroupName(int id)
		{
			switch (id)
			{
				case -3:
					return "��������������";
				case -2:
					return "�������-���������";
				case -1:
					return "�����";
				case 0:
					return "������������";
			}
			return "";
		}

		public static string GetInternalPartName(int id)
		{
			switch (id)
			{
				case -3:
					return "�����������������";
				case -2:
					return "���������� ���������";
				case -1:
					return "�����";
				case 0:
					return "��� �������������";
			}
			return "";
		}

		public static string GetInternalActionName(int id)
		{
			switch (id)
			{
				case -1:
					return "������";
			}
			return "";
		}

		public static string GetInternalActionVar(int id)
		{
			switch (id)
			{
				case -1:
					return "access";
			}
			return "";
		}

		#endregion
	}
}