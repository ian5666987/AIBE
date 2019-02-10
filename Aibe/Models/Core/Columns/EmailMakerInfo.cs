using Extension.Models;

namespace Aibe.Models.Core {
  public class EmailMakerInfo : CommonBaseInfo {
    public string TemplateName { get; private set; }
    public EmailMakerInfo(string desc) : base(desc) {
      if (!HasRightSide) {
        IsValid = false;
        return;
      }
      TemplateName = RightSide;
    }
  }
}