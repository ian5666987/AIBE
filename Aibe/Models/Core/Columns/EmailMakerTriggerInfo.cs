using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aibe.Models.Core {
  public class EmailMakerTriggerInfo : CommonBaseInfo {
    public List<string> RowActions { get; private set; } = new List<string>();
    public string TriggerConditionScript { get; private set; }
    public EmailMakerTriggerInfo(string desc) : base(desc) {
      IsValid = false;
      if (!HasRightSide)
        return;
      if (Extension.Checker.DB.ContainsUnenclosedDangerousElement(RightSide))
        return;
      int index = Name.IndexOf('|');
      if (index >= 0) {
        SimpleExpression exp = new SimpleExpression(Name, "|", false);
        if (!exp.IsValid)
          return;
        Name = exp.LeftSide;
        if (!exp.IsSingular)
          RowActions = exp.RightSide.GetTrimmedNonEmptyParts(',')
            .Select(x => DH.BaseTriggerRowActions.FirstOrDefault(y => y.EqualsIgnoreCase(x)))
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToList();
      }
      TriggerConditionScript = RightSide;
      IsValid = true;
    }

    public DataTable GetTriggeredDataTable(string tableSource, string rowAction, int cid, DataRow originalRow) {
      if (RowActions != null && RowActions.Count > 0 && !RowActions.Any(x => x.EqualsIgnoreCase(rowAction)))
        return null; //if there is something in row action, and nont of them is not what is allowed to trigger, return null
      string usedTriggerCondition = string.Concat(DH.Cid, "=", cid, string.IsNullOrWhiteSpace(TriggerConditionScript) ? string.Empty :
        string.Concat(" AND (", TriggerConditionScript, ")"));
      DataTable table = SQLServerHandler.GetFullDataTableWhere(DH.DataDBConnectionString, tableSource, usedTriggerCondition);
      if (table == null || table.Rows == null || table.Rows.Count <= 0)
        return null;
      if (originalRow != null) {
        bool isIdentical = Extension.Checker.DB.DataRowEquals(originalRow, table.Rows[0]);
        if (isIdentical)
          return null; //do not trigger data table when everything is identical
      }
      return table;
    }
  }
}