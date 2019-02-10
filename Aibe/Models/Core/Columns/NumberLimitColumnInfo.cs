using Extension.Models;
using Extension.String;
using System.Collections.Generic;

namespace Aibe.Models.Core {
  public class NumberLimitColumnInfo : CommonBaseInfo {
    public double Min { get; private set; } = double.MinValue; //preset as extreme values
    public double Max { get; private set; } = double.MaxValue;
    public NumberLimitColumnInfo(string desc) : base(desc) {
      if (!HasRightSide) //TODO, remember to change the number limits parser to use "|" in the docs and also in the code. Done in the code, in the docs remain unchanged yet (2017-09-12)
        return;

      List<string> parts = RightSide.GetTrimmedNonEmptyParts('|');
      foreach (string part in parts) {
        SimpleExpression exp = new SimpleExpression(part, ":", false);
        if (!exp.IsValid || exp.IsSingular)
          continue;
        double value;
        bool result;
        if (exp.LeftSide.EqualsIgnoreCaseTrim(DH.Min)) {
          result = double.TryParse(exp.RightSide, out value);
          if (result) //if the parse is successful, then use it as number limit
            Min = value;
        } else if (exp.LeftSide.EqualsIgnoreCaseTrim(DH.Max)) {
          result = double.TryParse(exp.RightSide, out value);
          if (result) //if the parse is successful, then use it as number limit
            Max = value;
        }
      }
    }
  }
}