using Intervent.Web.DTO;

namespace Intervent.Business.Mail
{
    public class GenericErrorTemplate : NotificationTemplate
    {
        public GenericErrorTemplate(EmailRequestDto requestDto)
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
                    <h2>Error occurred when processing the request. Functionality -@@FUNCTIONALITY_NAME@@</h2>
                    <br />
                    <div>
                    Additional Details - @@ADDITIONAL_MESSAGE@@
                    </div>
                </body>
            </html>
            ";
                html = html.Replace("@@TITLE@@", this.Subject).Replace("@@FUNCTIONALITY_NAME@@", base._requestDto.FunctionalityName).Replace("@@ADDITIONAL_MESSAGE@@", base._requestDto.AdditionalMessage);
                return html;
            }
        }

        public override string Subject
        {
            get { return "ERROR"; }
        }

        public override string[] ToEmailAddress
        {
            get { return new string[] { System.Configuration.ConfigurationManager.AppSettings["AlertEmail"].ToString() }; }
        }
    }
}
