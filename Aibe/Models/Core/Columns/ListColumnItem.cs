using Extension.Models;
using System.Collections.Generic;
using System.Text;

namespace Aibe.Models.Core {
  public class ListColumnItem : BaseInfo {
    public List<ListColumnSubItem> SubItems { get; private set; } = new List<ListColumnSubItem>();
    public List<int> Widths { get; private set; }
    public string Type { get; private set; } = "LVO";
    public string CurrentDesc { get {
        if (SubItems == null)
          return string.Empty;
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < SubItems.Count; ++i) {
          if (i > 0)
            sb.Append("|");
          sb.Append(SubItems[i].CurrentDesc);
        }
        return sb.ToString();
      }
    }

    //Called for creation, such as when add button is used in the javascript
    //legacy: TipLength=10|mm|mm,cm,m  OR  HasTipLength=True OR Ian=17|He is a good officer OR Name=val|ddc1,ddc2|passed!
    //new one: TipLength|10|mm|mm,cm,m  OR  HasTipLength|True OR Ian|17|He is a good officer OR Name|val|ddc1,ddc2|passed!
    public ListColumnItem (string desc, string lcType, List<int> widths) : base(desc) { 
      if (string.IsNullOrWhiteSpace(desc))
        return;
      IsValid = true; //this point onwards is OK
      Widths = widths;
      Type = lcType.ToUpper();
      string newDesc = getNonLegacyDesc(desc);
      var descParts = newDesc.Split('|'); //yes, split, because it can be empty
      int descIndex = 0; //start from 0
      int widthIndex = 0; //width index also starts from 0
      foreach (char c in Type) {
        bool hasDescPart = descIndex < descParts.Length;
        bool hasDescNextPart = descIndex + 1 < descParts.Length;
        bool hasWidthDefined = widths != null && widthIndex < widths.Count;
        int width = hasWidthDefined ? widths[widthIndex] : ListColumnInfo.DefaultWidth;
        ++widthIndex; //widthIndex always only increase by one
        if (c == 'L' || c == 'V') { //if c is label or value, then simply creates a sub-item
          ListColumnSubItem subItem = new ListColumnSubItem(hasDescPart ? descParts[descIndex] : string.Empty, c, width);
          SubItems.Add(subItem);
          ++descIndex;
          continue;
        }
        if (c == 'O' || c == 'C') { //option or check list, next part is thus expected
          StringBuilder sb = new StringBuilder(hasDescPart ? descParts[descIndex] : string.Empty);
          sb.Append("|");
          sb.Append(hasDescNextPart ? descParts[descIndex + 1] : string.Empty);
          ListColumnSubItem subItem = new ListColumnSubItem(sb.ToString(), c, width);
          SubItems.Add(subItem);
          descIndex += 2;
        }
      }
    }

    private string getNonLegacyDesc(string desc) {
      int index = desc.IndexOf('='); //check for legacy
      int indexVbar = desc.IndexOf('|');
      StringBuilder usedDesc = new StringBuilder();

      //has equal sign, for legacy purpose, AND
      //if index vbar does not exist or if index vbar comes after the equal sign, change the equal sign to vbar
      if (index >= 0 && (indexVbar < 0 || indexVbar > index)) {
        usedDesc.Append(desc.Substring(0, index));
        usedDesc.Append("|");
        if (desc.Length > index + 1) //means, there is something after the original equal sign
          usedDesc.Append(desc.Substring(index + 1));
        return usedDesc.ToString();
      }

      //has no equal sign, OR
      //has equal sign after the vbar, leave it alone
      return desc;
    }


  }
}

//StringBuilder usedDesc = new StringBuilder();

//if (index >= 0) { //has equal sign, for legacy purpose
//  if (indexVbar < 0 || indexVbar > index) { //if index vbar does not exist or if index vbar comes after the equal sign, change the equal sign to vbar

//  } else {

//  }
//}

//if (index >= 0 && desc.Length <= index + 1) { //no = sign is found, then do not need to do anything to change
//  Name = desc.Trim(); //TipLength        
//  return;
//}

//Name = desc.Substring(0, index).Trim(); //TipLength
//string itemDesc = desc.Substring(index + 1).Trim(); //10|mm|mm,cm,m  OR  10||  OR  False OR 17|He is a good officer OR val|ddc1,ddc2|passed!

//var itemDescParts = itemDesc.Split('|').Select(x => x.Trim()).ToList(); //[10  mm  mm,cm,m] OR [17  "He is a good officer"] OR [val ddc1,ddc2 passed!]
//if (itemDescParts == null || itemDescParts.Count <= 0)
//  return;
//Value = itemDescParts[0];
//if (itemDescParts.Count <= 1 || Type.EqualsIgnoreCase("check"))
//  return;

//if (Type.EqualsIgnoreCase("remarks")) {
//  Remarks = itemDescParts[1];
//  return;
//} else if(Type.EqualsIgnoreCase("dropdown")) {
//  Dropdown = itemDescParts[1];
//  DropdownList = itemDescParts[1].Split(',').Select(x => x.Trim()).ToList();
//  if (itemDescParts.Count <= 2)
//    return;
//  Remarks = itemDescParts[2];
//  return;
//}

//Ending = itemDescParts[1];
//if (itemDescParts.Count <= 2)
//  return;
//EndingList = itemDescParts[2].Split(',').Select(x => x.Trim()).ToList();

//public string Name { get; private set; }
//public string Value { get; set; } //this can be set outside
//public string Ending { get; set; } //this can be set outside
//public string Dropdown { get; private set; }
//public List<string> DropdownList { get; private set; }
//public bool HasDropdownList { get { return DropdownList != null && DropdownList.Count > 0; } }
//public string Remarks { get; set; } //this can be set outside
//public List<string> EndingList { get; private set; }
//public bool HasEndingList { get { return EndingList != null && EndingList.Count > 0; } }
