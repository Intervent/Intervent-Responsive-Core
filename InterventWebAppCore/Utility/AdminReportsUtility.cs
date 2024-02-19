using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using System.Text;

namespace InterventWebApp
{
    public class AdminReportsUtility
    {
        public static ListLogResponse ListLogReport(DateTime? StartDate, DateTime? EndDate, int page, int pageSize, int totalRecords, string timezone, string level, string textsearch, bool download)
        {
            LogReader reader = new LogReader();
            LogReportRequest request = new LogReportRequest();
            request.startDate = StartDate;
            request.endDate = EndDate;
            request.level = level;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = page;
                request.pageSize = pageSize;
                request.totalRecords = totalRecords;
            }
            request.timezone = timezone;
            request.textsearch = textsearch;
            return reader.ListLogReport(request);
        }

        public static ClaimsErrorLogResponse ListEligibilityErrorReport(DateTime? StartDate, DateTime? EndDate, int page, int pageSize, int totalRecords, string timezone, int? organization, bool download)
        {
            AdminReportsReader reader = new AdminReportsReader();
            LogReportRequest request = new LogReportRequest();
            request.startDate = StartDate;
            request.endDate = EndDate;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = page;
                request.pageSize = pageSize;
                request.totalRecords = totalRecords;
            }
            request.timezone = timezone;
            request.organization = organization;
            return reader.ListEligibilityErrorReport(request);
        }

        public static LabErrorLogResponse ListLabErrorLogReport(DateTime? StartDate, DateTime? EndDate, int? organization, int page, int pageSize, int totalRecords, string timezone, bool download, int userId, bool isSuperAdmin)
        {
            AdminReportsReader reader = new AdminReportsReader();
            LabErrorLogRequest request = new LabErrorLogRequest();
            request.AdminId = userId;
            request.startDate = StartDate;
            request.endDate = EndDate;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = page;
                request.pageSize = pageSize;
                request.totalRecords = totalRecords;
            }
            request.timezone = timezone;
            request.Organization = organization;
            request.IsSuperAdmin = isSuperAdmin;
            return reader.ListLabErrorLogReport(request);
        }

        public static SreeningDataErrorLogResponse ListsSreeningDataErrorLogReport(DateTime? StartDate, DateTime? EndDate, int? organization, int page, int pageSize, int totalRecords, string timezone, bool download, int userId)
        {
            LogReader reader = new LogReader();
            LabErrorLogRequest request = new LabErrorLogRequest();
            request.AdminId = userId;
            request.startDate = StartDate;
            request.endDate = EndDate;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = page;
                request.pageSize = pageSize;
                request.totalRecords = totalRecords;
            }
            request.timezone = timezone;
            request.Organization = organization;
            return reader.ListsSreeningDataErrorLogReport(request);
        }

        public static IncentiveReportResponse ListIncentiveReport(int PortalId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            IncentiveReportRequest request = new IncentiveReportRequest();
            request.PortalId = PortalId;
            return reader.ListIncentiveReport(request);
        }

        public static ListLabAlertResponse ListLabAlert(ReportListModel model, DateTime? StartDate, DateTime? EndDate, int? userId, string timezone, int alertType, int labsource, int? status, int? organization, bool download, int adminId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            ListLabAlertRequest request = new ListLabAlertRequest();
            request.AdminId = adminId;
            request.userId = userId;
            request.StartDate = StartDate;
            request.EndDate = EndDate;
            request.alerttype = alertType;
            request.labsource = labsource;
            request.status = status;
            request.timezone = timezone;
            if (download)
            {
                request.Page = 1;
                request.PageSize = 0;
                request.TotalRecords = 0;
            }
            else
            {
                request.Page = model.page;
                request.PageSize = model.pageSize;
                request.TotalRecords = model.totalRecords;
            }
            request.Organization = organization;
            return reader.ListLabAlert(request);
        }

        public static ReviewNoShowResponse ReviewNoShow(string appIds, int adminId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            ReviewNoShowRequest request = new ReviewNoShowRequest();
            request.ApptIds = appIds;
            request.AdminId = adminId;
            return reader.ReviewNoShow(request);
        }

        public static NoShowApptReportResponse ListNoShowReport(int? orgId, int? ApptType, string language, DateTime? StartDate, DateTime? EndDate, string timezone, string timeZoneDefault, bool? isReviewed, ReportListModel model, bool download, int userId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            NoShowApptReportRequest request = new NoShowApptReportRequest();
            request.orgId = orgId;
            request.ApptType = ApptType;
            request.language = language;
            request.startDate = StartDate;
            request.endDate = EndDate;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = model.page;
                request.pageSize = model.pageSize;
                request.totalRecords = model.totalRecords;
            }
            request.timezone = timezone;
            request.timeZoneDefault = timeZoneDefault;
            request.isReviewed = isReviewed;
            request.AdminId = userId;
            var response = reader.ListNoShowReport(request);
            return response;
        }

        public static SmokingCessationIncentiveResponse ListSmokingCessationIncentive(int PortalId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            var response = reader.ListSmokingCessationIncentive(new SmokingCessationIncentiveRequest { PortalId = PortalId });
            return response;
        }

        public static ListPaymentTransactionResponse ListPaymentTransactionReport(ReportListModel model, DateTime? StartDate, DateTime? EndDate, string timezone, string type, int? organization, bool download, int userId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            ListPaymentTransactionRequest request = new ListPaymentTransactionRequest();
            request.orgId = organization;
            request.timezone = timezone;
            request.type = type;
            request.startDate = StartDate;
            request.endDate = EndDate;
            if (download)
            {
                request.page = 1;
                request.pageSize = 0;
                request.totalRecords = 0;
            }
            else
            {
                request.page = model.page;
                request.pageSize = model.pageSize;
                request.totalRecords = model.totalRecords;
            }
            request.AdminId = userId;
            var response = reader.ListPaymentTransactionReport(request);
            return response;
        }

        public static CoachTrackingResponse ListCoachTrackingReport(ReportListModel model, int? coach, int? organization, bool forDownload, int adminId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            ListCoachTrackingRequest request = new ListCoachTrackingRequest();
            request.orgId = organization;
            request.coach = coach;
            request.page = model.page;
            request.pageSize = model.pageSize;
            request.totalRecords = model.totalRecords;
            request.forDownload = forDownload;
            request.AdminId = adminId;
            var response = reader.ListCoachTrackingReport(request);
            return response;
        }

        public static StringBuilder ListKitUserReport(int portalId, int kitId)
        {
            AdminReportsReader reader = new AdminReportsReader();
            KitUserReportRequest request = new KitUserReportRequest();
            request.kitId = kitId;
            request.portalId = portalId;
            request.timezone = System.Web.HttpContext.Current.User.TimeZone().ToString();
            return reader.ListKitUserReport(request).KitsPortalReport;

        }

        public static ListUnapprovedCarePlanResponse ListUnapprovedCarePlans(int page, int pageSize, int totalRecords, int userId, int? poralId, int? assessmentType, string userStatus, bool download)
        {
            AdminReportsReader reader = new AdminReportsReader();
            UnapprovedCarePlanRequest request = new UnapprovedCarePlanRequest();
            request.userId = userId;
            request.poralId = poralId;
            request.assessmentType = assessmentType;
            request.userStatus = userStatus;
            if (download)
            {
                request.Page = 1;
                request.PageSize = 0;
                request.TotalRecords = 0;
            }
            else
            {
                request.Page = page;
                request.PageSize = pageSize;
                request.TotalRecords = totalRecords;
            }
            return reader.ListUnapprovedCarePlans(request);
        }
        
    }
}