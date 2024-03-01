using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using NLog;
using System.Data;

namespace Intervent.Web.DataLayer
{
    public class ParticipantReader : BaseDataReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ReadUserParticipationResponse ReadUserParticipation(ReadUserParticipationRequest request)
        {
            ReadUserParticipationResponse response = new ReadUserParticipationResponse();
            var user = context.Users.Include("TimeZone").Include("HRAs").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Include("UsersinPrograms.FollowUps")
                .Include("Organization").Include("Organization.Portals").Include("Organization.Portals.PortalIncentives").Include("Country1").Where(x => x.Id == request.UserId).FirstOrDefault();
            user.UserLogs = context.UserLogs.Where(x => x.UserId == request.UserId).ToList();

            response.user = Utility.mapper.Map<DAL.User, UserDto>(user);
            if (user.TimeZone != null)
            {
                response.user.TimeZone = user.TimeZone.TimeZoneId;
                response.user.TimeZoneName = user.TimeZone.TimeZone1;
            }
            if (user.Organization.Portals.Where(x => x.Active == true).Count() == 0)
                response.hasActivePortal = false;
            else
            {
                response.hasActivePortal = true;
                response.Portal = Utility.mapper.Map<DAL.Portal, PortalDto>(user.Organization.Portals.Where(x => x.Active == true).OrderByDescending(x => x.Id).FirstOrDefault());

                var hra = user.HRAs.Where(x => x.PortalId == response.Portal.Id && x.Portal.Active == true).OrderByDescending(x => x.Id).FirstOrDefault();
                if (hra != null)
                {
                    response.HRA = Utility.mapper.Map<DAL.HRA, HRADto>(hra);
                }
                var userinProgram = user.UsersinPrograms.Where(x => x.ProgramsinPortal.PortalId == response.Portal.Id).OrderByDescending(x => x.Id).FirstOrDefault();
                if (userinProgram != null)
                {
                    response.usersinProgram = Utility.mapper.Map<DAL.UsersinProgram, UsersinProgramDto>(userinProgram);
                }
                var appointment = context.Appointments.Where(x => x.UserId == request.UserId && x.Date > DateTime.Now && x.Active == true).OrderBy(x => x.Date).FirstOrDefault();
                if (appointment != null)
                {
                    response.appointment = Utility.mapper.Map<DAL.Appointment, AppointmentDTO>(appointment);
                }
                var eligibility = context.Eligibilities.Where(x => x.UniqueId == user.UniqueId && x.PortalId == response.Portal.Id).FirstOrDefault();
                if (eligibility != null)
                {
                    response.UserStatus = eligibility.UserStatus;
                }
            }
            return response;
        }

