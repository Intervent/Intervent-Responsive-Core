namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class HRA
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HRA()
        {
            HRA_ActionSteps = new HashSet<HRA_ActionSteps>();
            UsersinPrograms = new HashSet<UsersinProgram>();
            IntuityEligibilityLogs = new HashSet<IntuityEligibilityLog>();
            ReportFeedbacks = new HashSet<ReportFeedback>();
        }

        public int Id { get; set; }

        public int PortalId { get; set; }

        public int UserId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        [StringLength(30)]
        public string? HAPageSeqDone { get; set; }

        public bool GoalsGenerated { get; set; }

        [StringLength(100)]
        public string? RiskCode { get; set; }

        public float? UOMRisk { get; set; }

        public float? IVRisk { get; set; }

        public float? WellnessScore { get; set; }

        public int? Age { get; set; }

        [StringLength(5)]
        public string? Language { get; set; }

        public int? CreatedBy { get; set; }

        public int? ADAScore { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public double? PAISummary { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HRA_ActionSteps> HRA_ActionSteps { get; set; }

        public virtual HRA_ExamsandShots HRA_ExamsandShots { get; set; }

        public virtual HRA_Goals HRA_Goals { get; set; }

        public virtual HRA_HealthNumbers HRA_HealthNumbers { get; set; }

        public virtual HRA_HSP HRA_HSP { get; set; }

        public virtual HRA_Interests HRA_Interests { get; set; }

        public virtual HRA_MedicalConditions HRA_MedicalConditions { get; set; }

        public virtual HRA_OtherRiskFactors HRA_OtherRiskFactors { get; set; }

        public virtual User User { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual User User1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UsersinProgram> UsersinPrograms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEligibilityLog> IntuityEligibilityLogs { get; set; }

        public virtual ICollection<ReportFeedback> ReportFeedbacks { get; set; }
    }
}
