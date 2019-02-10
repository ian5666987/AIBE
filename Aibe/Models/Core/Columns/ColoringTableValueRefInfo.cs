using System.Collections.Generic;
using Extension.String;
using Extension.Models;

namespace Aibe.Models.Core {
  public class ColoringTableValueRefInfo : BaseInfo { //It is actually used only by ColoringInfo, nevertheless, leave it separated for now
    //valid self keyword
    private static string self = DH.Self;
    //valid comparators
    private static List<string> operators = new List<string> { "+", "-" };
    public string RefTableName { get; private set; }
    public string RefTableColumn { get; private set; }
    public bool IsSelf { get; private set; }
    public int Cid { get; private set; }
    public string Operator { get; private set; }
    public int ShiftValue { get; private set; }
    public bool HasLastExpression { get; private set; }
    public ColoringTableValueRefInfo(string desc) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      var parts = desc.GetTrimmedNonEmptyParts(':');
      if (parts.Count < 2) //minimum contains of two parts
        return;
      IsValid = true;
      RefTableName = parts[0];
      RefTableColumn = parts[1];
      if (parts.Count < 3)
        return;

      string lastExpression = parts[2];

      SimpleExpression exp = new SimpleExpression(lastExpression, operators, false);
      if (!exp.IsValid) {
        IsValid = false;
        return;
      }

      //get either self or cid
      IsSelf = exp.LeftSide.ToUpper().Equals(self);
      if (!IsSelf) {
        int value;
        bool result = int.TryParse(exp.LeftSide, out value);
        if (result && value > 0) //this is proven to be a Cid value
          Cid = value;
      }

      //If not singular, then operates further the right hand side
      if (!exp.IsSingular) {
        int value;
        bool result = int.TryParse(exp.RightSide, out value);
        if (result && value > 0)
          ShiftValue = value;
      }

      //if the third expression part exists
      IsValid = (IsSelf || Cid > 0) && (exp.IsSingular || ShiftValue > 0); //can only be valid if Cid is found or it is self...
                                                                           //And must either be singular or (if not singular) contains the valid shift value
      HasLastExpression = IsValid; //up to this point assume to have last expression, if it is valid
    }

  }
}