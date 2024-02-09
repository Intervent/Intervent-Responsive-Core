using Intervent.Web.DTO;
using System.Configuration;
using System.Net;
using System.Net.Mail;

namespace Intervent.Business.Mail
{
    public abstract class NotificationTemplate
    {
        //need to move the setting to a config file

        const bool ENABLE_SSL = true;
        protected EmailRequestDto _requestDto;
        public NotificationTemplate(EmailRequestDto requestDto)
        {
            _requestDto = requestDto;
            FromEmailAddress = ConfigurationManager.AppSettings["SecureEmail"].ToString();
        }

        public abstract string MailBody { get; }

        public abstract string Subject { get; }

        public abstract string[] ToEmailAddress { get; }

        public string FromEmailAddress { get; set; }

        public void SendEmail()
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(this.FromEmailAddress, "INTERVENT Support");
                foreach (string mailAddress in this.ToEmailAddress)
                    mail.To.Add(mailAddress);
                mail.Subject = this.Subject;
                mail.Body = MailBody;
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
