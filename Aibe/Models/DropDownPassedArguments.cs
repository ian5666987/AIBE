using Extension.String;

namespace Aibe.Models {
  public class DropdownPassedArguments {
    public object OriginalValue { get; set; }
    public object Value { get; set; }
    public string DataType { get; set; }

    public string GetFilterStringOriginalValue() {
      if (DataType.EqualsIgnoreCase(DH.StringDataType) || DataType.EqualsIgnoreCase(DH.DateTimeDataType))
        return string.Concat("'", OriginalValue.ToString().Replace("'", "''"), "'");
      return OriginalValue.ToString();
    }

    public string GetFilterStringValue() {
      if (DataType.EqualsIgnoreCase(DH.StringDataType) || DataType.EqualsIgnoreCase(DH.DateTimeDataType))
        return string.Concat("'", Value.ToString().Replace("'", "''"), "'");
      return Value.ToString();
    }
  }
}