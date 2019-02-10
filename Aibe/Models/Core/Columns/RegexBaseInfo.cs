using Extension.Models;
namespace Aibe.Models.Core {
  public abstract class RegexBaseInfo : BaseInfo {
    public string Name { get; protected set; }
    public string Content { get; protected set; }
    public RegexBaseInfo(string desc) : base(desc) {
      SimpleExpression exp = new SimpleExpression(desc, "=", false);
      if (!exp.IsValid || exp.IsSingular)
        return;
      Name = exp.LeftSide;
      Content = exp.RightSide;
    }
  }
}