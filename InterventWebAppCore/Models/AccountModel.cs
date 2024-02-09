using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace InterventWebApp
{

    public class IntuityModel
    {
        public string token { get; set; }

        public string utcdatetime { get; set; }

        public string authorization { get; set; }

    }



    public class IntuityRedirectModel
    {
        public string state { get; set; }

    }
    public class ChangePasswordModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 8)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }

    public class LoginModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }

        public string SuccessMessage { get; set; }

        public string FailureMessage { get; set; }

        public string OrganizationEmail { get; set; }

        public string DeviceId { get; set; }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Unique ID")]
        public string UniqueID { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Display(Name = "SID")]
        public string SID { get; set; }

    }

    public class ExternalRegisterModel
    {
        [Required]
        [Display(Name = "User name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [Display(Name = "Email address")]
        public string Email { get; set; }

        [Required]
        [Display(Name = "First name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last name")]
        public string LastName { get; set; }

        [Required]
        [Display(Name = "Date Of Birth")]
        public DateTime BirthDate { get; set; }

        [Required]
        [Display(Name = "Gender")]
        public IEnumerable<SelectListItem> GenderList { get; set; }

        public byte? Gender { get; set; }
    }

    public class NoSamlModel
    {
        public string Url { get; set; }
    }

    public class UserProfileModel
    {
        public UserDto user { get; set; }

        public IEnumerable<SelectListItem> NamePrefixList { get; set; }

        public IEnumerable<SelectListItem> years { get; set; }

        public int year { get; set; }

        public IEnumerable<SelectListItem> months { get; set; }

        public int month { get; set; }

        public IEnumerable<SelectListItem> days { get; set; }

        public int day { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }

        public IEnumerable<SelectListItem> RaceList { get; set; }

        public IEnumerable<SelectListItem> PreferredContactTimes { get; set; }

        public IEnumerable<SelectListItem> ContactModes { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public IEnumerable<SelectListItem> LanguagePreferences { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> DoctorInfo_States { get; set; }

        public IEnumerable<SelectListItem> Sources { get; set; }

        public IEnumerable<SelectListItem> ListOrganizations { get; set; }

        public IEnumerable<SelectListItem> Units { get; set; }

        public IEnumerable<SelectListItem> InactiveReason { get; set; }

        public IEnumerable<SelectListItem> Departments { get; set; }

        public IList<int> EligibilityImportLoadList { get; set; }

        public bool notifyEmailChange { get; set; }

        public bool readOnly { get; set; }

        public IList<string> readOnlyList { get; set; }

        public string module { get; set; }

        public bool? create { get; set; }

        public string existingEmail { get; set; }

        public bool LockOption { get; set; }

        public bool IsLocked { get; set; }

        public bool basicDetails { get; set; }

        public bool contactDetails { get; set; }

        public bool basicDetailsCompeleted { get; set; }

        public bool contactDetailsCompeleted { get; set; }

        public bool ProviderDropDown { get; set; }

        public bool SkipValidation { get; set; }

        public IEnumerable<SelectListItem> ProvidersList { get; set; }

        public int? gender { get; set; }
        public bool coachingProgram { get; set; }
        public bool selfHelpProgram { get; set; }
        public int? adminId { get; set; }
        public string sso { get; set; }
        public string integrationWith { get; set; }
        public string orgContactEmail { get; set; }

        public string DateFormat { get; set; }

        public bool ShowSelfScheduling { get; set; }

        public bool ShowProgram { get; set; }
    }

    public class AdminProfileModel
    {
        public UserDto user { get; set; }

        public IEnumerable<SelectListItem> NamePrefixList { get; set; }

        public IEnumerable<SelectListItem> years { get; set; }

        public int year { get; set; }

        public IEnumerable<SelectListItem> months { get; set; }

        public int month { get; set; }

        public IEnumerable<SelectListItem> days { get; set; }

        public int day { get; set; }

        public IEnumerable<SelectListItem> GenderList { get; set; }

        public IEnumerable<SelectListItem> TimeZones { get; set; }

        public IEnumerable<SelectListItem> Countries { get; set; }

        public IEnumerable<SelectListItem> States { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }

        public IEnumerable<SelectListItem> Specialization { get; set; }

        public IEnumerable<SelectListItem> Language { get; set; }

        public IEnumerable<SelectListItem> States1 { get; set; }

        public IEnumerable<SelectListItem> Units { get; set; }

        public string Role { get; set; }

        public IList<RoleDto> UserRoles { get; set; }

        public IList<SpecializationDto> UserSpecializations { get; set; }

        public IList<LanguagesDto> UserLanguages { get; set; }

        public IList<StateDto> CoachStates { get; set; }

        public string CurrentRole { get; set; }

        public string Password { get; set; }

        public string existingEmail { get; set; }

        public IEnumerable<SelectListItem> ListOrganizations { get; set; }

        public string CurrentSpecialization { get; set; }

        public string CurrentLanguage { get; set; }

        public string CurrenCoachtState { get; set; }

        public string BaseUrl { get; set; }
    }
    public class CreateUserModel
    {
        public UserDto user { get; set; }

        public bool? welcomeEmail { get; set; }

        public string rootPath { get; set; }
        public string InfoEmail { get; set; }
        public string SecureEmail { get; set; }
        public string SMPTAddress { get; set; }
        public string PortNumber { get; set; }
        public string SecureEmailPassword { get; set; }
        public string MailAttachmentPath { get; set; }
    }
    public class RolesModel
    {
    }

    public class ManageRoleModel
    {
        [Required]
        [Display(Name = "Role name")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "Portals")]
        public IEnumerable<SelectListItem> Portals { get; set; }

        public int PortalId { get; set; }

        public string PortalIdList { get; set; }

        public int RoleId { get; set; }
    }

    public class ForgotPasswordModel
    {
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }

        public string FailureMessage { get; set; }
    }

    public class ConfirmEmailModel
    {
        public string Email { get; set; }

        public string Message { get; set; }
    }

    public class ResetPasswordModel
    {
        //public int UserId { get; set; }

        public string Email { get; set; }

        public string Token { get; set; }

        public string DOB { get; set; }

        [Required]
        [Display(Name = "New Password")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }

        public string FailureMessage { get; set; }
    }

    public class DateItem
    {
        public IEnumerable<SelectListItem> years { get; set; }

        public IEnumerable<SelectListItem> months { get; set; }

        public IEnumerable<SelectListItem> days { get; set; }
    }

    public class EmailSubscriptionModel
    {
        public int userId { get; set; }

        public string name { get; set; }

        public string emailId { get; set; }

        public string unsubscribedEmail { get; set; }

        public IList<NotificationCategoryDto> emailCategory { get; set; }
    }

    public class MediOrbisOAuth
    {
        public string access_token { get; set; }

        public string id_token { get; set; }

        public string token_type { get; set; }

        public int expires_in { get; set; }
    }

    public class MediOrbisProfile
    {
        public string firstname { get; set; }

        public string middlename { get; set; }

        public string lastname { get; set; }

        public string email { get; set; }

        public string gender { get; set; }

        public string dob { get; set; }

        public string city { get; set; }

        public string address { get; set; }

        public string state { get; set; }

        public string country { get; set; }

        public string postalcode { get; set; }

        public string phone { get; set; }

        public string language { get; set; }

        public string unit { get; set; }

        public string timezone { get; set; }

        public string refId { get; set; }

        public string clientRefId { get; set; }

        public string selfCareTool { get; set; }
    }
}