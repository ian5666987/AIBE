using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;

namespace Aibe.Models.Core {
  public class ActionProcedureFullInfo {
    public ActionTriggerInfo Trigger { get; private set; }
    public ActionProcedureInfo Procedure { get; private set; }
    public DataTable LastTriggeredTable { get; private set; }
    public bool IsValid { get; private set; }
    public bool HasTriggeredTable { get {
        return LastTriggeredTable != null && LastTriggeredTable.Rows != null && LastTriggeredTable.Rows.Count > 0;
    } }
    public ActionProcedureFullInfo(ActionTriggerInfo triggerInfo, ActionProcedureInfo procedureInfo) {
      if (triggerInfo == null || procedureInfo == null || string.IsNullOrWhiteSpace(procedureInfo.Name) || string.IsNullOrWhiteSpace(triggerInfo.Name) ||
        !procedureInfo.Name.EqualsIgnoreCase(triggerInfo.Name))
        return;
      IsValid = true;
      Trigger = triggerInfo;
      Procedure = procedureInfo;
    }

    public bool TryTrigger(string tableSource, string rowAction, int cid, DataRow originalRow) {
      LastTriggeredTable = Trigger.GetTriggeredDataTable(tableSource, rowAction, cid, originalRow);
      if (!Trigger.HasTriggerCondition) //if trigger does NOT have trigger condition        
        return Trigger.IsActionApplied(rowAction); //the try trigger is only checked against if the row action is applied      
      return HasTriggeredTable; //else it will only be successful if there is a triggered table
    }

    public Dictionary<string, object> GetLastDataParameters() {
      Dictionary<string, object> result = new Dictionary<string, object>();
      if (!HasTriggeredTable) //if there is no triggered table, then just get the base parameters here
        return result;
      DataRow row = LastTriggeredTable.Rows[0]; //only takes from the first row
      foreach (DataColumn column in LastTriggeredTable.Columns)
        result.Add("@@" + DH.ParameterDataPrefix + column.ColumnName, row[column]);
      return result;
    }
  }
}
