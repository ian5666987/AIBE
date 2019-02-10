using System;

namespace Aibe.Helpers {
  public class DateTimeHelper {
    public static string ProcessPossibleDateTimeString(string input, bool isDateTime, string dateTimeFormat) {
      string replacementString = null;
      if (isDateTime && !string.IsNullOrWhiteSpace(input)) {
        DateTime dateTimeValue;
        bool result = DateTime.TryParse(input, out dateTimeValue);
        if (result) {
          replacementString = dateTimeValue.ToString(dateTimeFormat);
        }
      }
      return replacementString ?? input;
    }

    public static string ProcessPossibleDateTimeString(object input, bool isDateTime, string dateTimeFormat) {
      return ProcessPossibleDateTimeString(input?.ToString(), isDateTime, dateTimeFormat);
    }
  }
}
