using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class CustomDateTimeFormatInfo : CommonBaseInfo {
    public Dictionary<string, string> DtFormatDictionary { get; private set; } = new Dictionary<string, string>();
    public CustomDateTimeFormatInfo(string desc) : base(desc) {
      IsValid = false;
      if (!HasRightSide)
        return;
      var rightParts = RightSide.GetTrimmedNonEmptyParts('|');
      if (rightParts == null || rightParts.Count < 2)
        return;
      for (int i = 0; i < rightParts.Count; i += 2) {
        if (i + 1 >= rightParts.Count)
          break;
        if (!DH.AcceptablePageNames.Any(x => x.EqualsIgnoreCase(rightParts[i])) || 
          string.IsNullOrWhiteSpace(rightParts[i + 1]))
          continue;
        DtFormatDictionary.Add(DH.AcceptablePageNames
          .FirstOrDefault(x => x.EqualsIgnoreCase(rightParts[i])), rightParts[i + 1]);
      }
      IsValid = DtFormatDictionary.Any();
    }

    public bool IsAppliedFor(string pageName) {
      return DtFormatDictionary.Any(x => x.Key.EqualsIgnoreCase(pageName));
    }
  }
}