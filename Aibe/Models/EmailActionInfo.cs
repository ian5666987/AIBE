using Aibe.Models.Core;
using Extension.Database.SqlServer;
using Extension.Extractor;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Aibe.Models {
  public class EmailActionInfo {
    public EmailMakerFullInfo EmailMaker { get; set; }
    public Dictionary<string, object> Parameters { get; set; }
    public Dictionary<string, object> RawTransferredValues { get; set; }

    public BaseScriptModel GetTransferSQLScript() {
      List<SqlParameter> pars = new List<SqlParameter>();
      StringBuilder sb = new StringBuilder(string.Concat("INSERT INTO ", DH.EmailInfoTableName, " ("));
      StringBuilder backSb = new StringBuilder(" VALUES (");
      int index = 0;
      string parName = string.Empty;
      List<string> transferredColumnNames = 
        SQLServerHandler.GetColumns(DH.DataDBConnectionString, DH.EmailInfoTableName)
        .Select(x => x.ColumnName)
        .ToList();
      foreach (var item in RawTransferredValues) {
        if (item.Key.EqualsIgnoreCase(DH.Cid) || //Cid is explicitly not transferred
          !transferredColumnNames.Any(x => x.EqualsIgnoreCase(item.Key))) //If the column does not exist in the EmailInfo table, it is also cannot be transferred 
          continue;
        bool isSqlScript = item.Value is string && item.Value != null && 
          item.Value.ToString().ToUpper().StartsWith(DH.SQLScriptDirectivePrefix.ToUpper());
        if (isSqlScript && item.Value.ToString().Trim().Length <= DH.SQLScriptDirectivePrefix.Length)
          continue; //invalid SQL script
        if (index > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        parName = "@par" + index; //generic parIndex
        index++;
        sb.Append(item.Key);
        backSb.Append(parName);
        if (!(item.Value is string)) { //the simplest case, just immediately use the raw value
          pars.Add(new SqlParameter(parName, item.Value ?? DBNull.Value));
          continue;
        }
        string itemStr = item.Value.ToString();
        //this is a singular parameter @@Now
        if (Parameters.Any(x => itemStr.EqualsIgnoreCase(DH.PP + x.Key))) { 
          pars.Add(new SqlParameter(parName, Parameters.FirstOrDefault(x => itemStr.EqualsIgnoreCase(DH.PP + x.Key)).Value ?? DBNull.Value));
          continue;
        }

        //this is an SQL-Script
        if (isSqlScript) {
          string usedStr = itemStr.Substring(DH.SQLScriptDirectivePrefix.Length).Trim();
          string sqlScript = ApplyParameters(usedStr, true); //the item parameters must be changed to SQL-Script
          DataTable table = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, sqlScript);
          if (table == null || table.Rows == null || table.Rows.Count <= 0 ||
            table.Columns == null || table.Columns.Count <= 0) {
            pars.Add(new SqlParameter(parName, DBNull.Value));
          } else {
            List<object> objs = new List<object>();
            foreach (DataRow row in table.Rows) //for each row
              objs.Add(row.ItemArray[0]); //only gets the value of the first column
            if (objs.Count == 0)
              pars.Add(new SqlParameter(parName, DBNull.Value));
            else if (objs.Count == 1)
              pars.Add(new SqlParameter(parName, objs[0] ?? DBNull.Value)); //if there is only one object, put as it is
            else { //the objects must be stringify
              string strObj = string.Join(";", objs.Select(x => x == null ? string.Empty : x.ToString()));
              pars.Add(new SqlParameter(parName, (object)strObj ?? DBNull.Value));
            }
          }
          continue;
        } 

        //neutralized parameter, a pure string with parameter like "test at @@Item"
        string pureString = ApplyParameters(itemStr, false);
        pars.Add(new SqlParameter(parName, (object)pureString ?? DBNull.Value));
      }

      if (Parameters != null) {
        StringBuilder parsb = new StringBuilder();
        int parIndex = 0;
        foreach (var par in Parameters) {
          if (parIndex > 0)
            parsb.Append("; ");
          ++parIndex;
          parsb.Append(par.Key);
          parsb.Append("=");
          BaseSystemData data = new BaseSystemData(par.Value);
          parsb.Append(data.GetParameterValueString());
        }
        parName = "@par" + index;
        if (index > 0) {
          sb.Append(", ");
          backSb.Append(", ");
        }
        sb.Append(DH.EmailMakerParamatersColumnName);
        backSb.Append(parName);
        string parsString = parsb.ToString();
        pars.Add(new SqlParameter(parName, (object)parsString ?? DBNull.Value));
      }

      sb.Append(")");
      backSb.Append(")");
      BaseScriptModel model = new BaseScriptModel(string.Concat(sb.ToString(), backSb.ToString()), pars);
      return model;
    }

    public string ApplyParameters(string itemStr, bool asSqlString) {
      if (string.IsNullOrWhiteSpace(itemStr))
        return itemStr;
      MatchCollection matches = DH.ParRegex.Matches(itemStr);
      Dictionary<string, string> replacements = new Dictionary<string, string>();
      //StringBuilder sb = new StringBuilder();
      foreach (Match match in matches) {
        string matchStr = match.ToString().Trim();
        string replacementStr = DH.NULL; //so that it won't be considered as column name in the case of SQL Script
        //sb.AppendLine("Match: " + matchStr);
        //foreach (var par in Parameters)
        //  sb.AppendLine(par.Key + ": " + par.Value);
        if (Parameters.Any(x => matchStr.EqualsIgnoreCase(DH.PP + x.Key))) {//if found
          object obj = Parameters.First(x => matchStr.EqualsIgnoreCase(DH.PP + x.Key)).Value;
          BaseSystemData data = new BaseSystemData(obj);
          replacementStr = asSqlString ? data.GetSqlValueString() : data.Value?.ToString();
        }

        if (!replacements.ContainsKey(matchStr))
          replacements.Add(matchStr, replacementStr);        
      }
      string result = itemStr;
      foreach (var replacement in replacements)
        result = result.Replace(replacement.Key, replacement.Value ?? string.Empty);
      return result;
    }
  }
}