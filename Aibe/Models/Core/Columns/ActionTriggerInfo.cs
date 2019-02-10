using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aibe.Models.Core {
  public class ActionTriggerInfo : CommonBaseInfo {
    public string TriggerName { get; private set; }
    public List<string> RowActions { get; private set; } = new List<string>();
    public bool MustEditHaveChange { get; private set; } = true;
    public string TriggerConditionScript { get; private set; }
    public bool HasTriggerCondition { get { return !string.IsNullOrWhiteSpace(TriggerConditionScript); } } //check if trigger condition exist
    public ActionTriggerInfo(string desc) : base(desc) {
      if (!IsValid)
        return;

      //The "Name" here is in the form of TriggerName3|RowAction31,RowAction32,…|MustEditHaveChange thus use further divider
      var parts = Name.GetTrimmedNonEmptyParts('|');
      if (parts == null || parts.Count <= 0 || string.IsNullOrWhiteSpace(parts[0])) { //at least the first part must have something
        IsValid = false;
        return;
      }

      Name = parts[0]; //just to avoid confusion and letting this having different things than TriggerName, make them the same here
      TriggerName = parts[0];

      if (parts.Count > 1) {
        //RowAction31,RowAction32,…
        RowActions = parts[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
          .Where(x => DH.BaseTriggerRowActions.Any(y => y.EqualsIgnoreCaseTrim(x)))
          .Select(x => DH.BaseTriggerRowActions.First(y => y.EqualsIgnoreCaseTrim(x)))
          .ToList();

        if (parts.Count > 2) {
          bool mustEditHaveChangeTest;
          bool result = bool.TryParse(parts[2], out mustEditHaveChangeTest);
          if (result) //If exists and if the result is shown to be valid
            MustEditHaveChange = mustEditHaveChangeTest; //then takes it, whatever is the result
        }
      }

      if (!HasRightSide) //if does not have right side, then we can return here
        return;

      if (Extension.Checker.DB.ContainsUnenclosedDangerousElement(RightSide)) {
        IsValid = false;
        return;
      }

      TriggerConditionScript = RightSide;
    }

    public bool IsActionApplied(string rowAction) {
      //applied if:
      //- not defined
      //- empty deinition
      //- defined and exist
      return RowActions == null || RowActions.Count <= 0 || RowActions.Any(x => x.EqualsIgnoreCase(rowAction));
    }

    public DataTable GetTriggeredDataTable(string tableSource, string rowAction, int cid, DataRow originalRow) {
      if (!IsActionApplied(rowAction) || cid <= 0)
        return null; 
      string usedTriggerCondition = string.Concat(DH.Cid, "=", cid, 
        string.IsNullOrWhiteSpace(TriggerConditionScript) ? //Trigger Condition Script may be empty, yet if cid is supplied, we can still get the triggered data
        string.Empty : string.Concat(" AND (", TriggerConditionScript, ")"));
      DataTable table = SQLServerHandler.GetFullDataTableWhere(DH.DataDBConnectionString, tableSource, usedTriggerCondition);
      if (table == null || table.Rows == null || table.Rows.Count <= 0)
        return null;
      //only if the edit must have changed and the current row action is edit we could go here
      if (originalRow != null && MustEditHaveChange && rowAction.EqualsIgnoreCase(DH.EditActionName)) { 
        bool isIdentical = Extension.Checker.DB.DataRowEquals(originalRow, table.Rows[0]);
        if (isIdentical)
          return null; //do not trigger data table when everything is identical
      }
      return table;
    }
  }
}

//May not have the right side
//if (!HasRightSide) { //must have the right side to be valid
//  IsValid = false;
//  return;
//}
