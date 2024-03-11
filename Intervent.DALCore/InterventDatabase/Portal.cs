namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class Portal
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Portal()
        {
            AWVs = new HashSet<AWV>();
            Eligibilities = new HashSet<Eligibility>();
            HRAs = new HashSet<HRA>();
            Labs = new HashSet<Lab>();
            Notes = new HashSet<Note>();
            PortalIncentives = new HashSet<PortalIncentive>();
            ProgramsinPortals = new HashSet<ProgramsinPortal>();
            UserTrackingStatuses = new HashSet<UserTrackingStatus>();
            NotificationEventTypes = new HashSet<NotificationEventType>();
            Kits = new HashSet<Kit>();
            Languages = new HashSet<Languages>();
            Specializations = new HashSet<Specialization>();
            PortalFollowUps = new HashSet<PortalFollowUp>();
            PortalCoachingConditions = new HashSet<PortalCoachingConditions>();
            PortalLabProcedures = new HashSet<PortalLabProcedure>();
        }

        public int OrganizationId { get; set; }

        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Column(TypeName = "date")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime EndDate { get; set; }

        public byte? HasHRA { get; set; }

        [StringLength(30)]
        public string? HAPageSeq { get; set; }

        public bool HasCoachingProgram { get; set; }

        public bool? HRAforCoachingProgram { get; set; }

        public bool HasSelfHelpProgram { get; set; }

        public bool? HRAforSelfHelpProgram { get; set; }

        public bool Active { get; set; }

        public bool AWV { get; set; }

        public bool LabIntegration { get; set; }

        public bool AppointmentCalls { get; set; }

        public bool CampaignCalls { get; set; }

        public bool CACScanQuestion { get; set; }

        public bool GINAQuestion { get; set; }

        public int? HRAValidity { get; set; }

        public bool? FollowUpforCoaching { get; set; }

        public bool? FollowUpforSelfHelp { get; set; }

        public bool? ShowProgramOption { get; set; }

        public bool? ClientNameInReport { get; set; }

        public bool TrackingCalls { get; set; }

        public byte? NoOfTrackingCalls { get; set; }

        public bool AutoImmune { get; set; }

        public bool MailScoreCard { get; set; }

        public bool CarePlan { get; set; }

        public bool? ShowPricing { get; set; }

        public int? NoProgDays { get; set; }

        [StringLength(1024)]
        public string? IncompleteHRA { get; set; }

        [StringLength(1024)]
        public string? HRAOnly { get; set; }

        [StringLength(1024)]
        public string? HRAandBiometrics { get; set; }

        [StringLength(1024)]
        public string? FirstCoaching { get; set; }

        [StringLength(1024)]
        public string? CompletedCoaching { get; set; }

        [StringLength(1024)]
        public string? FirsttimeHRA { get; set; }

        [StringLength(256)]
        public string? LabCorpAttachment { get; set; }

        [StringLength(256)]
        public string? DoctorOfficeAttachment { get; set; }

        public bool? SelfScheduling { get; set; }

        [StringLength(256)]
        public string? ReportText { get; set; }

        [StringLength(256)]
        public string? ProgramText { get; set; }

        [StringLength(256)]
        public string? ContactText { get; set; }

        [StringLength(256)]
        public string? EmployerIncentiveText { get; set; }

        public byte? HRAVer { get; set; }

        public byte? EligibilityFormat { get; set; }

        public byte? CarePlanType { get; set; }

        [StringLength(400)]
        public string? EligibilityFolderPath { get; set; }

        public bool? EligibilityImportLoadFlag { get; set; }

        public bool? EligibilityTerminateFlag { get; set; }

        public bool ProcessInterventCodes { get; set; }

        public bool ProcessLivongoCodes { get; set; }

        [StringLength(100)]
        public string? ClaimsDirectoryPath { get; set; }

        public bool usePreviousLabs { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ValidLabs { get; set; }

        public string? PatientReleaseForm { get; set; }

        public string? MedicalClearanceForm { get; set; }

        public string? KnowYourNumbersForm { get; set; }

        public string? TestimonialForm { get; set; }

        public string? TobaccoReleaseForm { get; set; }

        public bool AssignPrograms { get; set; }

        public bool AllowCardiacQuestion { get; set; }

        public byte? EligtoIntuity { get; set; }

        public string? WelcomeText { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AWV> AWVs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Eligibility> Eligibilities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HRA> HRAs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Lab> Labs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Note> Notes { get; set; }

        public virtual Organization Organization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalIncentive> PortalIncentives { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ProgramsinPortal> ProgramsinPortals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserTrackingStatus> UserTrackingStatuses { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<NotificationEventType> NotificationEventTypes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<RafflesinPortals> RafflesinPortals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserKeys> UserKeys { get; set; }

        public virtual ICollection<Kit> Kits { get; set; }

        public virtual ICollection<Languages> Languages { get; set; }

        public virtual ICollection<Specialization> Specializations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalFollowUp> PortalFollowUps { get; set; }

        public int? FollowUpValidity { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CampaignStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CampaignEndDate { get; set; }

        public bool ShowPostmenopausal { get; set; }

        public bool NeedCareplanApproval { get; set; }

        public string? CareplanPath { get; set; }

        public bool? ShowTimeTracker { get; set; }

        public byte ProviderDetails { get; set; }

        public bool SelfEnterLabs { get; set; }

        public bool FaxReports { get; set; }

        public bool KitAlert { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalCoachingConditions> PortalCoachingConditions { get; set; }


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PortalLabProcedure> PortalLabProcedures { get; set; }
    }
}
