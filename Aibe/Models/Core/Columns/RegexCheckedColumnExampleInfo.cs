using System.Collections.Generic;

namespace Aibe.Models.Core {
  public class RegexCheckedColumnExampleInfo : RegexBaseInfo {
    public RegexCheckedColumnExampleInfo(string desc) : base(desc) {
      IsValid = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Content);
    }
  }
}