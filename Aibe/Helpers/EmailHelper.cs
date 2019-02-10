using Aibe.Models;
using Aibe.Models.DB;
using Extension.Database.SqlServer;
using Extension.Extractor;
using Extension.Models;
using Extension.String;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Configuration;
using System.Net.Mail;
using System.Threading;
using SWM = System.Web.Mail;

namespace Aibe.Helpers {
  public class EmailHelper<T> where T : BaseEmailInfo {

    private SmtpSection smtpSection;
    private EmailServiceType emailServiceType;
    private string SmtpHost { get { return smtpSection.Network.Host; } }
    private int SmtpPort { get { return smtpSection.Network.Port; } }
    private string SmtpUserName { get { return smtpSection.Network.UserName; } }
    private string SmtpPassword { get { return smtpSection.Network.Password; } }
    private bool SmtpEnableSsl { get { return smtpSection.Network.EnableSsl; } }
    public delegate string GetUpdateSqlStringDelegate(int cid);
    public EmailHelper(EmailServiceType emailServiceType) {
      smtpSection = (SmtpSection)ConfigurationManager.GetSection(DH.SMTPSectionConf);
      this.emailServiceType = emailServiceType;
    }

    public virtual DataTable GetNotSentEmailInfoTable() {
      return SQLServerHandler.GetFullDataTableWhere(DH.DataDBConnectionString, DH.EmailInfoTableName,
        string.Concat(DH.EmailMakerIsSentColumnName, "=0"));
    }

    public List<BaseErrorModel> SendEmails(DataTable emailInfoTable = null, GetUpdateSqlStringDelegate updateSQLString = null) {
      DataTable table = emailInfoTable ?? GetNotSentEmailInfoTable();
      List<T> emailInfos = BaseExtractor.ExtractList<T>(table);
      List<BaseErrorModel> errors = new List<BaseErrorModel>();
      foreach (T emailInfo in emailInfos) {
        BaseErrorModel em = SendEmail(emailInfo);
        errors.Add(em);
        if (!em.HasError) {
          string sqlScript = updateSQLString == null ? emailInfo.GetUpdateIsSentSqlString() : updateSQLString(emailInfo.Cid);
          SQLServerHandler.ExecuteScript(DH.DataDBConnectionString, sqlScript);
        }
      }
      return errors;
    }

    public BaseErrorModel SendEmail(T emailInfo) {
      BaseErrorModel em = new BaseErrorModel();
      switch (emailServiceType) {
        case EmailServiceType.WEBMAIL: em = sendWebMail(emailInfo); break;
        case EmailServiceType.NETMAIL:
        default: em = sendNetMail(emailInfo); break;
      }
      return em;
    }

    private BaseErrorModel sendWebMail(T emailInfo) {
      BaseErrorModel errorModel = new BaseErrorModel();
      SWM.MailMessage webMailMessage = new SWM.MailMessage();
      webMailMessage.Fields[DH.SMTP_SERVER] = SmtpHost;
      webMailMessage.Fields[DH.SMTP_SERVER_PORT] = SmtpPort;
      webMailMessage.Fields[DH.SMTP_USE_SSL] = SmtpEnableSsl;
      webMailMessage.Fields[DH.SEND_USERNAME] = SmtpUserName;
      webMailMessage.Fields[DH.SEND_USERNAME] = SmtpPassword;
      webMailMessage.Fields[DH.SEND_USING] = 2; //What is this number?
      webMailMessage.Fields[DH.SMTP_AUTHENTICATE] = 1; //what is this number?
      webMailMessage.To = emailInfo.EmailTo;
      webMailMessage.From = emailInfo.EmailFrom;
      webMailMessage.Cc = emailInfo.EmailCc;
      webMailMessage.Bcc = emailInfo.EmailBcc;
      webMailMessage.Subject = emailInfo.EmailSubject;
      webMailMessage.Body = emailInfo.EmailBody;

      if (!string.IsNullOrEmpty(emailInfo.AttachmentFilePaths)) {
        List<string> filePaths = emailInfo.AttachmentFilePaths.GetTrimmedNonEmptyParts(';');
        foreach(string filePath in filePaths) {
          if (!File.Exists(filePath))
            continue;
          SWM.MailAttachment attachment = new SWM.MailAttachment(filePath);
          webMailMessage.Attachments.Add(attachment);
        }
      }

      try {
        SWM.SmtpMail.Send(webMailMessage);
        Thread.Sleep(1000); //why is there a sleep here?
      } catch (Exception ex) {
        errorModel.Code = -1;
        errorModel.Message = ex.Message;
        errorModel.StackTrace = ex.StackTrace;
        errorModel.Exception = ex.ToString();
      }
      return errorModel;
    }

    private BaseErrorModel sendNetMail(T emailInfo) {
      List<string> toList = emailInfo.EmailTo.GetTrimmedNonEmptyParts(';');
      MailMessage netMailMessage = new MailMessage(emailInfo.EmailFrom, toList[0]);
      for (int i = 1; i < toList.Count; ++i)
        netMailMessage.To.Add(toList[i]);
      if (!string.IsNullOrWhiteSpace(emailInfo.EmailCc))
        foreach (string cc in emailInfo.EmailCc.GetTrimmedNonEmptyParts(';'))
          netMailMessage.CC.Add(cc);
      if (!string.IsNullOrWhiteSpace(emailInfo.EmailBcc))
        foreach (string bcc in emailInfo.EmailBcc.GetTrimmedNonEmptyParts(';'))
          netMailMessage.Bcc.Add(bcc);
      netMailMessage.Subject = emailInfo.EmailSubject;
      netMailMessage.Body = emailInfo.EmailBody;
      if (!string.IsNullOrWhiteSpace(emailInfo.AttachmentFilePaths)) {
        List<string> filePaths = emailInfo.AttachmentFilePaths.GetTrimmedNonEmptyParts(';');
        foreach (string filePath in filePaths) {
          if (!File.Exists(filePath))
            continue;
          Attachment attachment = new Attachment(filePath);
          netMailMessage.Attachments.Add(attachment);
        }
      }

      BaseErrorModel errorModel = new BaseErrorModel();
      try {
        SmtpClient client = new SmtpClient(SmtpHost, SmtpPort);
        client.EnableSsl = SmtpEnableSsl;
        client.Credentials = new NetworkCredential(SmtpUserName, SmtpPassword);
        client.Send(netMailMessage);
        Thread.Sleep(1000);
      } catch (Exception ex) {
        errorModel.Code = -1;
        errorModel.Message = ex.Message;
        errorModel.StackTrace = ex.StackTrace;
        errorModel.Exception = ex.ToString();
      }
      return errorModel;
    }
  }
}