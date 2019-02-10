using Aibe.Models.Core;
using System.Collections.Generic;
using System.Linq;
using System.Data;

namespace Aibe.Models {
  public class ListColumnResult {
    public string Name { get; private set; }
    public bool IsSuccessful { get; set; } //TODO originally private
    public string DataValue { get; set; } //TODO originally private
    public string ViewString { get; set; } //can be set outside, and this is OK
    public ListColumnInfo UsedListColumnInfo { get; set; } //can be set outside, especially from MetaInfo method when creating LiveListSubcolumns

    public ListColumnResult(string name, string dataValue) {
      Name = name;
      DataValue = dataValue;
    }

    //To get LIVE subcolumns
    public bool UpdateLiveSubcolumnsDataValue(ListColumnInfo info, string changedColumnName, string changedColumnValue) {
      IsSuccessful = false;
      string newDataValue = string.Empty;
      bool extractResult = info.GetRefDataValue(changedColumnName, changedColumnValue, out newDataValue);
      if (!extractResult)
        return false;
      DataValue = newDataValue;
      IsSuccessful = true; //This is going to be used by Javascript, so leave this be
      return true; //not need to update IsSuccessfulHere
    }

    //To update the list column result to produce new data value
    public bool UpdateDataValue(ListColumnInfo info, string inputValue, int rowNo, int columnNo, string lcType) {
      IsSuccessful = false;
      string newDataValue = DataValue;

      if (rowNo < 1)
        return false;

      int rowIndex = rowNo - 1;

      List<ListColumnItem> listColumnItems = DataValue.Split(';')
        .Select(x => new ListColumnItem(x.Trim(), lcType, info.Widths))
        .Where(x => x.IsValid).ToList();

      if (rowIndex >= listColumnItems.Count) //no such row
        return false;

      ListColumnItem changedListColumnItem = listColumnItems[rowIndex]; //able to get the wanted changed item

      if (changedListColumnItem == null) //item not found
        return false;

      int columnIndex = columnNo - 1;

      if (changedListColumnItem.SubItems == null || columnIndex >= changedListColumnItem.SubItems.Count) //no such column
        return false;

      ListColumnSubItem changedListColumnSubItem = changedListColumnItem.SubItems[columnIndex];

      //not, process the input value here...
      changedListColumnSubItem.Value = inputValue;

      //Rejoin the string
      newDataValue = string.Join(";", listColumnItems.Select(x => x.CurrentDesc));
      DataValue = newDataValue;
      IsSuccessful = true; //this is to be used in the Javascript, so leave it there
      return true;
    }

    //To add or to delete an item
    public bool AddCopyOrDeleteDataValue(int itemNo, MetaInfo meta, string columnName, string addString, string lcType, bool isCopy) {
      IsSuccessful = false;
      string newDataValue = DataValue;

      if (itemNo == 0) { //add
        if (!meta.IsListColumn(columnName) || string.IsNullOrWhiteSpace(addString))
          return false;
        ListColumnInfo info = meta.GetListColumnInfo(columnName);
        ListColumnItem item = new ListColumnItem(addString, lcType, info.Widths);
        if (!item.IsValid)
          return false;
        newDataValue = string.Concat(string.IsNullOrWhiteSpace(DataValue) ? string.Empty : DataValue + ";", item.CurrentDesc);
      } else { //copy or delete
        int itemIndex = itemNo - 1;
        var dataParts = DataValue.Split(';').Select(x => x.Trim()).ToList();
        if (isCopy) {
          dataParts.Add(dataParts[itemIndex]);
        } else
          dataParts.RemoveAt(itemIndex);
        newDataValue = string.Join(";", dataParts);
      }

      DataValue = newDataValue;
      IsSuccessful = true; //this is to be used in the Javascript, so leave it there
      return true;
    }
  }
}