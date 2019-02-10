using Aibe.Customs;
using Aibe.Models;
using Aibe.Models.Core;
using Extension.Checker;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;
using System.ComponentModel.DataAnnotations;

namespace Aibe.Helpers {
  public class CheckerHelper {
    public static Dictionary<string, List<string>> AllowedTableTags = new Dictionary<string, List<string>>(); //may need to define this in the beginning
    public Dictionary<string, string> CheckModelValidity(
      string tableModelClassPrefix,
      string tableSource,
      List<DataColumn> columns,
      Dictionary<string, string> collections,
      List<string> modelKeys,
      MetaInfo meta,
      List<string> checkExclusions,
      bool userIsDeveloper,
      DateTime refDtNow,
      string actionType,
      bool strongCheck = true, //assuming strongCheck unless stated otherwise
      bool isTagChecked = false, //if tag is to be checked, then checks the tag
      bool filterStyle = false) {
      Dictionary<string, string> errorDict = new Dictionary<string, string>();

      foreach (var key in modelKeys) {
        if (checkExclusions.Contains(key))
          continue; //no need to check if explicitly excluded

        bool isExplicitlyRequired = false;
        if (!filterStyle) //Only on creation and edit
          isExplicitlyRequired = meta.RequiredColumns != null &&
            meta.RequiredColumns.Any(x => !string.IsNullOrWhiteSpace(x) && x.EqualsIgnoreCase(key));

        //Now, get the value
        string val = collections[key];
        KeyInfo keyInfo = new KeyInfo(tableSource, key);
        DataColumn column = columns.Where(x => x.ColumnName.EqualsIgnoreCase(keyInfo.PureKeyName)).FirstOrDefault();

        //If not filter style, then update the data type from the column given
        //filter style would already have correct data type
        if (!filterStyle)
          keyInfo.DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);

        //If column names is not found, assumes it is an unknown injection
        string displayName = meta.GetColumnDisplayName(keyInfo.PureKeyName);
        string columnName = keyInfo.PureKeyName.ToCamelBrokenString();
        string usedColumnName = displayName == columnName ? columnName : displayName + "/" + columnName;
        if (column == null) {
          errorDict.Add(LCZ.I_UnknownColumn, string.Format(LCZ.E_ColumnUnknown, usedColumnName));
          continue;
        }

        //If things are not required, and not excepted and is null, no need for further check
        if (string.IsNullOrWhiteSpace(val)) {
          //if (isRequired || !column.AllowDBNull) //column.AllowDBNull, again, does NOT do what it ought to!
          if (!filterStyle) { //only non-filtered style has concept of "required"
            bool isNullable = columnIsNullableByClass(tableModelClassPrefix, meta.TableSource, column.ColumnName, strongCheck); //TODO TableSource check if this is OK, this should be table source
            bool isImplicitlyRequired = columnIsRequiredByAttribute(tableModelClassPrefix, meta.TableSource, column.ColumnName, strongCheck);
            if (isExplicitlyRequired || isImplicitlyRequired || !isNullable) //if things are required but null, there must be something wrong, required cannot be null
              errorDict.Add(key, string.Format(LCZ.E_FieldIsRequired, usedColumnName));
          }
          continue; //if things are not required or is filterStyle, immediately continue
        }

        if (isTagChecked) {
          if (tableSource.EqualsIgnoreCase(DH.MetaTableName)) { //Meta table may contain tags
            if (!Text.ContainsOnlyAllowedTags(val, DH.AllowedMetaTableTags)) //can only have allowed tags
              errorDict.Add(key, string.Format(LCZ.E_ContainsPotentiallyDangerousElements, displayName ?? string.Empty, val ?? string.Empty));
          } else if (AllowedTableTags != null && AllowedTableTags.ContainsKey(key)) { //this is among the tables which are declared to be able to have tags
            List<string> allowedTags = AllowedTableTags[key];
            if (!Text.ContainsOnlyAllowedTags(val, allowedTags))
              errorDict.Add(key, string.Format(LCZ.E_ContainsPotentiallyDangerousElements, displayName ?? string.Empty, val ?? string.Empty));
          } else if (Text.ContainsTag(val)) //unacceptable value
            errorDict.Add(key, string.Format(LCZ.E_ContainsPotentiallyDangerousElements, displayName ?? string.Empty, val ?? string.Empty));
          continue;
        }

        //These must be checked AFTER the check for required/not-required, because if they are required, they cannot be skipped
        //Check if the items are implicitly excluded, filter or not, these keys don't matter to be further checked
        if (key.EndsWith(DH.CreateEditTimeAppendixName) ||
            DH.FilterTimeAppendixNames.Any(x => key.EndsWith(x)) ||
            DH.FilterDateAppendixNames.Any(x => key.EndsWith(x)) ||
            key.EndsWith(DH.CreateEditPictureLinkAppendixName) || //picture name is unchecked from here onwards
            key.EndsWith(DH.CreateEditNonPictureAttachmentAppendixName) //attachment name is unchecked from here onwards
          )
          continue; //some null/non-null items like datetime need not to be further checked

        //From this point onwards, the item is not null or empty and is ont excluded explicitly or implicitly (that is, not datetime)
        //At this point, filter style need not further check
        if (filterStyle)
          continue; //TODO this was previously put as return. Check if it is error or if it is intended

        keyInfo.UpdateTimeStampAndAutoGenerated(meta, keyInfo.PureKeyName, actionType);
        //From this point onwards, data is already to be checked
        object value = keyInfo.ExtractValueAsObject(collections, refDtNow, filterStyle);
        if (value == null) //there must be something wrong with the format, otherwise the extracting cannot be null
          errorDict.Add(keyInfo.Key, string.Format(LCZ.E_WrongFormat, usedColumnName, keyInfo.DataType, collections[keyInfo.Key]));

        //Now, check if string length is violated against the length specified
        if (keyInfo.DataType.EqualsIgnoreCase(DH.StringDataType) || keyInfo.DataType.EqualsIgnoreCase(DH.CharDataType)) {
          //Painfully gets length by reflection because DataColumn.MaxLength does NOT show desired string length's limit!!
          int maxLength = getMaximumStringLengthFor(tableModelClassPrefix, meta.TableSource, keyInfo.PureKeyName, strongCheck);
          int minLength = getMinimumStringLengthFor(tableModelClassPrefix, meta.TableSource, keyInfo.PureKeyName, strongCheck);
          string strVal = value.ToString();
          if (strVal.Length > maxLength)
            errorDict.Add(keyInfo.Key, string.Format(LCZ.E_MaxLengthViolated, value, usedColumnName, maxLength));
          if (strVal.Length < minLength)
            errorDict.Add(keyInfo.Key, string.Format(LCZ.E_MinLengthViolated, value, usedColumnName, maxLength));          

          RegexCheckedColumnInfo regexInfo = meta.GetRegexCheckedColumn(keyInfo.PureKeyName);
          if (regexInfo != null) { //regex checked items
            ListColumnInfo listColumn = meta.GetListColumnInfo(keyInfo.PureKeyName); //to determine if items are in listColumn or not
            string valueString = value.ToString();
            string[] valueParts = listColumn == null ? new string[] { valueString } : valueString.Split(';');
            if (valueParts != null)
              for (int i = 0; i < valueParts.Length; ++i) {
                Regex regex = new Regex(regexInfo.Content);
                Match match = regex.Match(valueParts[i].ToString());
                if (!match.Success) {
                  StringBuilder regexError = new StringBuilder(
                    string.Format(LCZ.E_DoesNotMatchWithPattern, valueParts[i], usedColumnName));

                  if (userIsDeveloper) {
                    regexError.Append(" [");
                    regexError.Append(regex.ToString());
                    regexError.Append("]");
                  }

                  RegexCheckedColumnExampleInfo exampleInfo = meta.GetRegexCheckedColumnExample(keyInfo.PureKeyName);
                  if (exampleInfo != null) {
                    regexError.Append(". ");
                    regexError.Append(string.Format(LCZ.E_PatternExample, exampleInfo.Content));
                  }

                  errorDict.Add(keyInfo.Key, regexError.ToString());
                }
              }
          }
        }

        //lastly, check number limits if data type is number types
        //ColumnName1=min:23.555|max:75.112;ColumnName2=max:36.991
        NumberLimitColumnInfo numberLimitInfo = meta.GetNumberLimitColumn(keyInfo.PureKeyName);
        if (DH.NumberDataTypes.Contains(keyInfo.DataType) && //if it is indeed number data types
          numberLimitInfo != null) { //And the limit keys contain pure key name
          double columnValue;
          bool result = double.TryParse(value.ToString(), out columnValue);
          if (result) {
            if (columnValue < numberLimitInfo.Min) //min is violated
              errorDict.Add(keyInfo.Key, string.Format(LCZ.E_MinLengthViolated, columnValue, usedColumnName, numberLimitInfo.Min));
            if (columnValue > numberLimitInfo.Max) //max is violated
              errorDict.Add(keyInfo.Key, string.Format(LCZ.E_MaxLengthViolated, columnValue, usedColumnName, numberLimitInfo.Max));
          }
        }
      }
      return errorDict;
    }

