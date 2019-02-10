using Extension.Models;
using Extension.String;

namespace Aibe.Models.Core {
  public class PrefilledColumnInfo : CommonBaseInfo {
    public string Value { get; private set; }
    public PrefilledColumnInfo(string desc) : base(desc) {
      if (!HasRightSide) {
        IsValid = false;
        return;
      }
      Value = RightSide.ExtractSqlValue();
    }
  }
}