using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class IncentiveReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public IList<UserIncentiveDto> ReadUserIncentives(int userId)
        {
            var dal = context.UserIncentives.Where(u => u.UserId == userId && (u.IsActive || (!u.IsActive && !string.IsNullOrEmpty(u.Reference)))).ToList();

            return Utility.mapper.Map<IList<DAL.UserIncentive>, IList<UserIncentiveDto>>(dal);
        }

        public IList<PortalIncentiveDto> ReadPortalIncentives(int portalId)
        {
            var dal = context.PortalIncentives.Include("IncentiveType").Where(u => u.PortalId == portalId && u.IsActive).ToList();
            return Utility.mapper.Map<IList<DAL.PortalIncentive>, IList<PortalIncentiveDto>>(dal);
        }

        public PortalIncentiveDto ReadPortalIncentivesByIncentiveId(int portalIncentiveId)
        {
            var dal = context.PortalIncentives.Where(u => u.Id == portalIncentiveId).FirstOrDefault();
            if (dal == null)
                return null;
            else
                return Utility.mapper.Map<DAL.PortalIncentive, PortalIncentiveDto>(dal);
        }

        public ProcessIncentives_ResultDto ProcessIncentives()
        {
            StoredProcedures sp = new StoredProcedures();
            var response = sp.ProcessIncentives();
            return Utility.mapper.Map<DAL.ProcessIncentives_Result, ProcessIncentives_ResultDto>(response);
        }

        public int ProcessUserKeys()
        {
            StoredProcedures sp = new StoredProcedures();
            var portals = context.Portals.Where(x => x.Active == true).ToList();
            int count = 0;
            foreach (var portal in portals)
            {
                count = count + sp.ProcessUserKeys(portal.Id);
            }
            return count;
        }

        public IList<IncentiveTypeDto> GetIncentiveTypes()
        {
            var dal = context.IncentiveTypes.ToList();

            return Utility.mapper.Map<IList<DAL.IncentiveType>, IList<IncentiveTypeDto>>(dal);
        }

        public IList<CustomIncentiveTypesDto> GetCustomIncentiveTypes()
        {
            var dal = context.CustomIncentiveTypes.ToList();

            return Utility.mapper.Map<IList<DAL.CustomIncentiveType>, IList<CustomIncentiveTypesDto>>(dal);
        }

        private List<DAL.PortalIncentive> GetPortalIncentives(int portalId)
        {
            return context.PortalIncentives.Where(p => p.PortalId == portalId && p.IsActive).ToList();
        }

        private List<DAL.PortalIncentive> GetIncentiveByType(IncentiveTypes type, int portalId, int? RefId)
        {
            return context.PortalIncentives.Where(p => p.PortalId == portalId && p.IsActive && p.IncentiveTypeId == (int)type && (!RefId.HasValue || p.RefId == RefId)).ToList();
        }

        private List<DAL.PortalIncentive> GetIncentivesByType(IncentiveTypes type, int portalId, int refId)
        {
            return context.PortalIncentives.Where(p => p.PortalId == portalId && p.IsActive && p.IncentiveTypeId == (int)type && p.RefId == refId).OrderBy(o => o.RefValue).ToList();
        }

        private List<PortalIncentive> SaveUserIncentive(int userId, List<PortalIncentive> incentives, int portalId, int? adminId = null)
        {
            List<PortalIncentive> successfulIncentives = new List<PortalIncentive>();
            foreach (var incentive in incentives)
            {
                if (SaveUserIncentive(userId, incentive, portalId, true, null, adminId))
                    successfulIncentives.Add(incentive);
            }
            return successfulIncentives;
        }

        public SaveUserIncentiveResponse SaveUserIncentive(SaveUserIncentiveRequest request)
        {
            SaveUserIncentiveResponse response = new SaveUserIncentiveResponse();
            var incentive = context.PortalIncentives.Where(p => p.Id == request.incentiveId).FirstOrDefault();
            response.success = SaveUserIncentive(request.userId, incentive, request.portalId, true, request.Reference, request.adminId);
            return response;
        }

        private bool SaveUserIncentive(int userId, PortalIncentive incentive, int portalId, bool isActive = true, string reference = null, int? adminId = null)
        {
            if (context.UserIncentives.Include("PortalIncentive").Where(p => p.PortalIncentive.PortalId == portalId && p.PortalIncentive.IncentiveTypeId == incentive.IncentiveTypeId && p.PortalIncentive.IsCompanyIncentive == incentive.IsCompanyIncentive && p.UserId == userId && p.IsActive).FirstOrDefault() == null
                || incentive.IncentiveTypeId == (int)IncentiveTypes.Vitals_Completion
                || (incentive.IncentiveTypeId == (int)IncentiveTypes.Kit_Completion && context.UserIncentives.Include("PortalIncentive").Where(p => p.PortalIncentive.PortalId == portalId && p.PortalIncentive.IncentiveTypeId == incentive.IncentiveTypeId && p.PortalIncentive.IsCompanyIncentive == incentive.IsCompanyIncentive && p.UserId == userId && p.IsActive && p.Reference == reference).FirstOrDefault() == null))
            {
                var userIncentive = context.UserIncentives.Include("PortalIncentive").Where(p => p.PortalIncentive.PortalId == portalId && p.PortalIncentive.IncentiveTypeId == incentive.IncentiveTypeId && p.PortalIncentive.IsCompanyIncentive == incentive.IsCompanyIncentive && p.UserId == userId).FirstOrDefault();
                if (userIncentive == null || isActive)
                {
                    DAL.UserIncentive dal = new UserIncentive();
                    dal.IsActive = isActive;
                    dal.UserId = userId;
                    dal.PortalIncentiveId = incentive.Id;
                    dal.Reference = reference;
                    dal.CreatedBy = adminId;
                    dal.DateCreated = System.DateTime.UtcNow;
                    context.UserIncentives.Add(dal);
                }
                else
                {
                    userIncentive.Reference = reference;
                    context.UserIncentives.Attach(userIncentive);
                    context.Entry(userIncentive).State = EntityState.Modified;
                }
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public AddCustomIncentiveResponse AddCustomIncentive(AddCustomIncentiveRequest request)
        {
            AddCustomIncentiveResponse response = new AddCustomIncentiveResponse();
            DAL.UserIncentive dal = new UserIncentive();
            dal.IsActive = true;
            dal.UserId = request.userId;
            dal.PortalIncentiveId = request.PortalIncentiveId;
            dal.Reference = request.CustomIncentiveType.ToString();
            dal.Points = request.Points;
            dal.CreatedBy = request.adminId;
            dal.DateCreated = System.DateTime.UtcNow;
            dal.Comments = request.comments;
            context.UserIncentives.Add(dal);
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public SaveTobaccoAffidavitResponse SaveTobaccoAffidavit(SaveTobaccoAffidavitRequest tobaccoAffidavitRequest)
        {
            SaveTobaccoAffidavitResponse response = new SaveTobaccoAffidavitResponse();
            var incentive = GetIncentiveByType(IncentiveTypes.Tobacco_Initiative, tobaccoAffidavitRequest.portalId, tobaccoAffidavitRequest.programsInPortalId).FirstOrDefault();
            incentive = incentive == null ? GetIncentiveByType(IncentiveTypes.Tobacco_Initiative, tobaccoAffidavitRequest.portalId, null).FirstOrDefault() : incentive;
            response.success = false;
            if (incentive != null)
            {
                response.success = SaveUserIncentive(tobaccoAffidavitRequest.userId, incentive, tobaccoAffidavitRequest.portalId, false, tobaccoAffidavitRequest.reference);
                if (response.success)
                {
                    AdminReader reader = new AdminReader();
                    AddEditTaskRequest request = new AddEditTaskRequest();
                    request.task = new TasksDto();
                    request.task.Owner = request.task.UserId = request.task.UpdatedBy = request.task.CreatedBy = tobaccoAffidavitRequest.userId;
                    request.task.Status = DTO.TaskStatus.N.ToString();
                    request.task.TaskTypeId = reader.TaskTypes().Find(t => t.Name == DTO.Constants.Tobacco_Affidavit_TT).Id;
                    request.task.Comment = "Tobacco affidavit requires approval";
                    request.task.IsActive = true;
                    reader.AddEditTask(request);
                }
            }
            return response;
        }

        public bool ApproveTobaccoAffidavit(int userIncentiveId)
        {
            var incentive = context.UserIncentives.Where(p => p.Id == userIncentiveId).FirstOrDefault();
            if (incentive != null && context.UserIncentives.Where(p => p.PortalIncentiveId == incentive.PortalIncentiveId && p.UserId == incentive.UserId && p.IsActive).FirstOrDefault() == null)
            {
                incentive.IsActive = true;
                context.UserIncentives.Attach(incentive);
                context.Entry(incentive).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;
        }

        public DeleteUserIncentiveResponse DeleteUserIncentive(DeleteUserIncentiveRequest request)
        {
            DeleteUserIncentiveResponse response = new DeleteUserIncentiveResponse();
            var dal = context.UserIncentives.Where(p => p.Id == request.userIncentiveId).FirstOrDefault();
            if (request.adminId.HasValue)
            {
                dal.IsActive = false;
                dal.UpdatedBy = request.adminId;
                dal.UpdatedOn = System.DateTime.UtcNow;
            }
            dal.Reference = null;
            context.UserIncentives.Attach(dal);
            context.Entry(dal).State = EntityState.Modified;
            context.SaveChanges();
            response.success = true;
            return response;
        }

        public bool SavePortalIncentive(PortalIncentiveDto dto)
        {
            var dal = Utility.mapper.Map<PortalIncentiveDto, DAL.PortalIncentive>(dto);
            dal.DateCreated = System.DateTime.UtcNow;
            var incentiveDAL = context.PortalIncentives.Where(x => x.Id == dal.Id).FirstOrDefault();
            if (incentiveDAL != null)
                context.Entry(incentiveDAL).CurrentValues.SetValues(dal);
            else
                context.PortalIncentives.Add(dal);
            context.SaveChanges();
            return true;
        }

        public void AwardIncentives(AwardIncentivesRequest request)
        {
            var incentives = GetIncentiveByType(request.incentiveType, request.portalId, null);
            if (incentives != null && incentives.Count > 0)
            {
                if (!request.isEligible)
                    incentives = incentives.FindAll(p => !p.IsCompanyIncentive);
                var successfulIncentive = SaveUserIncentive(request.userId, incentives, request.portalId, request.adminId);
                if (request.isEligible && !string.IsNullOrEmpty(request.companyIncentiveMessage))
                {
                    var incentive = successfulIncentive.FindAll(p => p.IsCompanyIncentive).FirstOrDefault();
                    if (incentive != null)
                    {
                        var portal = context.Portals.Where(x => x.Id == request.portalId).FirstOrDefault();
                        string portalName = string.Empty;
                        if (portal != null)
                            portalName = portal.Name;
                        CommonReader reader = new CommonReader();
                        reader.AddDashboardMessage(request.userId, request.companyIncentiveMessage, null, null, incentive.Points, portalName);
                    }
                }
                var interventincentive = successfulIncentive.FindAll(p => !p.IsCompanyIncentive).FirstOrDefault();
                if (interventincentive != null && !string.IsNullOrEmpty(request.pointsIncentiveMessage))
                {
                    CommonReader reader = new CommonReader();
                    reader.AddDashboardMessage(request.userId, request.pointsIncentiveMessage, null, null, interventincentive.Points);
                }
            }

        }

        //will be called for every coaching calls
        public void Program_NumberOfCalls(Program_NumberOfCallsRequest request)
        {
            var portalIncentives = GetPortalIncentives(request.portalId);
            Program_Maternity_Completion(request.programsInPortalId, request.portalId, request.userId, request.usersInProgramId, request.hraId, portalIncentives);
            var incentiveList = portalIncentives.FindAll(inc => inc.IncentiveTypeId == (int)IncentiveTypes.Intervent_NSession);
            var incentives = incentiveList.FindAll(inc => inc.RefId == request.programsInPortalId);
            if (incentives == null || incentives.Count == 0)
            {
                incentives = incentiveList.FindAll(inc => inc.RefId == null);
            }

            Validate_NCalls(incentives, request.portalId, request.userId, request.usersInProgramId, request.hraId, request.orgId, request.southUniversityOrgId);
        }

        public void Tobacco_Incentive(Tobacco_IncentiveRequest request)
        {
            var incentiveList = GetIncentiveByType(IncentiveTypes.Tobacco_Initiative, request.portalId, null);
            if (incentiveList != null && incentiveList.Count > 0)
            {
                var usersInProgram = context.UsersinPrograms.Where(p => p.Id == request.usersInProgramId).FirstOrDefault();
                if (usersInProgram != null && usersInProgram.StartDate.HasValue)
                {
                    var programsInPortalId = usersInProgram.ProgramsinPortalsId;
                    var incentives = incentiveList.FindAll(inc => inc.RefId == programsInPortalId);
                    if (incentives.FirstOrDefault() != null && incentives.FirstOrDefault().RefValue3.HasValue && NumberOfWeeks(usersInProgram.StartDate.Value) >= incentives.FirstOrDefault().RefValue3.Value)
                        Validate_NCalls(incentives, request.portalId, request.userId, request.usersInProgramId, request.hraId, null, request.southUniversityOrgId, IncentiveMessageTypes.TC_Incentive);
                }
            }
        }

        private int NumberOfWeeks(DateTime startDate)
        {
            TimeSpan ts = DateTime.UtcNow - startDate;
            return (ts.Days / 7);
        }

        private void Validate_NCalls(List<PortalIncentive> incentiveList, int portalId, int userId, int usersInProgramId, int hraId, int? organizationId, int southUniversityOrgId, string dashboardmessageType = IncentiveMessageTypes.Incentive)
        {
            if (incentiveList != null && incentiveList.Count > 0)
            {
                ParticipantReader reader = new ParticipantReader();
                GetCoachingCountRequest request = new GetCoachingCountRequest();
                request.userId = userId;
                request.portalId = portalId;
                request.refId = usersInProgramId;
                request.hraId = hraId;
                var coachingNotes = reader.GetCoachingCount(request);
                var incentives = incentiveList.FindAll(inc => inc.RefValue == coachingNotes.count).ToList();
                if (incentives != null)
                {
                    foreach (var incentive in incentives)
                    {
                        if ((organizationId.HasValue && organizationId.Value == southUniversityOrgId && incentive.IncentiveTypeId == (int)IncentiveTypes.Intervent_NSession && incentive.IsCompanyIncentive)
                        && (coachingNotes.participantNotes.OrderBy(x => x.NotesDate).FirstOrDefault().NotesDate.Date > Convert.ToDateTime("9/30/2023").Date
                        || coachingNotes.participantNotes.OrderBy(x => x.NotesDate).LastOrDefault().NotesDate.Date > Convert.ToDateTime("12/31/2023").Date
                        || context.UserIncentives.Where(p => p.PortalIncentiveId == incentive.Id && p.PortalIncentive.IncentiveTypeId == (int)IncentiveTypes.Intervent_NSession && p.IsActive).Count() > 100))
                            return;
                        var success = SaveUserIncentive(userId, incentive, portalId);
                        if (success)
                        {
                            CommonReader commonreader = new CommonReader();
                            commonreader.AddDashboardMessage(userId, dashboardmessageType, null, null, incentive.Points);
                        }
                    }
                }
            }
        }

        //Will be called only if the completed successfully
        private void Program_Maternity_Completion(int programsInPortalId, int portalId, int userId, int usersInProgramId, int hraId, List<PortalIncentive> portalIncentives)
        {
            var incentiveList = portalIncentives.FindAll(inc => inc.IncentiveTypeId == (int)IncentiveTypes.Maternity_Completion && inc.RefId == programsInPortalId);
            if (incentiveList != null && incentiveList.Count > 0)
            {
                CommonReader commonReader = new CommonReader();
                ParticipantReader reader = new ParticipantReader();
                GetCoachingCountRequest coachingCountRequest = new GetCoachingCountRequest();
                coachingCountRequest.userId = userId;
                coachingCountRequest.portalId = portalId;
                coachingCountRequest.hraId = hraId;
                coachingCountRequest.refId = usersInProgramId;
                var count = reader.GetCoachingCount(coachingCountRequest).count;
                var incentives = incentiveList.FindAll(inc => inc.RefValue2 == count);
                if (incentives != null && incentives.Count > 0)
                {
                    HRAReader hra = new HRAReader();
                    var up = context.UsersinPrograms.Where(x => x.Id == usersInProgramId).FirstOrDefault();
                    if (up != null && up.EnrolledOn.HasValue)
                    {
                        DateTime startDate = up.EnrolledOn.Value;
                        GetPregnencyDaysRequest request = new GetPregnencyDaysRequest();
                        request.userId = userId;
                        request.hraId = hraId;
                        request.startDate = startDate;
                        int days = hra.GetPregnencyDays(request).days;
                        if (days > 0)
                        {
                            foreach (var inc in incentives)
                            {
                                if (inc.RefValue > days)
                                {
                                    var success = SaveUserIncentive(userId, inc, portalId);
                                    if (success)
                                    {
                                        commonReader.AddDashboardMessage(userId, IncentiveMessageTypes.PP_Incentive, null, null);
                                    }
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}