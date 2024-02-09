namespace Intervent.Web.DTO
{
    public class UserDto
    {
        public int Id { get; set; }

        public string? UserName { get; set; }

        public string? Email { get; set; }

        public bool EmailConfirmed { get; set; }

        public string? PasswordHash { get; set; }

        public string? SecurityStamp { get; set; }

        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public Nullable<System.DateTime> LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        public string? NamePrefix { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public string? MiddleName { get; set; }

        public string? Suffix { get; set; }

        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public int? Race { get; set; }

        public string? RaceOther { get; set; }

        public string? Address { get; set; }

        public string? Address2 { get; set; }

        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        public string? Zip { get; set; }

        public string? HomeNumber { get; set; }

        public string? WorkNumber { get; set; }

        public string? CellNumber { get; set; }

        public int PortalId { get; set; }

        public int? TimeZoneId { get; set; }

        public string? TimeZone { get; set; }

        public string? TimeZoneName { get; set; }

        public byte? PreferredContactTimeId { get; set; }

        public int? ProfessionId { get; set; }

        public int OrganizationId { get; set; }

        public string? Occupation { get; set; }

        public byte? Source { get; set; }

        public string? SourceOther { get; set; }

        public string? ReferralDetails { get; set; }

        public byte? Text { get; set; }

        public byte? PrimaryCarePhysician { get; set; }

        public string? Picture { get; set; }

        public byte? Unit { get; set; }

        public bool IsActive { get; set; }

        public int? InactiveReason { get; set; }

        public IList<RoleDto> Roles { get; set; }

        public string? RoleCode { get; set; }

        public bool? Complete { get; set; }

        public byte? ContactMode { get; set; }

        public string? UniqueId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string? CouponCode { get; set; }

        public bool? TermsAccepted { get; set; }

        public int? DeptId { get; set; }

        public OrganizationDto Organization { get; set; }

        public string? LanguagePreference { get; set; }

        public string? LastVisited { get; set; }

        public string? EmployeeId { get; set; }

        public string? UnsubscribedEmail { get; set; }

        public List<UserDoctorInfoDto> UserDoctorInfoes { get; set; }

        public IList<HRADto> HRAs { get; set; }

        public IList<LabDto> Labs2 { get; set; }

        public IList<AppointmentDTO> Appointments { get; set; }

        public IList<NotesDto> Notes { get; set; }

        public List<NotificationEventDto> NotificationEvents { get; set; }

        public IList<UsersinProgramDto> UsersinPrograms { get; set; }

        public IList<UserDashboardMessageDto> UserDashboardMessages { get; set; }

        public IList<SpecializationDto> Specializations { get; set; }

        public IList<LanguagesDto> Languages { get; set; }

        public IList<StateDto> CoachStates { get; set; }

        public AdminPropertyDto AdminProperty { get; set; }

        public int? CreatedBy { get; set; }

        public CountryDto Country1 { get; set; }

        public StateDto State1 { get; set; }

        public IList<UserTrackingStatusDto> UserTrackingStatuses { get; set; }

        public IList<UserLogsDto> UserLogs { get; set; }

        public virtual IList<UserTimeTrackerDto> UserTimeTracker { get; set; }

        public IList<UserLoggedInDevicesDto> UserLoggedInDevices { get; set; }

        public virtual string BaseUrl { get; set; }
    }
}