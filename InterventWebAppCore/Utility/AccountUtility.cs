using Intervent.Business;
using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Identity;
using System.Net;

namespace InterventWebApp
{
    public static class AccountUtility
    {
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return Convert.ToBase64String(plainTextBytes);
        }

        public static async Task<RegisterUserResponse> CreateUser(UserManager<ApplicationUser> userManager, CreateUserModel model)
        {
            RegisterUserRequest request = new RegisterUserRequest();
            UserDto user = new UserDto();
            user.UserName = model.user.UserName;
            user.Email = model.user.Email;
            user.EmailConfirmed = !model.user.EmailConfirmed;
            user.PasswordHash = model.user.PasswordHash;
            user.NamePrefix = model.user.NamePrefix;
            user.FirstName = model.user.FirstName.Trim();
            user.LastName = model.user.LastName.Trim();
            user.MiddleName = string.IsNullOrEmpty(model.user.MiddleName) ? model.user.MiddleName : model.user.MiddleName.Trim();
            user.Suffix = string.IsNullOrEmpty(model.user.Suffix) ? model.user.Suffix : model.user.Suffix.Trim();
            user.DOB = model.user.DOB;
            user.Gender = model.user.Gender;
            user.Race = model.user.Race;
            user.RaceOther = string.IsNullOrEmpty(model.user.RaceOther) ? model.user.RaceOther : model.user.RaceOther.Trim();
            user.HomeNumber = string.IsNullOrEmpty(model.user.HomeNumber) ? model.user.HomeNumber : model.user.HomeNumber.Trim();
            user.WorkNumber = string.IsNullOrEmpty(model.user.WorkNumber) ? model.user.WorkNumber : model.user.WorkNumber.Trim();
            user.CellNumber = string.IsNullOrEmpty(model.user.CellNumber) ? model.user.CellNumber : model.user.CellNumber.Trim();
            user.TimeZoneId = model.user.TimeZoneId;
            user.ContactMode = model.user.ContactMode;
            user.Occupation = string.IsNullOrEmpty(model.user.Occupation) ? model.user.Occupation : model.user.Occupation.Trim();
            user.Source = model.user.Source;
            user.SourceOther = string.IsNullOrEmpty(model.user.SourceOther) ? model.user.SourceOther : model.user.SourceOther.Trim();
            user.ReferralDetails = string.IsNullOrEmpty(model.user.ReferralDetails) ? model.user.ReferralDetails : model.user.ReferralDetails.Trim();
            user.LanguagePreference = model.user.LanguagePreference;
            user.Text = model.user.Text;
            user.PrimaryCarePhysician = model.user.PrimaryCarePhysician.HasValue ? model.user.PrimaryCarePhysician : model.user.UserDoctorInfoes != null && model.user.UserDoctorInfoes.Count > 0 && model.user.UserDoctorInfoes[0].ProviderId.HasValue ? 1 : user.PrimaryCarePhysician;
            user.Unit = model.user.Unit;
            user.IsActive = true;
            if (user.PrimaryCarePhysician == 1)
            {
                user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                user.UserDoctorInfoes.Add(model.user.UserDoctorInfoes[0]);
            }
            user.PreferredContactTimeId = model.user.PreferredContactTimeId;
            user.Address = string.IsNullOrEmpty(model.user.Address) ? model.user.Address : model.user.Address.Trim();
            user.Address2 = string.IsNullOrEmpty(model.user.Address2) ? model.user.Address2 : model.user.Address2.Trim();
            user.City = string.IsNullOrEmpty(model.user.City) ? model.user.City : model.user.City.Trim();
            user.State = model.user.State;
            user.Zip = string.IsNullOrEmpty(model.user.Zip) ? model.user.Zip : model.user.Zip.Trim();
            user.Country = model.user.Country;
            user.ProfessionId = model.user.ProfessionId;
            user.UniqueId = model.user.UniqueId;
            user.OrganizationId = model.user.OrganizationId;
            user.CreatedBy = model.user.CreatedBy;
            user.Complete = model.user.Complete;
            user.CouponCode = model.user.CouponCode;
            user.TermsAccepted = model.user.TermsAccepted;
            user.EmployeeId = model.user.EmployeeId;
            request.User = user;

            AccountReader reader = new AccountReader(userManager);
            var response = await reader.CreateUser(request);
            //confirmation email
            if (response.error == null && !string.IsNullOrEmpty(model.user.Email) && model.user.EmailConfirmed)
            {
                ConfirmationEmail(model.rootPath, model.user.OrganizationId, model.user.Email, model.user.LanguagePreference, response.Token, model.InfoEmail, model.SecureEmail, model.SMPTAddress, model.PortNumber, model.SecureEmailPassword, model.MailAttachmentPath);
            }
            //welcome email
            else if (response.error == null && !string.IsNullOrEmpty(model.user.Email) && model.welcomeEmail.HasValue && model.welcomeEmail.Value && !model.user.Email.Contains("noemail.myintervent.com") && !model.user.Email.Contains("samlnoemail.com"))
            {
                NotificationUtility.CreateWelcomeNotificationEvent(NotificationEventTypeDto.Welcome, response.userId, model.user.Email);
            }
            return response;
        }

