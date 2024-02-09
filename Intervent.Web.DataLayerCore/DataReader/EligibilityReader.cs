using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class EligibilityReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public const string Program_Ineligible = "Program_Ineligible";

        public void BulkAddEditIntuityEligibility(List<IntuityEligibilityDto> request)
        {
            if (request.FindAll(x => x.Id.HasValue).Count() > 1)
            {
                using (var scope = new System.Transactions.TransactionScope())
                {
                    /*using (var context1 = new InterventDatabase())
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (IntuityEligibilityDto req in request.Where(x => x.Id.HasValue))
                        {
                            var intuityeligibilityDbModel = context1.IntuityEligibilities.Where(x => x.Id == req.Id).FirstOrDefault();
                            var intuityeligibility = req.MapToIntuityEligibility(intuityeligibilityDbModel);
                            context1.Entry(intuityeligibilityDbModel).CurrentValues.SetValues(intuityeligibility);
                        }
                        context1.SaveChanges();
                    }*/

                    scope.Complete();
                }
            }

            //insert
            if (request.FindAll(x => !x.Id.HasValue).Count() > 1)
            {
                using (var scope = new System.Transactions.TransactionScope())
                {
                    /*using (var context1 = new InterventDatabase())
                    {
                        //context1.Configuration.AutoDetectChangesEnabled = false;
                        foreach (IntuityEligibilityDto req in request.Where(x => !x.Id.HasValue))
                        {
                            var intuityeligibility = req.MapToIntuityEligibility(null);
                            context1.IntuityEligibilities.Add(intuityeligibility);
                        }
                        context1.SaveChanges();
                    }*/
                    scope.Complete();
                }
            }
        }

        public bool IsDiabeticByClaims(string uniqueId, int orgId)
        {
            var claim = context.InsuranceSummaries.Include("CandidateReasonForLastChanges").Where(x => x.UniqueID == uniqueId && x.OrganizationId == orgId
            && x.CandidateReasonForLastChanges.Where(y => y.ConditionType == "DIAB").Count() > 0).FirstOrDefault();
            if (claim != null)
                return true;
            return false;
        }

        public void BulkEditFulfillment(List<IntuityFulfillments> fulfillmentList)
        {
            //edit
            using (var scope = new System.Transactions.TransactionScope())
            {
                /* using (var context1 = new InterventDatabase())
                 {
                     //context1.Configuration.AutoDetectChangesEnabled = false;
                     foreach (IntuityFulfillments req in fulfillmentList.Where(x => x.Id != 0))
                     {
                         var intuityfulfillmentDbModel = context1.IntuityFulfillments.Where(x => x.Id == req.Id).FirstOrDefault();
                         context1.Entry(intuityfulfillmentDbModel).CurrentValues.SetValues(req);
                     }
                     context1.SaveChanges();
                 }*/
                scope.Complete();
            }

            //insert
            using (var scope = new System.Transactions.TransactionScope())
            {
                /*using (var context1 = new InterventDatabase())
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    IntuityFulfillmentsDto intuityFulfillment = new IntuityFulfillmentsDto();
                    foreach (IntuityFulfillments req in fulfillmentList.Where(x => x.Id == 0))
                    {
                        var fulfillment = intuityFulfillment.MapToIntuityFulfillment(req);
                        context1.IntuityFulfillments.Add(fulfillment);
                    }
                    context1.SaveChanges();
                }*/
                scope.Complete();
            }
        }

        public void BulkEditEligibility(List<IntuityEligibility> eligibilityList)
        {
            //insert
            using (var scope = new System.Transactions.TransactionScope())
            {
                /*using (var context1 = new InterventDatabase())
                {
                    //context1.Configuration.AutoDetectChangesEnabled = false;
                    foreach (IntuityEligibility req in eligibilityList)
                    {

                        var intuityeligibilityDbModel = context1.IntuityEligibilities.Where(x => x.Id == req.Id).FirstOrDefault();
                        context1.Entry(intuityeligibilityDbModel).CurrentValues.SetValues(req);
                    }
                    context1.SaveChanges();
                }*/
                scope.Complete();
            }
        }

        public IntuityShipmentResponse GetLatestEligibilityByUniqueId(string uniqueId, int orgId, string SoNbr)
        {
            IntuityShipmentResponse response = new IntuityShipmentResponse();
            //TODO: need to include organization
            var intuityeligibilityDbModel = context.IntuityEligibilities.Include("IntuityFulfillments").Where(x => x.UniqueId == uniqueId && x.OrganizationId == orgId).FirstOrDefault();
            if (intuityeligibilityDbModel != null)
            {
                response.Eligibility = intuityeligibilityDbModel;
                response.Fulfillment = intuityeligibilityDbModel.IntuityFulfillments.Where(x => x.SoNbr == SoNbr).FirstOrDefault();
            }
            return response;

        }

        public GetEligAndIntuityEligByPortalResponse GetEligAndIntuityEligByPortal(int orgId, int portalId)
        {
            GetEligAndIntuityEligByPortalResponse response = new GetEligAndIntuityEligByPortalResponse();
            response.IntuityEligibilities = new List<IntuityEligibilityDto>();
            //All active eligibility for active portal
            var eligibilityList = context.Eligibilities.Where(e => e.PortalId == portalId).ToList();
            response.Eligibilities = Utility.mapper.Map<IList<DAL.Eligibility>, IList<EligibilityDto>>(eligibilityList);
            //All intuityeligibility for org
            var intuityEligList = context.IntuityEligibilities.Where(x => x.OrganizationId == orgId).ToList();
            if (intuityEligList.Count() > 0)
            {
                foreach (var intuityElig in intuityEligList)
                    response.IntuityEligibilities.Add(IntuityEligibilityDto.MapToIntuityEligibilityDto(intuityElig));
            }
            return response;
        }

        public List<IntuityEligibilityStatusCsvModel> GetIntuityEligibilityList(OrganizationDto org)
        {
            List<IntuityEligibilityStatusCsvModel> eligibilityList = new List<IntuityEligibilityStatusCsvModel>();
            List<IntuityEligibility> intuityEligibilityData;
            var currentPortal = org.Portals.Where(x => x.Active == true && x.EligtoIntuity.HasValue).FirstOrDefault();
            CommonReader _commonReader = new CommonReader();
            var States = _commonReader.ListAllStates().States;
            var Countries = _commonReader.ListCountries(new ListCountriesRequest()).Countries;
            intuityEligibilityData = context.IntuityEligibilities
                                    .Where(x => x.OrganizationId == org.Id.Value && !x.OptingOut.HasValue).ToList();
            if (intuityEligibilityData != null && intuityEligibilityData.Count() > 0)
            {
                DateTime ChildAge = DateTime.UtcNow.AddYears(-13);
                foreach (var data in intuityEligibilityData)
                {
                    var eligibility = context.Eligibilities.Where(x => x.UniqueId == data.UniqueId && x.PortalId == currentPortal.Id && x.DOB < ChildAge).FirstOrDefault();
                    if (eligibility != null)
                    {
                        #region eligibility
                        IntuityEligibilityStatusCsvModel eligibilityModel = new IntuityEligibilityStatusCsvModel();
                        eligibilityModel.PatientUniqueId = org.Code + "-" + data.UniqueId;
                        eligibilityModel.BenefitHolderId = org.Code + "-" + eligibility.EmployeeUniqueId;
                        eligibilityModel.Firstname = eligibility.FirstName;
                        eligibilityModel.Lastname = eligibility.LastName;
                        eligibilityModel.Middlename = eligibility.MiddleName;
                        eligibilityModel.EmailAddress = eligibility.Email;
                        eligibilityModel.Dob = eligibility.DOB;
                        if (eligibility.Gender.HasValue)
                        {
                            eligibilityModel.Gender = GenderDto.GetByKey(eligibility.Gender.Value);
                        }
                        eligibilityModel.Address1 = eligibility.Address;
                        eligibilityModel.Address2 = eligibility.Address2;
                        eligibilityModel.City = eligibility.City;
                        eligibilityModel.State = States.Where(x => x.Code == eligibility.State && x.CountryId == Countries.Where(c => c.UNCode == eligibility.Country).Select(c => c.Id).FirstOrDefault()).Select(x => x.Name).FirstOrDefault();
                        eligibilityModel.Country = Countries.Where(c => c.UNCode == eligibility.Country).Select(c => c.Name).FirstOrDefault();
                        if (!string.IsNullOrEmpty(eligibility.CellNumber))
                        {
                            eligibilityModel.Phone = eligibility.CellNumber;
                        }
                        else if (!string.IsNullOrEmpty(eligibility.HomeNumber))
                        {
                            eligibilityModel.Phone = eligibility.HomeNumber;
                        }
                        else if (!string.IsNullOrEmpty(eligibility.WorkNumber))
                        {
                            eligibilityModel.Phone = eligibility.WorkNumber;
                        }
                        eligibilityModel.Zip = eligibility.Zip;
                        eligibilityModel.UserEnrollmentType = eligibility.UserEnrollmentType == "C" || eligibility.UserEnrollmentType == "D" ? "AD" : eligibility.UserEnrollmentType;
                        eligibilityModel.HireDate = eligibility.HireDate;
                        eligibilityModel.TerminationDate = eligibility.TerminatedDate;
                        eligibilityModel.UserStatus = eligibility.UserStatus;
                        var _businessUnit = _commonReader.ReadBusinessUnit(new ReadBusinessUnitRequest() { name = eligibility.BusinessUnit }).businessUnit;
                        eligibilityModel.BusinessUnit = _businessUnit != null ? _businessUnit.Code : "";
                        eligibilityModel.RegionCode = eligibility.RegionCode;
                        eligibilityModel.UnionFlag = eligibility.UnionFlag;
                        eligibilityModel.PayType = eligibility.PayType;
                        eligibilityModel.MedicalPlanCode = eligibility.MedicalPlanCode;
                        eligibilityModel.MedicalPlanEndDate = eligibility.MedicalPlanEndDate;
                        eligibilityModel.MedicalPlanStartDate = eligibility.MedicalPlanStartDate;
                        eligibilityModel.TobaccoFlag = eligibility.TobaccoFlag;
                        #endregion

                        eligibilityList.Add(eligibilityModel);
                    }
                }
            }
            return eligibilityList;
        }

        public Eligibility GetEligibility(string uniqueId, int portalId)
        {
            return context.Eligibilities.Where(x => x.UniqueId == uniqueId && x.PortalId == portalId).FirstOrDefault();
        }
    }
}
