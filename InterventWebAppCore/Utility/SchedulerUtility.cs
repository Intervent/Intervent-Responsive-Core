using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class SchedulerUtility
    {
        public static GetAppointmentsResponse GetAppointments(DateTime? startDate, DateTime? endDate, int? coachId, int? userId, string timeZone, int? participantPortalId)
        {
            SchedulerReader reader = new SchedulerReader();
            GetAppointmentsRequest request = new GetAppointmentsRequest();
            request.StartDate = startDate;
            request.EndDate = endDate;
            if (timeZone != null)
                request.TimeZone = timeZone;
            else
                request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            request.coachId = coachId;
            request.userId = userId;
            if (participantPortalId.HasValue)
            {
                PortalReader portalReader = new PortalReader();
                ReadPortalRequest portalRequest = new ReadPortalRequest();
                portalRequest.portalId = participantPortalId.Value;
                var portalResponse = portalReader.ReadPortal(portalRequest);
                request.portalStartDate = Convert.ToDateTime(portalResponse.portal.StartDate);
                request.portalEndDate = Convert.ToDateTime(portalResponse.portal.EndDate);
            }
            return reader.GetAppointments(request);
        }

        public static GetAppointmentsCountResponse GetAppointmentsCount(DateTime startDate, DateTime endDate, int? coachId, string timeZone)
        {
            SchedulerReader reader = new SchedulerReader();
            GetAppointmentsCountRequest request = new GetAppointmentsCountRequest();
            request.StartDate = startDate;
            request.EndDate = endDate;
            request.TimeZone = timeZone;
            request.coachId = coachId;
            return reader.GetAppointmentsCount(request);
        }

        public static CheckFreeSlotsResponse CheckFreeSlot(DateTime startDate, DateTime endDate, int? coachId, string timeZone, string languages, bool thirtyMinutes, int? organizationId, int? integrationWith, int? stateId, bool isPregnant)
        {
            SchedulerReader reader = new SchedulerReader();
            GetFreeSlotsRequest request = new GetFreeSlotsRequest();
            request.StartDate = startDate;
            request.EndDate = endDate;
            request.TimeZone = timeZone;
            request.coachId = coachId;
            request.languages = languages;
            request.thirtyMinutes = thirtyMinutes;
            if (organizationId.HasValue)
                request.OrganizationId = organizationId.Value;
            request.adminView = false;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.isLMC = true;
                request.stateId = stateId;
            }
            if (isPregnant)
            {
                request.specialities = Constants.MaternityManagement;
            }
            request.hasFilter = true;
            return reader.CheckFreeSlot(request);
        }

        public static GetFreeSlotsResponse GetFreeSlotforDay(DateTime startDate, DateTime endDate, int? coachId, string timeZone, string languages, bool thirtyMinutes, int? organizationId, int? integrationWith, int? stateId, bool isPregnant)
        {
            SchedulerReader reader = new SchedulerReader();
            GetFreeSlotsRequest request = new GetFreeSlotsRequest();
            request.StartDate = startDate;
            request.EndDate = endDate;
            request.TimeZone = timeZone;
            request.coachId = coachId;
            request.thirtyMinutes = thirtyMinutes;
            request.languages = languages;
            if (organizationId.HasValue)
                request.OrganizationId = organizationId.Value;
            request.adminView = false;
            request.hasFilter = true;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.stateId = stateId;
            }
            if (isPregnant)
            {
                request.specialities = Constants.MaternityManagement;
            }
            return reader.GetFreeSlotforDay(request);
        }

        public static ScheduleAppointmentResponse ScheduleAppointment(int? appId, DateTime Date, int? Type, int coachId, byte minutes, string Comments, int createdBy, int participantId, string timeZone, bool fromAdmin, bool videoRequired, string meetingId, int? integrationWith, int? userinProgramId, int? programType, int? hraId, int participantPortalId, int? hasHRA, bool assignPrograms, int userId, string orgContactEmail, string orgContactNumber, int southUniversityOrgId)
        {
            SchedulerReader reader = new SchedulerReader();
            ScheduleAppointmentRequest request = new ScheduleAppointmentRequest();
            AppointmentDTO appDto = new AppointmentDTO();
            if (appId.HasValue)
                appDto.Id = appId.Value;
            if (Type.HasValue)
                appDto.Type = Type.Value;
            appDto.CoachId = coachId;
            appDto.Minutes = minutes;
            appDto.Comments = Comments;
            request.TimeZone = timeZone;
            appDto.Date = Date.ToString();
            request.Appointment = appDto;
            request.Appointment.CreatedBy = createdBy;
            request.Appointment.UserId = participantId;
            request.Appointment.VideoRequired = videoRequired;
            var appointmentResponse = reader.ScheduleAppointment(request);
            if (appointmentResponse.Status != 2)
            {
                if (userinProgramId.HasValue && programType.HasValue
                    && programType == (int)ProgramTypes.Coaching)
                {
                    IncentiveReader incentiveReader = new IncentiveReader();
                    Tobacco_IncentiveRequest incentiveRequest = new Tobacco_IncentiveRequest();
                    incentiveRequest.userId = participantId;
                    if (hraId.HasValue)
                        incentiveRequest.hraId = hraId.Value;
                    incentiveRequest.usersInProgramId = userinProgramId.Value;
                    incentiveRequest.portalId = participantPortalId;
                    incentiveRequest.southUniversityOrgId = southUniversityOrgId;
                    incentiveReader.Tobacco_Incentive(incentiveRequest);
                }
                else if (Type.HasValue && Type == 1 && fromAdmin == false)
                {
                    ProgramReader programReader = new ProgramReader();
                    GetProgramsByPortalRequest Portalrequest = new GetProgramsByPortalRequest();
                    ProgramsinPortalDto programsinPortal = new ProgramsinPortalDto();
                    GetInfoforProgramResponse infoforProgram = new GetInfoforProgramResponse();
                    Portalrequest.PortalId = participantPortalId;
                    Portalrequest.onlyActive = true;
                    if (hasHRA.HasValue && hasHRA.Value == (int)HRAStatus.No)
                    {
                        var portalPrograms = programReader.GetProgramsByPortal(Portalrequest);
                        if (portalPrograms.ProgramsinPortal.Count() > 0)
                        {
                            programsinPortal = portalPrograms.ProgramsinPortal.FirstOrDefault();
                        }
                    }
                    else
                    {
                        infoforProgram = ParticipantUtility.GetInfoforProgram(participantId);
                        if (infoforProgram.HRA != null && infoforProgram.HRA.CompleteDate.HasValue)
                        {
                            if (assignPrograms)
                            {
                                int riskLevel = infoforProgram.HRA.RiskCode.StartsWith("L") ? 1 : infoforProgram.HRA.RiskCode.StartsWith("M") ? 2 : infoforProgram.HRA.RiskCode.StartsWith("H") ? 3 : 0;
                                var portalPrograms = programReader.GetProgramsByPortal(Portalrequest);
                                if (portalPrograms.ProgramsinPortal.Count() > 1)
                                    programsinPortal = portalPrograms.ProgramsinPortal.Where(x => (x.program.RiskLevel == riskLevel || CommonUtility.IsIntegratedWithLMC(integrationWith)) && x.program.ProgramType == 2 && !x.program.Smoking).FirstOrDefault();
                                else if (portalPrograms.ProgramsinPortal.Count() == 1)
                                    programsinPortal = portalPrograms.ProgramsinPortal.FirstOrDefault();
                            }
                            else
                            {
                                if (infoforProgram.HRA.RiskCode.Contains("DL9"))
                                {
                                    programsinPortal = programReader.GetProgramsByPortal(Portalrequest).ProgramsinPortal.Where(x => x.program.Pregancy == true).FirstOrDefault();
                                }
                                else if (ParticipantUtility.GetPrevYearStatus(participantId, timeZone, "").prevPortal)
                                {
                                    int riskLevel = infoforProgram.HRA.RiskCode.StartsWith("L") ? 1 : infoforProgram.HRA.RiskCode.StartsWith("M") ? 2 : infoforProgram.HRA.RiskCode.StartsWith("H") ? 3 : 0;
                                    programsinPortal = programReader.GetProgramsByPortal(Portalrequest).ProgramsinPortal.Where(x => (x.program.RiskLevel == riskLevel || CommonUtility.IsIntegratedWithLMC(integrationWith)) && x.program.ProgramType == 2 && !x.program.Smoking).FirstOrDefault();
                                }
                            }
                        }
                    }
                    if (programsinPortal != null && programsinPortal.Id > 0)
                    {
                        EnrollinProgramRequest programRequest = new EnrollinProgramRequest();
                        programRequest.ProgramsinPortalsId = programsinPortal.Id;
                        programRequest.UserId = participantId;
                        if (infoforProgram.HRA != null)
                            programRequest.hraId = infoforProgram.HRA.Id;
                        programRequest.CoachId = coachId;
                        programRequest.LoginId = userId;
                        programRequest.PortalId = participantPortalId;
                        appointmentResponse.enrollinProgramResponse = programReader.EnrollinProgram(programRequest);
                        if (appointmentResponse.enrollinProgramResponse.success)
                            ProgramUtility.AddNotification(participantId, integrationWith);
                    }
                }
                if (NotificationUtility.hasValidEmail(participantId))
                {
                    NotificationUtility.CreateAppointmentNotificationEvent(NotificationEventTypeDto.AppointmentConfirmation, participantId, coachId, Date, minutes, orgContactEmail, orgContactNumber, meetingId, videoRequired);
                    var dateStr = Date.ToLongDateString();
                    appointmentResponse.ConfirmationMessage = String.Format(Translate.Message("L3656"), dateStr);
                }
            }
            return appointmentResponse;
        }

        public static SelectList GetAppointmentLength()
        {
            List<KeyValue> length = new List<KeyValue>();
            length.Add(new KeyValue() { Text = "--Select--" });
            length.Add(new KeyValue() { Value = "15", Text = "15 Minutes" });
            length.Add(new KeyValue() { Value = "30", Text = "30 Minutes" });
            SelectList appointmentLength = new SelectList(length, "Value", "Text");

            return appointmentLength;
        }

        public static IList<AppointmentTypesDto> GetAppointmentType()
        {
            SchedulerReader reader = new SchedulerReader();
            GetAppointmentTypesRequest request = new GetAppointmentTypesRequest();
            return reader.GetAppointmentType(request).AppointmentTypes;

        }

        public static SelectList GetInactiveReason()
        {
            List<KeyValue> days = new List<KeyValue>();
            days.Add(new KeyValue() { Value = "1", Text = Translate.Message("L979") });
            days.Add(new KeyValue() { Value = "2", Text = Translate.Message("L567") });
            days.Add(new KeyValue() { Value = "3", Text = Translate.Message("L573") });
            days.Add(new KeyValue() { Value = "4", Text = Translate.Message("L574") });
            days.Add(new KeyValue() { Value = "5", Text = Translate.Message("L575") });
            SelectList daysList = new SelectList(days, "Value", "Text");
            return daysList;
        }

        public static CoachListResponse GetCoachList(bool? active, bool? allowAppt, int? organizationId, int? adminId, int? integrationWith, int? stateId)
        {
            AccountReader reader = new AccountReader();
            CoachListRequest request = new CoachListRequest();
            request.active = active;
            request.allowAppt = allowAppt;
            if (organizationId.HasValue)
                request.OrganizationIds = organizationId.Value.ToString();
            else if (adminId.HasValue)
            {
                PortalReader portalReader = new PortalReader();
                var organizationsList = portalReader.GetFilteredOrganizationsList(adminId.Value).Organizations.Select(x => x.Id).ToList();
                request.OrganizationIds = string.Join(",", organizationsList.Select(x => x.ToString()).ToArray());
            }
            if (integrationWith.HasValue && CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.stateId = stateId;
            }
            return reader.GetCoachList(request);
        }

        public static FilteredCoachListResponse GetFilteredCoachList(string coachName, int? speciality, string language, string startdate, string time, bool? byCoach, string timezone, bool isPregnant, int? OrganizationId, int? integrationWith, int? stateId)
        {
            AccountReader reader = new AccountReader();
            FilteredCoachListRequest request = new FilteredCoachListRequest();
            request.coachName = coachName;
            if (isPregnant)
            {
                speciality = Convert.ToInt32(Constants.MaternityManagement);
            }
            if (speciality.HasValue)
                request.speciality = speciality.ToString();
            request.language = language;
            if (!string.IsNullOrEmpty(startdate))
            {
                request.startDate = Convert.ToDateTime(startdate + time);
                request.endDate = request.startDate.Value.AddMinutes(30);
            }
            request.byCoach = byCoach;
            if (OrganizationId.HasValue)
                request.OrganizationId = OrganizationId.Value;
            request.TimeZone = timezone;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.stateId = stateId;
            }
            return reader.GetFilteredCoachList(request);
        }

        public static GetFreeSlotsResponse SearchFreeSlot(DateTime StartDateTime, DateTime EndDateTime, int? coachId, string TimeZone, string day, bool thirtyMinutes,
            bool? video, bool? healthcoach, int? organizationId, int? integrationWith, int? stateId, string specialities = null, string languages = null)
        {
            SchedulerReader reader = new SchedulerReader();
            GetFreeSlotsRequest request = new GetFreeSlotsRequest();
            request.StartDate = StartDateTime;
            request.EndDate = EndDateTime;
            request.coachId = coachId;
            if (healthcoach.HasValue && healthcoach.Value == true)
                request.adminView = false;
            else
                request.adminView = true;
            if (day != "")
                request.day = day.Split('-');
            request.thirtyMinutes = thirtyMinutes;
            if (video.HasValue && video.Value == true)
                request.video = video;
            if (specialities != null)
                request.specialities = specialities;
            if (languages != null)
                request.languages = languages;
            request.TimeZone = TimeZone;
            if (organizationId.HasValue)
                request.OrganizationId = organizationId.Value;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.stateId = stateId;
            }
            request.hasFilter = true;
            return reader.SearchFreeSlot(request);
        }

        public static GetAppointmentsResponse ListAppointments(DateTime StartDateTime, DateTime EndDateTime, int? coachId, int page, int pageSize, int? totalRecords, int adminId)
        {
            SchedulerReader reader = new SchedulerReader();
            GetAppointmentsRequest request = new GetAppointmentsRequest();
            request.StartDate = StartDateTime;
            request.EndDate = EndDateTime;
            request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            request.coachId = coachId;
            request.userId = adminId;
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.ListAppointments(request);
        }

        public static GetCoachAvailabilityResponse GetCoachAvailability(int coachId)
        {
            SchedulerReader reader = new SchedulerReader();
            GetCoachAvailabilityRequest request = new GetCoachAvailabilityRequest();
            request.timeZone = System.Web.HttpContext.Current.User.TimeZone();
            request.coachId = coachId;
            return reader.GetCoachAvailability(request);
        }

        public static GetCoachAvailabilityDetailsResponse GetCoachAvailabilityDetails(long refId, string startDate)
        {
            SchedulerReader reader = new SchedulerReader();
            GetCoachAvailabilityDetailsRequest request = new GetCoachAvailabilityDetailsRequest();
            request.RefId = refId;
            if (!string.IsNullOrEmpty(startDate))
                request.StartDate = Convert.ToDateTime(startDate);
            request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            return reader.GetCoachAvailabilityDetails(request);
        }

        public static SetCoachAvailabilityResponse SetCoachAvailability(DateTime startDateTime, DateTime endDateTime, string Days, DateTime? fromDate, DateTime? toDate, int coachId)
        {
            SchedulerReader reader = new SchedulerReader();
            SetCoachAvailabilityRequest request = new SetCoachAvailabilityRequest();
            request.FromDate = fromDate;
            request.ToDate = toDate;
            request.StartDateTime = startDateTime;
            request.EndDateTime = endDateTime;
            request.Days = Days;
            request.coachId = coachId;
            request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            request.CreatedBy = Convert.ToInt32(System.Web.HttpContext.Current.User.UserId());
            return reader.SetCoachAvailability(request);
        }

        public static DeleteCoachAvailabilityResponse DeleteCoachAvailability(long refId, DateTime startTime, DateTime endTime, DateTime? ToDate, bool? AllFuture, string days)
        {
            SchedulerReader reader = new SchedulerReader();
            DeleteCoachAvailabilityRequest request = new DeleteCoachAvailabilityRequest();
            request.RefId = refId;
            request.startTime = startTime;
            request.endTime = endTime;
            if (ToDate.HasValue)
                request.ToDate = ToDate.Value.AddDays(1);
            request.AllFuture = AllFuture;
            request.Days = days;
            request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            request.UpdatedBy = Convert.ToInt32(System.Web.HttpContext.Current.User.UserId());
            return reader.DeleteCoachAvailability(request);
        }

        public static IList<CancellationReasonDto> GetCancellationReasons(int appid)
        {
            SchedulerReader reader = new SchedulerReader();
            GetCancellationReasonRequest request = new GetCancellationReasonRequest();
            request.AppId = appid;
            return reader.GetCancellationReasons(request).ReasonTypes;

        }

        public static bool CancelAppointment(int id, byte reason, string comments, int updatedBy, int adminId, string intuityURL, string authToken)
        {
            SchedulerReader reader = new SchedulerReader();
            CancelAppointmentRequest request = new CancelAppointmentRequest();
            request.id = id;
            request.reason = reason;
            request.updatedBy = updatedBy;
            CancelAppointmentResponse response = reader.CancelAppointment(request);
            if (!String.IsNullOrEmpty(comments))
            {
                ParticipantReader participantReader = new ParticipantReader();
                NotesDto noteDto = new NotesDto();
                noteDto.Admin = adminId;
                //Need to check the scenario where there is no active portal
                noteDto.PortalId = response.portalId;
                noteDto.userId = response.userId;
                noteDto.NotesDate = DateTime.MinValue;
                noteDto.RefId2 = id;
                noteDto.Type = (int)NoteTypes.Other;
                noteDto.Text = comments;
                participantReader.AddEditNotes(new AddNotesRequest { note = noteDto, TimeZone = System.Web.HttpContext.Current.User.TimeZone() });
            }
            if (reason == 5)
            {
                if (NotificationUtility.hasValidEmail(response.userId))
                {
                    CreateMissedApptEventRequest eventRequest = new CreateMissedApptEventRequest();
                    eventRequest.evt = NotificationEventTypeDto.MissedAppointment;
                    eventRequest.userId = response.userId;
                    eventRequest.coachId = response.coachId;
                    eventRequest.orgContactEmail = response.orgContactEmail;
                    eventRequest.orgContactNumber = response.orgContactNumber;
                    NotificationUtility.CreateMissedAppointmentEvent(eventRequest);
                }
                //if (response.integrationWith.HasValue && response.integrationWith.Value == (int)IntegrationPartner.Intuity)
                //{
                //    new IntuityManager().SendNoShow(response.userId, response.AptTime, intuityURL, authToken);
                //}
            }
            return response.success;
        }

        public static EditApptCommentResponse EditAppointment(int id, string comments, int Length, bool videoRequired, string meetingId, int userId, int? integrationWith, int? stateId, string participantTimeZoneName, string orgContactEmail, string orgContactNumber)
        {
            SchedulerReader reader = new SchedulerReader();
            EditApptCommentRequest request = new EditApptCommentRequest();
            request.id = id;
            request.comments = comments;
            request.length = Length;
            request.timezone = System.Web.HttpContext.Current.User.TimeZone();
            request.videoRequired = videoRequired;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timezone);
            request.updatedBy = userId;
            if (CommonUtility.IsIntegratedWithLMC(integrationWith))
            {
                request.stateId = stateId;
            }
            var response = reader.EditAppointment(request);
            if (response.success && response.changeinAppointment)
            {
                if (NotificationUtility.hasValidEmail(response.appointment.UserId))
                {
                    NotificationUtility.CreateAppointmentNotificationEvent(NotificationEventTypeDto.AppointmentConfirmation, response.appointment.UserId, response.appointment.CoachId, TimeZoneInfo.ConvertTimeFromUtc(DateTime.Parse(response.appointment.Date), TimeZoneInfo.FindSystemTimeZoneById(participantTimeZoneName)), response.appointment.Minutes, orgContactEmail, orgContactNumber, meetingId, videoRequired);
                }
            }
            return response;
        }

        public static bool RevertNoShow(int id)
        {
            SchedulerReader reader = new SchedulerReader();
            return reader.RevertNoShow(id);
        }

        public static GetCoachCalendarResponse GetCoachCalendar(int coachId, bool? userTimeZone, string participantTimeZone)
        {
            SchedulerReader reader = new SchedulerReader();
            GetCoachCalendarRequest request = new GetCoachCalendarRequest();
            request.CoachId = coachId;
            request.FromDate = DateTime.UtcNow;
            request.ToDate = request.FromDate.AddMonths(6);
            if (userTimeZone.HasValue && userTimeZone.Value && !string.IsNullOrEmpty(participantTimeZone))
                request.TimeZoneId = participantTimeZone;
            else
                request.TimeZoneId = System.Web.HttpContext.Current.User.TimeZone();
            return reader.GetCoachCalendar(request);
        }

        public static AppointmentMoveResponse MoveAppointments(List<AppointmentDTO> Appointments, int coachId, string ToCoachIds)
        {
            SchedulerReader reader = new SchedulerReader();
            MoveAppointmentsRequest request = new MoveAppointmentsRequest();
            request.Appointments = Appointments;
            request.CoachId = coachId;
            request.ToCoachIds = ToCoachIds;
            request.TimeZone = System.Web.HttpContext.Current.User.TimeZone();
            return reader.MoveAppointments(request);
        }

        public static bool AddAppointmentFeedback(int id, int rating, string comments)
        {
            SchedulerReader reader = new SchedulerReader();
            AddAppointmentFeedbackRequest request = new AddAppointmentFeedbackRequest();
            request.id = id;
            request.rating = rating;
            request.comments = comments;
            return reader.AddAppointmentFeedback(request);
        }

        public static GetAppointmentDetailsResponse GetAppointmentDetails(int id, string timeZone)
        {
            SchedulerReader reader = new SchedulerReader();
            GetAppointmentDetailsRequest request = new GetAppointmentDetailsRequest();
            request.apptId = id;
            if (!string.IsNullOrEmpty(timeZone))
                request.timeZone = timeZone;
            else
                request.timeZone = System.Web.HttpContext.Current.User.TimeZone();
            return reader.GetAppointmentDetails(request);
        }
    }
}