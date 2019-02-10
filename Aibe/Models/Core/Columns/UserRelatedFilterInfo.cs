using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class UserRelatedFilterInfo : BaseInfo { //this is specially parsed and thus not derived from CommonBaseInfo, but BaseInfo
    public string ThisColumnName { get; private set; } //something like "TeamAssigned"
    public bool HasColumnFreeCandidate { get { return ThisColumnFreeCandidates != null && ThisColumnFreeCandidates.Count >= 0; } }
    public List<string> ThisColumnFreeCandidates { get; private set; } //having like {All:Any,Unassigned}
    public string UserInfoColumnName { get; private set; } //something like "Team"
    public bool HasUserInfoColumnFreeCandidate { get { return UserInfoColumnFreeCandidates != null && UserInfoColumnFreeCandidates.Count >= 0; } }
    public List<string> UserInfoColumnFreeCandidates { get; private set; } //having like {All:Home,Ruler}
    public string Relationship { get; private set; } = "=";
    public UserRelatedFilterInfo(string desc) : base(desc) {
      var strs = desc.Split('=');
      if (strs.Length != 2)
        return;

      string leftStr = strs[0].Trim();
      string rightStr = strs[1].Trim();
      if (string.IsNullOrWhiteSpace(leftStr) || string.IsNullOrWhiteSpace(rightStr))
        return;

      var leftDivs = leftStr.GetTrimmedNonEmptyParts('|');
      var rightDivs = rightStr.GetTrimmedNonEmptyParts('|');

      if (leftDivs == null || rightDivs == null || leftDivs.Count <= 0 || rightDivs.Count <= 0)
        return;

      ThisColumnName = leftDivs[0];
      UserInfoColumnName = rightDivs[0];

      if (leftDivs.Count > 1) {
        for (int i = 1; i < leftDivs.Count; ++i) {
          string leftDiv = leftDivs[i];
          if (string.IsNullOrWhiteSpace(leftDiv) || !leftDiv.StartsWith("{") || !leftDiv.EndsWith("}") || leftDiv.Length < 5)
            continue;
          leftDiv = leftDiv.Substring(1, leftDiv.Length - 2);
          var parts = leftDiv.GetTrimmedNonEmptyParts(':');
          if (parts.Count < 2)
            continue;
          if (DH.UserRelatedDirectives.Any(x => x.EqualsIgnoreCase(parts[0]))) //as of now, only one candidate is acceptable: All
            ThisColumnFreeCandidates = parts[1].GetTrimmedNonEmptyParts(',');          
        }
      }

      if (rightDivs.Count > 1) {
        for (int i = 1; i < rightDivs.Count; ++i) {
          string rightDiv = rightDivs[i].Trim();
          if (string.IsNullOrWhiteSpace(rightDiv) || !rightDiv.StartsWith("{") || !rightDiv.EndsWith("}") || rightDiv.Length < 5)
            continue;
          rightDiv = rightDiv.Substring(1, rightDiv.Length - 2);
          var parts = rightDiv.GetTrimmedNonEmptyParts(':');
          if (parts.Count < 2)
            continue;
          if (DH.UserRelatedDirectives.Any(x => x.EqualsIgnoreCase(parts[0]))) //as of now, only one candidate is acceptable: All
            UserInfoColumnFreeCandidates = parts[1].GetTrimmedNonEmptyParts(',');          
        }
      }

      IsValid = true;
    }
  }
}