using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using NLog;
using NLog.Targets;

namespace Intervent.Web.DataLayer
{
    public class LogReader : TargetWithLayout
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public void WriteLogMessage(LogEventInfo logEvent)
        {
            var message = Layout.Render(logEvent);
            var messageElements = message.Split('|');
            if (messageElements.Length == 4 || messageElements.Length == 8)
            {
                DAL.Log log = new DAL.Log();
                log.TimeStamp = DateTime.UtcNow;
                log.Level = messageElements[1];
                log.Logger = messageElements[2]; // userId
                log.Message = messageElements[3]; //controller and action name

                if (logEvent.Exception != null && messageElements.Length > 4)
                {
                    log.ExceptionType = messageElements[4]; //Exception type
                    log.Operation = messageElements[5];
                    log.ExceptionMessage = messageElements[6]; //Exception message
                    log.StackTrace = messageElements[7];  //Exception 
                }
                else if (logEvent.Exception != null)
                {
                    log.ExceptionMessage = logEvent.Exception.InnerException != null ? logEvent.Exception.InnerException.Message : logEvent.Exception.Message;
                    log.ExceptionType = logEvent.Exception.GetType().ToString();
                    var site = logEvent.Exception.TargetSite;
                    log.Operation = site != null ? site.Name : null;
                    log.StackTrace = logEvent.Exception.StackTrace;
                }
                context.Logs.Add(log);
                context.SaveChanges();
            }
        }


        public ListLogResponse ListLogReport(LogReportRequest request)
        {
            ListLogResponse response = new ListLogResponse();
            bool download = false;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.Logs.Where(x => (String.IsNullOrEmpty(request.textsearch) || x.Message.Contains(request.textsearch)) && (String.IsNullOrEmpty(request.level) || x.Level == request.level) && x.TimeStamp > startdate && x.TimeStamp < enddate).OrderByDescending(x => x.Id).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var logDAL = context.Logs.Where(x => (String.IsNullOrEmpty(request.textsearch) || x.Message.Contains(request.textsearch)) && (String.IsNullOrEmpty(request.level) || x.Level == request.level) && x.TimeStamp > startdate && x.TimeStamp < enddate).OrderByDescending(x => x.Id).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.report = Utility.mapper.Map<IList<DAL.Log>, IList<ListLogReportDto>>(logDAL);
            response.totalRecords = totalRecords;
            return response;
        }
        public bool AddScreeningData(Dictionary<string, string> Errors, int orgId)
        {
            foreach (var error in Errors)
            {
                DAL.ScreeningDataErrorLog screeningdata = new DAL.ScreeningDataErrorLog();
                screeningdata.UniqueId = error.Key;
                screeningdata.Error = error.Value;
                screeningdata.OrgId = orgId;
                screeningdata.Date = DateTime.UtcNow;
                context.ScreeningDataErrorLogs.Add(screeningdata);
                context.SaveChanges();
            }
            return true;
        }
        public SreeningDataErrorLogResponse ListsSreeningDataErrorLogReport(LabErrorLogRequest request)
        {
            SreeningDataErrorLogResponse response = new SreeningDataErrorLogResponse();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            PortalReader reader = new PortalReader();
            var download = false;
            var organizationsList = reader.GetFilteredOrganizationsList(request.AdminId).Organizations.Select(x => x.Id).ToArray();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var startdate = request.startDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone) : System.DateTime.MinValue;
            var enddate = request.endDate.HasValue ? TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone).AddDays(1) : System.DateTime.MaxValue;
            if (totalRecords == 0)
            {
                totalRecords = context.ScreeningDataErrorLogs.Where(x => (!request.Organization.HasValue || x.OrgId == request.Organization) && x.Date > startdate && x.Date < enddate && organizationsList.Contains(x.OrgId)).OrderByDescending(x => x.Id).Count();
                if (request.pageSize == 0)
                {
                    request.pageSize = totalRecords;
                    download = true;
                }
            }
            var logDAL = context.ScreeningDataErrorLogs.Include("Organization").Where(x => (!request.Organization.HasValue || x.OrgId == request.Organization) && x.Date > startdate && x.Date < enddate && organizationsList.Contains(x.OrgId)).OrderByDescending(x => x.Id).Skip((download ? 0 : request.page * request.pageSize)).Take(request.pageSize).ToList();
            response.report = Utility.mapper.Map<IList<DAL.ScreeningDataErrorLog>, IList<ScreeningDataErrorLogDto>>(logDAL);
            response.totalRecords = totalRecords;
            return response;
        }
        public void AddTransactionDetails(AddPaymentTransactionRequest request)
        {
            DAL.PaymentTransaction transaction = new DAL.PaymentTransaction();
            transaction.UserId = request.UserId;
            transaction.RelatedId = request.RelatedId;
            transaction.TransactionId = request.TransactionId;
            transaction.Type = request.Type;
            transaction.Date = DateTime.UtcNow;
            context.PaymentTransactions.Add(transaction);
            context.SaveChanges();
        }
    }
}