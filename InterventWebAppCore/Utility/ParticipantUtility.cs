using Intervent.Business.Account;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using static Intervent.HWS.PlacerRequest;

namespace InterventWebApp
{
    public class ParticipantUtility
    {
        public static ReadUserParticipationResponse ReadUserParticipation(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            ReadUserParticipationRequest request = new ReadUserParticipationRequest();
            request.UserId = userId;
            return reader.ReadUserParticipation(request);
        }

        public static ReadParticipantInfoResponse ReadParticipantInfo(int userId, int adminId)
        {
            ParticipantReader reader = new ParticipantReader();
            ReadParticipantInfoRequest request = new ReadParticipantInfoRequest();
            request.userId = userId;
            request.adminId = adminId;
            return reader.ReadParticipantInfo(request);
        }

        public static bool IsIntuityUser(int userId)
        {
            ExternalReader reader = new ExternalReader();
            return reader.GetIntuityUsersByUserId(userId) == null ? false : true;
        }

        public static GetDashboadMessagesResponse GetDashboadMessages(int userId, int page, int pageSize, int? totalRecords, string portalStartDate, bool read, string timeZone, string timeZoneName, string dateFormat, int type, bool isBoth)
        {
            ParticipantReader reader = new ParticipantReader();
            GetDashboadMessagesRequest request = new GetDashboadMessagesRequest();
            request.userId = userId;
            if (portalStartDate != "")
                request.portalStartDate = DateTime.Parse(portalStartDate);
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            request.newMessage = read;
            request.isBoth = isBoth;
            request.timeZone = timeZone;
            request.messageType = type;
            var response = reader.GetDashboadMessages(request);
            var custzone = TimeZoneInfo.FindSystemTimeZoneById(timeZone);
            if (response.dashboardMessages != null)
            {
                for (var i = 0; i < response.dashboardMessages.Count(); i++)
                {
                    var message = response.dashboardMessages[i];
                    if (message.appointment != null)
                    {
                        DateTime time = DateTime.Parse(message.appointment.Date);
                        message.Message = string.Format(Translate.Message("" + message.LanguageItem + ""), TimeZoneInfo.ConvertTimeFromUtc(time, custzone).ToString(dateFormat + " hh:mm tt") + " (" + timeZoneName + ")");
                    }
                    else
                    {
                        if (message.Parameters != null)
                        {
                            var args = message.Parameters.Split('|');
                            for (var j = 0; j < args.Count(); j++)
                            {
                                args[j] = Translate.Message("" + args[j] + "");
                            }
                            message.Message = string.Format(Translate.Message("" + message.LanguageItem + ""), args);
                        }
                        else if (message.Message != null && message.Parameters == null)
                        {
                            message.Message = message.Message;
                        }
                        else
                        {
                            message.Message = Translate.Message("" + message.LanguageItem + "");
                        }
                    }
                }
            }
            return response;
        }

        public static UpdateDashboardMessageResponse UpdateDashboardMessage(int? messageId, int? relatedId, int? messageType, bool? New, bool? Active)
        {
            ParticipantReader reader = new ParticipantReader();
            UpdateDashboardMessageRequest request = new UpdateDashboardMessageRequest();
            request.id = messageId;
            request.relatedId = relatedId;
            request.messageType = messageType;
            request.New = New;
            request.Active = Active;
            return reader.UpdateDashboardMessage(request);
        }

        public static GetNotesResponse GetNotes(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetNotesRequest request = new GetNotesRequest();
            request.Id = userId;
            return reader.GetNotes(request);
        }

        public static GetEligibilityNoteResponse ReadEligibilityNote(int noteId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetNotesRequest request = new GetNotesRequest();
            request.Id = noteId;
            return reader.ReadEligibilityNote(request);
        }

        public static ListWellnessDataResponse ListWellnessData(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            ListWellnessDataRequest request = new ListWellnessDataRequest();
            request.userId = userId;
            return reader.ListWellnessData(request);
        }

        public static ListWellnessDataResponse ListWellnessDataByPage(ListWellnessDataModel model, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            ListWellnessDataRequest request = new ListWellnessDataRequest();
            request.userId = userId;
            request.Page = model.Page;
            request.PageSize = model.PageSize;
            request.TotalRecords = model.TotalRecords;
            return reader.ListWellnessDataByPage(request);
        }

        public static ListGlucoseResponse ListGlucoseData(string uniqueId, int organizationId, bool onlyValidData)

        {
            ParticipantReader reader = new ParticipantReader();
            ListGlucoseRequest request = new ListGlucoseRequest();
            request.uniqueId = uniqueId;
            request.organizationId = organizationId;
            request.onlyValidData = onlyValidData;
            return reader.ListGlucoseData(request);
        }

        public static AddEditWellnessDataResponse AddEditWellnessData(WellnessDataModel model)
        {
            ParticipantReader reader = new ParticipantReader();
            if (model.wellnessData.Weight.HasValue)
            {
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = model.wellnessData.UserId;
                healthDataRequest.HealthData.Weight = model.wellnessData.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.WellnessData;
                healthDataRequest.HealthData.CreatedBy = model.userId;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                reader.AddtoHealthData(healthDataRequest);
            }
            AddEditWellnessDataRequest request = new AddEditWellnessDataRequest();
            if (model.wellnessData.Id == 0)
                model.wellnessData.CollectedOn = DateTime.UtcNow;
            else
                model.wellnessData.UpdatedOn = DateTime.UtcNow;
            model.wellnessData.UpdatedBy = model.userId;
            request.updatedbyUser = model.updatedbyUser;
            request.WellnessData = model.wellnessData;
            return reader.AddEditWellnessData(request);
        }

        public static AddEditTeamsBP_PPRResponse AddEditTeamsBP_PPR(TeamsBP_PPRModel model)
        {
            ParticipantReader reader = new ParticipantReader();
            if (model.TeamsBP_PPR.WellnessData.Id == 0)
            {
                model.TeamsBP_PPR.WellnessData.CollectedOn = DateTime.UtcNow;
                model.TeamsBP_PPR.WellnessData.CollectedBy = model.userId;
            }
            else
            {
                model.TeamsBP_PPR.WellnessData.UpdatedOn = DateTime.UtcNow;
                model.TeamsBP_PPR.WellnessData.UpdatedBy = model.userId;
            }
            return reader.AddEditTeamsBP_PPR(new AddEditTeamsBP_PPRRequest { TeamsBP_PPR = model.TeamsBP_PPR });
        }

