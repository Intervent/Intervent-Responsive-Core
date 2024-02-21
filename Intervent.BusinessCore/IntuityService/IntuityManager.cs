using Intervent.DAL;
using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using NLog;
using System.ComponentModel.DataAnnotations;

namespace Intervent.Business
{
    public class IntuityManager : BaseManager
    {
		public const int InvalidReading = 999;
        public int systemAdminId;
        public string DTCOrgCode;
        public string EbenOrgCode;

        ParticipantReader _participantReader = new ParticipantReader();
        IntuityReader _intuityReader = new IntuityReader();
        CommonReader commonReader = new CommonReader();
        PortalReader _portalReader = new PortalReader();

        public IntuityManager(string dtcOrgCode, string ebenOrgCode, int systemAdminId)
        {
            DTCOrgCode = dtcOrgCode;
            EbenOrgCode = ebenOrgCode;
            this.systemAdminId = systemAdminId;
        }

        public bool IsEligibleOrgCode(string code)
        {
            return DTCOrgCode.ToLower() != code.ToLower() && EbenOrgCode.ToLower() == code.ToLower();
        }

        #region

        public IntuityEligibilityResponse ProcessIntuityEligibility(IntuityEligibilityRequest request)
        {
            IntuityEligibilityResponse response = new IntuityEligibilityResponse();
            try
            {
                response.unique_id = request.unique_id;
                response.Status = false;
                var orgToProcess = _portalReader.GetOrganizationByCode(request.unique_id.Split('-')[0].ToString());

                if (orgToProcess != null)
                {
                    var orgName = orgToProcess.Name;
                    var activePortal = orgToProcess.Portals.Where(p => p.Active).FirstOrDefault();
                    var portalId = activePortal.Id;

                    var eligibility = _participantReader.GetEligibilityByUniqueId(request.unique_id.Split('-')[1].ToString(), portalId);
                    if (eligibility != null)
                    {
                        IntuityEligibilityLogDto intuityEligibilityLog = GetIntuityEligibility(request);
                        intuityEligibilityLog.OrganizationId = orgToProcess.Id.Value;
                        List<string> missingDataFields = new List<string>();
                        var validationResults = new List<ValidationResult>();
                        var validationContext = new ValidationContext(intuityEligibilityLog);
                        var isValid = Validator.TryValidateObject(intuityEligibilityLog, validationContext, validationResults, true);
                        if (!isValid)
                            missingDataFields.AddRange(validationResults.Where(x => !x.MemberNames.Contains("AddressLine2")).Select(x => x.MemberNames.FirstOrDefault()).ToList());
                        missingDataFields.AddRange(intuityEligibilityLog.Validate());
                        if (missingDataFields.Count > 0)
                        {
                            IDictionary<string, string> propertyName = PropertyNameMapper();
                            foreach (var missingDataField in missingDataFields)
                            {
                                if (string.IsNullOrEmpty(response.ErrorMsg))
                                {
                                    response.ErrorMsg = "Missing Data: " + propertyName.Where(v => v.Key == missingDataField).Select(v => v.Value).FirstOrDefault();
                                }
                                else
                                    response.ErrorMsg = response.ErrorMsg + ", " + propertyName.Where(v => v.Key == missingDataField).Select(v => v.Value).FirstOrDefault();
                            }
                        }
                        else
                        {
                            try
                            {
                                UpdateIntuityRequest intuityRequest = new UpdateIntuityRequest();
                                intuityRequest.PortalId = activePortal.Id;
                                intuityRequest.EligibilityId = eligibility.Id.Value;
                                intuityRequest.IntuityEligibilityLog = intuityEligibilityLog;
                                intuityRequest.UserId = systemAdminId;
                                UpdateIntuityResponse intuityResponse = _intuityReader.UpdateIntuityEligibility(intuityRequest);
                                response.Status = true;
                                response.eligibility_status = intuityResponse.EligibilityStatus.ToLower();
                                response.eligibility_message = intuityResponse.EligibilityReason;
                            }
                            catch (Exception ex)
                            {
                                LogReader logreader = new LogReader();
                                var logError = new LogEventInfo(LogLevel.Error, "ProcessIntuityEligibility", null, ex.Message, null, ex);
                                logreader.WriteLogMessage(logError);
                                response.Exception = ex;
                            }
                        }
                    }
                    else
                    {
                        response.ErrorMsg = "UniqueId not found";
                    }
                }
                else
                {
                    response.ErrorMsg = "Not a valid Organization";
                }

                response.Request = JsonConvert.SerializeObject(request);
                return response;
            }
            catch (Exception ex)
            {
                LogReader logreader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "ProcessIntuityEligibility", null, ex.Message, null, ex);
                logreader.WriteLogMessage(logEvent);
                response.Exception = ex;
                return response;
            }
        }

