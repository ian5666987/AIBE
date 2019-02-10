using Extension.Models;
namespace Aibe.Models.Core {
  public class TextFieldColumnInfo : CommonBaseInfo {
    public const int DefaultRowSize = 4;
    public int RowSize { get; private set; } = DefaultRowSize;
    public TextFieldColumnInfo(string desc) : base(desc) {
      if (HasRightSide) {
        int value;
        bool result = int.TryParse(RightSide, out value);
        if (result && value > 0) //if the parsing is successful and the value is positive, only then we can take it
          RowSize = value;
      }
    }
  }
}