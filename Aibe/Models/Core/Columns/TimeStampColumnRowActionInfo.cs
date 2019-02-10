using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class TimeStampColumnRowActionInfo : BaseInfo {
    private static List<string> validRowActionsApplied = new List<string> { DH.CreateActionName, DH.EditActionName };
    private static List<string> validTimeShiftValues = new List<string> { "+", "-" };
    public string Name { get; private set; }
    public string Operator { get; private set; }
    public double ShiftValue { get; private set; }
    public bool IsFixed { get; private set; } = true; //by default, IsFixed is chosen
    public TimeStampColumnRowActionInfo(string desc) : base(desc) {
      if (string.IsNullOrWhiteSpace(desc))
        return;
      var descParts = desc.GetTrimmedNonEmptyParts('|');
      if (descParts == null || descParts.Count <= 0)
        return;
      if (!validRowActionsApplied.Any(x => x.EqualsIgnoreCaseTrim(descParts[0]))) //the action name must be valid
        return;
      IsValid = true;
      Name = descParts[0];

      if (descParts.Count < 2 || descParts[1].Length < 2) //the length of the descParts must be at least two to create things like +4 or -5
        return;

      string valuePart = descParts[1].Substring(1).Trim();

      if (string.IsNullOrWhiteSpace(valuePart) || !validTimeShiftValues.Any(x => descParts[1].StartsWith(x))) //invalid time described
        return;

      double testValue;
      bool result = double.TryParse(valuePart, out testValue);
      if (result && testValue > 0) {
        Operator = validTimeShiftValues.FirstOrDefault(x => descParts[1].StartsWith(x));
        ShiftValue = testValue;
      }

      if (descParts.Count < 3)
        return;

      //only become false if it is asked to be false, otherwise remains true
      IsFixed = !descParts[2].EqualsIgnoreCaseTrim(false.ToString());
    }
  }
}