        public IntuityEligibilityLogDto GetIntuityEligibility(IntuityEligibilityRequest request)
        {
            IntuityEligibilityLogDto intuityEligibilityLog = new IntuityEligibilityLogDto();
            var stringList = request.unique_id.Split('-');
            intuityEligibilityLog.OrganizationCode = stringList[0].ToString();
            intuityEligibilityLog.UniqueId = stringList[1].ToString();
            intuityEligibilityLog.FirstName = request.first_name;
            intuityEligibilityLog.LastName = request.last_name;
            intuityEligibilityLog.email = request.email;
            intuityEligibilityLog.PhoneNumber = request.phone_number;
            intuityEligibilityLog.AddressLine1 = request.address_line1;
            intuityEligibilityLog.AddressLine2 = request.address_line2;
            intuityEligibilityLog.City = request.city;
            if (!string.IsNullOrEmpty(request.country))
            {
                var countryResponse = commonReader.ListCountries(new ListCountriesRequest()).Countries.Where(x => x.Name == request.country).FirstOrDefault();
                if (countryResponse != null)
                    intuityEligibilityLog.Country = countryResponse.Id;
            }
            if (!string.IsNullOrEmpty(request.state))
            {
                var stateResponse = commonReader.ListAllStates().States.Where(x => x.Name == request.state).FirstOrDefault();
                if (stateResponse != null)
                    intuityEligibilityLog.State = stateResponse.Id;
            }
            intuityEligibilityLog.Zip = request.zip;
            if (!string.IsNullOrEmpty(request.has_diabetes))
                intuityEligibilityLog.HasDiabetes = (byte)(GetYesorNoAnswer(request.has_diabetes));
            if (!string.IsNullOrEmpty(request.diabetes_type))
            {
                var diabetesType = ListOptions.GetDiabetesTypes().Where(x => x.DisplayText == request.diabetes_type).FirstOrDefault();
                if (diabetesType != null)
                    intuityEligibilityLog.DiabetesType = Convert.ToByte(diabetesType.Value);
            }
            intuityEligibilityLog.DiabetesDate = request.diabetes_diagnosis_date;
            if (!string.IsNullOrEmpty(request.has_prediabetes))
                intuityEligibilityLog.HasPreDiabetes = (byte)(GetYesorNoAnswer(request.has_prediabetes));
            if (!string.IsNullOrEmpty(request.take_diabetes_medication))
                intuityEligibilityLog.TakeDiabetesMed = (byte)(GetYesorNoAnswer(request.take_diabetes_medication));
            if (!string.IsNullOrEmpty(request.take_insulin))
                intuityEligibilityLog.TakeInsulin = (byte)(GetYesorNoAnswer(request.take_insulin));
            if (!string.IsNullOrEmpty(request.had_a1c_test))
                intuityEligibilityLog.HadA1CTest = (byte)(GetYesorNoAnswer(request.had_a1c_test));
            intuityEligibilityLog.A1CTestDate = request.a1c_test_date;
            if (request.a1c_value != 0)
                intuityEligibilityLog.A1CValue = (float)request.a1c_value;
            if (request.height != 0)
                intuityEligibilityLog.Height = (float)request.height;
            if (request.weight != 0)
                intuityEligibilityLog.Weight = (float)request.weight;
            if (!string.IsNullOrEmpty(request.no_a1c_test_reason))
            {
                var noA1CTestReason = ListOptions.GetA1cTestReasons().Where(x => x.Text == request.no_a1c_test_reason).FirstOrDefault();
                if (noA1CTestReason != null)
                    intuityEligibilityLog.NoA1cTestReason = Convert.ToByte(noA1CTestReason.Value);
            }
            return intuityEligibilityLog;

        }

        public static int GetYesorNoAnswer(string answer)
        {
            return answer == "Yes" ? 1 : answer == "No" ? 2 : 0;
        }

        static IDictionary<string, string> PropertyNameMapper()
        {
            IDictionary<string, string> propertyName = new Dictionary<string, string>();
            propertyName.Add("UniqueId", "unique_id");
            propertyName.Add("FirstName", "first_name");
            propertyName.Add("LastName", "last_name");
            propertyName.Add("email", "email");
            propertyName.Add("PhoneNumber", "phone_number");
            propertyName.Add("AddressLine1", "address_line1");
            propertyName.Add("AddressLine2", "address_line2");
            propertyName.Add("City", "city");
            propertyName.Add("Country", "country");
            propertyName.Add("State", "state");
            propertyName.Add("Zip", "zip");
            propertyName.Add("HasDiabetes", "has_diabetes");
            propertyName.Add("DiabetesType", "diabetes_type");
            propertyName.Add("DiabetesDate", "diabetes_diagnosis_date");
            propertyName.Add("HasPreDiabetes", "has_prediabetes");
            propertyName.Add("TakeDiabetesMed", "take_diabetes_medication");
            propertyName.Add("TakeInsulin", "take_insulin");
            propertyName.Add("HadA1CTest", "had_a1c_test");
            propertyName.Add("A1CValue", "a1c_value");
            propertyName.Add("A1CTestDate", "a1c_test_date");
            propertyName.Add("A1CTestReason", "no_a1c_test_reason");
            propertyName.Add("Height", "height");
            propertyName.Add("Weight", "weight");

            return propertyName;
        }
        #endregion

