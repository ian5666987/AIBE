using Extension.Models;
using Extension.String;

namespace Aibe.Models.Core {
  public class PictureColumnInfo : CommonBaseInfo {
    private const string skip = DH.Skip;
    public const int DefaultWidth = 100;
    public const int DefaultHeight = 100;
    public bool HeightComesFirst { get; private set; } //by default, false. Width comes first.
    public bool IsStretched { get; private set; } //by default false
    public int Width { get; private set; } = DefaultWidth;
    public int Height { get; private set; } = DefaultHeight;
    public bool IndexHeightComesFirst { get; private set; } //by default, false. Width comes first.
    public bool IndexIsStretched { get; private set; } //by default false
    public int IndexWidth { get; private set; } = DefaultWidth;
    public int IndexHeight { get; private set; } = DefaultHeight;
    public PictureColumnInfo(string desc) : base(desc) { //now it looks like 100,50|60,70
      if (HasRightSide) {
        SimpleExpression mainExp = new SimpleExpression(RightSide, "|", false); //To separate between the index expression from the rests
        if (!mainExp.IsValid)
          return;

        int width, height;
        bool widthResult, heightResult;
        SimpleExpression leftExp = new SimpleExpression(mainExp.LeftSide, ",", false);
        if (leftExp.IsValid) {
          HeightComesFirst = leftExp.LeftSide.EqualsIgnoreCase(skip); //can only happen if width is skipped
          widthResult = int.TryParse(leftExp.LeftSide, out width); //whatever is this, try to parse
          if (widthResult && width > 0) //if the parsing is successful and the value is positive, only then we can take it
            Width = width;
          if (!leftExp.IsSingular) {
            heightResult = int.TryParse(leftExp.RightSide, out height);
            if (heightResult && height > 0)
              Height = height;
            IsStretched = widthResult && heightResult; //only if both are specified correctly then IsStretched can be true
          }
        }
        
        if (mainExp.IsSingular) { //just copy the left exp to the right if singular
          IndexHeightComesFirst = HeightComesFirst;
          IndexIsStretched = IsStretched;
          IndexWidth = Width;
          IndexHeight = Height;
          return;
        } //else, start to make rules for the index page

        SimpleExpression rightExp = new SimpleExpression(mainExp.RightSide, ",", false);
        if (!rightExp.IsValid)
          return;

        IndexHeightComesFirst = rightExp.LeftSide.EqualsIgnoreCase(skip);
        widthResult = int.TryParse(rightExp.LeftSide, out width); //whatever is this, try to parse
        if (widthResult && width > 0) //if the parsing is successful and the value is positive, only then we can take it
          IndexWidth = width;
        if (rightExp.IsSingular)
          return;
        heightResult = int.TryParse(rightExp.RightSide, out height);
        if (heightResult && height > 0)
          IndexHeight = height;
        IndexIsStretched = widthResult && heightResult; //only if both are specified correctly then IsStretched can be true
      }
    }
  }
}