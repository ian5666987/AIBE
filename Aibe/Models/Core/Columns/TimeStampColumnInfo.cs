using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class TimeStampColumnInfo : CommonBaseInfo {
    private static List<string> defaultRowActionsApplied = new List<string> { DH.CreateActionName, DH.EditActionName };
    public List<TimeStampColumnRowActionInfo> RowActionsApplied { get; private set; } = new List<TimeStampColumnRowActionInfo>();
    public TimeStampColumnInfo(string desc) : base(desc) {
      if (!IsValid)
        return;
      if (HasRightSide) //take whatever is in the right side
        RowActionsApplied = RightSide.GetTrimmedNonEmptyParts(',')
          .Select(x => new TimeStampColumnRowActionInfo(x))
          .Where(x => x.IsValid)
          .ToList();
      if (RowActionsApplied == null || RowActionsApplied.Count <= 0) { //valid but no row actions, means we apply to both
        RowActionsApplied = defaultRowActionsApplied
          .Select(x => new TimeStampColumnRowActionInfo(x))
          .ToList();
      };
    }    
  }
}