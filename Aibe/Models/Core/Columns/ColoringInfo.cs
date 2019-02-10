using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aibe.Models.Core {
  public class ColoringInfo : BaseInfo { //it is not derived from CommonBaseInfo
    public string ColumnName { get; private set; } //something like "TeamAssigned"
    public string ConditionColumnName { get; private set; }
    public string ComparatorCode { get; private set; }
    public ComparisonExpressionInfo CompExp { get; private set; }
    public string Color { get; private set; }
    public bool ConditionColumnIsDifferentColumn { get { return ConditionColumnName != ColumnName; } }

    public ColoringInfo(string desc) : base(desc) {
      string descContent = desc.GetNonEmptyTrimmedInBetween("[", "]");
      if (string.IsNullOrWhiteSpace(descContent))
        return;
      var descParts = descContent.GetTrimmedNonEmptyParts('|');
      if (descParts == null || descParts.Count != 4) //it must consists of exactly 4 parts
        return;
      CompExp = new ComparisonExpressionInfo(descParts[2]);
      if (!CompExp.IsValid) //the comparison expression must be valid
        return;
      if (!DH.ValidComparatorSigns.Any(x => x.Equals(descParts[1]))) //the comparison code must also be valid
        return;
      SimpleExpression colExp = new SimpleExpression(descParts[0], ",", false);
      ColumnName = colExp.LeftSide;
      ConditionColumnName = colExp.IsSingular ? colExp.LeftSide : colExp.RightSide;
      ComparatorCode = descParts[1];
      Color = descParts[3];
      IsValid = true;
    }

    public bool CheckConditionMet(string dataType, object val, int id) {
      try {
        bool isNumber = DH.NumberDataTypes.Contains(dataType);
        bool isDateTime = dataType.EqualsIgnoreCase(DH.DateTimeDataType);
        if (!isNumber && !isDateTime && !DH.ValidComparatorSignsForOthers.Any(x => x.EqualsIgnoreCaseTrim(ComparatorCode))) //other comparator but number and date can only use equal or not equal to
          return false;
        if (!DH.ValidComparatorSigns.Any(x => x.EqualsIgnoreCaseTrim(ComparatorCode)))
          return false;
        object value = null; //the one that is going to be used to store actual value

        //The easiest to compare, just check with original expression
        if (DH.EqualNotEqualOnlyDataTypes.Contains(dataType))
          if (ComparatorCode == "E")
            return dataType.EqualsIgnoreCaseTrim(DH.BooleanDataType) ?
              CompExp.OriginalDesc.EqualsIgnoreCaseTrim(val.ToString()) :
              CompExp.OriginalDesc.Equals(val.ToString());
        if (ComparatorCode == "NE")
          return dataType.EqualsIgnoreCaseTrim(DH.BooleanDataType) ?
            !CompExp.OriginalDesc.EqualsIgnoreCaseTrim(val.ToString()) :
            !CompExp.OriginalDesc.Equals(val.ToString());

        int addVal = CompExp.GetTrueShiftValue(); //TODO though the ShiftValue is double, here the addVal is integer... is it OK?

        //Check value or directives
        bool parseResult = false;
        StringBuilder queryScript;

        if (isNumber) {
          if (CompExp.ExpType == ComparisonExpressionInfo.ExpressionType.TableValueReference ||
             CompExp.ExpType == ComparisonExpressionInfo.ExpressionType.Aggregation) { //only applied for TableValued or aggregation                                                                                                    
            string aggregation = string.IsNullOrWhiteSpace(CompExp.AggregateName) ? string.Empty : CompExp.AggregateName + "(";
            string endAggregationIfAny = string.IsNullOrWhiteSpace(CompExp.AggregateName) ? string.Empty : ")";
            queryScript = new StringBuilder(string.Concat( //Script initialization
              "SELECT ", aggregation, "[", CompExp.RefInfo.RefTableColumn,
              "]", endAggregationIfAny, " FROM [", CompExp.RefInfo.RefTableName, "]")); //TODO this is quite dangerous since it is not parameterized
            if (CompExp.RefInfo.HasLastExpression) { //give where clause, use addVal
              queryScript.Append(string.Concat(" WHERE [", DH.Cid, "] = ", CompExp.RefInfo.IsSelf ? id.ToString() : CompExp.RefInfo.Cid.ToString()));
            } else
              addVal = 0; //otherwise, nullify the value
            try {
              value = SQLServerHandler.ExecuteScriptExtractDecimalWithAddition(DH.DataDBConnectionString, queryScript.ToString(), addVal);
            } catch {
              return false;
            }
          } else { //normal case, just take the number directly...
            decimal decVal;
            parseResult = decimal.TryParse(CompExp.OriginalDesc, out decVal);
            if (!parseResult)
              return false;
            value = decVal;
          }
        } else if (isDateTime) {
          DateTime dtVal = new DateTime();
          if (CompExp.IsNow) {
            dtVal = DateTime.Now;
            if (CompExp.Operator == "+")
              dtVal.AddSeconds(CompExp.ShiftValue);
            else if (CompExp.Operator == "-")
              dtVal.AddSeconds(-CompExp.ShiftValue);
          } else if (CompExp.ExpType == ComparisonExpressionInfo.ExpressionType.TableValueReference) { //If it is table referencing
            queryScript = new StringBuilder(string.Concat("SELECT [", CompExp.RefInfo.RefTableColumn, "] FROM [", CompExp.RefInfo.RefTableName, "] WHERE [", DH.Cid, "] = ",
              CompExp.RefInfo.IsSelf ? id.ToString() : CompExp.RefInfo.Cid.ToString())); //TODO this is quite dangerous since it is not parameterized
            try {
              dtVal = (DateTime)SQLServerHandler.ExecuteScriptExtractDateTimeWithAddition(DH.DataDBConnectionString, queryScript.ToString(), addVal);
            } catch {
              return false;
            }
          } else { //Us the DateTime value directly
            parseResult = DateTime.TryParseExact(CompExp.OriginalDesc, ComparisonExpressionInfo.DateTimeFormat,
              null, System.Globalization.DateTimeStyles.AssumeLocal, out dtVal);
            if (!parseResult)
              return false;
          }
          value = dtVal;
        } else if (dataType.EqualsIgnoreCase(DH.BooleanDataType)) {
          bool boolVal;
          parseResult = bool.TryParse(CompExp.OriginalDesc, out boolVal);
          if (!parseResult)
            return false;
          value = boolVal;
        } else { //String or char
          value = CompExp.OriginalDesc; //no need to check further for now
        }

        //process the data other than direct comparison here...
        if (isNumber) {
          decimal decVal;
          parseResult = decimal.TryParse(val.ToString(), out decVal);
          if (!parseResult)
            return false;
          decimal thisVal = (decimal)value;
          switch (ComparatorCode) {
            case "GE": return decVal >= thisVal;
            case "G": return decVal > thisVal;
            case "E": return decVal == thisVal;
            case "L": return decVal < thisVal;
            case "LE": return decVal <= thisVal;
            case "NE": return decVal != thisVal;
            default: return false;
          }
        }

        if (isDateTime) {
          DateTime dtVal;
          parseResult = DateTime.TryParse(val.ToString(), out dtVal);
          if (!parseResult)
            return false;
          DateTime thisDtVal = (DateTime)value;
          switch (ComparatorCode) {
            case "GE": return dtVal >= thisDtVal;
            case "G": return dtVal > thisDtVal;
            case "E": return dtVal == thisDtVal;
            case "L": return dtVal < thisDtVal;
            case "LE": return dtVal <= thisDtVal;
            case "NE": return dtVal != thisDtVal;
            default: return false;
          }
        }

        return false;
      } catch {
        return false;
      }
    }

    public class ComparisonExpressionInfo : BaseInfo {
      //valid now keyword
      private static string now = DH.Now;
      //valid date time format
      public readonly static string DateTimeFormat = "M/d/yyyy HH:mm:ss";
      //valid comparators
      private static List<string> operators = new List<string> { "+", "-" };
      public string AggregateName { get; private set; }
      public ColoringTableValueRefInfo RefInfo { get; private set; }
      public bool IsNow { get; private set; }
      public DateTime? DateTimeValue { get; private set; }
      public string Operator { get; private set; } //If it is null, means there isn't operator
      public double NumberWithOperator { get; private set; }
      public ExpressionType ExpType { get; private set; } = ExpressionType.Raw;
      public double ShiftValue { get; private set; }
      public ComparisonExpressionInfo(string desc) : base(desc) {
        if (string.IsNullOrWhiteSpace(desc))
          return;
        //Check if it starts with aggregation.
        bool isPossiblyAggregate = DH.AggregateNames.Any(x => desc.ToUpper().StartsWith(x));
        if (isPossiblyAggregate) {
          string checkedAggregateValue = DH.AggregateNames.FirstOrDefault(x => desc.ToUpper().StartsWith(x));
          if (desc.Length <= checkedAggregateValue.Length) //no definition, not possible to be correct aggregate
            return;
          string descContent = desc.Substring(checkedAggregateValue.Length).GetNonEmptyTrimmedInBetween("(", ")");
          if (string.IsNullOrWhiteSpace(descContent)) //cannot be empty
            return;
          ColoringTableValueRefInfo testRefInfo = new ColoringTableValueRefInfo(descContent);
          if (testRefInfo.IsValid) {
            ExpType = ExpressionType.Aggregation;
            AggregateName = checkedAggregateValue;
            RefInfo = testRefInfo;
            IsValid = true;
          }
        } else { //it must not be aggregate, then it can be anything..., but the most important type is if NOW or having datetime value, or others...

          //The very first test must be if it is a table reference value
          ColoringTableValueRefInfo testRefInfo = new ColoringTableValueRefInfo(desc);
          if (testRefInfo.IsValid) {
            ExpType = ExpressionType.TableValueReference;
            RefInfo = testRefInfo;
            IsValid = true;
            return; //go no further, terminates here
          }

          SimpleExpression exp = new SimpleExpression(desc, operators, false);
          if (!exp.IsValid) //invalid expression, just return
            return;

          //Second test is now, followed by DateTime, then the rests...
          IsNow = exp.LeftSide.EqualsIgnoreCase(now); //test if it is now
          if (IsNow) {
            ExpType = ExpressionType.Now;
          } else { //test date time
            DateTime testDateTime;
            bool result = DateTime.TryParseExact(exp.LeftSide, DateTimeFormat, null, System.Globalization.DateTimeStyles.AssumeLocal, out testDateTime);
            if (result) {
              DateTimeValue = testDateTime;
              ExpType = ExpressionType.DateTime;
            }
          }

          if (exp.IsSingular) //if it is singular expression, it is always valid here
            IsValid = true;
          else { //if it is non-singular expression, the right hand must be correct number if it is not raw, or it is completely raw
            //If it is raw and has right hand side, then it must either:
            // 1. the left hand side is number and the right hand side too, or
            // 2. it is truly raw
            double testNumberRight;
            bool resultRight = double.TryParse(exp.RightSide, out testNumberRight);
            if (ExpType == ExpressionType.Raw) {
              double testNumberLeft;
              bool resultLeft = double.TryParse(exp.LeftSide, out testNumberLeft); //test the left side, see if it is number
              if (resultLeft && resultRight && testNumberLeft > 0 && testNumberRight > 0) { //the number must be greater than zero
                ExpType = ExpressionType.NumberWithOperator;
                NumberWithOperator = testNumberLeft;
                ShiftValue = testNumberRight;
                Operator = exp.MiddleSign; //the operator is counted here, since it is in number
              } //it is not a number but have operator, the operator is not counted here, and the type remains raw
              IsValid = true; //in whichever case, the non singular expression with raw is always valid (either as raw or as number with operator)
            } else { //it is non singular and it is not raw (now or datetime), then the right hand-side must be a valid positive number, else it is invalid
              IsValid = resultRight && testNumberRight > 0; //other than these, the non singular expression will be invalid
            }
          }
        }
      }

      public int GetTrueShiftValue() { //check the value before make it integer is its additional features
        int value = 0;
        if(ExpType == ExpressionType.TableValueReference ||
          ExpType == ExpressionType.Aggregation) {
          if (RefInfo != null && RefInfo.HasLastExpression) {
            if (RefInfo.Operator == "+") {
              value = RefInfo.ShiftValue;
            } else if (RefInfo.Operator == "-") {
              value = RefInfo.ShiftValue;
              value *= -1;
            }
          }
        }
        return value;
      }

      public enum ExpressionType {
        Raw, //either string or number
        Now,
        DateTime,
        TableValueReference,
        NumberWithOperator, //can only be true if it has operator
        Aggregation, //must also be table value reference by definition
      }
    }
  }
}