using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class ActionProcedureInfo : CommonBaseInfo {
    public string TriggerName { get; private set; }
    public bool IsUser { get; private set; }
    public string FullProcedureString { get; private set; }
    public string ProcedureName { get; private set; }
    public List<ActionProcedureParameterInfo> ProcedureParameters { get; private set; } = new List<ActionProcedureParameterInfo>();
    public ActionProcedureInfo(string desc) : base(desc) {
      if (!HasRightSide) { //must have the right side to be valid
        IsValid = false;
        return;
      }
      TriggerName = Name;
      if (RightSide.StartsWith(DH.UserPrefix)) {
        if (RightSide.Length <= DH.UserPrefix.Length) {
          IsValid = false;
          return;
        }
        IsUser = true;
      }

      FullProcedureString = IsUser ? RightSide.Substring(DH.UserPrefix.Length).Trim() : RightSide;
      if (string.IsNullOrWhiteSpace(FullProcedureString)) {
        IsValid = false;
        return;
      }

      SimpleExpression exp = new SimpleExpression(FullProcedureString, "(", false);
      if (!exp.IsValid) {
        IsValid = false;
        return;
      }
      ProcedureName = exp.LeftSide;
      if (exp.IsSingular) //has no parameter
        return; //simply returns, it has finished
      if (!exp.RightSide.EndsWith(")") || exp.RightSide.Length <= 1) {
        IsValid = false; //false because the right side does not contain the last ")"
        return;
      }

      string rightSide = exp.RightSide.Substring(0, exp.RightSide.Length - 1).Trim();
      if (string.IsNullOrWhiteSpace(rightSide)) {
        IsValid = false;
        return;
      }
      //has parameters
      var parPairs = rightSide.ParseComponents(',');
      if (!parPairs.Any()) //if there isn't any, just return
        return;
      var pPars = SQLServerHandler.GetProcedureParameters(IsUser ? DH.UserDBConnectionString : DH.DataDBConnectionString, ProcedureName);
      foreach (var parPair in parPairs) { //@SpPar=@@SomeItem.Something
        SimpleExpression subExp = new SimpleExpression(parPair, "=", false);
        if (!subExp.IsValid || subExp.IsSingular) {
          IsValid = false; //invalid, immediately returns
          return;
        }
        if (!ProcedureParameters.Any(x => x.ParameterName.EqualsIgnoreCase(subExp.LeftSide)) && //the parameter has not existed yet
          pPars.Any(x => x.Key.EqualsIgnoreCase(subExp.LeftSide))) { //and the parameter in the database expects that
          var pPar = pPars.First(x => x.Key.EqualsIgnoreCase(subExp.LeftSide)); //get the relevant actual procedure parameters
          Type pParDataType = SQLServerHandler.GetEquivalentDataType(pPar.Value);
          ActionProcedureParameterInfo pParInfo = new ActionProcedureParameterInfo {
            ParameterName = pPar.Key,
            ParameterValueString = subExp.RightSide,
            DataType = pParDataType,
          };
          ProcedureParameters.Add(pParInfo);
        } else { //immediately wrong when duplicated
          IsValid = false;
          return;
        }
      }
    }
  }
}
