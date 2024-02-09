using Intervent.Business.Account;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using System.Configuration;
using System.Text;

namespace Intervent.Business
{
    public class LMCManager : BaseManager
    {
        public int SendInterventCanriskParticipants()
        {
            string path = ConfigurationManager.AppSettings["LMCFilePath"].ToString();
            CommonReader commonReader = new CommonReader();
            ParticipantReader reader = new ParticipantReader();
            var users = reader.GetCanriskParticipants();
            if (users.Count > 0)
            {
                var builder = new StringBuilder();
                var header = builder.AppendLine("UniqueId|FirstName|LastName|DOB|Address1|Address2|City|StateOrProvince|ZipOrPostalCode|HomePhone|EmailAddress|Is Eligible|Permission to contact the family doctor|Family doctor name|Family doctor location|Family doctor phone|Family doctor fax|Referral type");
                IEnumerable<string> response = users
                    .Select(x => String.Join("|", x.UniqueId,
                    x.FirstName,
                    x.LastName,
                    x.DOB.HasValue ? x.DOB.Value.ToString("yyyyMMdd") : "",
                    x.Address,
                    x.Address2,
                    x.City,
                    x.State1.Name,
                    x.Zip,
                    x.HomeNumber,
                    x.Email,
                    LabManager.CheckProgramCondition(commonReader.GetAge(Convert.ToDateTime(x.DOB.Value)), x.Labs2.FirstOrDefault().A1C, x.Labs2.FirstOrDefault().Glucose, x.Labs2.FirstOrDefault().Weight, reader.GetCanriskResponse(new GetCanriskRequest() { uniqueId = x.UniqueId }), x.Labs2.FirstOrDefault().DidYouFast, x.Labs2.FirstOrDefault().BMI).eligibility ? "Eligible" : "Not Eligible",
                    x.UserDoctorInfoes.FirstOrDefault() != null ? (x.UserDoctorInfoes.FirstOrDefault().Provider == null ? (x.UserDoctorInfoes.FirstOrDefault().ContactPermission == 1 ? "Yes" : "No") : "Yes") : "",
                    x.UserDoctorInfoes.FirstOrDefault() != null ? (x.UserDoctorInfoes.FirstOrDefault().Provider == null ? x.UserDoctorInfoes.FirstOrDefault().Name : x.UserDoctorInfoes.FirstOrDefault().Provider.Name) : "",
                    x.UserDoctorInfoes.FirstOrDefault() != null ? (x.UserDoctorInfoes.FirstOrDefault().Provider == null ? (x.UserDoctorInfoes.FirstOrDefault().Address + ", " + x.UserDoctorInfoes.FirstOrDefault().City + ", " + x.UserDoctorInfoes.FirstOrDefault().State1?.Name + ", " + x.UserDoctorInfoes.FirstOrDefault().Country1?.Name) : (x.UserDoctorInfoes.FirstOrDefault().Provider.Address + ", " + x.UserDoctorInfoes.FirstOrDefault().Provider.City + ", " + x.UserDoctorInfoes.FirstOrDefault().Provider.State1?.Name + ", " + x.UserDoctorInfoes.FirstOrDefault().Provider.Country1?.Name)) : "",
                    x.UserDoctorInfoes.FirstOrDefault() != null ? (x.UserDoctorInfoes.FirstOrDefault().Provider == null ? x.UserDoctorInfoes.FirstOrDefault().PhoneNumber : x.UserDoctorInfoes.FirstOrDefault().Provider.PhoneNumber) : "",
                    x.UserDoctorInfoes.FirstOrDefault() != null ? (x.UserDoctorInfoes.FirstOrDefault().Provider == null ? x.UserDoctorInfoes.FirstOrDefault().FaxNumber : x.UserDoctorInfoes.FirstOrDefault().Provider.FaxNumber) : "",
                    (reader.GetReferralType(x.UniqueId))));
                builder.AppendLine(String.Join(Environment.NewLine, response));
                System.IO.File.WriteAllText(
                    System.IO.Path.Combine(
                    path, "PartAEligibility" + DateTime.Now.ToString("yyyyMMdd") + ".csv"),
                    builder.ToString());
                reader.LogSentRecords(users.Select(x => x.UniqueId).ToList());
            }
            return users.Count();
        }

        public int DeactivateDuplicates()
        {
            AccountManager accountManager = new AccountManager();
            ParticipantReader participantReader = new ParticipantReader();
            var portalId = accountManager.CurrentPortalId(71).PortalId; //LMC Organization = 71
            var duplicateCount = participantReader.GetDuplicateEligiblityRecords(portalId.Value);
            return duplicateCount;
        }

        public int CompIntroKitsOnTime()
        {
            AccountManager accountManager = new AccountManager();
            ParticipantReader participantReader = new ParticipantReader();
            var portalId = accountManager.CurrentPortalId(71).PortalId; //LMC Organization = 71
            return participantReader.CompIntroKitsOnTime(portalId.Value);
        }
    }
}
