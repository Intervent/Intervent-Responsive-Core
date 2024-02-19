using Intervent.DAL;
using Intervent.Web.DTO;
using Intervent.Web.DTO.Diff;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Security.Claims;

namespace Intervent.Web.DataLayer
{
	public class AccountReader
    {
        private UserManager<ApplicationUser> userManager;
        private readonly InterventDatabase dbcontext;

        public AccountReader(UserManager<ApplicationUser> _userManager)
        {
            userManager = _userManager;
            dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        }

        public AccountReader()
        {
            dbcontext = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        }

        public async Task<RegisterUserResponse> CreateUserFromEligibility(EligibilityDto eligibility, int orgId, bool fromFile, string passwordHash = null)
        {
            RegisterUserResponse response = new RegisterUserResponse();
            CommonReader commonReader = new CommonReader();
            ApplicationUser user = new ApplicationUser();

            if (eligibility.Gender != null)
            {
                user.Gender = eligibility.Gender.Key == GenderDto.Male.Key ? (byte)1 : (byte)2;
            }
            if (!string.IsNullOrEmpty(eligibility.State))
            {
                var stateResponse = commonReader.GetState(new GetStateRequest { stateCode = eligibility.State });
                if (stateResponse != null && stateResponse.state != null)
                    user.State = stateResponse.state.Id;
            }
            if (!string.IsNullOrEmpty(eligibility.Country))
            {
                var countryResponse = commonReader.GetCountry(new GetCountryRequest { code = eligibility.Country });
                if (countryResponse != null && countryResponse.country != null)
                    user.Country = countryResponse.country.Id;
            }
            var emailAddress = eligibility.Email;
            if (string.IsNullOrEmpty(emailAddress))
            {
                //retaining email domain so that UI will force the user to enter the email address they try to update the profile
                emailAddress = eligibility.UniqueId + orgId + "@samlnoemail.com";
            }
            user.Email = emailAddress;
            user.OrganizationId = orgId;
            if (fromFile)
                user.PasswordHash = eligibility.FirstName.Substring(0, 1).ToUpper() + (eligibility.FirstName.Length >= 4 ? eligibility.FirstName.Substring(1, 3).ToLower() : eligibility.FirstName.Substring(1, eligibility.FirstName.Length - 1).ToLower()) +
                                        eligibility.LastName.Substring(0, 1).ToUpper() + (eligibility.LastName.Length >= 4 ? eligibility.LastName.Substring(1, 3).ToLower() : eligibility.LastName.Substring(1, eligibility.LastName.Length - 1).ToLower()) + eligibility.UniqueId;
            else
                user.PasswordHash = passwordHash;
            user.FirstName = eligibility.FirstName;
            user.LastName = eligibility.LastName;
            user.MiddleName = eligibility.MiddleName;
            user.DOB = eligibility.DOB;
            user.HomeNumber = eligibility.HomeNumber;
            user.WorkNumber = eligibility.WorkNumber;
            user.CellNumber = eligibility.CellNumber;
            user.Address = eligibility.Address;
            user.Address2 = eligibility.Address2;
            user.City = eligibility.City;
            user.Zip = eligibility.Zip;
            user.UserName = emailAddress;
            user.NormalizedUserName = emailAddress;
            user.NormalizedEmail = emailAddress;
            user.UniqueId = eligibility.UniqueId;
            user.EmailConfirmed = true;
            //user.CreatedBy = 
            user.TermsAccepted = false;

            user.CreatedOn = DateTime.UtcNow;
            user.IsActive = true;
            IdentityResult result;

            result = await userManager.CreateAsync(user, user.PasswordHash);

            response.Succeeded = result.Succeeded;
            if (!result.Succeeded)
                response.error = result.Errors;
            else
            {
                var User = userManager.FindByNameAsync(user.UserName).Result;
                response.userId = User.Id;
                response.EmailId = User.Email;
            }
            return response;
        }

        public void AddEditSpecializations(AddCoachSpecializationRequest request)
        {
            var user = dbcontext.Users.Include("Specializations").Where(x => x.Id == request.userId).FirstOrDefault();
            user.Specializations.Clear();
            if (!string.IsNullOrEmpty(request.specializations))
            {
                var specialities = request.specializations.Split('-');
                foreach (var specId in specialities)
                {
                    if (!string.IsNullOrEmpty(specId))
                    {
                        var id = Convert.ToInt16(specId);
                        var speciality = dbcontext.Specializations.Where(x => x.Id == id).FirstOrDefault();
                        user.Specializations.Add(speciality);
                    }
                }
            }
            dbcontext.SaveChanges();
        }

        public void AddEditUserLanguages(AddCoachLanguageRequest request)
        {
            var user = dbcontext.Users.Include("Languages").Where(x => x.Id == request.userId).FirstOrDefault();
            user.Languages.Clear();
            if (!string.IsNullOrEmpty(request.languages))
            {
                var languages = request.languages.Split('-');
                foreach (var langId in languages)
                {
                    if (!string.IsNullOrEmpty(langId))
                    {
                        var id = Convert.ToInt16(langId);
                        var language = dbcontext.Languages.Where(x => x.Id == id).FirstOrDefault();
                        user.Languages.Add(language);
                    }
                }
            }
            dbcontext.SaveChanges();
        }

        public void AddEditCoachStates(AddCoachStateRequest request)
        {
            var user = dbcontext.Users.Include("CoachStates").Where(x => x.Id == request.userId).FirstOrDefault();
            user.CoachStates.Clear();
            if (!string.IsNullOrEmpty(request.states))
            {
                var states = request.states.Split('-');
                foreach (var state in states)
                {
                    if (!string.IsNullOrEmpty(state))
                    {
                        var id = Convert.ToInt16(state);
                        var stateDAL = dbcontext.States.Where(x => x.Id == id).FirstOrDefault();
                        user.CoachStates.Add(stateDAL);
                    }
                }
            }
            dbcontext.SaveChanges();
        }

        //dataprotectionprovider can be null
        public async Task<RegisterUserResponse> CreateUser(RegisterUserRequest request)
        {
            RegisterUserResponse response = new RegisterUserResponse();

            ApplicationUser daluser = new ApplicationUser()
            {
                UserName = request.User.UserName,
                Email = request.User.Email,
                NormalizedUserName = request.User.UserName,
                NormalizedEmail = request.User.Email,
                EmailConfirmed = request.User.EmailConfirmed,
                NamePrefix = request.User.NamePrefix,
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                MiddleName = request.User.MiddleName,
                Suffix = request.User.Suffix,
                DOB = request.User.DOB,
                Gender = request.User.Gender,
                Race = request.User.Race,
                RaceOther = request.User.RaceOther,
                HomeNumber = request.User.HomeNumber,
                WorkNumber = request.User.WorkNumber,
                CellNumber = request.User.CellNumber,
                TimeZoneId = request.User.TimeZoneId,
                UniqueId = request.User.UniqueId,
                ContactMode = request.User.ContactMode,
                Occupation = request.User.Occupation,
                Source = request.User.Source,
                SourceOther = request.User.SourceOther,
                ReferralDetails = request.User.ReferralDetails,
                LanguagePreference = request.User.LanguagePreference,
                Text = request.User.Text,
                PreferredContactTimeId = request.User.PreferredContactTimeId,
                Address = request.User.Address,
                Address2 = request.User.Address2,
                City = request.User.City,
                State = request.User.State,
                Zip = request.User.Zip,
                Country = request.User.Country,
                ProfessionId = request.User.ProfessionId,
                OrganizationId = request.User.OrganizationId,
                PrimaryCarePhysician = request.User.PrimaryCarePhysician,
                Unit = request.User.Unit,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.User.CreatedBy,
                Complete = request.User.Complete,
                IsActive = request.User.IsActive,
                CouponCode = request.User.CouponCode,
                TermsAccepted = request.User.TermsAccepted,
                EmployeeId = request.User.EmployeeId
            };

            IdentityResult result;
            if (!string.IsNullOrWhiteSpace(request.User.PasswordHash))
                result = await userManager.CreateAsync(daluser, request.User.PasswordHash);
            else
                result = await userManager.CreateAsync(daluser);
            response.Succeeded = result.Succeeded;
            if (!result.Succeeded)
                response.error = result.Errors;
            else
            {
                var User = userManager.FindByNameAsync(request.User.UserName).Result;
                response.userId = User.Id;
                if (request.User.PrimaryCarePhysician == 1 && request.User.UserDoctorInfoes != null)
                {
                    IncentiveReader reader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.PCP_incentive;
                    incentivesRequest.userId = User.Id;
                    incentivesRequest.portalId = User.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                    reader.AwardIncentives(incentivesRequest);
                    var doctorInfo = request.User.UserDoctorInfoes.FirstOrDefault();
                    var userDoctorInfo = new UserDoctorInfo();
                    userDoctorInfo.UserId = User.Id;
                    userDoctorInfo.ContactPermission = doctorInfo.ContactPermission;
                    userDoctorInfo.Name = doctorInfo.Name;
                    userDoctorInfo.Address = doctorInfo.Address;
                    userDoctorInfo.City = doctorInfo.City;
                    userDoctorInfo.State = doctorInfo.State;
                    userDoctorInfo.Country = doctorInfo.Country;
                    userDoctorInfo.Zip = doctorInfo.Zip;
                    userDoctorInfo.FaxNumber = doctorInfo.FaxNumber;
                    userDoctorInfo.PhoneNumber = doctorInfo.PhoneNumber;
                    userDoctorInfo.ProviderId = doctorInfo.ProviderId;
                    userDoctorInfo.Active = true;
                    dbcontext.UserDoctorInfoes.Add(userDoctorInfo);
                    dbcontext.SaveChanges();
                }
                if (!request.User.EmailConfirmed)
                {
                    //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int, logger>(dataProtectionProvider.Create("ConfirmEmail"));
                    response.Token = await userManager.GenerateEmailConfirmationTokenAsync(daluser);
                }
                if (!request.User.Complete.HasValue || !request.User.Complete.Value)
                {
                    CommonReader commonReader = new CommonReader();
                    commonReader.AddDashboardMessage(User.Id, IncentiveMessageTypes.Profile_Complete, null, null);
                }
                await userManager.AddPasswordAsync(daluser, request.User.PasswordHash);
            }
            return response;
        }

