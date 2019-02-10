using System.Collections.Generic;
using System.Linq;
using Aibe.Models;
using Aibe.Models.Core;
using System.Data.SqlClient;
using System.Text;
using System.Data;
using System.Text.RegularExpressions;
using Extension.Database.SqlServer;
using Extension.String;
using Extension.Checker;

namespace Aibe.Helpers {
  public class DropDownHelper {
    static List<string> removedValuedChangedStrings = new List<string> { "@@", "[", "]" };
    public static Regex VarToValRegex = new Regex(@"(@@|\[@@)\w+(\s|$|\)|\])", RegexOptions.Multiline);
    public static string GetCheckedMatch(object match) {
      string checkedMatch = match.ToString().Trim().ToLower();
      foreach (var str in removedValuedChangedStrings)
        checkedMatch = checkedMatch.Replace(str, string.Empty);
      return checkedMatch;
    }

    public static List<string> GetDropDownStringsFor(
      DropDownInfo dropdown,
      string originalValue, string dataType = DH.DropDownNumberDataType,
      bool filterApplied = false, 
      Dictionary<string, DropdownPassedArguments> passedColumnsAndValues = null) {
      List<string> dropdownItems = new List<string>();

      //Enumerate here, sharp (#) symbol is ignored
      foreach (var dropdownItem in dropdown.Items) {
        if (dropdownItem.IsItem) { //If it is item, just put it immediately
          if (!string.IsNullOrWhiteSpace(dropdownItem.Item))
            dropdownItems.Add(dropdownItem.Item); //the same display and value
          continue;
        }

        if (dropdownItem.RefInfo == null) //if it is not item, and not having table value reference, just skip it
          continue;

        //The dropdown parts will be only in terms of a single column name in the table that is referred to already
        TableValueRefInfo rInfo = dropdownItem.RefInfo;
        DropdownPassedArguments passedValue = null;
        if (filterApplied && passedColumnsAndValues != null &&
          !string.IsNullOrWhiteSpace(rInfo.CrossTableCondColumn) &&
          !string.IsNullOrWhiteSpace(rInfo.CondColumn) &&
          passedColumnsAndValues.ContainsKey(rInfo.CrossTableCondColumn)) { //if the passed values contain the tableColumnName, then gets it
          passedValue = passedColumnsAndValues[rInfo.CrossTableCondColumn];
        }

        //Enumerate from the given table and column
        try {
          //Script initialization
          StringBuilder selectScript = new StringBuilder(string.Concat(
            "SELECT DISTINCT [", rInfo.Column, "] FROM [", rInfo.RefTableName, "] WHERE [",
            rInfo.Column, "] IS NOT NULL"));

          List<SqlParameter> pars = new List<SqlParameter>();
          if (passedValue != null &&
            !string.IsNullOrWhiteSpace(passedValue.ToString())) {//other reference has to be made
            SqlParameter par2 = new SqlParameter("@par2", passedValue.DataType.EqualsIgnoreCase(DH.StringDataType) &&
              passedValue.Value != null ? passedValue.Value.ToString() : passedValue.Value);
            pars.Add(par2);
            selectScript.Append(string.Concat(" AND [", rInfo.CondColumn, "] ", rInfo.CondComparator, " @par2"));
          }

          if (!string.IsNullOrWhiteSpace(rInfo.AdditionalWhereClause)) {
            var matches = VarToValRegex.Matches(rInfo.AdditionalWhereClause);
            bool isApplied = true; //assume applied till proven otherwise
            string appliedScript = rInfo.AdditionalWhereClause; //to change whenever necessary
            if (passedColumnsAndValues != null) { //only if there is something passed in the first place
              foreach (var match in matches) {
                string checkedMatch = GetCheckedMatch(match);
                DropdownPassedArguments passedItem = passedColumnsAndValues
                  .Where(x => x.Key.EqualsIgnoreCase(checkedMatch))
                  .Select(x => x.Value)
                  .FirstOrDefault();
                if (passedItem == null || string.IsNullOrWhiteSpace(passedItem.Value.ToString())) { //failed case
                  isApplied = false;
                  break;
                }

                string checkString = passedItem.Value.ToString();
                if (DB.ContainsDangerousElement(checkString, passedItem.DataType)) {
                  isApplied = false;
                  LogHelper.Error(LCZ.W_Auto, LCZ.W_SQLInjection, LCZ.W_Auto, LCZ.W_Auto, LCZ.W_Auto, LCZ.W_DropDownUpdate,
                    rInfo.AdditionalWhereClause, string.Format(LCZ.E_SQLInjection, match.ToString(), passedItem.DataType, checkString));
                  break; //WARNING! SQL injection detected!
                }

                //then change the applied item
                appliedScript = appliedScript.Replace(match.ToString(), passedItem.GetFilterStringValue()); //TODO this may be bad since the data type is unkown
              }

              if (isApplied) {
                selectScript.Append(" AND (");
                selectScript.Append(appliedScript);
                selectScript.Append(")");
              }
            } else {
              if (matches.Count <= 0) { //Only apply the script if there is no parameters
                selectScript.Append(" AND (");
                selectScript.Append(appliedScript);
                selectScript.Append(")");
              }
            }
          }

          DataTable dataTable = SQLServerHandler.GetDataTable(DH.DataDBConnectionString, selectScript.ToString(), pars);
          if (dataTable == null)
            return null;

          foreach (DataRow row in dataTable.Rows)
            dropdownItems.Add(row.ItemArray[0].ToString());
        } catch { //TODO put something if necessary, just leave it like this for now
          return null;
        }
      }

      if (dropdownItems != null && !string.IsNullOrWhiteSpace(originalValue) && !dropdownItems.Contains(originalValue))
        dropdownItems.Insert(0, originalValue);

      if (dropdownItems == null || dropdownItems.Count <= 0)
        return null;

      if (!string.IsNullOrWhiteSpace(dropdown.OrderByDirective)) { //If there is order-by directive
        if (dropdown.OrderByDirective.EqualsIgnoreCase(DH.DescOrderWord))
          if (dataType != null && dataType.EqualsIgnoreCase(DH.DropDownNumberDataType)) {
            dropdownItems = dropdownItems.OrderByDescending(x => double.Parse(x)).ToList();
          } else
            dropdownItems = dropdownItems.OrderByDescending(x => x).ToList();
        else if (dropdown.OrderByDirective.EqualsIgnoreCase(DH.AscOrderWord))
          if (dataType != null && dataType.EqualsIgnoreCase(DH.DropDownNumberDataType)) {
            dropdownItems = dropdownItems.OrderBy(x => double.Parse(x)).ToList();
          } else
            dropdownItems = dropdownItems.OrderBy(x => x).ToList();
      }

      return dropdownItems;
    }
  }
}