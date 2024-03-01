using Intervent.Business;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using System.Net;

namespace InterventWebApp
{
    public class ProgramUtility
    {
        public static ListProgramsResponse ListPrograms(int? page, int? pageSize, int? totalRecords, bool? onlyActive = null)
        {
            ProgramReader reader = new ProgramReader();
            ListProgramsRequest request = new ListProgramsRequest();
            request.onlyActive = onlyActive;
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.ListPrograms(request);
        }

        public static ProgramDto ReadProgram(int id)
        {
            ProgramReader reader = new ProgramReader();
            return reader.ReadProgram(id);
        }

        public static AddUserChoiceResponse AddUserOptions(AddUserChoiceRequest request, int userId, int participantId, int? integrationWith, string DTCOrgCode)
        {
            KitReader reader = new KitReader();
            request.UpdatedBy = userId;
            request.ParticipantId = participantId;
            if (integrationWith.HasValue && integrationWith.Value == (int)Integrations.Intuity && ParticipantUtility.IsIntuityUser(request.ParticipantId))
                request.IsIntuityUser = true;
            if (request.UserAnswer != null)
            {
                foreach (var option in request.UserAnswer)
                {
                    if (!string.IsNullOrEmpty(option.Value))
                        option.Value = WebUtility.UrlDecode(option.Value);
                }
            }
            return reader.AddUserOptions(request, DTCOrgCode);
        }

        public static GetKitsinProgramResponse GetKitsinProgram(int programId)
        {
            ProgramReader reader = new ProgramReader();
            GetKitsinProgramRequest request = new GetKitsinProgramRequest();
            request.programId = programId;
            return reader.GetKitsinProgram(request);
        }

        public static UsersinProgramDto GetUserinProgramDetails(int userinProgramId)
        {
            ProgramReader reader = new ProgramReader();
            return reader.GetUserinProgramDetails(userinProgramId);
        }

        public static AddEditProgramResponse AddEditProgram(int? id, string name, string desc, byte type, byte? riskLevel, bool active, bool smoking, bool pregancy, string imageUrl)
        {
            ProgramReader reader = new ProgramReader();
            AddEditProgramRequest request = new AddEditProgramRequest();
            ProgramDto program = new ProgramDto();
            if (id.HasValue)
                program.Id = id.Value;
            program.Name = name;
            program.Description = desc;
            program.ProgramType = type;
            if (riskLevel.HasValue)
                program.RiskLevel = riskLevel.Value;
            program.Active = active;
            program.Smoking = smoking;
            program.Pregancy = pregancy;
            program.ImageUrl = imageUrl;
            request.program = program;
            return reader.AddEditProgram(request);
        }

        public static bool DeleteProgram(int Id)
        {
            ProgramReader reader = new ProgramReader();
            return reader.DeleteProgram(Id);
        }

        public static DeleteKitfromProgramResponse DeleteKitfromProgram(int programId, int kitId)
        {
            ProgramReader reader = new ProgramReader();
            DeleteKitfromProgramRequest request = new DeleteKitfromProgramRequest();
            request.programId = programId;
            request.kitId = kitId;
            return reader.DeleteKitfromProgram(request);
        }

        public static AddEditKittoProgramResponse AddEditKittoProgram(int kitId, int programId, short order)
        {
            ProgramReader reader = new ProgramReader();
            AddEditKittoProgramRequest request = new AddEditKittoProgramRequest();
            KitsinProgramDto kitinProgram = new KitsinProgramDto();
            kitinProgram.ProgramId = programId;
            kitinProgram.KitId = kitId;
            kitinProgram.Order = order;
            request.kitinProgram = kitinProgram;
            var response = reader.AddEditKittoProgram(request);
            return response;
        }

        public static EnrollinProgramResponse EnrollinProgram(int userId, int ProgramsinPortalsId, int? hraId, int? CoachId, int? LoginId, string Language, int participantPortalId, int? integrationWith)
        {
            ProgramReader reader = new ProgramReader();
            EnrollinProgramRequest request = new EnrollinProgramRequest();
            request.UserId = userId;
            request.ProgramsinPortalsId = ProgramsinPortalsId;
            request.hraId = hraId;
            request.CoachId = CoachId;
            request.LoginId = LoginId;
            request.Language = Language;
            request.PortalId = participantPortalId;
            var response = reader.EnrollinProgram(request);
            if (response.success && response.ProgramType == (int)ProgramTypes.Coaching)
                AddNotification(userId, integrationWith);
            return response;
        }