        public async Task<RegisterUserResponse> ApiCreateUser(RegisterUserRequest request)
        {
            RegisterUserResponse response = new RegisterUserResponse();
            request.User.PasswordHash = request.User.FirstName.Substring(0, 1).ToUpper() + (request.User.FirstName.Length >= 4 ? request.User.FirstName.Substring(1, 3).ToLower() : request.User.FirstName.Substring(1, request.User.FirstName.Length - 1).ToLower()) +
                                        request.User.LastName.Substring(0, 1).ToUpper() + (request.User.LastName.Length >= 4 ? request.User.LastName.Substring(1, 3).ToLower() : request.User.LastName.Substring(1, request.User.LastName.Length - 1).ToLower()) + request.User.DOB.Value.Year;
            request.User.UserName = request.User.Email;
            request.User.EmailConfirmed = true;
            request.User.Complete = true;
            request.User.IsActive = true;
            ApplicationUser daluser = new ApplicationUser()
            {
                UserName = request.User.UserName,
                Email = request.User.Email,
                EmailConfirmed = request.User.EmailConfirmed,
                NamePrefix = request.User.NamePrefix,
                FirstName = request.User.FirstName,
                LastName = request.User.LastName,
                MiddleName = request.User.MiddleName,
                Suffix = request.User.Suffix,
                DOB = request.User.DOB,
                Gender = request.User.Gender,
                Race = request.User.Race,
                RaceOther = request.User.RaceOther,
                HomeNumber = request.User.HomeNumber,
                WorkNumber = request.User.WorkNumber,
                CellNumber = request.User.CellNumber,
                TimeZoneId = request.User.TimeZoneId,
                UniqueId = request.User.UniqueId,
                ContactMode = request.User.ContactMode,
                Occupation = request.User.Occupation,
                Source = request.User.Source,
                SourceOther = request.User.SourceOther,
                LanguagePreference = request.User.LanguagePreference,
                Text = request.User.Text,
                PreferredContactTimeId = request.User.PreferredContactTimeId,
                Address = request.User.Address,
                Address2 = request.User.Address2,
                City = request.User.City,
                State = request.User.State,
                Zip = request.User.Zip,
                Country = request.User.Country,
                ProfessionId = request.User.ProfessionId,
                OrganizationId = request.User.OrganizationId,
                PrimaryCarePhysician = request.User.PrimaryCarePhysician,
                Unit = request.User.Unit,
                CreatedOn = DateTime.UtcNow,
                CreatedBy = request.User.CreatedBy,
                Complete = request.User.Complete,
                IsActive = request.User.IsActive
            };

            IdentityResult result;
            if (!string.IsNullOrWhiteSpace(request.User.PasswordHash))
                result = await userManager.CreateAsync(daluser, request.User.PasswordHash);
            else
                result = await userManager.CreateAsync(daluser);
            response.Succeeded = result.Succeeded;
            if (!result.Succeeded)
                response.error = result.Errors;
            else
            {
                var User = userManager.FindByNameAsync(request.User.UserName).Result;
                response.userId = User.Id;
            }
            return response;
        }

        public async Task<bool> DeleteUserAsync(int userId)
        {

            var user = await userManager.FindByIdAsync(userId.ToString());
            if (user != null)
            {
                var result = await userManager.DeleteAsync(user);
                return result.Succeeded;
            }
            return true;
        }

        public async Task<int?> GetUserIdByNameAsync(string name)
        {
            try
            {
                var user = await userManager.FindByNameAsync(name);
                if (user != null)
                    return user.Id;
            }
            catch { }
            return null;
        }

        public UserDto GetUserDetails(int id)
        {
            var user = GetUserById(id);
            if (user != null)
            {
                UserDto userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.PhoneNumber = user.PhoneNumber;
                userDto.NamePrefix = user.NamePrefix;
                userDto.FirstName = user.FirstName;
                userDto.MiddleName = user.MiddleName;
                userDto.Suffix = user.Suffix;
                userDto.LastName = user.LastName;
                userDto.PasswordHash = user.PasswordHash;
                userDto.DOB = user.DOB;
                userDto.Gender = user.Gender;
                userDto.Race = user.Race;
                userDto.RaceOther = user.RaceOther;
                userDto.Address = user.Address;
                userDto.Address2 = user.Address2;
                userDto.City = user.City;
                userDto.State = user.State;
                userDto.Country = user.Country;
                userDto.Zip = user.Zip;
                userDto.HomeNumber = user.HomeNumber;
                userDto.WorkNumber = user.WorkNumber;
                userDto.CellNumber = user.CellNumber;
                userDto.LanguagePreference = user.LanguagePreference;
                userDto.PrimaryCarePhysician = user.PrimaryCarePhysician;
                userDto.Unit = user.Unit;
                userDto.IsActive = user.IsActive;
                userDto.InactiveReason = user.InactiveReason;
                userDto.UniqueId = user.UniqueId;
                userDto.PreferredContactTimeId = user.PreferredContactTimeId;
                userDto.ProfessionId = user.ProfessionId;
                userDto.OrganizationId = user.OrganizationId;
                userDto.Occupation = user.Occupation;
                userDto.Source = user.Source;
                userDto.SourceOther = user.SourceOther;
                userDto.Text = user.Text;
                userDto.Picture = user.Picture;
                userDto.Complete = user.Complete;
                userDto.ContactMode = user.ContactMode;
                return userDto;
            }
            return null;
        }

        public User GetUserById(int id)
        {
            return dbcontext.Users.Include("Organization").Include("Organization.Portals").Include("UserTrackingStatuses").Include("UserDoctorInfoes").Include("UserDoctorInfoes.Provider").Where(x => x.Id == id).FirstOrDefault();
        }

        public UserDto GetBasicUserInfo(int id, bool dashboard = false, bool userInProgram = false)
        {
            if (userInProgram)
            {
                var user = dbcontext.Users.Include("UsersinPrograms").Include("UsersinPrograms.User1.AdminProperty").Include("UsersinPrograms.User1.UserRoles").Include("Country1").Where(x => x.Id == id).FirstOrDefault();
                return Utility.mapper.Map<User, UserDto>(user);
            }
            else if (dashboard)
            {
                var user = dbcontext.Users.Include("Appointments").Include("HRAs").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Include("Organization").Include("Organization.Portals").Where(x => x.Id == id).FirstOrDefault();
                return Utility.mapper.Map<User, UserDto>(user);
            }
            else
            {
                var user = dbcontext.Users.Include("Organization").Include("Organization.Portals").Include("Country1").Where(x => x.Id == id).FirstOrDefault();
                return Utility.mapper.Map<User, UserDto>(user);
            }
        }

        public User GetUserByUserName(string email)
        {
            return dbcontext.Users.Include("Organization").Where(x => x.UserName == email).FirstOrDefault();
        }

