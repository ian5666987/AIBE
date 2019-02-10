using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace Aibe.Models.Core {
  public class ScriptConstructorColumnInfo : CommonBaseInfo {
    public string TableSource { get; private set; } //Table source of origin, where this Column is located
    public string RefTableName { get; private set; } //Reference table name, where this Column gets its data from
    public string ScriptConstructor { get; private set; }
    public List<DataColumn> DataColumns { get; private set; }
    public List<string> PictureLinks { get; private set; } = new List<string>();
    public List<int> PictureWidths { get; private set; } = new List<int>(); //TODO as of now, the way to do this is not so good, but leave if for now... it should be just use the PictureColumnInfo
    public ScriptConstructorColumnInfo(string tableSource, string desc) : base(desc) {
      if (!HasRightSide || string.IsNullOrWhiteSpace(tableSource)) {
        IsValid = false;
        return;
      }

      //The "Name" here is in the form of Name|attrName:item1,item2,...,itemN, thus use further divider
      var parts = Name.GetTrimmedNonEmptyParts('|');
      Name = parts[0]; //the actual "Name" is only the first part
      if (string.IsNullOrWhiteSpace(Name)) {
        IsValid = false;
        return;
      }

      if (parts.Count > 1) //has some other parts
        for (int i = 1; i < parts.Count; ++i) {
          SimpleExpression exp = new SimpleExpression(parts[i], ":", false);
          if (exp.IsSingular || !exp.IsValid) //do not process singular or invalid expression
            continue;
          if (!DH.ScAttributes.Any(x => exp.LeftSide.EqualsIgnoreCase(x))) //not among the listed attribute, not allowed
            continue;
          if (exp.LeftSide.EqualsIgnoreCase(DH.ScPictureLinksAttribute)) { //handles correct attributes
            var subparts = exp.RightSide.GetTrimmedNonEmptyParts(','); //ColumnName1#Width1, ColumnName2#Width2, ..., ColumnNameN#WidthN
            foreach (var subpart in subparts) { //each subpart is in the form of ColumnName#Width like Photo#100
              SimpleExpression subExp = new SimpleExpression(subpart, "#", false);
              if (!subExp.IsValid)
                continue;
              PictureLinks.Add(subExp.LeftSide);
              int width = PictureColumnInfo.DefaultWidth;
              bool result = false;
              if (!subExp.IsSingular)
                result = int.TryParse(subExp.RightSide, out width);
              PictureWidths.Add(result ? width : PictureColumnInfo.DefaultWidth);
            }
          } else if (exp.LeftSide.EqualsIgnoreCase(DH.ScRefTableNameAttribute)) {
            RefTableName = exp.RightSide; //RightSide is like MyRefTableName
          } //add as many "else if" as necessary
        }      

      try {
        DataColumns = SQLServerHandler.GetColumns(DH.DataDBConnectionString, tableSource); //get the DataColumns of the table first
      } catch {
        IsValid = false;
        return;
      }

      //For now, if the DataColumn does not contain Cid, it would not be allowed to proceed, this is because picture image needs this Cid
      if (DataColumns == null || DataColumns.Count <= 0 || !DataColumns.Any(x => x.ColumnName.EqualsIgnoreCase(DH.Cid))) {
        IsValid = false;
        return;
      }

      //TODO check script constructor validity here
      ScriptConstructor = RightSide;
      TableSource = tableSource;
    }
  }
}