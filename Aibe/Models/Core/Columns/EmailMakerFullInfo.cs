using Extension.Database.SqlServer;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;

namespace Aibe.Models.Core {
  public class EmailMakerFullInfo {
    public EmailMakerTriggerInfo Trigger { get; private set; }
    public EmailMakerInfo Maker { get; private set; }
    public DataTable LastTriggeredTable { get; private set; }
    public bool IsValid { get; private set; }
    public EmailMakerFullInfo (EmailMakerTriggerInfo triggerInfo, EmailMakerInfo makerInfo) {
      if (triggerInfo == null || makerInfo == null || string.IsNullOrWhiteSpace(makerInfo.Name) || string.IsNullOrWhiteSpace(triggerInfo.Name) ||
        !makerInfo.Name.EqualsIgnoreCase(triggerInfo.Name))
        return;
      IsValid = true;
      Trigger = triggerInfo;
      Maker = makerInfo;
    }

    public bool TryTrigger(string tableSource, string rowAction, int cid, DataRow originalRow) {
      LastTriggeredTable = Trigger.GetTriggeredDataTable(tableSource, rowAction, cid, originalRow);
      if (LastTriggeredTable == null || LastTriggeredTable.Rows == null || LastTriggeredTable.Rows.Count <= 0)
        return false;
      return true;
    }

    public Dictionary <string, object> GetLastDataParameters(string tableName, string tableSource) {
      if (LastTriggeredTable == null || LastTriggeredTable.Rows == null || LastTriggeredTable.Rows.Count <= 0)
        return null;
      DataRow row = LastTriggeredTable.Rows[0]; //only takes from the first row
      Dictionary<string, object> result = new Dictionary<string, object>();
      DateTime now = DateTime.Now;
      DateTime today = new DateTime(now.Year, now.Month, now.Day);
      foreach (DataColumn column in LastTriggeredTable.Columns)
        result.Add(DH.ParameterDataPrefix + column.ColumnName, row[column]);
      result.Add(DH.ParameterTableName, tableName);
      result.Add(DH.ParameterTableSource, tableSource);
      result.Add(DH.ParameterNow, now);
      result.Add(DH.ParameterToday, today);
      return result;
    }

    public Dictionary<string, object> GetRawTransferredValues(Dictionary<string, object> parameters) {
      DataTable table = SQLServerHandler.GetFullDataTableWhere(DH.DataDBConnectionString, DH.EmailTemplateTableName,
        string.Concat(DH.EmailMakerTemplateNameColumnName, "=", Maker.TemplateName.AsSqlStringValue()));
      return SQLServerHandler.GetFirstRow(table);
    }
  }
}