        public UserDto GetUserHRAByUniqueId(GetUserRequestByUniqueId request)
        {
            var user = dbcontext.Users.Include("HRAs").Include("HRAs.HRA_MedicalConditions").Include("HRAs.HRA_HealthNumbers").Where(x => x.OrganizationId == request.OrganizationId && x.UniqueId == request.UniqueId).FirstOrDefault();

            return Utility.mapper.Map<User, UserDto>(user);
        }

        public GetUserResponse GetUserByUniqueId(GetUserRequestByUniqueId request)
        {
            var user = dbcontext.Users.Include("HRAs").Include("UsersinPrograms").Include("UsersinPrograms.ProgramsinPortal").Include("UsersinPrograms.ProgramsinPortal.Program").Where(x => (!request.OrganizationId.HasValue || x.OrganizationId == request.OrganizationId) && x.UniqueId == request.UniqueId).FirstOrDefault();
            GetUserResponse response = new GetUserResponse();
            response.User = Utility.mapper.Map<User, UserDto>(user);
            return response;
        }

        public async Task<GetUserResponse> GetUser(GetUserRequest request)
        {
            GetUserResponse response = new GetUserResponse();

            User user = null;
            if (request.id.HasValue)
            {
                response.appUser = await userManager.FindByIdAsync(request.id.Value.ToString());
                if (response.appUser != null)
                    user = dbcontext.Users.Include("Organization").Include("Organization.Portals").Include("UserDoctorInfoes").Include("CoachStates").Include("Languages").Include("Specializations").Include("AdminProperty").Include("UserRoles").Include("UserRoles.AdminModules").Include("TimeZone").Where(x => x.Id == response.appUser.Id).FirstOrDefault();
            }
            else
            {
                response.appUser = await userManager.FindByNameAsync(request.userName);
                if (response.appUser != null)
                    user = dbcontext.Users.Include("Organization").Include("Organization.Portals").Include("UserDoctorInfoes").Include("CoachStates").Include("Languages").Include("Specializations").Include("AdminProperty").Include("UserRoles").Include("UserRoles.AdminModules").Include("TimeZone").Where(x => x.Id == response.appUser.Id).FirstOrDefault();

                if (user != null && !ValidateUserEligibility(user))
                {
                    response.error = "Your account is inactive. For further assistance please email us at " + user.Organization.ContactEmail + ".";
                    return response;
                }
                if (user != null && (request.validateUser.HasValue && request.validateUser.Value))
                {
                    if (user.IsActive == false)
                    {
                        response.error = "Your account has been deactivated. For further assistance please email us at " + user.Organization.ContactEmail + ".";
                        return response;
                    }
                    var locked = await userManager.IsLockedOutAsync(response.appUser);
                    if (locked)
                    {
                        response.error = "We temporarily locked your account after failed login attempts. Please wait till " + TimeZoneInfo.ConvertTimeFromUtc(user.LockoutEndDateUtc.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) + " Eastern Time before trying to log in again";
                        return response;
                    }
                    else
                    {
                        if (!user.EmailConfirmed)
                        {
                            response.error = "Please check your email to activate your account.";
                            response.EmailConfirmed = false;
                            return response;
                        }

                        if (request.verifyDeviceLogin && !string.IsNullOrEmpty(request.deviceID))
                            response.IsVerifiedDevice = IsVerifiedUserDevice(user.Id, request.deviceID);

                        if (request.appLogin)
                            response.IsVerifiedDevice = true;

                        var result = userManager.PasswordHasher.VerifyHashedPassword(response.appUser, user.PasswordHash, request.password);
                        if (result == PasswordVerificationResult.Failed)
                        {
                            var lastFailedDate = GetLastFailedAccessDate(user.Id);
                            if (lastFailedDate != DateTime.MinValue && lastFailedDate.AddDays(1) < DateTime.UtcNow)
                            {
                                var resetCount = await userManager.GetAccessFailedCountAsync(response.appUser);
                                if (resetCount > 0)
                                    await userManager.ResetAccessFailedCountAsync(response.appUser);
                            }
                            var failedCount = await userManager.AccessFailedAsync(response.appUser);
                            var failcount = await userManager.GetAccessFailedCountAsync(response.appUser);
                            LogUserLastAccess(user.Id, false, response.IsVerifiedDevice ? request.deviceID : null);
                            if (failcount == 0)
                                response.error = "We temporarily locked your account after failed login attempts. Please wait till " + TimeZoneInfo.ConvertTimeFromUtc(user.LockoutEndDateUtc.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) + " Eastern Time before trying to log in again";
                            else if (failcount == 1)
                                response.error = "The email/password you entered is incorrect. Please try again (make sure your caps lock is off)";
                            else if (failcount == 2)
                                response.error = "There have been two failed login attempts. Your account will be locked for 30 minutes after another failed attempt. Please click Forgot Password link to reset your password";
                            return response;
                        }
                        var failCount = await userManager.GetAccessFailedCountAsync(response.appUser);
                        if (failCount > 0)
                        {
                            await userManager.ResetAccessFailedCountAsync(response.appUser);
                        }

                        if (request.verifyDeviceLogin && !response.IsVerifiedDevice && !string.IsNullOrEmpty(request.deviceID))
                        {
                            response.UserId = user.Id;
                            string emailId = user.Email;
                            int index = emailId.IndexOf('@');
                            string encodeEmailId = emailId;
                            if (index > 3)
                                encodeEmailId = emailId.Substring(0, 2) + new string('*', index - 2) + emailId.Substring(index);

                            response.error = "verify device";
                            response.EmailId = encodeEmailId;

                            // Send verification code to phone no
                            /*if (!string.IsNullOrEmpty(user.CellNumber))
                                response.PhoneNo = Regex.Replace(user.CellNumber, @"(?<=\d{2})\d(?=\d{2})", "x");*/

                            AddOrEditUserLoggedInDevice(user.Id, request.deviceID);
                            return response;
                        }
                        if (!string.IsNullOrEmpty(request.userName))
                        {
                            LogUserLastAccess(user.Id, true, response.IsVerifiedDevice ? request.deviceID : null);
                            await userManager.UpdateAsync(response.appUser);
                        }
                    }
                }
            }
            if (user != null)
            {
                UserDto userDto = new UserDto();
                userDto.Id = user.Id;
                userDto.UserName = user.UserName;
                userDto.Email = user.Email;
                userDto.EmailConfirmed = user.EmailConfirmed;
                userDto.PhoneNumber = user.PhoneNumber;
                userDto.NamePrefix = user.NamePrefix;
                userDto.FirstName = user.FirstName;
                userDto.MiddleName = user.MiddleName;
                userDto.Suffix = user.Suffix;
                userDto.LastName = user.LastName;
                userDto.PasswordHash = user.PasswordHash;
                userDto.DOB = user.DOB;
                userDto.Gender = user.Gender;
                userDto.Race = user.Race;
                userDto.RaceOther = user.RaceOther;
                userDto.Address = user.Address;
                userDto.Address2 = user.Address2;
                userDto.City = user.City;
                userDto.State = user.State;
                userDto.Country = user.Country;
                userDto.Zip = user.Zip;
                userDto.HomeNumber = user.HomeNumber;
                userDto.WorkNumber = user.WorkNumber;
                userDto.CellNumber = user.CellNumber;
                userDto.LanguagePreference = user.LanguagePreference;
                userDto.PrimaryCarePhysician = user.PrimaryCarePhysician;
                userDto.Unit = user.Unit;
                userDto.IsActive = user.IsActive;
                userDto.InactiveReason = user.InactiveReason;
                userDto.LockoutEndDateUtc = user.LockoutEndDateUtc;
                userDto.UniqueId = user.UniqueId;
                userDto.UnsubscribedEmail = user.UnsubscribedEmail;
                userDto.UserDoctorInfoes = Utility.mapper.Map<List<UserDoctorInfo>, List<UserDoctorInfoDto>>(user.UserDoctorInfoes.Where(x => x.Active == true).ToList());
                if (user.Organization != null)
                {
                    userDto.Organization = new OrganizationDto();
                    userDto.Organization.TermsForSSO = user.Organization.TermsForSSO;
                    userDto.Organization.Url = user.Organization.Url;
                    userDto.Organization.IntegrationWith = user.Organization.IntegrationWith;
                    userDto.Organization.Name = user.Organization.Name;
                    userDto.Organization.Code = user.Organization.Code;
                }
                if (user.TimeZone != null)
                {
                    userDto.TimeZoneId = user.TimeZone.Id;
                    userDto.TimeZone = user.TimeZone.TimeZoneId;
                    userDto.TimeZoneName = user.TimeZone.TimeZone1;
                    await userManager.AddClaimAsync(response.appUser, new Claim("TimeZone", userDto.TimeZone));
                }
                userDto.PreferredContactTimeId = user.PreferredContactTimeId;
                userDto.ProfessionId = user.ProfessionId;
                userDto.OrganizationId = user.OrganizationId;
                userDto.Occupation = user.Occupation;
                userDto.Source = user.Source;
                userDto.SourceOther = user.SourceOther;
                userDto.ReferralDetails = user.ReferralDetails;
                userDto.Text = user.Text;
                userDto.Picture = user.Picture;
                userDto.Complete = user.Complete;
                userDto.ContactMode = user.ContactMode;
                userDto.TermsAccepted = user.TermsAccepted;
                userDto.DeptId = user.DeptId;
                if (user.UserRoles != null && user.UserRoles.Count > 0)
                {
                    List<RoleDto> roles = new List<RoleDto>();
                    foreach (var userRole in user.UserRoles.OrderByDescending(x => x.Id))
                    {
                        RoleDto role = new RoleDto();
                        role.Id = userRole.Id;
                        role.Name = userRole.Name;
                        role.Code = userRole.Code;
                        roles.Add(role);
                        if (userRole.AdminModules != null && userRole.AdminModules.Count > 0)
                        {
                            var AdminModuleList = userRole.AdminModules.ToList();
                            List<AdminModuleDto> adminModules = new List<AdminModuleDto>();
                            for (int i = 0; i < AdminModuleList.Count; i++)
                            {
                                AdminModuleDto adminModule = new AdminModuleDto();
                                adminModule.Id = AdminModuleList[i].Id;
                                adminModule.Name = AdminModuleList[i].Name;
                                adminModules.Add(adminModule);
                            }
                            role.AdminModules = adminModules;
                        }
                    }
                    userDto.Roles = roles;
                }
                if (user.AdminProperty != null)
                {
                    AdminPropertyDto adminProperty = new AdminPropertyDto();
                    adminProperty.Profile = user.AdminProperty.Profile;
                    adminProperty.ProfileLanguageItem = user.AdminProperty.ProfileLanguageItem;
                    adminProperty.Video = user.AdminProperty.Video;
                    adminProperty.AllowAppt = user.AdminProperty.AllowAppt;
                    adminProperty.MeetingId = !string.IsNullOrEmpty(user.AdminProperty.MeetingId) ? user.AdminProperty.MeetingId : "";
                    userDto.AdminProperty = adminProperty;
                }
                if (user.Specializations != null)
                {
                    List<SpecializationDto> specialities = new List<SpecializationDto>();
                    foreach (var speciality in user.Specializations)
                    {
                        SpecializationDto specDto = new SpecializationDto();
                        specDto.Id = speciality.Id;
                        specDto.Name = speciality.Name;
                        specialities.Add(specDto);
                    }
                    userDto.Specializations = specialities;
                }
                if (user.Languages != null)
                {
                    List<LanguagesDto> languages = new List<LanguagesDto>();
                    foreach (var language in user.Languages)
                    {
                        LanguagesDto langDto = new LanguagesDto();
                        langDto.Id = language.Id;
                        langDto.Language = language.Language;
                        languages.Add(langDto);
                    }
                    userDto.Languages = languages;
                }
                if (user.CoachStates != null)
                {
                    List<StateDto> states = new List<StateDto>();
                    foreach (var state in user.CoachStates)
                    {
                        StateDto stateDto = new StateDto();
                        stateDto.Id = state.Id;
                        stateDto.Name = state.Name;
                        stateDto.Code = state.Code;
                        stateDto.CountryId = state.CountryId ?? 0;
                        states.Add(stateDto);
                    }
                    userDto.CoachStates = states;
                }
                response.User = userDto;
            }
            return response;
        }

