using Intervent.DAL;
using Intervent.Framework.Clone;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class FollowUpReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public List<UserDto> TriggerFollowUp(int systemAdminId, int? usersinProgramId = null, int? updatedBy = null)
        {
            CommonReader commonreader = new CommonReader();
            ProgramReader programReader = new ProgramReader();
            List<DAL.UsersinProgram> response = new List<DAL.UsersinProgram>();

            if (usersinProgramId.HasValue)
            {
                var UsersinProgram = context.UsersinPrograms.Include("User").Include("FollowUps").Include("ProgramsinPortal.Program").Include("ProgramsinPortal.Portal.Organization")
                    .Include("ProgramsinPortal.Portal.PortalFollowUps").Include("ProgramsinPortal.Portal.PortalFollowUps.FollowUpType").Include("ProgramsinPortal.Portal.PortalFollowUps.KitsinPortalFollowUps")
                    .Where(x => x.Id == usersinProgramId.Value).FirstOrDefault();
                UsersinProgram.AssignedFollowUp = (byte)((UsersinProgram.AssignedFollowUp ?? 0) + 1);
                UsersinProgram.UpdatedBy = updatedBy;
                UsersinProgram.UpdatedOn = DateTime.UtcNow;
                var followupCount = UsersinProgram.FollowUps.Where(x => x.CompleteDate != null).Count();
                var userPrograms = programReader.GetUserProgramsByPortal(new GetUserProgramHistoryRequest { portalId = UsersinProgram.ProgramsinPortal.PortalId, userId = UsersinProgram.UserId });
                var followUpType = UsersinProgram.ProgramsinPortal.Portal.PortalFollowUps.OrderBy(x => x.FollowUpType.Days).Skip(followupCount).Take(1);
                if (userPrograms.Count() == 1 && followUpType.Count() > 0 && followUpType.FirstOrDefault().KitsinPortalFollowUps.Where(x => x.Active == true).Count() > 0)
                {
                    AssignKitsToUserProgramRequest assignKitsRequest = new AssignKitsToUserProgramRequest();
                    assignKitsRequest.kitIds = followUpType.FirstOrDefault().KitsinPortalFollowUps.Where(x => x.Active == true).Select(x => x.KitId).ToList();
                    assignKitsRequest.usersinProgramsId = UsersinProgram.Id;
                    assignKitsRequest.programType = UsersinProgram.ProgramsinPortal.Program.ProgramType;
                    assignKitsRequest.userId = UsersinProgram.UserId;
                    assignKitsRequest.language = UsersinProgram.User.LanguagePreference;
                    programReader.AssignKitsToUserProgram(assignKitsRequest);
                }
                response.Add(UsersinProgram);
            }
            else
            {
                List<DAL.UsersinProgram> UsersinPrograms;
                UsersinPrograms = context.UsersinPrograms.Include("User").Include("ProgramsinPortal").Include("FollowUps")
                    .Include("ProgramsinPortal.Program").Include("ProgramsinPortal.Portal")
                    .Include("ProgramsinPortal.Portal.PortalFollowUps").Include("ProgramsinPortal.Portal.PortalFollowUps.FollowUpType").Include("ProgramsinPortal.Portal.PortalFollowUps.KitsinPortalFollowUps").Include("ProgramsinPortal.Portal.Organization")
                    .Where(x => !ListOptions.AlbertaUsers.Contains(x.UserId) && x.IsActive == true && x.ProgramsinPortal.Portal.Active == true && x.User.IsActive == true &&
                        ((x.ProgramsinPortal.Program.ProgramType == 2 && x.ProgramsinPortal.Program.Pregancy == false && x.ProgramsinPortal.Portal.FollowUpforCoaching == true)
                        || (x.ProgramsinPortal.Program.ProgramType == 1 && x.ProgramsinPortal.Portal.FollowUpforSelfHelp == true))).ToList();
                foreach (var UsersinProgram in UsersinPrograms)
                {
                    var followupCount = UsersinProgram.FollowUps.Where(x => x.CompleteDate != null).Count();
                    var followupTypeIds = UsersinProgram.ProgramsinPortal.Portal.PortalFollowUps.Where(x => x.ProgramType == UsersinProgram.ProgramsinPortal.Program.ProgramType).Select(x => x.FollowupTypeId).ToList();
                    var followUpTypes = commonreader.GetFollowUpTypes().Where(x => followupTypeIds.Contains(x.Id)).ToList();
                    var followUpType = followUpTypes.OrderBy(x => x.Days).Skip(followupCount).Take(1);
                    var followUpTypewithKits = UsersinProgram.ProgramsinPortal.Portal.PortalFollowUps.OrderBy(x => x.FollowUpType.Days).Skip(followupCount).Take(1);
                    if (followUpType == null || followUpType.Count() == 0)
                        continue;
                    var isValid = UsersinProgram.StartDate?.AddDays(followUpType.FirstOrDefault().Days) <= DateTime.UtcNow;
                    if (isValid && (UsersinProgram.AssignedFollowUp ?? 0) == followupCount)
                    {
                        if (followUpType.Count() > 0 && followUpTypewithKits.Count() > 0 && followUpTypewithKits.FirstOrDefault().KitsinPortalFollowUps.Where(x => x.Active == true).Count() > 0)
                        {
                            AssignKitsToUserProgramRequest assignKitsRequest = new AssignKitsToUserProgramRequest();
                            assignKitsRequest.kitIds = followUpTypewithKits.FirstOrDefault().KitsinPortalFollowUps.Where(x => x.Active == true).Select(x => x.KitId).ToList();
                            assignKitsRequest.usersinProgramsId = UsersinProgram.Id;
                            assignKitsRequest.programType = UsersinProgram.ProgramsinPortal.Program.ProgramType;
                            assignKitsRequest.userId = UsersinProgram.UserId;
                            assignKitsRequest.language = UsersinProgram.User.LanguagePreference;
                            programReader.AssignKitsToUserProgram(assignKitsRequest);
                        }
                        UsersinProgram.AssignedFollowUp = (byte)(followupCount + 1);
                        UsersinProgram.UpdatedBy = systemAdminId;
                        UsersinProgram.UpdatedOn = DateTime.UtcNow;
                        response.Add(UsersinProgram);
                    }
                }
            }
            var users = UpdateFollowUpInfo(response);
            return users;
        }

        public List<UserDto> UpdateFollowUpInfo(List<DAL.UsersinProgram> UsersinPrograms)
        {
            CommonReader commonreader = new CommonReader();
            List<UserDto> users = new List<UserDto>();
            foreach (var UsersinProgram in UsersinPrograms)
            {
                context.UsersinPrograms.Attach(UsersinProgram);
                context.Entry(UsersinProgram).State = EntityState.Modified;
                context.SaveChanges();
                commonreader.AddDashboardMessage(UsersinProgram.User.Id, IncentiveMessageTypes.Follow_Up, null, null);
                UserDto User = new UserDto();
                User.Id = UsersinProgram.User.Id;
                User.Email = UsersinProgram.User.Email;
                OrganizationDto Organization = new OrganizationDto();
                Organization.ContactEmail = UsersinProgram.ProgramsinPortal.Portal.Organization.ContactEmail;
                Organization.ContactNumber = UsersinProgram.ProgramsinPortal.Portal.Organization.ContactNumber;
                User.Organization = Organization;
                users.Add(User);
            }
            return users;
        }

        public FollowUpResponse CreateFollowUp(FollowUpDto request)
        {
            FollowUpResponse response = new FollowUpResponse();
            DAL.FollowUp followUp = context.FollowUps.Where(x => x.Id == request.Id).FirstOrDefault();
            if (followUp != null)
            {
                response.FollowUpDto = Utility.mapper.Map<DAL.FollowUp, FollowUpDto>(followUp);
                return response;
            }
            var usersinProgram = context.UsersinPrograms.Include("FollowUps").Where(x => x.Id == request.UsersinProgramsId).FirstOrDefault();
            if (usersinProgram != null && usersinProgram.FollowUps.Count() >= usersinProgram.AssignedFollowUp)
            {

                response.FollowUpDto = new FollowUpDto();
                response.FollowUpDto.Id = usersinProgram.FollowUps.OrderByDescending(x => x.Id).FirstOrDefault().Id;
                return response;
            }
            followUp = new DAL.FollowUp();
            followUp.UsersinProgramsId = request.UsersinProgramsId;
            followUp.StartDate = DateTime.UtcNow;
            followUp.CreatedBy = request.CreatedBy;
            context.FollowUps.Add(followUp);
            context.SaveChanges();
            response.FollowUpDto = new FollowUpDto();
            response.FollowUpDto.Id = followUp.Id;
            return response;
        }

        public FollowUpResponse UpdateFollowUp(UpdateFollowUpRequest request, string DTCOrgCode)
        {
            FollowUpResponse response = new FollowUpResponse();
            CommonReader commonReader = new CommonReader();
            bool complete = false;
            var followUp = context.FollowUps.Where(x => x.Id == request.followUpId).FirstOrDefault();
            if (!followUp.CompleteDate.HasValue)
            {
                if (string.IsNullOrEmpty(followUp.PageSeqDone))
                    followUp.PageSeqDone = request.pageName.ToString() + ".";
                else if (!followUp.PageSeqDone.Contains(request.pageName.ToString()))
                    followUp.PageSeqDone = followUp.PageSeqDone + request.pageName.ToString() + ".";
                string[] pageSeqArray = followUp.PageSeqDone.Split('.').Take(followUp.PageSeqDone.Split('.').Length - 1).ToArray();
                Array.Sort(pageSeqArray);
                string[] pageSeqRefArray = Constants.FollowUpPageSeq;
                Array.Sort(pageSeqRefArray);
                if (pageSeqArray.SequenceEqual(pageSeqRefArray))
                {
                    followUp.CompleteDate = DateTime.UtcNow;
                    complete = true;
                    IncentiveReader reader = new IncentiveReader();
                    commonReader.AddDashboardMessage(request.userId, IncentiveMessageTypes.Followup_Report, null, null);
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.FollowUp_Evaluation;
                    incentivesRequest.userId = request.userId;
                    incentivesRequest.portalId = request.portalId;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.FollowUp_Incentive;
                    reader.AwardIncentives(incentivesRequest);
                    if (request.isIntuityUser)
                    {
                        IntuityReader intuityReader = new IntuityReader();
                        AddIntuityEventRequest intuityEventRequest = new AddIntuityEventRequest();
                        intuityEventRequest.intuityEvent = new IntuityEventDto();
                        intuityEventRequest.intuityEvent.UserId = request.userId;
                        intuityEventRequest.organizationCode = request.OrganizationCode;
                        intuityEventRequest.intuityEvent.UniqueId = request.uniqueId;
                        intuityEventRequest.intuityEvent.EventType = (int)IntuityEventTypes.FollowUp_Completion;
                        intuityEventRequest.intuityEvent.CreatedBy = request.updatedBy;
                        intuityEventRequest.intuityEvent.EventDate = followUp.CompleteDate;
                        intuityReader.AddIntuityEvent(intuityEventRequest, DTCOrgCode);
                    }
                }
                context.FollowUps.Attach(followUp);
                context.Entry(followUp).State = EntityState.Modified;
                context.SaveChanges();
            }
            if (followUp.CompleteDate.HasValue || complete)
                GenFollowUpGoals(request.followUpId);
            response.FollowUpDto = Utility.mapper.Map<DAL.FollowUp, FollowUpDto>(followUp);
            return response;
        }

        public FollowUpResponse ReadFollowUp(ReadFollowUpRequest request)
        {
            FollowUpResponse response = new FollowUpResponse();
            var followUp = context.FollowUps.Include("UsersinProgram").Include("UsersinProgram.User").Include("UsersinProgram.User.Organization")
                    .Include("UsersinProgram.ProgramsinPortal").Include("UsersinProgram.ProgramsinPortal.Portal").Include("FollowUp_HealthNumbers").Where(x => x.Id == request.FollowUpId).FirstOrDefault();
            response.FollowUpDto = Utility.mapper.Map<DAL.FollowUp, FollowUpDto>(followUp);
            return response;
        }

        public FollowUpResponse AddEditMedicalCondition(AddEditFUMedicalConditionsRequest request, string DTCOrgCode)
        {
            FollowUpResponse response = new FollowUpResponse();
            var medicalCondition = Utility.mapper.Map<FollowUp_MedicalConditionsDto, DAL.FollowUp_MedicalConditions>(request.medicalCondition);
            var currentmedicalCondition = context.FollowUp_MedicalConditions.Where(x => x.Id == request.medicalCondition.Id).FirstOrDefault();
            var clonemedicalCondition = CloneUtil.DeepClone<DAL.FollowUp_MedicalConditions>(currentmedicalCondition);
            if (currentmedicalCondition != null)
            {
                context.Entry(currentmedicalCondition).CurrentValues.SetValues(medicalCondition);
                UserHistoryReader.LogUserChanges(clonemedicalCondition, medicalCondition, request.ParticipantId, request.UserId, UserHistoryCategoryDto.FollowUp);
            }
            else
                context.FollowUp_MedicalConditions.Add(medicalCondition);
            context.SaveChanges();
            response.FollowUpDto = UpdateFollowUp(new UpdateFollowUpRequest { pageName = PageName.MC, followUpId = request.medicalCondition.Id, userId = request.ParticipantId, portalId = request.PortalId, updatedBy = request.UserId, isIntuityUser = request.IsIntuityUser, uniqueId = request.UniqueId, OrganizationCode = request.OrganizationCode }, DTCOrgCode).FollowUpDto;
            return response;
        }

        public FollowUpMedicalConditionsResponse ReadMedicalCondition(ReadFollowUpRequest request)
        {
            FollowUpMedicalConditionsResponse response = new FollowUpMedicalConditionsResponse();
            if (request.FollowUpId > 0)
            {
                var medicalCondition = context.FollowUp_MedicalConditions.Where(x => x.Id == request.FollowUpId).FirstOrDefault();
                response.FollowUp_MedicalConditionsDto = Utility.mapper.Map<DAL.FollowUp_MedicalConditions, FollowUp_MedicalConditionsDto>(medicalCondition);
            }
            if (response.FollowUp_MedicalConditionsDto == null)
                response.FollowUp_MedicalConditionsDto = new FollowUp_MedicalConditionsDto();
            var usersinProgram = context.UsersinPrograms.Include("HRA").Include("HRA.HRA_Goals").Include("HRA.HRA_MedicalConditions").Where(x => x.Id == request.UsersInProgramsId).FirstOrDefault();
            response.FollowUp_MedicalConditionsDto.FollowUp = new FollowUpDto();
            response.FollowUp_MedicalConditionsDto.FollowUp.UsersinProgram = Utility.mapper.Map<DAL.UsersinProgram, UsersinProgramDto>(usersinProgram);
            return response;
        }

        public FollowUpResponse AddEditHealthConditions(AddEditFUHealthConditionRequest request, string DTCOrgCode)
        {
            FollowUpResponse response = new FollowUpResponse();
            var healthCondition = Utility.mapper.Map<FollowUp_HealthConditionsDto, DAL.FollowUp_HealthConditions>(request.healthConditionsDto);
            var currenthealthCondition = context.FollowUp_HealthConditions.Where(x => x.Id == request.healthConditionsDto.Id).FirstOrDefault();
            var clonedhealthCondition = CloneUtil.DeepClone<DAL.FollowUp_HealthConditions>(currenthealthCondition);
            if (currenthealthCondition != null)
            {
                context.Entry(currenthealthCondition).CurrentValues.SetValues(healthCondition);
                UserHistoryReader.LogUserChanges(clonedhealthCondition, healthCondition, request.ParticipantId, request.UserId, UserHistoryCategoryDto.FollowUp);
            }
            else
                context.FollowUp_HealthConditions.Add(healthCondition);

            context.SaveChanges();
            response.FollowUpDto = UpdateFollowUp(new UpdateFollowUpRequest { pageName = PageName.YL, followUpId = request.healthConditionsDto.Id, userId = request.ParticipantId, portalId = request.PortalId, updatedBy = request.UserId, isIntuityUser = request.IsIntuityUser, uniqueId = request.UniqueId, OrganizationCode = request.OrganizationCode }, DTCOrgCode).FollowUpDto;
            return response;
        }

        public FollowUpHealthConditionsResponse ReadHealthCondition(ReadFollowUpRequest request)
        {
            FollowUpHealthConditionsResponse response = new FollowUpHealthConditionsResponse();
            var healthCondition = context.FollowUp_HealthConditions.Where(x => x.Id == request.FollowUpId).FirstOrDefault();
            response.FollowUp_HealthConditionsDto = Utility.mapper.Map<DAL.FollowUp_HealthConditions, FollowUp_HealthConditionsDto>(healthCondition);
            return response;
        }

        public FollowUpResponse AddEditHealthNumbers(AddEditFUHealthNumbersRequest request, string DTCOrgCode)
        {
            FollowUpResponse response = new FollowUpResponse();
            var healthNumbers = Utility.mapper.Map<FollowUp_HealthNumbersDto, DAL.FollowUp_HealthNumbers>(request.healthNumbersDto);
            var currenthealthNumbers = context.FollowUp_HealthNumbers.Where(x => x.Id == request.healthNumbersDto.Id).FirstOrDefault();
            var clonedhealthNumbers = CloneUtil.DeepClone<DAL.FollowUp_HealthNumbers>(currenthealthNumbers);
            if (currenthealthNumbers != null)
            {
                UserHistoryReader.LogUserChanges(clonedhealthNumbers, healthNumbers, request.ParticipantId, request.UserId, UserHistoryCategoryDto.FollowUp);
            }

            var healthNumber = context.FollowUp_HealthNumbers.Where(x => x.Id == request.healthNumbersDto.Id).FirstOrDefault();
            if (healthNumber != null)
            {
                DAL.FollowUp_HealthNumbers currentNumbers = null;
                currentNumbers = CloneUtil.DeepClone<DAL.FollowUp_HealthNumbers>(healthNumber);
                healthNumber.Id = request.healthNumbersDto.Id;
                if (request.bloodwork)
                {
                    healthNumber.DidYouFast = request.healthNumbersDto.DidYouFast;
                    healthNumber.BloodTestDate = request.healthNumbersDto.BloodTestDate;
                    healthNumber.TotalChol = request.healthNumbersDto.TotalChol;
                    healthNumber.LDL = request.healthNumbersDto.LDL;
                    healthNumber.HDL = request.healthNumbersDto.HDL;
                    healthNumber.Trig = request.healthNumbersDto.Trig;
                    healthNumber.Glucose = request.healthNumbersDto.Glucose;
                    healthNumber.A1C = request.healthNumbersDto.A1C;
                }
                else
                {
                    healthNumber.BPArm = request.healthNumbersDto.BPArm;
                    healthNumber.SBP = request.healthNumbersDto.SBP;
                    healthNumber.DBP = request.healthNumbersDto.DBP;
                    healthNumber.Weight = request.healthNumbersDto.Weight;
                    healthNumber.Height = request.healthNumbersDto.Height;
                    healthNumber.Waist = request.healthNumbersDto.Waist;
                    healthNumber.BMI = request.healthNumbersDto.BMI;
                    healthNumber.RMR = request.healthNumbersDto.RMR;
                    healthNumber.THRFrom = request.healthNumbersDto.THRFrom;
                    healthNumber.THRTo = request.healthNumbersDto.THRTo;
                    healthNumber.CRF = request.healthNumbersDto.CRF;
                    healthNumber.RHR = request.healthNumbersDto.RHR;
                }
                context.FollowUp_HealthNumbers.Attach(healthNumber);
                context.Entry(healthNumber).State = EntityState.Modified;
                UserHistoryReader.LogUserChanges(clonedhealthNumbers, healthNumbers, request.ParticipantId, request.UserId, UserHistoryCategoryDto.FollowUp);
            }
            else
            {
                DAL.FollowUp_HealthNumbers newHealthNumbers = new DAL.FollowUp_HealthNumbers();
                newHealthNumbers.Id = request.healthNumbersDto.Id;
                if (request.bloodwork)
                {
                    newHealthNumbers.DidYouFast = request.healthNumbersDto.DidYouFast;
                    newHealthNumbers.BloodTestDate = request.healthNumbersDto.BloodTestDate;
                    newHealthNumbers.TotalChol = request.healthNumbersDto.TotalChol;
                    newHealthNumbers.LDL = request.healthNumbersDto.LDL;
                    newHealthNumbers.HDL = request.healthNumbersDto.HDL;
                    newHealthNumbers.Trig = request.healthNumbersDto.Trig;
                    newHealthNumbers.Glucose = request.healthNumbersDto.Glucose;
                    newHealthNumbers.A1C = request.healthNumbersDto.A1C;
                }
                else
                {
                    newHealthNumbers.BPArm = request.healthNumbersDto.BPArm;
                    newHealthNumbers.SBP = request.healthNumbersDto.SBP;
                    newHealthNumbers.DBP = request.healthNumbersDto.DBP;
                    newHealthNumbers.Weight = request.healthNumbersDto.Weight;
                    newHealthNumbers.Height = request.healthNumbersDto.Height;
                    newHealthNumbers.Waist = request.healthNumbersDto.Waist;
                    newHealthNumbers.BMI = request.healthNumbersDto.BMI;
                    newHealthNumbers.RMR = request.healthNumbersDto.RMR;
                    newHealthNumbers.THRFrom = request.healthNumbersDto.THRFrom;
                    newHealthNumbers.THRTo = request.healthNumbersDto.THRTo;
                    newHealthNumbers.CRF = request.healthNumbersDto.CRF;
                    newHealthNumbers.RHR = request.healthNumbersDto.RHR;
                }
                context.FollowUp_HealthNumbers.Add(newHealthNumbers);
            }
            context.SaveChanges();
            response.FollowUpDto = UpdateFollowUp(new UpdateFollowUpRequest { pageName = PageName.YN, followUpId = request.healthNumbersDto.Id, userId = request.ParticipantId, portalId = request.PortalId, updatedBy = request.UserId, isIntuityUser = request.IsIntuityUser, uniqueId = request.UniqueId, OrganizationCode = request.OrganizationCode }, DTCOrgCode).FollowUpDto;
            ParticipantReader participantReader = new ParticipantReader();
            if (!request.bloodwork)
            {
                AddEditWellnessDataRequest WDrequest = new AddEditWellnessDataRequest();
                WDrequest.FollowUpId = request.healthNumbersDto.Id;
                WDrequest.WellnessData = new WellnessDataDto();
                WDrequest.WellnessData.Weight = request.healthNumbersDto.Weight;
                WDrequest.WellnessData.waist = request.healthNumbersDto.Waist;
                WDrequest.WellnessData.SBP = request.healthNumbersDto.SBP;
                WDrequest.WellnessData.DBP = request.healthNumbersDto.DBP;
                WDrequest.WellnessData.UserId = request.ParticipantId;
                WDrequest.WellnessData.UpdatedBy = request.UserId;
                participantReader.AddtoWellnessData(WDrequest);
                AddtoHealthDataRequest healthDataRequest = new AddtoHealthDataRequest();
                healthDataRequest.HealthData = new HealthDataDto();
                healthDataRequest.HealthData.UserId = request.ParticipantId;
                healthDataRequest.HealthData.Weight = request.healthNumbersDto.Weight.Value;
                healthDataRequest.HealthData.Source = (int)HealthDataSource.FollowUp;
                healthDataRequest.HealthData.CreatedBy = request.UserId;
                healthDataRequest.HealthData.CreatedOn = DateTime.UtcNow;
                participantReader.AddtoHealthData(healthDataRequest);
            }
            return response;
        }

        public ReadFUHealthNumberResponse ReadFUHealthNumber(ReadFUHealthNumberRequest request)
        {
            ReadFUHealthNumberResponse response = new ReadFUHealthNumberResponse();
            var healthNumber = context.FollowUp_HealthNumbers.Where(x => x.Id == request.followupId).FirstOrDefault();
            response.FollowUp_HealthNumbersDto = Utility.mapper.Map<DAL.FollowUp_HealthNumbers, FollowUp_HealthNumbersDto>(healthNumber);
            return response;
        }

        public FollowUpResponse AddEditOtherRiskFactors(AddEditFUOtherRisksRequest request, string DTCOrgCode)
        {
            FollowUpResponse response = new FollowUpResponse();
            var RiskFactors = Utility.mapper.Map<FollowUp_OtherRiskFactorsDto, DAL.FollowUp_OtherRiskFactors>(request.otherRiskFactors);
            var currentRiskFactors = context.FollowUp_OtherRiskFactors.Where(x => x.Id == request.otherRiskFactors.Id).FirstOrDefault();
            var clonedRiskFactors = CloneUtil.DeepClone<DAL.FollowUp_OtherRiskFactors>(currentRiskFactors);
            if (currentRiskFactors != null)
            {
                context.Entry(currentRiskFactors).CurrentValues.SetValues(RiskFactors);
                UserHistoryReader.LogUserChanges(clonedRiskFactors, RiskFactors, request.ParticipantId, request.UserId, UserHistoryCategoryDto.FollowUp);
            }
            else
                context.FollowUp_OtherRiskFactors.Add(RiskFactors);

            context.SaveChanges();
            response.FollowUpDto = UpdateFollowUp(new UpdateFollowUpRequest { pageName = PageName.OR, followUpId = request.otherRiskFactors.Id, userId = request.ParticipantId, portalId = request.PortalId, updatedBy = request.UserId, isIntuityUser = request.IsIntuityUser, uniqueId = request.UniqueId, OrganizationCode = request.OrganizationCode }, DTCOrgCode).FollowUpDto;
            return response;
        }

        public ReadFollowUpOtherRiskFactorsResponse ReadOtherRiskFactor(ReadFollowUpRequest request)
        {
            ReadFollowUpOtherRiskFactorsResponse response = new ReadFollowUpOtherRiskFactorsResponse();
            var healthNumber = context.FollowUp_OtherRiskFactors.Where(x => x.Id == request.FollowUpId).FirstOrDefault();
            response.otherRisks = Utility.mapper.Map<DAL.FollowUp_OtherRiskFactors, FollowUp_OtherRiskFactorsDto>(healthNumber);
            return response;
        }

        public void GenFollowUpGoals(int followupId)
        {
            StoredProcedures sp = new StoredProcedures();
            sp.GenFollowUpGoals(followupId);
        }

        public void UpdateFUHealthNumbersfromLab(UpdateFUHealthNumbersfromLabRequest request)
        {
            HRAReader hraReader = new HRAReader();
            AddEditFUHealthNumbersRequest addrequest = new AddEditFUHealthNumbersRequest();
            var validityResponse = CheckFollowupValidity(request.UsersInProgramsId, request.Lab, null);
            if (validityResponse != null && validityResponse.isValid)
            {
                var followup_healthNumbersDAL = context.FollowUp_HealthNumbers.Where(x => x.Id == validityResponse.followUpId).FirstOrDefault();
                if (followup_healthNumbersDAL != null)
                {
                    //Lab values
                    if (!request.doNotOverrideLab)
                    {
                        followup_healthNumbersDAL.HDL = request.Lab.HDL;
                        followup_healthNumbersDAL.LDL = request.Lab.LDL;
                        followup_healthNumbersDAL.TotalChol = request.Lab.TotalChol;
                        followup_healthNumbersDAL.Trig = request.Lab.Trig;
                        followup_healthNumbersDAL.A1C = request.Lab.A1C;
                        followup_healthNumbersDAL.Glucose = request.Lab.Glucose;
                        followup_healthNumbersDAL.DidYouFast = request.Lab.DidYouFast;
                        followup_healthNumbersDAL.BloodTestDate = request.Lab.BloodTestDate;
                    }
                    //Biometric
                    if (request.Lab.Height.HasValue)
                        followup_healthNumbersDAL.Height = request.Lab.Height;
                    if (request.Lab.Weight.HasValue)
                        followup_healthNumbersDAL.Weight = request.Lab.Weight;
                    if (request.Lab.BMI.HasValue)
                        followup_healthNumbersDAL.BMI = request.Lab.BMI;
                    if (request.Lab.Waist.HasValue)
                        followup_healthNumbersDAL.Waist = request.Lab.Waist;
                    if (request.Lab.SBP.HasValue && request.Lab.DBP.HasValue)
                    {
                        followup_healthNumbersDAL.SBP = request.Lab.SBP;
                        followup_healthNumbersDAL.DBP = request.Lab.DBP;
                        followup_healthNumbersDAL.BPArm = request.Lab.BPArm;
                    }
                    //Lab id
                    followup_healthNumbersDAL.LabId = request.Lab.Id;
                    context.FollowUp_HealthNumbers.Attach(followup_healthNumbersDAL);
                    context.Entry(followup_healthNumbersDAL).State = EntityState.Modified;
                    context.SaveChanges();
                    var followup = context.FollowUps.Where(x => x.Id == validityResponse.followUpId).FirstOrDefault();
                    GenFollowUpGoals(followup.Id);
                    //update additionalLab
                    LabReader labreader = new LabReader();
                    labreader.UpdateAdditionalLab(request.Lab.Id);
                }
            }
        }

        public CheckFollowupValidityResponse CheckFollowupValidity(int usersInProgramsId, LabDto lab = null, DateTime? bloodTestDate = null)
        {
            CheckFollowupValidityResponse response = new CheckFollowupValidityResponse();
            var followUpList = context.FollowUps.Include("UsersinProgram").Include("UsersinProgram.User").Include("UsersinProgram.User.Organization")
                    .Include("UsersinProgram.ProgramsinPortal").Include("UsersinProgram.ProgramsinPortal.Portal").Include("FollowUp_HealthNumbers")
                    .Where(x => x.UsersinProgramsId == usersInProgramsId && x.CompleteDate != null).OrderByDescending(x => x.Id).ToList();
            var compfollowUpCount = followUpList.Count();
            var followUp = followUpList.OrderByDescending(x => x.Id).FirstOrDefault();
            if (followUp != null)
            {
                UsersinProgram usersinProgram = null;
                HRA hra = null;
                if (lab != null)
                {
                    usersinProgram = context.UsersinPrograms.Include("FollowUps").Include("FollowUps.FollowUp_HealthNumbers").Where(x => x.Id == followUp.UsersinProgramsId
                    && x.FollowUps.Any(y => (y.FollowUp_HealthNumbers != null && y.FollowUp_HealthNumbers.LabId == lab.Id) && y.Id != followUp.Id)).FirstOrDefault();

                    hra = context.HRAs.Include("HRA_HealthNumbers").Where(x => x.UserId == followUp.UsersinProgram.UserId && x.HRA_HealthNumbers != null && x.HRA_HealthNumbers.LabId == lab.Id).FirstOrDefault();

                }
                else if (bloodTestDate != null)
                {
                    usersinProgram = context.UsersinPrograms.Include("FollowUps").Include("FollowUps.FollowUp_HealthNumbers").Where(x => x.Id == followUp.UsersinProgramsId
                    && x.FollowUps.Any(y => (y.FollowUp_HealthNumbers != null && y.FollowUp_HealthNumbers.BloodTestDate.HasValue && y.FollowUp_HealthNumbers.BloodTestDate.Value == bloodTestDate)
                    && y.Id != followUp.Id)).FirstOrDefault();

                    hra = context.HRAs.Include("HRA_HealthNumbers").Where(x => x.UserId == followUp.UsersinProgram.UserId && x.HRA_HealthNumbers != null && x.HRA_HealthNumbers.BloodTestDate == bloodTestDate).FirstOrDefault();
                }
                if (usersinProgram == null && hra == null)
                {
                    if (!bloodTestDate.HasValue)
                        bloodTestDate = lab.BloodTestDate;
                    TimeSpan diff = followUp.CompleteDate.Value.Subtract(bloodTestDate.Value);
                    if (followUp.UsersinProgram.User.Organization.IntegrationWith == (byte)IntegrationPartner.Captiva)
                    {
                        var HRA = context.HRAs.Include("HRA_HealthNumbers").Where(x => x.UserId == followUp.UsersinProgram.UserId && x.HRA_HealthNumbers != null).FirstOrDefault();
                        if ((followUp.UsersinProgram.AssignedFollowUp == 1 && HRA.CompleteDate.Value.AddDays(28) < bloodTestDate.Value && diff.Days >= -followUp.UsersinProgram.ProgramsinPortal.Portal.FollowUpValidity)
                            || (followUp.UsersinProgram.AssignedFollowUp == 2 && compfollowUpCount == 2 && followUp.CompleteDate.Value.AddDays(-168) < bloodTestDate.Value && diff.Days >= -followUp.UsersinProgram.ProgramsinPortal.Portal.FollowUpValidity))
                        {
                            response.followUpId = followUp.Id;
                            response.isValid = true;
                        }
                        else
                            response.isValid = false;
                    }
                    else if (diff.Days > followUp.UsersinProgram.ProgramsinPortal.Portal.FollowUpValidity || diff.Days < -followUp.UsersinProgram.ProgramsinPortal.Portal.FollowUpValidity)
                        response.isValid = false;
                    else
                    {
                        response.followUpId = followUp.Id;
                        response.isValid = true;
                    }
                }
            }
            return response;
        }

        public GetAllFollowUpsResponse GetAllFollowUps(GetAllFollowUpsRequest request)
        {
            GetAllFollowUpsResponse response = new GetAllFollowUpsResponse();
            var followUps = context.FollowUps.Include("UsersinProgram").Include("UsersinProgram.ProgramsinPortal").Where(x => x.UsersinProgram.UserId == request.UserId).ToList();
            response.FollowUps = Utility.mapper.Map<IList<DAL.FollowUp>, IList<FollowUpDto>>(followUps);
            return response;
        }
    }
}