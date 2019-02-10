﻿using Aibe.Models.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using Extension.String;
using Extension.Database.SqlServer;

namespace Aibe.Models {
  public class KeyInfo {
    public string TableSource { get; private set; }
    public string Key { get; private set; }
    public string PureKeyName { get; private set; }
    public string AddKeyName { get; private set; }
    public string DataType { get; set; }
    public bool IsNullified { get; set; } = false;
    public bool IsAutoGenerated { get; set; } = false;
    public bool IsAutoGeneratedByGroup { get; set; } = false;
    public bool IsTimeStamp { get; set; } = false;
    public bool IsTimeStampFixed { get; set; } = true;
    public bool IsTimeStampByGroup { get; set; } = false;
    public int TimeStampShift { get; set; }
    public KeyInfo(string tableSource, string usedKey) {
      int addIndex = usedKey.IndexOf(DH.BaseAppendixName);
      TableSource = tableSource;
      Key = usedKey;
      PureKeyName = addIndex >= usedKey.Length || addIndex == -1 ? usedKey : usedKey.Substring(0, addIndex);
      AddKeyName = PureKeyName == usedKey ? string.Empty : usedKey.Substring(PureKeyName.Length);
      if (Key == PureKeyName)
        DataType = DH.StringDataType;
      else if (DH.NumberTypeFilterColumns.Contains(AddKeyName)) { //time is excluded if date does not contain
        int startIndex = DH.BaseFilterAppendixName.Length;
        int length = AddKeyName.Length - DH.BaseFilterAppendixName.Length - (AddKeyName.EndsWith("From") ? 4 : 2);
        DataType = AddKeyName.Substring(startIndex, length);
      } else if (DH.FilterDateAppendixNames.Contains(AddKeyName))
        DataType = DH.DateTimeDataType;
      else if (DH.BooleanTypeFilterColumns.Contains(AddKeyName))
        DataType = DH.BooleanDataType;
      else if (DH.CharTypeFilterColumns.Contains(AddKeyName))
        DataType = DH.CharDataType;
      //Add other DataType here
      else
        DataType = DH.UnknownDataType;
    }

    public void UpdateTimeStampAndAutoGenerated(MetaInfo meta, string columnName, string actionType) {
      IsTimeStamp = meta != null &&
        DataType.EqualsIgnoreCase(DH.DateTimeDataType) &&
        !string.IsNullOrWhiteSpace(actionType) &&
        meta.IsTimeStampAppliedFor(columnName, actionType);
      if (IsTimeStamp) {
        IsTimeStampFixed = meta.IsTimeStampFixedFor(columnName, actionType);
        TimeStampShift = meta.GetTimeStampShiftFor(columnName, actionType);
      }

      IsAutoGenerated = meta != null &&
        DH.NumberDataTypes.Any(x => x.EqualsIgnoreCase(DataType)) &&
        !string.IsNullOrWhiteSpace(actionType) &&
        actionType.EqualsIgnoreCase(DH.CreateActionName) && //Auto-Generated is only valid for Create action
        meta.IsAutoGenerated(columnName);

      //To support group creation
      IsTimeStampByGroup = meta != null && meta.GroupByColumnInfos != null &&
        !string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(actionType) &&
        actionType.EqualsIgnoreCase(DH.CreateGroupActionName) && //note that as of now, EditGroup is not making a timestamp here
        meta.GroupByColumnInfos.Any(x => x.ColumnName.EqualsIgnoreCase(columnName) && x.IsAutoDateTime);

      IsAutoGeneratedByGroup = meta != null && meta.GroupByColumnInfos != null && 
        !string.IsNullOrWhiteSpace(columnName) && !string.IsNullOrWhiteSpace(actionType) &&
        actionType.EqualsIgnoreCase(DH.CreateGroupActionName) &&
        meta.GroupByColumnInfos.Any(x => x.ColumnName.EqualsIgnoreCase(columnName) && x.IsAutoInt);
    }

    public static List<KeyValuePair<string, string>> GetTableColumnPairs(MetaInfo meta, string columnName) {
      if (!meta.IsAutoGenerated(columnName)) //Not need to further check
        return null;
      List<KeyValuePair<string, string>> tableColumnNamePairs = null;
      AutoGeneratedColumnInfo agcInfo = meta.AutoGeneratedColumns //check if there is such info
        .FirstOrDefault(x => x.Name.EqualsIgnoreCase(columnName) &&
          x.TableColumnPairs.Any()); //then get the auto generated items with the Table Pair description
      if (agcInfo != null)
        tableColumnNamePairs = agcInfo.TableColumnPairs.ToList();
      return tableColumnNamePairs;
    }

