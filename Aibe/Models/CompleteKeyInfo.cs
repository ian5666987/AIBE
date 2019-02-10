using System.Collections.Generic;

namespace Aibe.Models {
  public class CompleteKeyInfo {
    public List<KeyInfo> ValidKeys { get; set; }
    public List<KeyInfo> NullifiedKeys { get; set; }
  }
}