namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class User
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            AdminTasks = new HashSet<AdminTask>();
            AdminTasks1 = new HashSet<AdminTask>();
            AdminTasks2 = new HashSet<AdminTask>();
            AdminTasks3 = new HashSet<AdminTask>();
            Appointments = new HashSet<Appointment>();
            Appointments1 = new HashSet<Appointment>();
            Appointments2 = new HashSet<Appointment>();
            Appointments3 = new HashSet<Appointment>();
            Appointments4 = new HashSet<Appointment>();
            ApptCallTemplates = new HashSet<ApptCallTemplate>();
            Availabilities = new HashSet<Availability>();
            AWVs = new HashSet<AWV>();
            EligibilityNotes = new HashSet<EligibilityNote>();
            Emails = new HashSet<Email>();
            Emails1 = new HashSet<Email>();
            ExerciseDiaries = new HashSet<ExerciseDiary>();
            EXT_Glucose = new HashSet<EXT_Glucose>();
            ExternalUsers = new HashSet<ExternalUser>();
            FoodDiaries = new HashSet<FoodDiary>();
            HRAs = new HashSet<HRA>();
            HRAs1 = new HashSet<HRA>();
            LabErrorLogs = new HashSet<LabErrorLog>();
            Labs = new HashSet<Lab>();
            Labs1 = new HashSet<Lab>();
            Labs2 = new HashSet<Lab>();
            Mails = new HashSet<Mail>();
            Mails1 = new HashSet<Mail>();
            Notes = new HashSet<Note>();
            Notes1 = new HashSet<Note>();
            Recipes = new HashSet<Recipe>();
            NotificationEvents = new HashSet<NotificationEvent>();
            PaymentTransactions = new HashSet<PaymentTransaction>();
            SleepLogs = new HashSet<SleepLog>();
            StrengthTrainingLogs = new HashSet<StrengthTrainingLog>();
            StressDiaries = new HashSet<StressDiary>();
            StressManagementLogs = new HashSet<StressManagementLog>();
            TobaccoLogs = new HashSet<TobaccoLog>();
            TobaccoLogs1 = new HashSet<TobaccoLog>();
            TobaccoLogs2 = new HashSet<TobaccoLog>();
            UserClaims = new HashSet<UserClaim>();
            UserDashboardMessages = new HashSet<UserDashboardMessage>();
            UserDoctorInfoes = new HashSet<UserDoctorInfo>();
            UserDrugs = new HashSet<UserDrug>();
            UserDrugs1 = new HashSet<UserDrug>();
            UserDrugs2 = new HashSet<UserDrug>();
            UserHistories = new HashSet<UserHistory>();
            UserHistories1 = new HashSet<UserHistory>();
            UserLogs = new HashSet<UserLog>();
            UserLogins = new HashSet<UserLogin>();
            Users1 = new HashSet<User>();
            UsersinPrograms = new HashSet<UsersinProgram>();
            UsersinPrograms1 = new HashSet<UsersinProgram>();
            UsersinPrograms2 = new HashSet<UsersinProgram>();
            UserTrackingStatuses = new HashSet<UserTrackingStatus>();
            VitalsLogs = new HashSet<VitalsLog>();
            WeightLossJournals = new HashSet<WeightLossJournal>();
            WellnessDatas = new HashSet<WellnessData>();
            WellnessDatas1 = new HashSet<WellnessData>();
            WellnessDatas2 = new HashSet<WellnessData>();
            UserRoles = new HashSet<UserRole>();
            Specializations = new HashSet<Specialization>();
            Languages = new HashSet<Languages>();
            CoachStates = new HashSet<State>();
            UserRaffles = new HashSet<UserRaffles>();
            EXT_Summaries = new HashSet<EXT_Summaries>();
            EXT_Sleeps = new HashSet<EXT_Sleeps>();
            EXT_Weights = new HashSet<EXT_Weights>();
            EXT_Workouts = new HashSet<EXT_Workouts>();
            EXT_BloodPressure = new HashSet<EXT_BloodPressures>();
            PreviousUserPasswords = new List<PreviousPassword>();
            Messages = new HashSet<Message>();
            MessageRecipients = new HashSet<MessageRecipient>();
            MessageRecipients1 = new HashSet<MessageRecipient>();
            CRM_ChangeLogs = new HashSet<CRM_ChangeLog>();
            CRM_Contacts = new HashSet<CRM_Contact>();
            CRM_Contacts1 = new HashSet<CRM_Contact>();
            CRM_Notes = new HashSet<CRM_Note>();
            CRM_Notes1 = new HashSet<CRM_Note>();
            IntuityEvents = new HashSet<IntuityEvent>();
            RegisteredUsersforWebinars = new HashSet<RegisteredUsersforWebinar>();
            CarePlanReports = new HashSet<CarePlanReport>();
            Webinars = new HashSet<Webinar>();
            Webinars1 = new HashSet<Webinar>();
            Webinars2 = new HashSet<Webinar>();
            InvoiceBilledDetails = new HashSet<InvoiceBilledDetail>();
            InvoiceDetails = new HashSet<InvoiceDetail>();
            UserTimeTracker = new HashSet<UserTimeTracker>();
            ExternalReports = new HashSet<ExternalReport>();
            UserLoggedInDevices = new HashSet<UserLoggedInDevice>();
            FaxedReports = new HashSet<FaxedReport>();
            FaxedReports1 = new HashSet<FaxedReport>();
            HealthDatas = new HashSet<HealthData>();
            HealthDatas1 = new HashSet<HealthData>();
            UserWearableDevices = new HashSet<UserWearableDevice>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(50)]
        public string Email { get; set; }

        public bool EmailConfirmed { get; set; }

        [StringLength(100)]
        public string? PasswordHash { get; set; }

        [Required]
        [StringLength(100)]
        public string SecurityStamp { get; set; }

        [StringLength(25)]
        public string? PhoneNumber { get; set; }

        public bool PhoneNumberConfirmed { get; set; }

        public bool TwoFactorEnabled { get; set; }

        public DateTime? LockoutEndDateUtc { get; set; }

        public bool LockoutEnabled { get; set; }

        public int AccessFailedCount { get; set; }

        [StringLength(50)]
        public string? NamePrefix { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public int? Race { get; set; }

        [StringLength(50)]
        public string? RaceOther { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Address2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        [StringLength(50)]
        public string? HomeNumber { get; set; }

        [StringLength(50)]
        public string? WorkNumber { get; set; }

        [StringLength(50)]
        public string? CellNumber { get; set; }

        public int? TimeZoneId { get; set; }

        public byte? PreferredContactTimeId { get; set; }

        public int? ProfessionId { get; set; }

        public int OrganizationId { get; set; }

        [StringLength(50)]
        public string? Occupation { get; set; }

        public byte? Source { get; set; }

        [StringLength(50)]
        public string? SourceOther { get; set; }

        public string? ReferralDetails { get; set; }

        public byte? Text { get; set; }

        [StringLength(100)]
        public string? Picture { get; set; }

        public bool? Complete { get; set; }

        public byte? ContactMode { get; set; }

        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? UniqueId { get; set; }

        public int? CreatedBy { get; set; }

        [StringLength(10)]
        public string? LanguagePreference { get; set; }

        public byte? PrimaryCarePhysician { get; set; }

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [StringLength(50)]
        public string? Suffix { get; set; }

        public byte? Unit { get; set; }

        public bool IsActive { get; set; }

        public int? InactiveReason { get; set; }

        [StringLength(50)]
        public string? CouponCode { get; set; }

        public bool? TermsAccepted { get; set; }

        [StringLength(1024)]
        public string? LastVisited { get; set; }

        public int? DeptId { get; set; }

        [StringLength(50)]
        public string? EmployeeId { get; set; }

        [StringLength(50)]
        public string? UnsubscribedEmail { get; set; }

        public virtual AdminProperty AdminProperty { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminTask> AdminTasks { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminTask> AdminTasks1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminTask> AdminTasks2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AdminTask> AdminTasks3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Appointment> Appointments4 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ApptCallTemplate> ApptCallTemplates { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Availability> Availabilities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Availability> Availabilities1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Availability> Availabilities2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]

        public virtual ICollection<AWV> AWVs { get; set; }

        public virtual Country Country1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EligibilityNote> EligibilityNotes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Email> Emails1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExerciseDiary> ExerciseDiaries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<EXT_Glucose> EXT_Glucose { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExternalUser> ExternalUsers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FoodDiary> FoodDiaries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HRA> HRAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HRA> HRAs1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<LabErrorLog> LabErrorLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lab> Labs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lab> Labs1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lab> Labs2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lab> Labs3 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mail> Mails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mail> Mails1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationEvent> NotificationEvents { get; set; }

        public virtual Organization Organization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PaymentTransaction> PaymentTransactions { get; set; }

        public virtual Profession Profession { get; set; }

        public virtual ICollection<Recipe> Recipes { get; set; }

        public virtual Race Race1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SleepLog> SleepLogs { get; set; }

        public virtual State State1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrengthTrainingLog> StrengthTrainingLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StressDiary> StressDiaries { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StressManagementLog> StressManagementLogs { get; set; }

        public virtual TimeZone TimeZone { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TobaccoLog> TobaccoLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TobaccoLog> TobaccoLogs1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<TobaccoLog> TobaccoLogs2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserClaim> UserClaims { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDashboardMessage> UserDashboardMessages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDoctorInfo> UserDoctorInfoes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDrug> UserDrugs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDrug> UserDrugs1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserDrug> UserDrugs2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserHistory> UserHistories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserHistory> UserHistories1 { get; set; }

        public virtual UserInactiveReason UserInactiveReason { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLog> UserLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserLogin> UserLogins { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users1 { get; set; }

        public virtual User User1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersinProgram> UsersinPrograms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersinProgram> UsersinPrograms1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersinProgram> UsersinPrograms2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTrackingStatus> UserTrackingStatuses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VitalsLog> VitalsLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WeightLossJournal> WeightLossJournals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WellnessData> WellnessDatas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WellnessData> WellnessDatas1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<WellnessData> WellnessDatas2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRaffles> UserRaffles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserKeys> UserKeys { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Specialization> Specializations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Languages> Languages { get; set; }

        public virtual IntuityUsers IntuityUsers { get; set; }

        public virtual ICollection<EXT_Nutrition> EXT_Nutrition { get; set; }

        public virtual ICollection<EXT_Summaries> EXT_Summaries { get; set; }

        public virtual ICollection<EXT_BloodPressures> EXT_BloodPressure { get; set; }

        public virtual ICollection<EXT_Workouts> EXT_Workouts { get; set; }

        public virtual ICollection<EXT_Sleeps> EXT_Sleeps { get; set; }

        public virtual ICollection<EXT_Weights> EXT_Weights { get; set; }

        public virtual ICollection<State> CoachStates { get; set; }

        public virtual IList<PreviousPassword> PreviousUserPasswords { get; set; }

        public virtual ICollection<CanriskQuestionnaire> CanriskQuestionnaires { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Message> Messages { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageRecipient> MessageRecipients { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<MessageRecipient> MessageRecipients1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ChangeLog> CRM_ChangeLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Contact> CRM_Contacts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Contact> CRM_Contacts1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes1 { get; set; }

        public virtual Department Department { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEvent> IntuityEvents { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RegisteredUsersforWebinar> RegisteredUsersforWebinars { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CarePlanReport> CarePlanReports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Webinar> Webinars { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Webinar> Webinars1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Webinar> Webinars2 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceBilledDetail> InvoiceBilledDetails { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; }

        public virtual ICollection<UserTimeTracker> UserTimeTracker { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExternalReport> ExternalReports { get; set; }

        public virtual ICollection<UserLoggedInDevice> UserLoggedInDevices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FaxedReport> FaxedReports { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FaxedReport> FaxedReports1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HealthData> HealthDatas { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HealthData> HealthDatas1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserWearableDevice> UserWearableDevices { get; set; }
    }
}
