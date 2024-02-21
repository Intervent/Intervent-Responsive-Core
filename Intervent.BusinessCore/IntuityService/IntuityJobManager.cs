using CsvHelper;
using Intervent.Business.Mail;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text;
using static Intervent.HWS.IntuityEligibilityLogAPIRequest;
using static Intervent.HWS.SendPatternDetailsRequest;

namespace Intervent.Business
{
    public class IntuityEligibilityManager : BaseManager
    {
        IntuityReader _intuityReader = new IntuityReader();
        EligibilityReader _eligibilityReader = new EligibilityReader();
        AccountReader _accountReader = new AccountReader();
        PortalReader _portalReader = new PortalReader();
        CommonReader _commonReader = new CommonReader();
        string _intuityFolderPath = ConfigurationManager.AppSettings["IntuityFolderPath"];

        //Folder structure
        public const string ShipmentFolder = "Shipment";
        public const string FromIntervent = "FromINTERVENT";
        public const string ToIntervent = "ToINTERVENT";
        public const string Employer = "Employer";
        public const string EligibilityFolder = "Eligibility";
        public const string ActivityFolder = "Activity";
        public const string ErrorFolder = "Errors";
        public const string SuccessFolder = "Success";
        public const string ArchieveFolder = "ARCHIVE";

        public IntuityEligibilityManager()
        {
            if (string.IsNullOrEmpty(_intuityFolderPath))
            {
                string extraMessage = "Intuity Eligibility Folder path in table is empty";
                Log.Warn(extraMessage);
                throw new Exception(extraMessage);
            }
        }

        #region Eligibility File

        public void ConstructEligibilityFile()
        {
            var orgs = GetOrganizationsForIntuityEligibility();
            foreach (OrganizationDto org in orgs)
            {
                var currentPortal = org.Portals.Where(x => x.Active == true && x.EligtoIntuity.HasValue).FirstOrDefault();
                if (currentPortal != null)
                {
                    List<string> errorList = new List<string>();
                    var eligibilityFileName = Path.Combine(_intuityFolderPath, EligibilityFolder, org.Code + "_Elig_" + DateTime.UtcNow.ToString("MM_dd_yyyy_H_mm") + ".csv");
                    //Write Header
                    if (!File.Exists(eligibilityFileName))
                    {
                        //create error directory if directory does not exist
                        if (!System.IO.Directory.Exists(Path.Combine(_intuityFolderPath, EligibilityFolder)))
                        {
                            System.IO.Directory.CreateDirectory(Path.Combine(_intuityFolderPath, EligibilityFolder));
                        }
                        using (TextWriter writer = new StreamWriter(eligibilityFileName, false, Encoding.UTF8))
                        {
                            using (var csv = new CsvWriter(writer))
                            {
                                csv.Configuration.Encoding = Encoding.UTF8;
                                csv.Configuration.Delimiter = ",";
                                csv.Configuration.RegisterClassMap<IntuityEligibilityStatusCsvClassMap>();
                                csv.WriteHeader(typeof(IntuityEligibilityStatusCsvModel));
                            }
                        }
                    }

                    using (TextWriter intuityFile = new StreamWriter(eligibilityFileName, true, Encoding.UTF8))
                    {
                        using (CsvWriter writer = new CsvWriter(intuityFile))
                        {

                            writer.Configuration.Encoding = Encoding.UTF8;
                            writer.Configuration.RegisterClassMap<IntuityEligibilityStatusCsvClassMap>();
                            // writer config
                            writer.Configuration.Delimiter = ",";
                            var eligibilityList = _eligibilityReader.GetIntuityEligibilityList(org);
                            foreach (var data in eligibilityList)
                            {
                                try
                                {
                                    writer.WriteRecord(data);
                                }
                                catch (CsvHelperException ex)
                                {
                                    string msg = "";
                                    foreach (System.Collections.DictionaryEntry de in ex.Data)
                                    {
                                        msg += de.Value;
                                    }
                                    errorList.Add(data.PatientUniqueId + "," + DateTime.Now.ToString() + "," + "error string: " + msg + "," + JsonConvert.SerializeObject(data).Replace(",", " "));
                                    Log.Error(ex);
                                }
                                catch (Exception ex)
                                {
                                    errorList.Add(data.PatientUniqueId + "," + DateTime.Now.ToString() + "," + "error string: " + ex.Message + "," + JsonConvert.SerializeObject(data).Replace(",", " "));
                                    Log.Error(ex);
                                }
                            }
                        }
                    }

                    #region Error Handling
                    if (errorList.Count > 0)
                    {
                        var csv = new StringBuilder();
                        csv.AppendLine("UniqueId,Date & Time, Error Message, Record");
                        string errorDirectory = Path.Combine(_intuityFolderPath, EligibilityFolder, ErrorFolder);
                        //create error directory if directory does not exist
                        if (!System.IO.Directory.Exists(errorDirectory))
                        {
                            System.IO.Directory.CreateDirectory(errorDirectory);
                        }
                        // do something with entry.Value or entry.Key
                        string errorFile = Path.Combine(errorDirectory, "_" + DateTime.UtcNow.ToString("yyyyMMdd") + ".csv");
                        foreach (var errorString in errorList)
                        {
                            csv.AppendLine(errorString);
                        }
                        File.WriteAllText(errorFile, csv.ToString());
                    }
                    #endregion
                }
            }

        }