        public async Task<VerifyUserResponse> VerifyUser(VerifyUserRequest request)
        {
            VerifyUserResponse response = new VerifyUserResponse();

            ApplicationUser appUser = await userManager.FindByNameAsync(request.userName);
            User user = Utility.mapper.Map<ApplicationUser, User>(appUser);
            if (user != null)
            {
                var result = userManager.PasswordHasher.VerifyHashedPassword(appUser, user.PasswordHash, request.password);
                if (result == PasswordVerificationResult.Failed)
                {
                    var lastFailedDate = GetLastFailedAccessDate(user.Id);
                    if (lastFailedDate != DateTime.MinValue && lastFailedDate.AddDays(1) < DateTime.UtcNow)
                    {
                        var resetCount = await userManager.GetAccessFailedCountAsync(appUser);
                        if (resetCount > 0)
                            await userManager.ResetAccessFailedCountAsync(appUser);
                    }
                    var failedCount = await userManager.AccessFailedAsync(appUser);
                    var failcount = await userManager.GetAccessFailedCountAsync(appUser);
                    LogUserLastAccess(user.Id, false, request.deviceID);
                    if (failcount == 0)
                        response.error = "We temporarily locked your account after failed login attempts. Please wait till " + TimeZoneInfo.ConvertTimeFromUtc(user.LockoutEndDateUtc.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) + " Eastern Time before trying to log in again";
                    else if (failcount == 1)
                        response.error = "The email/password you entered is incorrect. Please try again (make sure your caps lock is off)";
                    else if (failcount == 2)
                        response.error = "There have been two failed login attempts. Your account will be locked for 30 minutes after another failed attempt. Please click Forgot Password link to reset your password";
                    response.status = false;
                    return response;
                }
                var failCount = await userManager.GetAccessFailedCountAsync(appUser);
                if (failCount > 0)
                {
                    await userManager.ResetAccessFailedCountAsync(appUser);
                }
                if (!string.IsNullOrEmpty(request.userName))
                {
                    LogUserLastAccess(user.Id, true, request.deviceID);
                    await userManager.UpdateAsync(appUser);
                }
                response.status = true;
                return response;
            }
            else
            {
                response.error = "User not found.";
                response.status = false;
                return response;
            }
        }

        public bool ValidateUserEligibility(User user)
        {
            if (user.Organization.Portals.Where(x => x.Active).FirstOrDefault() != null)
            {
                if (user.Organization.Portals.Where(x => x.Active).FirstOrDefault().EligibilityImportLoadFlag ?? default(bool))
                {
                    var portalId = user.Organization.Portals.Where(x => x.Active).OrderByDescending(x => x.Id).FirstOrDefault().Id;
                    var eligibility = dbcontext.Eligibilities.Where(x => (x.UniqueId == user.UniqueId && x.PortalId == portalId)).FirstOrDefault();
                    if (eligibility == null)
                        return false;
                }
            }
            else
                return false;
            return true;
        }

        public DateTime GetLastFailedAccessDate(int userId)
        {
            GetUserResponse response = new GetUserResponse();
            var userLog = dbcontext.UserLogs.Where(x => x.UserId == userId && x.IsSuccess.HasValue && !x.IsSuccess.Value).OrderByDescending(x => x.Id).FirstOrDefault();
            return userLog != null ? userLog.LastAccessedOn : DateTime.MinValue;
        }

        public void LogUserLastAccess(int userId, bool status, string deviceId)
        {
            UserLog userLog = new UserLog();
            userLog.UserId = userId;
            userLog.LastAccessedOn = DateTime.UtcNow;
            userLog.IsSuccess = status;
            if (!string.IsNullOrEmpty(deviceId))
                userLog.LoggedInDeviceId = AddOrEditUserLoggedInDevice(userId, deviceId);
            dbcontext.UserLogs.Add(userLog);
            dbcontext.SaveChanges();
        }

        public GetUserResponse ReadUser(GetUserRequest request)
        {
            GetUserResponse response = new GetUserResponse();
            var user = dbcontext.Users.Include("UserLogs").Include("UserTimeTracker").Include("Organization").Include("Country1").Where(x => (String.IsNullOrEmpty(request.userName) || x.UserName == request.userName)
                && (!request.id.HasValue || x.Id == request.id)).FirstOrDefault();
            response.User = Utility.mapper.Map<User, UserDto>(user);
            return response;
        }

        public FindUsersResponse FindUsers(FindUsersRequest request)
        {
            FindUsersResponse response = new FindUsersResponse();
            var users = dbcontext.Users.Include("Organization").Include("Organization.Portals").Include("UsersinPrograms").Include("UsersinPrograms").Include("UsersinPrograms.KitsinUserPrograms").Where(x => (request.OrganizationIds.Count() != 0 && request.OrganizationIds.Contains(x.OrganizationId))
                        || (request.UserId != null && x.Id == request.UserId.Value)).ToList();
            response.Users = Utility.mapper.Map<IList<User>, IList<UserDto>>(users);
            return response;
        }

