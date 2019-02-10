using Extension.Models;
namespace Aibe.Models.Core {
  public class AffixColumnInfo : CommonBaseInfo {
    public string AffixValue { get; private set; }
    public AffixColumnInfo(string desc) : base(desc) {
      if (!HasRightSide) {
        IsValid = false;
        return;
      }
      AffixValue = RightSide;
    }
  }
}