        public int GetDuplicateEligiblityRecords(int portalId)
        {
            var count = 0;
            var eligibilities = context.Eligibilities.Where(x => x.PortalId == portalId && x.IsActive != false).GroupBy(x => new { x.FirstName, x.LastName, x.DOB }).Where(x => x.Count() > 1).ToList();
            foreach (var eligiblitygroup in eligibilities)
            {
                var selfReferral = eligiblitygroup.Where(x => x.BusinessUnit != "LMC").ToList();
                var lmcReferral = eligiblitygroup.Where(x => x.BusinessUnit == "LMC").OrderBy(x => x.Id).ToList();
                var oldReferral = new Eligibility();
                var newReferral = new Eligibility();
                if (lmcReferral.Count() > 1)
                {
                    oldReferral = lmcReferral.FirstOrDefault();
                    newReferral = lmcReferral.LastOrDefault();
                }
                else
                {
                    oldReferral = selfReferral.FirstOrDefault();
                    newReferral = lmcReferral.FirstOrDefault();
                }
                if (oldReferral == null || newReferral == null)
                    continue;
                var user = context.Users.Where(x => x.UniqueId == oldReferral.UniqueId).FirstOrDefault();
                if (user != null)
                {
                    user.UniqueId = newReferral.UniqueId;
                    context.Users.Attach(user);
                    context.Entry(user).State = EntityState.Modified;
                    count++;
                }
                if (lmcReferral.Count() == 1)
                {
                    oldReferral.IsActive = false;
                    context.Eligibilities.Attach(oldReferral);
                    context.Entry(oldReferral).State = EntityState.Modified;
                    var canrisk = context.CanriskQuestionnaires.Where(x => x.EligibilityId == oldReferral.Id).FirstOrDefault();
                    if (canrisk != null)
                    {
                        canrisk.EligibilityId = newReferral.Id;
                        context.CanriskQuestionnaires.Attach(canrisk);
                        context.Entry(canrisk).State = EntityState.Modified;
                    }
                }
                else
                {
                    newReferral.IsActive = false;
                    context.Eligibilities.Attach(newReferral);
                    context.Entry(newReferral).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
            return count;
        }

        public string GetReferralType(string uniqueId)
        {
            var eligibility = context.Eligibilities.Where(x => x.UniqueId == uniqueId).FirstOrDefault();
            if (eligibility != null && eligibility.BusinessUnit == "LMC")
            {
                return "Physician Referral";
            }
            return "Self Referral";
        }

        public EligibilityDto GetEligibilityByUniqueId(string uniqueId, int? portalId)
        {
            var eligibility = context.Eligibilities.Include("Portal").Include("Portal.Organization").Where(x => x.UniqueId == uniqueId && (!portalId.HasValue || x.PortalId == portalId)).OrderByDescending(x => x.Id).ToList();
            if (eligibility != null && eligibility.Count > 0)
            {
                return MapToEligibilityDto(eligibility[0]);
            }
            return null;
        }

        public List<string> GetProfilePictures()
        {
            var pics = context.Users.Where(x => !String.IsNullOrEmpty(x.Picture)).ToList().Select(x => x.Picture).ToList();
            return pics;
        }

        public ReadParticipantInfoResponse ReadParticipantInfo(ReadParticipantInfoRequest request)
        {
            ReadParticipantInfoResponse response = new ReadParticipantInfoResponse();
            StoredProcedures sp = new StoredProcedures();
            var userlist = sp.ParticipantProfile(request.userId, request.adminId);
            if (userlist == null)
            {
                response.success = false;
                return response;
            }
            var participant = Utility.mapper.Map<DAL.ParticipantProfile_Result, ParticipantProfile_ResultDto>(userlist);
            var userId = participant.UserId;
            var portalId = participant.PortalId != null ? participant.PortalId : 0;
            var hraId = participant.HRAId != null ? participant.HRAId : 0;
            if (userId != 0)
            {
                AccountReader accreader = new AccountReader();
                SchedulerReader schedulerReader = new SchedulerReader();
                GetUserRequest userrequest = new GetUserRequest();
                userrequest.id = userId;
                var usr = accreader.ReadUser(userrequest);
                response.user = usr.User;
            }
            if (hraId != 0)
            {
                HRAReader hrareader = new HRAReader();
                ReadHRARequest hrarequest = new ReadHRARequest();
                hrarequest.hraId = hraId.Value;
                hrarequest.portalId = portalId.Value;
                var hra = hrareader.ReadHRA(hrarequest);
                response.hra = hra.hra;
            }

            if (portalId != 0)
            {
                PortalReader reader = new PortalReader();
                ReadPortalRequest portalRequest = new ReadPortalRequest();
                portalRequest.portalId = portalId.Value;
                var portal = reader.ReadPortal(portalRequest);
                response.portal = portal.portal;
            }
            if (userId != 0 && portalId != 0)
            {
                GetUserFormsRequest userFormsRequest = new GetUserFormsRequest();
                userFormsRequest.userId = userId;
                userFormsRequest.portalId = portalId.Value;
                response.UserForms = GetUserForms(userFormsRequest).userForms;
            }
            if (portalId != 0 && response.portal.LabIntegration)
            {
                LabReader labreader = new LabReader();
                ReadLabRequest labRequest = new ReadLabRequest();
                labRequest.UserId = userId;
                var labs = labreader.ListLabs(labRequest).Labs;
                if (labs != null && labs.Count > 0)
                    response.labNo = labs.Where(x => x.PortalId == response.portal.Id).Count();
                response.lab = labs.FirstOrDefault();
            }
            if (participant.UPId != null && portalId != 0)
            {
                ProgramReader programReader = new ProgramReader();
                var userinProgram = programReader.GetUserProgramHistory(new GetUserProgramHistoryRequest { userId = userId, portalId = portalId.Value, timeZone = participant.TimeZoneId });
                if (userinProgram.usersinPrograms != null)
                {
                    response.UsersinProgram = userinProgram.usersinPrograms.ToList();
                }
            }
            if (!string.IsNullOrEmpty(response.user.UniqueId))
            {
                var claims = context.InsuranceSummaries.Where(x => x.UniqueID == response.user.UniqueId && x.OrganizationId == response.user.OrganizationId).OrderByDescending(x => x.LastModifiedDate).FirstOrDefault();
                if (claims != null)
                {
                    var lastclaim = context.CandidateReasonForLastChanges.Where(x => x.ClaimsId == claims.ID).OrderByDescending(x => x.ConditionDate).FirstOrDefault();
                    if (lastclaim != null)
                    {
                        response.ConditionType = lastclaim.ConditionType;
                        response.ConditionDate = lastclaim.ConditionDate.Value;
                    }
                }
            }

            response.participant = participant;
            response.success = true;
            return response;
        }

        public int CompIntroKitsOnTime(int portalId)
        {
            int count = 0;
            CommonReader commonReader = new CommonReader();
            var canriskUsers = context.CanriskQuestionnaires.Include("Eligibility").Where(x => x.CompletedOn != null && x.Eligibility != null).ToList();
            var canriskUsersUniqueId = canriskUsers.Select(y => y.Eligibility.UniqueId);
            var users = context.Users.Include("Labs2").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Include("UsersinPrograms.ProgramsinPortal.Program.KitsinPrograms").Include("UsersinPrograms.KitsinUserPrograms").
                        Where(x => (canriskUsersUniqueId.Contains(x.UniqueId)) && (x.Labs2.Where(y => y.PortalId == portalId && y.DateCompleted != null).Count() > 0)
                        && x.UsersinPrograms.Where(y => y.IsActive == true && y.ProgramsinPortal.PortalId == portalId && y.CompIntroKitsOnTime == null).FirstOrDefault().ProgramsinPortal.Program.KitsinPrograms.Count() > 0
                        && x.UsersinPrograms.Where(y => y.IsActive == true && y.ProgramsinPortal.PortalId == portalId && y.CompIntroKitsOnTime == null).FirstOrDefault().KitsinUserPrograms.Where(z => z.IsActive).Count() > 0).ToList();
            foreach (var user in users)
            {
                var canriskUser = context.CanriskQuestionnaires.Include("Eligibility").Where(x => x.Eligibility.UniqueId == user.UniqueId).FirstOrDefault();
                var dateRange = canriskUser.CompletedOn.Value.AddDays(28);
                if ((DateTime.Now - canriskUser.CompletedOn.Value).TotalDays >= 28)
                {
                    count = count + 1;
                    bool CompIntroKitsOnTime = false;
                    var userinPrograms = user.UsersinPrograms.Where(y => y.IsActive == true && y.ProgramsinPortal.PortalId == portalId && y.CompIntroKitsOnTime == null).FirstOrDefault();
                    var kitsinPrograms = userinPrograms.ProgramsinPortal.Program.KitsinPrograms.Select(x => x.KitId).ToList();
                    if (user.Labs2 != null && user.Labs2.Where(y => y.PortalId == portalId && y.DateCompleted != null && y.DateCompleted <= dateRange).Count() > 0 && userinPrograms.KitsinUserPrograms.Where(z => z.IsActive).Where(x => kitsinPrograms.Contains(x.KitId) && x.CompleteDate <= dateRange).Count() >= kitsinPrograms.Count)
                    {
                        CompIntroKitsOnTime = true;
                    }
                    using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                    {
                        var usersinPrograms = ctx.UsersinPrograms.Where(y => y.Id == userinPrograms.Id).FirstOrDefault();
                        usersinPrograms.CompIntroKitsOnTime = CompIntroKitsOnTime;
                        ctx.UsersinPrograms.Attach(usersinPrograms);
                        ctx.Entry(usersinPrograms).State = EntityState.Modified;
                        ctx.SaveChanges();
                    }
                }
            }
            return count;
        }

        public GetDashboadMessagesResponse GetDashboadMessages(GetDashboadMessagesRequest request)
        {
            StoredProcedures sp = new StoredProcedures();
            GetDashboadMessagesResponse response = new GetDashboadMessagesResponse();
            var dashboardMessages = sp.GetDashboadMessages(request.userId, request.portalStartDate, request.isBoth, request.messageType, request.newMessage, request.page, request.pageSize);


            if (dashboardMessages != null && dashboardMessages.Count > 0)
            {
                List<UserDashboardMessageDto> newDashboardMessages = new List<UserDashboardMessageDto>();
                if (string.IsNullOrEmpty(request.timeZone))
                    request.timeZone = "Eastern Standard Time";
                TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
                for (int i = 0; i < dashboardMessages.Count; i++)
                {
                    UserDashboardMessageDto newDashboardMessage = new UserDashboardMessageDto();
                    DashboardMessageTypeDto DashboardMessageType = new DashboardMessageTypeDto();
                    if (dashboardMessages[i].Type == "Appointment" && dashboardMessages[i].RelatedId.HasValue)
                    {
                        int appointmentId = dashboardMessages[i].RelatedId.Value;
                        var appointment = context.Appointments.Where(x => x.Id == appointmentId).FirstOrDefault();
                        if (appointment == null)
                            newDashboardMessage.appointment = null;
                        else
                        {
                            newDashboardMessage.appointment = Utility.mapper.Map<DAL.Appointment, AppointmentDTO>(appointment);
                            newDashboardMessage.appointment.UserTimeZone = request.timeZone;
                        }
                    }
                    else
                    {
                        newDashboardMessage.Message = dashboardMessages[i].Message;
                    }
                    newDashboardMessage.LanguageItem = dashboardMessages[i].LanguageItem;
                    newDashboardMessage.Parameters = dashboardMessages[i].Parameters;
                    newDashboardMessage.Id = dashboardMessages[i].Id;
                    newDashboardMessage.Url = dashboardMessages[i].Url;
                    newDashboardMessage.New = dashboardMessages[i].New;
                    DashboardMessageType.Image = dashboardMessages[i].Image;
                    DashboardMessageType.Alt = dashboardMessages[i].Alt;
                    DashboardMessageType.MessageTemplate = dashboardMessages[i].MessageTemplate;
                    DashboardMessageType.Type = dashboardMessages[i].Type;
                    DashboardMessageType.NotificationType = dashboardMessages[i].NotificationType;
                    newDashboardMessage.DashboardMessageType = DashboardMessageType;
                    newDashboardMessage.MessageType = dashboardMessages[i].MessageType;
                    newDashboardMessage.CreatedOn = TimeZoneInfo.ConvertTimeFromUtc(dashboardMessages[i].CreatedOn, timeZone);
                    newDashboardMessage.CreatedOnString = TimeZoneInfo.ConvertTimeFromUtc(dashboardMessages[i].CreatedOn, timeZone).ToString();
                    newDashboardMessages.Add(newDashboardMessage);
                }
                response.hasReadMessages = context.UserDashboardMessages.Where(x => x.UserId == request.userId && x.Active == true && request.portalStartDate != null && x.CreatedOn >= request.portalStartDate && (x.DashboardMessageType.NotificationType == request.messageType || (request.isBoth && x.DashboardMessageType.NotificationType == (int)NotificationTypes.Both)) && x.New == false && x.DashboardMessageType.Type != "Appointment").Count() > 0;
                response.totalRecords = dashboardMessages.Count > 0 ? dashboardMessages.FirstOrDefault().Records.Value : 0;
                response.dashboardMessages = newDashboardMessages;
            }
            return response;
        }

        public UpdateDashboardMessageResponse UpdateDashboardMessage(UpdateDashboardMessageRequest request)
        {
            UpdateDashboardMessageResponse response = new UpdateDashboardMessageResponse();
            var dashboardMessage = context.UserDashboardMessages.Include("DashboardMessageType").Where(x => (!request.id.HasValue || x.Id == request.id)
                && (!request.relatedId.HasValue || (x.RelatedId == request.relatedId && x.MessageType == request.messageType))).FirstOrDefault();
            if (dashboardMessage != null)
            {
                if (request.New.HasValue && dashboardMessage.DashboardMessageType.Type != "Appointment")
                    dashboardMessage.New = request.New.Value;
                if (request.Active.HasValue)
                    dashboardMessage.Active = request.Active.Value;
                if (request.Status.HasValue)
                    dashboardMessage.Status = request.Status.Value;
                context.UserDashboardMessages.Attach(dashboardMessage);
                context.Entry(dashboardMessage).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public GetNotesResponse GetNotes(GetNotesRequest request)
        {
            GetNotesResponse response = new GetNotesResponse();
            var note = context.Notes.Include("User1").Where(x => x.userId == request.Id).OrderByDescending(x => x.Type == 4 && x.Pinned == true).ThenByDescending(x => x.Id).ToList();
            response.participantNotes = Utility.mapper.Map<IList<DAL.Note>, IList<NotesDto>>(note);
            return response;
        }

        public void ProcessBillingNotes(ProcessBillingNotesRequest request)
        {
            var WellnessDataList = context.WellnessDatas.Include("BillingNotes").Where(x => x.User.OrganizationId == request.orgId && x.CollectedBy.HasValue && x.User.IsActive && x.BillingNotes.Count == 0).ToList();
            var UserTimeList = context.UserTimeTracker.Where(x => x.User.OrganizationId == request.orgId && x.EndTime.HasValue).ToList();

            foreach (var data in WellnessDataList)
            {
                var userTime = UserTimeList.Where(x => x.UserId == data.UserId && x.StartTime.Date == data.CollectedOn.Value.Date).ToList();
                if (userTime.Count > 0)
                {
                    int timeSpent = 0;
                    foreach (UserTimeTracker time in userTime)
                    {
                        timeSpent += (time.EndTime - time.StartTime).Value.Hours * 60 + (time.EndTime - time.StartTime).Value.Minutes;
                        time.Billed = true;
                        context.UserTimeTracker.Attach(time);
                        context.Entry(time).State = EntityState.Modified;
                        context.SaveChanges();
                    }
                    context.BillingNotes.Add(new BillingNote
                    {
                        Submitted = false,
                        TimeSpent = timeSpent,
                        WellnessId = data.Id
                    });
                    context.SaveChanges();
                }
                else
                {
                    LogReader logreader = new LogReader();
                    var logEvent = new LogEventInfo(LogLevel.Error, "ProcessBillingNotes", "Missing timer record for wellness data : " + data.Id);
                    logreader.WriteLogMessage(logEvent);
                }
            }
        }

        public GetBillingNotesResponse GetPendingBillingNotesList()
        {
            GetBillingNotesResponse response = new GetBillingNotesResponse();
            var billingNotes = context.BillingNotes.Include("WellnessData").Include("WellnessData.TeamsBP_PPR").Include("WellnessData.User")
                .Include("WellnessData.User.Appointments").Include("WellnessData.User1").Include("WellnessData.User1.UserRoles").Include("WellnessData.User2").Include("WellnessData.User2.UserRoles")
                .Where(x => !x.Submitted).ToList();
            response.billingNotes = Utility.mapper.Map<IList<DAL.BillingNote>, IList<BillingNotesDto>>(billingNotes);
            return response;
        }

        public string GetPreviousWellnessData(int userId, string goal)
        {
            var wellnessDatas = context.WellnessDatas.Include("TeamsBP_PPR").Where(x => x.UserId == userId && x.BillingNotes.FirstOrDefault().Submitted).OrderByDescending(x => x.BillingNotes.FirstOrDefault().SubmittedOn).ToList();
            string oldGoal = null;
            switch (goal)
            {
                case "NewHDGoal":
                    var NewHDGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewHDGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewHDGoal != null ? NewHDGoal.TeamsBP_PPR.FirstOrDefault().NewHDGoal : null;
                    break;
                case "NewAlcoholGoal":
                    var NewAlcoholGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewAlcoholGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewAlcoholGoal != null ? NewAlcoholGoal.TeamsBP_PPR.FirstOrDefault().NewAlcoholGoal : null;
                    break;
                case "NewStressGoal":
                    var NewStressGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewStressGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewStressGoal != null ? NewStressGoal.TeamsBP_PPR.FirstOrDefault().NewStressGoal : null;
                    break;
                case "NewWeightGoal":
                    var NewWeightGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewWeightGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewWeightGoal != null ? NewWeightGoal.TeamsBP_PPR.FirstOrDefault().NewWeightGoal : null;
                    break;
                case "NewBPMonitoringGoal":
                    var NewBPMonitoringGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewBPMonitoringGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewBPMonitoringGoal != null ? NewBPMonitoringGoal.TeamsBP_PPR.FirstOrDefault().NewBPMonitoringGoal : null;
                    break;
                case "OtherGoals":
                    var OtherGoals = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().OtherGoals)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = OtherGoals != null ? OtherGoals.TeamsBP_PPR.FirstOrDefault().OtherGoals : null;
                    break;
                case "NewPAGoal":
                    var NewPAGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewPAGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewPAGoal != null ? NewPAGoal.TeamsBP_PPR.FirstOrDefault().NewPAGoal : null;
                    break;
                case "NewSmokingGoal":
                    var NewSmokingGoal = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewSmokingGoal)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewSmokingGoal != null ? NewSmokingGoal.TeamsBP_PPR.FirstOrDefault().NewSmokingGoal : null;
                    break;
                case "NewBPMedPrescribed":
                    var NewBPMedPrescribed = wellnessDatas.Where(x => !string.IsNullOrEmpty(x.TeamsBP_PPR.FirstOrDefault().NewBPMedPrescribed)).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = NewBPMedPrescribed != null ? NewBPMedPrescribed.TeamsBP_PPR.FirstOrDefault().NewBPMedPrescribed : null;
                    break;
                case "MinGoal":
                    var MinGoal = wellnessDatas.Where(x => x.TeamsBP_PPR.FirstOrDefault().MinGoal.HasValue).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = MinGoal != null ? MinGoal.TeamsBP_PPR.FirstOrDefault().MinGoal.Value.ToString() : null;
                    break;
                case "StepGoal":
                    var StepGoal = wellnessDatas.Where(x => x.TeamsBP_PPR.FirstOrDefault().StepGoal.HasValue).OrderByDescending(y => y.CollectedOn).FirstOrDefault();
                    oldGoal = StepGoal != null ? StepGoal.TeamsBP_PPR.FirstOrDefault().StepGoal.Value.ToString() : null;
                    break;
            }
            return oldGoal;
        }

        public bool HasPreviousWellnessData(int userId)
        {
            return context.WellnessDatas.Where(x => x.UserId == userId && x.BillingNotes.Any(y => y.Submitted)).Count() > 1;
        }

        public bool EditBillingNotes(EditBillingNotesRequest request)
        {
            GetBillingNotesResponse response = new GetBillingNotesResponse();
            var billingNotes = context.BillingNotes.Where(x => x.Id == request.BillingNote.Id).FirstOrDefault();
            if (billingNotes != null)
            {
                billingNotes.JsonRequest = request.BillingNote.JsonRequest;
                billingNotes.Submitted = request.BillingNote.Submitted;
                billingNotes.SubmittedOn = request.BillingNote.SubmittedOn;
                context.BillingNotes.Attach(billingNotes);
                context.Entry(billingNotes).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public GetCoachingCountResponse GetCoachingCount(GetCoachingCountRequest request)
        {
            GetCoachingCountResponse response = new GetCoachingCountResponse();
            var notes = context.Notes.Where(x => x.RefId.HasValue && (x.Type == (int)NoteTypes.Coaching && x.RefId.Value == request.refId) || (x.Type == (int)NoteTypes.BiometricReview && request.hraId.HasValue && x.RefId.Value == request.hraId) && x.userId == request.userId && x.PortalId == request.portalId).OrderByDescending(x => x.Id).ToList();
            response.participantNotes = Utility.mapper.Map<IList<DAL.Note>, IList<NotesDto>>(notes);
            response.count = notes.Count();
            return response;
        }

        public AddNotesResponse AddEditNotes(AddNotesRequest request)
        {
            AddNotesResponse response = new AddNotesResponse();
            DateTime noteDate;
            TimeZoneInfo userTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            if (request.note.NotesDate == DateTime.MinValue)
            {
                noteDate = DateTime.UtcNow;
                noteDate = TimeZoneInfo.ConvertTimeFromUtc(noteDate, userTZone);
            }
            else
            {
                noteDate = request.note.NotesDate;
            }
            DAL.Note noteData = null;
            if (request.note.Id > 0)
                noteData = context.Notes.Where(x => x.Id == request.note.Id).FirstOrDefault();
            if (noteData == null)
            {
                noteData = new DAL.Note();
            }
            if (request.note.Type == (int)NoteTypes.Note)
            {
                var pastPinnedNote = context.Notes.Where(x => x.userId == request.note.userId && x.Pinned == true).FirstOrDefault();
                if (pastPinnedNote == null)
                    noteData.Pinned = true;
            }
            noteData.Admin = request.note.Admin;
            //Don't update date and type for edit
            if (request.note.Id == 0)
            {
                noteData.NotesDate = TimeZoneInfo.ConvertTimeToUtc(noteDate, userTZone);
                noteData.Type = request.note.Type;
                noteData.RefId = request.note.RefId;
                noteData.RefId2 = request.note.RefId2;
            }
            else if (request.note.Type == (int)NoteTypes.OtherReferrals)
            {
                noteData.RefId = request.note.RefId;
            }
            noteData.Text = request.note.Text;
            noteData.userId = request.note.userId;
            noteData.PortalId = request.note.PortalId;
            if (request.note.Id > 0)
            {
                context.Notes.Attach(noteData);
                context.Entry(noteData).State = EntityState.Modified;
            }
            else
            {
                context.Notes.Add(noteData);
                EditContactRequirementAlertActiveStatus(noteData.userId, null);
                if (noteData.Type == (int)NoteTypes.Coaching)
                {
                    ReportReader reportReader = new ReportReader();
                    var list = reportReader.GetBillingServiceTypes();
                    reportReader.AddInvoiceDetails(new InvoiceDetailsRequest { UserId = request.note.userId, CreatedOn = DateTime.UtcNow, Type = list.Where(x => x.Type == CommonReader.BillingServiceTypes.HealthCoaching).Select(x => x.Id).FirstOrDefault() });
                }
            }
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public GetNoteResponse GetNote(GetNoteRequest request)
        {
            GetNoteResponse response = new GetNoteResponse();
            var note = context.Notes.Include("User1").Where(x => x.Id == request.Id).FirstOrDefault();
            response.note = Utility.mapper.Map<DAL.Note, NotesDto>(note);
            return response;
        }

        public ReadUserTrackingStatusResponse ReadUserTrackingStatus(ReadUserTrackingStatusRequest request)
        {
            ReadUserTrackingStatusResponse response = new ReadUserTrackingStatusResponse();
            var userTrackingStatus = context.UserTrackingStatuses.Where(x => x.UserId == request.userId && x.PortalId == request.portalId).FirstOrDefault();
            response.UserTrackingStatus = Utility.mapper.Map<DAL.UserTrackingStatus, UserTrackingStatusDto>(userTrackingStatus);
            return response;
        }

        public ListWellnessDataResponse ListWellnessData(ListWellnessDataRequest request)
        {
            ListWellnessDataResponse response = new ListWellnessDataResponse();
            var WellnessData = context.WellnessDatas.Where(x => x.UserId == request.userId).OrderBy(x => x.CollectedOn).ToList();
            response.WellnessData = Utility.mapper.Map<IList<DAL.WellnessData>, IList<WellnessDataDto>>(WellnessData);
            return response;
        }

        public ListWellnessDataResponse ListWellnessDataByPage(ListWellnessDataRequest request)
        {
            ListWellnessDataResponse response = new ListWellnessDataResponse();
            var WellnessData = context.WellnessDatas.Where(x => x.UserId == request.userId).OrderBy(x => x.CollectedOn).ToList();
            var TableWellnessData = context.WellnessDatas.Where(x => x.UserId == request.userId).OrderByDescending(x => x.CollectedOn).Skip(request.Page * request.PageSize).Take(request.PageSize).ToList();
            response.WellnessData = Utility.mapper.Map<IList<DAL.WellnessData>, IList<WellnessDataDto>>(WellnessData);
            response.TotalRecords = WellnessData.Count();
            response.TableWellnessData = Utility.mapper.Map<IList<DAL.WellnessData>, IList<WellnessDataDto>>(TableWellnessData);
            return response;
        }

        public ListGlucoseResponse ListGlucoseData(ListGlucoseRequest request)
        {
            ListGlucoseResponse response = new ListGlucoseResponse();
            var glucoseDAL = context.EXT_Glucose.Where(x => request.uniqueId != null && x.UniqueId == request.uniqueId && x.OrganizationId == request.organizationId
                                && (!request.onlyValidData || !x.IsValid.HasValue || x.IsValid.Value)).OrderByDescending(x => x.EffectiveDateTime).ToList();
            response.listGlucose = Utility.mapper.Map<IList<DAL.EXT_Glucose>, IList<EXT_GlucoseDto>>(glucoseDAL);
            return response;
        }

        public AddEditWellnessDataResponse AddEditWellnessData(AddEditWellnessDataRequest request)
        {
            AddEditWellnessDataResponse response = new AddEditWellnessDataResponse();

            var wellnessData = context.WellnessDatas.Where(x => x.Id == request.WellnessData.Id).FirstOrDefault();
            if (wellnessData != null)
            {
                var updatedWellnessData = Utility.mapper.Map<WellnessDataDto, DAL.WellnessData>(request.WellnessData);
                if (request.IsTeamsBP)
                {
                    wellnessData.Weight = request.WellnessData.Weight;
                    wellnessData.ExerInt = request.WellnessData.ExerInt;
                    wellnessData.ExerMin = request.WellnessData.ExerMin;
                    wellnessData.SBP = request.WellnessData.SBP;
                    wellnessData.DBP = request.WellnessData.DBP;
                    wellnessData.ManageStress = request.WellnessData.ManageStress;
                    wellnessData.PhysicallyActive = request.WellnessData.PhysicallyActive;
                    wellnessData.Motivation = request.WellnessData.Motivation;
                    wellnessData.UpdatedBy = request.WellnessData.UpdatedBy;
                    wellnessData.UpdatedOn = request.WellnessData.UpdatedOn;
                }
                else if (request.updatedbyUser)
                {
                    wellnessData.Weight = request.WellnessData.Weight;
                    wellnessData.SBP = request.WellnessData.SBP;
                    wellnessData.DBP = request.WellnessData.DBP;
                    wellnessData.WellnessVision = request.WellnessData.WellnessVision;
                    wellnessData.waist = request.WellnessData.waist;
                    wellnessData.UpdatedBy = request.WellnessData.UpdatedBy;
                    wellnessData.UpdatedOn = request.WellnessData.UpdatedOn;
                }
                else
                    context.Entry(wellnessData).CurrentValues.SetValues(updatedWellnessData);
            }
            else
            {
                wellnessData = Utility.mapper.Map<WellnessDataDto, DAL.WellnessData>(request.WellnessData);
                context.WellnessDatas.Add(wellnessData);
            }
            context.SaveChanges();
            response.Id = wellnessData.Id;
            response.wellnessDataId = wellnessData.Id;
            response.success = true;
            return response;
        }

        public AddEditTeamsBP_PPRResponse AddEditTeamsBP_PPR(AddEditTeamsBP_PPRRequest request)
        {
            AddEditTeamsBP_PPRResponse response = new AddEditTeamsBP_PPRResponse();
            var wellnessData = context.TeamsBP_PPR.Include("WellnessData").Where(x => x.Id == request.TeamsBP_PPR.Id).FirstOrDefault();
            if (wellnessData != null)
            {
                AddEditWellnessData(new AddEditWellnessDataRequest { WellnessData = request.TeamsBP_PPR.WellnessData, updatedbyUser = false, IsTeamsBP = true });
                var updatedWellnessData = Utility.mapper.Map<TeamsBP_PPRDto, TeamsBP_PPR>(request.TeamsBP_PPR);
                context.Entry(wellnessData).CurrentValues.SetValues(updatedWellnessData);
            }
            else
            {
                wellnessData = Utility.mapper.Map<TeamsBP_PPRDto, TeamsBP_PPR>(request.TeamsBP_PPR);
                context.TeamsBP_PPR.Add(wellnessData);
            }
            context.SaveChanges();
            response.Id = wellnessData.Id;
            response.wellnessId = wellnessData.WellnessId;
            response.success = true;
            return response;
        }

        public void AddtoWellnessData(AddEditWellnessDataRequest request)
        {
            var wellnessData = context.WellnessDatas.Where(x => (request.HRAId.HasValue && x.SourceHRA == request.HRAId) || (request.FollowUpId.HasValue && x.SourceFollowUp == request.FollowUpId)).FirstOrDefault();
            if (wellnessData != null)
            {

                if (request.WellnessData.Weight.HasValue)
                {
                    wellnessData.Weight = request.WellnessData.Weight;
                    wellnessData.sameDevice = request.WellnessData.sameDevice;
                }
                wellnessData.SBP = request.WellnessData.SBP;
                wellnessData.DBP = request.WellnessData.DBP;
                wellnessData.waist = request.WellnessData.waist;
                wellnessData.UpdatedOn = DateTime.UtcNow;
                wellnessData.UpdatedBy = request.WellnessData.UpdatedBy;
                context.WellnessDatas.Attach(wellnessData);
                context.Entry(wellnessData).State = EntityState.Modified;
            }
            else
            {
                DAL.WellnessData newWellnessData = new DAL.WellnessData();
                newWellnessData.Weight = request.WellnessData.Weight;
                newWellnessData.SBP = request.WellnessData.SBP;
                newWellnessData.DBP = request.WellnessData.DBP;
                newWellnessData.waist = request.WellnessData.waist;
                if (request.HRAId.HasValue)
                    newWellnessData.SourceHRA = request.HRAId;
                if (request.FollowUpId.HasValue)
                {
                    newWellnessData.SourceFollowUp = request.FollowUpId;
                    var previousWellnessData = ListWellnessData(new ListWellnessDataRequest { userId = request.WellnessData.UserId }).WellnessData.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (previousWellnessData != null)
                    {
                        newWellnessData.isPregnant = previousWellnessData.isPregnant;
                        newWellnessData.DueDate = previousWellnessData.DueDate;
                    }
                }
                newWellnessData.UserId = request.WellnessData.UserId;
                newWellnessData.CollectedOn = DateTime.UtcNow;
                context.WellnessDatas.Add(newWellnessData);
            }
            context.SaveChanges();
        }

        public void AddtoHealthData(AddtoHealthDataRequest request)
        {
            var healthData = Utility.mapper.Map<HealthDataDto, DAL.HealthData>(request.HealthData);
            context.HealthDatas.Add(healthData);
            context.SaveChanges();
        }
        public ReadWellnessDataResponse ReadWellnessData(ReadWellnessDataRequest request)
        {
            ReadWellnessDataResponse response = new ReadWellnessDataResponse();
            var wellnessdata = context.WellnessDatas.Where(x => x.UserId == request.participantId).OrderByDescending(x => x.Id).ToList();
            if (request.id.HasValue)
            {
                var wellnessData = wellnessdata.ToList().Where(x => x.Id == request.id).FirstOrDefault();
                response.WellnessData = Utility.mapper.Map<DAL.WellnessData, WellnessDataDto>(wellnessData);
            }
            else
            {
                DAL.WellnessData objModel = new DAL.WellnessData();
                objModel.WellnessVision = (wellnessdata.Select(x => x.WellnessVision).FirstOrDefault());
                response.WellnessData = Utility.mapper.Map<DAL.WellnessData, WellnessDataDto>(objModel);
            }
            return response;
        }

        public ReadTeamsBP_PPRResponse ReadTeamsBP_PPRData(ReadTeamsBP_PPRDataRequest request)
        {
            ReadTeamsBP_PPRResponse response = new ReadTeamsBP_PPRResponse();
            var teamsBP_PPRList = context.TeamsBP_PPR.Include("WellnessData").Include("WellnessData.BillingNotes").Where(x => x.WellnessData.UserId == request.participantId).OrderByDescending(x => x.Id).ToList();
            response.TeamsBP_PPR = Utility.mapper.Map<List<TeamsBP_PPR>, List<TeamsBP_PPRDto>>(teamsBP_PPRList);
            return response;
        }

        public bool DeleteWellnessDataRecord(int ID)
        {
            var WellnessData = context.WellnessDatas.Where(x => x.Id == ID).FirstOrDefault();
            if (WellnessData != null)
            {
                context.WellnessDatas.Remove(WellnessData);
                context.SaveChanges();
            }
            return true;
        }

        public CheckTobaccoUserResponse CheckIfTobaccoUser(CheckTobaccoUserRequest request)
        {
            CheckTobaccoUserResponse response = new CheckTobaccoUserResponse();
            response.smoker = false;
            var user = context.Users.Where(x => x.Id == request.participantId).FirstOrDefault();
            if (request.checkEligibility)
            {
                response.smoker = context.Eligibilities.Where(x => x.UniqueId == user.UniqueId && x.PortalId == request.portalId && x.TobaccoFlag == "Y").Count() > 0;
            }
            if (!response.smoker)
            {
                var hra = context.HRAs.Include("HRA_OtherRiskFactors").Where(y => y.PortalId == request.portalId && y.UserId == request.participantId).OrderByDescending(x => x.Id).FirstOrDefault();
                if (hra != null && hra.HRA_OtherRiskFactors != null)
                {
                    response.smoker = hra.HRA_OtherRiskFactors.ECig == 1 || hra.HRA_OtherRiskFactors.OtherTobacco == 1 || hra.HRA_OtherRiskFactors.SmokeCig == 1;
                }
            }
            return response;
        }

        public string GetGuid(int? eligibilityId)
        {
            var canriskTrack = context.CanriskTrackings.Where(x => x.EligibilityId == eligibilityId).FirstOrDefault();
            if (canriskTrack != null)
            {
                return canriskTrack.Guid;
            }
            return null;
        }

        public bool HasDiabetes(CheckTobaccoUserRequest request)
        {
            bool hasDiabetes = false;
            var wellnessData = context.WellnessDatas.Where(x => x.UserId == request.participantId && x.isDiabetic == true).FirstOrDefault();
            if (wellnessData != null)
                return true;
            var hra = context.HRAs.Include("HRA_Goals").Where(y => y.UserId == request.participantId && y.HRA_Goals != null && y.HRA_Goals.Diabetes).OrderByDescending(x => x.Id).FirstOrDefault();
            if (hra != null)
                return true;
            var UsersinProgram = context.UsersinPrograms.Include("FollowUps").Where(x => x.UserId == request.participantId && x.FollowUps.Any(y => y.FollowUp_Goals.Diabetes.HasValue && y.FollowUp_Goals.Diabetes.Value)).FirstOrDefault();
            if (UsersinProgram != null)
                return true;
            return hasDiabetes;
        }

        public bool IsPostmenopausal(int HRAId, string dob, int? gender)
        {
            bool isPostmenopausal = false;
            CommonReader commonReader = new CommonReader();
            int age = commonReader.GetAge(Convert.ToDateTime(dob));
            var hra = context.HRAs.Include("HRA_MedicalConditions").Where(y => ((y.Id == HRAId) && y.HRA_MedicalConditions.Hysterectomy == 1 && y.HRA_MedicalConditions.OvariesRemoved == 1) || (age >= 45 && gender.HasValue && gender == 2))
                .OrderByDescending(x => x.Id).FirstOrDefault();
            if (hra != null)
                return true;
            return isPostmenopausal;
        }

        public SearchUsersResponse SearchUsers(SearchUsersRequest request)
        {
            SearchUsersResponse response = new SearchUsersResponse();
            List<ListUsers_Result> user = null;
            StoredProcedures sp = new StoredProcedures();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
            if (request.HraStartDate.HasValue)
                request.HraStartDate = TimeZoneInfo.ConvertTimeToUtc(request.HraStartDate.Value, custTZone);
            if (request.HraEndDate.HasValue)
                request.HraEndDate = TimeZoneInfo.ConvertTimeToUtc(request.HraEndDate.Value, custTZone);
            user = sp.ListUsers(request.firstName, request.lastName, request.organizationId, request.Id, request.Email, request.Phone, request.RiskCode, request.HraStartDate, request.HraEndDate, request.userId, request.Page, request.PageSize, false);
            var userlist = user.ToList();
            var result = Utility.mapper.Map<IList<DAL.ListUsers_Result>, IList<ListUsers_ResultsDto>>(userlist);
            response.result = result;
            response.totalRecords = userlist.Any() ? userlist.First().Records.Value : 0;
            return response;
        }

        public void TrackCanriskParticipation(TrackCanriskRequest request)
        {
            var canriskTrack = context.CanriskTrackings.Where(x => x.Guid == request.Guid).FirstOrDefault();
            if (canriskTrack == null)
            {
                var track = new CanriskTracking();
                track.Guid = request.Guid;
                track.pageCompleted = request.pageCompleted;
                track.utm_source = request.utm_source;
                track.utm_medium = request.utm_medium;
                track.utm_campaign = request.utm_campaign;
                track.utm_keywords = request.utm_keywords;
                track.DOB = request.DOB;
                track.Gender = request.Gender;
                track.Reason = request.Reason;
                track.ReasonId = request.ReasonId;
                track.CreatedOn = DateTime.UtcNow;
                if (request.EligibilityId.HasValue)
                {
                    track.EligibilityId = request.EligibilityId;
                }
                context.CanriskTrackings.Add(track);
            }
            else
            {
                canriskTrack.pageCompleted = request.pageCompleted;
                canriskTrack.DOB = request.DOB;
                canriskTrack.Gender = request.Gender;
                if (request.ReasonId != canriskTrack.ReasonId)
                {
                    canriskTrack.Reason = request.Reason;
                    canriskTrack.ReasonId = request.ReasonId;
                }
                context.CanriskTrackings.Attach(canriskTrack);
                context.Entry(canriskTrack).State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public UpdateUserTrackingStatusResponse UpdateUserTrackingStatus(UpdateUserTrackingStatusRequest request)
        {
            UpdateUserTrackingStatusResponse response = new UpdateUserTrackingStatusResponse();
            var trackingstatus = context.UserTrackingStatuses.Where(x => x.UserId == request.UserId && x.PortalId == request.PortalId).FirstOrDefault();
            if (trackingstatus != null)
            {
                if (request.DeclinedEnroll.HasValue)
                {
                    trackingstatus.DeclinedEnrollment = request.DeclinedEnroll;
                    if (!request.DeclinedEnroll.Value)
                        trackingstatus.DeclinedEnrollmentReason = null;
                    else if (request.DeclinedEnrollmentReason.HasValue)
                        trackingstatus.DeclinedEnrollmentReason = request.DeclinedEnrollmentReason;

                }
                if (request.DoNotTrack.HasValue)
                    trackingstatus.DoNotTrack = request.DoNotTrack;
                trackingstatus.UpdatedOn = DateTime.UtcNow;
                context.UserTrackingStatuses.Attach(trackingstatus);
                context.Entry(trackingstatus).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                DAL.UserTrackingStatus newtrackingstatus = new DAL.UserTrackingStatus();
                newtrackingstatus.UserId = request.UserId;
                newtrackingstatus.PortalId = request.PortalId;
                newtrackingstatus.DeclinedEnrollment = request.DeclinedEnroll;
                newtrackingstatus.DoNotTrack = request.DoNotTrack;
                newtrackingstatus.UpdatedOn = DateTime.UtcNow;
                if (request.DeclinedEnrollmentReason.HasValue)
                    newtrackingstatus.DeclinedEnrollmentReason = request.DeclinedEnrollmentReason;
                context.UserTrackingStatuses.Add(newtrackingstatus);
                context.SaveChanges();
            }
            return response;
        }

        public ListEligibilityResponse ListEligibility(ListEligibilityRequest request)
        {
            ListEligibilityResponse response = new ListEligibilityResponse();
            List<ListEligibilityResult> EligibilityResult = null;
            StoredProcedures sp = new StoredProcedures();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                EligibilityResult = sp.ListEligibility(request.userId, request.firstName, request.lastName, request.portalId, request.uniqueId, request.Email, request.Telephone, request.Language, request.EligibilityStartDate, request.EligibilityEndDate, request.ClaimStartDate, request.ClaimEndDate, request.enrollmentStatus, request.ClaimDiagnosisCode, request.canrisk, request.CoachingEnabled, request.Page, request.PageSize, true);
                totalRecords = EligibilityResult.ToList().FirstOrDefault().Records.Value;
            }
            EligibilityResult = sp.ListEligibility(request.userId, request.firstName, request.lastName, request.portalId, request.uniqueId, request.Email, request.Telephone, request.Language, request.EligibilityStartDate, request.EligibilityEndDate, request.ClaimStartDate, request.ClaimEndDate, request.enrollmentStatus, request.ClaimDiagnosisCode, request.canrisk, request.CoachingEnabled, request.Page, request.PageSize, false);
            response.Eligibilities = Utility.mapper.Map<List<DAL.ListEligibilityResult>, List<ListEligibilityResultDto>>(EligibilityResult);
            response.totalRecords = totalRecords;
            return response;
        }

        public ListEligibilityByUserResponse ListEligibilityByUserName(string name)
        {
            ListEligibilityByUserResponse response = new ListEligibilityByUserResponse();
            var eligibilities = context.Eligibilities.Include("Portal").Include("Portal.Organization").Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(name.ToLower()) && x.Portal.Active).ToList();
            response.eligibilityList = (from item in eligibilities
                                        select new
                                        {
                                            Id = item.UniqueId,
                                            EligibilityOrgId = item.Portal.OrganizationId,
                                            Name = item.FirstName + " " + item.LastName + " (Unique Id: " + item.UniqueId + ",Org: " + (item.Portal != null && item.Portal.Organization.Name != null ? item.Portal.Organization.Name.Split(' ').First() : " ") + ")"
                                        }).ToArray();
            return response;
        }

        public ListEligibilityResponse ReadEligibilityInMemory(ListEligibilityRequest request)
        {
            ListEligibilityResponse response = new ListEligibilityResponse();
            var eligibility = context.Eligibilities.Where(x => x.PortalId == request.portalId.Value && (x.UniqueId == request.uniqueId)).ToList();
            response.eligibilityList = eligibility.Select(MapToEligibilityDto).ToList();
            return response;
        }

        public ListEligibilityResponse GetSimilarEligibilities(ListEligibilityRequest request)
        {
            ListEligibilityResponse response = new ListEligibilityResponse();
            var eligibility = context.Eligibilities.Include("Portal").Include("Portal.Organization").Where(x => x.PortalId == request.portalId.Value && ((!request.eBenChild && x.UniqueId.StartsWith(request.uniqueId)) || (request.eBenChild && x.UniqueId.Remove(x.UniqueId.Length - 2) == request.uniqueId && x.UserEnrollmentType == "C"))).ToList();
            response.eligibilityList = eligibility.Select(MapToEligibilityDto).ToList();
            return response;
        }

        public GetProcessedEligibilityResponse GetProcessedEligibilityList(GetProcessedEligibilityRequest request)
        {
            GetProcessedEligibilityResponse response = new GetProcessedEligibilityResponse();
            var pastHour = DateTime.UtcNow.AddHours(-1);
            response.processedUniqueIds = context.Eligibilities.Where(x => x.PortalId == request.portalId && (x.CreateDate > pastHour || x.UpdateDate > pastHour)
            && request.uniqueIds.Contains(x.UniqueId)).Select(x => x.UniqueId).ToList();
            return response;
        }

        public GetEligibilityResponse GetEligibility(GetEligibilityRequest request)
        {
            GetEligibilityResponse response = new GetEligibilityResponse();
            var eligibility = context.Eligibilities.Include("Portal").Include("Portal.Organization")
                .Where(x => (x.Id == request.EligibilityId || (x.UniqueId == request.UniqueId && request.PortalIds.Contains(x.PortalId))) && x.Portal.Active).FirstOrDefault();
            response.Eligibility = MapToEligibilityDto(eligibility);
            return response;
        }

        public GetEligibilityResponse GetSpouseEligibility(GetEligibilityRequest request)
        {
            GetEligibilityResponse response = new GetEligibilityResponse();
            var eligibility = context.Eligibilities.Include("Portal").Include("Portal.Organization")
                .Where(x => x.EmployeeUniqueId == request.UniqueId && request.PortalIds.Contains(x.PortalId) && x.UserEnrollmentType == "S" && x.Portal.Active).FirstOrDefault();
            response.Eligibility = MapToEligibilityDto(eligibility);
            return response;
        }

        public void UpdateUserEligibilitySetting(UpdateUserEligibilitySettingRequest request)
        {
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                var userSetting = ctx.UserEligibilitySettings.FirstOrDefault(x => x.UniqueId == request.UserEligibilitySetting.UniqueId && x.OrganizationId == request.UserEligibilitySetting.OrganizationId);
                if (userSetting == null)
                {
                    //insert
                    DAL.UserEligibilitySetting s = new DAL.UserEligibilitySetting();
                    s.OrganizationId = request.UserEligibilitySetting.OrganizationId;
                    s.UniqueId = request.UserEligibilitySetting.UniqueId;
                    s.Language = request.UserEligibilitySetting.Language;
                    ctx.UserEligibilitySettings.Add(s);
                    ctx.SaveChanges();
                }
                else
                {
                    //update
                    userSetting.Language = request.UserEligibilitySetting.Language;
                    ctx.SaveChanges();
                }
            }
        }

        public GetUserEligibilitySettingResponse GetUserEligibilitySetting(GetUserEligibilitySettingRequest request)
        {
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                var response = new GetUserEligibilitySettingResponse();
                var userSetting = ctx.UserEligibilitySettings.FirstOrDefault(x => x.UniqueId == request.UniqueID && x.OrganizationId == request.OrgID);
                if (userSetting != null)
                {
                    response.UserEligibilitySetting = new UserEligibilitySettingDto();
                    response.UserEligibilitySetting.UniqueId = userSetting.UniqueId;
                    response.UserEligibilitySetting.OrganizationId = userSetting.OrganizationId;
                    response.UserEligibilitySetting.Language = userSetting.Language;
                }
                return response;
            }
        }

        public int TerminateNotSentEligibilityRecords(int portalId)
        {
            StoredProcedures sp = new StoredProcedures();
            return sp.EligibilityUpdateTermDate(portalId);
        }

        public int DeleteDuplicateRecords(int portalId)
        {
            var duplicates = context.Eligibilities.Where(x => x.PortalId == portalId).GroupBy(x => x.UniqueId).Where(e => e.Count() > 1);
            int count = duplicates.Count();
            foreach (var duplicate in duplicates)
            {
                var oldRecord = duplicate.FirstOrDefault();
                context.Entry(oldRecord).State = EntityState.Deleted;
                context.SaveChanges();
            }
            return count;
        }

        public GetInfoforProgramResponse GetInfoforProgram(GetInfoforProgramRequest request)
        {
            GetInfoforProgramResponse response = new GetInfoforProgramResponse();
            var user = context.Users.Include("HRAs").Include("HRAs.HRA_MedicalConditions").Include("HRAs.HRA_OtherRiskFactors").Include("Organization.Portals")
                .Include("Appointments").Where(x => x.Id == request.userId).FirstOrDefault();
            if (user.Organization.Portals.Where(x => x.Active == true).Count() > 0)
            {
                var hra = user.HRAs.Where(x => x.Portal != null && x.Portal.Active == true).OrderByDescending(x => x.Id).FirstOrDefault();
                if (hra != null)
                {
                    response.HRA = Utility.mapper.Map<DAL.HRA, HRADto>(hra);
                }
                //future or past appointment
                var appointment = user.Appointments.Where(x => x.Active == true).OrderByDescending(x => x.Date).FirstOrDefault();
                if (appointment != null)
                {
                    response.appointment = Utility.mapper.Map<DAL.Appointment, AppointmentDTO>(appointment);
                }
            }
            return response;
        }

        public void AddEligibilityImportLog(AddEligibilityImportLogRequest request)
        {
            DAL.EligibilityImportLog log = new DAL.EligibilityImportLog();

            log.Action = request.EligibilityImportLog.Action;
            log.ChangedFields = request.EligibilityImportLog.ChangedFields;
            log.CreatedByUser = request.EligibilityImportLog.CreatedByUser;
            log.EligibilityId = request.EligibilityImportLog.EligibilityId;
            log.ErrorDetails = request.EligibilityImportLog.ErrorDetails;
            log.IsLoadError = request.EligibilityImportLog.IsLoadError ? (byte)1 : (byte)0;
            log.LogDate = DateTime.UtcNow;
            log.PortalId = request.EligibilityImportLog.PortalId;
            log.UniqueId = request.EligibilityImportLog.UniqueId;
            log.FirstName = request.EligibilityImportLog.FirstName;
            log.LastName = request.EligibilityImportLog.LastName;
            context.EligibilityImportLogs.Add(log);
            context.SaveChanges();
        }

        public void DeleteUserForm(DeleteUserFormRequest request)
        {
            var form = context.UserForms.Where(x => x.UserId == request.UserId && x.FormTypeId == request.FormTypeId && x.PortalId == request.PortalId).FirstOrDefault();
            if (form != null)
            {
                context.UserForms.Remove(form);
                context.SaveChanges();
            }
        }

        public AddEditEligibilityResponse AddEditEligibility(AddEditEligibilityRequest request)
        {
            var req = request.Eligibility;
            var eligibilityDbModel = new DAL.Eligibility();
            if (req.Id.HasValue)
            {
                eligibilityDbModel = context.Eligibilities.Where(x => x.Id == req.Id).FirstOrDefault();
            }
            else
            {
                eligibilityDbModel.CreateDate = DateTime.UtcNow;
            }
            var eligibility = MapToEligibilityDAL(req, eligibilityDbModel);
            if (req.Id.HasValue)
            {
                context.Eligibilities.Attach(eligibility);
                context.Entry(eligibility).State = EntityState.Modified;
            }
            else
                context.Eligibilities.Add(eligibility);
            context.SaveChanges();

            AddEditEligibilityResponse response = new AddEditEligibilityResponse();
            response.success = true;
            response.Eligibility = request.Eligibility;
            if (!req.Id.HasValue)
            {
                response.Eligibility.Id = eligibility.Id;
            }
            return response;

        }

        public bool CheckIfExists(EligibilityDto request)
        {
            var eligibilities = context.Eligibilities.Include("CanriskQuestionnaire").Where(x => x.FirstName == request.FirstName && x.LastName == request.LastName && x.DOB == request.DOB).ToList();
            if (eligibilities.Count() > 0)
            {
                var canRisk = eligibilities.Where(x => x.CanriskQuestionnaire != null && x.CanriskQuestionnaire.Count > 0).ToList();
                if (canRisk.Count() > 0)
                {
                    if (canRisk.Where(x => x.CanriskQuestionnaire.FirstOrDefault().CompletedOn.Value.Date > DateTime.UtcNow.Date.AddDays(-7)).FirstOrDefault() != null)
                        return true;
                }
            }
            return false;
        }

        public void SubmitCanriskQuestionnaire(SubmitCanriskQuestionnaireRequest request)
        {
            var canriskDAL = Utility.mapper.Map<CanriskQuestionnaireDto, DAL.CanriskQuestionnaire>(request.canriskAnswers);
            context.CanriskQuestionnaires.Add(canriskDAL);
            context.SaveChanges();
        }

        public bool CheckIfEligible(string postalCode)
        {
            if (context.CanriskPostalCodes.Where(x => (postalCode.Replace(" ", "").ToUpper().Substring(0, 3) == x.PostalCode.ToUpper() || x.PostalCode.ToUpper() == postalCode.Replace(" ", "").ToUpper()) && x.IsActive == true).FirstOrDefault() != null)
                return true;
            return false;
        }

        public void BulkAddEligibilityImportLog(BulkAddEligibilityImportLogRequest request)
        {
            DAL.EligibilityImportLog log = null;
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (EligibilityImportLogDto dto in request.EligibilityImportLogs)
                    {
                        log = new DAL.EligibilityImportLog();
                        log.Action = dto.Action;
                        log.ChangedFields = dto.ChangedFields;
                        log.CreatedByUser = dto.CreatedByUser;
                        log.EligibilityId = dto.EligibilityId;
                        log.ErrorDetails = dto.ErrorDetails;
                        log.IsLoadError = dto.IsLoadError ? (byte)1 : (byte)0;
                        log.LogDate = DateTime.UtcNow;
                        log.PortalId = dto.PortalId;
                        log.UniqueId = String.IsNullOrEmpty(dto.UniqueId) ? "No UniqueId" : dto.UniqueId;
                        log.FirstName = dto.FirstName;
                        log.LastName = dto.LastName;
                        context1.EligibilityImportLogs.Add(log);
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void UpdateEligiblity(UpdateEligibilityDetailsRequest request)
        {

            var eligibility = context.Eligibilities.Include("Portal").Where(x => x.Id == request.EligibilityId).FirstOrDefault();
            if (eligibility != null)
            {
                if (request.isSecEmail)
                {
                    eligibility.Email2 = request.email2;
                }
                else
                {

                    if (eligibility.EnrollmentStatus != request.enrollmentStatus)
                    {
                        eligibility.EnrollmentStatusDate = DateTime.UtcNow;
                    }
                    eligibility.EnrollmentStatus = request.enrollmentStatus;
                    eligibility.DeclinedEnrollmentReason = request.declinedEnrollmentReason;
                    eligibility.IsFalseReferral = request.FalseReferral;
                }
                context.Eligibilities.Attach(eligibility);
                context.Entry(eligibility).State = EntityState.Modified;
                context.SaveChanges();
                if (!request.isSecEmail)
                {
                    UserEligibilitySettingDto settingDto = new UserEligibilitySettingDto
                    {
                        UniqueId = eligibility.UniqueId,
                        OrganizationId = eligibility.Portal.OrganizationId,
                        Language = request.Language
                    };
                    UpdateUserEligibilitySetting(new UpdateUserEligibilitySettingRequest { UserEligibilitySetting = settingDto });
                }
            }
        }

        public BulkAddEditEligibilityResponse BulkAddEditEligibility(BulkAddEditEligibilityRequest request)
        {
            try
            {
                using (var scope = new System.Transactions.TransactionScope())
                {
                    using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (EligibilityDto req in request.Eligibilities.Where(x => x.Id.HasValue))
                        {
                            var eligibilityDbModel = context1.Eligibilities.Where(x => x.Id == req.Id).FirstOrDefault();
                            Eligibility eligibility = new Eligibility();
                            MapToEligibilityDAL(req, eligibility);
                            //Don't update the enrollment status
                            if (eligibilityDbModel.EnrollmentStatus.HasValue)
                                eligibility.EnrollmentStatus = eligibilityDbModel.EnrollmentStatus;
                            if (eligibilityDbModel.DeclinedEnrollmentReason.HasValue)
                                eligibility.DeclinedEnrollmentReason = eligibilityDbModel.DeclinedEnrollmentReason;
                            if (eligibilityDbModel.EnrollmentStatusDate.HasValue)
                                eligibility.EnrollmentStatusDate = eligibilityDbModel.EnrollmentStatusDate;
                            if (!string.IsNullOrEmpty(eligibilityDbModel.Email2))
                                eligibility.Email2 = eligibilityDbModel.Email2;
                            if (eligibility.UserStatus == EligibilityUserStatusDto.Terminated.Key && !eligibility.TerminatedDate.HasValue)
                            {
                                if (!eligibilityDbModel.TerminatedDate.HasValue)
                                    eligibility.TerminatedDate = DateTime.UtcNow.Date;
                                else
                                    eligibility.TerminatedDate = eligibilityDbModel.TerminatedDate;
                            }
                            else if (eligibility.UserStatus != EligibilityUserStatusDto.Terminated.Key)
                                eligibility.TerminatedDate = null;
                            context1.Entry(eligibilityDbModel).CurrentValues.SetValues(eligibility);
                        }
                        context1.SaveChanges();
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "BulkAddEditEligibility Edit", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }

            try
            {
                //insert
                using (var scope = new System.Transactions.TransactionScope())
                {
                    using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (EligibilityDto req in request.Eligibilities.Where(x => x.Id.HasValue == false))
                        {
                            var eligibilityDbModel = new DAL.Eligibility();
                            if (req.UserStatus == EligibilityUserStatusDto.Terminated && !req.TerminatedDate.HasValue)
                                req.TerminatedDate = DateTime.UtcNow.Date;
                            context1.Eligibilities.Add(MapToEligibilityDAL(req, eligibilityDbModel));
                        }
                        context1.SaveChanges();
                    }
                    scope.Complete();
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "BulkAddEditEligibility Add", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }

            BulkAddEditEligibilityResponse response = new BulkAddEditEligibilityResponse();
            response.success = true;
            return response;
        }

        static DAL.Eligibility MapToEligibilityDAL(EligibilityDto source, DAL.Eligibility target)
        {
            try
            {
                if (source == null)
                    return target;
                target.Address = source.Address;
                target.Address2 = source.Address2;
                target.BusinessUnit = source.BusinessUnit;
                target.CellNumber = source.CellNumber;
                target.City = source.City;
                target.Country = source.Country;
                target.DeathDate = source.DeathDate;
                target.DOB = source.DOB;
                if (!string.IsNullOrEmpty(source.Email))
                    target.Email = source.Email.Replace(" ", String.Empty);
                target.EmployeeUniqueId = source.EmployeeUniqueId;
                target.FirstName = source.FirstName;
                target.LastName = source.LastName;
                target.MiddleName = source.MiddleName;
                if (source.Gender != null)//map unknown to null or target value
                    target.Gender = source.Gender.Key;
                target.HireDate = source.HireDate;
                target.HomeNumber = source.HomeNumber;
                if (source.Id.HasValue)
                    target.Id = source.Id.Value;
                target.LastName = source.LastName;
                target.MedicalPlanCode = source.MedicalPlanCode;
                target.MedicalPlanEndDate = source.MedicalPlanEndDate;
                target.MedicalPlanStartDate = source.MedicalPlanStartDate;
                target.PortalId = source.PortalId;
                if (source.PayType != null)
                    target.PayType = source.PayType.Key;
                target.RegionCode = source.RegionCode;
                target.SSN = source.SSN;
                target.State = source.State;
                target.TerminatedDate = source.TerminatedDate;
                if (source.TobaccoFlag.HasValue)
                    target.TobaccoFlag = source.TobaccoFlag.Value ? "Y" : "N";
                else
                    target.TobaccoFlag = null;
                if (source.EducationalAssociates.HasValue)
                    target.EducationalAssociates = source.EducationalAssociates.Value ? "Y" : "N";
                else
                    target.EducationalAssociates = null;
                if (source.UnionFlag.HasValue)
                    target.UnionFlag = source.UnionFlag.Value ? "Y" : "N";
                else
                    target.UnionFlag = null;
                target.UniqueId = source.UniqueId;
                target.UserEnrollmentType = source.UserEnrollmentType.UserEnrollmentTypeKey;
                target.UserStatus = source.UserStatus != null ? source.UserStatus.Key : "";
                target.WorkNumber = source.WorkNumber;
                target.Zip = source.Zip;
                if (source.EnrollmentStatus.HasValue)
                    target.EnrollmentStatus = source.EnrollmentStatus;
                if (source.DeclinedEnrollmentReason.HasValue)
                    target.EnrollmentStatus = source.DeclinedEnrollmentReason;
                target.IsFalseReferral = source.IsFalseReferral;
                target.DentalPlanCode = source.DentalPlanCode;
                target.DentalPlanStartDate = source.DentalPlanStartDate;
                target.DentalPlanEndDate = source.DentalPlanEndDate;
                target.VisionPlanCode = source.VisionPlanCode;
                target.VisionPlanStartDate = source.VisionPlanStartDate;
                target.VisionPlanEndDate = source.VisionPlanEndDate;
                target.UpdateDate = DateTime.UtcNow;
                target.PayrollArea = source.PayrollArea;
                if (!string.IsNullOrEmpty(source.Ref_FirstName))
                    target.Ref_FirstName = source.Ref_FirstName.Length > 50 ? source.Ref_FirstName.Substring(0, 49) : source.Ref_FirstName;
                if (!string.IsNullOrEmpty(source.Ref_LastName))
                    target.Ref_LastName = source.Ref_LastName.Length > 50 ? source.Ref_LastName.Substring(0, 49) : source.Ref_LastName;
                target.Ref_OfficeName = source.Ref_OfficeName;
                target.Ref_City = source.Ref_City;
                target.Ref_StateOrProvince = source.Ref_StateOrProvince;
                target.Ref_PractNum = source.Ref_PractNum;
                target.Ref_Phone = source.Ref_Phone;
                target.Ref_Fax = source.Ref_Fax;
                if (source.DiabetesType != null)
                    target.DiabetesType = source.DiabetesType.UserDiabetesTypeKey;
                target.CoachingEnabled = source.CoachingEnabled;
                target.CoachingExpirationDate = source.CoachingExpirationDate;
                target.Email2 = source.Email2;
                target.Lab_Date = source.Lab_Date;
                target.Lab_A1C = source.Lab_A1C;
                target.Lab_Glucose = source.Lab_Glucose;
                target.Lab_DidYouFast = source.Lab_DidYouFast;
                if (String.IsNullOrEmpty(target.UpdatedBy))
                {
                    target.UpdatedBy = "ELIGIBILITY PROCESS";
                }
                target.CreateDate = source.CreateDate;
                if (source.FirstEligibleDate.HasValue)
                    target.FirstEligibleDate = source.FirstEligibleDate;
                target.IsActive = true;
                target.Ref_Address1 = source.Ref_Address1;
                target.Ref_Address2 = source.Ref_Address2;
                target.Ref_Zip = source.Ref_Zip;
                target.Ref_Country = source.Ref_Country;
                target.Ref_Phone2 = source.Ref_Phone2;
                target.Ref_Email = source.Ref_Email;
                target.Race = source.Race;
                target.TerminationReason = source.TerminationReason;
                return target;
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "MapToEligibilityDAL", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
                return target;
            }
        }

        public static EligibilityDto MapToEligibilityDto(DAL.Eligibility source)
        {
            if (source == null)
                return null;
            EligibilityDto eligibility = new EligibilityDto();
            eligibility.CreateDate = source.CreateDate;
            eligibility.Address = source.Address;
            eligibility.Address2 = source.Address2;
            eligibility.BusinessUnit = source.BusinessUnit;
            eligibility.CellNumber = source.CellNumber;
            eligibility.City = source.City;
            eligibility.Country = source.Country;
            eligibility.DeathDate = source.DeathDate;
            eligibility.DOB = source.DOB;
            eligibility.Email = source.Email;
            eligibility.EmployeeUniqueId = source.EmployeeUniqueId;
            eligibility.FirstName = source.FirstName;
            eligibility.MiddleName = source.MiddleName;
            if (source.Gender.HasValue)
                eligibility.Gender = GenderDto.GetByKey(source.Gender.Value);
            eligibility.HireDate = source.HireDate;
            eligibility.HomeNumber = source.HomeNumber;
            eligibility.Id = source.Id;
            eligibility.LastName = source.LastName;
            eligibility.MedicalPlanCode = source.MedicalPlanCode;
            eligibility.MedicalPlanEndDate = source.MedicalPlanEndDate;
            eligibility.MedicalPlanStartDate = source.MedicalPlanStartDate;
            eligibility.PortalId = source.PortalId;
            eligibility.PayType = EligibilityPayTypeDto.GetByKey(source.PayType);
            eligibility.RegionCode = source.RegionCode;
            eligibility.SSN = source.SSN;
            eligibility.State = source.State;
            eligibility.TerminatedDate = source.TerminatedDate;
            if (!String.IsNullOrEmpty(source.TobaccoFlag))
                eligibility.TobaccoFlag = source.TobaccoFlag == "Y" ? true : false;
            if (!String.IsNullOrEmpty(source.UnionFlag))
                eligibility.UnionFlag = source.UnionFlag == "Y" ? true : false;
            if (!String.IsNullOrEmpty(source.EducationalAssociates))
                eligibility.EducationalAssociates = source.EducationalAssociates == "Y" ? true : false;
            eligibility.PayrollArea = source.PayrollArea;
            eligibility.UniqueId = source.UniqueId;
            eligibility.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.GetByKey(source.UserEnrollmentType);
            eligibility.UserStatus = EligibilityUserStatusDto.GetByKey(source.UserStatus);
            eligibility.WorkNumber = source.WorkNumber;
            eligibility.Zip = source.Zip;
            PortalDto portal = new PortalDto();
            if (source.Portal != null)
            {
                portal.Name = source.Portal.Name;
                portal.OrganizationId = source.Portal.OrganizationId;
                portal.HAPageSeq = source.Portal.HAPageSeq;
                portal.Id = source.Portal.Id;
                portal.EligibilityFormat = source.Portal.EligibilityFormat;
                portal.StartDate = source.Portal.StartDate.ToString();
                portal.EndDate = source.Portal.EndDate.ToString();
                if (source.Portal.CampaignStartDate.HasValue)
                    portal.CampaignStartDate = source.Portal.CampaignStartDate.ToString();
                if (source.Portal.CampaignEndDate.HasValue)
                    portal.CampaignEndDate = source.Portal.CampaignEndDate.ToString();
                eligibility.Portal = portal;
                var organizationDto = new OrganizationDto();
                organizationDto.IntegrationWith = source.Portal.Organization.IntegrationWith;
                organizationDto.Code = source.Portal.Organization.Code;
                organizationDto.Name = source.Portal.Organization.Name;
                eligibility.Portal.Organization = organizationDto;
            }
            eligibility.EnrollmentStatus = source.EnrollmentStatus;
            eligibility.DeclinedEnrollmentReason = source.DeclinedEnrollmentReason;
            eligibility.IsFalseReferral = source.IsFalseReferral;
            eligibility.VisionPlanCode = source.VisionPlanCode;
            eligibility.VisionPlanEndDate = source.VisionPlanEndDate;
            eligibility.VisionPlanStartDate = source.VisionPlanStartDate;
            eligibility.DentalPlanCode = source.DentalPlanCode;
            eligibility.DentalPlanEndDate = source.DentalPlanEndDate;
            eligibility.DentalPlanStartDate = source.DentalPlanStartDate;
            eligibility.Ref_FirstName = source.Ref_FirstName;
            eligibility.Ref_LastName = source.Ref_LastName;
            eligibility.Ref_OfficeName = source.Ref_OfficeName;
            eligibility.Ref_City = source.Ref_City;
            eligibility.Ref_StateOrProvince = source.Ref_StateOrProvince;
            eligibility.Ref_PractNum = source.Ref_PractNum;
            eligibility.Ref_Phone = source.Ref_Phone;
            eligibility.Ref_Phone2 = source.Ref_Phone2;
            eligibility.Ref_Address1 = source.Ref_Address1;
            eligibility.Ref_Address2 = source.Ref_Address2;
            eligibility.Ref_Zip = source.Ref_Zip;
            eligibility.Ref_Country = source.Ref_Country;
            eligibility.Ref_Email = source.Ref_Email;
            eligibility.Race = source.Race;
            eligibility.TerminationReason = source.TerminationReason;
            if (source.DiabetesType.HasValue)
                eligibility.DiabetesType = EligibilityUserDiabetesTypeDto.GetByKey(source.DiabetesType.Value);
            eligibility.CoachingEnabled = source.CoachingEnabled;
            eligibility.CoachingExpirationDate = source.CoachingExpirationDate;
            eligibility.Email2 = source.Email2;
            eligibility.Lab_Date = source.Lab_Date;
            eligibility.Lab_DidYouFast = source.Lab_DidYouFast;
            eligibility.Lab_A1C = source.Lab_A1C;
            eligibility.Lab_Glucose = source.Lab_Glucose;
            eligibility.FirstEligibleDate = source.FirstEligibleDate;
            return eligibility;
        }

        public MedicalPlanEligbilityResponse MedicalPlanEligbility(MedicalPlanEligbilityRequest request)
        {
            MedicalPlanEligbilityResponse response = new MedicalPlanEligbilityResponse();
            bool Eligible = false;
            var user = context.Users.Where(x => x.Id == request.participantId).FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(user.UniqueId))
            {
                var eligibility = context.Eligibilities.Where(x => x.UniqueId == user.UniqueId && x.PortalId == request.portalId &&
                    !string.IsNullOrEmpty(x.MedicalPlanCode)).FirstOrDefault();
                if (eligibility != null)
                {
                    if (request.checkValidCode)
                    {
                        if (request.isProgram)
                        {
                            if (eligibility.MedicalPlanCode.ToUpper().StartsWith("H"))
                            {
                                response.IsEligbilble = Eligible;
                                return response;
                            }
                            CommonReader reader = new CommonReader();
                            ReadMedicalPlanRequest medicalplanrequest = new ReadMedicalPlanRequest();
                            medicalplanrequest.code = eligibility.MedicalPlanCode;
                            var medicalPlan = reader.ReadMedicalPlan(medicalplanrequest);
                            if (medicalPlan == null || medicalPlan.medicalPlanCode == null || medicalPlan.medicalPlanCode.IVEligible)
                            {
                                Eligible = true;
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrEmpty(eligibility.MedicalPlanCode))
                            {
                                response.IsEligbilble = Eligible;
                                return response;
                            }
                        }
                    }
                    else
                        Eligible = true;
                }
            }
            response.IsEligbilble = Eligible;
            return response;
        }

        public string GetLocation(string regionCode)
        {
            string locationName = "";
            var location = context.Locations.Where(x => x.Code == regionCode).FirstOrDefault();
            if (location != null)
                locationName = location.Name;
            return locationName;
        }

        public GetEligibilityforUserResponse GetELigibilityforUser(GetEligibilityforUserRequest request)
        {
            GetEligibilityforUserResponse response = new GetEligibilityforUserResponse();
            var user = context.Users.Where(x => x.Id == request.participantId).FirstOrDefault();
            if (user != null && !string.IsNullOrEmpty(user.UniqueId))
            {
                var eligibility = context.Eligibilities.Where(x => x.UniqueId == user.UniqueId && x.PortalId == request.portalId).FirstOrDefault();
                if (eligibility != null)
                {
                    response.eligibility = MapToEligibilityDto(eligibility);
                }
            }
            return response;
        }

        public bool AddEditEligibilityNotes(EligibilityNotesDto eligibilityNotes)
        {
            EligibilityNote eligibilityNotesDAL;
            if (eligibilityNotes.Id > 0)
            {

                eligibilityNotesDAL = context.EligibilityNotes.Where(x => x.Id == eligibilityNotes.Id).FirstOrDefault();
                eligibilityNotesDAL.NoteType = eligibilityNotes.NoteType;
                eligibilityNotesDAL.ModuleType = eligibilityNotes.ModuleType;
                eligibilityNotesDAL.Notes = eligibilityNotes.Notes;
                eligibilityNotesDAL.UpdatedBy = eligibilityNotes.UpdatedBy;
                eligibilityNotesDAL.UpdatedOn = eligibilityNotes.UpdatedOn;
                context.EligibilityNotes.Attach(eligibilityNotesDAL);
                context.Entry(eligibilityNotesDAL).State = EntityState.Modified;
            }
            else
            {
                eligibilityNotesDAL = Utility.mapper.Map<EligibilityNotesDto, EligibilityNote>(eligibilityNotes);
                context.EligibilityNotes.Add(eligibilityNotesDAL);
            }
            context.SaveChanges();
            return true;
        }

        public bool RemoveEligibilityNote(EligibilityNotesDto eligibilityNotes)
        {
            EligibilityNote eligibilityNotesDAL = context.EligibilityNotes.Where(x => x.Id == eligibilityNotes.Id).FirstOrDefault();
            eligibilityNotesDAL.Active = false;
            eligibilityNotesDAL.UpdatedBy = eligibilityNotes.UpdatedBy;
            eligibilityNotesDAL.UpdatedOn = eligibilityNotes.UpdatedOn;
            context.SaveChanges();
            return true;
        }

        public GetEligibilityNotesResponse GetEligibilityNotes(GetEligibilityNotesRequest request)
        {
            GetEligibilityNotesResponse response = new GetEligibilityNotesResponse();
            var eligibilitynote = context.EligibilityNotes.Include("User").Where(x => x.UniqueId == request.UniqueId && x.Active == true).OrderByDescending(n => n.CreatedOn).ThenByDescending(n => n.Id).ToList();
            response.participantEligibilityNotes = Utility.mapper.Map<IList<DAL.EligibilityNote>, IList<EligibilityNotesDto>>(eligibilitynote);
            return response;
        }

        public GetEligibilityNoteResponse ReadEligibilityNote(GetNotesRequest request)
        {
            GetEligibilityNoteResponse response = new GetEligibilityNoteResponse();
            var eligibilitynote = context.EligibilityNotes.Where(x => x.Id == request.Id && x.Active == true).FirstOrDefault();
            if (eligibilitynote != null)
            {
                response.EligibilityNote = Utility.mapper.Map<DAL.EligibilityNote, EligibilityNotesDto>(eligibilitynote);
            }
            return response;
        }

        public ReadAWVReportResponse ListAWVReports(ReadAWVReportRequest request)
        {
            ReadAWVReportResponse response = new ReadAWVReportResponse();
            var awv = context.AWVs.Where(x => x.UserId == request.UserId).ToList();
            if (awv.Count > 0)
            {
                response.AWV = Utility.mapper.Map<IList<DAL.AWV>, IList<AWVDto>>(awv);
            }
            return response;
        }

        public AddEditSurveyResponse AddEditSurvey(AddEditSurveyRequest request)
        {
            AddEditSurveyResponse response = new AddEditSurveyResponse();
            if (request.surveyList != null && request.surveyList.Count > 0)
            {
                for (int i = 0; i <= request.surveyList.Count - 1; i++)
                {
                    var survey = request.surveyList[i];
                    var SurveyDAL = context.SurveyResponses.Where(x => x.QuestionId == survey.QuestionId && x.UsersinProgramsId == survey.UsersinProgramsId).FirstOrDefault();
                    if (SurveyDAL != null)
                    {
                        survey.DateCreated = DateTime.UtcNow;
                        survey.Id = SurveyDAL.Id;
                        var UpdatedSurveyDAL = Utility.mapper.Map<SurveyResponseDto, SurveyResponse>(survey);
                        context.Entry(SurveyDAL).CurrentValues.SetValues(UpdatedSurveyDAL);
                    }
                    else
                    {
                        survey.DateCreated = DateTime.UtcNow;
                        SurveyDAL = Utility.mapper.Map<SurveyResponseDto, SurveyResponse>(survey);
                        context.SurveyResponses.Add(SurveyDAL);
                    }
                    context.SaveChanges();
                }
                if (request.surveyList != null && request.surveyList.Count > 0 && request.isEligibleForIncentive)
                {
                    IncentiveReader reader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.Participant_Survey;
                    incentivesRequest.userId = request.userId;
                    incentivesRequest.portalId = request.portalId;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Satisfaction_Survey;
                    reader.AwardIncentives(incentivesRequest);
                }
                if (request.surveyList != null && request.surveyList.Count > 0 && request.comments != null && request.comments.Length > 0)
                    UsersInProgramCommentsSave(request.surveyList[0].UsersinProgramsId, request.comments);
                response.success = true;
            }
            return response;
        }

        public void UsersInProgramCommentsSave(int Id, string Comments)
        {
            var usersinPrograms = context.UsersinPrograms.Where(x => x.Id == Id).FirstOrDefault();
            if (usersinPrograms != null)
            {
                usersinPrograms.UserComments = Comments;
                context.UsersinPrograms.Attach(usersinPrograms);
                context.Entry(usersinPrograms).State = EntityState.Modified;
                context.SaveChanges();
            }
        }

        public AddTestimonialResponse AddTestimonial(AddTestimonialRequest request)
        {
            AddTestimonialResponse response = new AddTestimonialResponse();
            DAL.Testimonial testimonial = new DAL.Testimonial();
            testimonial.UserId = request.userid;
            testimonial.PortalId = request.portalid;
            testimonial.Feedback = request.feedback;
            testimonial.SignedName = request.SignedName;
            testimonial.Date = request.Date;
            context.Testimonials.Add(testimonial);
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public bool AcceptTerms(int userid)
        {
            var user = context.Users.Where(x => x.Id == userid).FirstOrDefault();
            if (user != null)
            {
                user.TermsAccepted = true;
                context.Users.Attach(user);
                context.Entry(user).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public SaveUserFormResponse SaveUserForm(SaveUserFormRequest request)
        {
            SaveUserFormResponse response = new SaveUserFormResponse();
            var form = context.UserForms.Where(x => x.UserId == request.userId && x.PortalId == request.portalId && x.FormTypeId == request.formTypeId).FirstOrDefault();
            if (form != null)
            {
                form.Form = request.reference;
                context.UserForms.Attach(form);
                context.Entry(form).State = EntityState.Modified;
            }
            else
            {
                DAL.UserForm userForm = new DAL.UserForm();
                userForm.UserId = request.userId;
                userForm.PortalId = request.portalId;
                userForm.Form = request.reference;
                userForm.FormTypeId = request.formTypeId;
                context.UserForms.Add(userForm);
            }
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public GetUserFormsResponse GetUserForms(GetUserFormsRequest request)
        {
            GetUserFormsResponse response = new GetUserFormsResponse();
            var userform = context.UserForms.Where(x => x.PortalId == request.portalId && x.UserId == request.userId).ToList();
            response.userForms = Utility.mapper.Map<List<DAL.UserForm>, List<UserFormDto>>(userform);
            return response;
        }

        public ApproveUserFormResponse ApproveForm(ApproveUserFormRequest request)
        {
            ApproveUserFormResponse response = new ApproveUserFormResponse();
            var form = context.UserForms.Where(x => x.Id == request.Id).FirstOrDefault();
            if (form != null)
            {
                form.Approved = true;
                form.ApprovedBy = request.AdminId;
                form.ApprovedOn = DateTime.UtcNow;
                context.UserForms.Attach(form);
                context.Entry(form).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        #region Subsequent year

        public GetPrevYearStatusResponse GetPrevYearStatus(GetPrevYearStatusRequest request)
        {
            var user = context.Users.Include("Organization").Include("Organization.Portals").Where(x => x.Id == request.userId).FirstOrDefault();
            var portalId = 0;
            GetPrevYearStatusResponse response = new GetPrevYearStatusResponse();
            response.prevPortal = false;
            if (user.Organization != null && user.Organization.Portals.Count > 1)
            {
                var portal = user.Organization.Portals.OrderByDescending(x => x.Id).Skip(1).Take(1).FirstOrDefault();
                if (portal.Active)
                    return response;
                portalId = portal.Id;
                var hra = context.HRAs.Include("User").Include("HRA_ExamsandShots").Include("HRA_Interests").Include("HRA_HSP").Include("HRA_MedicalConditions").Include("HRA_HealthNumbers")
                    .Include("HRA_OtherRiskFactors").Include("HRA_Goals").Where(h => h.PortalId == portalId && h.UserId == request.userId).FirstOrDefault();
                response.hra = Utility.mapper.Map<DAL.HRA, HRADto>(hra);

                if (response.hra != null)
                {
                    HRAReader hraReader = new HRAReader();
                    if (response.hra.HealthNumbers != null && hraReader.hasCompletedHealthNumbers(response.hra.HealthNumbers))
                        response.prevCompBiometrics = true;
                }
                if (request.getCoachingInfo == true)
                {
                    var usersinProgram = context.UsersinPrograms.Include("ProgramsinPortal").Include("ProgramsinPortal.Program").Include("ProgramsinPortal.ApptCallTemplate").Include("User.Notes").Include("FollowUps")
                        .Include("KitsinUserPrograms").Include("KitsinUserPrograms.Kit").Where(x => x.UserId == request.userId && x.ProgramsinPortal.PortalId == portalId).OrderByDescending(x => x.Id).FirstOrDefault();
                    if (usersinProgram != null && usersinProgram.ProgramsinPortal != null && usersinProgram.ProgramsinPortal.Program.ProgramType == 2)
                    {
                        if (usersinProgram.FollowUps.Count() > 0 && usersinProgram.FollowUps.FirstOrDefault().CompleteDate.HasValue && usersinProgram.User.Notes.Where(x => x.RefId == usersinProgram.Id).Count() >= 0)
                        {
                            response.prevCompCoaching = true;
                            if (request.getFollowupDetails)
                            {
                                ReadFollowupReportResponse followUp = new ReadFollowupReportResponse();

                                followUp.PreviousYearScheduledCoachingSession = usersinProgram.ProgramsinPortal.ApptCallTemplate.NoOfCalls;
                                followUp.PreviousYearCompletedCoachingSession = usersinProgram.User.Notes.Where(x => x.RefId == usersinProgram.Id).Count();

                                followUp.PreviousYearKits = new List<AssignedKit>();
                                if (usersinProgram.KitsinUserPrograms != null)
                                {
                                    var kits = (usersinProgram.KitsinUserPrograms.Where(c => c.IsActive == true)).ToList();
                                    if (kits != null && kits.Count > 0)
                                    {
                                        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(request.timeZone);
                                        for (int i = 0; i < kits.Count; i++)
                                        {
                                            if (kits[i].Kit != null)
                                            {
                                                AssignedKit readfollowup = new AssignedKit();
                                                readfollowup.EducationalTopic = kits[i].Kit.Name;
                                                readfollowup.DateAssigned = TimeZoneInfo.ConvertTimeFromUtc(kits[i].StartDate, timeZone);
                                                readfollowup.PercentCompleted = kits[i].PercentCompleted;
                                                followUp.PreviousYearKits.Add(readfollowup);
                                            }
                                        }
                                    }
                                }
                                response.followupResponse = followUp;
                            }
                        }
                        else if (usersinProgram.User.Notes.Where(x => x.RefId == usersinProgram.Id).Count() >= 1)
                            response.prevCompFirstCoaching = true;
                    }
                }
                response.prevPortal = true;
            }
            return response;
        }

        #endregion

        public void AddGlucometerData(List<GlucometerUserDto> glucometerUserDtos)
        {
            foreach (GlucometerUserDto glucometerUserDto in glucometerUserDtos)
            {
                var glucometerdata = context.GlucometerUsers.Where(x => x.UniqueId.Equals(glucometerUserDto.UniqueId)).FirstOrDefault();
                if (glucometerdata == null)
                {
                    DAL.GlucometerUser glucometerUser = new DAL.GlucometerUser();
                    glucometerUser.UniqueId = glucometerUserDto.UniqueId;
                    glucometerUser.RegisteredDate = glucometerUserDto.RegisteredDate;
                    glucometerUser.ActivationDate = glucometerUserDto.ActivationDate;
                    glucometerUser.OrganizationId = glucometerUserDto.OrganizationId;
                    context.GlucometerUsers.Add(glucometerUser);
                }
                else
                {
                    glucometerdata.UniqueId = glucometerUserDto.UniqueId;
                    glucometerdata.RegisteredDate = glucometerUserDto.RegisteredDate;
                    glucometerdata.ActivationDate = glucometerUserDto.ActivationDate;
                    glucometerdata.OrganizationId = glucometerUserDto.OrganizationId;
                    context.GlucometerUsers.Attach(glucometerdata);
                    context.Entry(glucometerdata).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
        }

        public CanriskQuestionnaireDto GetCanriskResponse(GetCanriskRequest request)
        {
            var response = context.CanriskQuestionnaires.Include("Eligibility").Where(x => (String.IsNullOrEmpty(request.uniqueId) || x.Eligibility.UniqueId == request.uniqueId)
                            && (!request.eligibilityId.HasValue || x.EligibilityId == request.eligibilityId.Value)).FirstOrDefault();
            var canriskDto = Utility.mapper.Map<CanriskQuestionnaire, CanriskQuestionnaireDto>(response);
            return canriskDto;
        }

        public List<UserDto> GetCanriskParticipants()
        {
            var eligibilityData = context.Eligibilities.Include("CanriskQuestionnaire")
                .Where(x => x.CanriskQuestionnaire.Where(y => y.SenttoVendor != true)
                .Count() > 0).ToList().Select(x => x.UniqueId).ToList();
            var eligibleUsers = context.Users.Include("Labs2").Include("State1").Include("UserDoctorInfoes").Include("UserDoctorInfoes.State1")
                .Include("UserDoctorInfoes.Country1").Include("UserDoctorInfoes.Provider").Include("UserDoctorInfoes.Provider.Country1").Include("UserDoctorInfoes.Provider.State1").Where(x => x.Labs2.Count() > 0 && x.Labs2.FirstOrDefault().DateCompleted.HasValue && eligibilityData.Contains(x.UniqueId)).ToList();
            var response = Utility.mapper.Map<List<User>, List<UserDto>>(eligibleUsers);
            return response;
        }

        public void LogSentRecords(List<string> unqiueIds)
        {
            var questionnaires = context.CanriskQuestionnaires.Where(x => unqiueIds.Contains(x.Eligibility.UniqueId)).ToList();
            foreach (var questionnaire in questionnaires)
            {
                questionnaire.SenttoVendor = true;
                context.CanriskQuestionnaires.Attach(questionnaire);
                context.Entry(questionnaire).State = EntityState.Modified;
            }
            context.SaveChanges();
        }

        public void UpdateEligiblity(UpdateEligibilityRequest request)
        {
            var eligibility = context.Eligibilities.Where(x => x.PortalId == request.PortalId && x.UniqueId == request.ExistingUniqueId).FirstOrDefault();
            if (eligibility != null)
            {
                eligibility.FirstName = request.FirstName;
                eligibility.LastName = request.LastName;
                eligibility.DOB = request.DOB;
                context.SaveChanges();
            }
        }

        public UserDto GetUsersforLabs(int userId)
        {
            var user = context.Users.Include("Labs2").Where(x => (x.Id == userId)).FirstOrDefault();
            var usersDto = Utility.mapper.Map<DAL.User, UserDto>(user);
            return usersDto;
        }

        public AdvancedSearchUsersResponse AdvanceSearchUsers(AdvancedSearchRequest request)
        {
            AdvancedSearchUsersResponse response = new AdvancedSearchUsersResponse();
            StoredProcedures sp = new StoredProcedures();
            List<ListSearchUsers_Result> user = sp.ListAdvancedSearchUsers(request.SearchText, request.OrganizationId, request.HraStartDate, request.HraEndDate, request.CoachId, request.MedicalCondition, request.ProgramType, request.RecentStats, request.ContactRequirement, request.UserId, request.Page, request.PageSize);
            var userlist = user.ToList();
            var result = Utility.mapper.Map<IList<ListSearchUsers_Result>, IList<ListSearchUsers_ResultsDto>>(userlist);
            response.Result = result;
            response.TotalRecords = userlist.Any() ? userlist.First().Records.Value : 0;
            return response;
        }

        public List<int> GetInadequateTestingUsers(int days)
        {
            var date = DateTime.UtcNow.AddDays(-days);
            var activeUsers = context.EXT_Glucose.Where(x => x.EffectiveDateTime > date).Select(x => x.UserId.Value).Distinct().ToList();
            var usersList = context.Users.Include("IntuityUsers").Include("Organization").Include("UserTrackingStatuses").Where(x => x.IsActive == true && x.IntuityUsers.IsEligible && x.Organization.Active && x.Organization.Portals.Any(o => o.Active)
                && (activeUsers.Count() == 0 || !activeUsers.Contains(x.Id))).Select(x => x.Id).ToList();
            return usersList;
        }

        public List<ContactRequirementsAlertDto> GetAllContactRequirementAlerts()
        {
            var alerts = context.ContactRequirementsAlert.OrderBy(x => x.Level).ToList();
            var alertsDto = Utility.mapper.Map<List<DAL.ContactRequirementsAlert>, List<ContactRequirementsAlertDto>>(alerts);
            return alertsDto;
        }

        public void CheckforGlucoseAlert(EXT_Glucose request)
        {
            List<ContactRequirementsAlertDto> alerts = GetAllContactRequirementAlerts().OrderByDescending(x => x.Code).ThenBy(x => x.Min).ToList();
            int value = request.Unit.Equals("mmol/L") ? request.Value * 18 : request.Value;
            ContactRequirementsAlertDto alertType = alerts.Where(x => ((x.Min == null && value <= x.Max) || (value >= x.Min && x.Max == null) || (value >= x.Min && value <= x.Max)) && (x.Code != null && x.Code.Contains(request.Code) || (x.Code == null))).FirstOrDefault();
            if (alertType != null)
            {
                if (!request.UserId.HasValue)
                {
                    AccountReader reader = new AccountReader();
                    request.UserId = reader.GetUserByUniqueId(new GetUserRequestByUniqueId { OrganizationId = request.OrganizationId.Value, UniqueId = request.UniqueId }).User.Id;
                }
                ContactRequirement newAlert = new ContactRequirement
                {
                    IsActive = true,
                    AlertId = alertType.Id,
                    UserId = request.UserId.Value,
                    RefId = request.Id,
                    Type = 1,//1 Glucose, 2 Blood Pressure
                    CreatedOn = DateTime.UtcNow,
                };
                if (CheckFrequencyAndInsertAlert(newAlert, alertType))
                {
                    int alertId = alerts.Where(x => x.Min == null && x.Max == null).Select(x => x.Id).FirstOrDefault();
                    EditContactRequirementAlertActiveStatus(request.UserId.Value, alertId);
                }
            }
        }

        public bool CheckFrequencyAndInsertAlert(ContactRequirement newAlert, ContactRequirementsAlertDto request)
        {
            bool isAlertAdded = false;
            if (request.Frequency.HasValue)
            {
                DateTime dateRange = DateTime.UtcNow.AddDays(-30);
                var result = context.ContactRequirements.Where(x => x.UserId == newAlert.UserId && x.AlertId == request.Id && x.CreatedOn > dateRange).OrderByDescending(x => x.CreatedOn).FirstOrDefault();
                if (result != null)
                    dateRange = result.CreatedOn;
                ExternalReader reader = new ExternalReader();
                int previousAlertCount = reader.GetGlucoseAlertCount(newAlert.UserId, request, dateRange);
                if (previousAlertCount >= request.Frequency)
                {
                    isAlertAdded = AddContactRequirementAlert(newAlert);
                }
            }
            else
                isAlertAdded = AddContactRequirementAlert(newAlert);
            return isAlertAdded;
        }

        public bool AddContactRequirementAlert(ContactRequirement request)
        {
            if (!context.ContactRequirements.Any(x => x.UserId == request.UserId && x.AlertId == request.AlertId && x.CreatedOn.Date == request.CreatedOn.Date))
            {
                context.ContactRequirements.Add(request);
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public bool EditContactRequirementAlertActiveStatus(int userId, int? alertType)
        {
            var details = context.ContactRequirements.Where(x => x.UserId == userId && x.IsActive == true && ((alertType.HasValue && alertType == x.AlertId) || !alertType.HasValue)).ToList();
            if (details.Count > 0)
            {
                foreach (var alert in details)
                {
                    alert.UpdatedOn = DateTime.UtcNow;
                    alert.IsActive = false;
                    context.ContactRequirements.Attach(alert);
                    context.Entry(alert).State = EntityState.Modified;
                }
                context.SaveChanges();
            }
            return true;
        }

        public List<Livongo_ResultDto> LivongoWeeklyHRAProcess()
        {
            StoredProcedures sp = new StoredProcedures();
            var livongoHRAs = sp.LivongoWeeklyHRAProcess();
            return Utility.mapper.Map<IList<DAL.Livongo_Result>, IList<Livongo_ResultDto>>(livongoHRAs).ToList();
        }

        public EligibilityDto MapCsvModelToDto(EligibilityCsvModel source)
        {
            CommonReader commonReader = new CommonReader();
            if (source == null)
                return null;
            EligibilityDto newParticipant = new EligibilityDto();
            newParticipant.Address = source.Address1;
            newParticipant.Address2 = source.Address2;
            newParticipant.BusinessUnit = source.BusinessUnit;
            newParticipant.CellNumber = source.CellNumber;
            newParticipant.City = source.City;
            newParticipant.Country = source.Country;
            if (source.DeathDate.HasValue)
                newParticipant.DeathDate = source.DeathDate.Value.Date;
            if (source.DOB.HasValue)
                newParticipant.DOB = source.DOB.Value.Date;
            if (!string.IsNullOrEmpty(source.EmailAddress))
                newParticipant.Email = source.EmailAddress.Trim();
            newParticipant.EmployeeUniqueId = source.EmployeeUniqueId;
            newParticipant.FirstName = source.FirstName;
            newParticipant.MiddleName = source.MiddleName;
            newParticipant.Gender = source.Gender;
            if (source.HireDate.HasValue)
                newParticipant.HireDate = source.HireDate.Value.Date;
            newParticipant.HomeNumber = source.HomePhone;
            if (!string.IsNullOrEmpty(source.Phone))
                newParticipant.HomeNumber = source.Phone;
            newParticipant.LastName = source.LastName;
            newParticipant.MedicalPlanCode = source.MedicalPlanCode;
            if (source.MedicalPlanEndDate.HasValue)
                newParticipant.MedicalPlanEndDate = source.MedicalPlanEndDate.Value.Date;
            if (source.MedicalPlanStartDate.HasValue)
                newParticipant.MedicalPlanStartDate = source.MedicalPlanStartDate.Value.Date;
            newParticipant.PayType = source.PayType;
            newParticipant.RegionCode = source.RegionCode;
            newParticipant.SSN = source.SSN;
            newParticipant.State = source.StateOrProvince;
            if (source.TerminatedDate.HasValue)
                newParticipant.TerminatedDate = source.TerminatedDate.Value.Date;
            if (source.TobaccoFlag != null)
            {
                newParticipant.TobaccoFlag = source.TobaccoFlag == YesNoDto.Yes ? true : false;
            }
            if (source.UnionFlag != null)
            {
                newParticipant.UnionFlag = source.UnionFlag == YesNoDto.Yes ? true : false;
            }
            if (source.EducationalAssociates != null)
            {
                newParticipant.EducationalAssociates = source.EducationalAssociates == YesNoDto.Yes ? true : false;
            }
            newParticipant.UserStatus = source.UserStatus;
            newParticipant.UniqueId = source.UniqueId;
            newParticipant.UserEnrollmentType = source.UserEnrollmentType;
            newParticipant.Zip = source.ZipOrPostalCode;
            newParticipant.DentalPlanCode = source.DentalPlanCode;
            newParticipant.DentalPlanStartDate = source.DentalPlanStartDate;
            newParticipant.DentalPlanEndDate = source.DentalPlanEndDate;
            newParticipant.VisionPlanCode = source.VisionPlanCode;
            newParticipant.VisionPlanStartDate = source.VisionPlanStartDate;
            newParticipant.VisionPlanEndDate = source.VisionPlanEndDate;
            newParticipant.PayrollArea = source.PayrollArea;
            newParticipant.Ref_FirstName = source.Ref_FirstName;
            newParticipant.Ref_LastName = source.Ref_LastName;
            newParticipant.Ref_OfficeName = source.Ref_OfficeName;
            newParticipant.Ref_Phone = source.Ref_OfficePhone;
            newParticipant.Ref_PractNum = source.Ref_PractNum;
            newParticipant.Ref_City = source.Ref_City;
            newParticipant.Ref_StateOrProvince = source.Ref_Province;
            newParticipant.Ref_Fax = source.Ref_FaxPhone;
            newParticipant.Ref_Address1 = source.Ref_Address1;
            newParticipant.Ref_Address2 = source.Ref_Address2;
            newParticipant.Ref_Zip = source.Ref_Zip;
            newParticipant.Ref_Country = source.Ref_Country;
            newParticipant.Ref_Phone2 = source.Ref_Phone2;
            newParticipant.Ref_Email = source.Ref_Email;
            newParticipant.Race = source.Race;
            newParticipant.TerminationReason = source.TerminationReason;
            newParticipant.Lab_Date = source.Lab_Date;
            if (source.Lab_Fasting != null)
                newParticipant.Lab_DidYouFast = source.Lab_Fasting == DidYouFastDto.Fasting ? (byte)1 : (byte)2;
            newParticipant.Lab_A1C = (float?)source.Lab_A1C;
            if (source.Lab_FBS.HasValue)
            {
                LabDto labModel = ListOptions.CovertIntoImperial(new LabDto { Glucose = (float?)source.Lab_FBS }, commonReader.MeasurementRange());
                newParticipant.Lab_Glucose = labModel.Glucose;
            }
            newParticipant.DiabetesType = source.DiabetesType;
            if (source.CoachingEnabled != null)
            {
                newParticipant.CoachingEnabled = source.CoachingEnabled == TrueFalseDto.True ? true : false;
                if (source.CoachingEnabled == TrueFalseDto.True)
                    newParticipant.FirstEligibleDate = DateTime.UtcNow;
            }
            if (source.CoachingExpirationDate.HasValue)
                newParticipant.CoachingExpirationDate = source.CoachingExpirationDate;
            newParticipant.Email2 = source.PatternsEmailID;
            if (!string.IsNullOrEmpty(source.PatternsPhoneNumber))
                newParticipant.CellNumber = source.PatternsPhoneNumber;
            return newParticipant;
        }

        public bool AddEditTrackingTime(UpdateTimeTrackingRequest request)
        {
            if (request.ForceEnd)
            {
                List<UserTimeTracker> userList = context.UserTimeTracker.Where(x => x.CoachId == request.CoachId && !x.EndTime.HasValue).ToList();
                foreach (UserTimeTracker timer in userList)
                {
                    timer.EndTime = DateTime.UtcNow;
                    timer.DispositionType = request.Disposition;
                    timer.Billed = false;
                    context.UserTimeTracker.Attach(timer);
                    context.Entry(timer).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            else
            {
                UserTimeTracker userTime = context.UserTimeTracker.Where(x => x.CoachId == request.CoachId && x.UserId == request.UserId && !x.EndTime.HasValue).FirstOrDefault();
                if (userTime != null)
                {
                    if (request.TimeSpent.HasValue)
                        userTime.EndTime = userTime.StartTime.AddSeconds(request.TimeSpent.Value);
                    else
                        userTime.EndTime = DateTime.UtcNow;
                    userTime.DispositionType = request.Disposition;
                    context.UserTimeTracker.Attach(userTime);
                    context.Entry(userTime).State = EntityState.Modified;
                    context.SaveChanges();
                }
                else if (request.TimeSpent.HasValue && request.TimeSpent.Value == 0)
                {
                    userTime = new UserTimeTracker();
                    userTime.UserId = request.UserId;
                    userTime.CoachId = request.CoachId;
                    userTime.StartTime = DateTime.UtcNow;
                    userTime.Billed = false;
                    context.UserTimeTracker.Add(userTime);
                    context.SaveChanges();
                }
                else if (request.StartTime.HasValue && request.EndTime.HasValue)
                {
                    var validation = context.UserTimeTracker.Any(x => x.UserId == request.UserId && x.Billed && x.StartTime >= request.StartTime.Value && x.EndTime.HasValue);
                    var validaddtion = context.UserTimeTracker.Where(x => x.UserId == request.UserId && x.Billed && x.StartTime >= request.StartTime.Value && x.EndTime.HasValue).ToList();
                    if (!validation)
                    {
                        userTime = new UserTimeTracker();
                        userTime.UserId = request.UserId;
                        userTime.CoachId = request.CoachId;
                        userTime.StartTime = request.StartTime.Value;
                        userTime.EndTime = request.EndTime.Value;
                        userTime.DispositionType = request.Disposition;
                        userTime.Billed = false;
                        context.UserTimeTracker.Add(userTime);
                        context.SaveChanges();
                        return true;
                    }
                    return false;
                }
            }
            return true;
        }

        public IList<TimeTrackerDispositionDto> GetTimeTrackingDispositionList()
        {
            var dispositionsList = context.TimeTrackerDispositions.ToList();
            return Utility.mapper.Map<IList<DAL.TimeTrackerDisposition>, IList<TimeTrackerDispositionDto>>(dispositionsList);
        }

        public TimeTrackerResponse GetParticipantTimeTracker(int userId, int page, int pageSize)
        {
            TimeTrackerResponse response = new TimeTrackerResponse();
            response.totalRecords = context.UserTimeTracker.Include("TimeTrackerDisposition").Where(x => x.UserId == userId && x.EndTime != null).OrderByDescending(x => x.StartTime).Count();
            var timerList = context.UserTimeTracker.Include("TimeTrackerDisposition").Where(x => x.UserId == userId && x.EndTime != null).OrderByDescending(x => x.StartTime).Skip(page * pageSize).Take(pageSize).ToList();
            response.timeList = Utility.mapper.Map<IList<DAL.UserTimeTracker>, IList<UserTimeTrackerDto>>(timerList);
            return response;
        }

        public GetPendingParticipantTimerResponse GetPendingParticipantTimer(int coachId)
        {
            GetPendingParticipantTimerResponse response = new GetPendingParticipantTimerResponse();
            var timer = context.UserTimeTracker.Include("User").Where(x => x.CoachId == coachId && x.EndTime == null).FirstOrDefault();
            response.timer = Utility.mapper.Map<DAL.UserTimeTracker, UserTimeTrackerDto>(timer);
            return response;
        }

        public ReportListResponse ListExternalReports(ReportListRequest request)
        {
            ReportListResponse response = new ReportListResponse();
            var reportLists = context.ExternalReports.Where(x => x.UserId == request.UserId).ToList();
            response.reportLists = Utility.mapper.Map<IList<DAL.ExternalReport>, IList<ExternalReportsDto>>(reportLists);
            return response;
        }

        public void UpdateReportList(List<ExternalReportsDto> reportLists)
        {
            foreach (var report in reportLists)
            {
                var externalReport = Utility.mapper.Map<ExternalReportsDto, DAL.ExternalReport>(report);
                context.ExternalReports.Add(externalReport);
                context.SaveChanges();
            }
        }

        public ExternalReportsDto ReadExternalReport(int reportId)
        {
            var ExternalReports = context.ExternalReports.Include("User").Where(x => x.Id == reportId).ToList();
            return Utility.mapper.Map<DAL.ExternalReport, ExternalReportsDto>(ExternalReports.LastOrDefault());
        }

        public IList<UsersinProgramDto> RecentlyEnrolledUsers(int userId)
        {
            CoachListResponse response = new CoachListResponse();
            DateTime date = DateTime.UtcNow.AddMonths(-1);
            var userinPrograms = context.UsersinPrograms.Include("User").Where(x => x.EnrolledOn > date && x.CoachId == userId).ToList();
            return Utility.mapper.Map<IList<DAL.UsersinProgram>, IList<UsersinProgramDto>>(userinPrograms);
        }

        public IEnumerable<EnrollmentStatusDto> GetEnrollmentStatus()
        {
            return EnrollmentStatusDto.GetAll();
        }

        public IEnumerable<DeclinedEnrollmentReasonsDto> GetDeclinedEnrollmentReasons()
        {
            var declinedEnrollmentReasons = context.DeclinedEnrollmentReasons.OrderBy(x => x.Reason).ToList();
            return Utility.mapper.Map<IList<DAL.DeclinedEnrollmentReason>, IList<DeclinedEnrollmentReasonsDto>>(declinedEnrollmentReasons);

        }

        public bool IsMediOrbisUser(int orgId)
        {
            var mediorbisOrg = context.Organizations.Include("Organizations1").Where(x => x.Name.ToLower() == "mediorbis").FirstOrDefault();
            if (mediorbisOrg == null)
                return false;
            else
                return mediorbisOrg.Id == orgId || mediorbisOrg.Organizations1.Where(x => x.Id == orgId).Any();
        }
    }
}