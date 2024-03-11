using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class PortalUtility
    {
        public static GetOrganizationDetailsResponse ListOrganizationsForSetup(OrganizationListModel model)
        {
            PortalReader reader = new PortalReader();
            ListOrganizationsRequest request = new ListOrganizationsRequest();
            if (model != null)
            {
                request.ParentOrganizationId = model.parentOrganizationId;
                if (model.removechildOrganizations.HasValue)
                    request.removechildOrganizations = model.removechildOrganizations.Value;
                request.Page = model.page;
                request.PageSize = model.pageSize;
                request.TotalRecords = model.totalRecords;
                request.FilterBy = model.filterBy;
                request.Search = model.search;
            }
            return reader.ListOrganizationsForSetup(request);
        }
        public static ListOrganizationsResponse ListOrganizations(OrganizationListModel model)
        {
            PortalReader reader = new PortalReader();
            ListOrganizationsRequest request = new ListOrganizationsRequest();
            if (model != null)
            {
                request.ParentOrganizationId = model.parentOrganizationId;
                if (model.removechildOrganizations.HasValue)
                    request.removechildOrganizations = model.removechildOrganizations.Value;
                request.includeParentOrganization = model.includeParentOrganization;
                request.Page = model.page;
                request.PageSize = model.pageSize;
                request.TotalRecords = model.totalRecords;
            }
            return reader.ListOrganizations(request);
        }

        public static ListOrganizationsResponse GetParentOrganizationsDetails()
        {
            PortalReader reader = new PortalReader();
            return reader.GetParentOrganizationsDetails();
        }

        public static OrganizationDto GetOrganizationById(int Id)
        {
            PortalReader reader = new PortalReader();
            return reader.GetOrganizationDetails(Id);
        }

        public static IList<OrganizationDto> GetFilteredOrganizationsList(int userId)
        {
            PortalReader reader = new PortalReader();
            return reader.GetFilteredOrganizationsList(userId).Organizations;
        }

        public static ReadOrganizationResponse ReadOrganization(string OrganizationName, int? orgId)
        {
            PortalReader reader = new PortalReader();

            ReadOrganizationRequest request = new ReadOrganizationRequest();
            request.Name = OrganizationName;
            if (orgId.HasValue)
                request.orgId = orgId.Value;
            var response = reader.ReadOrganization(request);
            return response;
        }

        public static AddEditOrganizationResponse AddEditOrganization(string OrganizationName, string description, int? Id, int? parentOrganizationId, string groups, string url, string contactnumber, string contactemail, bool IsAutoActivateEmail, bool IsActive, bool SSO, bool TermsForSSO, bool OwnCoach, byte? IntegrationWith)
        {
            PortalReader reader = new PortalReader();

            AddEditOrganizationRequest request = new AddEditOrganizationRequest();
            request.Organization = new OrganizationDto();
            request.Organization.Id = Id;
            request.Organization.Url = url;
            request.Organization.Name = OrganizationName;
            request.Organization.Description = description;
            request.Organization.ParentOrganizationId = parentOrganizationId;
            request.Organization.ContactNumber = contactnumber;
            request.Organization.ContactEmail = contactemail;
            request.groups = groups;
            request.Organization.EmailValidationRequired = IsAutoActivateEmail;
            request.Organization.Active = IsActive;
            request.Organization.SSO = SSO;
            request.Organization.TermsForSSO = TermsForSSO;
            request.Organization.OwnCoach = OwnCoach;
            request.Organization.IntegrationWith = IntegrationWith;
            return reader.AddEditOrganization(request);
        }

        public static ListPortalsResponse ListPortals(int? OrganizationId)
        {
            PortalReader reader = new PortalReader();
            ListPortalsRequest request = new ListPortalsRequest();
            if (OrganizationId.HasValue)
                request.organizationId = OrganizationId.Value;
            return reader.ListPortals(request);
        }

        public static AddEditPortalResponse AddPortal(PortalModel model)
        {
            PortalReader reader = new PortalReader();
            AddEditPortalRequest request = new AddEditPortalRequest();
            request.Portal = new PortalDto();
            request.Portal.EndDate = model.portal.EndDate.ToString();
            request.Portal.StartDate = model.portal.StartDate.ToString();
            request.Portal.Name = model.portal.Name;
            if (model.portal.OrganizationId != 0)
                request.Portal.OrganizationId = model.portal.OrganizationId;
            if (model.portal.Id != 0)
            {
                request.Portal.Id = model.portal.Id;
                request.Portal.Active = model.portal.Active;
            }
            else
                request.Portal.Active = true;
            request.Portal.ContactText = model.portal.ContactText;
            return reader.AddPortal(request);
        }

        public static AddEditPortalResponse EditPortal(PortalModel model)
        {
            PortalReader reader = new PortalReader();
            AddEditPortalRequest request = new AddEditPortalRequest();
            request.Portal = new PortalDto();
            if (model.portal.Id != 0)
                request.Portal.Id = model.portal.Id;
            request.UpdateFor = model.updateFor;
            switch (model.updateFor)
            {
                case 1:
                    request.Portal.EndDate = model.portal.EndDate.ToString();
                    request.Portal.StartDate = model.portal.StartDate.ToString();
                    request.Portal.Name = model.portal.Name;
                    if (model.portal.OrganizationId != 0)
                        request.Portal.OrganizationId = model.portal.OrganizationId;
                    if (model.portal.Id != 0)
                        request.Portal.Active = model.portal.Active;
                    else
                        request.Portal.Active = true;
                    request.Portal.languagesStr = model.portal.languagesStr;
                    request.Portal.specializationsStr = model.portal.specializationsStr;
                    break;
                case 2:
                    request.Portal.HasHRA = model.portal.HasHRA;
                    request.Portal.HRAVer = model.portal.HRAVer;
                    request.Portal.HRAValidity = model.portal.HRAValidity;
                    request.Portal.AWV = model.portal.AWV;
                    request.Portal.CACScanQuestion = model.portal.CACScanQuestion;
                    request.Portal.GINAQuestion = model.portal.GINAQuestion;
                    request.Portal.ClientNameInReport = model.portal.ClientNameInReport;
                    request.Portal.AutoImmune = model.portal.AutoImmune;
                    request.Portal.MailScoreCard = model.portal.MailScoreCard;
                    request.Portal.CarePlan = model.portal.CarePlan;
                    request.Portal.CarePlanType = model.portal.CarePlanType;
                    request.Portal.LabProceduresStr = model.portal.LabProceduresStr;
                    request.Portal.LabIntegration = model.portal.LabIntegration;
                    request.Portal.usePreviousLabs = model.portal.usePreviousLabs;
                    request.Portal.ValidLabs = model.portal.ValidLabs;
                    request.Portal.ShowPostmenopausal = model.portal.ShowPostmenopausal;
                    request.Portal.SelfEnterLabs = model.portal.SelfEnterLabs;
                    request.Portal.WelcomeText = model.portal.WelcomeText;
                    break;
                case 3:
                    request.Portal.HasCoachingProgram = model.portal.HasCoachingProgram;
                    request.Portal.HasSelfHelpProgram = model.portal.HasSelfHelpProgram;
                    request.Portal.HRAforCoachingProgram = model.portal.HRAforCoachingProgram;
                    request.Portal.HRAforSelfHelpProgram = model.portal.HRAforSelfHelpProgram.HasValue ? model.portal.HRAforSelfHelpProgram : false;
                    request.Portal.ShowPricing = model.portal.ShowPricing;
                    request.Portal.ShowProgramOption = model.portal.ShowProgramOption;
                    request.Portal.CoachingConditionsStr = model.portal.CoachingConditionsStr;
                    request.Portal.SelfScheduling = model.portal.SelfScheduling;
                    request.Portal.AssignPrograms = model.portal.AssignPrograms;
                    break;
                case 4:
                    request.Portal.FollowUpforCoaching = model.portal.FollowUpforCoaching.HasValue ? model.portal.FollowUpforCoaching : false;
                    request.Portal.FollowUpforSelfHelp = model.portal.FollowUpforSelfHelp.HasValue ? model.portal.FollowUpforSelfHelp : false;
                    request.Portal.followupTypeStr = model.portal.followupTypeStr;
                    request.Portal.followuplabStr = model.portal.followuplabStr;
                    request.Portal.selfhelpfollowuplabStr = model.portal.selfhelpfollowuplabStr;
                    request.Portal.selfhelpFollowupTypeStr = model.portal.selfhelpFollowupTypeStr;
                    request.Portal.FollowUpValidity = model.portal.FollowUpValidity;
                    break;
                case 5:
                    request.Portal.IncompleteHRA = model.portal.IncompleteHRA;
                    request.Portal.HRAOnly = model.portal.HRAOnly;
                    request.Portal.HRAandBiometrics = model.portal.HRAandBiometrics;
                    request.Portal.FirstCoaching = model.portal.FirstCoaching;
                    request.Portal.CompletedCoaching = model.portal.CompletedCoaching;
                    request.Portal.FirstTimeHRA = model.portal.FirstTimeHRA;
                    request.Portal.ReportText = model.portal.ReportText;
                    request.Portal.ProgramText = model.portal.ProgramText;
                    request.Portal.ContactText = model.portal.ContactText;

                    break;
                case 6:
                    request.Portal.EligibilityImportLoadFlag = model.portal.EligibilityImportLoadFlag;
                    request.Portal.EligibilityTerminateFlag = model.portal.EligibilityTerminateFlag;
                    request.Portal.ProcessInterventCodes = model.portal.ProcessInterventCodes;
                    request.Portal.ProcessLivongoCodes = model.portal.ProcessLivongoCodes;
                    request.Portal.EligibilityFolderPath = model.portal.EligibilityFolderPath;
                    request.Portal.ClaimsDirectoryPath = model.portal.ClaimsDirectoryPath;
                    request.Portal.EligibilityFormat = model.portal.EligibilityFormat;
                    request.Portal.EligtoIntuity = model.portal.EligtoIntuity;

                    break;
                case 7:
                    request.Portal.CampaignCalls = model.portal.CampaignCalls;
                    request.Portal.AppointmentCalls = model.portal.AppointmentCalls;
                    request.Portal.TrackingCalls = model.portal.TrackingCalls;
                    request.Portal.AllowCardiacQuestion = model.portal.AllowCardiacQuestion;
                    request.Portal.NoOfTrackingCalls = model.portal.NoOfTrackingCalls;
                    request.Portal.NoProgDays = model.portal.NoProgDays;
                    request.Portal.LabCorpAttachment = model.portal.LabCorpAttachment;
                    request.Portal.DoctorOfficeAttachment = model.portal.DoctorOfficeAttachment;
                    if (model.portal.CampaignStartDate != null)
                        request.Portal.CampaignStartDate = model.portal.CampaignStartDate.ToString();
                    if (model.portal.CampaignEndDate != null)
                        request.Portal.CampaignEndDate = model.portal.CampaignEndDate.ToString();
                    request.Portal.NeedCareplanApproval = model.portal.NeedCareplanApproval;
                    request.Portal.CareplanPath = model.portal.CareplanPath;
                    request.Portal.ShowTimeTracker = model.portal.ShowTimeTracker;
                    request.Portal.ProviderDetails = model.portal.ProviderDetails;
                    request.Portal.FaxReports = model.portal.FaxReports;
                    request.Portal.KitAlert = model.portal.KitAlert;
                    request.Portal.EmployerIncentiveText = model.portal.EmployerIncentiveText;
                    break;
            }
            return reader.EditPortal(request);
        }

        public static bool DeleteOrganization(int Id)
        {
            PortalReader reader = new PortalReader();
            return reader.DeleteOrganization(Id);
        }

        public static bool DeletePortal(int Id)
        {
            PortalReader reader = new PortalReader();
            return reader.DeletePortal(Id);
        }

        public static ReadPortalResponse ReadPortal(int Id)
        {
            PortalReader reader = new PortalReader();
            ReadPortalRequest request = new ReadPortalRequest();
            request.portalId = Id;
            return reader.ReadPortal(request);
        }

        public static ReadParticipantInactivePortalResponse ReadParticipantInactivePortal(int participantId)
        {
            PortalReader reader = new PortalReader();
            ReadParticipantInactivePortalRequest request = new ReadParticipantInactivePortalRequest();
            request.participantId = participantId;
            return reader.ReadParticipantInactivePortal(request);
        }

        public static ListProgramsinPortalsResponse ListProgramsinPortals(int portalId)
        {
            PortalReader reader = new PortalReader();
            ListProgramsinPortalsRequest request = new ListProgramsinPortalsRequest();
            request.portalId = portalId;
            return reader.ListProgramsinPortals(request);
        }

        public static ReadPrograminPortalResponse ReadPrograminPortal(int PrograminPortalId, string language = null)
        {
            PortalReader reader = new PortalReader();
            ReadPrograminPortalRequest request = new ReadPrograminPortalRequest();
            request.PrograminPortalId = PrograminPortalId;
            if (string.IsNullOrEmpty(language))
                language = ListOptions.DefaultLanguage;
            request.language = language;
            return reader.ReadPrograminPortal(request);
        }

        public static AddEditProgramsinPortalResponse AddEditProgramsinPortal(int? id, int portalId, short progId, decimal progCost, string moreInfo, bool isactive, string nameforUser, string descriptionforUser, string nameforAdmin, string descriptionforAdmin, int? apptCallTemplateId, string language, int? sortOrder)
        {
            PortalReader reader = new PortalReader();
            AddEditProgramsinPortalRequest request = new AddEditProgramsinPortalRequest();
            ProgramsinPortalDto programsinPortalDto = new ProgramsinPortalDto();
            if (id.HasValue)
                programsinPortalDto.Id = id.Value;
            programsinPortalDto.PortalId = portalId;
            programsinPortalDto.ProgramId = progId;
            programsinPortalDto.Cost = progCost;
            programsinPortalDto.MoreInfo = moreInfo;
            programsinPortalDto.Active = isactive;
            programsinPortalDto.ApptCallTemplateId = apptCallTemplateId;
            programsinPortalDto.NameforUser = nameforUser;
            programsinPortalDto.DescriptionforUser = descriptionforUser;
            programsinPortalDto.NameforAdmin = nameforAdmin;
            programsinPortalDto.DescriptionforAdmin = descriptionforAdmin;
            programsinPortalDto.SortOrder = sortOrder;
            request.ProgramsinPortal = programsinPortalDto;
            request.language = language;
            return reader.AddEditProgramsinPortal(request);
        }

        public static DeleteProgramsinPortalResponse DeleteProgramsinPortal(int PortalId, int ProgramId)
        {
            PortalReader reader = new PortalReader();
            DeleteProgramsinPortalRequest request = new DeleteProgramsinPortalRequest();
            request.PortalId = PortalId;
            request.ProgramId = ProgramId;
            return reader.DeleteProgramsinPortal(request);
        }

        public static GetPortalIncentivesResponse GetPortalIncentives(int portalId)
        {
            PortalReader reader = new PortalReader();
            GetPortalIncentivesRequest request = new GetPortalIncentivesRequest();
            request.portalId = portalId;
            return reader.GetPortalIncentives(request);
        }

        public static bool AddPortalIncentive(int? id, int portalId, string name, int incentiveType, float points, string pointsText, bool isPoint, bool isCompanyIncentive, string imageUrl, bool isActive, int? programPortalId, int? refValue, int? refValue2, int? refValue3, string attachment, string moreInfo, string langname, string langMoreInfo, bool isGiftCard, bool removeSurcharge)
        {
            IncentiveReader reader = new IncentiveReader();
            PortalIncentiveDto request = new PortalIncentiveDto();
            if (id.HasValue)
                request.Id = id.Value;
            request.PortalId = portalId;
            request.Name = name;
            request.IncentiveTypeId = incentiveType;
            request.Points = points;
            request.PointsText = pointsText;
            request.IsPoint = isPoint;
            request.IsCompanyIncentive = isCompanyIncentive;
            request.ImageUrl = imageUrl;
            request.IsActive = isActive;
            if (programPortalId.HasValue)
                request.RefId = programPortalId.Value;
            if (refValue.HasValue)
                request.RefValue = refValue.Value;
            request.RefValue2 = refValue2;
            request.RefValue3 = refValue3;
            request.Attachment = attachment;
            request.MoreInfo = moreInfo;
            request.LanguageItemName = langname;
            request.LanguageItemMoreInfo = langMoreInfo;
            request.isGiftCard = isGiftCard;
            request.removeSurcharge = removeSurcharge;
            return reader.SavePortalIncentive(request);
        }

        public static SelectList GetIncentiveTypes()
        {
            return new SelectList(new IncentiveReader().GetIncentiveTypes(), "Id", "description");
        }

        public static IList<CustomIncentiveTypesDto> GetCustomIncentiveTypes(int? integrationWith)
        {
            IncentiveReader reader = new IncentiveReader();
            if (integrationWith.HasValue && integrationWith.Value == (int)Integrations.Activate)
                return reader.GetCustomIncentiveTypes();
            else
                return reader.GetCustomIncentiveTypes().Where(x => x.Id < 10 || x.Id > 13).ToList();
        }

        public static GetProgramsByPortalResponse GetProgramsByPortal(int portalId, int? type = null, bool? onlyActive = null)
        {
            ProgramReader reader = new ProgramReader();
            GetProgramsByPortalRequest request = new GetProgramsByPortalRequest();
            request.PortalId = portalId;
            request.ProgramType = type;
            request.onlyActive = onlyActive;
            return reader.GetProgramsByPortal(request);
        }

        public static void AddTobaccoIncentive(string fileName, int userId, int participantPortalId, int? programsInPortalId)
        {
            IncentiveReader reader = new IncentiveReader();
            SaveTobaccoAffidavitRequest request = new SaveTobaccoAffidavitRequest();
            request.userId = userId;
            request.portalId = participantPortalId;
            if (programsInPortalId.HasValue)
                request.programsInPortalId = programsInPortalId.Value;
            request.reference = fileName;
            reader.SaveTobaccoAffidavit(request);
        }

        public static PortalIncentiveModel ReadIncentive(int portalId, int participantId)
        {
            PortalIncentiveModel model = new PortalIncentiveModel();
            IncentiveReader reader = new IncentiveReader();
            model.PortalIncentives = reader.ReadPortalIncentives(portalId);
            if (model.PortalIncentives.Count > 0)
            {
                var userIncentives = reader.ReadUserIncentives(participantId);
                ParticipantReader partReader = new ParticipantReader();
                GetEligibilityforUserRequest eligrequest = new GetEligibilityforUserRequest();
                eligrequest.portalId = portalId;
                eligrequest.participantId = participantId;
                var eligResponse = partReader.GetELigibilityforUser(eligrequest);
                bool tobaccoFlag = false;
                if (eligResponse != null && eligResponse.eligibility != null && eligResponse.eligibility.TobaccoFlag.HasValue && eligResponse.eligibility.TobaccoFlag.Value)
                    tobaccoFlag = true;
                model.IsSmoker = tobaccoFlag && model.PortalIncentives.FirstOrDefault(pi => pi.IncentiveTypeId == (int)IncentiveTypes.Tobacco_Initiative) != null;
                ProgramReader programReader = new ProgramReader();
                var usersinProgram = programReader.GetUserProgramsByPortal(new GetUserProgramHistoryRequest { portalId = portalId, userId = participantId });
                if (usersinProgram != null && usersinProgram.Count > 0)
                {
                    model.programsinportalId = usersinProgram[0].ProgramsinPortalId;
                }
                foreach (var incentive in userIncentives)
                {
                    var portalIncentive = model.PortalIncentives.FirstOrDefault(pi => pi.Id == incentive.PortalIncentiveId);
                    if (portalIncentive != null)
                    {
                        if (portalIncentive.UserIncentives == null)
                            portalIncentive.UserIncentives = new List<UserIncentiveDto>();
                        portalIncentive.UserIncentives.Add(incentive);
                    }
                }
            }
            return model;
        }

        public static bool CopyKitsFromPreviousPortal(int portalId)
        {
            PortalReader reader = new PortalReader();
            reader.CopyKitsFromPreviousPortal(portalId);
            return true;
        }

        public static DeleteUserIncentiveResponse RemoveCustomIncentive(int UserIncentiveId, int adminId)
        {
            IncentiveReader reader = new IncentiveReader();
            DeleteUserIncentiveRequest request = new DeleteUserIncentiveRequest();
            request.userIncentiveId = UserIncentiveId;
            request.adminId = adminId;
            return reader.DeleteUserIncentive(request);
        }

        public static IList<CouponCodeDto> ListCoupons(int OrganizationId, bool onlyActive)
        {
            PortalReader reader = new PortalReader();
            ListCouponsRequest request = new ListCouponsRequest();
            request.OrganizationId = OrganizationId;
            request.onlyActive = onlyActive;
            return reader.ListCoupons(request).Coupons;
        }

        public static CouponCodeDto ReadCoupon(int couponid)
        {
            PortalReader reader = new PortalReader();
            return reader.ReadCoupon(couponid);
        }

        public static AddEditCouponResponse AddEditCoupons(int? id, int orgid, string name, string code, bool active)
        {
            PortalReader reader = new PortalReader();
            AddEditCouponRequest request = new AddEditCouponRequest();
            request.id = id.HasValue ? id.Value : 0;
            request.orgId = orgid;
            request.name = name;
            request.code = code;
            request.active = active;
            return reader.AddEditCoupon(request);
        }

        public static bool AddEditPortalEmails(int portalId, string types)
        {
            PortalReader reader = new PortalReader();
            NotificationEventTypePortalDto request = new NotificationEventTypePortalDto();
            request.types = types;
            request.PortalId = portalId;
            return reader.SavePortalEmails(request);
        }

        public static bool AddEditPortalKits(int portalId, string kits)
        {
            PortalReader reader = new PortalReader();
            AddEditKitsinPortal request = new AddEditKitsinPortal();
            request.kits = kits;
            request.PortalId = portalId;
            return reader.SavePortalKits(request);
        }

        public static ListNotificationEventTypePortalResponse GetPortalEmails(int portalId)
        {
            NotificationReader reader = new NotificationReader();
            GetNotificationEventTypePortalRequest request = new GetNotificationEventTypePortalRequest();
            request.PortalId = portalId;
            return reader.ListNotificationEventTypePortal(request);
        }

        public static List<KitsDto> ListKitsforPortal(int portalId)
        {
            KitReader reader = new KitReader();
            return reader.ListKitsforPortal(portalId);
        }

        public static bool AddEditPortalRafflles(int? id, int portalId, int raffleType, Dictionary<int, DateTime?> date, string reward, string name, string description, bool isActive, int pointType)
        {
            PortalReader reader = new PortalReader();
            AddEditPortalRafflesRequest request = new AddEditPortalRafflesRequest();
            RafflesinPortalsDto rafflesinPortals = new RafflesinPortalsDto();
            if (id.HasValue)
                rafflesinPortals.Id = id.Value;
            rafflesinPortals.PortalId = portalId;
            rafflesinPortals.RaffleTypeId = raffleType;
            rafflesinPortals.Date = date;
            rafflesinPortals.Reward = reward;
            rafflesinPortals.Name = name;
            rafflesinPortals.Description = description;
            rafflesinPortals.isActive = isActive;
            rafflesinPortals.Points = pointType;
            request.rafflesinPortals = rafflesinPortals;
            return reader.AddEditPortalRaffles(request);
        }

        public static GetPortalRafflesResponse GetPortalRaffles(int portalId)
        {
            PortalReader reader = new PortalReader();
            GetRafflesinPortalsRequest request = new GetRafflesinPortalsRequest();
            request.portalId = portalId;
            return reader.GetPortalRaffles(request);
        }

        public static RafflesinPortalsDto GetPortalRafflesById(int Id)
        {
            PortalReader reader = new PortalReader();
            GetRafflesinPortalsRequest request = new GetRafflesinPortalsRequest();
            request.Id = Id;
            return reader.GetPortalRafflesById(request);
        }

        public static bool AddForms(int portalId, string patientReleaseForm, string medicalClearanceForm, string knowYourNumbersForm, string testimonialForm, string tobaccoReleaseForm)
        {
            PortalReader reader = new PortalReader();
            AddEditPortalRequest request = new AddEditPortalRequest();
            request.Portal = new PortalDto();
            request.Portal.Id = portalId;
            request.Portal.PatientReleaseForm = patientReleaseForm;
            request.Portal.MedicalClearanceForm = medicalClearanceForm;
            request.Portal.KnowYourNumbersForm = knowYourNumbersForm;
            request.Portal.TestimonialForm = testimonialForm;
            request.Portal.TobaccoReleaseForm = tobaccoReleaseForm;
            return reader.AddEditForms(request);
        }

        public static GetKitsinPortalFollowUpsResponse ReadFollowUpKits(int portalFollowUpId)
        {
            PortalReader reader = new PortalReader();
            GetPortalFollowUpRequest request = new GetPortalFollowUpRequest();
            request.PortalId = portalFollowUpId;
            return reader.ReadFollowUpKits(request);
        }

        public static GetPortalFollowUpResponse GetPortalFollowUps(int portalId, int? programType, bool? bothFollowUps = false)
        {
            PortalReader reader = new PortalReader();
            GetPortalFollowUpRequest request = new GetPortalFollowUpRequest();
            request.PortalId = portalId;
            request.ProgramType = programType;
            request.bothFollowUps = bothFollowUps.Value;
            return reader.GetPortalFollowUps(request);
        }

        public static AddEditKittoFollowUpResponse AddKittoFollowUp(int kitId, int portalFollowUpId, short order)
        {
            PortalReader reader = new PortalReader();
            AddEditKittoFollowUpRequest request = new AddEditKittoFollowUpRequest();
            KitsinPortalFollowUpDto kitinFollowUp = new KitsinPortalFollowUpDto();
            kitinFollowUp.PortalFollowUpId = portalFollowUpId;
            kitinFollowUp.KitId = kitId;
            kitinFollowUp.Order = order;
            request.KitsinPortalFollowUp = kitinFollowUp;
            var response = reader.AddKittoFollowUp(request);
            return response;
        }
        public static DeleteKitfromFollowUpResponse DeleteKitfromFollowUp(int portalFollowUpId, int kitId)
        {
            PortalReader reader = new PortalReader();
            DeleteKitfromFollowUpRequest request = new DeleteKitfromFollowUpRequest();
            request.portalFollowUpId = portalFollowUpId;
            request.kitId = kitId;
            return reader.DeleteKitfromFollowUp(request);
        }

        public static IList<int> GetEligibilityImportLoadList()
        {
            PortalReader reader = new PortalReader();
            return reader.GetEligibilityImportLoadList();
        }

        public static List<CoachingConditionsDto> GetCoachingConditions()
        {
            PortalReader reader = new PortalReader();
            return reader.GetCoachingConditions();
        }

        public static List<LabProcedureDto> GetLabProcedures()
        {
            PortalReader reader = new PortalReader();
            return reader.GetLabProcedures();
        }

        public static int ClonePortal(int portalId)
        {
            PortalReader reader = new PortalReader();
            return reader.ClonePortal(portalId);
        }

        public static OrganizationDto GetOrganizationByCode(string code)
        {
            PortalReader reader = new PortalReader();
            return reader.GetOrganizationByCode(code);
        }
    }
}