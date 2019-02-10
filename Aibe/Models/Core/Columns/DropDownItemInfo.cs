using Extension.Models;
using Extension.String;

namespace Aibe.Models.Core {
  public class DropDownItemInfo : BaseInfo {
    public bool IsItem { get { return !string.IsNullOrWhiteSpace(Item); } } //either item or table reference
    public string Item { get; private set; } //to store item
    public TableValueRefInfo RefInfo { get; private set; } //to store table value reference
    public DropDownItemInfo(string desc) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      string descContent = desc.GetNonEmptyTrimmedInBetween("[", "]");
      if (string.IsNullOrWhiteSpace(descContent)) { //if not found, then considers this an item, immediately returns it
        Item = desc;
        IsValid = true;
        return;
      }

      //Otherwise it must be table value reference
      TableValueRefInfo testRefInfo = new TableValueRefInfo(descContent);
      if (!testRefInfo.IsValid)
        return;

      RefInfo = testRefInfo;
      IsValid = true;
    }
  }
}