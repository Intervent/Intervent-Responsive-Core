using Intervent.Business.Account;
using Intervent.Business.Eligibility;
using Intervent.Business.Organization;
using Intervent.DAL;
using Intervent.Framework.Cryptography;
using Intervent.Web.DTO;
using InterventWebApp.Helpers;
using InterventWebApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using NLog;
using System.Globalization;
using System.Security.Claims;
using System.Text;

namespace InterventWebApp
{
    public class AccountController : AccountBaseController
    {
        protected Logger Log = LogManager.GetLogger(typeof(AccountController).FullName);
        private readonly AppSettings _appSettings;
        private readonly IHostEnvironment _environment;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(UserManager<ApplicationUser> userManager, IOptions<AppSettings> appSettings, IHostEnvironment environment)
        {
            _userManager = userManager;
            _appSettings = appSettings.Value;
            _environment = environment;
        }

        string GetCryptoPassword()
        {
            return "intervent for better health!";
        }

        byte[] GetCryptoSalt()
        {
            return BitConverter.GetBytes(DateTime.UtcNow.Date.ToBinary());
        }

        [HttpPost]
        public async Task<JsonResult> Login(LoginModel model)
        {
            var errMsg = string.Format(Translate.Message("L467"), model.UserName, model.OrganizationEmail);
            model.Password = Encoding.ASCII.GetString(Convert.FromBase64String(model.Password)).Substring(6);
            var response = await AccountUtility.GetUser(_userManager, model.UserName, model.Password, null, true, model.DeviceId, _appSettings.VerifyDeviceLogin);
            if (response.EmailConfirmed == false)
            {
                return Json("confirmEmail");
            }
            else if (response.User != null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                HttpContext.Session.SetInt32(SessionContext.UserId, response.User.Id);

                var claims = new List<Claim>
                {
                    new Claim("UserId", response.User.Id.ToString()),
                    new Claim("FullName", response.User.FirstName + " " + response.User.LastName),
                    new Claim("Module", GetGroups(response.User)),
                    new Claim("RoleCode", GetRoleCodes(response.User)),
                    new Claim("ExpirationUrl", response.User.Organization.Url),
                    new Claim("DeviceId", !string.IsNullOrEmpty(model.DeviceId) ? model.DeviceId : ""),
                    new Claim("SingleSignOn", "false"),
                    new Claim("MobileSignOn", "false"),
                };

                if (!string.IsNullOrEmpty(response.User.TimeZone))
                    claims.Add(new Claim("TimeZone", response.User.TimeZone));

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Json("success");
            }
            else if (response.error == null)
            {
                return Json(errMsg);
            }
            else if (!response.IsVerifiedDevice && response.error == "verify device")
            {
                return Json(new { verifyDevice = "verifyDevice", userId = response.UserId, emailId = response.EmailId, phoneNo = response.PhoneNo });
            }
            else
                return Json(response.error);
        }

        public static string GetGroups(UserDto user)
        {
            string moduleGroup = string.Empty;
            if (user != null && user.Roles != null && user.Roles.Count > 0)
            {
                if (user.Roles[0] != null && user.Roles[0].AdminModules != null && user.Roles[0].AdminModules.Count > 0)
                {
                    moduleGroup = string.Join(",", user.Roles[0].AdminModules.Select(mod => mod.Name));
                }
                //Add modules from all roles
                if (user.Roles.Count() > 1)
                {
                    for (int i = 1; i < user.Roles.Count(); i++)
                    {
                        if (user.Roles[i].AdminModules != null && user.Roles[i].AdminModules.Count > 0)
                        {
                            foreach (var module in user.Roles[i].AdminModules)
                            {
                                if (!moduleGroup.Contains(module.Name))
                                {
                                    moduleGroup += "," + module.Name;
                                }
                            }
                        }
                    }
                }
            }
            return moduleGroup;
        }

        public static string GetRoleCodes(UserDto user)
        {
            if (user != null && user.Roles != null && user.Roles.Count > 0 && user.Roles[0] != null)
            {
                return string.Join(",", user.Roles.Select(mod => mod.Code));
            }
            return string.Empty;
        }

        public ActionResult LogOut()
        {
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue && HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
            {
                ParticipantUtility.TrackTime(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.AdminId).Value, null, ParticipantUtility.GetTimeTrackingDispositionList().Where(x => x.Type == "Coaching (L)").Select(x => x.Id).FirstOrDefault(), true);
            }
            string deviceId = User.DeviceId();
            if (HttpContext.Session.GetInt32(SessionContext.ParticipantId).HasValue && !string.IsNullOrEmpty(deviceId))
            {
                AccountUtility.LogoutUser(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, deviceId);
            }
            else if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue && !string.IsNullOrEmpty(deviceId))
            {
                AccountUtility.LogoutUser(HttpContext.Session.GetInt32(SessionContext.AdminId).Value, deviceId);
            }