    public static GetTableTypeDelegate GetTableType;
    protected virtual Type getTableType(string prefix, string tableSource) {
      return GetTableType == null ? Type.GetType(string.Concat(prefix, tableSource)) : GetTableType(prefix, tableSource);
    }

    protected virtual PropertyInfo getColumnPropertyInfo(string prefix, string tableSource, string columnName) {
      Type tempClass = getTableType(prefix, tableSource);
      if (tempClass == null)
        return null;
      //object tableObject = Activator.CreateInstance(tempClass); //unused
      return tempClass.GetProperty(columnName);
    }

    //Strong check demands model (C# class) to be present, weak check doesn't
    protected virtual bool columnIsRequiredByAttribute(string prefix, string tableSource, string columnName, bool strongCheck = true) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableSource, columnName);
      if (propertyInfo == null && !strongCheck)
        return false; //assuming not required for weak check
      var attrs = propertyInfo.GetCustomAttributes(typeof(RequiredAttribute), true);
      return attrs != null && attrs.Length > 0;
    }

    //Strong check demands model (C# class) to be present, weak check doesn't
    protected virtual bool columnIsNullableByClass(string prefix, string tableSource, string columnName, bool strongCheck = true) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableSource, columnName);
      if (propertyInfo == null && !strongCheck)
        return true; //assuming nullable by weak check
      if (propertyInfo == null)
        throw new Exception("C# class or its property is missing for [" + columnName + "] column");
      string propertyType = propertyInfo.PropertyType.ToString();
      return DH.NullableIndicators.Any(x => propertyType.StartsWith(DH.SharedPrefixDataType + x));
    }

    //Strong check demands model (C# class) to be present, weak check doesn't
    protected virtual int getMaximumStringLengthFor(string prefix, string tableSource, string columnName, bool strongCheck = true) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableSource, columnName);
      if (propertyInfo == null && !strongCheck)
        return int.MaxValue; //assuming extreme value for weak check
      var attrs = propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true);
      return attrs != null && attrs.Length > 0 ? ((StringLengthAttribute)attrs[0]).MaximumLength : int.MaxValue;
    }

    //Strong check demands model (C# class) to be present, weak check doesn't
    protected virtual int getMinimumStringLengthFor(string prefix, string tableSource, string columnName, bool strongCheck = true) {
      PropertyInfo propertyInfo = getColumnPropertyInfo(prefix, tableSource, columnName);
      if (propertyInfo == null && !strongCheck)
        return int.MinValue; //assuming extreme value for weak check
      var attrs = propertyInfo.GetCustomAttributes(typeof(StringLengthAttribute), true);
      return attrs != null && attrs.Length > 0 ? ((StringLengthAttribute)attrs[0]).MinimumLength : int.MinValue;
    }
  }
}