        /*public async Task<UserDto> FindExternalUser(UserLoginInfo login)
        {
            User user = null;//TODO await userManager.FindAsync(login.LoginProvider.);
            return Utility.mapper.Map<User, UserDto>(user);
        }*/

        public async Task<UpdateUserResponse> UpdateUser(UpdateUserRequest request, bool? isComplete = true)
        {
            UpdateUserResponse response = new UpdateUserResponse();

            var existingUser = GetUserById(request.user.Id);
            var user = await userManager.FindByIdAsync(request.user.Id.ToString());

            //check if primary fields changes
            if (user.DOB != request.user.DOB || user.Gender != request.user.Gender || user.Race != request.user.Race)
                response.primaryFieldsChanged = true;
            else
                response.primaryFieldsChanged = false;
            user.NamePrefix = request.user.NamePrefix;
            user.FirstName = request.user.FirstName;
            user.LastName = request.user.LastName;
            user.MiddleName = request.user.MiddleName;
            user.Suffix = request.user.Suffix;
            user.DOB = request.user.DOB;
            user.Gender = request.user.Gender;
            user.Address = request.user.Address;
            user.Address2 = request.user.Address2;
            user.City = request.user.City;
            user.State = request.user.State;
            user.Country = request.user.Country;
            user.Zip = request.user.Zip;
            user.HomeNumber = request.user.HomeNumber;
            user.WorkNumber = request.user.WorkNumber;
            user.CellNumber = request.user.CellNumber;
            user.TimeZoneId = request.user.TimeZoneId;
            user.Unit = request.user.Unit;
            //check if active status changes
            if (user.IsActive != request.user.IsActive)
                response.activeStatusChanged = true;
            user.IsActive = request.user.IsActive;
            user.InactiveReason = request.user.InactiveReason;
            if (request.user.UnsubscribedEmail != null)
                user.UnsubscribedEmail = request.user.UnsubscribedEmail;
            if (!string.IsNullOrEmpty(request.user.UniqueId))
                user.UniqueId = request.user.UniqueId;
            if (request.IsLocked.HasValue && request.IsLocked == false)
            {
                user.AccessFailedCount = 0;
                user.LockoutEndDateUtc = null;
            }
            if (user.Email != request.user.Email)
            {
                user.Email = request.user.Email;
                user.UserName = request.user.Email;
            }
            if (!request.FromAdmin)
            {
                user.Race = request.user.Race;
                user.RaceOther = request.user.RaceOther;
                user.Occupation = request.user.Occupation;
                user.ContactMode = request.user.ContactMode;
                user.PreferredContactTimeId = request.user.PreferredContactTimeId;
                user.Source = request.user.Source;
                user.SourceOther = request.user.SourceOther;
                user.LanguagePreference = request.user.LanguagePreference;
                user.Text = request.user.Text;
                user.DeptId = request.user.DeptId;
                user.ReferralDetails = request.user.ReferralDetails;
                user.PrimaryCarePhysician = request.user.PrimaryCarePhysician;
                if (request.user.PrimaryCarePhysician == 1)
                {
                    IncentiveReader reader = new IncentiveReader();
                    AwardIncentivesRequest incentivesRequest = new AwardIncentivesRequest();
                    incentivesRequest.incentiveType = IncentiveTypes.PCP_incentive;
                    incentivesRequest.userId = user.Id;
                    incentivesRequest.portalId = existingUser.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().Id;
                    incentivesRequest.isEligible = true;
                    incentivesRequest.pointsIncentiveMessage = IncentiveMessageTypes.Incentive;
                    reader.AwardIncentives(incentivesRequest);
                }
            }
            else
            {
                user.OrganizationId = request.user.OrganizationId;
            }
            //set profile to complete
            if ((!user.Complete.HasValue || !user.Complete.Value) && isComplete.HasValue && isComplete.Value)
            {
                var userDashboardMessages = dbcontext.UserDashboardMessages.Where(x => x.Url.StartsWith("../Participant/Stream?openProfile=true") && x.UserId == user.Id && x.Active).FirstOrDefault();
                if (userDashboardMessages != null)
                {
                    userDashboardMessages.Active = false;
                    dbcontext.UserDashboardMessages.Attach(userDashboardMessages);
                    dbcontext.Entry(userDashboardMessages).State = EntityState.Modified;
                    dbcontext.SaveChanges();
                }
            }
            user.Complete = isComplete;
            var result = await userManager.UpdateAsync(user);
            response.Succeeded = result.Succeeded;
            if (!result.Succeeded)
                response.error = result.Errors;

            var updatedUser = Utility.mapper.Map<ApplicationUser, User>(user);
            IEnumerable<PropertyCompare> properties = PropertyCompare.FindDifferences(existingUser, updatedUser, excludeVirtual: true);
            if (properties.Count() > 0)
            {
                AddUserChangeRequest change = new AddUserChangeRequest();
                change.UserChange = new UserHistoryDto();
                change.UserChange.UserId = user.Id;
                change.UserChange.UpdatedBy = request.UpdatedByUserId;

                change.UserChange.UserHistoryCategoryId = UserHistoryCategoryDto.UserProfile.Id;
                change.UserChange.Changes = JsonConvert.SerializeObject(properties);
                new UserHistoryReader().AddUserChanges(change);
            }
            return response;
        }

        public async Task<ChangePasswordResponse> ChangePassword(ChangePasswordRequest request)
        {
            ChangePasswordResponse response = new ChangePasswordResponse();

            var result = await userManager.ChangePasswordAsync(new ApplicationUser { Id = request.UserId }, request.oldPassword, request.newPassword);
            response.success = result.Succeeded;
            if (!response.success)
                response.error = "";// result.Errors.FirstOrDefault();
            return response;
        }

        public string UpadateDoctorInfo(UpdateUserRequest request)
        {
            int UserId = request.user.Id;
            var userDoctorInfo = dbcontext.UserDoctorInfoes.Where(x => x.UserId == UserId).FirstOrDefault();
            if (userDoctorInfo == null && request.user.PrimaryCarePhysician == 1)
            {
                userDoctorInfo = new UserDoctorInfo();
                userDoctorInfo.UserId = request.user.UserDoctorInfoes[0].UserId;
                userDoctorInfo.ContactPermission = request.user.UserDoctorInfoes[0].ContactPermission;
                userDoctorInfo.Name = request.user.UserDoctorInfoes[0].Name;
                userDoctorInfo.Address = request.user.UserDoctorInfoes[0].Address;
                userDoctorInfo.City = request.user.UserDoctorInfoes[0].City;
                userDoctorInfo.State = request.user.UserDoctorInfoes[0].State;
                userDoctorInfo.Country = request.user.UserDoctorInfoes[0].Country;
                userDoctorInfo.Zip = request.user.UserDoctorInfoes[0].Zip;
                userDoctorInfo.FaxNumber = request.user.UserDoctorInfoes[0].FaxNumber;
                userDoctorInfo.PhoneNumber = request.user.UserDoctorInfoes[0].PhoneNumber;
                userDoctorInfo.ProviderId = request.user.UserDoctorInfoes[0].ProviderId;
                userDoctorInfo.Active = true;
                dbcontext.UserDoctorInfoes.Add(userDoctorInfo);
                dbcontext.SaveChanges();
            }
            else if (userDoctorInfo != null)
            {
                if (request.user.PrimaryCarePhysician == 1)
                {
                    userDoctorInfo.ContactPermission = request.user.UserDoctorInfoes[0].ContactPermission;
                    userDoctorInfo.Name = request.user.UserDoctorInfoes[0].Name;
                    userDoctorInfo.Address = request.user.UserDoctorInfoes[0].Address;
                    userDoctorInfo.City = request.user.UserDoctorInfoes[0].City;
                    userDoctorInfo.State = request.user.UserDoctorInfoes[0].State;
                    userDoctorInfo.Country = request.user.UserDoctorInfoes[0].Country;
                    userDoctorInfo.Zip = request.user.UserDoctorInfoes[0].Zip;
                    userDoctorInfo.FaxNumber = request.user.UserDoctorInfoes[0].FaxNumber;
                    userDoctorInfo.PhoneNumber = request.user.UserDoctorInfoes[0].PhoneNumber;
                    userDoctorInfo.ProviderId = request.user.UserDoctorInfoes[0].ProviderId;
                    userDoctorInfo.Active = true;
                }
                else
                    userDoctorInfo.Active = false;
                dbcontext.UserDoctorInfoes.Attach(userDoctorInfo);
                dbcontext.Entry(userDoctorInfo).State = EntityState.Modified;
                dbcontext.SaveChanges();
            }

            var response = "success";
            return response;
        }