        public static UpdateUserinProgramResponse UpdateUserinProgram(int? PrograminPortalId, int? CoachId, int? LoginId, byte? InactiveReasonId, string Language, bool AssignedFollowUp, int userinProgramId, int participantId, int participantPortalId, string orgContactEmail, string orgContactNumber, int systemAdminId, bool UpdateSubscriptionRenewal)
        {
            ProgramReader reader = new ProgramReader();
            UpdateUserinProgramRequest request = new UpdateUserinProgramRequest();
            request.UsersinProgramId = userinProgramId;
            if (PrograminPortalId.HasValue)
                request.PrograminPortalId = PrograminPortalId.Value;
            if (Language != null)
                request.Language = Language;
            if (CoachId.HasValue && CoachId > 0)
                request.CoachId = CoachId;
            if (InactiveReasonId.HasValue && InactiveReasonId > 0)
                request.InactiveReasonId = InactiveReasonId;
            if (AssignedFollowUp == true)
                request.AssignedFollowup = AssignedFollowUp;
            if (UpdateSubscriptionRenewal == true)
                request.UpdateSubscriptionRenewal = UpdateSubscriptionRenewal;
            request.userId = participantId;
            request.LoginId = LoginId;
            request.PortalId = participantPortalId;
            request.systemAdminId = systemAdminId;
            var response = reader.UpdateUserinProgram(request);
            if (AssignedFollowUp && response.success && response.user != null && !response.user.Email.Contains("noemail.myintervent.com") && !response.user.Email.Contains("samlnoemail.com"))
            {
                NotificationUtility.CreateFollowUpMessageEvent(NotificationEventTypeDto.FollowUp, response.user, orgContactEmail, orgContactNumber);
            }
            return response;
        }

        public static UpdateUserinProgramResponse ActivateUserinProgram(int UsersinProgramId, int LoginId, int participantId, int participantPortalId)
        {
            ProgramReader reader = new ProgramReader();
            UpdateUserinProgramRequest request = new UpdateUserinProgramRequest();
            request.PortalId = participantPortalId;
            request.UsersinProgramId = UsersinProgramId;
            request.LoginId = LoginId;
            request.userId = participantId;
            return reader.ActivateUserinProgram(request);
        }

        public static GetUserProgramHistoryResponse GetUserProgramHistory(int userId, string timeZone, string participantLanguagePreference)
        {
            ProgramReader reader = new ProgramReader();
            GetUserProgramHistoryRequest request = new GetUserProgramHistoryRequest();
            request.userId = userId;
            request.timeZone = timeZone;
            if (!string.IsNullOrEmpty(participantLanguagePreference))
                request.languageCode = participantLanguagePreference;

            return reader.GetUserProgramHistory(request);
        }

        public static IList<UsersinProgramDto> GetUserProgramsByPortal(int portalId, int participantId)
        {
            ProgramReader programReader = new ProgramReader();
            return programReader.GetUserProgramsByPortal(new GetUserProgramHistoryRequest { portalId = portalId, userId = participantId });
        }

        public static GetKitsHistoryforUserResponse GetKitsHistoryforUser(int userId)
        {
            ProgramReader reader = new ProgramReader();
            GetKitsHistoryforUserRequest request = new GetKitsHistoryforUserRequest();
            request.UserId = userId;
            return reader.GetKitsHistoryforUser(request);
        }

        public static AddKittoUserProgramResponse AddKittoUserProgram(int userinProgramId, int kitId, string participantLanguagePreference, int participantId, string orgContactEmail, string orgContactNumber, bool kitAlert)
        {
            ProgramReader reader = new ProgramReader();
            AddKittoUserProgramRequest request = new AddKittoUserProgramRequest();
            request.kitsId = kitId;
            request.userinProgramId = userinProgramId;
            request.languageCode = participantLanguagePreference;
            request.UserId = participantId;
            var response = reader.AddKittoUserProgram(request);
            if (response.success == true)
            {
                AccountReader acctReader = new AccountReader();
                var user = acctReader.GetUserById(request.UserId);
                if (!user.Email.Contains("noemail.myintervent.com") && !user.Email.Contains("samlnoemail.com"))
                    NotificationUtility.CreateKitNotificationEvent(NotificationEventTypeDto.ReviewKit, request.UserId, user.Email, response.KitName, orgContactNumber, orgContactEmail);
                if (user.CellNumber != null && user.Text == 1 && kitAlert)
                {
                    new TwilioManager().SendNewKitSms(user.Id);
                }
            }
            return response;
        }

        public static DeleteKitinUserProgramResponse DeleteKitinUserProgram(int id, int adminId)
        {
            ProgramReader reader = new ProgramReader();
            DeleteKitinUserProgramRequest request = new DeleteKitinUserProgramRequest();
            request.id = id;
            request.UpdatedBy = adminId;
            return reader.DeleteKitinUserProgram(request);
        }

        public static UsersinProgramDto GetUserProgramForHRA(int hraId)
        {
            ProgramReader reader = new ProgramReader();
            return reader.GetUserProgramForHRA(hraId);
        }