    public string CreateQuerySubstring(int parNo) {
      switch (DataType) {
        case DH.StringDataType:
        case DH.CharDataType:
          return string.Concat("[", PureKeyName, "] LIKE @par", parNo);
        case DH.Int16DataType: //Int and date time is shared here, something else could also be shared!
        case DH.Int32DataType:
        case DH.Int64DataType:
        case DH.UInt16DataType:
        case DH.UInt32DataType:
        case DH.UInt64DataType:
        case DH.ByteDataType:
        case DH.SByteDataType:
        case DH.DecimalDataType:
        case DH.DoubleDataType:
        case DH.SingleDataType:
        case DH.DateTimeDataType:
          return string.Concat("[", PureKeyName, "] ",
                AddKeyName.StartsWith(DH.BaseFilterAppendixName) && 
                AddKeyName.EndsWith(DH.FromName) ?
                ">" : "<", "= @par", parNo);
        case DH.BooleanDataType:        
          return string.Concat("[", PureKeyName, "] = @par", parNo);
        default:
          return null;
      }
    }

    public object CreateQueryValueAsObject(object value) {
      return DataType == DH.StringDataType || DataType == DH.CharDataType ? string.Concat("%", value, "%") :
        DataType == DH.BooleanDataType && value != null ? (value.ToString().EqualsIgnoreCase(DH.True) ? 1 : 0) :
        value;
    }

