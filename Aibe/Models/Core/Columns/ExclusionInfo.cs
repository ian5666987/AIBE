using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class ExclusionInfo : CommonBaseInfo {
    public List<string> Roles { get; private set; } = new List<string>();
    public ExclusionInfo(string desc) : base(desc) {
      if (HasRightSide)
        Roles = RightSide.GetTrimmedNonEmptyParts(',');
    }

    public bool IsExcluded (string role) { //if roles are empty means no body is allowed
      if (DH.MainAdminRoles.Any(x => x.EqualsIgnoreCase(role))) //role in the main admin roles cannot be excluded
        return false;
      return Roles == null || Roles.Any(x => x.EqualsIgnoreCase(role));
    }

    public override string ToString() {
      return Name.ToString() + (Roles != null && Roles.Count > 0 ? " {" + string.Join(", ", Roles) + "}" : " {All}");
    }
  }
}