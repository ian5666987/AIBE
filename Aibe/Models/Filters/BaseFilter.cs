namespace Aibe.Models.Filters {
  public class BaseFilter {
    public int? Page { get; set; } = 1;

    public int No { get; set; } = 0;

    public string Msg { get; set; }
  }
}