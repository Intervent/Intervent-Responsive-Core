using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class PortalReader
    {
        private readonly InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public void DeactivatePortal(int systemAdminId)
        {
            var endDate = DateTime.UtcNow;
            var Portals = context.Portals.Where(x => x.Active && x.EndDate < endDate).ToList();

            if (Portals != null)
            {
                foreach (var Portal in Portals)
                {
                    Portal.Active = false;
                    context.Portals.Attach(Portal);
                    context.Entry(Portal).State = EntityState.Modified;
                    context.SaveChanges();
                    //Deactivate usersinprogram record
                    var UsersInActivePrograms = context.UsersinPrograms.Include("ProgramsinPortal").Where(x => x.IsActive && x.ProgramsinPortal.PortalId == Portal.Id).ToList();
                    if (UsersInActivePrograms != null)
                    {
                        foreach (var userData in UsersInActivePrograms)
                        {
                            userData.IsActive = false;
                            userData.InactiveDate = userData.UpdatedOn = DateTime.UtcNow;
                            userData.InactiveReason = (int)ProgramInactiveReasons.SystemAutoInactivation;
                            userData.UpdatedBy = systemAdminId;
                            context.UsersinPrograms.Attach(userData);
                            context.Entry(userData).State = EntityState.Modified;
                            context.SaveChanges();
                        }
                    }
                }
            }
        }

        public List<int> GetOrganizationToProcessClaims()
        {
            var response = context.Portals.Where(p => p.Active == true && (p.ProcessLivongoCodes == true || p.ProcessInterventCodes == true)).Select(x => x.OrganizationId).ToList();
            return response;
        }

        public OrganizationDto GetOrganizationByCode(string code)
        {
            var org = context.Organizations.Include("Portals").Where(p => p.Active == true && p.Code == code).FirstOrDefault();
            if (org != null)
            {
                return Utility.mapper.Map<DAL.Organization, OrganizationDto>(org);
            }
            return null;
        }

        public OrganizationDto GetOrganizationById(int id)
        {
            var org = context.Organizations.Include("Portals").Where(p => p.Active == true && p.Id == id).FirstOrDefault();
            if (org != null)
            {
                return Utility.mapper.Map<DAL.Organization, OrganizationDto>(org);
            }
            return null;
        }

        public OrganizationDto GetOrganizationDetails(int id)
        {
            var org = context.Organizations.Include("Organization1").Include("Organizations1").Include("Portals").Include("Portals.Languages").Include("UserRoles").Where(p => p.Id == id).FirstOrDefault();
            if (org != null)
            {
                return Utility.mapper.Map<DAL.Organization, OrganizationDto>(org);
            }
            return null;
        }

        public IList<OrganizationDto> GetIntuityOrgs()
        {
            var orgList = context.Organizations.Include("Portals")
               .Where(p => p.Portals.Any(y => y.Active == true) && p.IntegrationWith.HasValue && p.IntegrationWith.Value == (byte)IntegrationPartner.Intuity).ToList();

            return Utility.mapper.Map<IList<DAL.Organization>, IList<OrganizationDto>>(orgList);

        }

        public GetOrganizationDetailsResponse ListOrganizationsForSetup(ListOrganizationsRequest request)
        {
            GetOrganizationDetailsResponse response = new GetOrganizationDetailsResponse();
            IList<Organization> organizationList = context.Organizations.Include("Organization1").Include("Organizations1").Include("Portals").Include("Portals.Languages").Include("UserRoles").Where(p => string.IsNullOrEmpty(request.Search) || p.Name.Contains(request.Search)).ToList();

            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            response.ActiveOrganizationsCount = organizationList.Where(p => p.Portals.Any(y => y.Active == true)).Count();
            response.CoachingOrganizationsCount = organizationList.Where(p => p.Portals.Any(y => y.Active == true && y.HasCoachingProgram == true)).Count();
            response.SelfHelpOrganizationsCount = organizationList.Where(p => p.Portals.Any(y => y.Active == true && y.HasSelfHelpProgram == true)).Count();
            response.InActiveOrganizationsCount = organizationList.Where(p => p.Portals == null || p.Portals.All(y => y.Active == false)).Count();

            if (request.FilterBy == 1)
            {
                totalRecords = response.ActiveOrganizationsCount;
                organizationList = organizationList.Where(p => p.Portals.Any(y => y.Active == true)).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : short.MaxValue).ToList();
            }
            else if (request.FilterBy == 2)
            {
                totalRecords = response.CoachingOrganizationsCount;
                organizationList = organizationList.Where(p => p.Portals.Any(y => y.Active == true && y.HasCoachingProgram == true)).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : short.MaxValue).ToList();
            }
            else if (request.FilterBy == 3)
            {
                totalRecords = response.SelfHelpOrganizationsCount;
                organizationList = organizationList.Where(p => p.Portals.Any(y => y.Active == true && y.HasSelfHelpProgram == true)).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : short.MaxValue).ToList();
            }
            else if (request.FilterBy == 4)
            {
                totalRecords = response.InActiveOrganizationsCount;
                organizationList = organizationList.Where(p => p.Portals == null || p.Portals.All(y => y.Active == false)).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : short.MaxValue).ToList();
            }
            else
            {
                totalRecords = organizationList.Count();
                organizationList = organizationList.OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : short.MaxValue).ToList();
            }

            response.Organizations = Utility.mapper.Map<IList<DAL.Organization>, IList<OrganizationDto>>(organizationList);
            response.totalRecords = totalRecords;
            return response;
        }

        public ListOrganizationsResponse ListOrganizations(ListOrganizationsRequest request)
        {
            ListOrganizationsResponse response = new ListOrganizationsResponse();
            IList<Organization> applications;

            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (request.PageSize > 0 && totalRecords == 0)
            {
                if (request.includeParentOrganization)
                    totalRecords = context.Organizations.Where(o => (o.ParentOrganizationId == request.ParentOrganizationId.Value && o.Active == true) || (o.Id == request.ParentOrganizationId.Value && o.Active == true)).Count();
                else if (request.ParentOrganizationId.HasValue)
                    totalRecords = context.Organizations.Where(o => o.ParentOrganizationId == request.ParentOrganizationId.Value && o.Active == true).Count();
                else if (request.removechildOrganizations)
                    totalRecords = context.Organizations.Where(o => o.Organization1 == null && o.Active == true).Count();
                else
                    totalRecords = context.Organizations.Where(o => o.Active == true).Count();
            }
            if (request.includeParentOrganization)
                applications = context.Organizations.Include("Portals").Include("CouponCodes").Where(o => (o.ParentOrganizationId == request.ParentOrganizationId.Value && o.Active == true) || (o.Id == request.ParentOrganizationId.Value && o.Active == true)).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : Int16.MaxValue).ToList();
            else if (request.ParentOrganizationId.HasValue)
                applications = context.Organizations.Include("Portals").Where(o => o.ParentOrganizationId == request.ParentOrganizationId.Value && o.Active == true).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : Int16.MaxValue).ToList();
            else if (request.removechildOrganizations)
                applications = context.Organizations.Include("Organization1").Include("Organizations1").Include("Portals").Where(o => o.Organization1 == null && o.Active == true).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : Int16.MaxValue).ToList();
            else
                applications = context.Organizations.Include("Organization1").Include("Organizations1").Include("Portals").Include("Portals.Languages").Include("UserRoles").Where(o => o.Active == true).OrderBy(x => x.Name).Skip(request.Page * request.PageSize).Take(request.PageSize > 0 ? request.PageSize : Int16.MaxValue).ToList();

            response.Organizations = Utility.mapper.Map<IList<DAL.Organization>, IList<OrganizationDto>>(applications);
            response.totalRecords = totalRecords;
            return response;
        }

        public ListOrganizationsResponse GetParentOrganizationsDetails()
        {
            ListOrganizationsResponse response = new ListOrganizationsResponse();
            var activeOrganizations = context.Organizations.Include("Portals").Where(x => x.Active == true && x.ParentOrganizationId == null).ToList();
            response.Organizations = Utility.mapper.Map<IList<DAL.Organization>, IList<OrganizationDto>>(activeOrganizations);
            return response;
        }

        public Dictionary<string, int> GetActiveChildOrgNodesWithActivePortal(int parentOrgId)
        {
            Dictionary<string, int> orgList = new Dictionary<string, int>();
            var childOrgs = context.Organizations.Include("Portals").Where(p => p.ParentOrganizationId == parentOrgId && p.Active == true).OrderBy(x => x.Name).ToList();
            foreach (var org in childOrgs)
            {
                if (!string.IsNullOrEmpty(org.Code))
                {
                    var activePortal = org.Portals.Where(p => p.Active).FirstOrDefault();
                    if (activePortal != null)
                    {
                        orgList[org.Code] = activePortal.Id;
                    }
                }
            }
            return orgList;
        }

        public Dictionary<string, int> GetOrganizationsWithCodes()
        {
            Dictionary<string, int> orgList = new Dictionary<string, int>();
            var orgs = context.Organizations.Include("Portals").Where(p => !String.IsNullOrEmpty(p.Code)).OrderBy(x => x.Name).ToList();
            foreach (var org in orgs)
            {
                orgList[org.Code] = org.Id;
            }
            return orgList;
        }

        public ListOrganizationsResponse GetFilteredOrganizationsList(int userId)
        {
            ListOrganizationsResponse response = new ListOrganizationsResponse();
            //TODO: Review Performance 
            var user = context.Users.Include("UserRoles").Where(x => x.Id == userId).FirstOrDefault();
            var userDto = Utility.mapper.Map<DAL.User, UserDto>(user);
            var userRole = userDto.Roles.Select(x => x.Id).ToList();
            var organizations = context.Organizations.Include("Portals").Where(x => x.UserRoles.Any(y => userRole.Contains(y.Id))).ToList();
            response.Organizations = Utility.mapper.Map<IList<DAL.Organization>, IList<OrganizationDto>>(organizations);
            return response;
        }

        public ListPortalsResponse GetPortalList(int?[] organizationsList)
        {
            ListPortalsResponse response = new ListPortalsResponse();
            if (organizationsList != null)
            {
                var portals = context.Portals.Where(x => organizationsList.Contains(x.OrganizationId)).ToList();
                response.portals = Utility.mapper.Map<IList<DAL.Portal>, IList<PortalDto>>(portals);
            }
            return response;
        }

        public ReadOrganizationResponse ReadOrganization(ReadOrganizationRequest request)
        {
            ReadOrganizationResponse response = new ReadOrganizationResponse();
            DAL.Organization organization = null;
            if (request.orgId.HasValue)
                organization = context.Organizations.Include("Organization1").Include("Portals").Include("UserRoles").Where(x => x.Id == request.orgId).FirstOrDefault();
            else
                organization = context.Organizations.Include("Organization1").Include("Portals")
                .Where(x => x.Name == request.Name && x.Portals.Any(y => y.Active == true)).FirstOrDefault();

            response.organization = Utility.mapper.Map<DAL.Organization, OrganizationDto>(organization);
            return response;
        }

        public AddEditOrganizationResponse AddEditOrganization(AddEditOrganizationRequest request)
        {
            AddEditOrganizationResponse response = new AddEditOrganizationResponse();

            Organization organization = new Organization();
            if (request.Organization.Id.HasValue && request.Organization.Id.Value > 0)
            {
                organization = context.Organizations.Include("UserRoles").Where(x => x.Id == request.Organization.Id.Value).FirstOrDefault();
                organization.UserRoles.Clear();
            }
            organization.Description = request.Organization.Description;
            organization.Name = request.Organization.Name;
            organization.ParentOrganizationId = request.Organization.ParentOrganizationId;
            organization.Url = request.Organization.Url;
            organization.ContactNumber = request.Organization.ContactNumber;
            organization.ContactEmail = request.Organization.ContactEmail;
            organization.Active = request.Organization.Active;
            organization.SSO = request.Organization.SSO;
            organization.TermsForSSO = request.Organization.TermsForSSO;
            organization.OwnCoach = request.Organization.OwnCoach;
            organization.IntegrationWith = request.Organization.IntegrationWith;
            organization.EmailValidationRequired = request.Organization.EmailValidationRequired;

            if (!string.IsNullOrEmpty(request.groups))
            {
                List<string> groupList = request.groups.Split('-').ToList<string>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    var groupId = Convert.ToInt32(groupList[i]);
                    var role = context.UserRoles.Where(x => x.Id == groupId).FirstOrDefault();
                    organization.UserRoles.Add(role);
                }
            }

            if (request.Organization.Id.HasValue && request.Organization.Id.Value > 0)
            {
                context.Organizations.Attach(organization);
                context.Entry(organization).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                context.Organizations.Add(organization);
                context.SaveChanges();
                request.Organization.Id = organization.Id;
            }
            response.Organization = request.Organization;
            response.success = true;
            return response;
        }

        public bool DeleteOrganization(int Id)
        {
            var organization = context.Organizations.Where(x => x.Id == Id).FirstOrDefault();
            if (organization != null)
            {
                organization.Active = false;
                context.Organizations.Attach(organization);
                context.Entry(organization).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public ListPortalsResponse ListPortals(ListPortalsRequest request)
        {
            ListPortalsResponse response = new ListPortalsResponse();

            var Organization = context.Organizations.Include("Portals").Where(x => x.Id == request.organizationId).FirstOrDefault();
            if (Organization != null)
            {
                var portals = Organization.Portals.Where(x => (!request.onlyActive.HasValue || x.Active == request.onlyActive.Value)).ToList();
                response.portals = Utility.mapper.Map<IList<DAL.Portal>, IList<PortalDto>>(portals);
                response.OrgName = Organization.Name;
                response.OrgCode = Organization.Code;
            }
            return response;
        }

        public GetCurrentPortalIdResponse CurrentPortalIdForOrganization(ListPortalsRequest request)
        {
            GetCurrentPortalIdResponse response = new GetCurrentPortalIdResponse();
            var portal = context.Portals.Where(x => x.OrganizationId == request.organizationId && x.Active == true).FirstOrDefault();
            if (portal != null)
            {
                response.PortalId = portal.Id;
                response.ProcessLivongoCodes = portal.ProcessLivongoCodes;
                response.ProcessInterventCodes = portal.ProcessInterventCodes;
                response.ClaimsDirectoryPath = portal.ClaimsDirectoryPath;
                response.StartDate = portal.StartDate;
                response.EndDate = portal.EndDate;
                response.CampaignStartDate = portal.CampaignStartDate;
                response.CampaignEndDate = portal.CampaignEndDate;
                response.EligtoIntuity = portal.EligtoIntuity;
            }
            return response;
        }

        public string GetPortalByName(int id)
        {
            var portal = context.Portals.Where(x => x.Id == id).FirstOrDefault();

            if (portal != null)
            {
                return portal.Name;
            }
            return null;
        }

        public ReadPortalResponse ReadPortal(ReadPortalRequest request)
        {
            ReadPortalResponse response = new ReadPortalResponse();

            var portal = context.Portals.Include("PortalFollowUps").Include("Organization").Include("ProgramsinPortals").Include("Languages").Include("Specializations").Include("ProgramsinPortals.Program").Include("ProgramsinPortals.ApptCallTemplate").Include("PortalCoachingConditions").Include("PortalLabProcedures").Include("PortalIncentives").Where(p => p.Id == request.portalId).FirstOrDefault();
            response.portal = Utility.mapper.Map<DAL.Portal, PortalDto>(portal);

            return response;
        }

        public ReadParticipantInactivePortalResponse ReadParticipantInactivePortal(ReadParticipantInactivePortalRequest request)
        {
            ReadParticipantInactivePortalResponse response = new ReadParticipantInactivePortalResponse();
            var user = context.Users.Include("Organization.Portals").Include("Organization.Portals.PortalIncentives").Include("HRAs").Include("UsersinPrograms").Include("UsersinPrograms.FollowUps").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Where(x => x.Id == request.participantId).FirstOrDefault();
            var portal = user.Organization.Portals.OrderByDescending(x => x.EndDate).FirstOrDefault();
            response.portal = Utility.mapper.Map<Portal, PortalDto>(portal);
            response.hra = Utility.mapper.Map<HRA, HRADto>(user.HRAs.OrderByDescending(x => x.StartDate).FirstOrDefault());
            response.usersinProgram = Utility.mapper.Map<UsersinProgram, UsersinProgramDto>(user.UsersinPrograms.OrderByDescending(x => x.Id).FirstOrDefault());
            return response;
        }

        public bool DeletePortal(int Id)
        {
            var portal = context.Portals.Where(x => x.Id == Id).FirstOrDefault();
            if (portal != null)
            {
                portal.Active = false;
                context.Portals.Attach(portal);
                context.Entry(portal).State = EntityState.Modified;
                context.SaveChanges();
            }
            return true;
        }

        public AddEditPortalResponse AddPortal(AddEditPortalRequest request)
        {
            AddEditPortalResponse response = new AddEditPortalResponse();

            Portal portal = new Portal();
            if (request.Portal.Id > 0)
                portal = context.Portals.Where(x => x.Id == request.Portal.Id).FirstOrDefault();
            else
                portal.HAPageSeq = "MC.OR.YL.EC.IN.YN.";
            portal.Name = request.Portal.Name;
            portal.Active = request.Portal.Active;
            DateTime output;
            if (DateTime.TryParse(request.Portal.StartDate, out output))
                portal.StartDate = Convert.ToDateTime(request.Portal.StartDate);
            if (DateTime.TryParse(request.Portal.EndDate, out output))
                portal.EndDate = Convert.ToDateTime(request.Portal.EndDate);
            if (request.Portal.Id > 0)
            {
                context.Portals.Attach(portal);
                context.Entry(portal).State = EntityState.Modified;
                context.SaveChanges();
            }
            else
            {
                portal.OrganizationId = request.Portal.OrganizationId;
                context.Portals.Add(portal);
                context.SaveChanges();
                request.Portal.Id = portal.Id;
            }
            response.Portal = request.Portal;
            response.success = true;
            return response;
        }

        public AddEditPortalResponse EditPortal(AddEditPortalRequest request)
        {
            AddEditPortalResponse response = new AddEditPortalResponse();

            Portal portal = new Portal();
            DateTime output;
            if (request.Portal.Id > 0)
                portal = context.Portals.Include("Languages").Include("Specializations").Where(x => x.Id == request.Portal.Id).FirstOrDefault();
            switch (request.UpdateFor)
            {
                case 1:
                    portal.Name = request.Portal.Name;
                    portal.Active = request.Portal.Active;
                    if (DateTime.TryParse(request.Portal.StartDate, out output))
                        portal.StartDate = Convert.ToDateTime(request.Portal.StartDate);
                    if (DateTime.TryParse(request.Portal.EndDate, out output))
                        portal.EndDate = Convert.ToDateTime(request.Portal.EndDate);

                    foreach (var lang in portal.Languages.ToList())
                        portal.Languages.Remove(lang);

                    foreach (var splz in portal.Specializations.ToList())
                        portal.Specializations.Remove(splz);

                    if (!string.IsNullOrEmpty(request.Portal.languagesStr))
                    {
                        List<string> languageList = request.Portal.languagesStr.Split('-').ToList<string>();
                        for (int i = 0; i < languageList.Count; i++)
                        {
                            var langId = Convert.ToInt32(languageList[i]);
                            if (!portal.Languages.Where(x => x.Id == langId).Any())
                            {
                                var language = context.Languages.Where(x => x.Id == langId)
                                    .FirstOrDefault();
                                portal.Languages.Add(language);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(request.Portal.specializationsStr))
                    {
                        List<string> specializationList = request.Portal.specializationsStr.Split('-').ToList<string>();
                        for (int i = 0; i < specializationList.Count; i++)
                        {
                            var langId = Convert.ToInt32(specializationList[i]);
                            if (!portal.Specializations.Where(x => x.Id == langId).Any())
                            {
                                var specialization = context.Specializations.Where(x => x.Id == langId).FirstOrDefault();
                                portal.Specializations.Add(specialization);
                            }
                        }
                    }
                    break;
                case 2:
                    portal.HasHRA = request.Portal.HasHRA;
                    if (request.Portal.HasHRA.HasValue && request.Portal.HasHRA == 2)
                    {
                        portal.HRAforCoachingProgram = false;
                        portal.HRAforSelfHelpProgram = false;
                        portal.FollowUpforCoaching = false;
                        portal.FollowUpforSelfHelp = false;
                        portal.IncompleteHRA = null;
                        portal.HRAOnly = null;
                        portal.HRAandBiometrics = null;
                        portal.FirstCoaching = null;
                        portal.CompletedCoaching = null;
                        portal.FirsttimeHRA = null;
                        portal.LabCorpAttachment = null;
                        portal.DoctorOfficeAttachment = null;
                        portal.ReportText = null;
                    }
                    else if (request.Portal.HasHRA.HasValue && request.Portal.HasHRA == 3)
                    {
                        portal.HRAforCoachingProgram = false;
                        portal.HRAforSelfHelpProgram = false;
                    }
                    portal.HRAVer = request.Portal.HRAVer;
                    portal.AssessmentName = request.Portal.AssessmentName;
                    portal.HRAValidity = request.Portal.HRAValidity;
                    portal.AWV = request.Portal.AWV;
                    portal.CACScanQuestion = request.Portal.CACScanQuestion;
                    portal.GINAQuestion = request.Portal.GINAQuestion;
                    portal.ClientNameInReport = request.Portal.ClientNameInReport;
                    portal.AutoImmune = request.Portal.AutoImmune;
                    portal.MailScoreCard = request.Portal.MailScoreCard;
                    portal.CarePlan = request.Portal.CarePlan;
                    portal.CarePlanType = request.Portal.CarePlanType;
                    portal.LabIntegration = request.Portal.LabIntegration;
                    portal.usePreviousLabs = request.Portal.usePreviousLabs;
                    portal.ShowPostmenopausal = request.Portal.ShowPostmenopausal;
                    portal.SelfEnterLabs = request.Portal.SelfEnterLabs;
                    portal.WelcomeText = request.Portal.WelcomeText;
                    if (string.IsNullOrEmpty(request.Portal.ValidLabs))
                        portal.ValidLabs = null;
                    else if (DateTime.TryParse(request.Portal.ValidLabs, out output))
                        portal.ValidLabs = Convert.ToDateTime(request.Portal.ValidLabs);
                    if (!portal.CarePlan)
                    {
                        portal.NeedCareplanApproval = false;
                        portal.CareplanPath = null;
                    }
                    foreach (var portalLabProcedure in context.PortalLabProcedures.Where(x => x.PortalId == portal.Id).ToList())
                    {
                        context.PortalLabProcedures.Remove(portalLabProcedure);
                        context.SaveChanges();
                    }
                    if (request.Portal.LabIntegration && request.Portal.LabProceduresStr != null && request.Portal.LabProceduresStr.Count() > 0)
                    {
                        List<string> LabProceduresList = request.Portal.LabProceduresStr.Split('-').ToList<string>();
                        for (int i = 0; i < LabProceduresList.Count; i++)
                        {
                            PortalLabProcedure labProceduresListTool = new PortalLabProcedure();
                            labProceduresListTool.PortalId = request.Portal.Id;
                            labProceduresListTool.LabProcedureId = Convert.ToInt32(LabProceduresList[i]);
                            context.PortalLabProcedures.Add(labProceduresListTool);
                            context.SaveChanges();
                        }
                    }
                    break;
                case 3:
                    portal.HasCoachingProgram = request.Portal.HasCoachingProgram;
                    if (request.Portal.HasCoachingProgram == false)
                        portal.FollowUpforCoaching = false;
                    portal.HasSelfHelpProgram = request.Portal.HasSelfHelpProgram;
                    if (request.Portal.HasSelfHelpProgram == false)
                        portal.FollowUpforSelfHelp = false;
                    portal.HRAforCoachingProgram = request.Portal.HRAforCoachingProgram;
                    portal.HRAforSelfHelpProgram = request.Portal.HRAforSelfHelpProgram;
                    portal.ShowPricing = request.Portal.ShowPricing;
                    portal.ShowProgramOption = request.Portal.ShowProgramOption;
                    portal.SelfScheduling = request.Portal.SelfScheduling;
                    portal.AssignPrograms = request.Portal.AssignPrograms;
                    foreach (var coachingConditions in context.PortalCoachingConditions.Where(x => x.PortalId == portal.Id).ToList())
                    {
                        context.PortalCoachingConditions.Remove(coachingConditions);
                        context.SaveChanges();
                    }
                    if (request.Portal.HasCoachingProgram && request.Portal.CoachingConditionsStr != null && request.Portal.CoachingConditionsStr.Count() > 0)
                    {
                        List<string> CoachingConditionsList = request.Portal.CoachingConditionsStr.Split('-').ToList<string>();
                        for (int i = 0; i < CoachingConditionsList.Count; i++)
                        {
                            PortalCoachingConditions coachingConditionsListTool = new PortalCoachingConditions();
                            coachingConditionsListTool.PortalId = request.Portal.Id;
                            coachingConditionsListTool.CoachCondId = Convert.ToInt32(CoachingConditionsList[i]);
                            context.PortalCoachingConditions.Add(coachingConditionsListTool);
                            context.SaveChanges();
                        }
                    }
                    break;
                case 4:
                    portal.FollowUpforCoaching = request.Portal.FollowUpforCoaching;
                    portal.FollowUpforSelfHelp = request.Portal.FollowUpforSelfHelp;
                    portal.FollowUpValidity = request.Portal.FollowUpValidity;
                    List<string> coachingFollowUpList = null, selfhelpFollowUpList = null;
                    var portalFollowUps = context.PortalFollowUps.Where(x => x.PortalId == portal.Id).ToList();
                    if (!string.IsNullOrEmpty(request.Portal.followupTypeStr))
                    {
                        coachingFollowUpList = request.Portal.followupTypeStr.Split('-').ToList<string>();
                        List<string> followUpLabs = null;
                        if (!string.IsNullOrEmpty(request.Portal.followuplabStr))
                        {
                            followUpLabs = request.Portal.followuplabStr.Split('-').ToList<string>();
                        }
                        for (int i = 0; i < coachingFollowUpList.Count; i++)
                        {
                            var ftypeId = Convert.ToInt32(coachingFollowUpList[i]);
                            if (portalFollowUps != null && portalFollowUps.Where(x => x.FollowupTypeId == ftypeId && x.ProgramType == (int)ProgramTypes.Coaching).FirstOrDefault() != null)
                            {
                                var portalFollowUp = context.PortalFollowUps.Where(x => x.PortalId == portal.Id && x.FollowupTypeId == ftypeId && x.ProgramType == (int)ProgramTypes.Coaching).FirstOrDefault();
                                if (portalFollowUp != null)
                                {
                                    portalFollowUp.LabIntegration = followUpLabs != null && followUpLabs.Contains(coachingFollowUpList[i]) ? true : false;
                                    portalFollowUp.ProgramType = (int)ProgramTypes.Coaching;
                                    context.PortalFollowUps.Attach(portalFollowUp);
                                    context.Entry(portalFollowUp).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                DAL.PortalFollowUp portalFollowUp = new DAL.PortalFollowUp();
                                portalFollowUp.FollowupTypeId = ftypeId;
                                portalFollowUp.ProgramType = (int)ProgramTypes.Coaching;
                                portalFollowUp.PortalId = portal.Id;
                                portalFollowUp.LabIntegration = followUpLabs != null && followUpLabs.Contains(coachingFollowUpList[i]) ? true : false;
                                context.PortalFollowUps.Add(portalFollowUp);
                                context.SaveChanges();

                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(request.Portal.selfhelpFollowupTypeStr))
                    {
                        selfhelpFollowUpList = request.Portal.selfhelpFollowupTypeStr.Split('-').ToList<string>();
                        List<string> followUpLabs = null;
                        if (!string.IsNullOrEmpty(request.Portal.selfhelpfollowuplabStr))
                        {
                            followUpLabs = request.Portal.selfhelpfollowuplabStr.Split('-').ToList<string>();
                        }
                        for (int i = 0; i < selfhelpFollowUpList.Count; i++)
                        {
                            var ftypeId = Convert.ToInt32(selfhelpFollowUpList[i]);
                            if (portalFollowUps != null && portalFollowUps.Where(x => x.FollowupTypeId == ftypeId && x.ProgramType == (int)ProgramTypes.SelfHelp).FirstOrDefault() != null)
                            {
                                var portalFollowUp = context.PortalFollowUps.Where(x => x.PortalId == portal.Id && x.FollowupTypeId == ftypeId && x.ProgramType == (int)ProgramTypes.SelfHelp).FirstOrDefault();
                                if (portalFollowUp != null)
                                {
                                    portalFollowUp.LabIntegration = followUpLabs != null && followUpLabs.Contains(selfhelpFollowUpList[i]) ? true : false;
                                    portalFollowUp.ProgramType = (int)ProgramTypes.SelfHelp;
                                    context.PortalFollowUps.Attach(portalFollowUp);
                                    context.Entry(portalFollowUp).State = EntityState.Modified;
                                    context.SaveChanges();
                                }
                            }
                            else
                            {
                                DAL.PortalFollowUp portalFollowUp = new DAL.PortalFollowUp();
                                portalFollowUp.FollowupTypeId = ftypeId;
                                portalFollowUp.ProgramType = (int)ProgramTypes.SelfHelp;
                                portalFollowUp.PortalId = portal.Id;
                                portalFollowUp.LabIntegration = followUpLabs != null && followUpLabs.Contains(selfhelpFollowUpList[i]) ? true : false;
                                context.PortalFollowUps.Add(portalFollowUp);
                                context.SaveChanges();

                            }
                        }
                    }
                    portalFollowUps = portalFollowUps.Where(x => (selfhelpFollowUpList != null && !selfhelpFollowUpList.Contains(x.FollowupTypeId.ToString()) && x.ProgramType == (int)ProgramTypes.SelfHelp) || (coachingFollowUpList != null && !coachingFollowUpList.Contains(x.FollowupTypeId.ToString()) && x.ProgramType == (int)ProgramTypes.Coaching)).ToList();

                    if (portalFollowUps != null && portalFollowUps.Count() > 0)
                    {
                        foreach (var portalFollowUp in portalFollowUps.ToList())
                        {
                            var kitsinPortalFollowUps = context.KitsinPortalFollowUps.Where(x => x.PortalFollowUpId == portalFollowUp.Id).ToList();
                            if (kitsinPortalFollowUps != null && kitsinPortalFollowUps.Count() > 0)
                            {
                                foreach (var kitsinPortalFollowUp in kitsinPortalFollowUps)
                                {
                                    context.KitsinPortalFollowUps.Remove(kitsinPortalFollowUp);
                                    context.SaveChanges();
                                }
                            }
                            var followUp = context.PortalFollowUps.Where(x => x.Id == portalFollowUp.Id).FirstOrDefault();
                            if (followUp != null)
                            {
                                followUp.KitsinPortalFollowUps = null;
                                context.PortalFollowUps.Remove(followUp);
                                context.SaveChanges();
                            }
                        }
                    }
                    break;
                case 5:
                    portal.IncompleteHRA = request.Portal.IncompleteHRA;
                    portal.HRAOnly = request.Portal.HRAOnly;
                    portal.HRAandBiometrics = request.Portal.HRAandBiometrics;
                    portal.FirstCoaching = request.Portal.FirstCoaching;
                    portal.CompletedCoaching = request.Portal.CompletedCoaching;
                    portal.FirsttimeHRA = request.Portal.FirstTimeHRA;
                    portal.ReportText = request.Portal.ReportText;
                    portal.ProgramText = request.Portal.ProgramText;
                    portal.ContactText = request.Portal.ContactText;
                    break;
                case 6:
                    portal.ProcessLivongoCodes = request.Portal.ProcessLivongoCodes;
                    portal.ProcessInterventCodes = request.Portal.ProcessInterventCodes;
                    portal.ClaimsDirectoryPath = request.Portal.ClaimsDirectoryPath;
                    portal.EligibilityImportLoadFlag = request.Portal.EligibilityImportLoadFlag;
                    portal.EligibilityFolderPath = request.Portal.EligibilityFolderPath;
                    portal.EligibilityTerminateFlag = request.Portal.EligibilityTerminateFlag;
                    portal.EligibilityFormat = request.Portal.EligibilityFormat;
                    portal.EligtoIntuity = request.Portal.EligtoIntuity;
                    break;
                case 7:
                    portal.CampaignCalls = request.Portal.CampaignCalls;
                    portal.AppointmentCalls = request.Portal.AppointmentCalls;
                    portal.TrackingCalls = request.Portal.TrackingCalls;
                    portal.AllowCardiacQuestion = request.Portal.AllowCardiacQuestion;
                    portal.NoOfTrackingCalls = Convert.ToByte(request.Portal.NoOfTrackingCalls);
                    portal.NoProgDays = Convert.ToByte(request.Portal.NoProgDays);
                    portal.LabCorpAttachment = request.Portal.LabCorpAttachment;
                    portal.DoctorOfficeAttachment = request.Portal.DoctorOfficeAttachment;
                    if (DateTime.TryParse(request.Portal.CampaignStartDate, out output))
                        portal.CampaignStartDate = Convert.ToDateTime(request.Portal.CampaignStartDate);
                    if (DateTime.TryParse(request.Portal.CampaignEndDate, out output))
                        portal.CampaignEndDate = Convert.ToDateTime(request.Portal.CampaignEndDate);
                    portal.NeedCareplanApproval = request.Portal.NeedCareplanApproval;
                    portal.CareplanPath = request.Portal.CareplanPath;
                    portal.ShowTimeTracker = request.Portal.ShowTimeTracker;
                    portal.ProviderDetails = request.Portal.ProviderDetails;
                    portal.FaxReports = request.Portal.FaxReports;
                    portal.KitAlert = request.Portal.KitAlert;
                    portal.EmployerIncentiveText = request.Portal.EmployerIncentiveText;
                    break;
            }
            context.Portals.Attach(portal);
            context.Entry(portal).State = EntityState.Modified;
            context.SaveChanges();
            response.Portal = request.Portal;
            response.success = true;
            return response;
        }

        public ListProgramsinPortalsResponse ListProgramsinPortals(ListProgramsinPortalsRequest request)
        {
            ListProgramsinPortalsResponse response = new ListProgramsinPortalsResponse();

            var ProgramsinPortals = context.ProgramsinPortals.Include("ApptCallTemplate").Include("Portal").Include("Program").Where(x => x.PortalId == request.portalId).ToList();

            response.ProgramsinPortals = Utility.mapper.Map<IList<DAL.ProgramsinPortal>, IList<ProgramsinPortalDto>>(ProgramsinPortals);
            return response;
        }

        public ReadPrograminPortalResponse ReadPrograminPortal(ReadPrograminPortalRequest request)
        {
            ReadPrograminPortalResponse response = new ReadPrograminPortalResponse();
            var PrograminPortal = context.ProgramsinPortals.Include("Portal").Include("Program").Where(x => x.Id == request.PrograminPortalId).FirstOrDefault();
            if (request.language != ListOptions.DefaultLanguage)
            {
                LanguageReader langReader = new LanguageReader();
                var codes = new List<string>();
                if (!string.IsNullOrEmpty(PrograminPortal.DescforUserLanguageItem))
                    codes.Add(PrograminPortal.DescforUserLanguageItem);
                if (!string.IsNullOrEmpty(PrograminPortal.NameforUserLanguageItem))
                    codes.Add(PrograminPortal.NameforUserLanguageItem);
                var langItems = langReader.GetLanguageItems(codes, request.language);
                if (langItems != null && langItems.Count > 0)
                {
                    var item = langItems.Find(l => l.ItemCode == PrograminPortal.DescforUserLanguageItem);
                    if (item != null)
                        PrograminPortal.DescriptionforUser = item.Text;
                    else
                        PrograminPortal.DescriptionforUser = string.Empty;
                    item = langItems.Find(l => l.ItemCode == PrograminPortal.NameforUserLanguageItem);
                    if (item != null)
                        PrograminPortal.NameforUser = item.Text;
                    else
                        PrograminPortal.NameforUser = string.Empty;
                }
                else
                {
                    PrograminPortal.DescriptionforUser = string.Empty;
                    PrograminPortal.NameforUser = string.Empty;
                }
            }
            response.programinPortal = Utility.mapper.Map<DAL.ProgramsinPortal, ProgramsinPortalDto>(PrograminPortal);
            return response;
        }

        public AddEditProgramsinPortalResponse AddEditProgramsinPortal(AddEditProgramsinPortalRequest request)
        {
            AddEditProgramsinPortalResponse response = new AddEditProgramsinPortalResponse();
            if (request.ProgramsinPortal.Id > 0)
            {
                var ProgramsinPortal = context.ProgramsinPortals.Where(x => x.Id == request.ProgramsinPortal.Id).FirstOrDefault();
                if (!string.IsNullOrEmpty(request.language) && request.language != ListOptions.DefaultLanguage)
                {
                    if (string.IsNullOrEmpty(ProgramsinPortal.NameforUserLanguageItem))
                        ProgramsinPortal.NameforUserLanguageItem = LanguageType.PRGNAME.ToString() + ProgramsinPortal.Id;
                    if (string.IsNullOrEmpty(ProgramsinPortal.DescforUserLanguageItem))
                        ProgramsinPortal.DescforUserLanguageItem = LanguageType.PRGDESC.ToString() + ProgramsinPortal.Id;
                    Dictionary<string, string> codes = new Dictionary<string, string>();
                    codes.Add(ProgramsinPortal.NameforUserLanguageItem, request.ProgramsinPortal.NameforUser);
                    codes.Add(ProgramsinPortal.DescforUserLanguageItem, request.ProgramsinPortal.DescriptionforUser);
                    LanguageReader reader = new LanguageReader();
                    reader.SaveLanguageItems(codes, request.language);
                }
                else
                {
                    ProgramsinPortal.NameforUser = request.ProgramsinPortal.NameforUser;
                    ProgramsinPortal.DescriptionforUser = request.ProgramsinPortal.DescriptionforUser;
                }
                ProgramsinPortal.PortalId = request.ProgramsinPortal.PortalId;
                ProgramsinPortal.ProgramId = request.ProgramsinPortal.ProgramId;
                ProgramsinPortal.Cost = request.ProgramsinPortal.Cost;
                ProgramsinPortal.ApptCallTemplateId = request.ProgramsinPortal.ApptCallTemplateId;
                ProgramsinPortal.MoreInfo = request.ProgramsinPortal.MoreInfo;
                ProgramsinPortal.Active = request.ProgramsinPortal.Active;
                ProgramsinPortal.NameforAdmin = request.ProgramsinPortal.NameforAdmin;
                ProgramsinPortal.DescriptionforAdmin = request.ProgramsinPortal.DescriptionforAdmin;
                ProgramsinPortal.SortOrder = request.ProgramsinPortal.SortOrder;
                context.ProgramsinPortals.Attach(ProgramsinPortal);
                context.Entry(ProgramsinPortal).State = EntityState.Modified;
            }
            else
            {
                DAL.ProgramsinPortal newProgramsinPortal = new DAL.ProgramsinPortal();
                newProgramsinPortal.PortalId = request.ProgramsinPortal.PortalId;
                newProgramsinPortal.ProgramId = request.ProgramsinPortal.ProgramId;
                newProgramsinPortal.Cost = request.ProgramsinPortal.Cost;
                newProgramsinPortal.MoreInfo = request.ProgramsinPortal.MoreInfo;
                newProgramsinPortal.ApptCallTemplateId = request.ProgramsinPortal.ApptCallTemplateId;
                newProgramsinPortal.Active = request.ProgramsinPortal.Active;
                newProgramsinPortal.NameforUser = request.ProgramsinPortal.NameforUser;
                newProgramsinPortal.DescriptionforUser = request.ProgramsinPortal.DescriptionforUser;
                newProgramsinPortal.NameforAdmin = request.ProgramsinPortal.NameforAdmin;
                newProgramsinPortal.DescriptionforAdmin = request.ProgramsinPortal.DescriptionforAdmin;
                newProgramsinPortal.SortOrder = request.ProgramsinPortal.SortOrder;
                context.ProgramsinPortals.Add(newProgramsinPortal);
            }
            context.SaveChanges();
            response.ProgramsinPortal = request.ProgramsinPortal;
            response.success = true;
            return response;
        }

        public GetPortalFollowUpResponse GetPortalFollowUps(GetPortalFollowUpRequest request)
        {
            GetPortalFollowUpResponse response = new GetPortalFollowUpResponse();
            var portalFollowUps = context.PortalFollowUps.Include("FollowUpType").Include("KitsinPortalFollowUps").Include("KitsinPortalFollowUps.Kit").Where(x => x.PortalId == request.PortalId && (request.bothFollowUps || x.ProgramType == request.ProgramType)).ToList();
            response.portalFollowUps = Utility.mapper.Map<IList<DAL.PortalFollowUp>, IList<PortalFollowUpDto>>(portalFollowUps);
            return response;
        }

        public AddEditKittoFollowUpResponse AddKittoFollowUp(AddEditKittoFollowUpRequest request)
        {
            AddEditKittoFollowUpResponse response = new AddEditKittoFollowUpResponse();
            var kitsinPortalFollowUp = context.KitsinPortalFollowUps.Where(x => x.PortalFollowUpId == request.KitsinPortalFollowUp.PortalFollowUpId && x.KitId == request.KitsinPortalFollowUp.KitId).FirstOrDefault();
            if (kitsinPortalFollowUp != null)
            {
                context.KitsinPortalFollowUps.Remove(kitsinPortalFollowUp);
            }
            DAL.KitsinPortalFollowUp newKitinFollowUp = new DAL.KitsinPortalFollowUp();
            newKitinFollowUp.PortalFollowUpId = request.KitsinPortalFollowUp.PortalFollowUpId;
            newKitinFollowUp.KitId = request.KitsinPortalFollowUp.KitId;
            newKitinFollowUp.Order = request.KitsinPortalFollowUp.Order;
            newKitinFollowUp.Active = true;
            context.KitsinPortalFollowUps.Add(newKitinFollowUp);
            context.SaveChanges();
            response.kitsinPortalFollowUp = request.KitsinPortalFollowUp;
            response.success = true;
            return response;
        }

        public DeleteKitfromFollowUpResponse DeleteKitfromFollowUp(DeleteKitfromFollowUpRequest request)
        {
            DeleteKitfromFollowUpResponse response = new DeleteKitfromFollowUpResponse();
            var kitsinPortalFollowUps = context.KitsinPortalFollowUps.Where(x => x.PortalFollowUpId == request.portalFollowUpId && x.KitId == request.kitId).FirstOrDefault();
            if (kitsinPortalFollowUps != null)
            {
                kitsinPortalFollowUps.Active = false;
                context.KitsinPortalFollowUps.Attach(kitsinPortalFollowUps);
                context.Entry(kitsinPortalFollowUps).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public GetKitsinPortalFollowUpsResponse ReadFollowUpKits(GetPortalFollowUpRequest request)
        {
            GetKitsinPortalFollowUpsResponse response = new GetKitsinPortalFollowUpsResponse();
            var portalFollowUps = context.KitsinPortalFollowUps.Include("Kit").Where(x => x.PortalFollowUpId == request.PortalId).ToList();
            response.kitsinPortalFollowUps = Utility.mapper.Map<IList<DAL.KitsinPortalFollowUp>, IList<KitsinPortalFollowUpDto>>(portalFollowUps);
            return response;
        }

        public DeleteProgramsinPortalResponse DeleteProgramsinPortal(DeleteProgramsinPortalRequest request)
        {
            DeleteProgramsinPortalResponse response = new DeleteProgramsinPortalResponse();
            var ProgramsinPortal = context.ProgramsinPortals
                .Where(x => x.PortalId == request.PortalId && x.ProgramId == request.ProgramId).FirstOrDefault();
            if (ProgramsinPortal != null)
            {
                context.ProgramsinPortals.Remove(ProgramsinPortal);
                context.SaveChanges();
            }
            response.success = true;
            return response;
        }

        public GetPortalIncentivesResponse GetPortalIncentives(GetPortalIncentivesRequest request)
        {
            GetPortalIncentivesResponse response = new GetPortalIncentivesResponse();

            var portalIncentives = context.PortalIncentives.Where(x => x.PortalId == request.portalId).ToList();

            response.portalIncentives = Utility.mapper.Map<IList<DAL.PortalIncentive>, IList<PortalIncentiveDto>>(portalIncentives);
            return response;
        }

        public void CopyKitsFromPreviousPortal(int portalId)
        {
            var currentPortal = context.Portals.Include("Kits").Where(x => x.Id == portalId).FirstOrDefault();
            var portals = context.Portals.Include("Kits").Where(x => x.OrganizationId == currentPortal.OrganizationId).OrderByDescending(x => x.Id).ToList();
            var kits = portals.Skip(1).Take(1).FirstOrDefault().Kits;
            currentPortal.Kits = kits;
            context.SaveChanges();
        }

        public ListCouponsResponse ListCoupons(ListCouponsRequest request)
        {
            ListCouponsResponse response = new ListCouponsResponse();

            var coupons = context.CouponCodes.Where(x => x.OrganizationId == request.OrganizationId && (request.onlyActive == false || x.Active == true)).ToList();

            response.Coupons = Utility.mapper.Map<IList<DAL.CouponCode>, IList<CouponCodeDto>>(coupons);

            return response;
        }

        public CouponCodeDto ReadCoupon(int couponid)
        {
            var coupons = context.CouponCodes.Where(x => x.Id == couponid).FirstOrDefault();
            return Utility.mapper.Map<DAL.CouponCode, CouponCodeDto>(coupons);
        }

        public AddEditCouponResponse AddEditCoupon(AddEditCouponRequest request)
        {
            AddEditCouponResponse response = new AddEditCouponResponse();
            var coupons = context.CouponCodes.Where(x => x.Id == request.id).FirstOrDefault();
            if (coupons != null)
            {
                coupons.Name = request.name;
                coupons.Code = request.code;
                coupons.Active = request.active;
                context.CouponCodes.Attach(coupons);
                context.Entry(coupons).State = EntityState.Modified;
                context.SaveChanges();
                response.success = true;
            }
            else
            {
                DAL.CouponCode coupon = new DAL.CouponCode();
                coupon.Name = request.name;
                coupon.OrganizationId = request.orgId;
                coupon.Code = request.code;
                coupon.Active = request.active;
                context.CouponCodes.Add(coupon);
                context.SaveChanges();
                response.success = true;
            }
            return response;
        }

        public bool SavePortalEmails(NotificationEventTypePortalDto request)
        {
            var portal = context.Portals.Include("NotificationEventTypes").Where(x => x.Id == request.PortalId).FirstOrDefault();
            portal.NotificationEventTypes.Clear();
            if (!string.IsNullOrEmpty(request.types))
            {
                List<string> groupList = request.types.Split('-').ToList<string>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    var eventId = Convert.ToInt32(groupList[i]);
                    var email = context.NotificationEventTypes.Where(x => x.Id == eventId).FirstOrDefault();
                    portal.NotificationEventTypes.Add(email);
                }
            }
            context.Portals.Attach(portal);
            context.Entry(portal).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }

        public bool SavePortalKits(AddEditKitsinPortal request)
        {
            var portal = context.Portals.Include("Kits").Where(x => x.Id == request.PortalId).FirstOrDefault();
            portal.Kits.Clear();
            if (!string.IsNullOrEmpty(request.kits))
            {
                List<string> groupList = request.kits.Split('-').ToList<string>();
                for (int i = 0; i < groupList.Count; i++)
                {
                    var kitId = Convert.ToInt32(groupList[i]);
                    var kit = context.Kits.Where(x => x.Id == kitId).FirstOrDefault();
                    portal.Kits.Add(kit);
                }
            }
            context.Portals.Attach(portal);
            context.Entry(portal).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }

        public bool AddEditPortalRaffles(AddEditPortalRafflesRequest request)
        {
            var raffles = context.RafflesinPortals.Include("RaffleTypes").Where(x => x.Id == request.rafflesinPortals.Id).FirstOrDefault();
            if (raffles == null)
            {
                DAL.RafflesinPortals raffle = new DAL.RafflesinPortals();
                raffle = Utility.mapper.Map<RafflesinPortalsDto, DAL.RafflesinPortals>(request.rafflesinPortals);
                context.RafflesinPortals.Add(raffle);
                context.SaveChanges();
                request.rafflesinPortals.Id = raffle.Id;
            }
            else
            {
                raffles.Name = request.rafflesinPortals.Name;
                raffles.RaffleTypeId = request.rafflesinPortals.RaffleTypeId;
                raffles.Reward = request.rafflesinPortals.Reward;
                raffles.Description = request.rafflesinPortals.Description;
                raffles.isActive = request.rafflesinPortals.isActive;
                raffles.Points = request.rafflesinPortals.Points;
                context.RafflesinPortals.Attach(raffles);
                context.Entry(raffles).State = EntityState.Modified;
                context.SaveChanges();
            }
            foreach (var date in request.rafflesinPortals.Date)
            {
                var raffledates = context.RaffleDates.Where(x => x.Id == date.Key && x.RafflesinPortalsId == request.rafflesinPortals.Id).FirstOrDefault();
                if (date.Value.HasValue)
                {
                    if (raffledates == null)
                    {
                        RaffleDates raffledate = new RaffleDates();
                        raffledate.RafflesinPortalsId = request.rafflesinPortals.Id;
                        raffledate.RaffleDate = date.Value.Value;
                        context.RaffleDates.Add(raffledate);
                    }
                    else
                    {
                        raffledates.RaffleDate = date.Value.Value;
                        context.Entry(raffledates).State = EntityState.Modified;
                    }
                }
                else if (raffledates != null)
                {
                    context.RaffleDates.Remove(raffledates);
                }
                context.SaveChanges();
            }
            return true;
        }

        public GetPortalRafflesResponse GetPortalRaffles(GetRafflesinPortalsRequest request)
        {
            GetPortalRafflesResponse response = new GetPortalRafflesResponse();
            var raffles = context.RafflesinPortals.Include("RaffleTypes").Include("RaffleDates").Where(x => x.PortalId == request.portalId).ToList();
            response.rafflesinPortals = Utility.mapper.Map<List<DAL.RafflesinPortals>, List<RafflesinPortalsDto>>(raffles);
            return response;
        }

        public RafflesinPortalsDto GetPortalRafflesById(GetRafflesinPortalsRequest request)
        {
            RafflesinPortalsDto response = new RafflesinPortalsDto();
            var raffles = context.RafflesinPortals.Include("RaffleTypes").Include("RaffleDates").Where(x => x.Id == request.Id).FirstOrDefault();
            response = Utility.mapper.Map<DAL.RafflesinPortals, RafflesinPortalsDto>(raffles);
            return response;
        }

        public bool AddEditForms(AddEditPortalRequest request)
        {
            var portal = context.Portals.Where(x => x.Id == request.Portal.Id).FirstOrDefault();
            portal.PatientReleaseForm = request.Portal.PatientReleaseForm;
            portal.MedicalClearanceForm = request.Portal.MedicalClearanceForm;
            portal.KnowYourNumbersForm = request.Portal.KnowYourNumbersForm;
            portal.TestimonialForm = request.Portal.TestimonialForm;
            portal.TobaccoReleaseForm = request.Portal.TobaccoReleaseForm;
            context.Portals.Attach(portal);
            context.Entry(portal).State = EntityState.Modified;
            context.SaveChanges();
            return true;
        }

        public List<LanguagesDto> GetPortalLanguages(int portalId)
        {
            var portal = context.Portals.Include("Languages").Where(x => x.Id == portalId).FirstOrDefault();
            var languages = Utility.mapper.Map<List<DAL.Languages>, List<LanguagesDto>>(portal.Languages.ToList());
            return languages;
        }

        public List<int> GetEligibilityImportLoadList()
        {
            var eligibilityImportLoad = context.Portals.Where(x => x.EligibilityImportLoadFlag == true).ToList();
            return eligibilityImportLoad.Select(x => x.OrganizationId).ToList();
        }

        public List<CoachingConditionsDto> GetCoachingConditions()
        {
            var types = context.CoachingConditions.ToList();
            return Utility.mapper.Map<IList<DAL.CoachingConditions>, IList<CoachingConditionsDto>>(types).ToList();
        }

        public List<PortalCoachingConditionsDto> GetPortalCoachingConditions(int portalId)
        {
            var portal = context.Portals.Include("PortalCoachingConditions").Where(x => x.Id == portalId).FirstOrDefault();
            var portalCoachingConditions = Utility.mapper.Map<List<DAL.PortalCoachingConditions>, List<PortalCoachingConditionsDto>>(portal.PortalCoachingConditions.ToList());
            return portalCoachingConditions;
        }

        public List<PortalLabProcedureDto> GetPortalLabProcedures(int portalId)
        {
            var portal = context.Portals.Include("PortalLabProcedures").Include("PortalLabProcedures.LabProcedure").Where(x => x.Id == portalId).FirstOrDefault();
            var PortalLabProcedures = Utility.mapper.Map<List<DAL.PortalLabProcedure>, List<PortalLabProcedureDto>>(portal.PortalLabProcedures.ToList());
            return PortalLabProcedures;
        }

        public List<LabProcedureDto> GetLabProcedures()
        {
            var labProcedures = context.LabProcedures.ToList();
            return Utility.mapper.Map<IList<DAL.LabProcedure>, IList<LabProcedureDto>>(labProcedures).ToList();
        }

        public int ClonePortal(int portalId)
        {
            Portal clonedPortal = context.Portals.Where(p => p.Id == portalId).FirstOrDefault();
            clonedPortal.Name = clonedPortal.Name + "_Cloned";
            clonedPortal.Active = false;
            clonedPortal.StartDate = DateTime.UtcNow.Date;
            clonedPortal.EndDate = DateTime.UtcNow.Date.AddYears(1).AddDays(-1);
            clonedPortal.Id = 0;
            context.Portals.Add(clonedPortal);
            context.SaveChanges();

            var proginportallist = context.ProgramsinPortals.Where(p => p.PortalId == portalId);
            foreach (var portalPrograms in proginportallist)
            {
                portalPrograms.PortalId = clonedPortal.Id;
                portalPrograms.Id = 0;
                context.ProgramsinPortals.Add(portalPrograms);
            }
            context.SaveChanges();

            var incentiveInPortalList = context.PortalIncentives.Where(p => p.PortalId == portalId);
            foreach (var portalIncentives in incentiveInPortalList)
            {
                portalIncentives.PortalId = clonedPortal.Id;
                portalIncentives.Id = 0;
                context.PortalIncentives.Add(portalIncentives);
            }
            context.SaveChanges();

            var rafflesInPortalList = context.RafflesinPortals.Include("RaffleTypes").Where(p => p.PortalId == portalId);
            foreach (var rafflesIncentives in rafflesInPortalList)
            {
                rafflesIncentives.PortalId = clonedPortal.Id;
                rafflesIncentives.Id = 0;
                context.RafflesinPortals.Add(rafflesIncentives);
            }
            context.SaveChanges();

            var newRafflesInPortalList = context.RafflesinPortals.Where(p => p.PortalId == clonedPortal.Id).ToList();
            foreach (var newRafflesinPortal in newRafflesInPortalList)
            {
                var currentRecord = context.RafflesinPortals.Where(x => x.PortalId == portalId).FirstOrDefault();
                var RaffleDatesList = context.RaffleDates.Where(x => x.RafflesinPortalsId == currentRecord.Id).ToList();
                foreach (var raffleDates in RaffleDatesList)
                {
                    RaffleDates raffledate = new RaffleDates();
                    raffledate.RafflesinPortalsId = newRafflesinPortal.Id;
                    raffledate.RaffleDate = raffleDates.RaffleDate;
                    context.RaffleDates.Add(raffledate);
                }
            }

            var portalFollowupsList = context.PortalFollowUps.Where(p => p.PortalId == portalId);
            foreach (var portalFollowUps in portalFollowupsList)
            {
                portalFollowUps.PortalId = clonedPortal.Id;
                portalFollowUps.Id = 0;
                context.PortalFollowUps.Add(portalFollowUps);
            }
            context.SaveChanges();

            var newPortalFollowUpList = context.PortalFollowUps.Where(p => p.PortalId == clonedPortal.Id).ToList();
            foreach (var newPortalFollowUp in newPortalFollowUpList)
            {
                var currentRecord = context.PortalFollowUps.Where(x => x.PortalId == portalId && x.FollowupTypeId == newPortalFollowUp.FollowupTypeId).FirstOrDefault();
                var kitsInPortalFollowUpList = context.KitsinPortalFollowUps.Where(x => x.PortalFollowUpId == currentRecord.Id).ToList();
                foreach (var kitsInPortalFollowUp in kitsInPortalFollowUpList)
                {
                    DAL.KitsinPortalFollowUp newKitinFollowUp = new DAL.KitsinPortalFollowUp
                    {
                        PortalFollowUpId = newPortalFollowUp.Id,
                        KitId = kitsInPortalFollowUp.KitId,
                        Order = kitsInPortalFollowUp.Order,
                        Active = kitsInPortalFollowUp.Active
                    };
                    context.KitsinPortalFollowUps.Add(newKitinFollowUp);
                    context.SaveChanges();
                }
                context.SaveChanges();
            }

            var portalLabProcedureList = context.PortalLabProcedures.Where(p => p.PortalId == portalId);
            foreach (var portalLabProcedure in portalLabProcedureList)
            {
                portalLabProcedure.PortalId = clonedPortal.Id;
                portalLabProcedure.Id = 0;
                context.PortalLabProcedures.Add(portalLabProcedure);
            }
            context.SaveChanges();

            var portalCoachingCondList = context.PortalCoachingConditions.Where(p => p.PortalId == portalId);
            foreach (var portalCoachingCond in portalCoachingCondList)
            {
                portalCoachingCond.PortalId = clonedPortal.Id;
                portalCoachingCond.Id = 0;
                context.PortalCoachingConditions.Add(portalCoachingCond);
            }
            context.SaveChanges();

            return clonedPortal.Id;
        }
    }
}