using Extension.String;

namespace Aibe.Models {
  //Useful for creating each column info in the FilterIndexInfo
  public class GroupByColumnInfo {
    public string ColumnName { get; private set; }
    public string AutoDirective { get; private set; }
    public bool IsAutoInt { get { return AutoDirective != null && AutoDirective.EqualsIgnoreCase(DH.GroupByAutoDirectiveInt); } }
    public bool IsAutoDateTime { get { return AutoDirective != null && AutoDirective.EqualsIgnoreCase(DH.GroupByAutoDirectiveDateTime); } }
    public GroupByColumnInfo(string columnName, string autoDirective) {
      ColumnName = columnName;
      AutoDirective = autoDirective;
    }
  }
}