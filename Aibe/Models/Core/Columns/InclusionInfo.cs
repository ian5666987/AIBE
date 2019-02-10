using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class InclusionInfo : CommonBaseInfo {
    public List<string> Roles { get; private set; } = new List<string>();
    public InclusionInfo(string desc) : base(desc) {
      if (HasRightSide)
        Roles = RightSide.GetTrimmedNonEmptyParts(',');
    }

    /// <summary>
    /// If roles (distinction) is not specificed, or the specific role is specified, or the role is admin, then it is true
    /// </summary>
    /// <param name="role">
    /// The role to check the inclusion
    /// </param>
    /// <returns></returns>
    public bool IsForcelyIncluded(string role) { 
      if (DH.MainAdminRoles.Any(x => x.EqualsIgnoreCase(role)))
        return true;
      return Roles == null || !Roles.Any() || Roles.Any(x => x.EqualsIgnoreCase(role));
    }
  }
}