        #endregion

        #region Shipment

        public void ProcessShipment()
        {
            //check if the folder has file
            string shipmentFilePath = Path.Combine(_intuityFolderPath, ToIntervent, Employer, ShipmentFolder);
            try
            {
                Dictionary<string, int> orgCache = new Dictionary<string, int>();
                orgCache = new PortalReader().GetOrganizationsWithCodes();
                if (orgCache != null)
                {
                    var files = Directory.GetFiles(shipmentFilePath, "*.csv");
                    var fulfillmentBuffer = new List<DAL.IntuityFulfillments>();
                    var eligibilityBuffer = new List<DAL.IntuityEligibility>();
                    List<string> _errorStrings = new List<string>();
                    int bufferCount = 1000;
                    foreach (string file in files)
                    {
                        string uniqueId = null;
                        using (var sr = new StreamReader(file))
                        {
                            CsvReader reader = new CsvReader(sr);
                            reader.Configuration.Delimiter = ",";
                            reader.Configuration.IsHeaderCaseSensitive = false;
                            reader.Configuration.RegisterClassMap<IntuityShipmentCsvClassMap>();
                            reader.Configuration.SkipEmptyRecords = true;
                            reader.Configuration.TrimFields = true;
                            reader.Configuration.TrimHeaders = true;
                            reader.Configuration.WillThrowOnMissingField = false;
                            while (reader.Read())
                            {
                                try
                                {
                                    //flush buffers
                                    if (fulfillmentBuffer.Count() > bufferCount)
                                    {
                                        _eligibilityReader.BulkEditFulfillment(fulfillmentBuffer);
                                        _eligibilityReader.BulkEditEligibility(eligibilityBuffer);
                                        fulfillmentBuffer = new List<DAL.IntuityFulfillments>();
                                        eligibilityBuffer = new List<DAL.IntuityEligibility>();
                                    }
                                    IntuityShipmentCsvModel record = reader.GetRecord<IntuityShipmentCsvModel>();
                                    //DOn't process old records
                                    if (record.DateSent < DateTime.UtcNow.AddDays(-3))
                                        continue;
                                    uniqueId = record.UniqueId;
                                    var orgCode = reader.GetField<string>("UniqueId").Split('-')[0];
                                    if (orgCode != null)
                                        record.OrgId = orgCache[orgCode];
                                    //Validate
                                    var validationResults = new List<ValidationResult>();
                                    var validationContext = new ValidationContext(record);
                                    var isValid = Validator.TryValidateObject(record, validationContext, validationResults, true);
                                    if (!isValid)
                                    {
                                        _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + JsonConvert.SerializeObject(validationResults).Replace(",", " "));
                                        continue;
                                    }
                                    if (!record.Valid())
                                    {
                                        _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + ",Record Not Valid:" + JsonConvert.SerializeObject(record).Replace(",", " "));
                                        continue;
                                    }
                                    var response = _eligibilityReader.GetLatestEligibilityByUniqueId(record.UniqueId, record.OrgId, record.SoNbr);
                                    if (response.Eligibility == null)
                                    {
                                        _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + "UniqueId match not found:" + JsonConvert.SerializeObject(record).Replace(",", " "));
                                        continue;
                                    }
                                    if (response.Fulfillment == null)
                                    {
                                        response.Fulfillment = new DAL.IntuityFulfillments();
                                        response.Fulfillment.DateCreated = DateTime.UtcNow;
                                        response.Fulfillment.IntuityEligibility = response.Eligibility;
                                        response.Fulfillment.IntuityEligibilityId = response.Eligibility.Id;
                                    }
                                    record.MapToFulfillmentDAL(response.Fulfillment);
                                    record.MapToEligibilityDAL(response.Eligibility);
                                    fulfillmentBuffer.Add(response.Fulfillment);
                                    eligibilityBuffer.Add(response.Eligibility);
                                }
                                catch (CsvMissingFieldException ex)
                                {
                                    _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                }
                                catch (CsvBadDataException ex)
                                {
                                    _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                }
                                catch (Exception ex)
                                {
                                    _errorStrings.Add(uniqueId + "," + DateTime.Now.ToString() + "," + ex.Message.Replace(",", " "));
                                }
                            }
                        }
                        //flush buffers
                        _eligibilityReader.BulkEditFulfillment(fulfillmentBuffer);
                        _eligibilityReader.BulkEditEligibility(eligibilityBuffer);

                        #region Log Eligibility
                        LogReader logreader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "Processed shipment file on " + DateTime.Now.ToString() + ". ErrorCount " + _errorStrings.Count(), null, null);
                        logreader.WriteLogMessage(logEvent);
                        #endregion
                    }