        public async Task<ForgotPasswordResponse> ForgotPassword(ForgotPasswordRequest request)
        {
            ForgotPasswordResponse response = new ForgotPasswordResponse();

            User userDAL;
            if (request.username != null)
            {
                userDAL = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => x.UserName == request.username).FirstOrDefault();
                if (userDAL != null)
                {
                    var locked = await userManager.IsLockedOutAsync(new ApplicationUser { Id = userDAL.Id });
                    if (ValidateUserEligibility(userDAL))
                    {
                        if (!locked)
                        {
                            var emailConfirmated = await userManager.IsEmailConfirmedAsync(new ApplicationUser { Id = userDAL.Id });
                            if (emailConfirmated)
                            {
                                /*userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ResetPassword"))
                                {
                                    TokenLifespan = TimeSpan.FromHours(1),
                                };*/
                                var token = await userManager.GeneratePasswordResetTokenAsync(new ApplicationUser { Id = userDAL.Id });
                                response.resetToken = token;
                                response.user = Utility.mapper.Map<User, UserDto>(userDAL);
                            }
                            else
                            {
                                response.Error = "confirmEmail";
                            }
                        }
                        else
                        {
                            response.Error = "Your account has been locked. Please wait till " + TimeZoneInfo.ConvertTimeFromUtc(userDAL.LockoutEndDateUtc.Value, TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time")) + " Eastern Time to reset your password";
                        }
                    }
                    else
                    {
                        response.Error = "Your account is inactive. For further assistance please email us at " + userDAL.Organization.ContactEmail + ".";
                        return response;
                    }
                }
                else
                {
                    response.Error = "user not found";
                }
            }
            return response;
        }

        public async Task<string> GenerateToken(int userId)
        {
            //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ConfirmEmail"));
            string Tocken = await userManager.GenerateEmailConfirmationTokenAsync(new ApplicationUser { Id = userId });
            return Tocken;
        }

        public async Task<ValidateTokenResponse> ValidateToken(ValidateTokenRequest request)
        {
            ValidateTokenResponse response = new ValidateTokenResponse();

            var userResult = await userManager.FindByNameAsync(request.email);
            //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ResetPassword"));
            var result = await userManager.VerifyUserTokenAsync(new ApplicationUser { Id = userResult.Id }, userManager.Options.Tokens.PasswordResetTokenProvider, UserManager<ApplicationUser>.ResetPasswordTokenPurpose, request.token);
            response.Succeeded = result;
            return response;
        }

        public async Task<ResetPasswordResponse> ResetPassword(ResetPasswordRequest request)
        {
            ResetPasswordResponse response = new ResetPasswordResponse();

            var userResult = await userManager.FindByNameAsync(request.Email);
            if (request.DOB.HasValue && userResult.DOB != request.DOB)
            {
                response.validateDOB = false;
                return response;
            }
            response.validateDOB = true;
            //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ResetPassword"));
            var result = await userManager.ResetPasswordAsync(new ApplicationUser { Id = userResult.Id }, request.token, request.Password);
            response.success = result.Succeeded;
            response.message = result.Errors.FirstOrDefault().Description;
            return response;
        }

        public async Task<ConfirmEmailResponse> ConfirmEmailAsync(ConfirmEmailRequest request)
        {
            ConfirmEmailResponse response = new ConfirmEmailResponse();

            var userResult = await userManager.FindByNameAsync(request.Email);
            //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ConfirmEmail"));
            var result = await userManager.ConfirmEmailAsync(new ApplicationUser { Id = userResult.Id }, request.token);
            response.Succeeded = result.Succeeded;
            if (!response.Succeeded)
                response.error = result.Errors;
            return response;
        }

        /*public async Task<string> SendConfirmationEmail(string email, IDataProtectionProvider dataProtectionProvider)
        {

            var userResult = await userManager.FindByNameAsync(email);
            //userManager.UserTokenProvider = new DataProtectorTokenProvider<User, int>(dataProtectionProvider.Create("ConfirmEmail"));
            return await userManager.GenerateEmailConfirmationTokenAsync(new ApplicationUser { Id = userResult.Id });
        }*/

        public async Task<string> UploadPicture(UploadProfilePictureRequest request)
        {
            var user = await userManager.FindByIdAsync(request.userId.ToString());
            var currentPicture = user.Picture;
            user.Picture = request.fileName;
            await userManager.UpdateAsync(user);
            return currentPicture;
        }

        public async void DeletePicture(int userId)
        {
            var user = await userManager.FindByIdAsync(userId.ToString());
            user.Picture = null;
            await userManager.UpdateAsync(user);
        }

        public async Task<AddUserToRoleResponse> AddUserToRole(AddUserToRoleRequest request)
        {
            AddUserToRoleResponse response = new AddUserToRoleResponse();

            var user = await userManager.FindByIdAsync(request.userId.ToString());
            //delete existing role
            if (!string.IsNullOrEmpty(request.currentRole))
            {
                var roles = request.currentRole.Split('-');
                await userManager.RemoveFromRolesAsync(user, roles);
            }
            //create new role
            if (!string.IsNullOrEmpty(request.newRole))
            {
                var roles = request.newRole.Split('-');
                var result = await userManager.AddToRolesAsync(user, roles);
                if (result.Succeeded)
                    response.succeeded = result.Succeeded;
            }
            return response;
        }

        public ListRolesResponse ListRoles(ListRolesRequest request)
        {
            ListRolesResponse response = new ListRolesResponse();
            var role = dbcontext.UserRoles.ToList();
            response.Roles = Utility.mapper.Map<IList<UserRole>, IList<RoleDto>>(role);
            return response;
        }

        public ListSpecializationResponse ListSpecialization(SpecializationRequest request)
        {
            ListSpecializationResponse response = new ListSpecializationResponse();
            IList<Specialization> specialization;
            if (request.portalId != 0)
            {
                var portal = dbcontext.Portals.Include("Specializations").Where(x => x.Id == request.portalId).FirstOrDefault();
                specialization = portal.Specializations.ToList();
            }
            else
                specialization = dbcontext.Specializations.ToList();

            response.specialization = Utility.mapper.Map<IList<Specialization>, IList<SpecializationDto>>(specialization);
            response.specializationList = string.Join(",", specialization.Select(x => x.Id));
            return response;
        }

        public ListUserInactiveReasonResponse ListInactiveReason()
        {
            ListUserInactiveReasonResponse response = new ListUserInactiveReasonResponse();
            var inactiveReason = dbcontext.UserInactiveReasons.ToList();
            response.InactiveReasons = Utility.mapper.Map<IList<UserInactiveReason>, IList<UserInactiveReasonDto>>(inactiveReason);
            return response;
        }

        public ReadRoleResponse ReadRole(ReadRoleRequest request)
        {
            ReadRoleResponse response = new ReadRoleResponse();
            var role = dbcontext.UserRoles.Include("AdminModules").Include("Organizations").Where(x => x.Id == request.id).FirstOrDefault();
            response.role = Utility.mapper.Map<UserRole, RoleDto>(role);
            return response;
        }

        public AddEditRoleResponse AddEditRole(AddEditRoleRequest request)
        {
            AddEditRoleResponse response = new AddEditRoleResponse();
            UserRole role = new UserRole();
            if (request.role.Id > 0)
                role = dbcontext.UserRoles.Include("AdminModules").Include("Organizations").Where(x => x.Id == request.role.Id).FirstOrDefault();
            role.Name = request.role.Name;
            role.Code = request.role.Code;
            role.AdminModules.Clear();
            role.Organizations.Clear();
            if (!string.IsNullOrEmpty(request.adminModules))
            {
                List<string> adminModules = request.adminModules.Split('-').ToList<string>();
                for (int i = 0; i < adminModules.Count; i++)
                {
                    var moduleId = Convert.ToInt32(adminModules[i]);
                    AdminModule currentModule = dbcontext.AdminModules.Where(x => x.Id == moduleId).FirstOrDefault();
                    role.AdminModules.Add(currentModule);
                }
            }
            if (!string.IsNullOrEmpty(request.organizations))
            {
                List<string> organizations = request.organizations.Split('-').ToList<string>();
                for (int i = 0; i < organizations.Count; i++)
                {
                    var OrganizationId = Convert.ToInt32(organizations[i]);
                    Organization organization = dbcontext.Organizations.Where(x => x.Id == OrganizationId).FirstOrDefault();
                    role.Organizations.Add(organization);
                }
            }
            if (request.role.Id > 0)
            {
                role.Id = request.role.Id;
                dbcontext.UserRoles.Attach(role);
                dbcontext.Entry(role).State = EntityState.Modified;
            }
            else
            {
                role.Name = request.role.Name;
                role.Code = request.role.Code;
                dbcontext.UserRoles.Add(role);
            }
            dbcontext.SaveChanges();
            response.success = true;
            return response;
        }

