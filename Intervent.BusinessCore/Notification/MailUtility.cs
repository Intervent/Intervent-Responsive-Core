using Intervent.Web.DTO;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Intervent.Business.Notification
{
    public static class MailUtility
    {
        const bool ENABLE_SSL = true;
        public static void SendEmail(NotificationMessageDto requestDto)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(requestDto.FromAddress, "INTERVENT Support");
                mail.To.Add(requestDto.ToAddress);
                mail.Subject = requestDto.Subject;
                mail.Body = requestDto.MessageBody;
                mail.IsBodyHtml = requestDto.isBodyHtml;
                if (requestDto.Attachment != null)
                {
                    string path = ConfigurationManager.AppSettings["MailAttachmentPath"] + "\\" + requestDto.Attachment;
                    mail.Attachments.Add(new Attachment(path));
                }
                // Can set to false, if you are sending pure text.
                mail.IsBodyHtml = true;
                using (SmtpClient smtp = new SmtpClient(ConfigurationManager.AppSettings["SMPTAddress"].ToString(), Convert.ToInt32(ConfigurationManager.AppSettings["PortNumber"].ToString())))
                {
                    smtp.Credentials = new NetworkCredential(ConfigurationManager.AppSettings["SecureEmail"].ToString(), ConfigurationManager.AppSettings["SecureEmailPassword"].ToString());
                    smtp.EnableSsl = ENABLE_SSL;
                    smtp.Send(mail);
                }
            }
        }
    }
}
