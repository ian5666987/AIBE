using Extension.Database.SqlServer;
using Extension.String;
using Extension.Models;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace Aibe.Models.Core {
  public class TableValueRefInfo : BaseInfo {
    private static List<string> defaultComparators = new List<string> { "=" }; //as of now, only accepts the "=" comparator for taking reference from other tables
    private static List<string> nonAllowedStrings = new List<string> { ",", ";", ":", "--" }; //by right all but double dashes "--" should have been handled outside, but just in case
    private static string skip = DH.Skip; //keyword to skip checking the RefAnotherColumnName=ThisOtherColumnName part
    public string RefTableName { get; private set; } 
    public string Column { get; private set; }
    public string CondColumn { get; private set; }
    public string CondComparator { get; private set; } //is sign, like =, now the only one accepted
    public string CrossTableCondColumn { get; private set; }
    public string StaticCrossTableCondColumn { get; private set; }
    public bool CrossTableColumnIsStatic { get { return !string.IsNullOrWhiteSpace(StaticCrossTableCondColumn); } }
    public string AdditionalWhereClause { get; private set; }
    public bool CrossTableCheckIsSkipped { get; private set; }
    public bool IsSelectUnconditional { get; private set; } = true; //assume true till proven otherwise
    //desc = RefTableName:Column:CondColumn {CondComparator} CrossTableCondColumn OR StaticCrossTableCondColumn
    public TableValueRefInfo(string desc) : base (desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      var parts = desc.GetTrimmedParts(':');
      if (parts.Count < 2) //minimum contains of two parts
        return;
      RefTableName = parts[0];
      //int sharpIndex = parts[1].IndexOf('#');
      //if (sharpIndex > 0 && parts[1].Length > sharpIndex + 1) {
      //  string columnCandidate = parts[1].Substring(0, sharpIndex).Trim();
      //  string valueColumnCandidate = parts[1].Substring(sharpIndex + 1).Trim();
      //  if (string.IsNullOrWhiteSpace(columnCandidate) || string.IsNullOrWhiteSpace(valueColumnCandidate))
      //    return; //invalid
      //  Column = columnCandidate;
      //  ValueColumn = valueColumnCandidate;
      //} else
      Column = parts[1]; //Take the column directly
      IsValid = true;
      if (parts.Count < 3)
        return;

      if (parts[2].EqualsIgnoreCaseTrim(skip))
        CrossTableCheckIsSkipped = true;
      else { //Only if cross table check is not skipped then we could check for the table reference validity, else, go directly to AdditionalWhereClause
        SimpleExpression exp = new SimpleExpression(parts[2], defaultComparators, false); //TODO as of now, only split by equality. Subsequently could be different.
        if (!exp.IsValid || exp.IsSingular) { //if it contains false expression, it cannot be singular too.
          IsValid = false; //revoke the validity
          return;
        }

        CondColumn = exp.LeftSide;
        CondComparator = exp.MiddleSign;
        string testRightSide = exp.RightSide.GetNonEmptyTrimmedInBetween("\"", "\"");
        if (!string.IsNullOrWhiteSpace(testRightSide)) //check if it is static
          StaticCrossTableCondColumn = testRightSide;
        else //then it must be dynamic
          CrossTableCondColumn = exp.RightSide;
      }
      IsSelectUnconditional = false; //this means that all things are taken

      if (parts.Count != 4) //parts cannot be more than 4
        return;

      //At this point, there is additional where clause, but it will not change the validity if wrong. It will simply be unused.
      if (nonAllowedStrings.Any(x => parts[3].IndexOf(x) != -1))
        return; //cannot continue if any of such things exist

      AdditionalWhereClause = parts[3];
    }

    public List<string> GetStaticDropDownItems() {
      List<string> items = new List<string>();
      //Script initialization
      StringBuilder selectScript = new StringBuilder(string.Concat(
        "SELECT DISTINCT [", Column, "] FROM [", RefTableName, "] WHERE [",
          Column, "] IS NOT NULL"));
      SqlParameter par = null;
      if (!IsSelectUnconditional && CrossTableColumnIsStatic) { //if the select is conditional, adds the condition static condition
        string addQueryString = //if the select is conditional, only static is allowed here.
          string.Concat(" AND [", CondColumn, "]", CondComparator, "@par0");
        par = new SqlParameter("@par0", StaticCrossTableCondColumn);
        selectScript.Append(addQueryString);
      }
      if (!string.IsNullOrWhiteSpace(AdditionalWhereClause)) {
        selectScript.Append(" AND (");
        selectScript.Append(AdditionalWhereClause);
        selectScript.Append(")");
      }
      DataTable dataTable = par == null ? SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString()) :
        SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString(), par);
      if (dataTable == null)
        return items;
      foreach (DataRow row in dataTable.Rows)
        items.Add(row.ItemArray[0].ToString());
      return items;
    }
  }
}