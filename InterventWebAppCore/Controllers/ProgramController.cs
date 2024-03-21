using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using NLog;
using System.Globalization;
using System.Net;

namespace InterventWebApp
{
    public class ProgramController : BaseController
    {
        private readonly AppSettings _appSettings;

        public ProgramController(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }
        #region Program

        [ModuleControl(Modules.Programs)]
        public ActionResult Programs()
        {
            return View("Programs");
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult ListPrograms(int page, int pageSize, int totalRecords)
        {
            var response = ProgramUtility.ListPrograms(page, pageSize, totalRecords);

            return Json(new
            {
                Result = "OK",
                TotalRecords = response.totalRecords,
                Records = response.Programs.Select(x => new
                {
                    Id = x.Id,
                    Name = x.Name,
                    Description = x.Description,
                    ProgramType = x.ProgramType,
                    Smoking = x.Smoking,
                    Pregancy = x.Pregancy,
                    RiskLevel = x.RiskLevel,
                    Active = x.Active
                })
            });
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult GetPrograms()
        {
            var programs = ProgramUtility.ListPrograms(null, null, null, true).Programs.Select(c => new { DisplayText = c.Name, Value = c.Id }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = programs });
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult GetCallTemplates()
        {
            var templates = ProgramUtility.ListAppointmentCallTemplates().CallTemplates.Select(c => new { DisplayText = c.TemplateName, Value = c.Id }).OrderBy(s => s.DisplayText);
            return Json(new { Result = "OK", Options = templates });
        }

        [HttpPost]
        public JsonResult SaveApptCallTemplate(int? templateId, string templateName, int noOfWeeks, int noOfCalls, bool isActive, IEnumerable<int> intervalList)
        {
            var response = ProgramUtility.SaveCallAppointmentTemplate(templateId, templateName, noOfWeeks, noOfCalls, isActive, intervalList, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            return Json(new { Result = "OK", Options = response });
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult ReadProgram(int id)
        {
            var response = ProgramUtility.ReadProgram(id);
            var eduKits = response.kitsinProgram.Where(x => x.Active == true).OrderBy(x => x.Order).Select(x => new
            {
                Id = x.eduKit.Id,
                Name = x.eduKit.Name,
                Order = x.Order
            });
            return Json(new
            {
                Result = "OK",
                response.Id,
                response.Name,
                response.Description,
                response.ProgramType,
                response.RiskLevel,
                response.Active,
                response.Smoking,
                response.Pregancy,
                response.ImageUrl,
                eduKits
            });
        }

        [ModuleControl(Modules.Programs)]
        [HttpPost]
        public JsonResult AddEditProgram(int? id, string name, string desc, byte type, byte? riskLevel, bool active, bool smoking, bool pregancy, string imageUrl)
        {
            var response = ProgramUtility.AddEditProgram(id, name, desc, type, riskLevel, active, smoking, pregancy, imageUrl);
            return Json(new { Result = "OK", Record = response.program });
        }

        [ModuleControl(Modules.Programs)]
        [HttpPost]
        public JsonResult DeleteProgram(int Id)
        {
            var response = ProgramUtility.DeleteProgram(Id);

            return Json(new { Result = "OK" });
        }

        #endregion

        #region Kits in Program

        [ModuleControl(Modules.Programs)]
        public JsonResult GetKitsinProgram(int programId)
        {
            var response = ProgramUtility.GetKitsinProgram(programId).kitsinProgram.OrderBy(x => x.Order).ToList();

            return Json(new { Result = "OK", Records = response.Select(x => new { Id = x.KitId, ProgramId = x.ProgramId, Order = x.Order, Name = x.eduKit.Name }) });
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult AddKittoProgram(int kitId, int programId, short order)
        {
            var response = ProgramUtility.AddEditKittoProgram(kitId, programId, order);
            return Json(new { Result = "OK", Record = response.kitinProgram });
        }

        [ModuleControl(Modules.Programs)]
        public JsonResult DeleteKitfromProgram(int ProgramId, int KitId)
        {
            var response = ProgramUtility.DeleteKitfromProgram(ProgramId, KitId).success;

            return Json(new { Result = "OK" });
        }

        #endregion

        #region User Programs

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult UserPrograms()
        {
            UserProgramModel model = new UserProgramModel();
            var portalId = GetParticipantPortalId();
            if (portalId != 0)
            {
                PortalDto portal = PortalUtility.ReadPortal(portalId).portal;
                if (portal.Active && ((portal.HasCoachingProgram && (portal.HRAforCoachingProgram.HasValue && portal.HRAforCoachingProgram.Value && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null) ||
                    (!portal.HRAforCoachingProgram.HasValue || !portal.HRAforCoachingProgram.Value)) && (portal.HasSelfHelpProgram && (portal.HRAforSelfHelpProgram.HasValue && portal.HRAforSelfHelpProgram.Value && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue && HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null) ||
                    (!portal.HRAforSelfHelpProgram.HasValue || !portal.HRAforSelfHelpProgram.Value))))
                    model.IsEligibileToEnroll = true;
            }
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_UserPrograms", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult SuggestedCoachingDates()
        {
            SuggestedCoachingDateModel model = new SuggestedCoachingDateModel();
            model.dateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.suggestedCoachingDates = ProgramUtility.GetSuggestedCoachingDates(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ProgramsInPortalId).Value, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null);
            return PartialView("_SuggestedCoachingDates", model);
        }

        static AppointmentCallTemplate MapCallTemplate(AppointmentCallTemplateDto source)
        {
            if (source == null)
                return null;
            return new AppointmentCallTemplate()
            {
                NoOfCalls = source.NoOfCalls,
                NoOfWeeks = source.NoOfWeeks,
                TemplateId = source.Id.Value,
                TemplateName = source.TemplateName,
                IsActive = source.IsActive
            };
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ReadAppointmentCallTemplate(int templateid)
        {
            var list = ProgramUtility.ReadAppointmentCallTemplate(templateid);
            return Json(new
            {
                TemplateId = list.CallTemplate.Id.Value,
                TemplateName = list.CallTemplate.TemplateName,
                NoOfWeeks = list.CallTemplate.NoOfWeeks,
                NoOfCalls = list.CallTemplate.NoOfCalls,
                IsActive = list.CallTemplate.IsActive,
                callInterval = list.CallIntervals
            });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpGet]
        public ActionResult AppointmentCallTemplate()
        {
            var list = ProgramUtility.ListAppointmentCallTemplates().CallTemplates.Select(MapCallTemplate).ToList();
            var model = new AppointmentCallTemplateModel();
            model.CallTemplates = list;
            return View(model);
        }

        static AppointmentCallInterval MapCallInterval(AppointmentCallIntervalDto source)
        {
            if (source == null)
                return null;
            return new AppointmentCallInterval()
            {
                CallNumber = source.CallNumber.ToString(),
                IntervalInDays = source.IntervalInDays.ToString()
            };
        }

        [HttpGet]
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public PartialViewResult AppointmentCallTemplateIntervals(int id)
        {
            var list = ProgramUtility.GetAppointmentCallIntervalForTemplate(id).CallIntervals.Select(MapCallInterval).ToList();
            var model = new AppointmentCallIntervalModel();
            model.CallIntervals = list;
            return PartialView("_AppointmentCallTemplateInterval", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult EnrollUserinProgram()
        {
            AddProgramModel model = new AddProgramModel();
            var infoforProgram = ParticipantUtility.GetInfoforProgram(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            if (infoforProgram.HRA != null && infoforProgram.HRA.CompleteDate.HasValue)
            {
                model.hraRisk = infoforProgram.HRA.RiskCode;
                if (infoforProgram.HRA.User.Gender == 2)
                {
                    if (infoforProgram.HRA.MedicalCondition.Pregnant.HasValue && infoforProgram.HRA.MedicalCondition.Pregnant == 1 && infoforProgram.HRA.MedicalCondition.PregDueDate > DateTime.Today)
                        model.eligible = "<h6>Maternity Management</h6>";
                    else
                    {
                        var wellnessData = ParticipantUtility.ListWellnessData(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                        if (wellnessData.WellnessData.Where(x => x.isPregnant == true && (x.DueDate > DateTime.Today)).Count() > 0)
                            model.eligible = "<h6>Maternity Management</h6>";
                    }
                }
                if (ParticipantUtility.CheckIfTobaccoUser(true, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, GetParticipantPortalId()).smoker)
                {
                    model.eligible += "<h6>Tobacco Cessation</h6>";
                }
            }
            if (infoforProgram.appointment != null)
            {
                model.suggestedCoach = infoforProgram.appointment.CoachId;
            }
            var response = SchedulerUtility.GetCoachList(true, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId)).users.Where(x => x.RoleCode == "COACH");
            model.Coaches = response.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() });

            var ProgramsinPortals = PortalUtility.GetProgramsByPortal(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, null, true);
            model.ProgramsinPortals = ProgramsinPortals.ProgramsinPortal.Select(x => new SelectListItem { Text = x.NameforAdmin, Value = x.Id.ToString() });

            model.Language = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.Programs = ProgramsinPortals.ProgramsinPortal.Select(x => new SelectListItem { Text = x.program.ProgramType.ToString(), Value = x.Id.ToString() });
            return PartialView("_EnrollUserinProgram", model);
        }


        [HttpPost]
        [Authorize]
        public JsonResult EnrollinProgram(int ProgramsinPortalsId, int? coachId, string Language)
        {
            int participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            int loginId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;

            int? hraId = null;
            if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
                hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            var response = ProgramUtility.EnrollinProgram(participantId, ProgramsinPortalsId, hraId, coachId, loginId, Language, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith));
            if (response.success)
            {
                if (ParticipantUtility.IsMediOrbisUser(HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value) && HttpContext.Session.GetString(SessionContext.ProgramCode) != null)
                {
                    LogReader reader = new LogReader();
                    string uniqueId = HttpContext.Session.GetString(SessionContext.UniqueId);
                    var mediOrbisResponse = MediOrbis.SendUserProgramStatus(new MediOrbisRequest { refId = uniqueId, selfCareTool = HttpContext.Session.GetString(SessionContext.ProgramCode), status = (int)MediOrbisProgramStatus.InProgress }, _appSettings.MediOrbisUrl, _appSettings.MediOrbisSecretKey);
                    if (mediOrbisResponse.Status && mediOrbisResponse.StatusCode == HttpStatusCode.OK)
                        reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "MediOrbis Program Status", null, "Program status update for user : " + participantId + " - RefId : " + uniqueId, null, null));
                    else
                        reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "MediOrbis Program Status", null, "Update program status failed for user : " + participantId + " - RefId : " + uniqueId + " - Status code : " + mediOrbisResponse.StatusCode + " - Message : " + mediOrbisResponse.ErrorMsg, null, null));
                }
                HttpContext.Session.SetInt32(SessionContext.UserinProgramId, response.UsersinProgramId);
                HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, response.ProgramsInPortalId);
                HttpContext.Session.SetInt32(SessionContext.ProgramType, response.ProgramType);
                ParticipantUtility.AssignProgramIncentive(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null);
            }
            return Json(new { Result = "OK", Options = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult UpdateUserProgram()
        {
            UpdateUserProgram model = new UpdateUserProgram();
            var userProgDetails = ProgramUtility.GetUserinProgramDetails(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value);
            var coacheResponse = SchedulerUtility.GetCoachList(true, true, HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : null, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.StateId)).users.Where(x => x.RoleCode == "COACH");
            model.Coaches = coacheResponse.Select(x => new SelectListItem { Text = x.FirstName + " " + x.LastName, Value = x.Id.ToString() });
            model.CoachId = userProgDetails.CoachId;
            var ProgramsbyPortals = PortalUtility.GetProgramsByPortal(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, null, true);
            var coachingprograms = ProgramsbyPortals.ProgramsinPortal.Where(x => x.program.ProgramType == 2 && x.Id == userProgDetails.ProgramsinPortalId).FirstOrDefault();
            if (coachingprograms == null)
            {
                model.disableProgramChange = true;
                model.ProgramsinPortals = ProgramsbyPortals.ProgramsinPortal.Select(x => new SelectListItem { Text = x.NameforAdmin, Value = x.Id.ToString() });
            }
            else
            {
                model.disableProgramChange = false;
                model.ProgramsinPortals = ProgramsbyPortals.ProgramsinPortal.Where(x => x.program.ProgramType == 2).Select(x => new SelectListItem { Text = x.NameforAdmin, Value = x.Id.ToString() });
            }
            model.PrograminPortalId = userProgDetails.ProgramsinPortalId;
            model.InactiveReason = ProgramUtility.ListInactiveReasons().Select(x => new SelectListItem { Text = x.Reason, Value = x.Id.ToString() });
            model.Language = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.LanguagePreference = userProgDetails.Language;
            return PartialView("_UpdateUserProgram", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult UpdateUserinProgram(int? PrograminPortalId, int? CoachId, byte? InactiveReasonId, string Language, bool AssignedFollowUp)
        {
            int LoginId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var response = ProgramUtility.UpdateUserinProgram(PrograminPortalId, CoachId, LoginId, InactiveReasonId, Language, AssignedFollowUp, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber), _appSettings.SystemAdminId);
            if (InactiveReasonId.HasValue)
            {
                ClearProgramRelatedSessions();
                var PortalId = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
                var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                ParticipantUtility.UpdateUserTrackingStatus(participantId, PortalId, true, null, null);   //marking user "Do Not Track" 
            }

            if (response.success && AssignedFollowUp)
            {
                HttpContext.Session.Remove(SessionContext.FollowUpId);
                if (!HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue)
                    HttpContext.Session.SetInt32(SessionContext.AssignedFollowUp, 1);
                else if (HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue && Convert.ToByte(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) >= 0)
                    HttpContext.Session.SetInt32(SessionContext.AssignedFollowUp, HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).Value + 1);
                HttpContext.Session.Remove(SessionContext.FollowUpCompleteDate);
            }
            return Json(new { Result = "OK", Records = response.success });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult ActivateUserProgram(int UsersinProgramId)
        {
            ActivateUserProgram model = new ActivateUserProgram();
            model.UsersinProgramId = UsersinProgramId;
            return PartialView("_ActivateUserProgram", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult ActivateUserinProgram(int UsersinProgramId)
        {
            int LoginId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            ProgramUtility.ActivateUserinProgram(UsersinProgramId, LoginId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
            return Json(new { Result = "OK" });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AddKittoUserProgram()
        {
            AddKittoUserProgramModel model = new AddKittoUserProgramModel();
            var response = ProgramUtility.GetKitsHistoryforUser(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            var eduKits = KitUtility.ListEduKits(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId), null, null, null, true).EduKits;

            if (response.KitsinUserPrograms != null && response.KitsinUserPrograms.Count > 0)
            {
                var eduKitIds = eduKits.Select(l => l.Id).ToList();
                for (int i = 0; i < response.KitsinUserPrograms.Count; i++)
                {
                    if (eduKitIds.Contains((response.KitsinUserPrograms[i].Kit.Id)))
                    {
                        eduKits.Where(x => x.Id == response.KitsinUserPrograms[i].Kit.Id).FirstOrDefault().Name = response.KitsinUserPrograms[i].Kit.Name + " (Assigned Date: " + response.KitsinUserPrograms[i].StartDate.ToString(HttpContext.Session.GetString(SessionContext.DateFormat) + ")");
                    }
                }
            }
            var list = (from item in eduKits.OrderBy(x => x.InvId)
                        select new
                        {
                            Id = item.Id,
                            Name = item.InvId + " : " + item.Name,
                            Category = item.KitTopic.Name
                        }).ToArray();

            model.Kits = new SelectList(list, "Id", "Name", 0, "Category");
            return PartialView("_AddKittoUserProgram", model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        [HttpPost]
        public JsonResult AddKittoUserProgram(int kitId)
        {
            var response = ProgramUtility.AddKittoUserProgram(HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, kitId, HttpContext.Session.GetString(SessionContext.LanguagePreference), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber), Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.KitAlert) != null ? HttpContext.Session.GetString(SessionContext.KitAlert) : false));
            return Json(new { Result = "OK", Records = response });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult GetUserProgramHistory()
        {
            var response = ProgramUtility.GetUserProgramHistory(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
            var activeProgram = response.usersinPrograms.Where(x => x.IsActive).OrderByDescending(x => x.Id).FirstOrDefault();
            string PregnancyStatus = null;
            if (activeProgram != null)
            {
                if (activeProgram.ProgramsinPortal.program.Pregancy == true)
                {
                    DateTime date = activeProgram.EnrolledOn;
                    int hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : 0;
                    PregnancyStatus = CommonUtility.GetPregancyStatus(hraId, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, date).Trimester;
                }
            }
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue)
            {
                var lastProgram = response.usersinPrograms.OrderByDescending(x => x.Id).FirstOrDefault();
                if (lastProgram != null && lastProgram.ProgramsinPortal.PortalId == Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value))
                {
                    HttpContext.Session.SetInt32(SessionContext.UserinProgramId, lastProgram.Id);
                    HttpContext.Session.SetInt32(SessionContext.ProgramsInPortalId, lastProgram.ProgramsinPortalId);
                    HttpContext.Session.SetInt32(SessionContext.ProgramType, lastProgram.ProgramsinPortal.program.ProgramType);
                }
            }
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());

            var allowFollowUp = false;
            var portalId = GetParticipantPortalId();
            var portalFollowups = PortalUtility.GetPortalFollowUps(portalId, HttpContext.Session.GetInt32(SessionContext.ProgramType)).portalFollowUps.OrderBy(y => y.FollowUpType.Days).ToArray();
            if (HttpContext.Session.GetInt32(SessionContext.HasHRA) != null && ((HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.Yes || HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.Optional) && HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue))
            {
                allowFollowUp = true;
            }
            var UserinProgram = response.usersinPrograms.Select(x => new
            {
                Id = x.Id,
                StartDate = x.StartDate.HasValue ? CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.StartDate.Value, custTZone), false, HttpContext.Session.GetString(SessionContext.DateFormat)) : "N/A",
                IsActive = x.IsActive,
                InactiveReason = x.ProgramInactiveReason,
                InactiveDate = x.InactiveDate.HasValue ? TimeZoneInfo.ConvertTimeFromUtc(x.InactiveDate.Value, custTZone).ToShortDateString() : null,
                Coach = x.CoachId.HasValue ? x.User1.FirstName + " " + x.User1.LastName : null,
                KitsinUserPrograms = x.KitsinUserPrograms.ToList(),
                Name = x.ProgramsinPortal.NameforAdmin,
                Portal = x.ProgramsinPortal.portal.Name,
                Reactivate = x.ProgramsinPortal.portal.Active,
                AllowFollowUp = allowFollowUp,
                Assignedfollowup = x.AssignedFollowUp,
                FollowupType = x.AssignedFollowUp != 0 && portalFollowups.Count() >= x.AssignedFollowUp ? portalFollowups[x.AssignedFollowUp - 1].FollowUpType.Type : x.AssignedFollowUp == 0 && portalFollowups.Count() > 0 ? portalFollowups[0].FollowUpType.Type : "",
                NextFollowupType = portalFollowups.Count() > x.AssignedFollowUp ? portalFollowups[x.AssignedFollowUp].FollowUpType.Type : "",
                FollowupDueDate = !ListOptions.AlbertaUsers.Contains(x.UserId) ? x.StartDate.HasValue && portalFollowups.Count() > x.AssignedFollowUp ? "(" + CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.StartDate.Value, custTZone).AddDays(portalFollowups[x.AssignedFollowUp].FollowUpType.Days), false, HttpContext.Session.GetString(SessionContext.DateFormat)) + ")" :
                                  x.StartDate.HasValue && portalFollowups.Count() > 0 && x.AssignedFollowUp == 0 ? "(" + CommonUtility.dateFormater(TimeZoneInfo.ConvertTimeFromUtc(x.StartDate.Value, custTZone).AddDays(portalFollowups[x.AssignedFollowUp].FollowUpType.Days), false, HttpContext.Session.GetString(SessionContext.DateFormat)) + ")" : "" : "",
                FollowUpCount = x.FollowUps.Where(c => c.CompleteDate != null).Count(),
                PregnancyStatus = x.IsActive ? PregnancyStatus : null,
                PortalFollowUps = portalFollowups.Count()
            });
            return Json(new
            {
                Result = "OK",
                HasActiveProgram = response.hasActiveProgram,
                UserinProgram
            });
        }
        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult DeleteKitinUserProgram(int id)
        {
            var response = ProgramUtility.DeleteKitinUserProgram(id, HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            return Json(new { Result = "OK", Records = response.success });
        }

        [Authorize]
        public ActionResult CurrentPrograms(bool? failed, bool? initialStage)
        {
            ViewData["InitialStage"] = initialStage;
            if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue || HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.Optional || HttpContext.Session.GetInt32(SessionContext.HasHRA) == (int)HRAStatus.No)
            {
                ProgramsinPortalsModel model = new ProgramsinPortalsModel();
                var response = ParticipantUtility.ReadUserParticipation(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
                bool isMediOrbisUser = ParticipantUtility.IsMediOrbisUser(HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value);

                if ((response.usersinProgram != null && !isMediOrbisUser) || (response.usersinProgram != null && isMediOrbisUser && response.usersinProgram.IsActive))
                {
                    model.userinProgram = true;
                    model.userinProgramId = response.usersinProgram.Id;
                    model.currentProgramId = response.usersinProgram.ProgramsinPortalId;
                }
                else
                {
                    model.userinProgram = false;
                }
                model.hasActivePortal = response.hasActivePortal;
                var progResponse = PortalUtility.ListProgramsinPortals(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value));
                model.ProgramsinPortals = progResponse.ProgramsinPortals.Where(x => x.program.ProgramType == 1 && x.Active == true && (HttpContext.Session.GetString(SessionContext.ProgramCode) == null || x.program.Code == HttpContext.Session.GetString(SessionContext.ProgramCode))).OrderBy(y => y.SortOrder).ToList();
                model.taskforProgram = ProgramUtility.GetProgramEnrollmentTask(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).success;
                model.failed = failed;
                model.participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                model.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
                model.showPricing = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.ShowPricing) != null ? HttpContext.Session.GetString(SessionContext.ShowPricing) : false);
                return View(model);
            }
            else
                return RedirectToAction("InitialDashboard", "Participant");

        }