        public static ReadWellnessDataResponse ReadWellnessData(int? id, int participantId)
        {
            ParticipantReader reader = new ParticipantReader();
            ReadWellnessDataRequest request = new ReadWellnessDataRequest();
            request.id = id;
            request.participantId = participantId;
            return reader.ReadWellnessData(request);
        }

        public static ReadTeamsBP_PPRResponse ReadTeamsBP_PPRData(int participantId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.ReadTeamsBP_PPRData(new ReadTeamsBP_PPRDataRequest { participantId = participantId });
        }

        public static CanriskQuestionnaireDto GetCanriskResponse(string uniqueId, int? eligibilityId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetCanriskRequest request = new GetCanriskRequest();
            request.uniqueId = uniqueId;
            request.eligibilityId = eligibilityId;
            return reader.GetCanriskResponse(request);
        }

        public static bool DeleteWellnessDataRecord(int Id)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.DeleteWellnessDataRecord(Id);
        }

        public static AddNotesResponse AddEditNotes(NotesDto note, int adminId, int? participantPortalId, int organizationId, int participantId, string timeZone, int? integrationWith, string organizationCode, string uniqueId, int? programType, int? userinProgramId, int? hraId, int southUniversityOrgId, string DTCOrgCode)
        {
            PortalReader portalReader = new PortalReader();
            ParticipantReader reader = new ParticipantReader();
            IncentiveReader incentiveReader = new IncentiveReader();
            AddNotesRequest request = new AddNotesRequest();
            NotesDto noteDto = new NotesDto();
            noteDto = note;
            noteDto.Admin = adminId;
            //Need to check the scenario where there is no active portal
            if (participantPortalId.HasValue)
            {
                noteDto.PortalId = participantPortalId.Value;
            }
            else
            {
                int? OrganizationId = organizationId;
                int?[] organization = { OrganizationId.Value };
                var portals = portalReader.GetPortalList(organization).portals;
                noteDto.PortalId = portals[portals.Count - 1].Id;
            }
            noteDto.userId = participantId;
            request.note = noteDto;
            request.TimeZone = timeZone;
            int? portalProgramId = null;
            bool isStart = false;
            if (note.Type == (int)NoteTypes.Coaching && note.Id == 0)
            {
                if (integrationWith.HasValue && integrationWith.Value == (int)Integrations.Intuity && IsIntuityUser(noteDto.userId))
                {
                    IntuityReader intuityReader = new IntuityReader();
                    AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                    intuityEventRequest.intuityEvent = new IntuityEventDto();
                    intuityEventRequest.intuityEvent.UserId = noteDto.userId;
                    intuityEventRequest.organizationCode = organizationCode;
                    intuityEventRequest.intuityEvent.UniqueId = uniqueId;
                    intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.Coaching_Call_Completion;
                    intuityEventRequest.intuityEvent.EventDate = DateTime.UtcNow;
                    intuityEventRequest.intuityEvent.CreatedBy = noteDto.Admin;
                    intuityReader.AddIntuityEvent(intuityEventRequest, DTCOrgCode);
                }
                if (userinProgramId.HasValue && programType.HasValue && programType.Value == (int)ProgramTypes.Coaching)
                {
                    var id = userinProgramId.Value;
                    ProgramReader programReader = new ProgramReader();
                    noteDto.RefId = id;
                    portalProgramId = programReader.StartProgram(id, out isStart, null);
                }
                else
                {
                    AddNotesResponse noresponse = new AddNotesResponse();
                    noresponse.NoProgram = true;
                    return noresponse;
                }
            }
            else if (note.Type == (int)NoteTypes.Navigation_Call)
            {
                MedicalPlanEligbilityRequest medicalrequest = new MedicalPlanEligbilityRequest();
                medicalrequest.portalId = noteDto.PortalId;
                medicalrequest.participantId = noteDto.userId;
                medicalrequest.checkValidCode = false;
                medicalrequest.isProgram = false;
                var medicalresponse = reader.MedicalPlanEligbility(medicalrequest);
                //it will check whether the incentive is already applied before applying this incentive
                AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                incentivesRequest.incentiveType = IncentiveTypes.Navigation_Incentive;
                incentivesRequest.userId = noteDto.userId;
                incentivesRequest.portalId = noteDto.PortalId;
                incentivesRequest.isEligible = medicalresponse.IsEligbilble;
                incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Navigation_Incentive;
                incentiveReader.AwardIncentives(incentivesRequest);
            }
            else if (note.Type == (int)NoteTypes.Critical_Alert)
            {
                LabReader labReader = new LabReader();
                ReadLabRequest labRequest = new ReadLabRequest();
                labRequest.UserId = participantId;
                labRequest.PortalId = participantPortalId.Value;
                var lab = labReader.ReadLabWork(labRequest).Lab;
                if (lab != null)
                    noteDto.RefId = lab.Id;
            }
            if (note.Type == (int)NoteTypes.BiometricReview && note.Id == 0)
            {
                //Biometric 
                if (userinProgramId.HasValue && programType.HasValue && programType.Value == (int)ProgramTypes.Coaching)
                {
                    var id = userinProgramId.Value;
                    ProgramReader programReader = new ProgramReader();
                    noteDto.RefId = id;
                    portalProgramId = programReader.StartProgram(id, out isStart, null);
                }
                ParticipantReader participantreader = new ParticipantReader();
                MedicalPlanEligbilityRequest medicalrequest = new MedicalPlanEligbilityRequest();
                medicalrequest.portalId = noteDto.PortalId;
                medicalrequest.participantId = noteDto.userId;
                medicalrequest.checkValidCode = false;
                medicalrequest.isProgram = false;
                var medicalresponse = reader.MedicalPlanEligbility(medicalrequest);
                AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                incentivesRequest.incentiveType = IncentiveTypes.Biometric_Review;
                incentivesRequest.userId = noteDto.userId;
                incentivesRequest.portalId = noteDto.PortalId;
                incentivesRequest.isEligible = medicalresponse.IsEligbilble;
                incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Biometric_ReviewIncentive;
                incentivesRequest.adminId = adminId;
                incentiveReader.AwardIncentives(incentivesRequest);
                //HRA not mandatory for Biometric notes
                if (hraId.HasValue)
                    noteDto.RefId = hraId.Value;
            }
            if (note.Type == (int)NoteTypes.HRA_Review && note.Id == 0)
            {
                noteDto.RefId = hraId;
            }
            AddNotesResponse response = reader.AddEditNotes(request);
            if (response.success && portalProgramId.HasValue)
            {
                //1st Session
                if (isStart)
                {
                    AwardIncentivesRequest awardIncentivesRequest = new AwardIncentivesRequest();
                    awardIncentivesRequest.incentiveType = IncentiveTypes.Intervent_1stCoaching;
                    awardIncentivesRequest.userId = noteDto.userId;
                    awardIncentivesRequest.portalId = noteDto.PortalId;
                    awardIncentivesRequest.isEligible = true;
                    awardIncentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                    incentiveReader.AwardIncentives(awardIncentivesRequest);
                    //it will check whether the incentive is already applied before applying this incentive
                    ParticipantReader participantreader = new ParticipantReader();
                    MedicalPlanEligbilityRequest medicalrequest = new MedicalPlanEligbilityRequest();
                    medicalrequest.portalId = noteDto.PortalId;
                    medicalrequest.participantId = noteDto.userId;
                    medicalrequest.checkValidCode = false;
                    medicalrequest.isProgram = false;
                    var medicalresponse = participantreader.MedicalPlanEligbility(medicalrequest);
                    //it will check whether the incentive is already applied before applying this incentive
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.Navigation_Incentive;
                    incentivesRequest.userId = noteDto.userId;
                    incentivesRequest.portalId = noteDto.PortalId;
                    incentivesRequest.isEligible = medicalresponse.IsEligbilble;
                    incentivesRequest.companyIncentiveMessage = IncentiveMessageTypes.Navigation_Incentive;
                    incentiveReader.AwardIncentives(incentivesRequest);

                }
                else //NSession (not the first one)
                {
                    Program_NumberOfCallsRequest numberOfCallsRequest = new Program_NumberOfCallsRequest();
                    numberOfCallsRequest.programsInPortalId = portalProgramId.Value;
                    numberOfCallsRequest.userId = noteDto.userId;
                    numberOfCallsRequest.usersInProgramId = userinProgramId.Value;
                    numberOfCallsRequest.hraId = hraId.Value;
                    numberOfCallsRequest.portalId = noteDto.PortalId;
                    numberOfCallsRequest.orgId = organizationId;
                    numberOfCallsRequest.southUniversityOrgId = southUniversityOrgId;
                    incentiveReader.Program_NumberOfCalls(numberOfCallsRequest);
                }
            }
            return response;
        }