                    #region log error
                    if (_errorStrings.Count > 0)
                    {
                        LogReader logreader = new LogReader();
                        foreach (var errorString in _errorStrings)
                        {
                            var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, "Error in the shipment file for the record : " + errorString, null, null);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    #endregion
                }

            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityJobService", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        #endregion

        #region Create New Eligibility
        /// <summary>
        /// Check if the participant is still valid or if there are any new participants
        /// </summary>

        public void CreateIntuityEligibility()
        {
            try
            {
                var orgs = GetOrganizationsForIntuityEligibility();
                foreach (OrganizationDto org in orgs)
                {
                    var currentPortal = org.Portals.Where(x => x.Active == true && x.EligtoIntuity.HasValue).FirstOrDefault();
                    if (currentPortal != null)
                    {
                        int orgId = org.Id.Value, portalId = currentPortal.Id;
                        var response = _eligibilityReader.GetEligAndIntuityEligByPortal(orgId, portalId);
                        List<IntuityEligibilityDto> newEligibilityList = new List<IntuityEligibilityDto>();
                        if (response != null && response.Eligibilities != null && response.Eligibilities.Count() > 0)
                        {
                            DateTime ChildAge = DateTime.UtcNow.AddYears(-13);
                            var eligibilities = response.Eligibilities.Where(x => x.DOB < ChildAge).ToList();
                            //Find all new user
                            foreach (var elig in eligibilities)
                            {
                                var intuityElig = response.IntuityEligibilities.Where(x => x.UniqueId == elig.UniqueId).FirstOrDefault();
                                //new user 
                                if (intuityElig == null)
                                {
                                    if (currentPortal.EligtoIntuity.Value == (byte)EligibilitytoIntuity.All || GetIntuityEligibilityStatus(elig.UniqueId, orgId) || elig.BusinessUnit.ToUpper() == "BUCKNER HEAVYLIFT CRANES, LLC")
                                    {
                                        List<string> eligUniqueIdList = new List<string>();
                                        eligUniqueIdList.Add(elig.UniqueId);
                                        if (elig.UserEnrollmentType.UserEnrollmentTypeKey != EligibilityUserEnrollmentTypeDto.Employee.UserEnrollmentTypeKey)
                                        {
                                            if (!eligUniqueIdList.Contains(elig.EmployeeUniqueId))
                                                eligUniqueIdList.Add(elig.EmployeeUniqueId);
                                        }
                                        foreach (var eligUniqueId in eligUniqueIdList)
                                        {
                                            if (response.IntuityEligibilities.Where(x => x.UniqueId == eligUniqueId).Count() == 0 && newEligibilityList.Where(x => x.UniqueId == eligUniqueId).Count() == 0)
                                            {
                                                IntuityEligibilityDto intuityEligibility = new IntuityEligibilityDto();
                                                intuityEligibility.EligibilityStatus = EligibilityStatus.New;
                                                intuityEligibility.OrganizationId = orgId;
                                                intuityEligibility.UniqueId = eligUniqueId;
                                                newEligibilityList.Add(intuityEligibility);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        //New Entry
                        if (newEligibilityList.Count() > 0)
                        {
                            _eligibilityReader.BulkAddEditIntuityEligibility(newEligibilityList);
                            LogReader reader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Trace, "Service", null, "New Intuity Eligibility Count " + newEligibilityList.Count(), null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        #endregion

        #region Verify Intuity User with Email

        public void VerifyIntuityUser()
        {
            ExternalReader externalReader = new ExternalReader();
            LogReader reader = new LogReader();
            PortalReader portalReader = new PortalReader();
            var orgList = portalReader.GetIntuityOrgs();
            foreach (OrganizationDto org in orgList)
            {
                if (org != null && org.Portals.Where(x => x.Active == true).Count() > 0)
                {
                    var uptUserresponse = externalReader.GetIntuityUser(org.Portals.Where(x => x.Active == true).FirstOrDefault().Id, true);
                    foreach (var elig in uptUserresponse.NewUsers)
                    {
                        try
                        {
                            VerifyIntuityUserRequest request = new VerifyIntuityUserRequest();
                            request.emailId = elig.Email2.ToLower(); //Patterns email
                            if (org.Code == ConfigurationManager.AppSettings["DTCOrgCode"])
                                request.shopifyCustomerNumber = elig.UniqueId;
                            else
                                request.UniqueId = elig.UniqueId;
                            var apiResponse = Intuity.PostVerifyUserResponse(request);
                            //Log the API response
                            LogLevel logLevel = LogLevel.Info;
                            string message = "Email id verification sent to Patterns for Unique ID: " + elig.UniqueId + ".";
                            if (!apiResponse.Status)
                            {
                                logLevel = LogLevel.Error;
                                message = message + " But it failed. Reason : " + apiResponse.StatusCode;
                            }
                            var logEvent = new LogEventInfo(logLevel, "VerifyIntuityUser", null, message, null, null);
                            reader.WriteLogMessage(logEvent);
                        }
                        catch (Exception ex)
                        {
                            LogReader logReader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService.VerifyIntuityUser", null, ex.Message, null, ex);
                            logReader.WriteLogMessage(logEvent);
                        }
                    }
                }
            }
        }

        #endregion

        #region Internal Methods [ProcessIntuityEligibility]

        private class IntuityEligibilityStatus
        {
            public EligibilityStatus EligibilityStatus { get; set; }

            //public EligibilityReason StatusReason { get; set; }

            //public int? HRAId { get; set; }
        }

        //Identify all the ppts who have diabetes based on their HRA or CandidateCondition. HRA - use the questions "have you been told you have diabetes?" & "do you take medication for diabetes?" as well as lab values.
        //If ppt says yes to diabetes & takes medication they qualify.
        //If ppt takes medication they qualify.
        //If ppt says yes to diabetes but does not take medication & has no labs or labs not in diabetes range (defined below) they need outreach to confirm diabetes - we need the UI to be able to reflect this confirmation.
        //If ppt says yes to diabetes & does not take medication but has labs in a diabetes range, they qualify.
        //If they say no to both of these questions but have labs completed with glucose or A1C in a diabetes range, they need outreach to confirm diabetes - we need the UI to be able to reflect this confirmation.
        //Lab values in diabetes range are defined as: Fasting glucose of 126 or higher; non-fasting glucose of 200 or higher; or A1C of 6.5 or higher
        private bool GetIntuityEligibilityStatus(string uniqueId, int orgId)
        {
            var request = new GetUserRequestByUniqueId();
            request.UniqueId = uniqueId;
            request.OrganizationId = orgId;
            if (_eligibilityReader.IsDiabeticByClaims(uniqueId, orgId))
            {
                return true;
            }
            var userDto = _accountReader.GetUserHRAByUniqueId(request);
            if (userDto != null && userDto.HRAs.Count > 0)
            {
                var count = userDto.HRAs.Count;
                var hras = userDto.HRAs;
                if (hras.Count > 2)
                {
                    count = count - 2;
                    hras = userDto.HRAs.OrderBy(o => o.Id).Skip(count).ToList();
                }

                foreach (var h in hras.OrderByDescending(h => h.Id))
                {
                    var isFasting = false;
                    if (h.MedicalCondition != null)
                    {
                        //Take Diabetes medicine or told diabetes
                        if (h.MedicalCondition.DiabetesMed.HasValue && h.MedicalCondition.DiabetesMed.Value == 1
                            || h.MedicalCondition.ToldDiabetes.HasValue && h.MedicalCondition.ToldDiabetes.Value == 1)
                        {
                            return true;
                        }
                    }

                    if (h.HealthNumbers != null)
                    {
                        //Dont take medication or told diabetes but glucose out of range
                        if (HasGlucose(h.HealthNumbers, ref isFasting))
                        {
                            return true;
                        }
                        //Dont take medication or told diabetes but A1c out of range
                        if (HasA1C(h.HealthNumbers))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        //Fasting glucose of 126 or higher; 
        //non-fasting glucose of 200 or higher;
        //or A1C of 6.5 or higher
        private bool HasGlucose(HealthNumbersDto healthNumber, ref bool isFasting)
        {
            if (healthNumber.DidYouFast.HasValue && healthNumber.DidYouFast.Value == 1)
            {
                if (healthNumber.Glucose.HasValue && healthNumber.Glucose.Value >= 126)
                {
                    isFasting = true;
                    return true;
                }
            }
            else if (healthNumber.Glucose.HasValue && healthNumber.Glucose.Value >= 200)
            {
                return true;
            }
            return false;
        }

        //A1C of 6.5 or higher
        private bool HasA1C(HealthNumbersDto healthNumber)
        {
            if (healthNumber.A1C.HasValue && healthNumber.A1C.Value >= 6.5)
                return true;
            return false;
        }

        private IEnumerable<OrganizationDto> GetOrganizationsForIntuityEligibility()
        {
            return new PortalReader().ListOrganizations(new ListOrganizationsRequest()).Organizations.Where(x => x.Active && (x.Portals.Where(y => y.Active && y.EligtoIntuity.HasValue).Count() > 0));
        }

        #endregion

        #region Process Failed API Intuity Eligibility
        public void ProcessFailedIntuityEligibilityRecords()
        {
            try
            {
                #region FulfillmentRequest
                IList<DAL.IntuityFulfillmentRequests> fulfillmentRequestsList = _intuityReader.GetIntuityFulfillmentRequestFailedList().IntuityFulfillmentRequests;
                if (fulfillmentRequestsList.Count > 0)
                {
                    IList<DAL.IntuityFulfillmentRequests> updatedFulfillmentRequestsList = new List<DAL.IntuityFulfillmentRequests>();
                    foreach (DAL.IntuityFulfillmentRequests request in fulfillmentRequestsList)
                    {
                        try
                        {
                            int counter = _intuityReader.GetIntuityFulfillmentRequestsCountByPortalDate(new GetIntuityFulfillmentRequestsListRequest { IntuityEligibilityId = request.IntuityEligibilityId, Organizationid = request.IntuityEligibility.OrganizationId }).IntuityFulfillmentRequests.Count;
                            string patient_unique_id = request.IntuityEligibility.Organization.Code + "-" + request.IntuityEligibility.UniqueId;
                            Replenishment replenishment = new Replenishment
                            {
                                patient = new Replenishment.Patient
                                {
                                    require_immediate_shipment = request.ImmediateShipment.HasValue ? request.ImmediateShipment.Value : false,
                                    custom_replenishment_quantity = request.ReplenishmentQuantity.Value,
                                    custom_replenishment_num_of_shipments = request.NumberOfShipments,
                                    custom_replenishment_qty_reason = request.Reason,
                                    custom_replenishment_counter = counter
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.IntuityEligibility.Organization.Code);
                            var apiResponse = Intuity.PostIntuityResponse(replenishment, patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            request.Submitted = apiResponse.Status;
                            LogAPIResponse(apiResponse, "PUT : Retried Intuity Eligibility Replenishment details through API for Unique ID: " + patient_unique_id + ".");
                            updatedFulfillmentRequestsList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility Replenishment", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedFulfillmentRequestsList.Count > 0)
                        _intuityReader.BulkUpdateIntuityFulfillmentRequestList(new BulkUpdateIntuityFulfillmentRequest { IntuityFulfillmentRequestsList = updatedFulfillmentRequestsList });
                }
                #endregion FulfillmentRequest

                #region Quantity on Hand
                IList<DAL.IntuityQOH> quantityonHandList = _intuityReader.GetIntuityQuantityonHandFailedList().IntuityQOHList;
                if (quantityonHandList.Count > 0)
                {
                    IList<DAL.IntuityQOH> updatedQuantityonHandList = new List<DAL.IntuityQOH>();
                    foreach (DAL.IntuityQOH request in quantityonHandList)
                    {
                        try
                        {
                            QuantityOnHand qoh = new QuantityOnHand
                            {
                                patient = new QuantityOnHand.Patient
                                {
                                    coachcall_cartridge_qoh = request.QuantityOnHand,
                                    coachcall_date = request.CreatedOn.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.IntuityEligibility.Organization.Code);
                            string patient_unique_id = request.IntuityEligibility.Organization.Code + "-" + request.IntuityEligibility.UniqueId;
                            var apiResponse = Intuity.PostIntuityResponse(qoh, patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "PUT : Retried Intuity Eligibility Quantity On Hand details updated through API for Unique ID: " + patient_unique_id + ".");
                            request.Submitted = apiResponse.Status;
                            updatedQuantityonHandList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility QOH", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedQuantityonHandList.Count > 0)
                        _intuityReader.BulkUpdateIntuityQuantityOnHandList(new BulkUpdateIntuityQuantityOnHandListRequest { IntuityQOHRequestsList = updatedQuantityonHandList });
                }
                #endregion Quantity on Hand

                #region Profile
                IList<DAL.IntuityEligibilityLog> intuityEligibilityProfileList = _intuityReader.GetIntuityEligibilityLogRequestFailedList(new GetIntuityEligibilityLogListRequest { SuccessStatus = APIStatus.SuccessProfile, FailedStatus = APIStatus.FailedProfile }).IntuityEligibilityLogList;
                if (intuityEligibilityProfileList.Count > 0)
                {
                    IList<DAL.IntuityEligibilityLog> updatedIntuityEligibilityLogList = new List<DAL.IntuityEligibilityLog>();
                    IList<CountryDto> Countries = _commonReader.ListCountries(new ListCountriesRequest()).Countries;
                    foreach (DAL.IntuityEligibilityLog request in intuityEligibilityProfileList)
                    {
                        try
                        {
                            IntuityEligibilityProfile profileRequest = new IntuityEligibilityProfile
                            {
                                patient = new IntuityEligibilityProfile.Patient
                                {
                                    firstname = request.FirstName,
                                    lastname = request.LastName,
                                    email = request.email,
                                    phone = request.PhoneNumber,
                                    address1 = request.AddressLine1,
                                    address2 = request.AddressLine2,
                                    city = request.City,
                                    state = _commonReader.ListStates(new ListStatesRequest { CountryId = request.Country.Value }).States.Where(x => x.Id == request.State.Value).Select(x => x.Name).FirstOrDefault(),
                                    country = Countries.Where(x => x.Id == request.Country.Value).Select(x => x.Name).FirstOrDefault(),
                                    zip = request.Zip
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.OrganizationCode);
                            string patient_unique_id = request.OrganizationCode + '-' + request.UniqueId;
                            var apiResponse = Intuity.PostIntuityResponse(profileRequest, patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "PUT : Retried Intuity Eligibility Profile details updated through API for Unique ID: " + patient_unique_id + ".");
                            request.APIStatus = apiResponse.Status ? (byte)APIStatus.SuccessProfile : (byte)APIStatus.FailedProfile;
                            updatedIntuityEligibilityLogList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility Profile", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedIntuityEligibilityLogList.Count > 0)
                        _intuityReader.BulkUpdateIntuityEligibilityLogRequestList(new BulkUpdateIntuityEligibilityLogRequest { IntuityEligibilityLogList = updatedIntuityEligibilityLogList });
                }
                #endregion Profile

                #region EligiblityStatus
                IList<DAL.IntuityEligibilityLog> intuityEligiblityStatusList = _intuityReader.GetIntuityEligibilityLogRequestFailedList(new GetIntuityEligibilityLogListRequest { SuccessStatus = APIStatus.SuccessEligibilityStatus, FailedStatus = APIStatus.FailedEligibilityStatus }).IntuityEligibilityLogList;
                if (intuityEligiblityStatusList.Count > 0)
                {
                    IList<DAL.IntuityEligibilityLog> updatedIntuityEligiblityStatusList = new List<DAL.IntuityEligibilityLog>();
                    foreach (DAL.IntuityEligibilityLog request in intuityEligiblityStatusList)
                    {
                        try
                        {
                            IntuityEligibilityLogAPIRequest.IntuityEligibilityStatus eligibilityStatus = new IntuityEligibilityLogAPIRequest.IntuityEligibilityStatus
                            {
                                patient = new IntuityEligibilityLogAPIRequest.IntuityEligibilityStatus.Patient
                                {
                                    eligibility_status = ListOptions.GetEligibilityStatusList().Where(x => x.Value == request.EligibilityStatus.ToString()).Select(x => x.Text).FirstOrDefault(),
                                    status_reason = ListOptions.GetEligibilityReasonsList().Where(x => x.Value == request.EligibilityReason.ToString()).Select(x => x.Text).FirstOrDefault()
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.OrganizationCode);
                            string patient_unique_id = request.OrganizationCode + '-' + request.UniqueId;
                            var apiResponse = Intuity.PostIntuityResponse(eligibilityStatus, patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "PUT : Retried Intuity Eligibility Status details updated through API for Unique ID: " + patient_unique_id + ".");
                            request.APIStatus = apiResponse.Status ? (byte)APIStatus.SuccessEligibilityStatus : (byte)APIStatus.FailedEligibilityStatus;
                            updatedIntuityEligiblityStatusList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibilityStatus", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedIntuityEligiblityStatusList.Count > 0)
                        _intuityReader.BulkUpdateIntuityEligibilityLogRequestList(new BulkUpdateIntuityEligibilityLogRequest { IntuityEligibilityLogList = updatedIntuityEligiblityStatusList });
                }
                #endregion EligiblityStatus

                #region Opting Out
                IList<DAL.IntuityEligibilityLog> intuityEligiblityOptOutList = _intuityReader.GetIntuityEligibilityLogRequestFailedList(new GetIntuityEligibilityLogListRequest { SuccessStatus = APIStatus.SuccessOptingOut, FailedStatus = APIStatus.FailedOptingOut }).IntuityEligibilityLogList;
                if (intuityEligiblityOptOutList.Count > 0)
                {
                    IList<DAL.IntuityEligibilityLog> updatedIntuityEligiblityOptOutList = new List<DAL.IntuityEligibilityLog>();
                    foreach (DAL.IntuityEligibilityLog request in intuityEligiblityOptOutList)
                    {
                        try
                        {
                            OptingOut optingOut = new OptingOut
                            {
                                patient = new OptingOut.Patient
                                {
                                    opting_out_date = request.OptingOut.HasValue ? request.OptingOut.Value.ToString("yyyy-MM-dd") : null
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.OrganizationCode);
                            string patient_unique_id = request.OrganizationCode + '-' + request.UniqueId;
                            var apiResponse = Intuity.PostIntuityResponse(optingOut, patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "PUT : Retried Intuity Eligibility Opting Out details updated through API for Unique ID: " + patient_unique_id + ".");
                            request.APIStatus = apiResponse.Status ? (byte)APIStatus.SuccessOptingOut : (byte)APIStatus.FailedOptingOut;
                            updatedIntuityEligiblityOptOutList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility OptingOut", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedIntuityEligiblityOptOutList.Count > 0)
                        _intuityReader.BulkUpdateIntuityEligibilityLogRequestList(new BulkUpdateIntuityEligibilityLogRequest { IntuityEligibilityLogList = updatedIntuityEligiblityOptOutList });
                }
                #endregion Opting Out

                #region Paired Date
                IList<DAL.IntuityEligibilityLog> pairedDateList = _intuityReader.GetIntuityEligibilityLogRequestFailedList(new GetIntuityEligibilityLogListRequest { SuccessStatus = APIStatus.SuccessPairedDate, FailedStatus = APIStatus.FailedPairedDate }).IntuityEligibilityLogList;
                if (pairedDateList.Count > 0)
                {
                    IList<DAL.IntuityEligibilityLog> updatedPairedDateList = new List<DAL.IntuityEligibilityLog>();
                    foreach (DAL.IntuityEligibilityLog request in pairedDateList)
                    {
                        try
                        {
                            string pattern_unique_id = _intuityReader.GetIntuityUserExternalId(new GetIntuityUserExternalIdRequest { OrganizationId = request.OrganizationId.Value, UniqueId = request.UniqueId }).ExternalId;
                            PatternPairing pattern = new PatternPairing
                            {
                                pattern = new PatternPairing.Pattern
                                {
                                    guid = pattern_unique_id,
                                    pairing_date = request.PairedDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                    pattern_paired_sn = request.Devices
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.OrganizationCode);
                            SendPatternDetailsResponse apiResponse = Intuity.SendPatternsDetailsRequest(pattern, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "PUT : Retried Pattern Pairing date sent to Intuity for User External ID : " + pattern_unique_id + ".");
                            request.APIStatus = apiResponse.Status ? (byte)APIStatus.SuccessPairedDate : (byte)APIStatus.FailedPairedDate;
                            updatedPairedDateList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility Pairing date", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedPairedDateList.Count > 0)
                        _intuityReader.BulkUpdateIntuityEligibilityLogRequestList(new BulkUpdateIntuityEligibilityLogRequest { IntuityEligibilityLogList = updatedPairedDateList });
                }
                #endregion Paired Date

                #region Patterns Created Date
                IList<DAL.IntuityEligibilityLog> createdDateList = _intuityReader.GetIntuityEligibilityLogRequestFailedList(new GetIntuityEligibilityLogListRequest { SuccessStatus = APIStatus.SuccessCreationDate, FailedStatus = APIStatus.FailedCreationDate }).IntuityEligibilityLogList;
                if (createdDateList.Count > 0)
                {
                    IList<DAL.IntuityEligibilityLog> updatedCreatedDateList = new List<DAL.IntuityEligibilityLog>();
                    foreach (DAL.IntuityEligibilityLog request in createdDateList)
                    {
                        try
                        {
                            string pattern_unique_id = _intuityReader.GetIntuityUserExternalId(new GetIntuityUserExternalIdRequest { OrganizationId = request.OrganizationId.Value, UniqueId = request.UniqueId }).ExternalId;
                            PatternCreation pattern = new PatternCreation
                            {
                                pattern = new PatternCreation.Pattern
                                {
                                    guid = pattern_unique_id,
                                    email = request.email,
                                    pattern_creation_date = request.PatternsRegDate.Value.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                                }
                            };
                            var orgDetails = GetIntuityAPIDetails(request.OrganizationCode);
                            SendPatternDetailsResponse apiResponse = Intuity.SendPatternsDetailsRequest(pattern, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            LogAPIResponse(apiResponse, "POST : Retried Pattern Creation date sent to Intuity for User External ID : " + pattern_unique_id + ".");
                            request.APIStatus = apiResponse.Status ? (byte)APIStatus.SuccessCreationDate : (byte)APIStatus.FailedCreationDate;
                            updatedCreatedDateList.Add(request);
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "IntuityEligibility Creation date", null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (updatedCreatedDateList.Count > 0)
                        _intuityReader.BulkUpdateIntuityEligibilityLogRequestList(new BulkUpdateIntuityEligibilityLogRequest { IntuityEligibilityLogList = updatedCreatedDateList });
                }
                #endregion Patterns Created Date
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityJobService", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        #region Refresh Intuity access token
        public void RefreshIntuityToken()
        {
            LogReader reader = new LogReader();
            try
            {
                var intuityRefreshToken = _intuityReader.GetIntuityRefreshToken();
                if (intuityRefreshToken != null && intuityRefreshToken.accessToken != null)
                {
                    IntuityRefreshTokenResponse response = Intuity.RefreshIntuityTokenRequest(intuityRefreshToken.accessToken.Token);
                    if (response.Status)
                    {
                        var logEvent = new LogEventInfo(LogLevel.Trace, "RefreshIntuityToken", null, "Latest Token : " + JsonConvert.SerializeObject(response.refreshToken), null, null);
                        reader.WriteLogMessage(logEvent);
                        if (_intuityReader.UpdateIntuityAccessToken(response))
                            logEvent = new LogEventInfo(LogLevel.Info, "RefreshIntuityToken", null, "Intuity access & refresh token updated, Will expire on " + response.refreshToken.access_token_expiry_date, null, null);
                        else
                            logEvent = new LogEventInfo(LogLevel.Error, "RefreshIntuityToken", null, "Intuity access & refresh token update failed", null, null);
                        reader.WriteLogMessage(logEvent);
                    }
                    else
                    {
                        var logEvent = new LogEventInfo(LogLevel.Error, "RefreshIntuityToken", null, "Intuity access token update failed. Status Code : " + response.StatusCode, null, null);
                        reader.WriteLogMessage(logEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService.RefreshIntuityToken", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
        }

        #endregion

        private static void LogAPIResponse(ProcessResponse apiResponse, string message)
        {
            LogReader reader = new LogReader();
            LogLevel logLevel = LogLevel.Info;
            if (!apiResponse.Status)
            {
                logLevel = LogLevel.Error;
                message = message + " Failed Again. Reason : " + apiResponse.StatusCode;
            }
            var logEvent = new LogEventInfo(logLevel, "IntuityEligibility", null, message, null, apiResponse.Exception);
            reader.WriteLogMessage(logEvent);
        }

        private OrganizationDto GetIntuityAPIDetails(string OrgCode)
        {
            OrganizationDto orgDetails = _portalReader.GetOrganizationByCode(OrgCode);
            if (orgDetails == null)
                orgDetails = new OrganizationDto();
            orgDetails.IntuityEmpToken = !string.IsNullOrEmpty(orgDetails.IntuityEmpToken) ? orgDetails.IntuityEmpToken : _commonReader.GetAccessToken(new GetAccessTokenRequest { OrgName = AccessToken.Intuity, AccessTokenName = AccessToken.Token });
            return orgDetails;
        }

        #endregion

        #region Fetch Intuity EP data
        public void GetIntuityEPData()
        {
            try
            {
                IList<DAL.IntuityEligibility> userList = _intuityReader.GetIntuityUsersList();
                if (userList.Count > 0)
                {
                    int count = 0;
                    foreach (DAL.IntuityEligibility request in userList)
                    {
                        try
                        {
                            var orgDetails = GetIntuityAPIDetails(request.Organization.Code);
                            string patient_unique_id = request.Organization.Code + "-" + request.UniqueId;
                            IntuityEligibilityLogAPIResponse apiResponse = Intuity.GetIntuityResponse(patient_unique_id, orgDetails.IntuityEmpUrl, orgDetails.IntuityEmpToken);
                            if (apiResponse.StatusCode == System.Net.HttpStatusCode.OK && apiResponse.intuityAPIResponse != null)
                                if (_intuityReader.AddEditIntuityEPData(request.Id, apiResponse.intuityAPIResponse.patient))
                                    count++;
                        }
                        catch (Exception ex)
                        {
                            LogReader logreader = new LogReader();
                            var logEvent = new LogEventInfo(LogLevel.Error, "Get Intuity Details for UniquId : " + request.UniqueId, null, ex.Message, null, ex);
                            logreader.WriteLogMessage(logEvent);
                        }
                    }
                    if (count > 0)
                    {
                        LogReader logreader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Trace, "IntuityJobService", null, "Updated Intuity Details for (" + count + ") user's", null, null);
                        logreader.WriteLogMessage(logEvent);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityJobService", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

        public void CheckGlucoseActivity()
        {
            try
            {
                if (_intuityReader.CheckGlucoseActivity())
                {
                    EmailRequestDto emailParams = new EmailRequestDto() { AdditionalMessage = DateTime.UtcNow.AddDays(-1).Date.ToShortDateString() };
                    GlucoseActivityAlertTemplate notification = new GlucoseActivityAlertTemplate(emailParams);
                    notification.SendEmail();
                }
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "Service", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
            }
        }

    }
}