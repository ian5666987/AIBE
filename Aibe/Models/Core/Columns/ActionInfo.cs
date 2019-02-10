using System.Collections.Generic;
using Extension.Models;
using Extension.String;
using System.Linq;

namespace Aibe.Models.Core {
  public class ActionInfo : CommonBaseInfo {
    public List<string> Roles { get; private set; } = new List<string>();
    public ActionInfo(string desc) : base(desc) {
      if (HasRightSide)
        Roles = RightSide.GetTrimmedNonEmptyParts(',');
    }

    public bool IsAllowed(string role) {
      return DH.MainAdminRoles.Any(x => x.EqualsIgnoreCase(role)) || //main admin roles are always allowed
          (!string.IsNullOrWhiteSpace(role) &&
        (Roles == null || !Roles.Any() || Roles.Any(x => x.EqualsIgnoreCase(role))));
    }
  }
}