        public static IList<ProgramInactiveReasonDto> ListInactiveReasons()
        {
            ProgramReader reader = new ProgramReader();
            ListInactiveReasonRequest request = new ListInactiveReasonRequest();
            return reader.ListInactiveReasons(request).InactiveReasion;

        }

        public static GetProgramEnrollmentResponse GetProgramEnrollmentTask(int participantId)
        {
            ProgramReader reader = new ProgramReader();
            GetProgramEnrollmentRequest request = new GetProgramEnrollmentRequest();
            request.userId = participantId;
            request.taskTypeId = 17;
            return reader.GetProgramEnrollmentTask(request);
        }

        public static void AddNotification(int userId, int? integrationWith)
        {
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                NotificationUtility.CreateLMCProgramEvent(NotificationEventTypeDto.LMCProgram, userId);
            }
        }

        #region Appointment Template

        public static ListAppointmentTemplateResponse ListAppointmentCallTemplates()
        {
            ProgramReader reader = new ProgramReader();
            return reader.AppointmentTemplateList(new ListAppointmentTemplateRequest());
        }

        public static AddOrEditAppointmentIntervalResponse GetAppointmentCallIntervalForTemplate(int templateId)
        {
            ProgramReader reader = new ProgramReader();
            return reader.AppointmentTemplateIntervalList(new AddOrEditAppointmentIntervalRequest() { AppointmentTemplateId = templateId.ToString() });
        }
        public static AddOrEditAppointmentTemplateResponse ReadAppointmentCallTemplate(int templateid)
        {
            ProgramReader reader = new ProgramReader();
            AddOrEditAppointmentTemplateRequest request = new AddOrEditAppointmentTemplateRequest();
            request.templateid = templateid;
            return reader.ReadAppointmentTemplate(request);
        }
        public static AddOrEditAppointmentTemplateResponse SaveCallAppointmentTemplate(int? templateId, string templateName, int noOfWeeks, int noOfCalls, bool isActive, IEnumerable<int> intervals, int userId)
        {
            AddOrEditAppointmentTemplateRequest req = new AddOrEditAppointmentTemplateRequest();
            req.Template = new AppointmentCallTemplateDto();
            req.Template.TemplateName = templateName;
            req.Template.NoOfWeeks = noOfWeeks;
            req.Template.NoOfCalls = noOfCalls;
            req.Template.IsActive = isActive;
            req.Template.UpdatedDate = DateTime.UtcNow;
            req.Template.UpdatedBy = userId;
            req.templateid = templateId ?? null;
            List<AppointmentCallIntervalDto> callIntervals = new List<AppointmentCallIntervalDto>();
            int i = 1;
            foreach (var interval in intervals)
            {
                callIntervals.Add(new AppointmentCallIntervalDto() { CallNumber = i, IntervalInDays = interval });
                i++;
            }
            req.Template.CallIntervals = callIntervals;
            ProgramReader reader = new ProgramReader();
            return reader.CreateAppointmentTemplate(req);

        }

