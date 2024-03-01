using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;
using System.Text;

namespace Intervent.Web.DataLayer
{

    public class ClaimsReader : BaseDataReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public ListClaimsConditionsResponse GetConditionsList(GetCandidateConditionListRequest request)
        {
            using (InterventDatabase ctx = GetContext)
            {
                ListClaimsConditionsResponse response = new ListClaimsConditionsResponse();
                //dont know why this doesnt work
                // response.Candidateconditions = Utility.mapper.Map<IList<DAL.Candidatecondition>, IList<CandidateconditionsDto>>(conditions);
                var conditionsCost = ctx.CandidateConditions.Include("InsuranceSummary").Where(x => x.InsuranceSummary.UniqueID == request.UniqueId && x.InsuranceSummary.OrganizationId == request.Organizationid).OrderByDescending(x => x.ConditionDate).ToList();
                List<CandidateConditionsDto> dtos = new List<CandidateConditionsDto>();
                foreach (DAL.CandidateCondition condition in conditionsCost)
                {
                    // DTO.CandidateConditionsDto dto = Utility.mapper.Map<DAL.CandidateCondition, DTO.CandidateConditionsDto>(condition);
                    var dto = new DTO.CandidateConditionsDto();
                    dto.ClaimsID = condition.ClaimsID;
                    dto.ConditionDate = condition.ConditionDate;
                    dto.ConditionName = condition.ConditionName;
                    dto.ConditionType = condition.ConditionType;
                    dto.BilledAmount = condition.BilledAmount;
                    dtos.Add(dto);
                }
                response.CandidateConditionsCost = dtos;

                var claimIds = ctx.InsuranceSummaries.Where(x => x.UniqueID == request.UniqueId && x.OrganizationId == request.Organizationid).Select(x => x.ID).Distinct().ToList();

                List<CandidateReasonForLastChangeDto> conditionDtos = new List<CandidateReasonForLastChangeDto>();
                if (claimIds != null && claimIds.Count() > 0)
                {
                    var conditions = ctx.CandidateReasonForLastChanges.Where(x => x.ClaimsId.HasValue && claimIds.Contains(x.ClaimsId.Value)).OrderByDescending(x => x.ConditionDate).ToList();
                    foreach (DAL.CandidateReasonForLastChange condition in conditions)
                    {
                        if (!conditionDtos.Any(x => x.ConditionType == condition.ConditionType))
                        {
                            var dto = new DTO.CandidateReasonForLastChangeDto();
                            dto.ConditionType = condition.ConditionType;
                            dto.ClaimsId = condition.ClaimsId.Value;
                            var codes = ctx.ClaimConditionCodes.Where(x => x.ClaimsID == dto.ClaimsId && x.Condition == dto.ConditionType).OrderByDescending(x => x.ConditionDate).ToList();
                            dto.ConditionDate = conditions.Where(x => x.ConditionType == dto.ConditionType && x.ClaimsId == dto.ClaimsId).OrderBy(x => x.ID).FirstOrDefault().ConditionDate;
                            dto.RecentConditionDate = codes.FirstOrDefault()?.ConditionDate;
                            var codeString = new StringBuilder();
                            foreach (var code in codes)
                            {
                                codeString.AppendLine(code.Code + ":" + code.CodeDescription + " on " + (code.ConditionDate.HasValue ? code.ConditionDate.Value.ToShortDateString() : ""));
                            }
                            dto.ConditionCode = codeString.ToString();
                            conditionDtos.Add(dto);
                        }
                    }
                }

                response.InsuranceSummary = conditionDtos;
                return response;
            }
        }

