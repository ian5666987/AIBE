using System.Collections.Generic;

namespace Aibe.Models.Core {
  public class RegexCheckedColumnInfo : RegexBaseInfo { //This is not derived from base Info because it has different way of parsing things....
    public RegexCheckedColumnInfo(string desc) : base(desc) {
      IsValid = !string.IsNullOrWhiteSpace(Name) && !string.IsNullOrWhiteSpace(Content);
    }
  }
}