            var landingpage = HttpContext.Session.GetString(SessionContext.LandingPage);
            bool hasSp = !string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.HasSP));
            if (!CommonUtility.HasAdminRole(User.RoleCode()))
                CommonUtility.UpdateLastVisited(null, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            HttpContext.Session.Clear();
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (hasSp)
                return RedirectToAction("SendLogoutRequest", "Saml");
            else if (landingpage == null)
                return RedirectToAction("Index", "Home");
            else
                return Redirect(landingpage.ToString());
        }

        [HttpPost]
        public async Task<JsonResult> Register(UserRegistrationModel model)
        {
            var errMsg = Translate.Message("L1966");
            var campaignExpired = Translate.Message("L4266");
            var emailerrMsg = Translate.Message("L2560");
            var portalerr = string.Format(Translate.Message("L3197"), model.OrganizationEmail);
            if (!model.SignUpDate.HasValue)
                model.SignUpDate = DateTime.Now;
            var orgList = PortalUtility.ListOrganizations(new OrganizationListModel() { parentOrganizationId = Int32.Parse(model.OrganizationId), includeParentOrganization = true }).Organizations;
            if (orgList == null || orgList.Count() == 0)
                return Json(new { Success = false, ErrorMessage = portalerr });
            var orgResponse = orgList.Where(x => x.CouponCodes.Select(y => y.Code).ToList().Contains(model.CouponCode)).FirstOrDefault();
            if (orgResponse == null)
                return Json(new { Success = false, ErrorMessage = errMsg });
            var portal = orgResponse.Portals.Where(x => x.Active).OrderByDescending(x => x.Id).FirstOrDefault();
            if (portal == null)
                return Json(new { Success = false, ErrorMessage = portalerr });
            if (!string.IsNullOrEmpty(portal.CampaignEndDate) && Convert.ToDateTime(portal.CampaignEndDate).Date < model.SignUpDate.Value.Date)
                return Json(new { Success = false, ErrorMessage = campaignExpired });
            else if (DateTime.Now < Convert.ToDateTime(portal.StartDate))
                return Json(new { Success = false, ErrorMessage = String.Format(Translate.Message("L3196"), Convert.ToDateTime(portal.StartDate).ToShortDateString()) });
            CreateUserModel request = new CreateUserModel();
            UserDto user = new UserDto();
            user.Email = model.EmailAddress;
            user.PasswordHash = Encoding.ASCII.GetString(Convert.FromBase64String(model.Password)).Substring(6);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            if (model.DOB != DateTime.MinValue)
                user.DOB = model.DOB;
            user.UserName = model.EmailAddress;
            user.OrganizationId = orgResponse.Id.Value;
            user.EmailConfirmed = orgResponse.EmailValidationRequired;
            user.CouponCode = model.CouponCode;
            user.LanguagePreference = HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "en-us";
            user.TermsAccepted = true;
            if (!string.IsNullOrEmpty(model.SID))
                user.EmployeeId = model.SID;
            if (model.ProviderId != 0)
            {
                user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                user.UserDoctorInfoes.Add(new UserDoctorInfoDto { ProviderId = model.ProviderId });
            }
            request.user = user;
            request.rootPath = _environment.ContentRootPath;
            request.InfoEmail = _appSettings.InfoEmail;
            request.SecureEmail = _appSettings.SecureEmail;
            request.SMPTAddress = _appSettings.SMPTAddress;
            request.PortNumber = _appSettings.PortNumber;
            request.SecureEmailPassword = _appSettings.SecureEmailPassword;
            request.MailAttachmentPath = _appSettings.MailAttachmentPath;
            var User = await AccountUtility.CreateUser(_userManager, request);
            if (User.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.DeviceId))
                {
                    int loggedinDeviceId = AccountUtility.AddOrEditUserLoggedInDevice(User.userId, model.DeviceId);
                    string code = new Random().Next(0, 1000000).ToString("D6");
                    AccountUtility.AddSecurityCode(User.userId, code, loggedinDeviceId);
                    AccountUtility.VerifySecurityCode(User.userId, code, loggedinDeviceId);
                    AccountUtility.LogUserLastAccess(User.userId, true, model.DeviceId);
                }
                else
                {
                    AccountUtility.LogUserLastAccess(User.userId, true, null);
                }
                return Json(new { Success = true, EmailValidationRequired = orgResponse.EmailValidationRequired });
            }
            else
                if (User.error.Any(x => x.Description.Contains("is already taken")))
                return Json(new { Success = false, ErrorMessage = emailerrMsg });
            else
                return Json(new { Success = false, ErrorMessage = User.error.FirstOrDefault().Description });
        }

        [ModuleControl(Modules.Profile, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult CreateProfile()
        {
            return View();
        }

        [HttpPost]
        public async Task<JsonResult> RegisterUsingEligibility(UserRegistrationModel model)
        {
            var errMsg1 = (string.IsNullOrEmpty(model.PersonnelType) && model.FirstName != null) ? Translate.Message("L2466") : model.IDText != null ? string.Format(Translate.Message("L1967"), model.IDText) : string.Format(Translate.Message("L1967"), "Personnel");
            var errMsg2 = Translate.Message("L1968");
            var errMsg3 = Translate.Message("L1969");
            var errMsg4 = Translate.Message("L1970");
            var errMsg5 = Translate.Message("L1989");
            var errMsg6 = string.Format(Translate.Message("L2569"), model.OrganizationEmail);
            var errMsg7 = Translate.Message("L4266");
            var emailerrMsg = Translate.Message("L2560");
            if (!model.SignUpDate.HasValue)
                model.SignUpDate = DateTime.Now;
            GetEligibilityResponse eligibilityResponse = new GetEligibilityResponse();
            if (model.OrganizationId != _appSettings.CityofPoolerOrgId.ToString())
            {
                errMsg1 = Translate.Message("L4260");
            }
            if (model.OrganizationId != _appSettings.MetLifeGulfOrgId.ToString())
            {
                var portal = new AccountManager().CurrentPortalId(Convert.ToInt32(model.OrganizationId));
                var portalId = portal.PortalId;
                if (portal.CampaignEndDate.HasValue && portal.CampaignEndDate.Value < model.SignUpDate.Value.Date)
                {
                    return Json(new { Success = false, ErrorMessage = errMsg7 });
                }
                if (portalId.HasValue && (DateTime.Now <= Convert.ToDateTime(portal.EndDate)))
                {
                    if (DateTime.Now < Convert.ToDateTime(portal.StartDate))
                        return Json(new { Success = false, ErrorMessage = string.Format(Translate.Message("L3196"), portal.StartDate?.ToShortDateString()) });
                }
                else
                {
                    return Json(new { Success = false, ErrorMessage = string.Format(Translate.Message("L3197"), model.OrganizationEmail) });
                }
                PersonnelTypeBase personnelType = null;
                EligibilityUserEnrollmentTypeDto eligibilityUserType = null;
                string uniqueId;
                bool eBenChild = false;

                if (!string.IsNullOrEmpty(model.PersonnelType))
                {
                    if (Convert.ToInt32(model.OrganizationId) == _appSettings.eBenOrgId)
                    {
                        if (model.PersonnelType == EligibilityUserEnrollmentTypeDto.Employee.UserEnrollmentTypeKey)
                            uniqueId = model.UniqueID + EbenPersonnelType.Employee;
                        else if (model.PersonnelType == EligibilityUserEnrollmentTypeDto.Spouse.UserEnrollmentTypeKey)
                            uniqueId = model.UniqueID + EbenPersonnelType.Spouse;
                        else
                        {
                            uniqueId = model.UniqueID;
                            eBenChild = true;
                        }
                    }
                    else
                    {
                        personnelType = PersonnelTypeBase.GetOrganizationPersonnelType(Convert.ToInt32(model.OrganizationId));
                        eligibilityUserType = EligibilityUserEnrollmentTypeDto.GetByKey(model.PersonnelType);
                        uniqueId = model.UniqueID + personnelType.Code(eligibilityUserType);
                    }
                }
                else
                    uniqueId = model.UniqueID;
                if (eBenChild || eligibilityUserType == EligibilityUserEnrollmentTypeDto.Dependent || eligibilityUserType == EligibilityUserEnrollmentTypeDto.Child)
                {
                    var eligibilityByUniqueId = ParticipantUtility.GetSimilarEligibilities(uniqueId, portalId, eBenChild).eligibilityList;
                    if (eligibilityByUniqueId == null || eligibilityByUniqueId.Count() == 0)
                    {
                        return Json(new { Success = false, ErrorMessage = errMsg1 });
                    }
                    else
                    {
                        var eligibilityByNameDob = eligibilityByUniqueId.Where(x => (x.FirstName.ToLower() == model.FirstName.ToLower().Trim()) && (x.LastName.ToLower() == model.LastName.ToLower().Trim()) && (x.DOB.HasValue && (model.DOB == (x.DOB)))).FirstOrDefault();
                        if (eligibilityByNameDob != null && (eligibilityByNameDob.UserStatus == EligibilityUserStatusDto.Terminated))
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg6 });
                        }
                        else if (eligibilityByNameDob == null)
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg2 });
                        }
                        else
                        {
                            if (eBenChild)
                                uniqueId = eligibilityByNameDob.UniqueId;
                            eligibilityResponse.Eligibility = eligibilityByNameDob;
                        }
                    }
                }
                else
                {
                    eligibilityResponse = ParticipantUtility.GetEligibility(null, uniqueId, portalId);

                    if (eligibilityResponse == null || eligibilityResponse.Eligibility == null)
                    {
                        if (string.IsNullOrEmpty(model.PersonnelType) || eligibilityUserType == EligibilityUserEnrollmentTypeDto.Employee)
                            return Json(new { Success = false, ErrorMessage = errMsg1 });
                        else
                        {
                            //check if the primary user record exists
                            var primaryUniqueId = model.UniqueID + personnelType.Code(EligibilityUserEnrollmentTypeDto.Employee);
                            GetEligibilityResponse primaryEligibilityResponse = ParticipantUtility.GetEligibility(null, primaryUniqueId, portalId);
                            if (eligibilityResponse == null || primaryEligibilityResponse.Eligibility == null)
                            {
                                return Json(new { Success = false, ErrorMessage = errMsg1 });
                            }
                            else if (primaryEligibilityResponse.Eligibility.FirstName == model.FirstName && primaryEligibilityResponse.Eligibility.LastName == model.LastName
                                && primaryEligibilityResponse.Eligibility.DOB == model.DOB)
                            {
                                return Json(new { Success = false, ErrorMessage = errMsg5 });
                            }
                            else//redirect to verify primary user information DOB
                            {
                                //encrypt the user registration model and send it back to pass to the next verify action
                                AesCryptoWithPassword cryptoProvider = new AesCryptoWithPassword(GetCryptoPassword(), GetCryptoSalt());
                                return Json(new { Success = false, @Action = "VerifyPrimaryInfo", RegisterData = cryptoProvider.Encrypt(JsonConvert.SerializeObject(model)) });
                            }
                        }
                    }
                    else//if record exists, validate data
                    {
                        var eligibility = eligibilityResponse.Eligibility;
                        if (model.FirstName != null && model.FirstName.ToLower().Trim() != eligibility.FirstName.ToLower())
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg2 });
                        }
                        else if (model.LastName != null && model.LastName.ToLower().Trim() != eligibility.LastName.ToLower())
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg2 });
                        }
                        else if (eligibility.DOB.HasValue && Convert.ToDateTime(model.DOB) != eligibility.DOB.Value)//date field localization format?
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg2 });
                        }
                        else if (eligibility.UserStatus == EligibilityUserStatusDto.Terminated)
                        {
                            return Json(new { Success = false, ErrorMessage = errMsg6 });
                        }

                    }
                }
            }
            else
            {
                var organizationsList = PortalUtility.ListOrganizations(new OrganizationListModel() { parentOrganizationId = _appSettings.MetLifeGulfOrgId }).Organizations;
                if (organizationsList != null && organizationsList.Count > 0)
                {
                    var portalsList = organizationsList.SelectMany(x => x.Portals).ToList().Select(x => x.Id).ToList();
                    eligibilityResponse = ParticipantUtility.GetEligibilityFromMultiplePortals(null, model.UniqueID, portalsList);
                }
                if (eligibilityResponse == null || eligibilityResponse.Eligibility == null)
                {
                    return Json(new { Success = false, ErrorMessage = "Please verify your Unique ID number. " + Translate.Message("L4258") });
                }
                if (!string.IsNullOrEmpty(eligibilityResponse.Eligibility.Portal.CampaignEndDate) &&
                    Convert.ToDateTime(eligibilityResponse.Eligibility.Portal.CampaignEndDate).Date < model.SignUpDate.Value.Date)
                {
                    return Json(new { Success = false, ErrorMessage = errMsg7 });
                }
                eligibilityResponse.Eligibility.FirstName = model.FirstName;
                eligibilityResponse.Eligibility.LastName = model.LastName;
            }
            //Need to check if we need it
            if (!AccountUtility.CheckifRegistered(eligibilityResponse.Eligibility.UniqueId, eligibilityResponse.Eligibility.Portal.OrganizationId).recordExist)
            {
                var orgResponse = PortalUtility.ReadOrganization(null, Convert.ToInt32(model.OrganizationId)).organization;
                model.Password = Encoding.ASCII.GetString(Convert.FromBase64String(model.Password)).Substring(6);
                var User = await CreateAccount(eligibilityResponse.Eligibility, model.EmailAddress, model.Password, orgResponse.EmailValidationRequired, false, true);
                if (User.Succeeded)
                    return Json(new { Success = true, EmailValidationRequired = orgResponse.EmailValidationRequired });
                else
                {
                    string errMsg = User.error.FirstOrDefault().Description;
                    if (errMsg.Contains("is already taken"))
                        return Json(new { Success = false, ErrorMessage = emailerrMsg });
                    else
                        return Json(new { Success = false, ErrorMessage = errMsg });
                }
            }
            else
                return Json(new { Success = false, ErrorMessage = errMsg3 });

        }

        [HttpPost]
        public async Task<JsonResult> TelephonicRegistration(int eligibilityId, string emailAddress)
        {
            var errMsg1 = Translate.Message("L1971");
            var errMsg2 = Translate.Message("L1986");
            string regexPattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,4}$";
            if (String.IsNullOrEmpty(emailAddress) || !System.Text.RegularExpressions.Regex.IsMatch(emailAddress, regexPattern))
            {
                return Json(new { Success = false, ErrorMessage = errMsg1 });
            }
            string password = CommonUtility.GeneratePassword(10, 3);
            var eligibility = ParticipantUtility.GetEligibility(eligibilityId, null, null).Eligibility;
            if (eligibility.DOB.HasValue)
            {
                if (DateTime.Now < eligibility.DOB.Value.AddYears(18))
                    return Json(new { Success = false, ErrorMessage = errMsg2 });
            }
            var User = await CreateAccount(eligibility, emailAddress, password, false, false, true);
            if (User.Succeeded)
            {
                HttpContext.Session.SetInt32(SessionContext.ParticipantId, User.userId);
                return Json(new { Success = true, PortalUserId = User.userId });
            }
            else
                return Json(new { Success = false, ErrorMessage = User.error.First() });
        }

        public async Task<RegisterUserResponse> CreateAccount(EligibilityDto eligibility, string emailAddress, string password, bool confirmationRequired, bool welcomeEmail, bool TermsAccepted)
        {
            byte? gender = null;
            if (eligibility.Gender != null)
            {
                gender = eligibility.Gender.Key == GenderDto.Male.Key ? (byte)1 : (byte)2;
            }
            int? state = null;
            if (!string.IsNullOrEmpty(eligibility.State))
            {
                var stateResponse = CommonUtility.GetState(eligibility.State);
                if (stateResponse != null)
                    state = stateResponse.state.Id;
            }
            int? country = null;
            if (!string.IsNullOrEmpty(eligibility.Country))
            {
                var countryResponse = CommonUtility.GetCountry(eligibility.Country);
                if (countryResponse != null && countryResponse.country != null)
                    country = countryResponse.country.Id;
            }
            int createdBy;
            if (HttpContext != null && HttpContext.User != null && HttpContext.User.Identity.IsAuthenticated)
                createdBy = Convert.ToInt32(Convert.ToInt32(HttpContext.User.UserId()));
            else
                createdBy = _appSettings.SystemAdminId;
            CreateUserModel request = new CreateUserModel();
            UserDto user = new UserDto();
            emailAddress = emailAddress.Trim();
            user.Email = emailAddress;
            user.PasswordHash = password;
            user.FirstName = eligibility.FirstName;
            user.LastName = eligibility.LastName;
            user.MiddleName = eligibility.MiddleName;
            user.DOB = eligibility.DOB;
            user.Gender = gender;
            user.HomeNumber = eligibility.HomeNumber;
            user.WorkNumber = eligibility.WorkNumber;
            user.CellNumber = eligibility.CellNumber;
            user.Address = eligibility.Address;
            user.Address2 = eligibility.Address2;
            user.City = eligibility.City;
            user.State = state;
            user.Zip = eligibility.Zip;
            user.Country = country;
            user.UserName = emailAddress;
            user.UniqueId = eligibility.UniqueId;
            user.OrganizationId = eligibility.Portal.OrganizationId;
            user.EmailConfirmed = confirmationRequired;
            user.CreatedBy = createdBy;
            user.TermsAccepted = TermsAccepted;
            user.LanguagePreference = HttpContext != null && HttpContext.Session.GetString(SessionContext.LanguagePreference) != null ? HttpContext.Session.GetString(SessionContext.LanguagePreference) : "en-us";
            if (!String.IsNullOrEmpty(eligibility.Ref_PractNum))
            {
                user.PrimaryCarePhysician = 1;
                user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                var doctorInfo = new UserDoctorInfoDto();
                doctorInfo.Name = eligibility.Ref_FirstName + " " + eligibility.Ref_LastName;
                doctorInfo.PhoneNumber = eligibility.Ref_Phone;
                doctorInfo.Address = eligibility.Ref_OfficeName;
                doctorInfo.City = eligibility.Ref_City;
                var physicianState = CommonUtility.GetState(eligibility.Ref_StateOrProvince);
                if (physicianState != null)
                {
                    doctorInfo.State = physicianState.state.Id;
                    doctorInfo.Country = physicianState.state.CountryId;
                }
                doctorInfo.FaxNumber = eligibility.Ref_Fax;
                user.UserDoctorInfoes.Add(doctorInfo);
            }
            request.welcomeEmail = welcomeEmail;
            request.user = user;
            request.rootPath = _environment.ContentRootPath;
            request.InfoEmail = _appSettings.InfoEmail;
            request.SecureEmail = _appSettings.SecureEmail;
            request.SMPTAddress = _appSettings.SMPTAddress;
            request.PortNumber = _appSettings.PortNumber;
            request.SecureEmailPassword = _appSettings.SecureEmailPassword;
            request.MailAttachmentPath = _appSettings.MailAttachmentPath;
            var User = await AccountUtility.CreateUser(_userManager, request);
            return User;
        }

        [HttpPost]
        public async Task<JsonResult> VerifyPrimaryUserInformation(VerifyPrimaryUserInformationModel model)
        {
            var errMsg1 = Translate.Message("L1972");
            var errMsg2 = Translate.Message("L1970");
            var errMsg3 = Translate.Message("L2576");
            //decrypt the information
            AesCryptoWithPassword cryptoProvider = new AesCryptoWithPassword(GetCryptoPassword(), GetCryptoSalt());
            UserRegistrationModel userModel = JsonConvert.DeserializeObject<UserRegistrationModel>(cryptoProvider.Decrypt(model.RegisterData));
            var personnelType = PersonnelTypeBase.GetOrganizationPersonnelType(Convert.ToInt32(userModel.OrganizationId));
            var primaryUniqueId = userModel.UniqueID + personnelType.Code(EligibilityUserEnrollmentTypeDto.Employee);
            var portalId = new AccountManager().CurrentPortalId(Convert.ToInt32(userModel.OrganizationId)).PortalId;
            var primaryEligibilityResponse = ParticipantUtility.GetEligibility(null, primaryUniqueId, portalId);
            var primaryParticipant = primaryEligibilityResponse.Eligibility;
            if (model.DOB != primaryParticipant.DOB.Value.ToString("MM/dd/yyyy"))
            {
                return Json(new { Success = false, ErrorMessage = errMsg3 });
            }
            else
            {
                //make sure record is not added previously
                var userEnrollmentType = EligibilityUserEnrollmentTypeDto.GetByKey(userModel.PersonnelType);

                var eligibilityResponse = ParticipantUtility.GetEligibility(null, userModel.UniqueID + personnelType.Code(userEnrollmentType), portalId);
                if (eligibilityResponse.Eligibility == null)
                {
                    //create an entry in the eligibility table
                    EligibilityDto newParticipant = new EligibilityDto();
                    newParticipant.PortalId = portalId.Value;
                    newParticipant.DOB = Convert.ToDateTime(userModel.DOB);
                    newParticipant.Email = userModel.EmailAddress;
                    newParticipant.FirstName = userModel.FirstName;
                    newParticipant.LastName = userModel.LastName;
                    newParticipant.UniqueId = userModel.UniqueID + personnelType.Code(userEnrollmentType);
                    newParticipant.UserEnrollmentType = userEnrollmentType;
                    newParticipant.CreateDate = DateTime.UtcNow;
                    //primary user's information
                    newParticipant.EmployeeUniqueId = primaryParticipant.EmployeeUniqueId;
                    newParticipant.Address = primaryParticipant.Address;
                    newParticipant.Address2 = primaryParticipant.Address2;
                    newParticipant.City = primaryParticipant.City;
                    newParticipant.State = primaryParticipant.State;
                    newParticipant.Country = primaryParticipant.Country;
                    ParticipantUtility.CopyPrimaryEligibilityDetails(primaryParticipant, newParticipant);
                    AddEditEligibilityRequest eligibilityRequest = new AddEditEligibilityRequest();
                    eligibilityRequest.Eligibility = new EligibilityDto();
                    IEligibilityManager eligibilityManager = new EligibilityManager();
                    var newParticipantResponse = eligibilityManager.AddEditEligibilityRecord(new AddEditEligibilityRequest() { Eligibility = newParticipant });
                    //created by user - self registration
                    eligibilityManager.AddEligibilityImportLog("N", null, portalId.Value, newParticipant.UniqueId, false, newParticipantResponse.Eligibility.Id, firstName: newParticipant.FirstName, lastName: newParticipant.LastName, createdByUser: "Self Registration");
                }
            }
            return await RegisterUsingEligibility(userModel);
        }

        [Authorize]
        public ActionResult ChangePassword()
        {
            return PartialView("_ChangePassword");
        }

        public async Task<JsonResult> ValidateToken(string email, string Token)
        {
            var response = await AccountUtility.ValidateToken(_userManager, email, Token);
            if (response.Succeeded == true)
                return Json("success");
            else
                return Json("fail");
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> ChangePassword(ChangePasswordModel model)
        {
            var errMsg1 = Translate.Message("L1977");
            model.ConfirmPassword = Encoding.ASCII.GetString(Convert.FromBase64String(model.ConfirmPassword)).Substring(6);
            model.NewPassword = Encoding.ASCII.GetString(Convert.FromBase64String(model.NewPassword)).Substring(6);
            model.OldPassword = Encoding.ASCII.GetString(Convert.FromBase64String(model.OldPassword)).Substring(6);
            var errMsg2 = Translate.Message("L1976");
            var errMsg3 = String.Format(Translate.Message("L3050"), _appSettings.PreviousPasswordLimit);
            var Identity = new ClaimsIdentity(User.Identity);
            string deviceId = Identity.Claims.Where(c => c.Type == "DeviceId").Select(c => c.Value).FirstOrDefault();
            var response = await AccountUtility.GetUser(_userManager, User.Identity.Name, model.OldPassword, null, true, deviceId, _appSettings.VerifyDeviceLogin);
            if (response.User != null)
            {
                var updateresponse = await AccountUtility.ChangePassword(_userManager, Convert.ToInt32(Convert.ToInt32(User.UserId())), model.OldPassword, model.NewPassword);
                if (!updateresponse.success)
                {
                    return Json(new { Success = false, ErrorMessage = errMsg3 });
                }
                return Json(new { Success = true, SuccessMessage = errMsg1 });
            }
            else
                return Json(new { Success = false, ErrorMessage = errMsg2 });
        }

        [HttpPost]
        public async Task<JsonResult> ForgotPassword(string email, string OrgContactEmail)
        {
            var response = await AccountUtility.ForgotPassword(_userManager, _environment.ContentRootPath, email, OrgContactEmail, _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ResendConfirmEmail(string email, string OrgContactEmail)
        {
            var response = await AccountUtility.ResendConfirmEmail(_userManager, _environment.ContentRootPath, email, OrgContactEmail, _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
            return Json(response);
        }

        [HttpPost]
        public async Task<JsonResult> ConfirmEmailAsync(string Email, string Token)
        {
            ConfirmEmailModel model = new ConfirmEmailModel();
            model.Email = Email;

            //Cannot verify
            //http://tech.trailmax.info/2015/05/asp-net-identity-invalid-token-for-password-reset-or-email-confirmation/
            //var result = await AccountUtility.VerifyUserToken(UserId, "ResetPassword", Token);
            //if (result.success == false)
            //    model.FailureMessage = "An error has occurred. Either the password link has expired or the key is not valid";
            var response = await AccountUtility.ConfirmEmailAsync(_userManager, model.Email, Token);
            if (!response.Succeeded)
                return Json(response.error.FirstOrDefault());
            else
                return Json("success");
        }

        [HttpPost]
        public async Task<JsonResult> ResetPassword(ResetPasswordModel model)
        {
            DateTime? dob = null;
            model.NewPassword = Encoding.ASCII.GetString(Convert.FromBase64String(model.NewPassword)).Substring(6);
            if (!string.IsNullOrEmpty(model.DOB))
            {
                if (model.DOB.Contains("-"))
                    model.DOB = model.DOB.Replace("-", "/");
                DateTime.ParseExact(model.DOB, "MM/dd/yyyy", CultureInfo.InvariantCulture);
            }
            var response = await AccountUtility.ResetPassword(_userManager, model.Email, model.Token, model.NewPassword, dob);
            if (response.success == true)
                return Json("success");
            else if (response.validateDOB == false)
                return Json("wrongDOB");
            else if (response.message == "Cannot reuse old password")
                return Json(String.Format("You cannot use any of your previous {0} passwords", _appSettings.PreviousPasswordLimit));
            else
                return Json("fail");
        }

        [Authorize]
        public async Task<ActionResult> UserProfile(string module, bool? create)
        {
            UserProfileModel model = new UserProfileModel();
            model.gender = HttpContext.Session.GetInt32(SessionContext.Gender);
            model.coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
            model.selfHelpProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null ? HttpContext.Session.GetString(SessionContext.SelfHelpProgram) : false);
            if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                model.adminId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            model.sso = HttpContext.Session.GetString(SessionContext.SSO) != null ? HttpContext.Session.GetString(SessionContext.SSO) : "";
            if (HttpContext.Session.GetInt32(SessionContext.IntegrationWith).HasValue)
                model.integrationWith = HttpContext.Session.GetInt32(SessionContext.IntegrationWith).ToString();
            model.orgContactEmail = HttpContext.Session.GetString(SessionContext.OrgContactEmail);
            model.PreferredContactTimes = ListOptions.GetPreferredContactTimes().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.ContactModes = ListOptions.GetPreferredContactMode().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.Countries = CommonUtility.ListCountries().OrderBy(t => t.Code == "US" ? 1 : 2).ThenBy(t => t.Name).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.TimeZones = CommonUtility.GetTimeZones(null).TimeZones.Select(x => new SelectListItem { Text = Translate.Message(x.TimeZoneDisplay), Value = x.Id.ToString() })
                .OrderBy(t => t.Text);
            model.NamePrefixList = ListOptions.GetNamePrefixList().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.GenderList = ListOptions.GetGenderList(null).Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.LanguagePreferences = CommonUtility.GetPortalLanguages(null, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).HasValue ? HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value : null).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.LanguageCode });
            model.Units = ListOptions.GetUnits().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.years = CommonUtility.GetYears(true).Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.months = CommonUtility.GetMonths().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.days = CommonUtility.GetDays().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.Sources = ListOptions.GetSource().Select(x => new SelectListItem { Text = Translate.Message(x.DisplayText), Value = x.Value });
            model.InactiveReason = AccountUtility.ListInactiveReason().Select(x => new SelectListItem { Text = x.Reason, Value = x.Id.ToString() });
            model.Departments = CommonUtility.ListDepartments(HttpContext.Session.GetInt32(SessionContext.OrganizationId).HasValue ? HttpContext.Session.GetInt32(SessionContext.OrganizationId).Value : 0).Departments.Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() });
            model.ProvidersList = new List<SelectListItem>();
            if (create.HasValue && create.Value)
            {
                model.ListOrganizations = PortalUtility.GetFilteredOrganizationsList(HttpContext.Session.GetInt32(SessionContext.UserId).Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text);
                model.EligibilityImportLoadList = PortalUtility.GetEligibilityImportLoadList();
            }
            model.States = Enumerable.Empty<SelectListItem>();
            model.DoctorInfo_States = Enumerable.Empty<SelectListItem>();
            model.RaceList = Enumerable.Empty<SelectListItem>();
            UserDto user = new UserDto();
            if (create != true)
            {
                int userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                var response = await AccountUtility.GetUser(_userManager, null, null, userId, null, null);
                var PortalId = GetParticipantPortalId();
                model.ProviderDropDown = PortalUtility.ReadPortal(PortalId).portal.ProviderDetails == (byte)ProviderDetails.DropDown;
                model.ProvidersList = CommonUtility.GetProvidersList(response.User.OrganizationId).Where(x => x.Active || (response.User.UserDoctorInfoes.Count > 0 && x.Id == response.User.UserDoctorInfoes[0].ProviderId)).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(y => y.Text);
                user.Id = response.User.Id;
                user.NamePrefix = response.User.NamePrefix;
                user.FirstName = response.User.FirstName;
                user.LastName = response.User.LastName;
                user.MiddleName = response.User.MiddleName;
                user.Suffix = response.User.Suffix;
                if (response.User.DOB.HasValue)
                {
                    model.year = response.User.DOB.Value.Year;
                    model.month = response.User.DOB.Value.Month;
                    model.day = response.User.DOB.Value.Day;
                }
                user.Gender = response.User.Gender;
                user.Race = response.User.Race;
                user.RaceOther = response.User.RaceOther;
                user.Address = response.User.Address;
                user.Address2 = response.User.Address2;
                user.City = response.User.City;
                user.State = response.User.State;
                user.Country = response.User.Country;
                user.Zip = response.User.Zip;
                if (!response.User.Email.EndsWith("@samlnoemail.com"))
                    user.Email = response.User.Email;
                user.EmailConfirmed = response.User.EmailConfirmed;
                user.UniqueId = response.User.UniqueId;
                user.HomeNumber = response.User.HomeNumber;
                user.WorkNumber = response.User.WorkNumber;
                user.CellNumber = response.User.CellNumber;
                user.ContactMode = response.User.ContactMode;
                user.PreferredContactTimeId = response.User.PreferredContactTimeId;
                user.TimeZoneId = response.User.TimeZoneId;
                user.LanguagePreference = response.User.LanguagePreference;
                user.Picture = response.User.Picture;
                user.Source = response.User.Source;
                user.SourceOther = response.User.SourceOther;
                user.ReferralDetails = response.User.ReferralDetails;
                user.Occupation = response.User.Occupation;
                user.Text = response.User.Text;
                user.Unit = response.User.Unit;
                user.IsActive = response.User.IsActive;
                user.InactiveReason = response.User.InactiveReason;
                user.PrimaryCarePhysician = response.User.PrimaryCarePhysician;
                user.Complete = response.User.Complete;
                user.DeptId = response.User.DeptId;
                if (user.LanguagePreference != null && user.LanguagePreference.Equals("fr"))
                {
                    model.NamePrefixList = model.NamePrefixList.Where(x => x.Value != "MS");
                }
                if (response.User.UserDoctorInfoes != null && response.User.UserDoctorInfoes.Where(x => x.Active == true).Count() > 0)
                {
                    var doctorInfo = response.User.UserDoctorInfoes.Where(x => x.Active == true).FirstOrDefault();
                    user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                    UserDoctorInfoDto userDoctorInfo = new UserDoctorInfoDto();
                    userDoctorInfo.ContactPermission = doctorInfo.ContactPermission;
                    userDoctorInfo.Name = doctorInfo.Name;
                    userDoctorInfo.Address = doctorInfo.Address;
                    userDoctorInfo.City = doctorInfo.City;
                    userDoctorInfo.Country = doctorInfo.Country;
                    if (userDoctorInfo.Country.HasValue)
                        model.DoctorInfo_States = CommonUtility.ListStates(doctorInfo.Country.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
                    userDoctorInfo.State = doctorInfo.State;
                    userDoctorInfo.FaxNumber = doctorInfo.FaxNumber;
                    userDoctorInfo.PhoneNumber = doctorInfo.PhoneNumber;
                    userDoctorInfo.Zip = doctorInfo.Zip;
                    userDoctorInfo.ProviderId = doctorInfo.ProviderId;
                    user.UserDoctorInfoes.Add(userDoctorInfo);
                }
                if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                {
                    if (user.Gender.HasValue && response.User.DOB.HasValue)
                        model.readOnlyList = new List<string>() { "FirstName", "LastName", "Gender", "DOB" };
                }
                if (user.Country.HasValue)
                {
                    model.States = CommonUtility.ListStates(user.Country.Value).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).OrderBy(x => x.Text);
                    model.RaceList = CommonUtility.ListRace(user.Country.Value).Select(x => new SelectListItem { Text = Translate.Message(x.LanguageCode), Value = x.Id.ToString() });
                }
                if (response.User.LockoutEndDateUtc.HasValue && response.User.LockoutEndDateUtc > DateTime.UtcNow)
                    model.IsLocked = model.LockOption = true;
            }
            model.user = user;
            model.user.BaseUrl = _appSettings.EmailUrl;
            model.module = module;
            model.create = create;
            return PartialView("_UserProfile", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<JsonResult> UserProfile(UserProfileModel model)
        {
            var emailerrMsg = Translate.Message("L2560");
            var error = "";
            if (model.create != true)
            {
                int portalId = GetParticipantPortalId();
                bool coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
                bool selfHelpProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null ? HttpContext.Session.GetString(SessionContext.SelfHelpProgram) : false);
                var response = await AccountUtility.UpdateUser(_userManager, model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetString(SessionContext.UniqueId), portalId, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.UserinProgramId), coachingProgram, selfHelpProgram, _appSettings.SystemAdminId);
                if (model.user.Gender.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.Gender, model.user.Gender.Value);
                if (model.user.OrganizationId > 0)
                    HttpContext.Session.SetInt32(SessionContext.OrganizationId, model.user.OrganizationId);
                if (model.year != 0 && model.month != 0 && model.day != 0)
                    HttpContext.Session.SetString(SessionContext.DOB, new DateTime(model.year, model.month, model.day).ToString());
                model.user.FirstName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.user.FirstName.ToLower());
                model.user.LastName = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(model.user.LastName.ToLower());
                HttpContext.Session.SetString(SessionContext.ParticipantName, model.user.FirstName + " " + model.user.LastName);
                HttpContext.Session.SetInt32(SessionContext.Unit, (int)model.user.Unit);
                if (!string.IsNullOrEmpty(model.user.LanguagePreference) && string.IsNullOrEmpty(model.module))
                {
                    HttpContext.Session.SetString(SessionContext.LanguagePreference, model.user.LanguagePreference);
                    HttpContext.Session.SetString(SessionContext.ParticipantLanguagePreference, model.user.LanguagePreference);
                }
                if (!HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                {
                    var Identity = new ClaimsIdentity(User.Identity);
                    Identity.TryRemoveClaim(Identity.FindFirst("TimeZone"));
                    Identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZoneId));
                    //AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties { IsPersistent = true });
                }
                else
                {
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZone, CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZoneId);
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZoneName, CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZone1);
                }
                if (response.Succeeded)
                {
                    if (!String.IsNullOrEmpty(model.user.UniqueId))
                        HttpContext.Session.SetString(SessionContext.UniqueId, model.user.UniqueId);
                    return Json("success");
                }
                else
                    error = response.error.FirstOrDefault().Description;
                if (error.Contains("is already taken"))
                    return Json(emailerrMsg);
                else
                    return Json(response.error);
            }
            else
            {
                string password = model.user.FirstName.Substring(0, 1).ToUpper() + (model.user.FirstName.Length >= 4 ? model.user.FirstName.Substring(1, 3).ToLower() : model.user.FirstName.Substring(1, model.user.FirstName.Length - 1).ToLower()) +
                                  model.user.LastName.Substring(0, 1).ToUpper() + (model.user.LastName.Length >= 4 ? model.user.LastName.Substring(1, 3).ToLower() : model.user.LastName.Substring(1, model.user.LastName.Length - 1).ToLower()) + model.year;
                CreateUserModel request = new CreateUserModel();
                request.user = model.user;
                request.user.DOB = new DateTime(model.year, model.month, model.day);
                request.user.UserName = request.user.Email;
                request.user.PasswordHash = password;
                request.user.EmailConfirmed = false;
                request.user.Complete = true;
                request.user.TermsAccepted = true;
                request.welcomeEmail = true;
                request.rootPath = _environment.ContentRootPath;
                request.InfoEmail = _appSettings.InfoEmail;
                request.SecureEmail = _appSettings.SecureEmail;
                request.SMPTAddress = _appSettings.SMPTAddress;
                request.PortNumber = _appSettings.PortNumber;
                request.SecureEmailPassword = _appSettings.SecureEmailPassword;
                request.MailAttachmentPath = _appSettings.MailAttachmentPath;
                var response = await AccountUtility.CreateUser(_userManager, request);
                if (response.Succeeded)
                {
                    HttpContext.Session.SetInt32(SessionContext.ParticipantId, response.userId);
                    return Json("success");
                }
                else
                    error = response.error.FirstOrDefault().Description;
                if (error.Contains("is already taken"))
                    return Json(emailerrMsg);
                else
                    return Json(response.error);
            }
        }

        [Authorize]
        public async Task<JsonResult> UpdateProfile(UserProfileModel model)
        {
            var emailerrMsg = Translate.Message("L2560");
            var error = "";
            int portalId = GetParticipantPortalId();
            bool coachingProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.CoachingProgram) != null ? HttpContext.Session.GetString(SessionContext.CoachingProgram) : false);
            bool selfHelpProgram = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.SelfHelpProgram) != null ? HttpContext.Session.GetString(SessionContext.SelfHelpProgram) : false);
            var response = await AccountUtility.UpdateUser(_userManager, model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetString(SessionContext.UniqueId), portalId, HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.UserinProgramId), coachingProgram, selfHelpProgram, _appSettings.SystemAdminId);
            if (response.Succeeded)
            {
                if (model.user.Gender.HasValue)
                    HttpContext.Session.SetInt32(SessionContext.Gender, model.user.Gender.Value);
                if (!string.IsNullOrEmpty(model.user.UniqueId))
                    HttpContext.Session.SetString(SessionContext.UniqueId, model.user.UniqueId);
                if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)) && model.user.TimeZoneId.HasValue)
                {
                    if (!HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                    {
                        var Identity = new ClaimsIdentity(User.Identity);
                        Identity.TryRemoveClaim(Identity.FindFirst("TimeZone"));
                        Identity.AddClaim(new Claim("TimeZone", CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZoneId));
                        //  AuthenticationManager.AuthenticationResponseGrant = new AuthenticationResponseGrant(new ClaimsPrincipal(Identity), new AuthenticationProperties { IsPersistent = true });
                    }
                    var TimeZones = CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones;
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZone, TimeZones.Select(x => x.TimeZoneId).FirstOrDefault());
                    HttpContext.Session.SetString(SessionContext.ParticipantTimeZoneName, CommonUtility.GetTimeZones(model.user.TimeZoneId).TimeZones[0].TimeZone1);
                }
                return Json("success");
            }
            else
                error = response.error.FirstOrDefault().Description;
            if (error.Contains("is already taken"))
                return Json(emailerrMsg);
            else
                return Json(response.error);
        }

        [Authorize]
        [HttpPost]
        public JsonResult RemovePicture(string picture, int? id, string source, bool? sendEmail, string reason)
        {
            string uri = Request.Headers["Referer"].ToString();

            if (!String.IsNullOrEmpty(picture))
            {
                if (!string.IsNullOrEmpty(source) && source == "Lab")
                {
                    System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "~/Lab", picture));
                    if (CommonUtility.HasAdminRole(User.RoleCode()) && sendEmail.HasValue && sendEmail.Value && !string.IsNullOrEmpty(reason))
                    {
                        string emailReason = ListOptions.GetLabRejectionReasons().FirstOrDefault(x => x.Value == reason).Text;
                        NotificationUtility.CreateRejectedLabNotificationEvent(NotificationEventTypeDto.RejectedLabs, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.OrgContactNumber), emailReason);
                    }
                    LabUtility.UpdateLabResultFile(id.Value, null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue ? HttpContext.Session.GetInt32(SessionContext.AdminId).Value : null, HttpContext.Session.GetInt32(SessionContext.IntegrationWith),User.RoleCode(), reason, null);
                    return Json(new { Result = "OK", AdminView = TempData["AdminView"] != null ? TempData["AdminView"].ToString() : "False" });
                }
                else if (uri.Contains("Recipe"))
                {
                    var recipeId = uri.Substring(uri.LastIndexOf("/") + 1);
                    System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "~/images/upload", picture));
                    RecipeUtility.UpdateImageUrl(int.Parse(recipeId), string.Empty);
                }
                else if (!string.IsNullOrEmpty(source) && source.Contains("Forms"))
                {
                    var userId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
                    System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "~/FormUploads", picture));
                    ParticipantUtility.DeleteUserForm(id.Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value);
                }
                else
                {
                    System.IO.File.Delete(Path.Combine(_environment.ContentRootPath, "~/ProfilePictures", picture));
                    AccountUtility.DeletePicture(_userManager, id.Value);
                }
                return Json("success");
            }
            else
                return Json("Failed");
        }

        public ActionResult NotAuthorized()
        {
            return View();
        }

        public ActionResult NotFoundError()
        {
            return View();
        }

        public ActionResult Error()
        {
            return View();
        }

        public async Task<ActionResult> EmailSubscription(string token)
        {
            EmailSubscriptionModel model = new EmailSubscriptionModel();
            string request = CommonUtility.Decrypt(token);
            if (!string.IsNullOrEmpty(request))
            {
                string[] result = request.Split(';');
                var user = await AccountUtility.GetUser(_userManager, null, null, int.Parse(result[0]), null, null);
                if (user != null && user.User != null && user.User.Email == result[1])
                {
                    model.userId = user.User.Id;
                    model.name = user.User.FirstName + " " + user.User.LastName;
                    model.emailId = user.User.Email;
                    model.unsubscribedEmail = user.User.UnsubscribedEmail;
                    model.emailCategory = NotificationUtility.ListNotificationCategory().Where(x => x.CanUnsubscribe).ToList();
                    return View(model);
                }
            }
            return RedirectToAction("Account", "NotAuthorized");
        }

        [HttpPost]
        public async Task<JsonResult> UpdateUserSubscription(int userId, string unsubscribeList)
        {
            var response = await AccountUtility.UpdateUserSubscription(_userManager, userId, unsubscribeList);
            if (response.Succeeded == true)
            {
                return Json("You have successfully set your subscription preferences");
            }
            else
            {
                return Json(response.error.FirstOrDefault().Description);
            }
        }

        public async Task<JsonResult> SendVerificationCode(int userId, string deviceId, bool throughEmail)
        {
            int loggedinDeviceId = AccountUtility.GetUserLoggedInDevice(userId, deviceId).Id;
            UserEligibleToSendCodeResponse response = AccountUtility.IsUserEligibleToSendCode(userId, loggedinDeviceId);
            if (response.isEligible)
            {
                var user = await AccountUtility.GetUser(_userManager, null, null, userId, null, null);
                string code = new Random().Next(0, 1000000).ToString("D6");
                bool sent;

                if (throughEmail)
                    sent = AccountUtility.SendSecurityCodeToEmail(_environment.ContentRootPath, user.User.FirstName, user.User.Email, code, _appSettings.InfoEmail, _appSettings.SecureEmail, _appSettings.SMPTAddress, _appSettings.PortNumber, _appSettings.SecureEmailPassword, _appSettings.MailAttachmentPath);
                else
                    sent = AccountUtility.SendSecurityCodeToPhone(user.User.Id, user.User.CellNumber, code);

                if (sent)
                {
                    AccountUtility.AddSecurityCode(userId, code, loggedinDeviceId);
                    return Json(new { Status = true, Message = "Success" });
                }
                else
                    return Json(new { Status = false, Message = "Something went wrong. Please try again." });
            }
            else
            {
                return Json(new { Status = false, Message = "try later", WaitingTime = TimeZoneInfo.ConvertTimeFromUtc(response.waitTime, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) + " Eastern Time." });
            }
        }

        public async Task<JsonResult> VerifySecurityCode(int userId, string code, string deviceId)
        {
            int loggedinDeviceId = AccountUtility.GetUserLoggedInDevice(userId, deviceId).Id;
            if (AccountUtility.VerifySecurityCode(userId, code, loggedinDeviceId))
            {
                var response = await AccountUtility.GetUser(_userManager, null, null, userId, null, null);
                if (response.User != null)
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    var identity = new ClaimsIdentity("InterventLogin");
                    HttpContext.Session.SetInt32(SessionContext.UserId, response.User.Id);
                    identity.AddClaim(new Claim("UserId", response.User.Id.ToString()));
                    identity.AddClaim(new Claim("FullName", response.User.FirstName + " " + response.User.LastName));
                    identity.AddClaim(new Claim("Module", GetGroups(response.User)));
                    identity.AddClaim(new Claim("RoleCode", GetRoleCodes(response.User)));
                    identity.AddClaim(new Claim("ExpirationUrl", response.User.Organization.Url));
                    identity.AddClaim(new Claim("DeviceId", deviceId));
                    identity.AddClaim(new Claim("SingleSignOn", "false"));
                    identity.AddClaim(new Claim("MobileSignOn", "false"));
                    if (!string.IsNullOrEmpty(response.User.TimeZone))
                        identity.AddClaim(new Claim("TimeZone", response.User.TimeZone));
                    var principal = new ClaimsPrincipal(identity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
                    AccountUtility.LogUserLastAccess(userId, true, deviceId);
                    return Json("Success");
                }
                return Json("Something went wrong.");
            }
            else
            {
                return Json("You have entered a wrong code.");
            }
        }

        #region ExternalLogin

        /*
        
        [HttpPost]
        [AllowAnonymous]
        public ActionResult ExternalLogin(string provider, string returnUrl)
        {
            // Request a redirect to the external login provider
            return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        }

        // GET: /Account/ExternalLoginCallback
        [AllowAnonymous]
        public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                return RedirectToAction("Login");
            }

            // Sign in the user with this external login provider if the user already has a login
            var user = await AccountUtility.FindExternalUser(loginInfo.Login);

            //if (user != null)
            //{
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var identity = await AccountUtility.CreateUserIdentity(user, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity.claimsIdentity);

                return RedirectToAction("MyPage");
            //}
            //else
            //{
            //    // If the user does not have an account, then prompt the user to create an account
            //    ViewBag.ReturnUrl = returnUrl;
            //    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
            //    return View("ExternalRegister", new ExternalRegisterModel { UserName = loginInfo.DefaultUserName, Email = loginInfo.Email, GenderList = CommonUtility.GetGenderList() });
            //}
        }

        [HttpPost]
        public async Task<ActionResult> ExternalRegister(ExternalRegisterModel model)
        {
            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }
            string uniqueId = null;
            int orgId = 2;
            CreateUserModel request = new CreateUserModel();
            request.Email = model.Email;
            request.firstName = model.FirstName;
            request.lastName = model.LastName;
            request.birthDate = model.BirthDate;
            request.gender = model.Gender;
            request.userName = model.Email;
            request.uniqueId = uniqueId;
            request.organizationId = orgId;
            var User = await AccountUtility.CreateUser(request, model.Email, null, null, model.FirstName,
                model.LastName, null, null, model.BirthDate, model.Gender, null, null, null, null, null, null, null, null, null, null, null, null, null, null, null,
                null, null, null, null, null, null, null, null, null, null, null, null, model.Email, uniqueId, orgId, false, null, null);

            var result1 = await AccountUtility.RegisterExternalUser(User.userId, info.Login);
            if (result1.Succeeded)
            {
                AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
                var userresponse = await AccountUtility.GetUser(null, null, User.userId, null);
                var identity = await AccountUtility.CreateUserIdentity(userresponse.User, DefaultAuthenticationTypes.ApplicationCookie);
                AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = true }, identity.claimsIdentity);
                return RedirectToAction("MyPage");
            }
            return View(model);
        }
        */
        #endregion
    }

    public class ChallengeResult : UnauthorizedResult
    {
        private const string XsrfKey = "XsrfId";

        public ChallengeResult(string provider, string redirectUri)
            : this(provider, redirectUri, null)
        {
        }

        public ChallengeResult(string provider, string redirectUri, string userId)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        public string LoginProvider { get; set; }

        public string RedirectUri { get; set; }

        public string UserId { get; set; }

        /*public override void ExecuteResult(ControllerContext context)
        {
            context.RequestContext.HttpContext.Response.SuppressFormsAuthenticationRedirect = true;

            var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }*/
    }
}