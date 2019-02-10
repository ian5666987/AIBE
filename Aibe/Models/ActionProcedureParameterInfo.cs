using System;

namespace Aibe.Models {
  public class ActionProcedureParameterInfo {
    public string ParameterName { get; set; }    
    public Type DataType { get; set; }
    public string ParameterValueString { get; set; }

    public object GetDBParameter() {
      if (string.IsNullOrWhiteSpace(ParameterValueString) || ParameterValueString == DH.NULL)
        return DBNull.Value;
      string parValueString = ParameterValueString.Trim();
      //just for debugging
      //string dataTypeString = DataType.ToString();
      //try {
      if (DataType == typeof(string) || DataType == typeof(DateTime)) {
        if (!parValueString.StartsWith("'") || !parValueString.EndsWith("'") || parValueString.Length <= 2)
          return parValueString;
        string parValue = parValueString.Substring(1, parValueString.Length - 2);
        if (DataType == typeof(string))
          return parValue;
        if (DataType == typeof(DateTime))
          return DateTime.Parse(parValue);
      } else if (DataType == typeof(byte))
        return byte.Parse(parValueString);
      else if (DataType == typeof(short))
        return short.Parse(parValueString);
      else if (DataType == typeof(int))
        return int.Parse(parValueString);
      else if (DataType == typeof(long))
        return long.Parse(parValueString);
      else if (DataType == typeof(float))
        return float.Parse(parValueString);
      else if (DataType == typeof(double))
        return double.Parse(parValueString);
      else if (DataType == typeof(decimal))
        return decimal.Parse(parValueString);
      else if (DataType == typeof(bool)) {
        if (parValueString == "0")
          return false;
        else if (parValueString == "1")
          return true;
        return bool.Parse(parValueString);
      }
      //} catch (Exception ex) {
      //  throw new Exception(parValueString + " " + dataTypeString + " " + ex.ToString());
      //}
      return DBNull.Value;
    }
  }
}