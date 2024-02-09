using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO.AWV;
using Microsoft.AspNetCore.Identity;
using NLog;
using LogLevel = NLog.LogLevel;

namespace InterventWebApp
{
    public class AWVUtility
    {
        public static AnnualWellnessVisitStatus CreateAnualWellnessVisit(UserManager<ApplicationUser> userManager, PostAnualWellnessVisitDto request, string externalId, string clientId, string baseUrl)
        {
            AnnualWellnessVisitStatus status = new AnnualWellnessVisitStatus();
            status.Status = false;
            try
            {
                AWVReader reader = new AWVReader();

                status.ValidationErrors = request.Validate();
                if (status.ValidationErrors.Count <= 0)
                {
                    status.Token = reader.CreateWellnessVisit(userManager, request, externalId, clientId);
                    status.Status = !string.IsNullOrEmpty(status.Token);
                    if (status.Status)
                        status.Link = string.Format("{0}AWVReport/View/{1}", baseUrl, status.Token);
                }
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                string loggerName = "AWVController.CreateAnualWellnessVisit";
                var logEvent = new LogEventInfo(LogLevel.Error, loggerName, ex.Message);
                logEvent.Exception = ex;
                reader.WriteLogMessage(logEvent);
                status.Status = false;
            }
            return status;
        }

        public static GetTokenResponse GetToken(UserManager<ApplicationUser> userManager, string externalId, string clientId, string wellnessAssessmentDate, string baseUrl)
        {
            GetTokenResponse response = new GetTokenResponse();
            try
            {
                AWVReader reader = new AWVReader();
                if (string.IsNullOrEmpty(wellnessAssessmentDate))
                    response.ValidationError = "AssessmentDate missing";
                var assessmentDate = DateTime.Parse(wellnessAssessmentDate);
                string validationString;
                response.Token = reader.GetToken(userManager, externalId, clientId, assessmentDate, out validationString);
                if (!string.IsNullOrEmpty(response.Token))
                    response.Link = string.Format("{0}AWVReport/View/{1}", baseUrl, response.Token);
                response.ValidationError = validationString;
            }
            catch (Exception ex)
            {
                LogReader reader = new LogReader();
                string loggerName = "AWVController.GetToken";
                var logEvent = new LogEventInfo(LogLevel.Error, loggerName, ex.Message);
                logEvent.Exception = ex;
                reader.WriteLogMessage(logEvent);
                response.ValidationError = "Error in GetToken";
            }
            return response;
        }

        public static void SaveComments(int id, string comments)
        {
            AWVReader reader = new AWVReader();
            reader.SaveComments(id, comments);
        }

        public static GoalsDto GetAWVReportDetails(int Id)
        {
            AWVReader reader = new AWVReader();
            return reader.GetAWVReportDetails(Id);

        }

    }
}