    public object ExtractValueAsObject(Dictionary<string, string> collections, DateTime refDtNow, bool filterStyle = false,
      List<KeyValuePair<string, string>> tableColumnNamePairs = null, bool forGroupMaking = false) {
      bool parseResult;
      string value = collections.ContainsKey(Key) ? collections[Key] : null;
      if (value == null)
        return null;

      if (filterStyle) {
        if (DataType.EqualsIgnoreCase(DH.StringDataType) || DataType.EqualsIgnoreCase(DH.CharDataType))
          return value;
        else if (DH.NumberDataTypes.Contains(DataType)) {
          decimal valDecimalFilter;
          parseResult = decimal.TryParse(value, out valDecimalFilter);
          if (parseResult)
            return valDecimalFilter;
          else return null;
        } else if (DataType.EqualsIgnoreCase(DH.DateTimeDataType)) {
          string dtStr = value;
          string timeStr = AddKeyName.EndsWith(DH.FromName) ? //get the time string
            collections.ContainsKey(PureKeyName + DH.FilterTimeAppendixFrontName + DH.FromName) ?
              collections[PureKeyName + DH.FilterTimeAppendixFrontName + DH.FromName] :
              string.Empty
            :
            collections.ContainsKey(PureKeyName + DH.FilterTimeAppendixFrontName + DH.ToName) ?
              collections[PureKeyName + DH.FilterTimeAppendixFrontName + DH.ToName] :
              string.Empty
            ;
          if (!string.IsNullOrWhiteSpace(timeStr)) //time string exists
            dtStr += " " + timeStr;
          DateTime valDt;
          parseResult = DateTime.TryParse(dtStr, out valDt);
          if (parseResult)
            return valDt;
          else return null;
        } else if (DataType.EqualsIgnoreCase(DH.BooleanDataType)) {
          //string parseVal = value?.ToString() == "on" ? "true" : "false";
          bool valBool;
          parseResult = bool.TryParse(value, out valBool);
          if (parseResult)
            return valBool;
          else return null;
        }
        return null; //imparsable
      }

      if (IsTimeStamp) {
        DateTime timeStamp = refDtNow.AddSeconds(TimeStampShift);
        if (string.IsNullOrWhiteSpace(value) || !IsTimeStampFixed) //if the value is empty or is not fixed
          return timeStamp; //immediately returns the object
        //if the value is not empty or it is fixed, then follow the procedure        
      }

      if (IsAutoGenerated) { //gives next value for auto-generation number
        if (tableColumnNamePairs == null) { //single valued
          decimal decVal = SQLServerHandler.GetAggregatedValue(DH.DataDBConnectionString, TableSource, PureKeyName, DH.AggrMax);
          return decVal + 1;
        } else {
          tableColumnNamePairs.Insert(0, new KeyValuePair<string, string>(TableSource, PureKeyName));
          decimal decVal = SQLServerHandler.GetAggregatedValues(DH.DataDBConnectionString, tableColumnNamePairs, DH.AggrMax);
          return decVal + 1;
        }
      }

      //To support group making
      if (forGroupMaking) {
        if (IsTimeStampByGroup)
          return refDtNow;
        if (IsAutoGeneratedByGroup) {
          if (tableColumnNamePairs == null) { //single valued
            decimal decVal = SQLServerHandler.GetAggregatedValue(DH.DataDBConnectionString, TableSource, PureKeyName, DH.AggrMax);
            return decVal + 1;
          } else {
            tableColumnNamePairs.Insert(0, new KeyValuePair<string, string>(TableSource, PureKeyName));
            decimal decVal = SQLServerHandler.GetAggregatedValues(DH.DataDBConnectionString, tableColumnNamePairs, DH.AggrMax);
            return decVal + 1;
          }
        }
      }

      switch (DataType) {
        case DH.StringDataType:
          if (collections.ContainsKey(PureKeyName + DH.CreateEditPictureLinkAppendixName)) {
            string picStr = collections[PureKeyName + DH.CreateEditPictureLinkAppendixName]; //Non filter time
            if (!string.IsNullOrWhiteSpace(picStr)) {
              //TODO Do something if necessary!
            }
          }
          if (collections.ContainsKey(PureKeyName + DH.CreateEditNonPictureAttachmentAppendixName)) {
            string attStr = collections[PureKeyName + DH.CreateEditNonPictureAttachmentAppendixName]; //Non filter time
            if (!string.IsNullOrWhiteSpace(attStr)) {
              //TODO Do something if necessary!
            }
          }      
          return value;
        case DH.Int16DataType:
          short valInt16;
          parseResult = short.TryParse(value, out valInt16);
          if (parseResult)
            return valInt16;
          else return null;
        case DH.Int32DataType:
          int valInt32;
          parseResult = int.TryParse(value, out valInt32);
          if (parseResult)
            return valInt32;
          else return null;
        case DH.Int64DataType:
          long valInt64;
          parseResult = long.TryParse(value, out valInt64);
          if (parseResult)
            return valInt64;
          else return null;
        case DH.DateTimeDataType:
          DateTime valDt; //this is going to be quite tricky
          string dtStr = value;
          if (collections.ContainsKey(PureKeyName + DH.CreateEditTimeAppendixName)) {
            string timeStr = collections[PureKeyName + DH.CreateEditTimeAppendixName]; //Non filter time
            //string timeStr = filterStyle ? AddKeyName.EndsWith(ConstantHelper.FromName) ? //get the time string
            //  collections[PureKeyName + ConstantHelper.FilterTimeAppendixFrontName + ConstantHelper.FromName] :
            //  collections[PureKeyName + ConstantHelper.FilterTimeAppendixFrontName + ConstantHelper.ToName] :
            //  collections[PureKeyName + ConstantHelper.CreateEditTimeAppendixName]; //Non filter time
            if (!string.IsNullOrWhiteSpace(timeStr)) //time string exists
              dtStr += " " + timeStr;
          }
          parseResult = DateTime.TryParse(dtStr, out valDt);
          if (parseResult)
            return valDt;
          else return null;
        case DH.UInt16DataType:
          ushort valUInt16;
          parseResult = ushort.TryParse(value, out valUInt16);
          if (parseResult)
            return valUInt16;
          else return null;
        case DH.UInt32DataType:
          uint valUInt32;
          parseResult = uint.TryParse(value, out valUInt32);
          if (parseResult)
            return valUInt32;
          else return null;
        case DH.UInt64DataType:
          ulong valUInt64;
          parseResult = ulong.TryParse(value, out valUInt64);
          if (parseResult)
            return valUInt64;
          else return null;
        case DH.ByteDataType:
          byte valByte;
          parseResult = byte.TryParse(value, out valByte);
          if (parseResult)
            return valByte;
          else return null;
        case DH.SByteDataType:
          sbyte valSByte;
          parseResult = sbyte.TryParse(value, out valSByte);
          if (parseResult)
            return valSByte;
          else return null;
        case DH.DecimalDataType:
          decimal valDec;
          parseResult = decimal.TryParse(value, out valDec);
          if (parseResult)
            return valDec;
          else return null;
        case DH.DoubleDataType:
          double valDouble;
          parseResult = double.TryParse(value, out valDouble);
          if (parseResult)
            return valDouble;
          else return null;
        case DH.SingleDataType:
          float valFloat;
          parseResult = float.TryParse(value, out valFloat);
          if (parseResult)
            return valFloat;
          else return null;
        case DH.BooleanDataType:
          //string parseVal = value?.ToString() == "on" ? "true" : "false";
          bool valBool;
          parseResult = bool.TryParse(value, out valBool);
          if (parseResult)
            return valBool;
          else return null;
        case DH.CharDataType:
          char valChar;
          parseResult = char.TryParse(value, out valChar);
          if (parseResult)
            return valChar;
          else return null;
        default:
          return null;
      }
    }
  }
}