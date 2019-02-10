using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class AttachmentInfo : CommonBaseInfo {
    public List<string> Formats { get; private set; } = new List<string>();
    public AttachmentInfo(string desc) : base(desc) {
      if (HasRightSide)
        Formats = RightSide.GetTrimmedNonEmptyParts(',')
          .Select(x => string.Concat(".", x)).ToList();
    }
  }
}