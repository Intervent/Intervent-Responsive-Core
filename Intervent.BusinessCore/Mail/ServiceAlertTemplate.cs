using Intervent.Web.DTO;

namespace Intervent.Business.Mail
{
    public class ServiceAlertTemplate : NotificationTemplate
    {
        public ServiceAlertTemplate(EmailRequestDto requestDto)
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
                    <title>@@Timing@@</title>
                </head>
                <body>
                    <h2>Windows Service has stopped at @@Timing@@</h2>
                </body>
            </html>
            ";
                html = html.Replace("@@TITLE@@", this.Subject);
                html = html.Replace("@@Timing@@", base._requestDto.DateandTime);
                return html;
            }
        }

        public override string Subject
        {
            get { return "Service Alert"; }
        }



        public override string[] ToEmailAddress
        {
            get { return new string[] { System.Configuration.ConfigurationManager.AppSettings["AlertEmail"].ToString() }; }
        }
    }
}