        public async Task<GetUsersByRoleResponse> GetUsersByRole(GetUsersByRoleRequest request)
        {
            GetUsersByRoleResponse response = new GetUsersByRoleResponse();
            IList<User> result = null;

            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            PortalReader reader = new PortalReader();
            var organizationsList = reader.GetFilteredOrganizationsList(request.userId.Value).Organizations.Select(x => x.Id).ToList();
            if (request.page.HasValue && request.pageSize.HasValue)
            {
                if (totalRecords == 0)
                {
                    if (request.allUserswithRole)
                    {
                        totalRecords = dbcontext.Users.Where(x => x.UserRoles.Any() && (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                            && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                            && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).Count();
                    }
                    else
                    {
                        totalRecords = userManager.Users.Where(x => (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                            && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                            && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).Count();
                    }
                }
                if (request.allUserswithRole)
                {
                    result = dbcontext.Users.Where(x => x.UserRoles.Any() && (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                        && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                        && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).Skip(request.page.Value * request.pageSize.Value).Take(request.pageSize.Value).ToList();
                }
                else
                {
                    result = dbcontext.Users.Where(x => (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                        && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                        && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).Skip(request.page.Value * request.pageSize.Value).Take(request.pageSize.Value).ToList();
                }
            }
            else
            {
                if (request.allUserswithRole)
                {
                    result = dbcontext.Users.Where(x => x.UserRoles.Any() && (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                        && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                        && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).ToList();
                }
                else
                {
                    result = dbcontext.Users.Where(x => (x.OrganizationId == request.organization || (request.organization == null && organizationsList.Count > 0 && organizationsList.Contains(x.OrganizationId)))
                        && (string.IsNullOrEmpty(request.firstName) || x.FirstName.StartsWith(request.firstName))
                        && (string.IsNullOrEmpty(request.LastName) || x.LastName.StartsWith(request.LastName))).OrderBy(x => x.FirstName).ToList();
                }
            }

            if (result != null)
            {
                List<UserDto> users = new List<UserDto>();
                for (int i = 0; i < result.Count; i++)
                {
                    UserDto user = new UserDto();
                    user.NamePrefix = result[i].NamePrefix;
                    user.FirstName = result[i].FirstName;
                    user.LastName = result[i].LastName;
                    user.Id = result[i].Id;
                    user.Email = result[i].Email;
                    user.HomeNumber = result[i].HomeNumber;
                    user.CellNumber = result[i].CellNumber;
                    user.DOB = result[i].DOB;
                    users.Add(user);
                }
                response.users = users;
                response.totalRecords = totalRecords;
            }
            return response;
        }

        public CoachListResponse GetCoachList(CoachListRequest request)
        {
            CoachListResponse response = new CoachListResponse();
            StoredProcedures sp = new StoredProcedures();
            var result = sp.GetCoachList(request.OrganizationIds, request.active, request.stateId, request.allowAppt).OrderBy(c => c.FirstName);
            if (result != null)
            {
                List<UserDto> users = new List<UserDto>();
                foreach (var user in result)
                {
                    UserDto User = new UserDto();
                    User.Id = user.Id;
                    User.FirstName = user.FirstName;
                    User.LastName = user.LastName;
                    User.RoleCode = user.Code;
                    users.Add(User);
                }
                response.users = users;
            }
            return response;
        }

        public FilteredCoachListResponse GetFilteredCoachList(FilteredCoachListRequest request)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            FilteredCoachListResponse response = new FilteredCoachListResponse();
            StoredProcedures sp = new StoredProcedures();
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            if (request.startDate.HasValue)
                startDate = TimeZoneInfo.ConvertTimeToUtc(request.startDate.Value, custTZone);
            if (request.endDate.HasValue)
                endDate = TimeZoneInfo.ConvertTimeToUtc(request.endDate.Value, custTZone);
            var result = sp.GetFilteredCoachList(request.coachName, request.speciality, request.language, request.OrganizationId, startDate, endDate, request.stateId).OrderByDescending(x => x.AvailabilityCount).ToList();
            response.coachList = Utility.mapper.Map<List<FilteredCoachListResult>, List<FilteredCoachListResultDto>>(result);
            return response;
        }

        public AddEditAdminPropResponse AddEditAdminProperty(AddEditAdminPropRequest request)
        {
            AddEditAdminPropResponse response = new AddEditAdminPropResponse();

            var adminProperty = dbcontext.AdminProperties.Where(x => x.Id == request.AdminProperty.Id).FirstOrDefault();
            if (adminProperty != null)
            {
                if (!string.IsNullOrEmpty(request.AdminProperty.ProfileLanguageItem))
                    request.AdminProperty.Profile = adminProperty.Profile;
                else
                    request.AdminProperty.ProfileLanguageItem = adminProperty.ProfileLanguageItem;
                if (!string.IsNullOrEmpty(request.AdminProperty.MeetingId))
                    adminProperty.MeetingId = request.AdminProperty.MeetingId;
                var Update = Utility.mapper.Map<AdminPropertyDto, AdminProperty>(request.AdminProperty);
                dbcontext.Entry(adminProperty).CurrentValues.SetValues(Update);
            }
            else
            {
                adminProperty = Utility.mapper.Map<AdminPropertyDto, AdminProperty>(request.AdminProperty);
                dbcontext.AdminProperties.Add(adminProperty);
            }
            dbcontext.SaveChanges();
            response.success = true;
            return response;
        }

        public CheckifRegisteredResponse CheckifRegistered(CheckifRegisteredRequest request)
        {
            CheckifRegisteredResponse response = new CheckifRegisteredResponse();
            bool recordExist = false;
            var user = dbcontext.Users.Where(x => x.UniqueId == request.UniqueId && x.OrganizationId == request.OrganizationId).FirstOrDefault();
            if (user != null)
            {
                recordExist = true;
                response.User = Utility.mapper.Map<User, UserDto>(user);
            }
            response.recordExist = recordExist;
            return response;
        }

        public IList<UserDto> TriggerBirthdayWishes()
        {
            var CurrentDate = DateTime.UtcNow;
            var users = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => x.DOB.Value.Month == CurrentDate.Date.Month && x.DOB.Value.Day == CurrentDate.Date.Day && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true) && !x.UserTrackingStatuses.Any(z => z.DeclinedEnrollment == true && z.Portal.Active == true)).ToList();
            var Users = Utility.mapper.Map<IList<User>, IList<UserDto>>(users);
            return Users;
        }

        public IList<UserDto> TriggerDowntimeEmail()
        {
            var users = dbcontext.Users.Include("Organization").Where(x => x.IsActive == true).ToList();
            var Users = Utility.mapper.Map<IList<User>, IList<UserDto>>(users);
            return Users;
        }

        public IList<UserDto> IncompleteProfile()
        {
            var CurrentDate = DateTime.UtcNow;
            List<UserDto> userlist = new List<UserDto>();
            var users = dbcontext.Users.Include("Organization.Portals").Where(x => (x.Complete == null || x.Complete == false) && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true && y.EndDate > CurrentDate) && ((x.CreatedOn.Value.AddMonths(1) >= CurrentDate && EF.Functions.DateDiffDay(x.CreatedOn, CurrentDate) % 7 == 0) || (x.CreatedOn.Value.Date.Day == CurrentDate.Day))).ToList();
            var Users = Utility.mapper.Map<IList<User>, IList<UserDto>>(users);
            foreach (var user in Users)
            {
                if (user.Organization.Portals.Where(x => x.Active == true).FirstOrDefault().EligibilityImportLoadFlag)
                {
                    var eligibility = dbcontext.Eligibilities.Where(x => x.EnrollmentStatus != 'T').FirstOrDefault();
                    if (eligibility != null)
                    {
                        userlist.Add(user);
                    }
                }
                else
                {
                    userlist.Add(user);
                }
            }
            return userlist;
        }

        public IList<UserDto> IncompleteHRA()
        {
            var CurrentDate = DateTime.UtcNow;
            List<UserDto> userlist = new List<UserDto>();
            var users = dbcontext.Users.Include("HRAs").Include("Organization.Portals").Where(x => x.Complete == true && x.IsActive == true && x.Organization.Portals.Any(y => y.Active == true && y.EndDate > CurrentDate)).ToList();
            var Users = Utility.mapper.Map<IList<User>, IList<UserDto>>(users);
            foreach (var user in Users)
            {
                var portal = user.Organization.Portals.LastOrDefault();
                if (user.HRAs == null)
                {
                    var InitialDate = Convert.ToDateTime(portal.StartDate) > user.CreatedOn ? Convert.ToDateTime(portal.StartDate) : user.CreatedOn;
                    if ((InitialDate.AddMonths(1) >= CurrentDate && InitialDate.DayOfWeek == CurrentDate.DayOfWeek) || (InitialDate.Day == CurrentDate.Day))
                    {
                        userlist.Add(user);
                    }
                }
                else if (user.HRAs.Where(x => x.PortalId == portal.Id && x.CompleteDate == null && ((x.StartDate.AddMonths(1) >= CurrentDate && x.StartDate.DayOfWeek == CurrentDate.DayOfWeek) || (x.StartDate.Day == CurrentDate.Day))).FirstOrDefault() != null)
                {
                    userlist.Add(user);
                }
            }
            return userlist;
        }

        public UserDto GetUserByPhone(string phoneNumber)
        {
            var phone = phoneNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "");
            var user = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => (x.HomeNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(phone) > -1)
                || (x.WorkNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(phone) > -1)
                || (x.CellNumber.Replace("-", "").Replace("(", "").Replace(")", "").Replace(" ", "").IndexOf(phone) > -1))
                .FirstOrDefault();
            var userDto = Utility.mapper.Map<User, UserDto>(user);
            return userDto;
        }

