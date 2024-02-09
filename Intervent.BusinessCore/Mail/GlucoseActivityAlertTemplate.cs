using Intervent.Web.DTO;

namespace Intervent.Business.Mail
{
    public class GlucoseActivityAlertTemplate : NotificationTemplate
    {
        public GlucoseActivityAlertTemplate(EmailRequestDto requestDto)
            : base(requestDto)
        {

        }

        public override string MailBody
        {
            get
            {
                string html = @"
                        
                <html>
                <head>
                    <title>@@TITLE@@</title>
                </head>
                <body>
                    <h2>We haven't received glucose data from patterns last @@ADDITIONAL_MESSAGE@@</h2>
                </body>
            </html>
            ";
                html = html.Replace("@@TITLE@@", this.Subject).Replace("@@ADDITIONAL_MESSAGE@@", base._requestDto.AdditionalMessage);
                return html;
            }
        }

        public override string Subject
        {
            get { return "Missing data from glucose"; }
        }

        public override string[] ToEmailAddress
        {
            get { return new string[] { System.Configuration.ConfigurationManager.AppSettings["AlertEmail"].ToString() }; }
        }
    }
}