        public static Dictionary<int, DateTime> GetSuggestedCoachingDates(int? participantPortalId, int participantId, int programsInPortalId, int userinProgramId, int? hraId)
        {
            Dictionary<int, DateTime> suggestedDates = new Dictionary<int, DateTime>();

            PortalDto portal = null;
            if (participantPortalId.HasValue)
            {
                portal = PortalUtility.ReadPortal(participantPortalId.Value).portal;
                int enrolledProgramId = programsInPortalId;
                ProgramsinPortalDto programsInPortal = PortalUtility.GetProgramsByPortal(participantPortalId.Value).ProgramsinPortal.FirstOrDefault(x => x.Id == enrolledProgramId);
                if (programsInPortal != null)
                {
                    int numberOfSessionsInProgram = 0;
                    if (programsInPortal.ApptCallTemplate != null)
                    {
                        numberOfSessionsInProgram = programsInPortal.ApptCallTemplate.NoOfCalls;
                    }
                    int numberOfSessionsTaken = ParticipantUtility.GetCoachingCount(participantId, participantPortalId.Value, userinProgramId, hraId).count;

                    //portal end date or portal start date plus one year
                    DateTime portalEndDate = String.IsNullOrEmpty(portal.EndDate) ? DateTime.Parse(portal.StartDate).AddYears(1) : DateTime.Parse(portal.EndDate);
                    //add 1 day offset
                    int numOfDaysRemaining = (portalEndDate.AddDays(1) - DateTime.Now).Days;
                    double numOfSessionsToBeTaken = numberOfSessionsInProgram - numberOfSessionsTaken;
                    if (numOfSessionsToBeTaken > 0)
                    {
                        int interval = (int)Math.Floor(numOfDaysRemaining / numOfSessionsToBeTaken);
                        int? templateInterval = null;
                        if (programsInPortal.ApptCallTemplateId.HasValue)
                        {
                            templateInterval = GetAppointmentCallIntervalForTemplate(programsInPortal.ApptCallTemplateId.Value).CallIntervals.FirstOrDefault(x => x.CallNumber == (numberOfSessionsTaken + 1)).IntervalInDays;
                        }
                        if (templateInterval.HasValue)
                            interval = Math.Min(interval, templateInterval.Value);
                        else
                            interval = Math.Min(interval, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
                        DateTime dt = DateTime.Now;
                        dt = dt.AddDays(interval);
                        suggestedDates.Add((numberOfSessionsTaken + 1), dt);
                    }
                }
            }
            return suggestedDates;
        }

        public static void ChangeExercisePlanWeekStatus(int week, int hraId)
        {
            ProgramReader reader = new ProgramReader();
            ChangeExercisePlanWeekStatusRequest request = new ChangeExercisePlanWeekStatusRequest();
            request.week = week;
            request.hraId = hraId;
            reader.ChangeExercisePlanWeekStatus(request);
        }
        #endregion

        public static ProgramWellnessDataModel GetProgramWellnessData(int unit, int participantId, string uniqueId, int organizationId)
        {
            ProgramWellnessDataModel model = new ProgramWellnessDataModel();
            model.Measurements = CommonUtility.ListMeasurements(unit).Measurements;
            var wellnessData = ParticipantUtility.ListWellnessData(participantId);
            var recentWellnessData = wellnessData.WellnessData.LastOrDefault();
            var recentWellnessDataforWeight = wellnessData.WellnessData.Where(x => x.Weight.HasValue).LastOrDefault();
            var recentWellnessDataforWaist = wellnessData.WellnessData.Where(x => x.waist.HasValue).LastOrDefault();
            var lastRecentWellnessData = wellnessData.WellnessData.Where(x => x.Weight.HasValue).OrderByDescending(x => x.Id).Skip(1).Take(1).FirstOrDefault();
            var recentWellnessDataforBP = wellnessData.WellnessData.Where(x => x.SBP.HasValue).LastOrDefault();
            var recentWellnessDataforWellnessVision = wellnessData.WellnessData.Where(x => x.WellnessVision != null).LastOrDefault();
            var glucoseData = ParticipantUtility.ListGlucoseData(!string.IsNullOrEmpty(uniqueId) ? uniqueId : null, organizationId, true).listGlucose;
            model.Glucose = glucoseData.Count != 0 ? glucoseData.FirstOrDefault().Value.ToString() + " " + @Translate.Message(model.Measurements[BioLookup.Glucose].MeasurementUnit) : "N/A";
            model.WeightStatus = "up-arrow";
            if (lastRecentWellnessData != null)
            {
                if (recentWellnessDataforWeight.Weight.Value < lastRecentWellnessData.Weight.Value)
                    model.WeightStatus = "down-arrow";
                model.WeightDiff = (float)Math.Round((recentWellnessDataforWeight.Weight.Value - lastRecentWellnessData.Weight.Value), 1);
            }
            if (recentWellnessData != null && recentWellnessData.CollectedOn.Date == DateTime.UtcNow.Date)
            {
                model.wellnessDataId = recentWellnessData.Id;
            }
            if (model.WeightDiff != 0 && unit == (int)Unit.Metric)
            {
                model.WeightDiff = (float)Math.Round(CommonUtility.ToMetric(model.WeightDiff, BioLookup.Weight, unit), 1);
            }
            if (recentWellnessDataforWellnessVision != null)
                model.WellnessVision = recentWellnessDataforWellnessVision.WellnessVision;
            if (recentWellnessDataforBP != null)
            {
                model.SBP = recentWellnessDataforBP.SBP;
                model.DBP = recentWellnessDataforBP.DBP;
            }
            if (recentWellnessDataforWeight != null && recentWellnessDataforWeight.Weight.HasValue)
                model.Weight = unit == (int)Unit.Metric ? (float)Math.Round(CommonUtility.ToMetric(recentWellnessDataforWeight.Weight.Value, BioLookup.Weight, unit), 1) : (float)Math.Round(recentWellnessDataforWeight.Weight.Value, 1);
            if (recentWellnessDataforWaist != null && recentWellnessDataforWaist.waist.HasValue)
                model.Waist = unit == (int)Unit.Metric ? (float)Math.Round(CommonUtility.ToMetric(recentWellnessDataforWaist.waist.Value, BioLookup.Waist, unit), 1) : (float)Math.Round(recentWellnessDataforWaist.waist.Value, 1);
            return model;
        }
    }
}