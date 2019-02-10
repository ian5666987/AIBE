using System.Collections.Generic;
using Extension.Models;
using Extension.String;
using System.Data;
using Extension.Database.SqlServer;
using System.Linq;
using System;

namespace Aibe.Models.Core {
  public class ForeignInfoColumnInfo : CommonBaseInfo {
    public string RefTableName { get; set; }
    public string ForeignKeyColumn { get; set; }
    public List<Tuple<string, string, string>> RefColumnNameTrios { get; set; } = new List<Tuple<string, string, string>>();
    public bool IsFullForeignInfo { get { return RefColumnNameTrios == null || RefColumnNameTrios.Count <= 0; } }
    public ForeignInfoColumnInfo(string desc) : base(desc) {
      if (!HasRightSide) {
        IsValid = false;
        return;
      }
      var rightParts = RightSide.GetTrimmedNonEmptyParts(':');
      if (rightParts.Count < 2) { //there must be at least two right parts
        IsValid = false;
        return;
      }
      IsValid = true;
      RefTableName = rightParts[0];
      ForeignKeyColumn = rightParts[1];

      if (rightParts.Count < 3)
        return;

      var columnNames = rightParts[2].GetTrimmedNonEmptyParts(',');
      foreach (var columnName in columnNames) {
        SimpleExpression exp = new SimpleExpression(columnName, "|", false);
        if (!exp.IsValid)
          continue;
        SimpleExpression rightExp = new SimpleExpression(exp.RightSide, "|", false);
        bool rightExpIsSingular = !rightExp.IsValid || rightExp.IsSingular;
        RefColumnNameTrios.Add(new Tuple<string, string, string>(exp.LeftSide,
          exp.IsSingular ? exp.LeftSide.ToCamelBrokenString() : 
          rightExpIsSingular ? exp.RightSide : rightExp.LeftSide,
          rightExpIsSingular ? null : rightExp.RightSide));
      }
    }

    public bool HasAnyAssignedColumn() {
      return RefColumnNameTrios != null && RefColumnNameTrios.Any(x => !string.IsNullOrWhiteSpace(x.Item3));      
    }

    public bool HasAssignedColumn(string foreignColumnName) {
      var trio = RefColumnNameTrios.FirstOrDefault(x => x.Item1.EqualsIgnoreCase(foreignColumnName)); //has trio data
      return trio != null && !string.IsNullOrWhiteSpace(trio.Item3);
    }

    public string GetAssignedColumn(string foreignColumnName) {
      var trio = RefColumnNameTrios.FirstOrDefault(x => x.Item1.EqualsIgnoreCase(foreignColumnName)); //has trio data
      return trio == null ? null : trio.Item3;
    }

    public List<KeyValuePair<string, object>> GetAssignedDataDictionary(object foreignValue) {
      List<KeyValuePair<string, object>> foreignDataDict = GetForeignDataDictionary(foreignValue);
      return commonGetAssignedDataDictionary(foreignDataDict);
    }

    public List<KeyValuePair<string, object>> GetAssignedDataDictionary(string foreignValueInString) {
      List<KeyValuePair<string, object>> foreignDataDict = GetForeignDataDictionary(foreignValueInString);
      return commonGetAssignedDataDictionary(foreignDataDict);
    }

    private List<KeyValuePair<string, object>> commonGetAssignedDataDictionary(List<KeyValuePair<string, object>> foreignDataDict) {
      List<KeyValuePair<string, object>> results = new List<KeyValuePair<string, object>>();
      foreach (var pair in foreignDataDict) {
        var trio = RefColumnNameTrios.FirstOrDefault(x => x.Item1.EqualsIgnoreCase(pair.Key)); //has trio data
        if (!string.IsNullOrWhiteSpace(trio.Item3))
          results.Add(new KeyValuePair<string, object>(trio.Item3, pair.Value));
      }
      return results;
    }

    //public DataRow GetForeignData(string foreignKey, string foreignValueInString)
    public DataRow GetForeignData(string foreignValueInString) {
      List<DataColumn> columns = SQLServerHandler.GetColumns(DH.DataDBConnectionString, RefTableName);
      DataColumn foreignKeyColumn = columns.FirstOrDefault(x => x.ColumnName.EqualsIgnoreCase(ForeignKeyColumn));
      Type foreignKeyColumnDataType = foreignKeyColumn.DataType;
      object foreignValue = foreignValueInString == null ? string.Empty : foreignValueInString.Convert(foreignKeyColumnDataType);
      return GetForeignData(foreignValue);
    }

