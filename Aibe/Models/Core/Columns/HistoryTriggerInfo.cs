using Extension.Models;
using Extension.Database.SqlServer;
using Extension.String;
using System.Data;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class HistoryTriggerInfo : CommonBaseInfo { //likely to be unique, not common, but common do just fine when properly constructed
    public bool IsDataDeleted { get; private set; } = true;
    public bool MustEditHaveChange { get; private set; } = true;
    public List<string> RowActions { get; private set; } = new List<string>();
    public string TriggerConditionScript { get; private set; }
    public HistoryTriggerInfo(string desc) : base(desc) { //IsDataDeleted,MustEditHaveChange|RowActionN1,RowActionN2,…=TC-SQLS-N
      IsValid = false;
      if (!HasRightSide || string.IsNullOrWhiteSpace(Name))
        return;
      if (!Name.ToLower().StartsWith(DH.True.ToLower()) && !Name.ToLower().StartsWith(DH.False.ToLower()))
        return;
      if (Extension.Checker.DB.ContainsUnenclosedDangerousElement(RightSide))
        return;
      IsDataDeleted = Name.ToLower().StartsWith(DH.True.ToLower());
      int index = Name.IndexOf('|');
      if (index >= 0) {
        SimpleExpression exp = new SimpleExpression(Name, "|", false);
        if (!exp.IsValid)
          return;
        int leftCommaIndex = exp.LeftSide.IndexOf(',');
        if (leftCommaIndex >= 0 && exp.LeftSide.Length > leftCommaIndex + 1) {
          string mustEditHaveChangeString = exp.LeftSide.Substring(leftCommaIndex + 1).Trim();
          if (mustEditHaveChangeString.EqualsIgnoreCase(DH.True) || mustEditHaveChangeString.EqualsIgnoreCase(DH.False)) //validity check
            MustEditHaveChange = mustEditHaveChangeString.EqualsIgnoreCase(DH.True);
        }
        if (!exp.IsSingular)
          RowActions = exp.RightSide.GetTrimmedNonEmptyParts(',')
            .Select(x => DH.BaseTriggerRowActions.FirstOrDefault(y => y.EqualsIgnoreCase(x)))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
      } else { //it is still possible to have MustEditHaveChanged here
        SimpleExpression exp = new SimpleExpression(Name, ",", false);
        if (exp.IsValid && !exp.IsSingular) //nothing is in the right of the name
          if (exp.RightSide.EqualsIgnoreCase(DH.True) || exp.RightSide.EqualsIgnoreCase(DH.False))
            MustEditHaveChange = exp.RightSide.EqualsIgnoreCase(DH.True);
      }
      Name = string.Empty; //emptified the Name to avoid confusion outside...
      TriggerConditionScript = RightSide;
      IsValid = true; //as long as it does not contain unenclosed dangerous element, it is OK to have
    }

    public DataTable GetTriggeredDataTable(string tableSource, string rowAction, int cid, DataRow originalRow) {
      if (RowActions != null && RowActions.Count > 0 && !RowActions.Any(x => x.EqualsIgnoreCase(rowAction)))
        return null; //if there is something in row action, and nont of them is not what is allowed to trigger, return null
      string usedTriggerCondition = string.Concat(DH.Cid, "=", cid, string.IsNullOrWhiteSpace(TriggerConditionScript) ? string.Empty :
        string.Concat(" AND (", TriggerConditionScript, ")"));
      DataTable table = SQLServerHandler.GetFullDataTableWhere(DH.DataDBConnectionString, tableSource, usedTriggerCondition);
      if (table == null || table.Rows == null || table.Rows.Count <= 0)
        return null;
      if (originalRow != null && MustEditHaveChange) { //only if edit must have changed is true we could check this. Otherwise, skip checking.
        bool isIdentical = Extension.Checker.DB.DataRowEquals(originalRow, table.Rows[0]);
        if (isIdentical)
          return null; //do not trigger data table when everything is identical
      }
      return table;
    }
  }
}