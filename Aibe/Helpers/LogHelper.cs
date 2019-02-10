using Extension.Database.SqlServer;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;

namespace Aibe.Helpers {
  public class LogHelper {
    public static void Access(string userName, string logType, string result = null, string logMessage = null) {
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("INSERT INTO [", DH.AccessLogTableName, "] VALUES(@par0, @par1, @par2, @par3, @par4)"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", userName),
          new SqlParameter("@par1", DateTime.Now),
          string.IsNullOrWhiteSpace(logType) ? new SqlParameter("@par2", DBNull.Value) : new SqlParameter("@par2", logType),
          string.IsNullOrWhiteSpace(result) ? new SqlParameter("@par3", DBNull.Value) : new SqlParameter("@par3", result),
          string.IsNullOrWhiteSpace(logMessage) ? new SqlParameter("@par4", DBNull.Value) : new SqlParameter("@par4", logMessage),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        Error(userName, "Access", null, null, null, logType, logMessage, ex.ToString());
      }
    }

    public static void Action(string userName, string callingType,
      string className, string tableName, string actionName, string logMessage = null) {
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("INSERT INTO [", DH.ActionLogTableName, "] VALUES(@par0, @par1, @par2, @par3, @par4, @par5, @par6)"));
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", userName),
          new SqlParameter("@par1", DateTime.Now),
          string.IsNullOrWhiteSpace(callingType) ? new SqlParameter("@par2", DBNull.Value) : new SqlParameter("@par2", callingType),
          string.IsNullOrWhiteSpace(className) ? new SqlParameter("@par3", DBNull.Value) : new SqlParameter("@par3", className),
          string.IsNullOrWhiteSpace(tableName) ? new SqlParameter("@par4", DBNull.Value) : new SqlParameter("@par4", tableName),
          string.IsNullOrWhiteSpace(actionName) ? new SqlParameter("@par5", DBNull.Value) : new SqlParameter("@par5", actionName),
          string.IsNullOrWhiteSpace(logMessage) ? new SqlParameter("@par6", DBNull.Value) : new SqlParameter("@par6", logMessage),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch (Exception ex) {
        Error(userName, "Action", callingType, className, tableName, actionName, logMessage, ex.ToString());
      }
    }

    static int nvarcharMax = 4000; //by default TODO currently hardcoded
    public static void Error(string userName, string errorCode, string callingType, string className, 
      string tableName, string actionName, string logMessage = null, string errorMessage = null) { //without EF, this is useful for the final error
      try {
        StringBuilder sb = new StringBuilder();
        sb.Append(string.Concat("INSERT INTO [", DH.ErrorLogTableName, "] VALUES(@par0, @par1, @par2, @par3, @par4, @par5, @par6, @par7, @par8)"));
        string errMsg = string.IsNullOrEmpty(errorMessage) ? errorMessage :
          errorMessage.Length > nvarcharMax ? errorMessage.Substring(0, nvarcharMax) : errorMessage;
        List<SqlParameter> pars = new List<SqlParameter> {
          string.IsNullOrWhiteSpace(userName) ? new SqlParameter("@par0", DBNull.Value) : new SqlParameter("@par0", userName),
          new SqlParameter("@par1", DateTime.Now),
          string.IsNullOrWhiteSpace(errorCode) ? new SqlParameter("@par2", DBNull.Value) : new SqlParameter("@par2", errorCode),
          string.IsNullOrWhiteSpace(callingType) ? new SqlParameter("@par3", DBNull.Value) : new SqlParameter("@par3", callingType),
          string.IsNullOrWhiteSpace(className) ? new SqlParameter("@par4", DBNull.Value) : new SqlParameter("@par4", className),
          string.IsNullOrWhiteSpace(tableName) ? new SqlParameter("@par5", DBNull.Value) : new SqlParameter("@par5", tableName),
          string.IsNullOrWhiteSpace(actionName) ? new SqlParameter("@par6", DBNull.Value) : new SqlParameter("@par6", actionName),
          string.IsNullOrWhiteSpace(logMessage) ? new SqlParameter("@par7", DBNull.Value) : new SqlParameter("@par7", logMessage),
          string.IsNullOrWhiteSpace(errMsg) ? new SqlParameter("@par8", DBNull.Value) : new SqlParameter("@par8", errMsg),
        };
        SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sb.ToString(), pars);
      } catch {
        //cannot be handled further
      }
    }
  }
}