        #region Verify Eligibility 

        public ProcessResponse PairPogo(PairPogoRequest request)
        {
            ProcessResponse response = new ProcessResponse();
            try
            {
                ExternalReader externalReader = new ExternalReader();
                AccountReader accountReader = new AccountReader();
                IntuityUsers intuityUser = externalReader.GetIntuityUsersByExtUserId(request.user_id);
                if (intuityUser != null)
                {
                    var user = accountReader.GetUserById(intuityUser.UserId);
                    if (request.devices != null && !string.IsNullOrEmpty(request.devices[0]) && IsEligibleOrgCode(user.Organization.Code))
                    {
                        IntuityEligibilityLogDto intuityEligibilityLog = new IntuityEligibilityLogDto();
                        intuityEligibilityLog.OrganizationId = user.OrganizationId;
                        intuityEligibilityLog.UniqueId = user.UniqueId;
                        UpdatePatternPairingDateRequest paringDate = new UpdatePatternPairingDateRequest
                        {
                            IntuityEligibilityLog = intuityEligibilityLog,
                            UserId = user.Id,
                            PairingDate = Convert.ToDateTime(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                            Devices = string.Join(",", request.devices),
                            systemAdminId = systemAdminId,
                        };
                        _intuityReader.UpdatePatternPairingDateRequest(paringDate);
                        response.Status = true;
                    }
                    else
                        response.Status = true;
                }
                else
                    response.Status = true;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService.PatternPairingDate", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            return response;
        }

        public ProcessResponse VerifyUserResponse(UserManager<ApplicationUser> userManager, VerifyIntuityUserResponse response)
        {
            ProcessResponse partResponse = new ProcessResponse();
            try
            {
                if (response.user_id.HasValue)
                {
                    ParticipantReader partReader = new ParticipantReader();
                    PortalReader portalReader = new PortalReader();
                    string orgCode, uniqueId;
                    bool isDTC;
                    if (string.IsNullOrEmpty(response.shopify_customer_number))
                    {
                        string[] org_uniqueid = response.unique_id.Split('-');
                        orgCode = org_uniqueid[0].ToString();
                        uniqueId = org_uniqueid[1].ToString();
                        isDTC = false;
                    }
                    else
                    {
                        isDTC = true;
                        orgCode = DTCOrgCode;
                        uniqueId = response.shopify_customer_number;
                    }
                    var orgDetails = portalReader.GetOrganizationByCode(orgCode);
                    int orgId = orgDetails.Id.Value;
                    int portalId = orgDetails != null ? orgDetails.Portals.Where(p => p.Active).FirstOrDefault().Id : 0;
                    string emailId = null;

                    if (portalId > 0 && !string.IsNullOrEmpty(orgCode) && !string.IsNullOrEmpty(uniqueId))
                    {
                        GetIntuityResponse intuityEligibility = _intuityReader.GetIntuityDetails(new GetIntuityRequest() { UniqueId = uniqueId, OrganizationId = orgId });
                        EligibilityDto eligibility = partReader.GetEligibilityByUniqueId(uniqueId, portalId);
                        if (intuityEligibility != null && intuityEligibility.IntuityEligibility != null && eligibility != null)
                        {
                            UpdateEligibilityDetails(eligibility, intuityEligibility);
                        }

                        if (eligibility != null)
                        {
                            AccountReader accountReader = new AccountReader(userManager);
                            int userId = 0;
                            var userRegistration = accountReader.CheckifRegistered(new CheckifRegisteredRequest { OrganizationId = eligibility.Portal.OrganizationId, UniqueId = eligibility.UniqueId });
                            if (userRegistration.recordExist)
                            {
                                userId = userRegistration.User.Id;
                                emailId = userRegistration.User.Email;
                            }
                            else
                            {
                                var userAccount = accountReader.ReadUser(new GetUserRequest() { userName = response.email_address });
                                if (userAccount != null && userAccount.User != null && userAccount.User.UniqueId != uniqueId)
                                {
                                    string[] currentEmail = userAccount.User.Email.Split('@');
                                    var username = currentEmail[0].ToString() + "_updated@";
                                    userAccount.User.Email = username + currentEmail[1].ToString();
                                    //need to update the email address in the current account
                                    Task.Run(() => accountReader.UpdateUser(new UpdateUserRequest() { user = userAccount.User, FromAdmin = false, UpdatedByUserId = systemAdminId }));
                                }
                            }
                            if (userId == 0)
                            {
                                eligibility.Email = response.email_address;
                                var user = Task.Run(() => accountReader.CreateUserFromEligibility(eligibility, eligibility.Portal.OrganizationId, true)).Result;
                                userId = user.userId;
                                emailId = user.EmailId;
                            }
                            if (userId > 0)
                            {
                                EligibilityResponse eligibilityResponse = new EligibilityResponse();
                                eligibilityResponse = PopulateEligibilityResponse(eligibilityResponse, Convert.ToDateTime(eligibility.Portal.EndDate), eligibility.MedicalPlanEndDate, eligibility.MedicalPlanCode, eligibility.CoachingEnabled,
                                    eligibility.CoachingExpirationDate, eligibility.TerminatedDate, isDTC);
                                GetIntuityUsersResponse intuityUsersResponse = CreateIntuityUsers(Convert.ToInt32(response.user_id), userId, eligibilityResponse.expires_on, eligibilityResponse.is_eligible, eligibilityResponse.is_coaching_active);
                                eligibilityResponse.UniqueId = response.unique_id;
                                eligibilityResponse.auth_token = intuityUsersResponse.IntuityUsers.AuthToken;
                                eligibilityResponse.user_id = intuityUsersResponse.IntuityUsers.ExternalUserId;
                                eligibilityResponse.expires_on = intuityUsersResponse.IntuityUsers.ExpiryOn.ToString("yyyy-MM-ddTHH:mm:ssZ");
                                eligibilityResponse.eligibility_type = isDTC ? (byte)1 : (byte)2;
                                var apiResponse = Intuity.PostEligibiltyResponse(eligibilityResponse);
                                //Log the API response
                                LogAPIResponse(apiResponse, "VerifyUserResponse", "Eligibility update sent to Patterns for User ID : " + response.user_id.Value + ".");

                                if (IsEligibleOrgCode(orgCode))
                                {
                                    if (intuityUsersResponse.IsNewUser && !string.IsNullOrEmpty(emailId))
                                    {
                                        IntuityEligibilityLogDto intuityEligibilityLog = new IntuityEligibilityLogDto();
                                        intuityEligibilityLog.OrganizationId = orgId;
                                        intuityEligibilityLog.UniqueId = uniqueId;
                                        UpdatePatternCreationRequest paringDate = new UpdatePatternCreationRequest
                                        {
                                            IntuityEligibilityLog = intuityEligibilityLog,
                                            UserId = userId,
                                            PatternCreationDate = DateTime.Parse(DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")),
                                            systemAdminId = systemAdminId
                                        };
                                        _intuityReader.UpdatePatternCreationRequest(paringDate);
                                    }
                                }
                            }
                        }
                    }
                }
                partResponse.Status = true;
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService.VerifyUserResponse", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }
            if (!partResponse.Status)
            {
                response.Request = JsonConvert.SerializeObject(response);
            }
            return partResponse;
        }

        private void UpdateEligibilityDetails(EligibilityDto eligibility, GetIntuityResponse intuityEligibility)
        {
            eligibility.FirstName = intuityEligibility.IntuityEligibility.FirstName;
            eligibility.LastName = intuityEligibility.IntuityEligibility.LastName;
            eligibility.Email = intuityEligibility.IntuityEligibility.email;
            eligibility.CellNumber = intuityEligibility.IntuityEligibility.PhoneNumber;
            eligibility.City = intuityEligibility.IntuityEligibility.City;
            eligibility.State = commonReader.ListStates(new ListStatesRequest { CountryId = intuityEligibility.IntuityEligibility.Country.Value }).States.Where(x => x.Id == intuityEligibility.IntuityEligibility.State.Value).Select(x => x.Code).FirstOrDefault();
            eligibility.Country = commonReader.ListCountries(new ListCountriesRequest()).Countries.Where(x => x.Id == intuityEligibility.IntuityEligibility.Country.Value).Select(x => x.Code).FirstOrDefault();
            eligibility.Zip = intuityEligibility.IntuityEligibility.Zip;
            eligibility.Address = intuityEligibility.IntuityEligibility.AddressLine1;
            eligibility.Address2 = intuityEligibility.IntuityEligibility.AddressLine2;
        }

        public void SendEligibilityUpdate(int portalId)
        {
            ExternalReader extReader = new ExternalReader();
            var uptUserresponse = extReader.GetIntuityUser(portalId, false);
            if (uptUserresponse != null)
            {
                foreach (var intUser in uptUserresponse.UserList)
                {
                    try
                    {
                        EligibilityResponse response = new EligibilityResponse();
                        response.user_id = intUser.IntUser.ExternalUserId;
                        bool isDTC = false;
                        if (!string.IsNullOrEmpty(intUser.IntUser.User.Organization.Code) && intUser.IntUser.User.Organization.Code == DTCOrgCode)
                        {
                            isDTC = true;
                        }
                        bool sendUpdate = false;
                        if (isDTC && (intUser.IntUser.IsEligible != intUser.Eligibility.CoachingEnabled ||
                            intUser.Eligibility.CoachingExpirationDate > intUser.IntUser.ExpiryOn))
                        {
                            sendUpdate = true;
                            response = PopulateEligibilityResponse(response, intUser.Eligibility.Portal.EndDate, null, null, intUser.Eligibility.CoachingEnabled,
                                intUser.Eligibility.CoachingExpirationDate, null, isDTC);
                            if (string.IsNullOrEmpty(response.expires_on))
                                response.expires_on = intUser.Eligibility.CoachingExpirationDate.HasValue ? intUser.Eligibility.CoachingExpirationDate.Value.ToString() : intUser.IntUser.ExpiryOn.ToString();
                        }
                        else if (!isDTC && ((intUser.IntUser.IsEligible && intUser.Eligibility.TerminatedDate.HasValue) ||
                            (!intUser.IntUser.IsEligible && !intUser.Eligibility.TerminatedDate.HasValue) ||
                            (intUser.IntUser.IsEligible && intUser.IntUser.ExpiryOn < intUser.Eligibility.Portal.EndDate)))
                        {
                            sendUpdate = true;
                            response = PopulateEligibilityResponse(response, intUser.Eligibility.Portal.EndDate,
                                intUser.Eligibility.MedicalPlanEndDate, intUser.Eligibility.MedicalPlanCode, intUser.Eligibility.CoachingEnabled,
                                intUser.Eligibility.CoachingExpirationDate, intUser.Eligibility.TerminatedDate, isDTC);
                        }
                        if (sendUpdate)
                        {
                            GetIntuityUsersResponse intuityUsersResponse = CreateIntuityUsers(intUser.IntUser.ExternalUserId, intUser.IntUser.UserId, response.expires_on, response.is_eligible, response.is_coaching_active);
                            response.auth_token = intuityUsersResponse.IntuityUsers.AuthToken;
                            response.user_id = intuityUsersResponse.IntuityUsers.ExternalUserId;
                            response.eligibility_type = isDTC ? (byte)1 : (byte)2;
                            response.expires_on = intuityUsersResponse.IntuityUsers.ExpiryOn.ToString("yyyy-MM-ddTHH:mm:ssZ");
                            response.UniqueId = intUser.IntUser.User.Organization.Code + '-' + intUser.IntUser.User.UniqueId;
                            var apiResponse = Intuity.PostEligibiltyResponse(response);
                            //Log the API response
                            LogAPIResponse(apiResponse, "SendEligibilityUpdate", "Eligibility update sent to Patterns for User ID : " + response.user_id + ".");
                        }
                    }
                    catch (Exception ex)
                    {
                        LogReader logReader = new LogReader();
                        var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService.EligibilityUpdate", null, ex.Message, null, ex);
                        logReader.WriteLogMessage(logEvent);
                    }
                }
            }
        }

        private EligibilityResponse PopulateEligibilityResponse(EligibilityResponse response, DateTime? portalEndDate,
            DateTime? medicalPlanEndDate, string medicalPlanCode, bool? coachingEnabled, DateTime? coachingExpirationDate, DateTime? terminationDate, bool isDTC)
        {
            ExternalReader externalReader = new ExternalReader();
            if (isDTC && coachingEnabled.HasValue && coachingEnabled.Value && coachingExpirationDate.HasValue && coachingExpirationDate >= DateTime.UtcNow)
            {
                response.is_coaching_active = response.is_eligible = coachingEnabled.Value;
                response.expires_on = coachingExpirationDate.Value.AddDays(1).ToShortDateString();
            }
            else if (!isDTC)
            {
                if (terminationDate.HasValue && terminationDate.Value < DateTime.UtcNow)
                {
                    response.is_coaching_active = false;
                    response.is_eligible = false;
                    response.expires_on = terminationDate.Value.ToShortDateString();
                }
                else
                {
                    if ((!medicalPlanEndDate.HasValue || medicalPlanEndDate.Value > DateTime.UtcNow)
                    && !string.IsNullOrEmpty(medicalPlanCode))
                    {
                        response.is_coaching_active = true;
                    }
                    response.is_eligible = true;
                    if (portalEndDate.HasValue)
                    {
                        response.expires_on = portalEndDate.Value.ToShortDateString();
                    }
                    else
                    {
                        response.expires_on = DateTime.Now.AddYears(1).ToShortDateString();
                    }
                }
            }
            return response;
        }

        private GetIntuityUsersResponse CreateIntuityUsers(int extUserId, int userId, string expiresOn, bool isEligible, bool isCoachingAction)
        {
            ExternalReader externalReader = new ExternalReader();
            //create intuity user entry
            IntuityUsers intuityUsers = new IntuityUsers();
            intuityUsers.UserId = userId;
            intuityUsers.ExternalUserId = extUserId;
            intuityUsers.IsEligible = isEligible;
            intuityUsers.IsCoachingActive = isCoachingAction;
            intuityUsers.ExpiryOn = Convert.ToDateTime(expiresOn);
            return externalReader.AddEditIntuityUsers(intuityUsers);
        }

        private void LogAPIResponse(ProcessResponse apiResponse, string eventTitle, string message)
        {
            LogReader reader = new LogReader();
            LogLevel logLevel = LogLevel.Info;
            if (!apiResponse.Status)
            {
                logLevel = LogLevel.Error;
                message = message + " But it failed. Reason : " + apiResponse.StatusCode;
            }
            var logEvent = new LogEventInfo(logLevel, eventTitle, null, message, null, apiResponse.Exception);
            reader.WriteLogMessage(logEvent);
        }

        #endregion

        #region Update Profile
        public ProcessResponse UpdateProfile(ProfileUpdate profileUpdate)
        {
            ProcessResponse response = new ProcessResponse();
            response.Status = true;
            return response;
        }
        #endregion

        #region Update Account
        public ProcessResponse UpdateAccount(UserManager<ApplicationUser> userManager, AccountUpdate accountUpdate)
        {
            ProcessResponse response = new ProcessResponse();
            response.Status = true;
            ExternalReader externalReader = new ExternalReader();
            ParticipantReader participantReader = new ParticipantReader();
            AccountReader accountReader = new AccountReader(userManager);
            PortalReader portalReader = new PortalReader();

            IntuityUsers intuityUser = externalReader.GetIntuityUsersByExtUserId(accountUpdate.user_id);
            if (intuityUser != null)
            {
                var getUserRequest = new GetUserRequest();
                getUserRequest.id = intuityUser.UserId;
                var userResponse = Task.Run(() => accountReader.GetUser(getUserRequest)).Result;
                if (userResponse != null && userResponse.User != null)
                {
                    var user = userResponse.User;
                    AddNotesRequest request = new AddNotesRequest();
                    NotesDto note = new NotesDto();
                    note.userId = user.Id;
                    note.Admin = systemAdminId;
                    note.Type = (int)NoteTypes.Other;
                    var portal = portalReader.GetOrganizationDetails(user.OrganizationId).Portals.OrderByDescending(x => x.Id).FirstOrDefault();
                    if (portal != null)
                        note.PortalId = portal.Id;
                    if (accountUpdate.account_status == 1)
                        note.Text = "Patterns account has been restored.";
                    else if (accountUpdate.account_status == 2)
                        note.Text = "Patterns account has been deleted.";
                    request.note = note;
                    request.TimeZone = user.TimeZone;
                    participantReader.AddEditNotes(request);
                }
                else
                {
                    response.ErrorMsg = "User not found";
                }
            }
            if (!response.Status)
            {
                response.Request = JsonConvert.SerializeObject(accountUpdate);
            }
            return response;
        }
        #endregion

        #region Reading Pogo
        public ProcessResponse ReadPogo(ReadingPogoRequest pogoReadingRequest)
        {
            ProcessResponse response = new ProcessResponse();
            ExternalReader externalReader = new ExternalReader();
            AccountReader accountReader = new AccountReader();
            EXT_Glucose extGlucose = new EXT_Glucose();
            IntuityUsers intuityUser = externalReader.GetIntuityUsersByExtUserId(pogoReadingRequest.user_id);
            if (intuityUser != null)
            {
                var user = accountReader.GetUserById(intuityUser.UserId);
                //extGlucose.userId
                DateTime dateCreated = DateTime.UtcNow;
                extGlucose.UniqueId = user.UniqueId;
                extGlucose.OrganizationId = user.OrganizationId;
                extGlucose.Value = pogoReadingRequest.blood_glucose;
                if (extGlucose.Value > 0 && extGlucose.Value != InvalidReading)
                {
                    extGlucose.IsValid = true;
                }
                else
                {
                    extGlucose.IsValid = false;
                }
                extGlucose.Unit = "mg/dL";
                //Default value
                extGlucose.Code = "2345-7";
                extGlucose.EffectiveDateTime = pogoReadingRequest.timestamp;
                extGlucose.DateTime = dateCreated;
                extGlucose.UserId = user.Id;
                extGlucose.Source = (byte)GlucoseSource.Intuity;
                extGlucose.ExtId = pogoReadingRequest.id.ToString();
                response.Status = externalReader.AddGlucose(extGlucose);

                if (response.Status)
                {
                    if (pogoReadingRequest.tags != null)
                    {
                        foreach (var tag in pogoReadingRequest.tags)
                        {
                            string code = null;
                            var extTag = new Ext_GlucoseTags();
                            extTag.GlucoseId = extGlucose.Id;
                            extTag.TagName = tag.tag_type;
                            extTag.Value = tag.value;
                            extTag.Unit = tag.unit;
                            externalReader.AddGlucoseTags(extTag);
                            if (!string.IsNullOrEmpty(tag.tag_type) && tag.tag_type.ToLower() == "meal")
                            {
                                var tagValue = tag.value.ToLower();
                                if (tagValue.StartsWith("pre"))
                                {
                                    code = "53049-3";
                                }
                                else if (tagValue.StartsWith("post"))
                                {
                                    code = "1521-4";
                                }
                                else if (tagValue.StartsWith("fasting"))
                                {
                                    code = "1558-6";
                                }
                            }

                            if (!string.IsNullOrEmpty(code) && !extGlucose.Code.Contains("~"))
                            {
                                extGlucose.Code = extGlucose.Code + "~" + code;
                                externalReader.AddGlucose(extGlucose);
                            }
                        }
                    }
                    if (pogoReadingRequest.target != null)
                    {
                        var extTarget = new Ext_GlucoseSetting();
                        extTarget.GlucoseId = extGlucose.Id;
                        extTarget.a1c = pogoReadingRequest.target.e_a1c_target;
                        extTarget.High = pogoReadingRequest.target.target_high;
                        extTarget.Low = pogoReadingRequest.target.target_low;
                        extTarget.Hyper = pogoReadingRequest.target.hyper_limit;
                        extTarget.Hypo = pogoReadingRequest.target.hypo_limit;
                        extTarget.PostMealHigh = pogoReadingRequest.target.post_meal_high;
                        extTarget.PostMealLow = pogoReadingRequest.target.post_meal_low;
                        extTarget.PreMealHigh = pogoReadingRequest.target.pre_meal_high;
                        extTarget.PreMealLow = pogoReadingRequest.target.pre_meal_low;
                        externalReader.AddGlucoseTarget(extTarget);
                    }
                }
            }
            if (!response.Status)
            {
                response.Request = JsonConvert.SerializeObject(pogoReadingRequest);
            }
            return response;
        }
        #endregion

        #region Reminder

        public void SendReminderList(string intuityURL, string authToken)
        {
            try
            {
                ServiceReader serviceReader = new ServiceReader();
                var aptList = serviceReader.GetIntuityMessageCallList();
                if (aptList != null)
                {
                    NotificationRequest request = new NotificationRequest();
                    request.Users = new List<ReminderUser>();
                    ExternalReader reader = new ExternalReader();
                    foreach (var apt in aptList)
                    {
                        ReminderUser user = new ReminderUser();
                        var intuityUser = reader.GetIntuityUsersByUserId(apt.UserId);
                        if (intuityUser == null)
                            continue;
                        user.user_id = intuityUser.ExternalUserId;
                        user.header_text = "Appointment Reminder";
                        user.type = (int)IntuityEnum.ReminderType.coaching;
                        user.body_text = string.Format("You have an appintment at {0}", apt.AptTime.ToString("hh:mm tt"));
                        request.Users.Add(user);
                    }
                    request.intuityURL = intuityURL;
                    request.authToken = authToken;
                    Intuity.SendReminderNotification(request);
                }
            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }

        }

        public void SendNoShow(int userId, DateTime aptTime, string intuityURL, string authToken)
        {
            try
            {
                NotificationRequest request = new NotificationRequest();
                request.Users = new List<ReminderUser>();

                ExternalReader reader = new ExternalReader();
                ReminderUser user = new ReminderUser();
                var intuityUser = reader.GetIntuityUsersByUserId(userId);
                if (intuityUser == null)
                    return;
                user.user_id = intuityUser.ExternalUserId;
                user.header_text = "NoShow";
                user.type = (int)IntuityEnum.ReminderType.missed_coaching;
                user.body_text = string.Format("You had an appintment at {0} and you missed it", aptTime.ToString("hh:mm tt"));
                request.Users.Add(user);
                request.intuityURL = intuityURL;
                request.authToken = authToken;
                Intuity.SendReminderNotification(request);

            }
            catch (Exception ex)
            {
                LogReader logReader = new LogReader();
                var logEvent = new LogEventInfo(LogLevel.Error, "IntuityService", null, ex.Message, null, ex);
                logReader.WriteLogMessage(logEvent);
            }

        }
        #endregion

        #region get external data

        public bool AddEXTFoodData(EXT_FoodRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (user != null)
            {
                EXT_Nutrition food = new EXT_Nutrition();
                food.UserId = user.UserId;
                food.Calories = request.calories;
                food.Carbohydrates = request.carbohydrates;
                food.Fiber = request.dietary_fiber;
                food.Fat = request.fat;
                food.InputMethod = request.input_method;
                food.Name = request.name;
                food.Protein = request.protein;
                food.ReadingId = request.reading_id;
                food.Servings = request.servings;
                food.ServingSize = request.serving_size;
                food.ServingUnit = request.serving_unit;
                food.Sodium = request.sodium;
                food.Source = request.source;
                food.TimeStamp = request.timestamp;
                food.IsActive = ValidateIsActive(request.deleted_at);
                food.Weight = request.weight;
                food.ExternalId = request.id.ToString();
                externalReader.AddExtFood(food);
                return true;
            }
            return false;
        }

        public bool AddEXTSummaryData(EXT_SummaryRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (user != null)
            {
                EXT_Summaries summary = new EXT_Summaries();
                summary.UserId = user.UserId;
                summary.ActiveDuration = request.active_duration;
                summary.Caloriesbmr = request.calories_bmr;
                summary.CaloriesBurned = request.calories_burned;
                summary.CaloriesBurnedbyActivity = request.calories_burned_by_activity;
                summary.Distance = request.distance;
                summary.EndTimeStamp = request.end_timestamp;
                summary.Floors = request.floors;
                summary.InputMethod = request.input_method;
                summary.Source = request.source;
                summary.StartTimeStamp = request.start_timestamp;
                summary.Steps = request.steps;
                summary.Water = request.water;
                summary.ExternalId = request.id.ToString();
                externalReader.AddExtSummary(summary);
                return true;
            }
            return false;
        }

        public bool AddEXTWeightData(EXT_WeightRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (request.weight == 0)
                return true;
            if (user != null)
            {
                EXT_Weights weight = new EXT_Weights();
                weight.UserId = user.UserId;
                weight.bmi = request.bmi;
                weight.IsActive = ValidateIsActive(request.deleted_at);
                weight.FatPercent = request.fat_percent;
                weight.InputMethod = request.input_method;
                weight.Source = request.source;
                weight.Weight = request.weight * 2.205;
                weight.TimeStamp = request.timestamp;
                weight.ExternalId = request.id.ToString();
                externalReader.AddExtWeight(weight, systemAdminId);
                return true;
            }
            return false;
        }

        public bool AddEXTSleepData(EXT_SleepRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (user != null)
            {
                EXT_Sleeps sleep = new EXT_Sleeps();
                sleep.UserId = user.UserId;
                sleep.AwakeCount = request.awake_count;
                sleep.AwakeDuration = request.awake_duration;
                sleep.DeepDuration = request.deep_duration;
                sleep.IsActive = ValidateIsActive(request.deleted_at);
                sleep.InputMethod = request.input_method;
                sleep.LightDuration = request.light_duration;
                sleep.RemDuration = request.rem_duration;
                sleep.Source = request.source;
                sleep.StartTimeStamp = request.start_timestamp;
                sleep.TotalSleepDuration = request.total_sleep_duration;
                sleep.ExternalId = request.id.ToString();
                externalReader.AddExtSleep(sleep);
                return true;
            }
            return false;
        }

        public bool AddEXTWorkoutData(EXT_WorkoutRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (user != null)
            {
                EXT_Workouts workout = new EXT_Workouts();
                workout.UserId = user.UserId;
                workout.CaloriesBurned = request.calories_burned;
                workout.Category = request.category;
                workout.IsActive = ValidateIsActive(request.deleted_at);
                workout.Distance = request.distance;
                workout.Duration = request.duration;
                workout.EndTimeStamp = request.end_timestamp;
                workout.InputMethod = request.input_method;
                workout.Name = request.name;
                workout.Source = request.source;
                workout.StartTimeStamp = request.start_timestamp;
                workout.ExternalId = request.id.ToString();
                externalReader.AddExtWorkout(workout);
                return true;
            }
            return false;
        }

        public bool AddEXTBloodPressureData(EXT_BloodPressureRequest request)
        {
            ExternalReader externalReader = new ExternalReader();
            var user = externalReader.GetIntuityUsersByExtUserId(request.user_id);
            if (user != null)
            {
                EXT_BloodPressures bloodpressure = new EXT_BloodPressures();
                bloodpressure.UserId = user.UserId;
                bloodpressure.IsActive = ValidateIsActive(request.deleted_at);
                bloodpressure.Diastolic = request.diastolic;
                bloodpressure.InputMethod = request.input_method;
                bloodpressure.RestingHeartRate = request.resting_heart_rate;
                bloodpressure.Source = request.source;
                bloodpressure.Systolic = request.systolic;
                bloodpressure.TimeStamp = request.timestamp;
                bloodpressure.ExternalId = request.id.ToString();
                externalReader.AddExtBloodPressure(bloodpressure);
                return true;
            }
            return false;
        }

        private bool ValidateIsActive(DateTime date)
        {
            if (date == null || date.Equals(DateTime.MinValue))
                return true;
            else
                return false;
        }
        #endregion
    }
}
