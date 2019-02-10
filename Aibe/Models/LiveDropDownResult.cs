using System.Collections.Generic;

namespace Aibe.Models {
  public class LiveDropDownResult {
    public string ColumnName { get; set; }
    public List<string> Values { get; set; }
    public string ViewString { get; set; }
    public DropdownPassedArguments Arg { get; set; }
    public string ArgOriginalValue { get { return Arg?.OriginalValue?.ToString(); } }
  }
}