        public static void AssignProgramIncentive(int participantId, int participantPortalId, int userinProgramId, int? hraId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetCoachingCountRequest request = new GetCoachingCountRequest();
            request.userId = participantId;
            request.portalId = participantPortalId;
            request.refId = userinProgramId;
            if (hraId.HasValue)
                request.hraId = hraId.Value;
            var coachingNotes = reader.GetCoachingCount(request);
            if (coachingNotes.count == 1)
            {
                var id = userinProgramId;
                ProgramReader programReader = new ProgramReader();
                bool isStart = false;
                programReader.StartProgram(id, out isStart, coachingNotes.participantNotes[0].NotesDate);
                if (isStart)
                {
                    IncentiveReader incentiveReader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.Intervent_1stCoaching;
                    incentivesRequest.userId = participantId;
                    incentivesRequest.portalId = participantPortalId;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                    incentiveReader.AwardIncentives(incentivesRequest);

                }
            }

        }

        public static GetNoteResponse GetNote(int? Id)
        {
            ParticipantReader reader = new ParticipantReader();
            GetNoteRequest request = new GetNoteRequest();
            request.Id = Id.Value;
            return reader.GetNote(request);
        }

        public static ReadUserTrackingStatusResponse ReadUserTrackingStatus(int userId, int participantPortalId)
        {
            ParticipantReader reader = new ParticipantReader();
            ReadUserTrackingStatusRequest request = new ReadUserTrackingStatusRequest();
            request.userId = userId;
            request.portalId = participantPortalId;
            return reader.ReadUserTrackingStatus(request);
        }

        public static CheckTobaccoUserResponse CheckIfTobaccoUser(bool checkEligibility, int participantId, int participantPortalId)
        {
            ParticipantReader reader = new ParticipantReader();
            CheckTobaccoUserRequest request = new CheckTobaccoUserRequest();
            request.participantId = participantId;
            request.portalId = participantPortalId;
            request.checkEligibility = checkEligibility;
            return reader.CheckIfTobaccoUser(request);
        }

        public static SearchUsersResponse SearchUsers(SearchUserModel Searchrequest, int userId, string timeZone)
        {
            ParticipantReader reader = new ParticipantReader();
            SearchUsersRequest request = new SearchUsersRequest();
            request.userId = userId;
            request.firstName = Searchrequest.FirstName;
            request.lastName = Searchrequest.LastName;
            if (Searchrequest.Organization.HasValue)
                request.organizationId = Searchrequest.Organization.Value;
            request.Id = Searchrequest.Id;
            request.Email = Searchrequest.Email;
            request.RiskCode = Searchrequest.RiskCode;
            request.Page = Searchrequest.page;
            request.Phone = Searchrequest.Phone;
            request.PageSize = Searchrequest.pageSize;
            request.TotalRecords = Searchrequest.totalRecords;
            request.HraStartDate = Searchrequest.HraStartDate;
            request.HraEndDate = Searchrequest.HraEndDate.HasValue ? Searchrequest.HraEndDate.Value.AddDays(1) : Searchrequest.HraEndDate;
            request.timeZone = timeZone;
            return reader.SearchUsers(request);
        }