    //public DataRow GetForeignData(string foreignKey, object foreignValue)
    public DataRow GetForeignData(object foreignValue) {
      List<string> columnNames = RefColumnNameTrios != null && RefColumnNameTrios.Count > 0 ? 
        RefColumnNameTrios.Select(x => x.Item1).ToList() : null;
      return SQLServerHandler.GetPartialFirstDataRowFilterByParameters(DH.DataDBConnectionString,
        RefTableName, columnNames, new Dictionary<string, object> { { ForeignKeyColumn, foreignValue } });
    }

    //public List<KeyValuePair<string, object>> GetForeignDataDictionary(string foreignKey, string foreignValueInString)
    public List<KeyValuePair<string, object>> GetForeignDataDictionary(string foreignValueInString) {
      //DataRow row = GetForeignData(foreignKey, foreignValueInString);
      DataRow row = GetForeignData(foreignValueInString);
      List<DataColumn> columns = GetAffectedColumns();
      List<KeyValuePair<string, object>> results = new List<KeyValuePair<string, object>>();
      foreach(var column in columns) {
        object val = row[column.ColumnName];
        results.Add(new KeyValuePair<string, object>(column.ColumnName, val is DBNull ? null : val));
      }
      return results;
    }

    //public List<KeyValuePair<string, object>> GetForeignDataDictionary(string foreignKey, object foreignValue)
    public List<KeyValuePair<string, object>> GetForeignDataDictionary(object foreignValue) {
      //DataRow row = GetForeignData(foreignKey, foreignValue);
      DataRow row = GetForeignData(foreignValue);
      List<DataColumn> columns = GetAffectedColumns();
      List<KeyValuePair<string, object>> results = new List<KeyValuePair<string, object>>();
      foreach (var column in columns) {
        object val = row[column.ColumnName];
        results.Add(new KeyValuePair<string, object>(column.ColumnName, val is DBNull ? null : val));
      }
      return results;
    }

    public List<DataColumn> GetForeignFullColumns() {
      return SQLServerHandler.GetColumns(DH.DataDBConnectionString, RefTableName);
    }

    public List<DataColumn> GetAffectedColumns() {
      var columns = GetForeignFullColumns();
      if (RefColumnNameTrios == null || RefColumnNameTrios.Count <= 0)
        return columns;
      var columnNames = RefColumnNameTrios.Select(x => x.Item1).ToList();
      List<DataColumn> affectedColumns = new List<DataColumn>();
      foreach(var columnName in columnNames) {
        DataColumn column = columns.FirstOrDefault(x => x.ColumnName.EqualsIgnoreCase(columnName));
        if (column == null)
          continue;
        affectedColumns.Add(column);
      }
      return affectedColumns;
    }
  }
}

//public string GetAssignedColumn(string foreignColumnName) {
//  var result = RefColumnNameTrios.FirstOrDefault(x => x.Item1.EqualsIgnoreCase(foreignColumnName));
//  return result == null || string.IsNullOrWhiteSpace(result.Item3) ? null : result.Item3;
//}

//public void AssignForeignData(object foreignValue, string tableSource, int cid) {
//  List<KeyValuePair<string, object>> dict = GetForeignDataDictionary(foreignValue);
//  commonAssignForeignData(dict, tableSource, cid);
//}

//public void AssignForeignData(string foreignValueInString, string tableSource, int cid) {
//  List<KeyValuePair<string, object>> dict = GetForeignDataDictionary(foreignValueInString);
//  commonAssignForeignData(dict, tableSource, cid);
//}

//private void commonAssignForeignData(List<KeyValuePair<string, object>> dict, string tableSource, int cid) {
//  if (IsFullForeignInfo) //we cannot do this if it is full, it must be one by one
//    return;
//  Dictionary<string, object> updates = new Dictionary<string, object>();
//  foreach (var foreignItem in dict) {
//    var trioItem = RefColumnNameTrios.FirstOrDefault(x => x.Item1 == foreignItem.Key);
//    if (!string.IsNullOrWhiteSpace(trioItem.Item3))
//      updates.Add(trioItem.Item3, foreignItem.Value); //insert foreignItem.Value into trioItem.Item3 column in an update statement for this
//  }
//  if (updates.Count > 0) //there is update
//    SQLServerHandler.UpdateWhere(DH.DataDBConnectionString, tableSource, updates, string.Concat(DH.Cid, "=", cid));
//}