        [Authorize]
        public ActionResult MyProgram()
        {
            UserinProgramModel model = new UserinProgramModel();
            CultureInfo ci = new CultureInfo(HttpContext.Session.GetString(SessionContext.LanguagePreference));
            var userId = Convert.ToInt32((HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).ToString());
            var UsersinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
            var portalId = HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value;
            var portalIncentive = PortalUtility.GetPortalIncentives((int)portalId).portalIncentives;
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                model.timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            else
                model.timeZone = User.TimeZone();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(model.timeZone);
            var vitalsLog = JournalUtility.ListVitalsLog(new DiaryListModel { page = 0, pageSize = 1 }, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            if (vitalsLog.dailyVitals.Count > 0)
            {
                model.dailyVitals = vitalsLog.dailyVitals.Where(x => x.Date.Value.Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date).FirstOrDefault();
                if (model.dailyVitals != null)
                {
                    model.hasPendingVitals = (model.dailyVitals.Weight.HasValue && model.dailyVitals.AerobicExercise.HasValue && model.dailyVitals.HealthyEating.HasValue &&
                            model.dailyVitals.Hydration.HasValue && model.dailyVitals.Alcohol.HasValue && model.dailyVitals.Tobacco.HasValue &&
                            model.dailyVitals.Medications.HasValue && model.dailyVitals.Sleep.HasValue && model.dailyVitals.Stress.HasValue &&
                            model.dailyVitals.Happy.HasValue) ? false : true;
                }
            }
            model.Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.kitsinUserProgramGoals = KitUtility.GetKitsActionGoals(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).KitsinUserProgramGoals;
            var programHistory = ProgramUtility.GetUserProgramHistory(userId, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
            var userinProgram = programHistory.usersinPrograms.Where(x => x.Id == UsersinProgramId).FirstOrDefault();
            if (userinProgram != null)
            {
                TextInfo ti = CultureInfo.CurrentCulture.TextInfo;
                var appointments = SchedulerUtility.GetAppointments(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById(model.timeZone)), null, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, model.timeZone, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null);
                if (appointments != null && appointments.Appointments != null && appointments.Appointments.Count > 0)
                {
                    string meetingLink = "";
                    var futureAppointment = appointments.Appointments.OrderBy(x => x.UTCDate).FirstOrDefault();
                    var appointmentDate = Convert.ToDateTime(futureAppointment.Date + " " + futureAppointment.StartTime);
                    if (futureAppointment.VideoRequired)
                        meetingLink = "<a class='text-blue' href='" + "https://zoom.us/j/" + futureAppointment.CoachMeetingId + "'>" + Translate.Message("L4600") + "</a>";
                    model.FutureAppointmentDate = "<div class='time'><p>" + ti.ToTitleCase(appointmentDate.ToString("m", ci)) + " " + ti.ToTitleCase(appointmentDate.ToString("D", ci).Substring(0, 3)) + "</b><br/>" + meetingLink + "</p></div><div class='time'><p><b>" + appointmentDate.ToString("t", ci) + " </b><br />" + HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName) + "</p></div>";

                }

                model.userinProgram = userinProgram;
                if (userinProgram.User1 != null)
                {
                    if (String.IsNullOrEmpty(userinProgram.User1.Picture))
                        model.CoachImage = CommonUtility.GetGenderSpecificImage(userinProgram.User1);
                    else
                        model.CoachImage = userinProgram.User1.Picture;
                }
                if (HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)) > 0 && userinProgram.FollowUps.Count() >= Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp)))
                    model.FollowupCompletedDate = userinProgram.FollowUps.OrderByDescending(x => x.Id).Select(c => c.CompleteDate).FirstOrDefault();
                var surveyStatus = ParticipantUtility.GetSurveyCompletedStatus(userinProgram.Id);
                model.IsSurveyCompleted = surveyStatus.SurveyCompleted;
                model.PartiallyCompleted = surveyStatus.PartiallyCompleted;
                if (userinProgram.ProgramsinPortal.program.ProgramType == 1)
                    model.IsCoachingProgramAvailable = PortalUtility.ListProgramsinPortals(portalId).ProgramsinPortals.Any(x => x.program.ProgramType == 2 && x.Active == true);
                if (!model.IsSurveyCompleted)
                    model.SurveyQuestions = ParticipantUtility.GetSurveyQuestions();
                model.IsPortalIncentivePresent = portalIncentive.Where(x => x.IsActive).Any();
                model.IsVitalsCompletionIncentive = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Vitals_Completion && x.IsActive == true).Count() > 0 ? true : false;
                if (model.IsVitalsCompletionIncentive)
                    model.VitalsCompletionPoints = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Vitals_Completion && x.IsActive == true).FirstOrDefault().Points.ToString();
                model.ShowFormsTab = !String.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.PatientReleaseForm) || !String.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.MedicalClearanceForm) || !String.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.KnowYourNumbersForm)
                                            || !String.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.TestimonialForm) || !String.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.TobaccoReleaseForm) || HttpContext.Session.GetString(SessionContext.LanguagePreference).Equals("en-us") ? true : false;
            }
            if (programHistory != null && programHistory.usersinPrograms != null)
            {
                model.pastPrograms = programHistory.usersinPrograms.Where(x => (userinProgram == null || !Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)) || userinProgram.Id != x.Id)
                && x.KitsinUserPrograms.Any(y => y.IsActive == true)).ToList();
            }
            model.ProgramWellnessData = ProgramUtility.GetProgramWellnessData(HttpContext.Session.GetInt32(SessionContext.Unit).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.UniqueId) != null ? HttpContext.Session.GetString(SessionContext.UniqueId) : "", HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value);
            model.IsKitGoalCompletionIncentive = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Goal_Completion && x.IsActive == true).Count() > 0 ? true : false;
            if (model.IsKitGoalCompletionIncentive)
                model.KitGoalCompletionPoints = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Goal_Completion && x.IsActive == true).FirstOrDefault().Points.ToString();
            model.showSelfScheduling = ShowSelfScheduling();
            model.dob = HttpContext.Session.GetString(SessionContext.DOB);
            model.assignedFollowUp = HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).HasValue ? HttpContext.Session.GetInt32(SessionContext.AssignedFollowUp).Value : null;
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.gender = HttpContext.Session.GetInt32(SessionContext.Gender).Value;
            model.hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue ? HttpContext.Session.GetInt32(SessionContext.HRAId).Value : null;
            model.hraCompleteDate = HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null ? HttpContext.Session.GetString(SessionContext.HRACompleteDate) : "";
            model.hasHRA = HttpContext.Session.GetInt32(SessionContext.HasHRA).HasValue ? HttpContext.Session.GetInt32(SessionContext.HasHRA).Value : null;
            model.incentivePoints = CommonUtility.GetIncentivePoints(GetParticipantPortalId(), HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Points;

            return View(model);
        }


        [HttpPost]
        [Authorize]
        public JsonResult SaveUserOptions(AddUserChoiceRequest request)
        {
            ProgramUtility.AddUserOptions(request, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), _appSettings.DTCOrgCode);
            if (request.PercentComplete == 100)
            {
                var UsersinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
                var userId = Convert.ToInt32((HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).ToString());
                var programHistory = ProgramUtility.GetUserProgramHistory(userId, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
                var userinProgram = programHistory.usersinPrograms.Where(x => x.IsActive == true && x.Id == UsersinProgramId).FirstOrDefault();
                if (userinProgram != null)
                {
                    if (userinProgram.ProgramsinPortal.program.ProgramType == (int)ProgramTypes.SelfHelp && 1 == 2)
                    {
                        var currentKit = userinProgram.KitsinUserPrograms.Where(x => x.IsActive == true && x.PercentCompleted != 100).FirstOrDefault();
                        if (currentKit != null)
                        {
                            AddKittoDashboardRequest dashboardRequest = new AddKittoDashboardRequest();
                            dashboardRequest.userId = userinProgram.UserId;
                            dashboardRequest.kitId = currentKit.KitId;
                            dashboardRequest.kitsinUserProgramId = currentKit.Id;
                            dashboardRequest.languageCode = HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "en-us"; ;
                            new ProgramReader().AddKittoDashboard(dashboardRequest);
                        }
                    }
                    if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)) && Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.IsMediOrbisUser)) && HttpContext.Session.GetString(SessionContext.ProgramCode) != null && userinProgram.KitsinUserPrograms.Where(x => x.IsActive).All(x => x.CompleteDate.HasValue && x.PercentCompleted == 100))
                    {
                        LogReader reader = new LogReader();
                        string uniqueId = HttpContext.Session.GetString(SessionContext.UniqueId);
                        var mediOrbisResponse = MediOrbis.SendUserProgramStatus(new MediOrbisRequest { refId = uniqueId, selfCareTool = HttpContext.Session.GetString(SessionContext.ProgramCode), status = (int)MediOrbisProgramStatus.Completed }, _appSettings.MediOrbisUrl, _appSettings.MediOrbisSecretKey);
                        if (mediOrbisResponse.Status && mediOrbisResponse.StatusCode == HttpStatusCode.OK)
                            reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Info, "MediOrbis Program Status", null, "Program status update for user : " + userId + " - RefId : " + uniqueId + " - status : Completed", null, null));
                        else
                            reader.WriteLogMessage(new LogEventInfo(NLog.LogLevel.Error, "MediOrbis Program Status", null, "Update program status failed for user : " + userId + " - RefId : " + uniqueId + " - status : Completed - Status code : " + mediOrbisResponse.StatusCode + " - Message : " + mediOrbisResponse.ErrorMsg, null, null));
                        ProgramUtility.UpdateUserinProgram(userinProgram.ProgramsinPortal.Id, null, userId, (int)ProgramInactiveReasons.SuccessfullyCompleted, null, false, HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetString(SessionContext.OrgContactEmail), HttpContext.Session.GetString(SessionContext.OrgContactNumber), _appSettings.SystemAdminId);
                    }
                    IncentiveReader incentiveReader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.Kit_Completion;
                    incentivesRequest.userId = userId;
                    incentivesRequest.portalId = userinProgram.ProgramsinPortal.PortalId;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                    if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                        incentivesRequest.adminId = Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.AdminId).Value.ToString());
                    incentivesRequest.reference = request.KitsInUserProgramsId.ToString();
                    incentiveReader.AwardIncentives(incentivesRequest);
                }
            }
            return Json(true);
        }

        [Authorize]
        public ActionResult UserForms()
        {
            UserinProgramModel model = new UserinProgramModel();
            var UsersinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;

            var userId = Convert.ToInt32((HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).ToString());
            var programHistory = ProgramUtility.GetUserProgramHistory(userId, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");

            var userinProgram = programHistory.usersinPrograms.Where(x => x.Id == UsersinProgramId).FirstOrDefault();

            var userForms = ParticipantUtility.GetUserForms(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value).userForms;
            model.userForms = userForms;
            if (userinProgram != null)
            {
                String languageCode = HttpContext.Session.GetString(SessionContext.LanguagePreference) != null && !HttpContext.Session.GetString(SessionContext.LanguagePreference).Equals("en-us") ? "_" + HttpContext.Session.GetString(SessionContext.LanguagePreference) : "";
                model.userinProgram = userinProgram;
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.PatientReleaseForm))
                    model.PatientReleaseForm = userinProgram.ProgramsinPortal.portal.PatientReleaseForm.Insert(userinProgram.ProgramsinPortal.portal.PatientReleaseForm.IndexOf("."), languageCode);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.MedicalClearanceForm))
                    model.MedicalClearanceForm = userinProgram.ProgramsinPortal.portal.MedicalClearanceForm.Insert(userinProgram.ProgramsinPortal.portal.MedicalClearanceForm.IndexOf("."), languageCode);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.KnowYourNumbersForm))
                    model.KnowYourNumbersForm = userinProgram.ProgramsinPortal.portal.KnowYourNumbersForm.Insert(userinProgram.ProgramsinPortal.portal.KnowYourNumbersForm.IndexOf("."), languageCode);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.TestimonialForm))
                    model.TestimonialForm = userinProgram.ProgramsinPortal.portal.TestimonialForm.Insert(userinProgram.ProgramsinPortal.portal.TestimonialForm.IndexOf("."), languageCode);
                if (!string.IsNullOrEmpty(userinProgram.ProgramsinPortal.portal.TobaccoReleaseForm) && ParticipantUtility.CheckIfTobaccoUser(true, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, GetParticipantPortalId()).smoker)
                    model.TobaccoReleaseForm = userinProgram.ProgramsinPortal.portal.TobaccoReleaseForm.Insert(userinProgram.ProgramsinPortal.portal.TobaccoReleaseForm.IndexOf("."), languageCode);
                model.FoodList = HttpContext.Session.GetString(SessionContext.LanguagePreference) != null && HttpContext.Session.GetString(SessionContext.LanguagePreference).Equals("en-us") ? "Food Search.pdf" : "";
            }
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_UserForms", model);
        }

        [Authorize]
        public ActionResult KitProgram()
        {
            UserinProgramModel model = new UserinProgramModel();
            var UsersinProgramId = HttpContext.Session.GetInt32(SessionContext.UserinProgramId).Value;
            var userId = Convert.ToInt32((HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).ToString());
            var programHistory = ProgramUtility.GetUserProgramHistory(userId, User.TimeZone(), HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "");
            var userinProgram = programHistory.usersinPrograms.Where(x => x.Id == UsersinProgramId).FirstOrDefault();
            if (userinProgram != null)
            {
                model.userinProgram = userinProgram;
            }
            if (programHistory != null && programHistory.usersinPrograms != null)
            {
                model.pastPrograms = programHistory.usersinPrograms.Where(x => (userinProgram == null || userinProgram.Id != x.Id) && x.KitsinUserPrograms.Any(y => y.IsActive == true)).ToList();
            }
            KitsinUserProgramDto lastKit = null;
            if (userinProgram != null)
            {
                lastKit = userinProgram.KitsinUserPrograms.Where(x => !x.CompleteDate.HasValue && x.IsActive == true).FirstOrDefault();
            }
            if (lastKit != null)
            {
                var kitResponse = KitUtility.GetKitById(HttpContext.Session.GetInt32(SessionContext.UserinProgramId), lastKit.KitId, userinProgram.KitsinUserPrograms.Where(x => x.KitId == lastKit.KitId && x.IsActive == true).FirstOrDefault().Id, false, "en-us");
                if (kitResponse.StepNames != null)
                {
                    model.StepNames = kitResponse.StepNames.ToList();
                    model.EduKit = kitResponse.EduKit;
                    model.StepName = model.StepNames.Where(x => x.HideLink == true).FirstOrDefault();
                }
            }
            var portalIncentive = PortalUtility.GetPortalIncentives(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value)).portalIncentives;
            model.IsKitCompletionIncentive = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Completion && x.IsActive == true).Count() > 0 ? true : false;
            if (model.IsKitCompletionIncentive)
                model.KitCompletionPoints = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Completion && x.IsActive == true).FirstOrDefault().Points.ToString();
            //model.IsKitGoalCompletionIncentive = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Goal_Completion && x.IsActive == true).Count() > 0 ? true : false;
            //if (model.IsKitGoalCompletionIncentive)
            //    model.KitGoalCompletionPoints = portalIncentive.Where(x => x.IncentiveTypeId == (int)IncentiveTypes.Kit_Goal_Completion && x.IsActive == true).FirstOrDefault().Points.ToString();
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_KitProgram", model);
        }

        [Authorize]
        public ActionResult ProgramWellnessData()
        {
            var wellnessData = ProgramUtility.GetProgramWellnessData(HttpContext.Session.GetInt32(SessionContext.Unit).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.UniqueId) != null ? HttpContext.Session.GetString(SessionContext.UniqueId) : "", HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value);
            wellnessData.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            wellnessData.isTeamsBP = HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value == _appSettings.TeamsBPOrgId;
            return PartialView("_ProgramWellnessData", wellnessData);
        }

        [HttpPost]
        public JsonResult AddEditProgramWellnessData(ProgramWellnessDataModel model)
        {
            WellnessDataModel request = new WellnessDataModel();
            request.wellnessData = new WellnessDataDto();
            if (model.wellnessDataId != 0)
                request.wellnessData.Id = model.wellnessDataId;
            request.updatedbyUser = true;
            if (model.SBP.HasValue)
                request.wellnessData.SBP = (short)model.SBP;
            if (model.DBP.HasValue)
                request.wellnessData.DBP = (short)model.DBP;
            request.wellnessData.WellnessVision = model.WellnessVision;
            request.wellnessData.UserId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            if (model.Weight.HasValue)
                request.wellnessData.Weight = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.Weight.Value;
            if (model.Waist.HasValue)
                request.wellnessData.waist = Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? (float)CommonUtility.ToImperial(model.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : model.Waist.Value;
            request.wellnessData.WellnessVision = model.WellnessVision;
            request.userId = HttpContext.Session.GetInt32(SessionContext.UserId).Value;
            var result = ParticipantUtility.AddEditWellnessData(request);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult GetWellnessVision()
        {
            string wellnessVision = "";
            var wellnessData = ParticipantUtility.ListWellnessData(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).WellnessData.Where(x => x.WellnessVision != null).LastOrDefault();
            if (wellnessData != null && !string.IsNullOrEmpty(wellnessData.WellnessVision))
                wellnessVision = wellnessData.WellnessVision;
            return Json(new { Result = "success", WellnessVision = wellnessVision });
        }

        #endregion

        #region Meal Plan
        [Authorize]
        public ActionResult NutritionRecommendation(bool onlymeal = false, bool onlydiet = false)
        {
            NutritionGoalModel model = new NutritionGoalModel();
            var Goals = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hraGoals;
            model = ReportUtility.NutritionGoal(Goals, null, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
            model.onlydiet = onlydiet;
            model.onlymeal = onlymeal;
            return PartialView("_NutritionData", model);
        }

        #endregion

        #region Exercise Plan
        public ActionResult ExerciseRecommendation(bool fromKit)
        {
            PhysicalActivityGoalModel model = new PhysicalActivityGoalModel();
            var hra = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra;
            model = ReportUtility.PhysicalActivityGoal(hra, null, HttpContext.Session.GetInt32(SessionContext.ProgramType));
            if (fromKit)
            {
                TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(User.TimeZone());
                model.HRADate = TimeZoneInfo.ConvertTimeFromUtc(hra.CompleteDate.Value, custTZone).ToShortDateString();
            }
            model.hasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_ProgramPhysicalActivityGoal", model);
        }

        public JsonResult ChangeExercisePlanWeekStatus(int week)
        {
            int hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            ProgramUtility.ChangeExercisePlanWeekStatus(week, hraId);
            return Json(true);
        }

        #endregion
    }
}