        public static AdvancedSearchUsersResponse AdvancedSearchUsers(AdvancedSearchRequestModel model, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            AdvancedSearchRequest request = new AdvancedSearchRequest();
            request.SearchText = model.SearchText;
            if (model.OrganizationId.HasValue && model.OrganizationId.Value != 0)
                request.OrganizationId = model.OrganizationId.Value;
            if (model.CoachId.HasValue && model.CoachId.Value != 0)
                request.CoachId = model.CoachId.Value;
            request.MedicalCondition = model.MedicalCondition;
            if (model.ProgramType.HasValue && model.ProgramType.Value != 0)
                request.ProgramType = model.ProgramType;
            if (model.ContactRequirement.HasValue && model.ContactRequirement.Value != 0)
                request.ContactRequirement = model.ContactRequirement;
            request.RecentStats = model.RecentStats;
            request.HraEndDate = model.HraEndDate;
            request.HraStartDate = model.HraStartDate;
            request.Page = model.Page;
            request.PageSize = model.PageSize;
            request.UserId = userId;
            return reader.AdvanceSearchUsers(request);
        }

        public static UpdateUserTrackingStatusResponse UpdateUserTrackingStatus(int UserId, int PortalId, bool? DoNotTrack, bool? DeclinedEnrollment, byte? DeclinedEnrollmentReason)
        {
            ParticipantReader reader = new ParticipantReader();
            UpdateUserTrackingStatusRequest request = new UpdateUserTrackingStatusRequest();
            request.UserId = UserId;
            request.PortalId = PortalId;
            request.DoNotTrack = DoNotTrack;
            request.DeclinedEnroll = DeclinedEnrollment;
            request.DeclinedEnrollmentReason = DeclinedEnrollmentReason;
            return reader.UpdateUserTrackingStatus(request);
        }

        public static ListEligibilityResponse ListEligibility(int? portalId, string uniqueId, string firstName, string lastName, string email, string telephone, string language, DateTime? eligibilityStartDate, DateTime? eligibilityEndDate, DateTime? claimStartDate, DateTime? claimEndDate, byte? enrollmentStatus, string claimDiagnosisMode, bool? canrisk, bool? coachingEnabled, int page, int pageSize, int? totalRecords, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            ListEligibilityRequest request = new ListEligibilityRequest();
            if (portalId.HasValue)
                request.portalId = portalId.Value;
            request.userId = userId;
            request.uniqueId = uniqueId;
            request.firstName = firstName;
            request.lastName = lastName;
            request.Email = email;
            request.Telephone = telephone;
            request.Language = language;
            request.EligibilityStartDate = eligibilityStartDate;
            request.EligibilityEndDate = eligibilityEndDate;
            request.ClaimStartDate = claimStartDate;
            request.ClaimEndDate = claimEndDate;
            request.enrollmentStatus = enrollmentStatus;
            request.ClaimDiagnosisCode = claimDiagnosisMode;
            request.canrisk = canrisk;
            request.CoachingEnabled = coachingEnabled;
            request.Page = page;
            request.PageSize = pageSize;
            request.TotalRecords = totalRecords;
            return reader.ListEligibility(request);
        }

