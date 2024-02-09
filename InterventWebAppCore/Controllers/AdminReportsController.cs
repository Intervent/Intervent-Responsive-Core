using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Data;
using System.Text;

namespace InterventWebApp
{
    public class AdminReportsController : BaseController
    {
        ReportSelectModel model = new ReportSelectModel();
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Index()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult LogReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListLogReport(DateTime? StartDate, DateTime? EndDate, int page, int pageSize, int totalRecords, string level, string textsearch, bool download)
        {
            var timezone = User.TimeZone();
            var logReportResponse = AdminReportsUtility.ListLogReport(StartDate, EndDate, page, pageSize, totalRecords, timezone, level, textsearch, download);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                TotalRecords = logReportResponse.totalRecords,
                Records = logReportResponse.report.Select(x => new
                {
                    Id = x.Id,
                    Level = x.Level,
                    Message = x.Message,
                    Operation = x.Operation,
                    TimeStamp = x.timestamp.HasValue ? CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.timestamp.Value, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)) : null,
                    Logger = x.logger,
                    Exception = x.ExceptionType,
                    ExceptionMessage = x.ExceptionMessage,
                    StackTrace = x.StackTrace
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult EligibilityErrorReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.DateFormat = model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text).ToList();
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListEligibilityErrorReport(DateTime? StartDate, DateTime? EndDate, int page, int pageSize, int totalRecords, int? organization)
        {
            var timezone = User.TimeZone();
            var logReportResponse = AdminReportsUtility.ListEligibilityErrorReport(StartDate, EndDate, page, pageSize, totalRecords, timezone, organization, false);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                TotalRecords = logReportResponse.totalRecords,
                Records = logReportResponse.report.Select(x => new
                {
                    UniqueId = x.UniqueId,
                    LogDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.LogDate, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)),
                    Error = x.ErrorDetails,
                    Name = x.FirstName + " " + x.LastName
                })
            });
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult LabErrorLogReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DateFormat = model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListLabErrorLogReport(DateTime? StartDate, DateTime? EndDate, int? organization, int page, int pageSize, int totalRecords, bool download)
        {
            var timezone = User.TimeZone();
            bool isSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            var logReportResponse = AdminReportsUtility.ListLabErrorLogReport(StartDate, EndDate, organization, page, pageSize, totalRecords, timezone, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value, isSuperAdmin);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                TotalRecords = logReportResponse.totalRecords,
                Records = logReportResponse.report.ToList().Select(x => new
                {
                    Id = x.Id,
                    UserId = x.UserId.HasValue ? x.UserId.Value.ToString() : "N/A",
                    UniqueId = !string.IsNullOrEmpty(x.UniqueId) ? x.UniqueId : "N/A",
                    Name = x.Name,
                    Error = !string.IsNullOrEmpty(x.Error) ? x.Error.ToString() : "N/A",
                    FirstLogDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.FirstLogDate, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)),
                    LastLogDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.LogDate, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)),
                    Data = x.Data != null && x.Data.Length > 0 ? true : false
                })
            });
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ScreeningDataErrorLog()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DateFormat = model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListsSreeningDataErrorLogReport(DateTime? StartDate, DateTime? EndDate, int? organization, int page, int pageSize, int totalRecords, bool download)
        {
            var timezone = User.TimeZone();
            var logReportResponse = AdminReportsUtility.ListsSreeningDataErrorLogReport(StartDate, EndDate, organization, page, pageSize, totalRecords, timezone, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                TotalRecords = logReportResponse.totalRecords,
                Records = logReportResponse.report.Select(x => new
                {
                    Id = x.Id,
                    Organization = x.Organization.Name,
                    UniqueId = x.UniqueId,
                    Error = x.Error,
                    LogDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.Date, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat))
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult IncentiveReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            var OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Where(x => x.Portals.Count > 0 && (x.Name.StartsWith("Compass") || x.Name.StartsWith("Activate") || x.Name.StartsWith("South"))).ToList();
            Dictionary<int, string> portalList = new Dictionary<int, string>();
            foreach (var org in OrganizationList)
            {
                foreach (var portal in org.Portals)
                {
                    portalList.Add(portal.Id, portal.Name);
                }
            }
            model.PortalList = portalList.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() });
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public FileResult ListIncentiveReport(int PortalId, int filetype)
        {
            var logReportResponse = AdminReportsUtility.ListIncentiveReport(PortalId).report;
            var orgName = PortalUtility.ReadPortal(PortalId).portal;
            var csv = new StringBuilder();
            if (orgName.Name.IndexOf("South University", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                csv.AppendLine("First Name,Last Name,Unique Id,SSN");
                csv.AppendLine(String.Join(Environment.NewLine, logReportResponse.Select(x => x.Content)));
            }
            else
            {
                csv.AppendLine("H|" + orgName.Organization.Name + "|Incentive|" + DateTime.Now.Date.ToString("yyyyMMdd"));
                csv.AppendLine(String.Join(Environment.NewLine, logReportResponse.Select(x => x.Content)));
                csv.AppendLine("T|" + (orgName.Name.ToString().Contains("Activate") ? (logReportResponse.Count() - 1) : logReportResponse.Count()));
            }
            if (filetype == 1)
            {
                return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/plain", "Intervent_" + orgName.Organization.Name + "_Incentive.txt");
            }
            else
            {
                return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "Intervent_" + orgName.Organization.Name + "_Incentive.csv");
            }
        }


        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult LabAlertReport(bool? BackToReport)
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            if (BackToReport.HasValue && BackToReport.Value)
            {
                JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DateFormat = model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListLabAlert(int page, int pageSize, int totalRecords, DateTime? startDate, DateTime? endDate, int? userId, int alertType, int labsource, int? status, int? organization, bool download)
        {
            var timezone = User.TimeZone();
            ReportListModel model = new ReportListModel();
            model.page = page;
            model.pageSize = pageSize;
            model.totalRecords = totalRecords;
            var reportListResponse = AdminReportsUtility.ListLabAlert(model, startDate, endDate, userId, timezone, alertType, labsource, status, organization, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            FilterReportTempData filterReportTempData = new FilterReportTempData
            {
                page = page,
                totalRecords = totalRecords,
                startDate = startDate,
                endDate = endDate,
                userId = userId,
                alertType = alertType,
                labsource = labsource,
                organization = organization,
                Status = status
            };
            TempData["filterReportData"] = JsonConvert.SerializeObject(filterReportTempData);
            return Json(new
            {
                Result = "OK",
                TotalRecords = reportListResponse.totalRecords,
                Records = reportListResponse.listLabAlertReportResponse.Select(x => new
                {
                    UserId = x.UserId,
                    Name = x.User2.FirstName + " " + x.User2.LastName,
                    AlertDate = TimeZoneInfo.ConvertTimeFromUtc(x.DateCompleted.Value, custTZone).ToString(HttpContext.Session.GetString(SessionContext.DateFormat)),
                    CoachAlert = x.CoachAlert,
                    CriticalAlert = x.CriticalAlert,
                    ReviewedBy = x.ReviewedBy.HasValue ? x.User1.FirstName + " " + x.User1.LastName : "",
                    Source = string.IsNullOrEmpty(x.HL7) ? "Others" : "Lab",
                    Status = x.ReviewedBy.HasValue ? "Reviewed <i class='fa fa-info-circle has-tip tip-left' aria-hidden='true data-tooltip aria-haspopup='true' title='Reviewed By: " + x.User1.FirstName + " " + x.User1.LastName + ", Reviewed On: " + CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.ReviewedOn.Value, custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)) + "' ></i>"
                             : x.User2.Notes.Where(y => y.RefId == x.Id && y.Type == (int)NoteTypes.Critical_Alert).Count() > 0 ? "In Progress" : "Not Reviewed"
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult NoShowReport(bool? BackToReport)
        {
            if (BackToReport.HasValue && BackToReport.Value)
            {
                JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.AppointmentTypeList = SchedulerUtility.GetAppointmentType().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.TimeZoneList = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.TimeZoneId }).OrderBy(t => t.Text);
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ReviewNoShow(string appIds)
        {
            var response = AdminReportsUtility.ReviewNoShow(appIds, HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            return Json(new { Result = "OK" });
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListNoShowReport(int? orgId, int? ApptType, string timezone, bool? isReviewed, string language, DateTime? StartDate, DateTime? EndDate, int page, int pageSize, int totalRecords, bool download)
        {
            ReportListModel model = new ReportListModel();
            model.page = page;
            model.pageSize = pageSize;
            model.totalRecords = totalRecords;
            var timezoneDefault = User.TimeZone();
            var response = AdminReportsUtility.ListNoShowReport(orgId, ApptType, language, StartDate, EndDate, timezone, timezoneDefault, isReviewed, model, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            FilterReportTempData filterReportTempData = new FilterReportTempData
            {
                page = page,
                totalRecords = totalRecords,
                startDate = StartDate,
                endDate = EndDate,
                organization = orgId,
                appointmentType = ApptType,
                TimeZoneType = timezone,
                language = language,
                isReviewed = isReviewed.HasValue ? (isReviewed.Value ? "1" : "0") : ""
            };

            TempData["filterReportData"] = JsonConvert.SerializeObject(filterReportTempData);
            return Json(new
            {
                Result = "OK",
                TotalRecords = response.totalRecords,
                Records = response.report.Select(x => new
                {
                    x.Id,
                    UserId = x.UserId,
                    Name = x.User.FirstName + " " + x.User.LastName,
                    Language = x.User.LanguagePreference,
                    x.Minutes,
                    AppointmentType = x.AppointmentType.Type,
                    CoachName = x.User1 != null ? x.User1.FirstName + " " + x.User1.LastName : "",
                    AppointmentDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(x.Date), custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)),
                    ReviewdBy = x.NSHandledBy.HasValue ? x.User4.FirstName + " " + x.User4.LastName : "",
                    TextMessage = x.User.Text == 1 ? "Yes" : "No",
                    Organization = x.User.Organization.Name,
                    FutureAppt = x.FutureAppointmentDate.HasValue ? CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(x.FutureAppointmentDate), custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)) : "",
                })
            });
        }


        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult SmokingCessationIncentive()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            var OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Where(x => x.Portals.Count > 0).ToList();
            Dictionary<int, string> portalList = new Dictionary<int, string>();
            foreach (var org in OrganizationList)
            {
                foreach (var portal in org.Portals)
                {
                    portalList.Add(portal.Id, portal.Name);
                }
            }
            model.PortalList = portalList.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() });
            return View(model);
        }


        [ModuleControl(null, RoleCode.Administrator)]
        public FileResult ListSmokingCessationIncentive(int PortalId, int filetype)
        {
            var response = AdminReportsUtility.ListSmokingCessationIncentive(PortalId);
            var csv = new StringBuilder();
            csv.AppendLine("PortalName,UserId,UniqueID,FirstName,LastName,ReasonForPass,DatePassed,InSmokingProgram");
            if (response.report != null)
            {
                foreach (var data in response.report)
                {
                    csv.AppendLine(data.PortalName + "," + data.UserId + "," + data.UniqueID + "," + data.FirstName + "," + data.LastName + "," + data.ReasonForPass + "," + data.DatePassed + "," + data.InSmokingProgram);
                }
            }
            if (filetype == 1)
            {
                return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/plain", "_SmokCessInc.txt");
            }
            else
            {
                return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "_SmokCessInc.csv");
            }
        }
        [ModuleControl(null, RoleCode.Administrator)]
        public ActionResult PaymentTransactionReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.DateFormat = model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public JsonResult ListPaymentTransactionReport(int page, int pageSize, int totalRecords, DateTime? startDate, DateTime? endDate, string type, int? organization, bool download)
        {
            var timezone = User.TimeZone();
            ReportListModel model = new ReportListModel();
            model.page = page;
            model.pageSize = pageSize;
            model.totalRecords = totalRecords;
            var reportListResponse = AdminReportsUtility.ListPaymentTransactionReport(model, startDate, endDate, timezone, type, organization, download, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            return Json(new
            {
                Result = "OK",
                TotalRecords = reportListResponse.totalRecords,
                Records = reportListResponse.report.Select(x => new
                {
                    x.UserId,
                    Name = x.User.FirstName + " " + x.User.LastName,
                    x.Type,
                    x.RelatedId,
                    x.TransactionId,
                    Date = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(x.Date), custTZone), true, HttpContext.Session.GetString(SessionContext.DateFormat)),
                })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CoachTrackingReport(bool? BackToReport)
        {
            if (BackToReport.HasValue && BackToReport.Value)
            {
                JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            model.OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            model.CoachList = SchedulerUtility.GetCoachList(null, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId)).users.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() }).OrderBy(x => x.Text);
            return View(model);
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public FileResult DownloadCoachTrackingReport(int? coach, int? organization)
        {
            ReportListModel model = new ReportListModel();
            var reportListResponse = AdminReportsUtility.ListCoachTrackingReport(model, coach, organization, true, HttpContext.Session.GetInt32(SessionContext.AdminId).Value).coachTrackingRecords;
            var timezone = User.TimeZone();
            var csv = new StringBuilder();
            csv.AppendLine("Id," + "Name," + " Coach," + "Program," + "Comp. Calls," + "Rem. Calls," + "Follow-up," + "TimeZone");
            csv.AppendLine(String.Join(Environment.NewLine, reportListResponse.Select(x =>
                x.Id + "," +
                x.Name + "," +
                x.CoachName + "," +
                x.ProgramName + "," +
                x.CompletedCalls + "," +
                ((x.TotalCalls - x.CompletedCalls) >= 0 ? (x.TotalCalls - x.CompletedCalls) : 0) + "," +
                x.FollowUp + "," + x.Timezone
            )));
            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "CoachTrackingReport.csv");
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public FileResult DownloadLogReport(DateTime? StartDate, DateTime? EndDate, string level, string textsearch)
        {
            var timezone = User.TimeZone();
            var date = DateTime.Now.ToShortDateString();
            var reportListResponse = AdminReportsUtility.ListLogReport(StartDate, EndDate, 0, 0, 0, timezone, level, textsearch, true).report;
            var csv = new StringBuilder();
            csv.AppendLine("Id," + "Level," + " Message," + "Operation," + "Timestamp," + "Logger," + "Exception," + "ExceptionMessage");
            csv.AppendLine(String.Join(Environment.NewLine, reportListResponse.Select(x =>
                x.Id + "," +
                x.Level + "," +
                x.Message + "," +
                x.Operation + "," +
                x.timestamp + "," +
                x.logger + "," +
                x.ExceptionType + "," +
                x.ExceptionMessage
            )));
            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "SystemLog_" + date + ".csv");
        }

        [ModuleControl(null, RoleCode.Administrator)]
        public FileResult DownloadEligibilityErrorLogReport(DateTime? StartDate, DateTime? EndDate, int? organization)
        {
            var date = DateTime.Now.ToShortDateString();
            var timezone = User.TimeZone();
            var logReportResponse = AdminReportsUtility.ListEligibilityErrorReport(StartDate, EndDate, 0, 0, 0, timezone, organization, false);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var csv = new StringBuilder();
            csv.AppendLine("UniqueId," + "LogDate," + "Error," + "Name");
            csv.AppendLine(String.Join(Environment.NewLine, logReportResponse.report.Select(x =>
                x.UniqueId + "," +
                x.LogDate + "," +
                x.ErrorDetails.Replace("\n", "").Replace("\r", "") + "," +
                x.FirstName
            )));
            return File(new UTF8Encoding().GetBytes(csv.ToString()), "text/csv", "eligibilityerrorlog_" + date + ".csv");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListCoachTrackingReport(int page, int pageSize, int totalRecords, int? coach, int? organization)
        {
            ReportListModel model = new ReportListModel();
            model.page = page;
            model.pageSize = pageSize;
            model.totalRecords = totalRecords;
            var reportListResponse = AdminReportsUtility.ListCoachTrackingReport(model, coach, organization, false, HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            FilterReportTempData filterReportTempData = new FilterReportTempData
            {
                page = page,
                totalRecords = totalRecords,
                organization = organization,
                coach = coach,
                forDownload = false
            };
            TempData["filterReportData"] = JsonConvert.SerializeObject(filterReportTempData);
            return Json(new
            {
                Result = "OK",
                TotalRecords = reportListResponse.totalRecords,
                Records = reportListResponse.coachTrackingRecords
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult KitUserReport()
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            var OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Where(x => x.Portals.Count > 0).ToList();
            Dictionary<int, string> kitsList = new Dictionary<int, string>();
            Dictionary<int, string> portalList = new Dictionary<int, string>();
            foreach (var org in OrganizationList)
            {
                foreach (var portal in org.Portals)
                {
                    portalList.Add(portal.Id, portal.Name);
                }
            }
            model.PortalList = portalList.OrderBy(x => x.Value).Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() });
            model.KitsInPortalList = kitsList.Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() });
            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListKitsInPortal(int? PortalId)
        {
            var kitList = PortalUtility.ListKitsforPortal(PortalId ?? 0);
            return Json(new
            {
                Result = "OK",
                Records = kitList.OrderBy(x => x.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() })
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public FileResult DownloadKitUserReport(int portalId, int kitId)
        {
            var reportListResponse = AdminReportsUtility.ListKitUserReport(portalId, kitId);
            //
            return File(new UTF8Encoding().GetBytes(reportListResponse.ToString()), "text/csv", "KitUserReport.csv");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult UnapprovedCarePlans(bool? BackToReport)
        {
            model.IsSuperAdmin = CommonUtility.IsSuperAdmin(User.RoleCode());
            if (BackToReport.HasValue && BackToReport.Value)
            {
                model.tempData = JsonConvert.DeserializeObject<FilterReportTempData>(TempData["filterReportData"] as string);
            }
            var OrganizationList = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Where(x => x.Portals.Count > 0).ToList();
            Dictionary<int, string> portalList = new Dictionary<int, string>();
            foreach (var org in OrganizationList)
            {
                foreach (var portal in org.Portals.Where(x => x.CarePlan))
                {
                    portalList.Add(portal.Id, portal.Name);
                }
            }
            model.PortalList = portalList.OrderBy(x => x.Value).Select(x => new SelectListItem { Text = x.Value, Value = x.Key.ToString() });
            model.AssessmentList = CommonUtility.GetAssessmentTypes().OrderBy(x => x.Value).Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); ;
            model.UserStautsList = ListOptions.GetUserStatusList().OrderBy(x => x.Value).Select(x => new SelectListItem { Text = x.Text, Value = x.Value.ToString() }); ;

            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult UnapprovedCarePlanList(int page, int pageSize, int totalRecords, int? poralId, int? assessmentType, string userStatus, bool download)
        {
            var reportListResponse = AdminReportsUtility.ListUnapprovedCarePlans(page, pageSize, totalRecords, HttpContext.Session.GetInt32(SessionContext.UserId).Value, poralId, assessmentType, userStatus, download);
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
            var careplans = reportListResponse.UnapprovedCarePlans.Select(x => new
            {
                UserId = x.userId,
                UserName = x.userName,
                UserStatus = string.IsNullOrEmpty(x.uniqueId) ? "N/A" : ParticipantUtility.GetEligibilityFromMultiplePortals(null, x.uniqueId, reportListResponse.portalIds).Eligibility.UserStatus.Description,
                AssessmentType = x.reportType == (byte)ReportTypes.HRA ? "HRA" : "Follow-Up (" + CommonUtility.GetFollowUpType(x.refId, x.usersinProgramId) + ")",
                CompletedDate = CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(Convert.ToDateTime(x.completedDate), custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)),
            }).ToList();
            FilterReportTempData filterReportTempData = new FilterReportTempData
            {
                page = page,
                totalRecords = totalRecords,
                portalId = poralId,
                assessmentType = assessmentType,
                userStatus = userStatus
            };
            TempData["filterReportData"] = JsonConvert.SerializeObject(filterReportTempData);
            return Json(new
            {
                Result = "OK",
                TotalRecords = reportListResponse.totalRecords,
                Records = careplans
            });
        }
    }
}