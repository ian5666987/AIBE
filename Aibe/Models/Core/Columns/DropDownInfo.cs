using Extension.Models;
using Extension.String;
using System.Collections.Generic;
using System.Linq;

namespace Aibe.Models.Core {
  public class DropDownInfo : CommonBaseInfo {
    private static List<string> validOrderByDirectives = new List<string>() { DH.AscOrderWord, DH.DescOrderWord };
    public string OrderByDirective { get; private set; } //by default this is null, unless this is specified as asc or desc
    public List<DropDownItemInfo> Items { get; private set; } = new List<DropDownItemInfo>();
    
    public DropDownInfo(string desc) : base(desc) { //Each Info should be like Info1=1,2,3,[RInfo1],[RInfo2],...
      if (!HasRightSide) { //must have right side
        IsValid = false;
        return;
      }

      //Now, allows ',' on the script as long as the are enclosed
      var parts = RightSide.ParseComponentsWithEnclosurePairs(',', true, new List<KeyValuePair<char, char>> { //does not allow ws here
        new KeyValuePair<char, char>('[', ']')
      }).Select(x => x.Trim()).ToList();

      string possibleDirectiveContent = parts.Select(x => x.GetNonEmptyTrimmedInBetween("{", "}"))
        .Where(x => !string.IsNullOrWhiteSpace(x)).FirstOrDefault();
      if (!string.IsNullOrWhiteSpace(possibleDirectiveContent)) //has something
        if (validOrderByDirectives.Any(x => x.Equals(possibleDirectiveContent.ToUpper()))) //the directive is valid
          OrderByDirective = possibleDirectiveContent.ToUpper();        

      Items = parts.Where(x => !x.StartsWith("{") && !x.EndsWith("}")) //select whatever is not order by directive
        .Select(x => new DropDownItemInfo(x.Trim())).Where(x => x.IsValid).ToList();

      IsValid = Items != null && Items.Any();
    }
  }
}