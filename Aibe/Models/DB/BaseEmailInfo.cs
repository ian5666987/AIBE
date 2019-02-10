using System.Text;

namespace Aibe.Models.DB {
  public class BaseEmailInfo {
    public int Cid { get; set; }
    public string TemplateName { get; set; }
    public string EmailFrom { get; set; }
    public string EmailTo { get; set; }
    public string EmailSubject { get; set; }
    public string EmailCc { get; set; }
    public string EmailBcc { get; set; }
    public string EmailBody { get; set; }
    public string EmailParameters { get; set; }
    public bool IsSent { get; set; }
    public string AttachmentFilePaths { get; set; }

    public virtual string GetUpdateIsSentSqlString() {
      StringBuilder sb = new StringBuilder();
      sb.Append(string.Concat("UPDATE ", DH.EmailInfoTableName, " SET "));
      sb.Append(string.Concat(DH.EmailMakerIsSentColumnName, "=1"));
      sb.Append(string.Concat(" WHERE ", DH.Cid, "=", Cid));
      return sb.ToString();
    }
  }
}