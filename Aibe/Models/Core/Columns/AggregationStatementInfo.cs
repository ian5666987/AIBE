using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class AggregationStatementInfo : CommonBaseInfo {
    public List<KeyValuePair<string, GroupByColumnInfo>> GroupByColumns { get; private set; } = new List<KeyValuePair<string, GroupByColumnInfo>>();
    public string AggregationQueryScript { get; private set; }
    public string IdentifierAggregationQueryScript { get; private set; } //v1.4.1.0 necessary so that the group by columns are all used just to make query script
    public AggregationStatementInfo(string desc) : base(desc) {
      if (!HasRightSide) { //must have the right side to be valid
        IsValid = false;
        return;
      }

      //The "Name" here is in the form of GroupByColumn31:AutoDirective31,GroupByColumn32:AutoDirective32,... thus use further divider
      var parts = Name.GetTrimmedNonEmptyParts(',');
      if (parts == null || parts.Count <= 0 || string.IsNullOrWhiteSpace(parts[0])) { //at least the first part must have something
        IsValid = false;
        return;
      }

      //List of GroupByColumns
      for (int i = 0; i < parts.Count; ++i) {
        SimpleExpression exp = new SimpleExpression(parts[i], ":", false);
        if (!exp.IsValid) //do not process invalid expression
          continue;
        if (exp.IsSingular) {
          GroupByColumns.Add(new KeyValuePair<string, GroupByColumnInfo>(exp.LeftSide, new GroupByColumnInfo(exp.LeftSide, null)));
          continue;
        }
        if (!DH.GroupByAutoDirectives.Any(x => exp.RightSide.EqualsIgnoreCase(x))) { //not among the listed directive, not allowed
          GroupByColumns.Add(new KeyValuePair<string, GroupByColumnInfo>(exp.LeftSide, new GroupByColumnInfo(exp.LeftSide, null))); //so the exp.RightSide is NOT included here
          continue;
        }
        GroupByColumns.Add(new KeyValuePair<string, GroupByColumnInfo>(exp.LeftSide, new GroupByColumnInfo(exp.LeftSide, exp.RightSide)));
      }

      //If there is no column found, then put IsValid as false
      if (GroupByColumns.Count <= 0) {
        IsValid = false;
        return;
      }

      AggregationQueryScript = RightSide;
      IdentifierAggregationQueryScript = string.Join(",", GroupByColumns.Select(x => x.Value.ColumnName)); //v1.4.1.0 this is to get the base way for aggregation columns
    }
  }
}
