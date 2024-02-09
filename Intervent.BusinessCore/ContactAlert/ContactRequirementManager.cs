using Intervent.DAL;
using Intervent.Web.DataLayer;
using NLog;

namespace Intervent.Business
{
    public class ContactRequirementManager : BaseManager
    {
        public void ProcessContactRequirementAlerts()
        {
            ParticipantReader reader = new ParticipantReader();
            try
            {
                List<Web.DTO.ContactRequirementsAlertDto> alerts = reader.GetAllContactRequirementAlerts();
                int alertId = alerts.Where(x => x.Min == null && x.Max == null).Select(x => x.Id).FirstOrDefault();
                var userList = reader.GetInadequateTestingUsers(14);
                int count = 0;
                if (userList.Count() > 0)
                {
                    foreach (int userId in userList)
                    {
                        ContactRequirement alert = new ContactRequirement
                        {
                            IsActive = true,
                            AlertId = alertId,
                            UserId = userId,
                            Type = 1,//1 Glucose, 2 Blood Pressure
                            CreatedOn = DateTime.UtcNow,
                        };
                        if (reader.AddContactRequirementAlert(alert))
                            count++;
                    }
                    if (count > 0)
                    {
                        LogReader logreader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "ContactRequirementManager", null, "Contact requirement alert created for " + count + " user's", null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }
    }
}