        public ListClaimsMedicationResponse GetMedicationsList(GetCandidateMedicationListRequest request)
        {
            using (InterventDatabase ctx = GetContext)
            {
                var medications = ctx.CandidateMedications.Include("InsuranceSummary").Where(x => x.InsuranceSummary.UniqueID == request.UniqueId && x.InsuranceSummary.OrganizationId == request.Organizationid).OrderByDescending(x => x.MedicationDate).ToList();
                ListClaimsMedicationResponse response = new ListClaimsMedicationResponse();
                //dont know why this doesnt work
                // response.CandidateMedications = Utility.mapper.Map<IList<DAL.CandidateMedication>, IList<CandidateMedicationsDto>>(medications);
                List<CandidateMedicationsDto> dtos = new List<CandidateMedicationsDto>();
                foreach (DAL.CandidateMedication medication in medications)
                {
                    var dto = new DTO.CandidateMedicationsDto();
                    dto.ClaimsId = medication.ClaimsID;
                    dto.CMRef = medication.Id;
                    dto.MedicationDate = medication.MedicationDate;
                    dto.MedicationName = medication.MedicationName;
                    dto.MedicationType = medication.MedicationType;
                    if (medication.Total_Amount_Paid_by_All_Source.HasValue)
                        dto.Total_Amount_Paid_by_All_Source = Math.Round((medication.Total_Amount_Paid_by_All_Source.Value), 2);
                    dtos.Add(dto);
                }
                response.CandidateMedications = dtos;
                return response;
            }
        }

        public IEnumerable<DiagnosisCodeDto> GetDiagnosisCodes()
        {
            return DiagnosisCodeDto.GetAll();
        }