        public static void ConfirmationEmail(string rootPath, int OrganizationId, string Email, string languagePreference, string Token, string InfoEmail, string SecureEmail, string SMPTAddress, string PortNumber, string SecureEmailPassword, string MailAttachmentPath)
        {
            PortalReader portalreader = new PortalReader();
            var orgResponse = portalreader.ReadOrganization(new ReadOrganizationRequest() { orgId = OrganizationId });
            string path, subject;
            if (languagePreference == "fr")
            {
                path = Path.Combine(rootPath, "Content/email/confirm-account-fr.html");
                subject = "Confirmation de création du compte";
            }
            else if (languagePreference == "pt")
            {
                path = Path.Combine(rootPath, "Content/email/confirm-account-pt.html");
                subject = "Confirmação de conta";
            }
            else
            {
                path = Path.Combine(rootPath, "Content/email/confirm-account.html");
                subject = "Account Confirmation";
            }
            string emailTemplate = System.IO.File.ReadAllText(path);
            emailTemplate = emailTemplate.Replace("{{EMAIL_URL}}", orgResponse.organization.Url);
            emailTemplate = emailTemplate.Replace("{{EMAIL}}", Email);
            emailTemplate = emailTemplate.Replace("{{TOKEN}}", WebUtility.UrlEncode(Token));
            CommonUtility.SendEmail(Email, emailTemplate, subject, InfoEmail, SecureEmail, SMPTAddress, PortNumber, SecureEmailPassword, MailAttachmentPath);
            NotificationUtility.CreateDirectMailEvent(NotificationEventTypeDto.ResendConfirmtaion, Email, emailTemplate);
        }

        public static async Task<GetUserResponse> GetValidUserByToken(UserManager<ApplicationUser> userManager, string token)
        {
            ExternalReader reader = new ExternalReader();
            var userName = reader.GetValidUserNameByToken(token);
            if (!string.IsNullOrEmpty(userName))
            {
                return await GetUser(userManager, userName, null, null, false, null);
            }
            return null;
        }

        public static async Task<GetUserResponse> GetUser(UserManager<ApplicationUser> userManager, string userName, string password, int? id, bool? validateUser, string deviceId, bool appLogin = false, bool verifyDeviceLogin = false)
        {
            AccountReader reader = new AccountReader(userManager);
            GetUserRequest request = new GetUserRequest();
            request.userName = userName;
            request.password = password;
            request.id = id;
            request.validateUser = validateUser;
            request.deviceID = deviceId;
            request.appLogin = appLogin;
            request.verifyDeviceLogin = verifyDeviceLogin;
            var response = await reader.GetUser(request);
            return response;
        }

        public static UserDoctorInfoDto GetUserDoctorInfo(int userId)
        {
            AccountReader reader = new AccountReader();
            var user = reader.GetUserById(userId);
            UserDoctorInfoDto response = new UserDoctorInfoDto();
            if (user.UserDoctorInfoes.Count > 0)
            {
                var userDoctorInfo = user.UserDoctorInfoes.Where(x => x.Active.HasValue && x.Active.Value).FirstOrDefault();
                if (userDoctorInfo != null)
                {
                    response.Name = userDoctorInfo.Provider != null ? userDoctorInfo.Provider.Name : userDoctorInfo.Name;
                    response.FaxNumber = userDoctorInfo.Provider != null ? userDoctorInfo.Provider.FaxNumber : userDoctorInfo.FaxNumber;
                }
            }
            return response;
        }

        public static async Task<VerifyUserResponse> VerifyUser(UserManager<ApplicationUser> userManager, string userName, string password, string deviceId)
        {
            AccountReader reader = new AccountReader(userManager);
            VerifyUserRequest request = new VerifyUserRequest();
            request.userName = userName;
            request.password = password;
            request.deviceID = deviceId;
            var response = await reader.VerifyUser(request);
            return response;
        }

