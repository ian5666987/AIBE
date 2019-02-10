using Extension.Cryptography;
using Extension.Database.SqlServer;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;

namespace Aibe.Helpers {
  public class UserHelper {
    public static void CreateUserMap(string userName, string password) {
      if (string.IsNullOrWhiteSpace(userName))
        return;
      string encryptedPassword = string.IsNullOrEmpty(password) ? null : Cryptography.Encrypt(password);
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("INSERT INTO [", DH.UserMapTableName, "] VALUES(@par0, @par1)"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", userName),
          string.IsNullOrWhiteSpace(encryptedPassword) ? new SqlParameter("@par1", DBNull.Value) : new SqlParameter("@par1", encryptedPassword),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        LogHelper.Error(userName, LCZ.W_Aibe, LCZ.W_Aibe, LCZ.I_UserMap, DH.UserMapTableName, LCZ.W_Create, null, ex.ToString());
      }
    }

    public static void DeleteUserMap(string userName) {
      if (string.IsNullOrWhiteSpace(userName))
        return;
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("DELETE FROM [", DH.UserMapTableName, "] WHERE [UserName] = @par0"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", userName),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        LogHelper.Error(userName, LCZ.W_Aibe, LCZ.W_Aibe, LCZ.I_UserMap, DH.UserMapTableName, LCZ.W_Delete, null, ex.ToString());
      }
    }

    public static void EditUserMapName(string userName, string newUserName) {
      if (string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(newUserName))
        return;
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("UPDATE [", DH.UserMapTableName, "] SET [UserName] = @par0 WHERE [UserName] = @par1"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(newUserName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", newUserName),
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par1", DBNull.Value) : new SqlParameter("@par1", userName),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        LogHelper.Error(userName, LCZ.W_Aibe, LCZ.W_Aibe, LCZ.I_UserMap, DH.UserMapTableName, LCZ.I_EditName, null, ex.ToString());
      }
    }

    public static void SetUserMapPassword(string userName, string newPassword) {
      if (string.IsNullOrWhiteSpace(userName))
        return;
      string password = string.IsNullOrEmpty(newPassword) ? null : Cryptography.Encrypt(newPassword);
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("UPDATE [", DH.UserMapTableName, "] SET [EncryptedPassword] = @par0 WHERE [UserName] = @par1"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(password) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", password),
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par1", DBNull.Value) : new SqlParameter("@par1", userName),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        LogHelper.Error(userName, LCZ.W_Aibe, LCZ.W_Aibe, LCZ.I_UserMap, DH.UserMapTableName, LCZ.I_SetPassword, null, ex.ToString());
      }
    }

    
    public static bool AuthenticateUser(string logType, string userName, string password) {
      if (string.IsNullOrWhiteSpace(userName))
        return false;
      StringBuilder sb = new StringBuilder();
      sb.Append(string.Concat("SELECT [UserName], [EncryptedPassword] FROM ", DH.UserMapTableName, 
        " WHERE [UserName] = @par0"));
      SqlParameter par0 = new SqlParameter("@par0", userName);
      DataTable dataTable = null;
      try {
        dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, sb.ToString(), par0);
      } catch (Exception ex) {
        LogHelper.Error(userName, LCZ.W_Aibe, LCZ.W_Aibe, LCZ.I_UserMap, DH.UserMapTableName, 
          LCZ.I_AuthenticateUser + " - " + LCZ.I_GetUser, null, ex.ToString());
        return false;
      }
      if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count <= 0) {
        LogHelper.Access(userName, logType, LCZ.W_NotFound);
        return false; //not found
      }
      DataRow row = dataTable.Rows[0];
      string encryptedPassword = (string)row["EncryptedPassword"];
      string decryptedPassword = Cryptography.Decrypt(encryptedPassword);
      if (password != decryptedPassword) {
        LogHelper.Access(userName, logType, LCZ.NFE_PasswordDoesNotMatch);
        return false; //not found
      }
      return true;
    }
  }
}