        public ListClaimProcessEligibilityResponse GetClaimsProcessEligibilityList(ListClaimProcessEligibilityRequest request)
        {

            ListClaimProcessEligibilityResponse response = new ListClaimProcessEligibilityResponse();
            List<string> userEnrollmentTypes = new List<string>();
            userEnrollmentTypes.Add("E");
            userEnrollmentTypes.Add("S");
            userEnrollmentTypes.Add("C");
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.Eligibility = (from e in ctx.Eligibilities.Where(x => request.PortalId == x.PortalId && userEnrollmentTypes.Contains(x.UserEnrollmentType) && x.UserStatus != "T")
                                        join p in ctx.Portals on e.PortalId equals p.Id
                                        join o in ctx.Organizations on p.OrganizationId equals o.Id
                                        where !ctx.ClaimLoadExcludeId.Any(x => x.Id == e.Id && x.TableName == "ELIGIBILITY")
                                        select new ClaimProcessEligibilityDto()
                                        {
                                            UniqueID = e.UniqueId,
                                            PortalId = e.PortalId,
                                            DateOfBirth = e.DOB,
                                            FirstName = e.FirstName,
                                            LastName = e.LastName,
                                            SSN = e.SSN,
                                            OrgId = p.OrganizationId,
                                            UserStatus = e.UserStatus,
                                            BusinessUnit = e.BusinessUnit,
                                            UserEnrollmentType = e.UserEnrollmentType,
                                            MedicalPlanCode = e.MedicalPlanCode,
                                            HomeNumber = e.HomeNumber,
                                            Email = e.Email,
                                            Address = e.Address,
                                            Address2 = e.Address2,
                                            City = e.City,
                                            State = e.State,
                                            Zip = e.Zip,
                                            OrgName = o.Name
                                        }).Distinct().ToList();
            }
            return response;
        }

        //Claim Analytics
        public ListClaimProcessEligibilityResponse GetEligibilityList()
        {

            ListClaimProcessEligibilityResponse response = new ListClaimProcessEligibilityResponse();
            List<string> userEnrollmentTypes = new List<string>();
            userEnrollmentTypes.Add("E");
            userEnrollmentTypes.Add("S");
            userEnrollmentTypes.Add("C");
            var create = Convert.ToDateTime("2019-07-01");
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.Eligibility = (from e in ctx.Eligibilities.Where(x => userEnrollmentTypes.Contains(x.UserEnrollmentType)
                                        && x.UserStatus != "T" /*&& x.CreateDate == create*/) //add create date when running claim analytics
                                        select new ClaimProcessEligibilityDto()
                                        {
                                            UniqueID = e.UniqueId,
                                            DateOfBirth = e.DOB,
                                            FirstName = e.FirstName,
                                            LastName = e.LastName,
                                            SSN = e.SSN,
                                            OrgId = 47,
                                            UserStatus = e.UserStatus,
                                            BusinessUnit = e.BusinessUnit,
                                            UserEnrollmentType = e.UserEnrollmentType,
                                            MedicalPlanCode = e.MedicalPlanCode,
                                            HomeNumber = e.HomeNumber,
                                            Email = e.Email,
                                            Address = e.Address,
                                            Address2 = e.Address2,
                                            City = e.City,
                                            State = e.State,
                                            Zip = e.Zip,
                                            OrgName = "Compass Group"
                                        }).Distinct().ToList();
            }
            return response;
        }

        //needed for Claims process to update files from cvs caremark
        public ListClaimProcessCrothalIDResponse GetClaimsCrothalIDs()
        {
            ListClaimProcessCrothalIDResponse response = new ListClaimProcessCrothalIDResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.CrothalIds = (from c in ctx.CrothalIDChange
                                       select new CrothalIdDto()
                                       {
                                           FirstName = c.FirstName,
                                           LastName = c.LastName,
                                           NewUniqueId = c.NewUniqueId,
                                           OldUniqueId = c.OldUniqueId
                                       }).ToHashSet();

            }
            return response;
        }

        public ListClaimProcessClaimCodeResponse GetClaimsCodes()
        {
            ListClaimProcessClaimCodeResponse response = new ListClaimProcessClaimCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.ClaimCodes = (from c in ctx.ClaimCodes.Where(x => x.Code != null && x.Code != "")
                                       select new ClaimCodeDto()
                                       {
                                           Code = c.Code,
                                           CodeFlag = c.CodeFlag,
                                           CodeSource = c.CodeSource,
                                           CodeDescription = c.CodeDescription
                                       }).Distinct().ToHashSet();

            }
            return response;
        }

        public ListClaimProcessInsuranceSummaryResponse GetInsuranceResultSummaries(int orgId)
        {
            ListClaimProcessInsuranceSummaryResponse response = new ListClaimProcessInsuranceSummaryResponse();

            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.InsuranceSummaries = (from irs in ctx.InsuranceSummaries.Include("CandidateReasonForLastChanges")
                                               join p in ctx.Portals on irs.OrganizationId equals p.OrganizationId
                                               where p.Active && orgId == p.OrganizationId &&
                                               !ctx.ClaimLoadExcludeId.Any(x => x.Id == irs.ID && x.TableName == "INSURANCESUMMARY")
                                               select new ClaimProcessInsuranceSummaryDto()
                                               {
                                                   Id = irs.ID,
                                                   UniqueId = irs.UniqueID,
                                                   PortalId = p.Id,
                                                   OrgId = p.OrganizationId,
                                                   HasCKD = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "CKD"),
                                                   NoIVClaimCondition = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "OTHER"),
                                                   HasHeartDisorder = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "CV"),
                                                   HasHyperTension = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "HYPTEN"),
                                                   HasLungDisorder = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "LUNG"),
                                                   HasSleepingDisorder = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "SLEEP"),
                                                   IsDiabetic = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "DIAB"),
                                                   IsObese = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "OBESE"),
                                                   IsPregnant = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "PREG"),
                                                   IsSmoking = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "SMOKE"),
                                                   IsDiabeticLivongo = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "DIABLV"),
                                                   HasPrediabetes = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "PREDIAB"),
                                                   LatestPregnancyDate = irs.CandidateReasonForLastChanges.Any(x => x.ConditionType == "PREG") ? irs.CandidateReasonForLastChanges.Where(x => x.ConditionType == "PREG").OrderByDescending(x => x.ID).FirstOrDefault().ConditionDate : null,
                                                   HRA = irs.HRA
                                               }).ToHashSet();
            }
            return response;
        }

        public ListClaimProcessEnrolledDataResponse GetClaimProcessEnrolledDataUniqueIds(int portalId)
        {
            ListClaimProcessEnrolledDataResponse response = new ListClaimProcessEnrolledDataResponse();
            List<string> userEnrollmentTypes = new List<string>();
            userEnrollmentTypes.Add("E");
            userEnrollmentTypes.Add("S");
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.EnrolledDatas = (from e in ctx.Eligibilities.Where(x => portalId == x.PortalId && userEnrollmentTypes.Contains(x.UserEnrollmentType) && x.UserStatus != "T")
                                          join u in ctx.Users on e.UniqueId equals u.UniqueId
                                          join up in ctx.UsersinPrograms on u.Id equals up.UserId
                                          join pp in ctx.ProgramsinPortals.Where(x => portalId == x.PortalId) on up.ProgramsinPortalsId equals pp.Id
                                          join p in ctx.Programs on pp.ProgramId equals p.Id
                                          select new ClaimProcessEnrolledDataDto()
                                          {
                                              UniqueId = e.UniqueId,
                                              EnrollType = p.ProgramType == 1 ? "S" : "C",
                                              PortalId = e.PortalId
                                          }).Distinct().ToHashSet();
            }
            return response;
        }

        public ListClaimProcessEnrolledDataResponse GetEbenClaimProcessEnrolledDataUniqueIds(int portalId)
        {
            ListClaimProcessEnrolledDataResponse response = new ListClaimProcessEnrolledDataResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.EnrolledDatas = (from e in ctx.Eligibilities.Where(x => portalId == x.PortalId && x.UserStatus != "T")
                                          join u in ctx.Users on e.UniqueId equals u.UniqueId
                                          join up in ctx.UsersinPrograms on u.Id equals up.UserId
                                          join pp in ctx.ProgramsinPortals.Where(x => portalId == x.PortalId) on up.ProgramsinPortalsId equals pp.Id
                                          join p in ctx.Programs on pp.ProgramId equals p.Id
                                          select new ClaimProcessEnrolledDataDto()
                                          {
                                              UniqueId = e.UniqueId,
                                              EnrollType = p.ProgramType == 1 ? "S" : "C",
                                              PortalId = e.PortalId
                                          }).Distinct().ToHashSet();
            }
            return response;
        }

        public ListClaimProcessHRAResponse GetClaimProcessHRAUniqueIds(int portalId)
        {
            ListClaimProcessHRAResponse response = new ListClaimProcessHRAResponse();
            List<string> userEnrollmentTypes = new List<string>();
            userEnrollmentTypes.Add("E");
            userEnrollmentTypes.Add("S");
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.HRAs = (from e in ctx.Eligibilities.Where(x => portalId == x.PortalId && userEnrollmentTypes.Contains(x.UserEnrollmentType) && x.UserStatus != "T")
                                 join u in ctx.Users on e.UniqueId equals u.UniqueId
                                 join h in ctx.HRAs on u.Id equals h.UserId
                                 select new ClaimProcessHRADto()
                                 {
                                     UniqueId = e.UniqueId,
                                     PortalId = e.PortalId
                                 }).Distinct().ToHashSet();
            }
            return response;
        }

        public ListClaimProcessHRAResponse GetEbenClaimProcessHRAUniqueIds(int portalId)
        {
            ListClaimProcessHRAResponse response = new ListClaimProcessHRAResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.HRAs = (from e in ctx.Eligibilities.Where(x => portalId == x.PortalId && x.UserStatus != "T")
                                 join u in ctx.Users on e.UniqueId equals u.UniqueId
                                 join h in ctx.HRAs on u.Id equals h.UserId
                                 select new ClaimProcessHRADto()
                                 {
                                     UniqueId = e.UniqueId,
                                     PortalId = e.PortalId
                                 }).Distinct().ToHashSet();
            }
            return response;
        }

        public ListClaimProcessTherapeuticClassCodeResponse GetTherapeuticClassCodes()
        {
            ListClaimProcessTherapeuticClassCodeResponse response = new ListClaimProcessTherapeuticClassCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.TherapeuticClassCodes = (from t in ctx.TherapeuticClassCodes
                                                  select new ClaimProcessTherapeuticClassCodeDto()
                                                  {
                                                      TheraCode = t.TheraCode,
                                                      DrugCategory = t.DrugCategory
                                                  }).ToHashSet();

            }
            return response;
        }
        //
        public ListClaimProcessLivongoICDCodeResponse GetLivongoICDCodes()
        {
            ListClaimProcessLivongoICDCodeResponse response = new ListClaimProcessLivongoICDCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.ICDCodes = (from t in ctx.LivongoICDCodess.Where(x => x.IsActive == true)
                                     select new LivongoICDCodesDto()
                                     {
                                         Code = t.LvCode,
                                         CodeSource = t.LvCodeSource,
                                         CodeDescription = t.LvCodeDescription
                                     }).ToHashSet();

            }
            return response;
        }

        public ListClaimProcessLivongoNDCCodeResponse GetLivongoNDCCodes()
        {
            ListClaimProcessLivongoNDCCodeResponse response = new ListClaimProcessLivongoNDCCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.NDCCodes = (from t in ctx.LivongoNDCCodes.Where(x => x.IsActive == true)
                                     select new LivongoNDCCodesDto()
                                     {
                                         Code = t.LvNDCCode
                                     }).ToHashSet();

            }
            return response;
        }

        public ClaimConditionCodeResponse GetClaimConditionCodes()
        {
            ClaimConditionCodeResponse response = new ClaimConditionCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.ClaimConditionCodes = (from t in ctx.ClaimConditionCodes
                                                select new ClaimConditionCodeDto()
                                                {
                                                    ClaimsID = t.ClaimsID,
                                                    Code = t.Code,
                                                    Condition = t.Condition
                                                }).Distinct().ToHashSet();

            }
            return response;
        }

        public ListClaimProcessClaimCodeResponse GetIVICDCodes()
        {
            ListClaimProcessClaimCodeResponse response = new ListClaimProcessClaimCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.ClaimCodes = (from t in ctx.IVICDCodes
                                       select new ClaimCodeDto()
                                       {
                                           Code = t.Code,
                                           CodeFlag = t.CodeFlag,
                                           CodeSource = t.CodeSource,
                                           CodeDescription = t.CodeDescription
                                       }).ToHashSet();

            }
            return response;
        }

        public ListClaimProcessClaimCodeResponse GetIVNDCCodes()
        {
            ListClaimProcessClaimCodeResponse response = new ListClaimProcessClaimCodeResponse();
            using (InterventDatabase ctx = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
            {
                response.ClaimCodes = (from t in ctx.IVNDCCodes
                                       select new ClaimCodeDto()
                                       {
                                           Code = t.Code,
                                           CodeFlag = t.CodeFlag,
                                           CodeDescription = t.CodeDescription
                                       }).ToHashSet();

            }
            return response;
        }

        public AddOrEditInsuranceSummaryResponse AddOrEditInsuranceSummary(InsuranceSummaryDto request)
        {
            AddOrEditInsuranceSummaryResponse response = new AddOrEditInsuranceSummaryResponse();
            var insuranceSummaries = context.InsuranceSummaries.Where(x => x.UniqueID == request.UniqueID && x.OrganizationId == request.OrganizationId).FirstOrDefault();
            if (insuranceSummaries == null)
            {
                InsuranceSummary result = Utility.mapper.Map<InsuranceSummaryDto, InsuranceSummary>(request);
                context.InsuranceSummaries.Add(result);
                context.SaveChanges();
                insuranceSummaries = context.InsuranceSummaries.Where(x => x.UniqueID == request.UniqueID && x.OrganizationId == request.OrganizationId).FirstOrDefault();
            }
            else
            {
                bool isChange = false;
                if (!string.IsNullOrEmpty(request.EnrollType) && insuranceSummaries.EnrollType != request.EnrollType)
                {
                    insuranceSummaries.EnrollType = request.EnrollType;
                    insuranceSummaries.LastModifiedDate = DateTime.UtcNow;
                    isChange = true;
                }
                if (insuranceSummaries.HRA != request.HRA)
                {
                    insuranceSummaries.HRA = request.HRA;
                    insuranceSummaries.LastModifiedDate = DateTime.UtcNow;
                    isChange = true;
                }
                if (isChange)
                {
                    context.InsuranceSummaries.Attach(insuranceSummaries);
                    context.Entry(insuranceSummaries).State = EntityState.Modified;
                    context.SaveChanges();
                }
            }
            response.InsuranceSummary = Utility.mapper.Map<InsuranceSummary, InsuranceSummaryDto>(insuranceSummaries);
            return response;
        }

        public void BulkEditInsuranceSummary(List<InsuranceSummaryDto> insuranceSummaryDtoList)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    ////context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (InsuranceSummaryDto request in insuranceSummaryDtoList)
                    {
                        var insuranceSummaries = context1.InsuranceSummaries.Where(x => x.UniqueID == request.UniqueID && x.OrganizationId == request.OrganizationId).FirstOrDefault();
                        if (insuranceSummaries == null)
                        {
                            InsuranceSummary result = Utility.mapper.Map<InsuranceSummaryDto, InsuranceSummary>(request);
                            context1.InsuranceSummaries.Add(result);
                        }
                        else
                        {
                            bool isChange = false;
                            if (!string.IsNullOrEmpty(request.EnrollType) && insuranceSummaries.EnrollType != request.EnrollType)
                            {
                                insuranceSummaries.EnrollType = request.EnrollType;
                                insuranceSummaries.LastModifiedDate = DateTime.UtcNow;
                                isChange = true;
                            }
                            if (insuranceSummaries.HRA != request.HRA)
                            {
                                insuranceSummaries.HRA = request.HRA;
                                insuranceSummaries.LastModifiedDate = DateTime.UtcNow;
                                isChange = true;
                            }
                            if (isChange)
                            {
                                context1.InsuranceSummaries.Attach(insuranceSummaries);
                                context1.Entry(insuranceSummaries).State = EntityState.Modified;
                            }
                        }
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }

        }

        public void BulkAddClaimConditionCode(List<ClaimConditionCodeDto> list)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (ClaimConditionCodeDto request in list)
                    {
                        var condition = context1.ClaimConditionCodes.Where(x => x.ClaimsID == request.ClaimsID && x.Code == request.Code && x.Condition == request.Condition && x.ConditionDate == request.ConditionDate && x.CodeDescription == request.CodeDescription).FirstOrDefault();
                        if (condition == null)
                        {
                            ClaimConditionCode result = Utility.mapper.Map<ClaimConditionCodeDto, ClaimConditionCode>(request);
                            context1.ClaimConditionCodes.Add(result);
                        }
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void BulkAddOrEditCandidateMedications(List<CandidateMedicationsDto> list)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (CandidateMedicationsDto request in list)
                    {
                        var medic = context1.CandidateMedications.Where(x => x.ClaimsID == request.ClaimsId && x.MedicationName == request.MedicationName && x.MedicationDate == request.MedicationDate).FirstOrDefault();
                        if (medic == null)
                        {
                            CandidateMedication result = Utility.mapper.Map<CandidateMedicationsDto, CandidateMedication>(request);
                            context1.CandidateMedications.Add(result);
                        }
                        else
                        {
                            medic.MedicationType = request.MedicationType;
                            medic.Net_Amount_Due__Total_Amount_Billed_Paid_ = request.Net_Amount_Due__Total_Amount_Billed_Paid_;
                            medic.Patient_Pay_Amount = request.Patient_Pay_Amount;
                            medic.Total_Amount_Paid_by_All_Source = request.Total_Amount_Paid_by_All_Source;
                            context1.CandidateMedications.Attach(medic);
                            context1.Entry(medic).State = EntityState.Modified;
                        }
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }
        }

        public void BulkAddCandidateCondition(List<CandidateConditionsDto> list)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (CandidateConditionsDto request in list)
                    {
                        var cc = context1.CandidateConditions.Where(x => x.ClaimsID == request.ClaimsID && x.ConditionType == request.ConditionType).FirstOrDefault();
                        if (cc == null)
                        {
                            CandidateCondition result = Utility.mapper.Map<CandidateConditionsDto, CandidateCondition>(request);
                            context1.CandidateConditions.Add(result);
                        }
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }
        }
        public void BulkAddCandidateReasonForLastChanges(List<CandidateReasonForLastChangeDto> reasonForLastChange)
        {
            using (var scope = new System.Transactions.TransactionScope())
            {
                using (var context1 = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption()))
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (CandidateReasonForLastChangeDto request in reasonForLastChange)
                    {
                        var cc = context1.CandidateReasonForLastChanges.Where(x => x.ClaimsId == request.ClaimsId && x.ConditionType == request.ConditionType).FirstOrDefault();
                        if (cc == null)
                        {
                            CandidateReasonForLastChange result = Utility.mapper.Map<CandidateReasonForLastChangeDto, CandidateReasonForLastChange>(request);
                            context1.CandidateReasonForLastChanges.Add(result);
                        }
                    }
                    context1.SaveChanges();
                }
                scope.Complete();
            }

        }
    }
}