using Intervent.DAL;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;

namespace InterventWebApp
{
    public class SchedulerController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly UserManager<ApplicationUser> _userManager;

        public SchedulerController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
        }

        #region Scheduling by User

        [Authorize]
        public JsonResult CheckFreeSlot(int year, int month, int? coachId, int? time, string timeZone, int? apptId)
        {
            var days = DateTime.DaysInMonth(year, month);
            var startDate = new DateTime(year, month, 1);
            bool thirtyMinutes = true;
            if (apptId.HasValue)
            {
                var appointmentResponse = SchedulerUtility.GetAppointmentDetails(apptId.Value, timeZone);
                if (appointmentResponse != null && appointmentResponse.appointment != null)
                {
                    thirtyMinutes = appointmentResponse.appointment.Minutes == 30 ? true : false;
                }
            }
            else if (IsRescheduling())
            {
                thirtyMinutes = false;
            }
            var endDate = new DateTime(year, month, days).AddDays(1);
            string languages = null;
            if (time.HasValue)
            {
                startDate = startDate.AddHours(time.Value);
                endDate = endDate.AddHours(time.Value).AddHours(3);
            }
            var response = SchedulerUtility.CheckFreeSlot(startDate, endDate, coachId, string.IsNullOrEmpty(timeZone) ? User.TimeZone() : timeZone, languages, thirtyMinutes, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId).Value, HttpContext.Session.GetString(SessionContext.isPregnant) == null ? Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.isPregnant)) : false);
            var portalResponse = PortalUtility.ReadPortal(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value));
            if (portalResponse.portal != null && portalResponse.portal.NoProgDays.HasValue && response.AvailabilityCount != null && !IsRescheduling())
            {
                DateTime hideFrom = DateTime.Parse(portalResponse.portal.EndDate).AddDays(-(portalResponse.portal.NoProgDays.Value + 1));
                DateTime hideTo = DateTime.Parse(portalResponse.portal.EndDate).AddDays(1);
                return Json(new { Result = "OK", Records = response.AvailabilityCount.Where(x => x.Date <= hideFrom || x.Date >= hideTo) });
            }
            return Json(new { Result = "OK", Records = response.AvailabilityCount });
        }

        [Authorize]
        public JsonResult GetFreeSlotforDay(DateTime startDate, int? coachId, int? time, string timeZone, bool? thirtyMinutes, int? apptId)
        {
            if (apptId.HasValue)
            {
                var appointmentResponse = SchedulerUtility.GetAppointmentDetails(apptId.Value, timeZone);
                if (appointmentResponse != null && appointmentResponse.appointment != null)
                {
                    thirtyMinutes = appointmentResponse.appointment.Minutes == 30 ? true : false;
                }
            }
            else if (!thirtyMinutes.HasValue)
                thirtyMinutes = false;
            var endDate = startDate;
            if (time.HasValue)
            {
                startDate = startDate.AddHours(time.Value);
                endDate = endDate.AddHours(time.Value).AddHours(3);
            }
            else
                endDate = startDate.AddDays(1);
            string languages = null;
            var response = SchedulerUtility.GetFreeSlotforDay(startDate, endDate, coachId, string.IsNullOrEmpty(timeZone) ? User.TimeZone() : timeZone, languages, thirtyMinutes.Value, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId).Value, HttpContext.Session.GetString(SessionContext.isPregnant) == null ? Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.isPregnant)) : false).Availabilities.Distinct().OrderBy(x => x.StartTime).ToList();
            var dateList = response.Select(s => s.StartTime.ToString()).Distinct().ToList(); var coachCount = response.Select(s => s.CoachId).Distinct().Count();
            return Json(new { Result = "OK", Records = dateList, coachCount = coachCount });
        }

        [Authorize]
        [HttpPost]
        public JsonResult ScheduleAppointment(int? apptId, string Date, int coachId, byte? minutes, string comments, string timeZone)
        {
            int createdBy;
            if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue && (HttpContext.Session.GetInt32(SessionContext.AdminId).Value != HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value))
                createdBy = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            else
                createdBy = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            int? apptType = null;
            if (apptId.HasValue)
            {
                var appointmentResponse = SchedulerUtility.GetAppointmentDetails(apptId.Value, timeZone);
                if (appointmentResponse != null && appointmentResponse.appointment != null)
                {
                    minutes = appointmentResponse.appointment.Minutes;
                }
                apptType = appointmentResponse.appointment.Type;
            }
            else if (!minutes.HasValue)
                minutes = 15;
            if (!apptType.HasValue && minutes == 15)
                apptType = 2;
            else
                apptType = 1;
            var response = SchedulerUtility.ScheduleAppointment(null, Convert.ToDateTime(Date), apptType, coachId, minutes.Value, comments, createdBy, participantId, string.IsNullOrEmpty(timeZone) ? User.TimeZone() : timeZone, false, false, "", HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue ? HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value : null, HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue ? HttpContext.Session.GetInt32(SessionContext.ProgramType).Value : null, HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.HasHRA).HasValue ? HttpContext.Session.GetInt32(SessionContext.HasHRA).Value : null, Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.AssignPrograms) != null ? HttpContext.Session.GetString(SessionContext.AssignPrograms) : false), HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber), _appSettings.SouthUniversityOrgId);
            if (response.Status != 2)
            {
                if (apptId.HasValue)
                {
                    SchedulerUtility.CancelAppointment(apptId.Value, 3, "", Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value), HttpContext.Session.GetInt32(SessionContext.AdminId).Value, _appSettings.IntuityURL, _appSettings.IntuityAPIKey);
                    if (DateTime.Parse(HttpContext.Session.GetString(SessionContext.NextApptDate)) > Convert.ToDateTime(Date))
                    {
                        HttpContext.Session.SetString(SessionContext.NextApptDate, Date.ToString());
                        HttpContext.Session.SetInt32(SessionContext.NextApptId, response.apptId);
                    }
                }
                else
                {
                    HttpContext.Session.SetString(SessionContext.NextApptDate, Date.ToString());
                    HttpContext.Session.SetInt32(SessionContext.NextApptId, response.apptId);
                }
            }
            if (response.enrollinProgramResponse != null && response.enrollinProgramResponse.success)
            {
                HttpContext.Session.SetInt32(SessionContext.UserinProgramId, response.enrollinProgramResponse.UsersinProgramId);
                HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, response.enrollinProgramResponse.ProgramsInPortalId);
                HttpContext.Session.SetInt32(SessionContext.ProgramType, response.enrollinProgramResponse.ProgramType);
            }
            return Json(response);
        }


        [Authorize]
        public ActionResult Reschedule()
        {
            RescheduleModel model = new RescheduleModel();
            model.BaseUrl = _appSettings.EmailUrl;
            if (!IsRescheduling())
                return RedirectToAction("MyCoach", "Participant");
            if (HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue)
            {
                var UsersinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
                model.coach = ProgramUtility.GetUserinProgramDetails(UsersinProgramId).User1;
            }
            else if (HttpContext.Session.GetInt32(SessionContext.NextApptId).HasValue)
            {
                string timeZone = "";
                if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                    timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
                var appointmentResponse = SchedulerUtility.GetAppointmentDetails(HttpContext.Session.GetInt32(SessionContext.NextApptId).Value, timeZone);
                if (appointmentResponse != null && appointmentResponse.appointment != null)
                    model.coach = appointmentResponse.appointment.User1;
            }
            if (model.coach != null)
            {
                model.Languages = String.Join(", ", model.coach.Languages.Select(x => Translate.Message(x.LanguageItem)).ToList());
                model.Speciality = String.Join(", ", model.coach.Specializations.Select(x => Translate.Message(x.LanguageId)).ToList());
            }
            return View(model);
        }

        [Authorize]
        public ActionResult RescheduleCalendar(int coachId, int? apptId)
        {
            MyCoachModel model = new MyCoachModel();
            model.apptId = apptId;
            model.coachId = coachId;
            model.Timezones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZone1), Value = x.TimeZoneId });
            model.participantTimeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith);
            model.BaseUrl = _appSettings.EmailUrl;
            return PartialView("_RescheduleCalendar", model);
        }

        #endregion

        #region Scheduling by Admin

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetAppointments(DateTime startDate, DateTime endDate, int? coachId)
        {
            var response = SchedulerUtility.GetAppointments(startDate, endDate, coachId, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            return Json(new { Result = "OK", Records = response.Appointments });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetAppointmentsCount(int year, int month, int? coachId)
        {
            var days = DateTime.DaysInMonth(year, month);
            var startDate = new DateTime(year, month, 1);
            var endDate = new DateTime(year, month, days).AddDays(1);
            var response = SchedulerUtility.GetAppointmentsCount(startDate, endDate, coachId, User.TimeZone());
            return Json(new { Result = "OK", Records = response.appointmentsCount });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult SearchFreeSlot()
        {
            SearchFreeSlotModel model = new SearchFreeSlotModel();
            model.StartDate = DateTime.Now.AddDays(1).ToShortDateString();
            model.EndDate = Convert.ToDateTime(model.StartDate).AddDays(8).ToShortDateString();
            var coachResponse = SchedulerUtility.GetCoachList(true, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId));
            var list = (from coach in coachResponse.users
                        select new
                        {
                            Id = coach.Id,
                            Text = coach.FirstName + " " + coach.LastName,
                            Category = coach.RoleCode
                        }).ToArray();
            model.Coaches = new SelectList(list, "Id", "Text", 0, "Category");
            model.TimeZones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.TimeZoneId.ToString() })
               .OrderBy(t => t.Text);
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                model.TimeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            model.Times = CommonUtility.GetPreferredTime();
            model.DaysList = CommonUtility.GetDayOfWeek();
            model.Specializations = AccountUtility.ListSpecialization(false, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageId), Value = x.Id.ToString() });
            model.Languages = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_SearchFreeSlot", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult SearchFreeSlot(DateTime StartDateTime, DateTime EndDateTime, string startTime, int? coachId, string TimeZone, string day, bool thirtyMinutes,
            bool? video, bool? healthcoach, string specialities = null, string languages = null)
        {
            if (!string.IsNullOrEmpty(startTime))
            {
                EndDateTime = EndDateTime.AddHours(3);
                EndDateTime = EndDateTime.AddDays(1);
            }
            else
            {
                EndDateTime = EndDateTime.AddDays(1);
            }
            var response = SchedulerUtility.SearchFreeSlot(StartDateTime, EndDateTime, coachId, string.IsNullOrEmpty(TimeZone) ? User.TimeZone() : TimeZone, day, thirtyMinutes, video, healthcoach, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId).Value, specialities, languages);
            var finalTimeZone = "";
            if (TimeZone != "")
                finalTimeZone = TimeZone;
            else
                finalTimeZone = User.TimeZone();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(finalTimeZone);
            var startDate = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone);
            response.Availabilities.RemoveAll(x => Convert.ToDateTime(x.AvailDate).Date >= EndDateTime.Date);
            response.Availabilities.RemoveAll(x => x.StartTime <= startDate);
            if (response.Availabilities.Count > 1000)
                response.Availabilities.RemoveRange(1000, response.Availabilities.Count() - 1000);
            return Json(new { Result = "OK", Records = response.Availabilities });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public async Task<ActionResult> ScheduleAppointmentByAdmin(int coachId, int minutes, string startTime, string timeZone)
        {
            ScheduleAppointmentModel model = new ScheduleAppointmentModel();
            model.Lengths = SchedulerUtility.GetAppointmentLength();
            model.Types = SchedulerUtility.GetAppointmentType().Select(x => new SelectListItem { Text = x.Type, Value = x.Id.ToString() });
            var response = await AccountUtility.GetUser(_userManager, null, null, coachId, null, null);
            model.CoachName = response.User.FirstName + " " + response.User.LastName;
            model.CoachID = coachId;
            model.AppointmentDate = startTime;
            model.Length = minutes;
            model.TimeZone = timeZone;
            model.MeetingId = response.User.AdminProperty.Video.Value ? response.User.AdminProperty.MeetingId : "";
            return PartialView("_ScheduleAppointmentByAdmin", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult ScheduleAppointmentByAdmin(string Date, int coachId, byte type, byte minutes, string comments, string timeZone, bool videoRequired, string meetingId)
        {
            var createdBy = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            var userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var response = SchedulerUtility.ScheduleAppointment(null, Convert.ToDateTime(Date), type, coachId, minutes, comments, createdBy, userId, string.IsNullOrEmpty(timeZone) ? User.TimeZone() : timeZone, true, videoRequired, meetingId, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue ? HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value : null, HttpContext.Session.GetInt32(SessionContext.ProgramType).HasValue ? HttpContext.Session.GetInt32(SessionContext.ProgramType).Value : null, HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.HasHRA).HasValue ? HttpContext.Session.GetInt32(SessionContext.HasHRA).Value : null, Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.AssignPrograms) != null ? HttpContext.Session.GetString(SessionContext.AssignPrograms) : false), HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber), _appSettings.SouthUniversityOrgId);
            if (response.enrollinProgramResponse != null && response.enrollinProgramResponse.success)
            {
                HttpContext.Session.SetInt32(SessionContext.UserinProgramId, response.enrollinProgramResponse.UsersinProgramId);
                HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, response.enrollinProgramResponse.ProgramsInPortalId);
                HttpContext.Session.SetInt32(SessionContext.ProgramType, response.enrollinProgramResponse.ProgramType);
            }
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CancelAppointment(int appId, bool? getappt)
        {
            CancelAppointmentModel model = new CancelAppointmentModel();
            model.Id = appId;
            model.getAppt = getappt.HasValue ? true : false;
            model.reasons = SchedulerUtility.GetCancellationReasons(appId).Select(x => new SelectListItem { Text = x.Reason, Value = x.Id.ToString() });
            return PartialView("_CancelAppointment", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public ActionResult CancelAppointment(int id, byte reason, string comments)
        {
            var response = SchedulerUtility.CancelAppointment(id, reason, comments, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, _appSettings.IntuityURL, _appSettings.IntuityAPIKey);
            //deactivate dashboard message
            if (response == true && reason != 5)
            {
                ParticipantUtility.UpdateDashboardMessage(null, id, 13, null, false);
            }
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult EditAppointmentComments(int appId, string comments, int Length, string meetingId, bool videoRequired)
        {
            EditAppointmentCommentsModel model = new EditAppointmentCommentsModel();
            model.Id = appId;
            model.comments = comments;
            model.Lengths = SchedulerUtility.GetAppointmentLength();
            model.Length = Length;
            model.MeetingId = meetingId;
            model.VideoRequired = videoRequired;
            return PartialView("_EditAppointmentComments", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public ActionResult EditAppointmentComment(int id, string comments, int Length, bool VideoRequired, string MeetingId)
        {
            var response = SchedulerUtility.EditAppointment(id, comments, Length, VideoRequired, MeetingId, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId), HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName), HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber));
            return Json(response.success);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public ActionResult RevertNoShow(int id)
        {
            var response = SchedulerUtility.RevertNoShow(id);
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult Appointments()
        {
            AppointmenstModel model = new AppointmenstModel();
            var response = SchedulerUtility.GetAppointments(null, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            model.appointments = response.Appointments;
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            if (HttpContext.Session.GetInt32(SessionContext.UserinProgramId).HasValue)
                model.userinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView("_Appointments", model);
        }

        #endregion

        #region Set/Cancel Coach Availability

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult SetCoachAvailability(int? coachId, string startDate)
        {
            if (string.IsNullOrEmpty(startDate))
            {
                ViewBag.Year = DateTime.Now.Year;
                ViewBag.Month = DateTime.Now.Month - 1;
                ViewBag.Day = DateTime.Now.Day;
            }
            else
            {
                var timeStart = Convert.ToDateTime(startDate);
                ViewBag.Year = timeStart.Year;
                ViewBag.Month = timeStart.Month - 1;
                ViewBag.Day = timeStart.Day;

            }
            int id = 0;
            if (coachId.HasValue)
                id = coachId.Value;
            ViewBag.CoachId = id;
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetCoachAvailability(int coachId)
        {
            var response = SchedulerUtility.GetCoachAvailability(coachId);
            var jsonResult = Json(response);
            return jsonResult;
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetCoachAvailabilityDetails(long refId, string startDate)
        {
            var response = SchedulerUtility.GetCoachAvailabilityDetails(refId, startDate);
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult SetCoachAvailability(string startDateTime, string endDateTime, string Days, DateTime? fromDate, DateTime? toDate, int coachId)
        {
            var timeStart = Convert.ToDateTime(startDateTime);
            var timeEnd = Convert.ToDateTime(endDateTime);
            ViewBag.Year = timeStart.Year;
            ViewBag.Month = timeStart.Month - 1;
            ViewBag.Day = timeStart.Day;
            var response = SchedulerUtility.SetCoachAvailability(timeStart, timeEnd, Days, fromDate, toDate, coachId);
            return Json(response);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult DeleteCoachAvailability(long refId, DateTime startTime, DateTime endTime, DateTime? toDate, bool allFuture, string days)
        {
            var timeStart = Convert.ToDateTime(startTime);
            ViewBag.Year = timeStart.Year;
            ViewBag.Month = timeStart.Month - 1;
            ViewBag.Day = timeStart.Day;
            var response = SchedulerUtility.DeleteCoachAvailability(refId, startTime, endTime, toDate, allFuture, days);
            return Json(response);
        }

        #endregion

        #region Coach Calendar

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CoachCalendar(int? coachId)
        {
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.Month = DateTime.Now.Month - 1;
            ViewBag.Day = DateTime.Now.Day;
            int id = 0;
            if (coachId.HasValue)
                id = coachId.Value;
            ViewBag.CoachId = id;
            return View();
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CalendarSearch(int? coachId)
        {
            ViewBag.Year = DateTime.Now.Year;
            ViewBag.Month = DateTime.Now.Month - 1;
            ViewBag.Day = DateTime.Now.Day;
            int id = 0;
            if (coachId.HasValue)
                id = coachId.Value;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName)))
                ViewBag.TimeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName);
            else
                ViewBag.TimeZone = System.Web.HttpContext.Current.User.TimeZone().ToString();
            ViewBag.CoachId = id;
            return PartialView("_CalendarSearch");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetCoachCalendar(int coachId, bool? userTimeZone)
        {
            var response = SchedulerUtility.GetCoachCalendar(coachId, userTimeZone, HttpContext.Session.GetString(SessionContext.ParticipantTimeZone));
            var jsonResult = Json(response);
            return jsonResult;
        }

        #endregion

        #region View Appointments

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ViewAppointments()
        {
            AppointmenstModel model = new AppointmenstModel();
            var coachResponse = SchedulerUtility.GetCoachList(null, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId));
            var coaches = coachResponse.users.ToList();
            var list = (from coach in coaches
                        select new
                        {
                            Id = coach.Id,
                            Text = coach.FirstName + " " + coach.LastName,
                            Category = coach.RoleCode
                        }).ToArray();
            model.Coaches = new SelectList(list, "Id", "Text", 0, "Category");
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            return PartialView(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ListAppointments(DateTime StartDateTime, DateTime EndDateTime, int? coachId, int page, int pageSize, int? totalRecords)
        {
            EndDateTime = EndDateTime.AddDays(1);
            var response = SchedulerUtility.ListAppointments(StartDateTime, EndDateTime, coachId, page, pageSize, totalRecords, HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            return Json(new { Result = "OK", Records = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetCoachList()
        {
            var coachResponse = SchedulerUtility.GetCoachList(true, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId));
            return Json(new { Result = "OK", Records = coachResponse.users, CSRCount = (coachResponse.users.Where(x => x.RoleCode == "CSR")).Count(), CoachCount = (coachResponse.users.Where(x => x.RoleCode == "COACH")).Count() });
        }

        #endregion

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult MoveAppointments(string ToCoachIds, DateTime StartDateTime, DateTime EndDateTime, int? coachId)
        {
            if (EndDateTime.TimeOfDay == TimeSpan.Zero)
                EndDateTime = EndDateTime.AddDays(1);
            var response = SchedulerUtility.ListAppointments(StartDateTime, EndDateTime, coachId, 0, 100, null, HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            if (response.Appointments != null)
            {
                var result = SchedulerUtility.MoveAppointments(response.Appointments, coachId.Value, ToCoachIds);
                return Json(new { Result = "OK", Records = result });
            }
            return Json(new { Result = "OK", Records = 0 });
        }

        [Authorize]
        [HttpPost]
        public JsonResult ShowAppointments()
        {
            var response = SchedulerUtility.GetAppointments(null, null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.ParticipantTimeZone), HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
            if (response != null && response.Appointments != null)
                return Json(new { Result = "OK", pastApt = response.Appointments.Where(x => x.UTCDate < DateTime.UtcNow && x.Active == true).ToList(), upcomingApt = response.Appointments.Where(x => x.UTCDate >= DateTime.UtcNow).ToList() });
            else
                return Json(new
                {
                    Result = "OK",
                    pastApt = "",
                    upcomingApt = ""
                });
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddAppointmentFeedback(int id, int rating, string comments)
        {
            SchedulerUtility.AddAppointmentFeedback(id, rating, comments);
            return Json(new { Result = "OK" });
        }
    }
}