        public LabDto GetUserByBasicDetail(string firstName, string lastName, DateTime dob)
        {
            var users = dbcontext.Users.Include("Labs2").Where(x => (x.FirstName.ToLower() == firstName.ToLower() && x.LastName.ToLower() == lastName.ToLower() && x.Labs2.Any(l => !string.IsNullOrEmpty(l.OrderNo)))).ToList();
            var userList = new List<User>();
            foreach (var user in users)
            {
                if (user.DOB.HasValue)
                {
                    var userDob = user.DOB.Value;
                    if (userDob.Year == dob.Year && userDob.Month == dob.Month && userDob.Date == dob.Date)
                    {
                        userList.Add(user);
                    }
                }
            }

            if (userList.Count() == 1)
            {
                if (userList[0].Labs2 != null && userList[0].Labs2.Count() > 0)
                {
                    Lab lab = userList[0].Labs2.FirstOrDefault();
                    if (userList[0].Labs2.Count() > 1)
                        lab = userList[0].Labs2.OrderByDescending(l => l.Id).FirstOrDefault();
                    lab.User = lab.User1 = lab.User2 = null;
                    return Utility.mapper.Map<Lab, LabDto>(lab);
                }
            }
            return null;
        }

        public IList<UserDto> GetUserByFullName(string fullName)
        {
            var user = dbcontext.Users.Include("Organization").Include("Organization.Portals").Where(x => (x.FirstName + " " + x.LastName).ToLower().Contains(fullName.ToLower())).ToList();
            var userDto = Utility.mapper.Map<IList<User>, IList<UserDto>>(user);
            return userDto;
        }

        public int AddOrEditUserLoggedInDevice(int userId, string deviceId)
        {
            UserLoggedInDevice detail = dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId).FirstOrDefault();
            if (detail != null)
            {
                detail.UserId = userId;
                detail.DeviceId = deviceId;
                dbcontext.UserLoggedInDevices.Attach(detail);
                dbcontext.Entry(detail).State = EntityState.Modified;
            }
            else
            {
                detail = new UserLoggedInDevice();
                detail.UserId = userId;
                detail.DeviceId = deviceId;
                var result = dbcontext.UserLoggedInDevices.Add(detail);
                dbcontext.SaveChanges();
                return result.Entity.Id;
            }
            dbcontext.SaveChanges();
            return detail.Id;
        }

        public void AddOrEditNotificationToken(int userId, string deviceId, string token)
        {
            UserLoggedInDevice detail = dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId).FirstOrDefault();
            if (detail != null)
            {
                detail.UserId = userId;
                detail.DeviceId = deviceId;
                detail.Token = token;
                dbcontext.UserLoggedInDevices.Attach(detail);
                dbcontext.Entry(detail).State = EntityState.Modified;
                dbcontext.SaveChanges();
            }
        }

        public bool AddOrEditRefreshToken(int userId, string deviceId, DateTime expires, string refreshToken)
        {
            UserLoggedInDevice detail = dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId).FirstOrDefault();
            if (detail != null)
            {
                detail.RefreshToken = refreshToken;
                detail.ExpiresOn = expires;
                dbcontext.UserLoggedInDevices.Attach(detail);
                dbcontext.Entry(detail).State = EntityState.Modified;
                dbcontext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool ValidateRefreshToken(int userId, string deviceId, DateTime expires, string refreshToken)
        {
            UserLoggedInDevice detail = dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId && x.RefreshToken == refreshToken).FirstOrDefault();
            return detail != null && DateTime.Compare(detail.ExpiresOn.Value, expires) == 0 && DateTime.Compare(detail.ExpiresOn.Value, DateTime.UtcNow) == 1 ? true : false;
        }

        public void LogoutUser(int userId, string deviceId)
        {
            UserLog detail = dbcontext.UserLogs.Where(x => x.UserLoggedInDevice.DeviceId == deviceId && x.UserLoggedInDevice.UserId == userId).OrderByDescending(x => x.Id).FirstOrDefault();
            if (detail != null)
            {
                detail.LoggedOutTime = DateTime.UtcNow;
                dbcontext.UserLogs.Attach(detail);
                dbcontext.Entry(detail).State = EntityState.Modified;
                dbcontext.SaveChanges();
            }
        }

        public bool IsVerifiedUserDevice(int userId, string deviceId)
        {
            return dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId && x.SecurityCodes.Any(y => y.Status == 2)).Count() > 0;
        }

        public UserLoggedInDevicesDto GetUserLoggedInDevice(int userId, string deviceId)
        {
            var device = dbcontext.UserLoggedInDevices.Where(x => x.UserId == userId && x.DeviceId == deviceId).FirstOrDefault();
            return Utility.mapper.Map<UserLoggedInDevice, UserLoggedInDevicesDto>(device);
        }

        public UserEligibleToSendCodeResponse IsUserEligibleToSendCode(UserEligibleToSendCodeRequest request)
        {
            UserEligibleToSendCodeResponse response = new UserEligibleToSendCodeResponse();
            DateTime expiryTime = DateTime.UtcNow.AddMinutes(-30);
            var securityCode = dbcontext.UserLoggedInDevices.Include("SecurityCodes").Where(x => x.UserId == request.userId && x.Id == request.loggedinDeviceId).FirstOrDefault().SecurityCodes.ToList();
            response.isEligible = securityCode.Where(y => y.ExpireTime > expiryTime).Count() < 3;
            if (!response.isEligible)
            {
                response.waitTime = securityCode.OrderByDescending(x => x.Id).FirstOrDefault().ExpireTime.AddMinutes(30);
            }
            var codes = securityCode.Where(x => x.Status == request.active).ToList();
            foreach (var code in codes)
            {
                ChangeSecurityCodeStatus(code, request.expired);
            }
            return response;
        }

        public void AddSecurityCode(AddSecurityCodeRequest request)
        {
            SecurityCode detail = new SecurityCode();
            detail.Code = request.code;
            detail.DeviceId = request.deviceId;
            detail.ExpireTime = DateTime.UtcNow.AddMinutes(5);
            detail.Status = request.status;
            dbcontext.SecurityCodes.Add(detail);
            dbcontext.SaveChanges();
        }

        public bool VerifySecurityCode(int userId, string code, int deviceId, byte active, byte validated)
        {
            DateTime expiryTime = DateTime.UtcNow;
            var record = dbcontext.UserLoggedInDevices.Include("SecurityCodes").Where(x => x.UserId == userId && x.Id == deviceId).FirstOrDefault().SecurityCodes.OrderByDescending(x => x.Id).FirstOrDefault();
            if (record != null && record.Code == code && record.ExpireTime > expiryTime && record.Status == active)
            {
                return ChangeSecurityCodeStatus(record, validated);
            }
            return false;
        }

        public bool ChangeSecurityCodeStatus(SecurityCode code, byte status)
        {
            if (code != null)
            {
                code.Status = status;
                dbcontext.SecurityCodes.Attach(code);
                dbcontext.Entry(code).State = EntityState.Modified;
                dbcontext.SaveChanges();
                return true;
            }
            return false;
        }

    }
}