using Extension.Models;
using Extension.String;
using System.Linq;

namespace Aibe.Models.Core {
  public class OrderByInfo : CommonBaseInfo {
    public string OrderDirection { get; private set; }
    public string Script { get; private set; }
    public bool IsScript { get { return !string.IsNullOrWhiteSpace(Script) && IsValid; } }
    public OrderByInfo(string desc, bool scripted) : base(desc) {
      if (scripted) { //special orderBy
        string testScript = OriginalDesc.Substring(DH.SQLScriptDirectivePrefix.Length);
        bool result = Extension.Checker.DB.ContainsUnenclosedDangerousElement(testScript);
        IsValid = !result;
        if (result)
          return;
        Script = testScript;
      } else if (HasRightSide && DH.ValidOrderDirections.Any(x => x.EqualsIgnoreCaseTrim(RightSide))) //legacy way of getting the info
        OrderDirection = DH.ValidOrderDirections.FirstOrDefault(x => x.EqualsIgnoreCaseTrim(RightSide));
    }

    public string GetOrderDirection () { return string.IsNullOrWhiteSpace(OrderDirection) ? string.Empty : OrderDirection; }
  }
}