        public static ListEligibilityByUserResponse ListEligibilityByUserName(string name)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.ListEligibilityByUserName(name);
        }

        public static GetEligibilityResponse GetEligibility(int? eligibilityId, string uniqueId, int? portalId)
        {
            ParticipantReader reader = new ParticipantReader();
            List<int> portalIds = new List<int>();
            if (portalId.HasValue)
                portalIds.Add(portalId.Value);
            GetEligibilityRequest request = new GetEligibilityRequest() { EligibilityId = eligibilityId, UniqueId = uniqueId, PortalIds = portalIds };
            return reader.GetEligibility(request);
        }

        public static GetEligibilityResponse GetSpouseEligibility(string uniqueId, int? portalId)
        {
            ParticipantReader reader = new ParticipantReader();
            List<int> portalIds = new List<int>();
            if (portalId.HasValue)
                portalIds.Add(portalId.Value);
            GetEligibilityRequest request = new GetEligibilityRequest() { UniqueId = uniqueId, PortalIds = portalIds };
            return reader.GetSpouseEligibility(request);
        }

        public static GetEligibilityResponse GetEligibilityFromMultiplePortals(int? eligibilityId, string uniqueId, List<int> portalIds)
        {
            ParticipantReader reader = new ParticipantReader();
            GetEligibilityRequest request = new GetEligibilityRequest() { EligibilityId = eligibilityId, UniqueId = uniqueId, PortalIds = portalIds };
            return reader.GetEligibility(request);
        }

        public static ListEligibilityResponse GetSimilarEligibilities(string uniqueId, int? portalId, bool eBenChild)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetSimilarEligibilities(new ListEligibilityRequest() { uniqueId = uniqueId, portalId = portalId, eBenChild = eBenChild });
        }

        public static EligibilityDto CopyPrimaryEligibilityDetails(EligibilityDto primaryParticipant, EligibilityDto newParticipant)
        {
            newParticipant.MedicalPlanCode = primaryParticipant.MedicalPlanCode;
            newParticipant.MedicalPlanStartDate = primaryParticipant.MedicalPlanStartDate;
            newParticipant.MedicalPlanEndDate = primaryParticipant.MedicalPlanEndDate;
            newParticipant.BusinessUnit = primaryParticipant.BusinessUnit;
            newParticipant.RegionCode = primaryParticipant.RegionCode;
            newParticipant.UnionFlag = primaryParticipant.UnionFlag;
            newParticipant.PayType = primaryParticipant.PayType;
            newParticipant.UserStatus = primaryParticipant.UserStatus;
            return newParticipant;
        }

        public static GetInfoforProgramResponse GetInfoforProgram(int participantId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetInfoforProgramRequest request = new GetInfoforProgramRequest();
            request.userId = participantId;
            return reader.GetInfoforProgram(request);
        }

        public static string MedicalEligibility(string code, DateTime? endDate)
        {
            string medicalCodeComment = code;
            if (!string.IsNullOrEmpty(code))
            {
                var medicalPlanCode = CommonUtility.ReadMedicalPlan(code).medicalPlanCode;
                if (medicalPlanCode != null && medicalPlanCode.Description != null)
                    medicalCodeComment = medicalCodeComment + "( " + medicalPlanCode.Description + " )";
                if (code.ToUpper().StartsWith("H"))
                {
                    medicalCodeComment = medicalCodeComment + " | " + "Not Eligible";
                    return medicalCodeComment;
                }
                bool Eligible = true;
                //var medicalPlanCode = CommonUtility.ReadMedicalPlan(code).medicalPlanCode;
                if (medicalPlanCode != null)
                {
                    Eligible = medicalPlanCode.IVEligible;
                }
                if (Eligible)
                    medicalCodeComment = medicalCodeComment + " | " + "Eligible";
                else
                    medicalCodeComment = medicalCodeComment + " | " + "Not Eligible";
                if (endDate.HasValue && endDate.Value.Date < DateTime.Now.Date)
                {
                    medicalCodeComment = medicalCodeComment + " | " + "Expired";
                }
            }
            else
            {
                medicalCodeComment = "None";
            }
            return medicalCodeComment;
        }

        public static string GetLocation(string regionCode)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetLocation(regionCode);
        }

        public static MedicalPlanEligbilityResponse MedicalPlanEligbility(int portalId, int participantId)
        {
            ParticipantReader reader = new ParticipantReader();
            MedicalPlanEligbilityRequest request = new MedicalPlanEligbilityRequest();
            request.portalId = portalId;
            request.participantId = participantId;
            request.checkValidCode = true;
            request.isProgram = true;
            return reader.MedicalPlanEligbility(request);
        }

        public static GetCoachingCountResponse GetCoachingCount(int userId, int portalId, int programId, int? hraId)
        {
            GetCoachingCountRequest request = new GetCoachingCountRequest();
            request.userId = userId;
            request.portalId = portalId;
            request.refId = programId;
            if (hraId.HasValue)
                request.hraId = hraId.Value;
            return new ParticipantReader().GetCoachingCount(request);
        }

        public static string GetGuid(int? eligibilityId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetGuid(eligibilityId);
        }

        public static void TrackCanriskParticipation(string guid, int page, string utm_source, string utm_medium, string utm_campaign, string utm_keywords, int? reasonId, string reason, int? eligibilityId, DateTime dob, byte gender)
        {
            ParticipantReader reader = new ParticipantReader();
            TrackCanriskRequest request = new TrackCanriskRequest();
            request.Guid = guid;
            request.pageCompleted = page;
            request.utm_source = utm_source;
            request.utm_medium = utm_medium;
            request.utm_campaign = utm_campaign;
            request.utm_keywords = utm_keywords;
            request.DOB = dob;
            request.Gender = gender;
            request.Reason = reason;
            request.ReasonId = reasonId;
            request.EligibilityId = eligibilityId;
            reader.TrackCanriskParticipation(request);
        }

        public static bool AddEditEligibilityNotes(EligibilityNotesModel model, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            EligibilityNotesDto eligibilityNotes = new EligibilityNotesDto();
            eligibilityNotes = model.eligibilityNotes;
            eligibilityNotes.CreatedOn = DateTime.UtcNow;
            eligibilityNotes.CreatedBy = userId;
            eligibilityNotes.UpdatedOn = DateTime.UtcNow;
            eligibilityNotes.UpdatedBy = userId;
            eligibilityNotes.Active = true;

            return reader.AddEditEligibilityNotes(eligibilityNotes);
        }

        public static bool RemoveEligibilityNote(int id, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            EligibilityNotesDto eligibilityNotes = new EligibilityNotesDto();
            eligibilityNotes.Id = id;
            eligibilityNotes.UpdatedOn = DateTime.UtcNow;
            eligibilityNotes.UpdatedBy = userId;

            return reader.RemoveEligibilityNote(eligibilityNotes);
        }

        public static GetEligibilityNotesResponse GetEligibilityNotes(string UniqueId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetEligibilityNotesRequest request = new GetEligibilityNotesRequest();
            request.UniqueId = UniqueId;
            return reader.GetEligibilityNotes(request);
        }

        public static ReadAWVReportResponse ListAWVReports(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.ListAWVReports(new ReadAWVReportRequest { UserId = userId });
        }

        #region Survey
        public static AddEditSurveyResponse AddEditSurvey(SurveyResponseModel model, int usersinProgramsId, int participantId, int participantPortalId)
        {
            AddEditSurveyRequest request = new AddEditSurveyRequest();
            ParticipantReader reader = new ParticipantReader();
            List<SurveyResponseDto> surveyList = new List<SurveyResponseDto>();
            if (model.SurveyResponse != null && model.SurveyResponse.Count > 0)
            {
                foreach (var survey in model.SurveyResponse)
                {
                    SurveyResponseDto surveyData = new SurveyResponseDto();
                    surveyData.Id = survey.Id;
                    surveyData.QuestionId = survey.QuestionId;
                    surveyData.UsersinProgramsId = usersinProgramsId;
                    surveyData.Answer = survey.Answer;
                    surveyList.Add(surveyData);
                }
            }
            request.surveyList = surveyList;
            request.isEligibleForIncentive = model.EligibleForIncentive;
            request.comments = model.Comments;
            request.userId = participantId;

            int portalId = 0;
            if (model.EligibleForIncentive)
                portalId = participantPortalId;

            request.portalId = portalId;

            return reader.AddEditSurvey(request);
        }

        public static List<SurveyQuestionDto> GetSurveyQuestions()
        {
            ReportReader reader = new ReportReader();
            return reader.GetSurveyQuestions();
        }

        public static GetSurveyCompletedResponse GetSurveyCompletedStatus(int UsersinProgramsId)
        {
            ReportReader reader = new ReportReader();
            GetSurveyCompletedRequest request = new GetSurveyCompletedRequest();
            request.usersinProgramId = UsersinProgramsId;
            return reader.GetSurveyCompletedStatus(request);
        }

        #endregion

        public static GetPrevYearStatusResponse GetPrevYearStatus(int participantId, string participantTimeZone, string userTimeZone, bool? getCoachingInfo = false, bool? getFollowupDetails = false)
        {
            ParticipantReader reader = new ParticipantReader();
            GetPrevYearStatusRequest request = new GetPrevYearStatusRequest();
            request.userId = participantId;
            request.getCoachingInfo = getCoachingInfo.Value;
            request.getFollowupDetails = getFollowupDetails.Value;
            if (!string.IsNullOrEmpty(participantTimeZone))
                request.timeZone = participantTimeZone;
            else
                request.timeZone = userTimeZone;
            return reader.GetPrevYearStatus(request);
        }

        public static AddTestimonialResponse AddTestimonial(string feedback, string SignedName, string Date, int participantId, int participantPortalId)
        {
            AddTestimonialRequest request = new AddTestimonialRequest();
            ParticipantReader reader = new ParticipantReader();
            request.userid = participantId;
            request.portalid = participantPortalId;
            request.feedback = feedback;
            request.SignedName = SignedName;
            request.Date = Date;
            return reader.AddTestimonial(request);
        }

        public static bool AcceptTerms(int participantId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.AcceptTerms(participantId);
        }

        public static void AddUserForm(string fileName, int userId, int formTypeId, int participantPortalId)
        {
            ParticipantReader reader = new ParticipantReader();
            SaveUserFormRequest request = new SaveUserFormRequest();
            request.userId = userId;
            request.portalId = participantPortalId;
            request.reference = fileName;
            request.formTypeId = formTypeId;
            reader.SaveUserForm(request);
        }

        public static GetUserFormsResponse GetUserForms(int participantId, int participantPortalId)
        {
            ParticipantReader reader = new ParticipantReader();
            GetUserFormsRequest request = new GetUserFormsRequest();
            request.userId = participantId;
            request.portalId = participantPortalId;
            return reader.GetUserForms(request);
        }

        public static ApproveUserFormResponse ApproveForm(int Id, int adminId)
        {
            ParticipantReader reader = new ParticipantReader();
            ApproveUserFormRequest request = new ApproveUserFormRequest();
            request.Id = Id;
            request.AdminId = adminId;
            return reader.ApproveForm(request);
        }

        public static string SubmitCanriskQuestionnaire1(CanriskModel modelDto, int OrganizationId, bool isEligible, string languagePreference)
        {
            ParticipantReader reader = new ParticipantReader();
            SubmitCanriskQuestionnaireRequest request = new SubmitCanriskQuestionnaireRequest();
            CommonReader commonReader = new CommonReader();
            var model = modelDto.canrisk;
            var uniqueId = "";
            if (!model.EligibilityId.HasValue)
            {
                AddEditEligibilityRequest eligRequest = new AddEditEligibilityRequest();
                EligibilityDto eligDto = new EligibilityDto();
                var names = model.Name.Split(' ');
                var lastName = names.Length > 1 ? (names.Length > 2 ? names[1] + " " + names[2] : names[1]) : "";
                if (!String.IsNullOrEmpty(lastName))
                {
                    var uniqueName = lastName.Length >= 4 ? lastName.Substring(0, 4) : lastName;
                    eligDto.UniqueId = uniqueName + model.DOB.GetValueOrDefault().Year.ToString() + DateTime.UtcNow.ToString("MMddHHmmss");
                }
                else if (!String.IsNullOrEmpty(names[0]))
                {
                    var uniqueName = names[0].Length >= 4 ? names[0].Substring(0, 4) : names[0];
                    eligDto.UniqueId = uniqueName.Substring(0, 4) + model.DOB.GetValueOrDefault().Year.ToString() + DateTime.UtcNow.ToString("MMddHHmmss");
                }
                eligDto.FirstName = names[0];
                eligDto.LastName = !String.IsNullOrEmpty(lastName) ? lastName : "BLANK";
                eligDto.DOB = model.DOB;
                eligDto.Gender = model.Gender.HasValue ? (model.Gender == 1 ? GenderDto.Male : GenderDto.Female) : null;
                eligDto.Zip = model.Zip;
                eligDto.Email = model.Email.Trim();
                eligDto.HomeNumber = model.PhoneNumber;
                eligDto.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
                eligDto.UserStatus = EligibilityUserStatusDto.Active;
                eligDto.PortalId = new AccountManager().CurrentPortalId(OrganizationId).PortalId ?? 0;
                eligDto.CreateDate = DateTime.UtcNow;
                eligRequest.Eligibility = eligDto;
                var eligResponse = reader.AddEditEligibility(eligRequest);
                if (!eligResponse.success)
                {
                    return "";
                }
                model.EligibilityId = eligResponse.Eligibility.Id;
                uniqueId = eligDto.UniqueId;
                UserEligibilitySettingDto settingDto = new UserEligibilitySettingDto
                {
                    UniqueId = uniqueId,
                    OrganizationId = OrganizationId,
                    Language = !string.IsNullOrEmpty(languagePreference) ? languagePreference : "en-us"
				};
				reader.UpdateUserEligibilitySetting(new UpdateUserEligibilitySettingRequest { UserEligibilitySetting = settingDto });
			}
			request.canriskAnswers = model;
			request.canriskAnswers.isEligible = isEligible;
			request.canriskAnswers.CompletedOn = DateTime.UtcNow;
			reader.SubmitCanriskQuestionnaire(request);
			return uniqueId;
		}


		public static string SubmitCanriskQuestionnaire(CanriskModel modelDto, int OrganizationId, bool isEligible, string languagePreference)
		{
			ParticipantReader reader = new ParticipantReader();
			SubmitCanriskQuestionnaireRequest request = new SubmitCanriskQuestionnaireRequest();
			CommonReader commonReader = new CommonReader();
			var model = modelDto.canrisk;
			var uniqueId = "";
			if (!model.EligibilityId.HasValue)
			{
				AddEditEligibilityRequest eligRequest = new AddEditEligibilityRequest();
				EligibilityDto eligDto = new EligibilityDto();
				var names = model.Name.Split(' ');
				var lastName = names.Length > 1 ? (names.Length > 2 ? names[1] + " " + names[2] : names[1]) : "";
				if (!String.IsNullOrEmpty(lastName))
				{
					var uniqueName = lastName.Length >= 4 ? lastName.Substring(0, 4) : lastName;
					eligDto.UniqueId = uniqueName + model.DOB.GetValueOrDefault().Year.ToString() + DateTime.UtcNow.ToString("MMddHHmmss");
				}
				else if (!String.IsNullOrEmpty(names[0]))
				{
					var uniqueName = names[0].Length >= 4 ? names[0].Substring(0, 4) : names[0];
					eligDto.UniqueId = uniqueName.Substring(0, 4) + model.DOB.GetValueOrDefault().Year.ToString() + DateTime.UtcNow.ToString("MMddHHmmss");
				}
				eligDto.FirstName = names[0];
				eligDto.LastName = !String.IsNullOrEmpty(lastName) ? lastName : "BLANK";
				eligDto.DOB = model.DOB;
				eligDto.Gender = model.Gender.HasValue ? (model.Gender == 1 ? GenderDto.Male : GenderDto.Female) : null;
				eligDto.Zip = model.Zip;
				eligDto.Email = model.Email.Trim();
				eligDto.HomeNumber = model.PhoneNumber;
				eligDto.UserEnrollmentType = EligibilityUserEnrollmentTypeDto.Patient;
				eligDto.UserStatus = EligibilityUserStatusDto.Active;
				eligDto.PortalId = new AccountManager().CurrentPortalId(OrganizationId).PortalId ?? 0;
				eligDto.CreateDate = DateTime.UtcNow;
				eligRequest.Eligibility = eligDto;
				var eligResponse = reader.AddEditEligibility(eligRequest);
				if (!eligResponse.success)
				{
					return "";
				}
				model.EligibilityId = eligResponse.Eligibility.Id;
				uniqueId = eligDto.UniqueId;
				UserEligibilitySettingDto settingDto = new UserEligibilitySettingDto
				{
					UniqueId = uniqueId,
					OrganizationId = OrganizationId,
					Language = languagePreference
				};
                reader.UpdateUserEligibilitySetting(new UpdateUserEligibilitySettingRequest { UserEligibilitySetting = settingDto });
            }
            request.canriskAnswers = model;
            request.canriskAnswers.isEligible = isEligible;
            request.canriskAnswers.CompletedOn = DateTime.UtcNow;
            reader.SubmitCanriskQuestionnaire(request);
            return uniqueId;
        }

        public static bool CheckIfExists(CanriskModel modelDto)
        {
            ParticipantReader reader = new ParticipantReader();
            EligibilityDto eligDto = new EligibilityDto();
            var names = modelDto.canrisk.Name.Split(' ');
            var lastName = names.Length > 1 ? (names.Length > 2 ? names[1] + " " + names[2] : names[1]) : "";
            eligDto.FirstName = names[0];
            eligDto.LastName = !String.IsNullOrEmpty(lastName) ? lastName : "BLANK";
            eligDto.DOB = modelDto.canrisk.DOB;
            return reader.CheckIfExists(eligDto);
        }

        public static CanriskEligibilityModel GetCanriskEligibility(CanriskModel modelDto, int score)
        {
            CanriskEligibilityModel response = new CanriskEligibilityModel();
            var age = CommonUtility.GetAge(modelDto.canrisk.DOB ?? DateTime.MinValue);
            var bmi = CommonUtility.GetBMI(modelDto.canrisk.Height ?? 0, modelDto.canrisk.Weight ?? 0);
            if (score > 32 || (modelDto.canrisk.PreDiabetes.HasValue && modelDto.canrisk.PreDiabetes.Value == 1))
            {
                response.isEligible = true;
                if (score > 32)
                {
                    response.riskLevel = Translate.Message("L3453");
                    response.qualifyText = Translate.Message("L3454");
                }
            }
            var glucMin = ListOptions.ToImperial((float)(6.1), BioLookup.Glucose, CommonUtility.ListMeasurements(2).Measurements);
            var glucMax = ListOptions.ToImperial((float)(6.9), BioLookup.Glucose, CommonUtility.ListMeasurements(2).Measurements);
            if ((modelDto.canrisk.Glucose.HasValue && modelDto.canrisk.Glucose >= glucMin && modelDto.canrisk.Glucose <= glucMax) || (modelDto.canrisk.A1c.HasValue && modelDto.canrisk.A1c >= 6 && modelDto.canrisk.A1c < 6.5))

            {
                response.isEligible = true;
                response.qualifyText = Translate.Message("L3455");
                response.riskLevel = "";
            }
            else if (score <= 22)
            {
                response.riskLevel = Translate.Message("L3456");
                if (modelDto.canrisk.PreDiabetes.HasValue && modelDto.canrisk.PreDiabetes.Value == 1)
                    response.qualifyText = Translate.Message("L3892");
                else if (age >= 45 && bmi >= 30)
                {
                    response.isEligible = true;
                    response.qualifyText = Translate.Message("L4261");
                    response.riskLevel = "";
                }
                else
                    response.qualifyText = Translate.Message("L3457");
            }
            else if (score > 22 && score <= 32)
            {
                response.riskLevel = Translate.Message("L3458");
                if (modelDto.canrisk.PreDiabetes.HasValue && modelDto.canrisk.PreDiabetes.Value == 1)
                    response.qualifyText = Translate.Message("L3893");
                else if (age >= 45 && bmi >= 30)
                {
                    response.isEligible = true;
                    response.qualifyText = Translate.Message("L4262");
                    response.riskLevel = "";
                }
                else
                    response.qualifyText = Translate.Message("L3459");
            }
            return response;
        }

        public static void DeleteUserForm(int formTypeId, int participantId, int participantPortalId)
        {
            ParticipantReader participantReader = new ParticipantReader();
            DeleteUserFormRequest request = new DeleteUserFormRequest();
            request.FormTypeId = formTypeId;
            request.UserId = participantId;
            request.PortalId = participantPortalId;
            participantReader.DeleteUserForm(request);
        }

        public static int GetCanriskScore(CanriskModel model)
        {
            var score = 0;
            var age = CommonUtility.GetAge(model.canrisk.DOB ?? DateTime.MinValue);
            var bmi = CommonUtility.GetBMI(model.canrisk.Height ?? 0, model.canrisk.Weight ?? 0);
            var mothersEthnicity = model.canrisk.MothersEthnic;
            var fathersEthnicity = model.canrisk.FathersEthnic;
            var ethnicScore = 0;
            var waist = model.canrisk.Waist;
            //age
            if (age >= 45 && age <= 54)
                score += 7;
            else if (age >= 55 && age <= 64)
                score += 13;
            else if (age >= 65 && age <= 74)
                score += 15;
            //gender
            score += (model.canrisk.Gender == 1 ? 6 : 0);
            //bmi
            if (bmi >= 25 && bmi < 30)
                score += 4;
            else if (bmi >= 30 && bmi < 35)
                score += 9;
            else if (bmi >= 35)
                score += 14;
            //waist
            if (model.canrisk.Gender == 1)
            {
                if (waist >= 37 && waist <= 40)
                    score += 4;
                else if (waist > 40)
                    score += 6;
            }
            else
            {
                if (waist >= 31.5 && waist <= 35)
                    score += 4;
                else if (waist > 35)
                    score += 6;
            }
            score += (model.canrisk.BriskWalking ?? 2) == 1 ? 0 : 1; //brisk walking
            score += (model.canrisk.FruitsandVeggies ?? 2) == 1 ? 0 : 2; //veggies and fruits
            //BP and sugar test
            score += (model.canrisk.BloodPressure ?? 2) == 1 ? 4 : 0;
            score += (model.canrisk.BloodGlucose ?? 2) == 1 ? 14 : 0;
            //birth
            score += (model.canrisk.GiveBirth ?? 2) == 1 ? 1 : 0;
            //diabetes
            score += (model.canrisk.DiabetesMother ?? 2) == 1 ? 2 : 0;
            score += (model.canrisk.DiabetesFather ?? 2) == 1 ? 2 : 0;
            score += (model.canrisk.DiabetesSiblings ?? 2) == 1 ? 2 : 0;
            score += (model.canrisk.DiabetesChildren ?? 2) == 1 ? 2 : 0;
            //ethnicity
            if (mothersEthnicity == 2 || mothersEthnicity == 6)
                ethnicScore = 3;
            else if (mothersEthnicity == 3)
                ethnicScore = 5;
            else if (mothersEthnicity == 4)
                ethnicScore = 10;
            else if (mothersEthnicity == 5)
                ethnicScore = 11;
            //father ethnicity
            if ((fathersEthnicity == 2 || fathersEthnicity == 6) && ethnicScore < 3)
                ethnicScore = 3;
            else if (fathersEthnicity == 3 && ethnicScore < 5)
                ethnicScore = 5;
            else if (fathersEthnicity == 4 && ethnicScore < 10)
                ethnicScore = 10;
            else if (fathersEthnicity == 5 && ethnicScore < 11)
                ethnicScore = 11;
            //education
            score += ethnicScore;
            score += (model.canrisk.Education ?? 0) == 4 ? 5 : 0;
            score += (model.canrisk.Education ?? 0) == 3 ? 1 : 0;
            return score;
        }

        public static void UpdateEligiblity(UpdateEligiblityModel model)
        {
            ParticipantReader reader = new ParticipantReader();
            UpdateEligibilityDetailsRequest request = new UpdateEligibilityDetailsRequest();
            request.isSecEmail = model.isSecEmail;
            request.EligibilityId = model.Id;
            if (model.isSecEmail)
            {
                request.email2 = model.email2;
            }
            else
            {
                request.FalseReferral = model.falseReferral;
                request.Language = !string.IsNullOrEmpty(model.language) ? model.language.Trim() : String.Empty;
                request.enrollmentStatus = model.enrollmentStatus;
                request.declinedEnrollmentReason = model.declinedEnrollmentReason;
            }
            reader.UpdateEligiblity(request);
        }

        public static bool CheckIfEligible(string postalCode)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.CheckIfEligible(postalCode);
        }

        public static List<ContactRequirementsAlertDto> GetAllContactRequirements()
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetAllContactRequirementAlerts();
        }

        public static bool TrackTime(int userId, int coachId, int? timeSpent, int? disposition, bool forceEnd, DateTime? StartTime = null, DateTime? EndTime = null)
        {
            ParticipantReader reader = new ParticipantReader();
            UpdateTimeTrackingRequest request = new UpdateTimeTrackingRequest();
            request.UserId = userId;
            request.CoachId = coachId;
            request.TimeSpent = timeSpent;
            request.Disposition = disposition;
            request.ForceEnd = forceEnd;
            request.StartTime = StartTime;
            request.EndTime = EndTime;
            return reader.AddEditTrackingTime(request);
        }

        public static TimeTrackerResponse GetParticipantTrackTime(int userId, int page, int pageSize)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetParticipantTimeTracker(userId, page, pageSize);
        }

        public static GetPendingParticipantTimerResponse CheckPendingParticipantTimer(int coachId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetPendingParticipantTimer(coachId);
        }

        public static ReportListResponse ListExternalReports(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.ListExternalReports(new ReportListRequest { UserId = userId });
        }

        public static IList<TimeTrackerDispositionDto> GetTimeTrackingDispositionList()
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.GetTimeTrackingDispositionList();
        }

        public static byte[] PullExternalReport(int reportId, int participantId, int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            var response = reader.ReadExternalReport(reportId);
            if (response != null && (response.UserId == participantId || PortalUtility.GetFilteredOrganizationsList(userId).Select(x => x.Id).Contains(response.User.OrganizationId)))
            {
                return response.ReportData;
            }
            return null;
        }

        public static byte[] GetRPMSummaryGraph(string uniqueId, DateTime startDate, DateTime endDate, string teamsBPApiKey, string teamsBPURL)
        {
            var apiResponse = Placer.GetRPMSummaryGraph(new GetRPMSummaryGraph { uniqueId = uniqueId, startDate = startDate, endDate = endDate }, teamsBPApiKey, teamsBPURL);
            return apiResponse.Status ? apiResponse.RPMGraphData.GraphData : null;
        }

        public static IList<UsersinProgramDto> RecentlyEnrolledUsers(int userId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.RecentlyEnrolledUsers(userId);
        }

        public static bool IsMediOrbisUser(int orgId)
        {
            ParticipantReader reader = new ParticipantReader();
            return reader.IsMediOrbisUser(orgId);
        }
    }
}