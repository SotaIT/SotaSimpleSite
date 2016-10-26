using System;
using System.Web.Security;
using System.Data.SqlClient;

/// <summary>
/// Доступ к системе авторизации основного сайта
/// </summary>
public class SSSMembership : MembershipProvider
{
	const string connectionString = @"Data Source=WEB\MSSQL2005;Initial Catalog=dspl;Integrated Security=SSPI;";

	public override bool ValidateUser(string username, string password)
	{
        //получаем хэш пароля указанного пользователя из БД
        string password_hash = "";
        SqlConnection conn = new SqlConnection(connectionString);
        try 
        {
            SqlCommand cmd = new SqlCommand("SELECT [sPassword] FROM [User] WHERE [sLogin]=@login");
            cmd.Parameters.Add("@login", System.Data.SqlDbType.NVarChar).Value = username;
            password_hash = cmd.ExecuteScalar().ToString();
        }
        finally 
        { 
            conn.Close();
            conn.Dispose();
        }

        if (password_hash.ToString().Length > 0)
        {
            //вычисляем хэш указанного пароля
            /* string new_password_hash = Convert.ToBase64String(
                         new System.Security.Cryptography.SHA1CryptoServiceProvider()
                         .ComputeHash(System.Text.Encoding.UTF8.GetBytes(password)));
             */
            byte[] arr = new System.Security.Cryptography.MD5CryptoServiceProvider()
                        .ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            for (int i = 0; i < arr.Length; i++)
            {
                sb.AppendFormat("{0:x2}", arr[i]);
            }
             string new_password_hash = sb.ToString();
            
            
            //сравниваем
            return password_hash == new_password_hash;
        }
        
        return false;
	}

    public override string ApplicationName
    {
        get
        {
            throw new NotImplementedException();
        }
        set
        {
            throw new NotImplementedException();
        }
    }

    public override bool ChangePassword(string username, string oldPassword, string newPassword)
    {
        throw new NotImplementedException();
    }

    public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
    {
        throw new NotImplementedException();
    }

    public override bool DeleteUser(string username, bool deleteAllRelatedData)
    {
        throw new NotImplementedException();
    }

    public override bool EnablePasswordReset
    {
        get { throw new NotImplementedException(); }
    }

    public override bool EnablePasswordRetrieval
    {
        get { throw new NotImplementedException(); }
    }

    public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
    {
        throw new NotImplementedException();
    }

    public override int GetNumberOfUsersOnline()
    {
        throw new NotImplementedException();
    }

    public override string GetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser GetUser(string username, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
    {
        throw new NotImplementedException();
    }

    public override string GetUserNameByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public override int MaxInvalidPasswordAttempts
    {
        get { throw new NotImplementedException(); }
    }

    public override int MinRequiredNonAlphanumericCharacters
    {
        get { throw new NotImplementedException(); }
    }

    public override int MinRequiredPasswordLength
    {
        get { throw new NotImplementedException(); }
    }

    public override int PasswordAttemptWindow
    {
        get { throw new NotImplementedException(); }
    }

    public override MembershipPasswordFormat PasswordFormat
    {
        get { throw new NotImplementedException(); }
    }

    public override string PasswordStrengthRegularExpression
    {
        get { throw new NotImplementedException(); }
    }

    public override bool RequiresQuestionAndAnswer
    {
        get { throw new NotImplementedException(); }
    }

    public override bool RequiresUniqueEmail
    {
        get { throw new NotImplementedException(); }
    }

    public override string ResetPassword(string username, string answer)
    {
        throw new NotImplementedException();
    }

    public override bool UnlockUser(string userName)
    {
        throw new NotImplementedException();
    }

    public override void UpdateUser(MembershipUser user)
    {
        throw new NotImplementedException();
    }
}