        public static string GetPreferredPhoneNumber(int userId)
        {
            AccountReader accountReader = new AccountReader();
            var response = accountReader.GetUserById(userId);
            string phoneNumber = "";
            if (response != null && response.ContactMode.HasValue)
            {
                if (response.ContactMode == 1)
                    phoneNumber = response.HomeNumber;
                else if (response.ContactMode == 2)
                    phoneNumber = response.WorkNumber;
                else if (response.ContactMode == 3)
                    phoneNumber = response.CellNumber;
            }
            return phoneNumber;
        }

        public static bool IsProfileCompleted(UserDto user, int participantPortalId, bool coachingProgram, bool selfHelpProgram)
        {
            CommonReader reader = new CommonReader();
            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName) && user.Gender.HasValue && user.DOB.HasValue && !string.IsNullOrEmpty(user.Address) && !string.IsNullOrEmpty(user.City) && user.State.HasValue
                && user.Country.HasValue && (!reader.ListCountries(new ListCountriesRequest()).Countries.Where(x => x.Id == user.Country).FirstOrDefault().HasZipCode || !string.IsNullOrEmpty(user.Zip)) && user.Unit.HasValue
                && user.ContactMode.HasValue && ((user.ContactMode.Value == 1 && !string.IsNullOrEmpty(user.HomeNumber)) || (user.ContactMode.Value == 2 && !string.IsNullOrEmpty(user.WorkNumber)) || user.ContactMode.Value == 3 && !string.IsNullOrEmpty(user.CellNumber))
                && !string.IsNullOrEmpty(user.Email) && user.TimeZoneId.HasValue && !string.IsNullOrEmpty(user.LanguagePreference)
                && (!coachingProgram && !selfHelpProgram || (user.Text.HasValue && ((user.Text == 1 && !string.IsNullOrEmpty(user.CellNumber)) || user.Text == 2)))
                && (PortalUtility.ReadPortal(participantPortalId).portal.ProviderDetails == (byte)ProviderDetails.DropDown
                || (user.PrimaryCarePhysician.HasValue && ((user.PrimaryCarePhysician == 1 && user.UserDoctorInfoes.Count > 0 && user.UserDoctorInfoes[0].ContactPermission.HasValue) || user.PrimaryCarePhysician == 2))))
                return true;
            else
                return false;
        }

        public static ProfileCompletedStatus CheckProfileCompleted(UserDto user)
        {
            ProfileCompletedStatus response = new ProfileCompletedStatus();
            if (!string.IsNullOrEmpty(user.FirstName) && !string.IsNullOrEmpty(user.LastName) && user.Gender.HasValue && user.DOB.HasValue && !string.IsNullOrEmpty(user.Address) && !string.IsNullOrEmpty(user.City) && user.Country.HasValue && user.State.HasValue && !string.IsNullOrEmpty(user.Zip) && user.Unit.HasValue)
                response.basicDetails = true;
            if (!string.IsNullOrEmpty(user.HomeNumber) && !string.IsNullOrEmpty(user.Email) && user.TimeZoneId.HasValue && !string.IsNullOrEmpty(user.LanguagePreference))
                response.contactDetails = true;
            if (response.basicDetails && response.contactDetails)
                response.profileCompleted = true;
            return response;
        }

        public static GetUserResponse GetUserByUniqueId(int? organizationId, string uniqueId)
        {
            AccountReader reader = new AccountReader();

            GetUserRequestByUniqueId request = new GetUserRequestByUniqueId();
            request.OrganizationId = organizationId;
            request.UniqueId = uniqueId;
            var response = reader.GetUserByUniqueId(request);
            return response;
        }

        public static void LogUserLastAccess(int userId, bool status, string deviceId)
        {
            AccountReader reader = new AccountReader();
            reader.LogUserLastAccess(userId, status, deviceId);
        }

        public static async Task<UpdateUserResponse> UpdateUser(UserManager<ApplicationUser> userManager, UserProfileModel model, int participantId, int userId, string uniqueId, int participantPortalId, int? integrationWith, int? userinProgramId, bool coachingProgram, bool selfHelpProgram, int systemAdminId)
        {
            AccountReader reader = new AccountReader(userManager);
            string orgCode = "";
            int orgId = 0;
            bool? isComplete = true;
            if (model.basicDetails || model.contactDetails)
            {
                isComplete = null;
                GetUserRequest getUserRequest = new GetUserRequest();
                getUserRequest.id = participantId;
                var getUserResponse = Task.Run(() => reader.GetUser(getUserRequest));
                UserDto User = new UserDto();
                User = getUserResponse.Result.User;
                orgCode = User.Organization.Code;
                orgId = User.OrganizationId;
                if (model.basicDetails)
                {
                    User.FirstName = model.user.FirstName;
                    User.LastName = model.user.LastName;
                    User.Gender = model.user.Gender;
                    User.DOB = model.user.DOB;
                    User.Address = model.user.Address;
                    User.Address2 = model.user.Address2;
                    User.City = model.user.City;
                    User.Country = model.user.Country;
                    User.State = model.user.State;
                    User.Zip = model.user.Zip;
                    User.Unit = model.user.Unit;
                    User.PrimaryCarePhysician = null;
                    User.UserDoctorInfoes = model.user.UserDoctorInfoes;
                }
                else
                {
                    User.HomeNumber = model.user.HomeNumber;
                    User.Email = model.user.Email;
                    User.TimeZoneId = model.user.TimeZoneId;
                    User.LanguagePreference = model.user.LanguagePreference;
                    User.Text = model.user.Text;
                    User.UserDoctorInfoes = model.user.UserDoctorInfoes;
                }
                model.user = User;
            }
            UpdateUserRequest request = new UpdateUserRequest();
            request.UpdatedByUserId = userId;
            if (model.LockOption)
                request.IsLocked = model.IsLocked;
            UserDto user = new UserDto();
            user.Id = participantId;
            user.UniqueId = model.user.UniqueId;
            user.NamePrefix = model.user.NamePrefix;
            user.FirstName = model.user.FirstName.Trim();
            user.LastName = model.user.LastName.Trim();
            user.MiddleName = string.IsNullOrEmpty(model.user.MiddleName) ? model.user.MiddleName : model.user.MiddleName.Trim();
            user.Suffix = string.IsNullOrEmpty(model.user.Suffix) ? model.user.Suffix : model.user.Suffix.Trim();
            if (model.user.DOB.HasValue)
                user.DOB = model.user.DOB;
            else if (model.year != 0 && model.month != 0 && model.day != 0)
                user.DOB = new DateTime(model.year, model.month, model.day);
            user.Gender = model.user.Gender;
            user.Race = model.user.Race;
            user.RaceOther = string.IsNullOrEmpty(model.user.RaceOther) ? model.user.RaceOther : model.user.RaceOther.Trim();
            user.Address = string.IsNullOrEmpty(model.user.Address) ? model.user.Address : model.user.Address.Trim();
            user.Address2 = string.IsNullOrEmpty(model.user.Address2) ? model.user.Address2 : model.user.Address2.Trim();
            user.City = string.IsNullOrEmpty(model.user.City) ? model.user.City : model.user.City.Trim();
            user.State = model.user.State;
            user.Zip = string.IsNullOrEmpty(model.user.Zip) ? model.user.Zip : model.user.Zip.Trim();
            user.Country = model.user.Country;
            user.HomeNumber = string.IsNullOrEmpty(model.user.HomeNumber) ? model.user.HomeNumber : model.user.HomeNumber.Trim();
            user.WorkNumber = string.IsNullOrEmpty(model.user.WorkNumber) ? model.user.WorkNumber : model.user.WorkNumber.Trim();
            user.CellNumber = string.IsNullOrEmpty(model.user.CellNumber) ? model.user.CellNumber : model.user.CellNumber.Trim();
            user.Email = model.user.Email;
            user.ContactMode = model.user.ContactMode;
            user.PreferredContactTimeId = model.user.PreferredContactTimeId;
            user.Occupation = string.IsNullOrEmpty(model.user.Occupation) ? model.user.Occupation : model.user.Occupation.Trim();
            user.Source = model.user.Source;
            user.SourceOther = string.IsNullOrEmpty(model.user.SourceOther) ? model.user.SourceOther : model.user.SourceOther.Trim();
            user.ReferralDetails = string.IsNullOrEmpty(model.user.ReferralDetails) ? model.user.ReferralDetails : model.user.ReferralDetails.Trim();
            user.TimeZoneId = model.user.TimeZoneId;
            user.Text = model.user.Text;
            user.DeptId = model.user.DeptId;
            if (model.ProviderDropDown)
            {
                if (model.user.UserDoctorInfoes.Count > 0 && model.user.UserDoctorInfoes[0].ProviderId.HasValue)
                    model.user.PrimaryCarePhysician = 1;
                else
                    model.user.PrimaryCarePhysician = null;
            }
            user.PrimaryCarePhysician = model.user.PrimaryCarePhysician;
            user.Unit = model.user.Unit;
            if (!string.IsNullOrEmpty(model.user.LanguagePreference))
                user.LanguagePreference = model.user.LanguagePreference;
            user.IsActive = model.user.IsActive;
            user.InactiveReason = model.user.IsActive == false ? model.user.InactiveReason : null;
            request.user = user;
            if (model.user.PrimaryCarePhysician == 1)
            {
                user.UserDoctorInfoes = new List<UserDoctorInfoDto>();
                model.user.UserDoctorInfoes[0].UserId = participantId;
                user.UserDoctorInfoes.Add(model.user.UserDoctorInfoes[0]);
            }
            //after updateuser httpcontext  is set to null
            isComplete = IsProfileCompleted(user, participantPortalId, coachingProgram, selfHelpProgram);

            var response = await reader.UpdateUser(request, isComplete);
            if (response.Succeeded == true)
            {
                //update eligiblity
                if (integrationWith.HasValue && integrationWith.Value == (byte)Integrations.LMC && uniqueId != null)
                {
                    ParticipantReader participantReader = new ParticipantReader();
                    UpdateEligibilityRequest updateEligiblityRequest = new UpdateEligibilityRequest();
                    updateEligiblityRequest.PortalId = participantPortalId;
                    updateEligiblityRequest.ExistingUniqueId = uniqueId;
                    updateEligiblityRequest.FirstName = model.user.FirstName;
                    updateEligiblityRequest.LastName = model.user.LastName;
                    updateEligiblityRequest.DOB = new DateTime(model.year, model.month, model.day);
                    participantReader.UpdateEligiblity(updateEligiblityRequest);
                }
                var doctorResponse = reader.UpadateDoctorInfo(request);

                if (string.IsNullOrEmpty(orgCode) || orgId == 0)
                {
                    var orgDetail = reader.GetUserById(participantId).Organization;
                    orgCode = orgDetail.Code;
                    orgId = orgDetail.Id;
                }

                // Update Intuity eligiblity if exists
                IntuityReader intuityReader = new IntuityReader();
                if (intuityReader.HasIntuityEligibilityRecord(new GetIntuityRequest() { UniqueId = uniqueId, OrganizationId = orgId }) && model.user.ContactMode != null && model.user.ContactMode.HasValue)
                {
                    string intuityPhone = "";
                    if (model.user.ContactMode.Value == 1)
                        intuityPhone = user.HomeNumber;
                    else if (model.user.ContactMode.Value == 2)
                        intuityPhone = user.WorkNumber;
                    else if (model.user.ContactMode.Value == 3)
                        intuityPhone = user.CellNumber;

                    if (!string.IsNullOrEmpty(intuityPhone))
                    {
                        IntuityEligibilityModel intuityEligibility = new IntuityEligibilityModel();
                        intuityEligibility.IntuityEligibilityLog = new IntuityEligibilityLogDto();
                        intuityEligibility.UniqueId = uniqueId;
                        intuityEligibility.UpdatedBy = request.UpdatedByUserId;
                        intuityEligibility.OrganizationCode = orgCode;
                        intuityEligibility.ShowNewForm = false;
                        intuityEligibility.IntuityEligibilityLog.UniqueId = uniqueId;
                        intuityEligibility.IntuityEligibilityLog.OrganizationId = orgId;
                        intuityEligibility.IntuityEligibilityLog.FirstName = user.FirstName;
                        intuityEligibility.IntuityEligibilityLog.LastName = user.LastName;
                        intuityEligibility.IntuityEligibilityLog.AddressLine1 = user.Address;
                        intuityEligibility.IntuityEligibilityLog.AddressLine2 = user.Address2;
                        intuityEligibility.IntuityEligibilityLog.City = user.City;
                        intuityEligibility.IntuityEligibilityLog.State = user.State;
                        intuityEligibility.IntuityEligibilityLog.Country = user.Country;
                        intuityEligibility.IntuityEligibilityLog.Zip = user.Zip;
                        intuityEligibility.IntuityEligibilityLog.PhoneNumber = intuityPhone;
                        intuityEligibility.IntuityEligibilityLog.email = user.Email;

                        IntuityUtility.UpdateIntuityEligibility(intuityEligibility, userId);
                    }
                }
                if (response.activeStatusChanged && model.user.IsActive == false)
                {
                    ParticipantUtility.UpdateUserTrackingStatus(user.Id, participantPortalId, true, null, null);   //marking user "Do Not Track" 
                    if (userinProgramId.HasValue)
                    {
                        var userinProgram = ProgramUtility.GetUserinProgramDetails(userinProgramId.Value);
                        if (userinProgram != null && userinProgram.IsActive)
                        {
                            ProgramReader programReader = new ProgramReader();
                            UpdateUserinProgramRequest programRequest = new UpdateUserinProgramRequest();
                            programRequest.UsersinProgramId = userinProgramId.Value;
                            programRequest.InactiveReasonId = (int)ProgramInactiveReasons.ProfileDeactivated;
                            programRequest.userId = user.Id;
                            programRequest.LoginId = userId;
                            programRequest.PortalId = participantPortalId;
                            programRequest.systemAdminId = systemAdminId;
                            programReader.UpdateUserinProgram(programRequest);
                            ProgramUtility.ClearProgramRelatedSessions();
                        }
                    }
                }
            }
            if (response.primaryFieldsChanged)
            {
                GetAllHRAsforUserRequest hraRequest = new GetAllHRAsforUserRequest();
                hraRequest.UserId = user.Id;
                HRAReader hraReader = new HRAReader();
                var hraResponse = hraReader.GetAllHRAsforUser(hraRequest);
                if (hraResponse != null && hraResponse.HRAIds != null && hraResponse.HRAIds.Count > 0)
                {
                    for (int i = 0; i < hraResponse.HRAIds.Count; i++)
                    {
                        hraReader.StratifyHRA(hraResponse.HRAIds[i]);
                    }
                }
            }

            if (!string.IsNullOrEmpty(model.existingEmail))
            {
                string email = "";
                if (!model.user.Email.Contains("noemail.myintervent.com") && !model.user.Email.Contains("samlnoemail.com"))
                    email = model.user.Email + ",";
                if (!model.existingEmail.Contains("noemail.myintervent.com") && !model.existingEmail.Contains("samlnoemail.com"))
                    email = email + model.existingEmail;
                if (!string.IsNullOrEmpty(email) && !model.notifyEmailChange)
                    NotificationUtility.CreateChangePasswordNotificationEvent(NotificationEventTypeDto.ChangeEmail, request.user.Id, email, true);
            }
            return response;
        }

        public static async Task<ChangePasswordResponse> ChangePassword(UserManager<ApplicationUser> userManager, int userId, string oldPassword, string newPassword)
        {
            AccountReader reader = new AccountReader(userManager);
            ChangePasswordRequest request = new ChangePasswordRequest();
            request.UserId = userId;
            request.newPassword = newPassword;
            request.oldPassword = oldPassword;
            var response = await reader.ChangePassword(request);
            if (response != null && response.success && NotificationUtility.hasValidEmail(userId))
                NotificationUtility.CreateChangePasswordNotificationEvent(NotificationEventTypeDto.ChangePassword, userId, "");
            return response;
        }

        public static async Task<ValidateTokenResponse> ValidateToken(UserManager<ApplicationUser> userManager, string email, string token)
        {
            AccountReader reader = new AccountReader(userManager);
            ValidateTokenRequest request = new ValidateTokenRequest();
            request.email = email;
            request.token = token;
            var response = await reader.ValidateToken(request);
            return response;
        }

        public static async Task UploadPicture(UserManager<ApplicationUser> userManager, int userId, string fileName, string basepath)
        {
            AccountReader reader = new AccountReader(userManager);
            UploadProfilePictureRequest request = new UploadProfilePictureRequest();
            request.userId = userId;
            request.fileName = fileName;
            var oldPicture = await reader.UploadPicture(request);
            if (!string.IsNullOrEmpty(oldPicture))
            {
                var path = Path.Combine(basepath, oldPicture);
                File.Delete(path);
            }
        }

        public static IList<RoleDto> ListRoles()
        {
            AccountReader reader = new AccountReader();
            ListRolesRequest request = new ListRolesRequest();
            return reader.ListRoles(request).Roles;
        }

        public static IList<SpecializationDto> ListSpecialization(bool byPortal, int? participantPortalId)
        {
            AccountReader reader = new AccountReader();
            SpecializationRequest request = new SpecializationRequest();
            request.portalId = byPortal ? participantPortalId.Value : 0;
            return reader.ListSpecialization(request).specialization;
        }

        public static IList<UserInactiveReasonDto> ListInactiveReason()
        {
            AccountReader reader = new AccountReader();
            return reader.ListInactiveReason().InactiveReasons;
        }
        public static ReadRoleResponse ReadRole(int id)
        {
            AccountReader reader = new AccountReader();
            ReadRoleRequest request = new ReadRoleRequest();
            request.id = id;
            return reader.ReadRole(request);
        }

        public static AddEditRoleResponse AddEditRole(int? id, string name, string code, string adminModules, string organizations)
        {
            AccountReader reader = new AccountReader();
            AddEditRoleRequest request = new AddEditRoleRequest();
            RoleDto role = new RoleDto();
            if (id.HasValue)
                role.Id = id.Value;
            role.Name = name;
            role.Code = code;
            request.role = role;
            request.adminModules = adminModules;
            request.organizations = organizations;
            return reader.AddEditRole(request);
        }

        public static async Task<string> ForgotPassword(UserManager<ApplicationUser> userManager, string rootPath, string userName, string orgContactEmail, string InfoEmail, string SecureEmail, string SMPTAddress, string PortNumber, string SecureEmailPassword, string MailAttachmentPath)
        {
            AccountReader reader = new AccountReader(userManager);
            ForgotPasswordRequest forgotRequest = new ForgotPasswordRequest();
            forgotRequest.username = userName;
            var forgotResponse = await reader.ForgotPassword(forgotRequest);
            if (!string.IsNullOrEmpty(forgotResponse.resetToken))
            {
                string path, subject;
                if (forgotResponse.user.LanguagePreference == "fr")
                {
                    path = Path.Combine(rootPath, "Content/email/forgot-password-fr.html");
                    subject = "Mot de passe oublié";
                }
                else if (forgotResponse.user.LanguagePreference == "pt")
                {
                    path = Path.Combine(rootPath, "Content/email/forgot-password-pt.html");
                    subject = "Esqueci a senha";
                }
                else
                {
                    path = Path.Combine(rootPath, "Content/email/forgot-password.html");
                    subject = "Forgot Password";
                }
                string emailTemplate = File.ReadAllText(path);
                emailTemplate = emailTemplate.Replace("{{EMAIL_URL}}", forgotResponse.user.Organization.Url);
                emailTemplate = emailTemplate.Replace("{{EMAIL}}", userName);
                emailTemplate = emailTemplate.Replace("{{TOKEN}}", WebUtility.UrlEncode(forgotResponse.resetToken));
                emailTemplate = emailTemplate.Replace("{{RESET}}", "ResetPassword");
                CommonUtility.SendEmail(userName, emailTemplate, subject, InfoEmail, SecureEmail, SMPTAddress, PortNumber, SecureEmailPassword, MailAttachmentPath);
                NotificationUtility.CreateDirectMailEvent(NotificationEventTypeDto.ForgotPassword, userName, emailTemplate);
                return "success";
            }
            else
            {
                if (forgotResponse.Error.IndexOf("user not found") > -1)
                {
                    return string.Format(Translate.Message("L467"), userName, orgContactEmail);
                }
                else
                {
                    return forgotResponse.Error;
                }
            }
        }

        public static async Task<string> ResendConfirmEmail(UserManager<ApplicationUser> userManager, string rootPath, string userName, string supportEmail, string InfoEmail, string SecureEmail, string SMPTAddress, string PortNumber, string SecureEmailPassword, string MailAttachmentPath)
        {
            AccountReader reader = new AccountReader(userManager);
            GetUserRequest request = new GetUserRequest();
            request.userName = userName;
            var response = reader.ReadUser(request);
            if (response.User != null && !response.User.EmailConfirmed)
            {
                var token = await reader.GenerateToken(response.User.Id);
                ConfirmationEmail(rootPath, response.User.OrganizationId, response.User.Email, response.User.LanguagePreference, token, InfoEmail, SecureEmail, SMPTAddress, PortNumber, SecureEmailPassword, MailAttachmentPath);
                return "success";
            }
            else
                return string.Format(Translate.Message("L467"), userName, supportEmail);
        }

        public static async Task<ResetPasswordResponse> ResetPassword(UserManager<ApplicationUser> userManager, string email, string token, string password, DateTime? DOB)
        {
            AccountReader reader = new AccountReader(userManager);
            ResetPasswordRequest request = new ResetPasswordRequest();
            request.Email = email;
            request.token = token;
            request.Password = password;
            request.DOB = DOB;
            var response = await reader.ResetPassword(request);
            if (response != null && response.success && !request.Email.Contains("noemail.myintervent.com") && !request.Email.Contains("samlnoemail.com"))
                NotificationUtility.CreateChangePasswordNotificationEvent(NotificationEventTypeDto.ChangePassword, 0, request.Email);
            return response;
        }

        public static async Task<ConfirmEmailResponse> ConfirmEmailAsync(UserManager<ApplicationUser> userManager, string email, string token)
        {
            AccountReader reader = new AccountReader(userManager);

            ConfirmEmailRequest request = new ConfirmEmailRequest();
            request.Email = email;
            request.token = token;
            return await reader.ConfirmEmailAsync(request);
        }

        public static async Task<GetUsersByRoleResponse> GetUsersByRole(UserManager<ApplicationUser> userManager, bool allUserswithRole, int? organization, string firstName, string lastName, int? page, int? pageSize, int? totalRecords, int userId)
        {
            AccountReader reader = new AccountReader(userManager);

            GetUsersByRoleRequest request = new GetUsersByRoleRequest();
            request.userId = userId;
            request.allUserswithRole = allUserswithRole;
            request.organization = organization;
            request.firstName = firstName;
            request.LastName = lastName;
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return await reader.GetUsersByRole(request);
        }

        public static void DeletePicture(UserManager<ApplicationUser> userManager, int userId)
        {
            AccountReader reader = new AccountReader(userManager);
            reader.DeletePicture(userId);
        }

        public static CheckifRegisteredResponse CheckifRegistered(string uniqueId, int orgId)
        {
            CheckifRegisteredRequest request = new CheckifRegisteredRequest();
            request.UniqueId = uniqueId;
            request.OrganizationId = orgId;
            AccountReader reader = new AccountReader();
            return reader.CheckifRegistered(request);
        }

        public static void LogoutUser(int userId, string deviceId)
        {
            AccountReader reader = new AccountReader();
            reader.LogoutUser(userId, deviceId);
        }

        public static async Task<UpdateUserResponse> UpdateUserSubscription(UserManager<ApplicationUser> userManager, int userId, string unsubscribedEmail)
        {
            AccountReader reader = new AccountReader(userManager);
            var getUserResponse = Task.Run(() => reader.GetUser(new GetUserRequest { id = userId }));
            UpdateUserRequest request = new UpdateUserRequest();
            request.UpdatedByUserId = userId;
            request.user = getUserResponse.Result.User;
            request.user.UnsubscribedEmail = unsubscribedEmail;
            return await reader.UpdateUser(request);
        }

        public static UserEligibleToSendCodeResponse IsUserEligibleToSendCode(int userId, int loggedinDeviceId)
        {
            AccountReader reader = new AccountReader();
            UserEligibleToSendCodeRequest request = new UserEligibleToSendCodeRequest();
            request.userId = userId;
            request.loggedinDeviceId = loggedinDeviceId;
            request.active = (byte)SecurityCodeValidityType.Active;
            request.expired = (byte)SecurityCodeValidityType.Expired;
            return reader.IsUserEligibleToSendCode(request);
        }

        public static void AddSecurityCode(int userId, string code, int deviceId)
        {
            AccountReader reader = new AccountReader();
            AddSecurityCodeRequest request = new AddSecurityCodeRequest();
            request.userId = userId;
            request.code = code;
            request.deviceId = deviceId;
            request.status = (byte)SecurityCodeValidityType.Active;
            reader.AddSecurityCode(request);
        }

        public static int AddOrEditUserLoggedInDevice(int userId, string deviceId)
        {
            AccountReader reader = new AccountReader();
            return reader.AddOrEditUserLoggedInDevice(userId, deviceId);
        }

        public static bool SendSecurityCodeToEmail(string rootPath, string userName, string sendTo, string code, string InfoEmail, string SecureEmail, string SMPTAddress, string PortNumber, string SecureEmailPassword, string MailAttachmentPath)
        {
            string path = Path.Combine(rootPath, "Content/email/device-verification-code.html");
            string subject = "Device Verification Code";
            string emailTemplate = File.ReadAllText(path);
            var firstName = userName.Substring(0, 1).ToUpper() + userName.Remove(0, 1);
            emailTemplate = emailTemplate.Replace("{{USER_NMAE}}", firstName);
            emailTemplate = emailTemplate.Replace("{{SECURITY_CODE}}", code);
            CommonUtility.SendEmail(sendTo, emailTemplate, subject, InfoEmail, SecureEmail, SMPTAddress, PortNumber, SecureEmailPassword, MailAttachmentPath);
            NotificationUtility.CreateDirectMailEvent(NotificationEventTypeDto.SecurityCode, sendTo, emailTemplate);
            return true;
        }

        public static bool SendSecurityCodeToPhone(int userId, string sendTo, string code)
        {
            return new TwilioManager().SendDeviceVerificationCode(userId, sendTo, code);
        }

        public static bool VerifySecurityCode(int userId, string code, int deviceId)
        {
            AccountReader reader = new AccountReader();
            return reader.VerifySecurityCode(userId, code, deviceId, (byte)SecurityCodeValidityType.Active, (byte)SecurityCodeValidityType.Validated);
        }

        public static UserLoggedInDevicesDto GetUserLoggedInDevice(int userId, string deviceId)
        {
            AccountReader reader = new AccountReader();
            return reader.GetUserLoggedInDevice(userId, deviceId);
        }
    }
}