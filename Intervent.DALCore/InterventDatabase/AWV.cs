namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("AWV")]
    public partial class AWV
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public AWV()
        {
            AWV_PreventiveServices = new HashSet<AWV_PreventiveServices>();
            AWV_TobaccoAid = new HashSet<AWV_TobaccoAid>();
        }

        public int Id { get; set; }

        public int PortalId { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime AssessmentDate { get; set; }

        public bool? IPPE { get; set; }

        [Column("AWV")]
        public bool? AWV1 { get; set; }

        public bool? SubAWV { get; set; }

        [StringLength(30)]
        public string? AWVPageSeqDone { get; set; }

        public bool GoalsGenerated { get; set; }

        [StringLength(30)]
        public string? ConductedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? LastAssessmentDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MedBEligDate { get; set; }

        [StringLength(20)]
        public string? ReferenceId { get; set; }

        public DateTime? DateCreated { get; set; }

        [StringLength(128)]
        public string? ProviderName { get; set; }

        [StringLength(512)]
        public string? ProviderAddress { get; set; }

        public DateTime? DateUpdated { get; set; }

        [StringLength(64)]
        public string? Token { get; set; }

        [StringLength(2048)]
        public string? DrComments { get; set; }

        public virtual AWV_AlcoholUse AWV_AlcoholUse { get; set; }

        public virtual AWV_Allergies AWV_Allergies { get; set; }

        public virtual AWV_General AWV_General { get; set; }

        public virtual AWV_HospitalVisits AWV_HospitalVisits { get; set; }

        public virtual AWV_Medications AWV_Medications { get; set; }

        public virtual AWV_OsteoporosisScreens AWV_OsteoporosisScreens { get; set; }

        public virtual AWV_Services AWV_Services { get; set; }

        public virtual AWV_STDandProstateRisk AWV_STDandProstateRisk { get; set; }

        public virtual AWV_Biometrics AWV_Biometrics { get; set; }

        public virtual AWV_DepressionScreens AWV_DepressionScreens { get; set; }

        public virtual AWV_Goals AWV_Goals { get; set; }

        public virtual AWV_HomeScreens AWV_HomeScreens { get; set; }

        public virtual AWV_MedicalHistandProviders AWV_MedicalHistandProviders { get; set; }

        public virtual Portal Portal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AWV_PreventiveServices> AWV_PreventiveServices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AWV_TobaccoAid> AWV_TobaccoAid { get; set; }

        public virtual AWV_TobaccoUse AWV_TobaccoUse { get; set; }

        public virtual AWV_UrinaryScreens AWV_UrinaryScreens { get; set; }

        public virtual User User { get; set; }
    }
}
