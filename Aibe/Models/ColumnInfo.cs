using Aibe.Models.Core;
using Extension.String;
using System.Collections.Generic;
using System.Data;

namespace Aibe.Models {
  //Useful for creating each column info in the FilterIndexInfo
  public class ColumnInfo {
    public string Name { get; private set; }
    public string DataType { get; private set; }
    public string DisplayName { get; set; }
    public bool IsColumnIncludedInFilter { get; set; }
    public bool IsIndexIncluded { get; set; }
    public bool IsFilterIncluded { get; set; }
    public bool IsPictureColumn { get; set; }
    public bool IsAttachmentColumn { get; set; }
    public bool IsDownloadColumn { get; set; }
    public bool IsIndexShowImage { get; set; }
    public bool IsListColumn { get; set; }
    public bool IsSciptColumn { get; set; }
    public bool IsPrefilledColumn { get; set; }
    public bool IsDateTime { get { return DataType != null && DataType.EqualsIgnoreCase(DH.DateTimeDataType); } }
    public string CustomDateTimeFormat { get; set; } //definitely used only in index
    public bool HasCustomDateTimeFormat { get { return !string.IsNullOrWhiteSpace(CustomDateTimeFormat); } }
    public ScTableInfo ScTable { get; set; }
    public List<ColoringInfo> Colorings { get; set; } = new List<ColoringInfo>();
    public DataColumn Column { get; private set; }
    public ColumnInfo (DataColumn column) {
      Column = column;
      DataType = column.DataType.ToString().Substring(DH.SharedPrefixDataType.Length);
      Name = column.ColumnName;
    }
  }
}