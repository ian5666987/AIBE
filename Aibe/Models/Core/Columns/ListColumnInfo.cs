using Extension.Database.SqlServer;
using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aibe.Models.Core {
  //ColumnName1|HeaderName11,HeaderName12,…,HeaderName1N=ListType|RefTableName:RefColumnName:RefAnotherColumnName=ThisOtherColumnName:AddWhereAndClause;
  public class ListColumnInfo : CommonBaseInfo {
    #region legacy
    private static List<string> defaultListTypes = new List<string> { "default", "check", "list", "remarks", "dropdown" };
    private static Dictionary<string, string> legacyToListTypeDictionary = new Dictionary<string, string> {
      { "default", "LVO" }, { "check", "LC" }, { "list", "L" }, { "remarks", "LVV" }, { "dropdown", "LOV" }
    };
    #endregion

    private static List<char> listTypeLetters = new List<char> { 'L', 'V', 'O', 'C' };
    public string DefaultHeaderPrefix;
    public new string Name { get; private set; } //this is hiding the original name, but make use of it
    public List<string> HeaderNames { get; private set; }
    public string ListType { get; private set; } = "LVO";
    public static int DefaultWidth { get; private set; } = 20;
    public List<int> Widths { get; private set; }
    public TableValueRefInfo TemplateRef { get; private set; }
    public ListColumnInfo(string desc) : base(desc) {
      if (!IsValid)
        return;

      DefaultHeaderPrefix = LCZ.W_Header; //initialized everytime now due to the localization
      HeaderNames = new List<string> { LCZ.W_Name, LCZ.W_Value, LCZ.W_Ending };
      Widths = new List<int> { DefaultWidth, DefaultWidth, DefaultWidth }; //same number as HeaderNames

      List<string> baseNameParts = base.Name.GetTrimmedNonEmptyParts('|');
      if(baseNameParts.Count <= 0) { //it cannot be without name
        IsValid = false;
        return;
      }
      Name = baseNameParts[0]; //The name exists
      if (baseNameParts.Count >= 2) { //have header names and width
        var headerWidthParts = baseNameParts[1].GetTrimmedNonEmptyParts(',');
        HeaderNames = new List<string>();
        Widths = new List<int>();
        foreach(var headerWidthPart in headerWidthParts) {
          var parts = headerWidthPart.GetTrimmedNonEmptyParts('#');
          if (parts == null || parts.Count <= 0)
            continue;
          HeaderNames.Add(parts[0]);
          if(parts.Count > 1) {
            int width;
            bool result = int.TryParse(parts[1], out width);
            Widths.Add(result && width > 0 ? width : DefaultWidth);
          } else
            Widths.Add(DefaultWidth);
        }
      }

      if (!HasRightSide) //it is ok not to have the right side for ListColumnInfo
        return;

      //Actually, where clause also cannot have "|"
      var rightParts = RightSide.GetTrimmedNonEmptyParts('|');

      if (rightParts.Count < 1)
        return;

      if (defaultListTypes.Any(x => x.EqualsIgnoreCaseTrim(rightParts[0]))) { //If it is a valid legacy type        
        string legacyListType = defaultListTypes.FirstOrDefault(x => x.EqualsIgnoreCaseTrim(rightParts[0]));
        if (legacyToListTypeDictionary.ContainsKey(legacyListType))
          ListType = legacyToListTypeDictionary[legacyListType]; //convert the legacy type to the new type
      } else if (rightParts[0].ToUpper().All(x => listTypeLetters.Contains(x))) { //check if it is a valid list type, all letters must be checked (they must all be allowed letters)
        ListType = rightParts[0].ToUpper(); //apply the list type here
      }

      if (rightParts.Count < 2)
        return;      

      TemplateRef = new TableValueRefInfo(rightParts[1]);
      if (!TemplateRef.IsValid) { //if it has reference, it must be valid. Otherwise revoke the validity
        IsValid = false;
        return;
      }
    }

    private int headerIndex = -1;

    public void ResetHeaderCount() {
      headerIndex = -1;
    }

    public string GetNextHeader() {
      ++headerIndex;
      return HeaderNames != null && HeaderNames.Count > headerIndex ?
        HeaderNames[headerIndex] : DefaultHeaderPrefix + " " + (headerIndex + 1).ToString();
    }

    public bool GetRefDataValue(string changedColumnName, string changedColumnValue, out string dataValue) {
      dataValue = string.Empty;
      if (TemplateRef == null ||
        string.IsNullOrWhiteSpace(TemplateRef.RefTableName) ||
        string.IsNullOrWhiteSpace(TemplateRef.Column) ||
        string.IsNullOrWhiteSpace(TemplateRef.CondColumn) ||
        (string.IsNullOrWhiteSpace(TemplateRef.StaticCrossTableCondColumn) &&  //if static cross-cond-column is empty
           (string.IsNullOrWhiteSpace(TemplateRef.CrossTableCondColumn) || //the dynamic cannot be empty
            string.IsNullOrWhiteSpace(changedColumnValue) || //the changed column value cannot be empty 
            string.IsNullOrWhiteSpace(changedColumnName) || //the changed column name cannot be empty
            !TemplateRef.CrossTableCondColumn.EqualsIgnoreCase(changedColumnName)))) //or if the changedColumnName is not equal to CrossTableCondColumn
        return false;

      try {
        //Script making.
        StringBuilder selectScript = new StringBuilder(string.Concat(
          "SELECT DISTINCT [", TemplateRef.Column, "] FROM [", TemplateRef.RefTableName, "] WHERE [",
          TemplateRef.Column, "] IS NOT ", DH.NULL, " AND [", TemplateRef.CondColumn, "] = @par")
         );

        if (!string.IsNullOrWhiteSpace(TemplateRef.AdditionalWhereClause)) {
          selectScript.Append(" AND (");
          selectScript.Append(TemplateRef.AdditionalWhereClause);
          selectScript.Append(")");
        }

        string appliedValue = string.IsNullOrWhiteSpace(TemplateRef.CrossTableCondColumn) &&
          !string.IsNullOrWhiteSpace(TemplateRef.StaticCrossTableCondColumn) ?
          TemplateRef.StaticCrossTableCondColumn : changedColumnValue;
        SqlParameter par = new SqlParameter("@par", appliedValue);
        DataTable dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString(), par);

        if (dataTable == null || dataTable.Rows == null || dataTable.Rows.Count <= 0 ||
          dataTable.Rows[0].ItemArray == null || dataTable.Rows[0].ItemArray.Length <= 0 ||
          dataTable.Rows[0].ItemArray[0] == null)
          return false;

        dataValue = dataTable.Rows[0].ItemArray[0].ToString();
        return true;
      } catch {
        return false;
      }
    }
  }
}
