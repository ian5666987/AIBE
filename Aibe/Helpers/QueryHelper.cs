using Aibe.Models.Core;
using Extension.Database.SqlServer;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Aibe.Helpers {
  public class QueryHelper {
    public static void HandleUserRelatedScripting(StringBuilder queryScript, string tableName, string userName, bool hasMainAdminRight,
      bool isAuthenticated, List<UserRelatedFilterInfo> userRelatedInfos) {
      bool whereExisted = queryScript.ToString().ToLower().Contains(" where ");
      StringBuilder userRelatedQueryScript = new StringBuilder();
      if (!hasMainAdminRight && //User is not main admin (main admin is free from user related filters)
        isAuthenticated && //User is authenticated
        userRelatedInfos != null && userRelatedInfos.Count > 0) { //there is a user related info
        //Get user info here!
        List<DataColumn> userColumns = new List<DataColumn>();
        string userQueryScript = "SELECT TOP 1 * FROM [" + tableName + "] WHERE [" + Aibe.DH.UserNameColumnName + "] = " +
          (string.IsNullOrWhiteSpace(userName) ? "''" : userName.AsSqlStringValue());
        DataTable userDataTable = SQLServerHandler.GetDataTable(Aibe.DH.UserDBConnectionString, userQueryScript);

        foreach (DataColumn column in userDataTable.Columns)
          userColumns.Add(column);

        DataRow userRow = userDataTable.Rows[0];
        int userRelatedInfoCount = 0;
        for (int i = 0; i < userRelatedInfos.Count; ++i) {
          if (userRelatedInfoCount > 0)
            userRelatedQueryScript.Append(" AND ");
          UserRelatedFilterInfo userRelatedInfo = userRelatedInfos[i];
          object userVal = userRow[userRelatedInfo.UserInfoColumnName];
          if (userRelatedInfo.HasUserInfoColumnFreeCandidate)
            if (userRelatedInfo.UserInfoColumnFreeCandidates.Contains(userVal.ToString())) //this user does not have the filter applied
              continue;
          userRelatedQueryScript.Append("([");
          userRelatedQueryScript.Append(userRelatedInfo.ThisColumnName);
          userRelatedQueryScript.Append("]=");
          userRelatedQueryScript.Append("'"); //as of now, assuming string
          userRelatedQueryScript.Append(userVal);
          userRelatedQueryScript.Append("'"); //as of now, assuming string
          if (userRelatedInfo.HasColumnFreeCandidate) {
            foreach (var cand in userRelatedInfo.ThisColumnFreeCandidates) {
              userRelatedQueryScript.Append(" OR [");
              userRelatedQueryScript.Append(userRelatedInfo.ThisColumnName);
              userRelatedQueryScript.Append("]=");
              userRelatedQueryScript.Append("'"); //as of now, assuming string
              userRelatedQueryScript.Append(cand);
              userRelatedQueryScript.Append("'"); //as of now, assuming string
            }
            //Means "Any" user will do
          }
          userRelatedQueryScript.Append(")");
          userRelatedInfoCount++;
        }
      }

      if (!string.IsNullOrWhiteSpace(userRelatedQueryScript.ToString())) {
        queryScript.Append(whereExisted ? " AND " : " WHERE ");
        queryScript.Append(userRelatedQueryScript.ToString());
      }
    }
  }
}
