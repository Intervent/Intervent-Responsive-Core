using Intervent.Web.DTO;

namespace Intervent.Business.Mail
{
    public class EligibilityImportTemplate : NotificationTemplate
    {
        public EligibilityImportTemplate(EmailRequestDto requestDto)
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
        <h2>Eligiility Import For @@ORGANNIZATION_NAME@@</h2>
        <table>
            <thead>
                <tr>
                    <th>Status</th>
                    <th>Count</th>
                </tr>
            </thead>
            <tbody>
                <tr>
                    <td>SUCCESS</td>
                    <td>@@SUCESS_COUNT@@</td>
                </tr>
                <tr>
                    <td>ERROR</td>
                    <td>@@ERROR_COUNT@@</td>
                </tr>
                <tr>
                    <td>TERMINATED</td>
                     <td>@@TERM_COUNT@@</td>
                </tr>
            </tbody>
        </table>
    </body>
</html>
";
                html = html.Replace("@@TITLE@@", this.Subject).Replace("@@SUCESS_COUNT@@", base._requestDto.SuccessCount.ToString()).Replace("@@ERROR_COUNT@@", base._requestDto.ErrorCount.ToString()).Replace("@@TERM_COUNT@@", base._requestDto.TerminatedRecordsCount.ToString());
                html = html.Replace("@@ORGANNIZATION_NAME@@", base._requestDto.OrganizationName);
                return html;
            }
        }

        public override string Subject
        {
            get { return "Eligibility Import"; }
        }



        public override string[] ToEmailAddress
        {
            get { return new string[] { System.Configuration.ConfigurationManager.AppSettings["AlertEmail"].ToString() }; }
        }
    }
}
