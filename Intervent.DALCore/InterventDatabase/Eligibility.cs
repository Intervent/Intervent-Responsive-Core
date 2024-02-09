namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Eligibility")]
    public partial class Eligibility
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Eligibility()
        {
            IntuityEligibilityLogs = new HashSet<IntuityEligibilityLog>();
        }
        public int Id { get; set; }

        [StringLength(50)]
        public string? NamePrefix { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? HomeNumber { get; set; }

        [StringLength(50)]
        public string? WorkNumber { get; set; }

        [StringLength(50)]
        public string? CellNumber { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(255)]
        public string? Address2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(50)]
        public string? State { get; set; }

        [StringLength(50)]
        public string? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        [StringLength(15)]
        public string? SSN { get; set; }

        [StringLength(2)]
        public string? UserEnrollmentType { get; set; }

        [StringLength(50)]
        public string? EmployeeUniqueId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? HireDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? TerminatedDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DeathDate { get; set; }

        [StringLength(100)]
        public string? BusinessUnit { get; set; }

        [StringLength(100)]
        public string? RegionCode { get; set; }

        [StringLength(1)]
        public string? UnionFlag { get; set; }

        [StringLength(50)]
        public string? MedicalPlanCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MedicalPlanStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MedicalPlanEndDate { get; set; }

        [StringLength(1)]
        public string? TobaccoFlag { get; set; }

        [StringLength(1)]
        public string? EducationalAssociates { get; set; }

        [StringLength(1)]
        public string? PayType { get; set; }

        [StringLength(2)]
        public string? PayrollArea { get; set; }

        [StringLength(15)]
        public string? UserStatus { get; set; }

        [Required]
        [StringLength(100)]
        public string UniqueId { get; set; }

        public int PortalId { get; set; }

        public byte? EnrollmentStatus { get; set; }

        public byte? DeclinedEnrollmentReason { get; set; }

        public bool? IsFalseReferral { get; set; }

        [StringLength(50)]
        public string? DentalPlanCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DentalPlanStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DentalPlanEndDate { get; set; }

        [StringLength(50)]
        public string? VisionPlanCode { get; set; }

        [Column(TypeName = "date")]
        public DateTime? VisionPlanStartDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? VisionPlanEndDate { get; set; }

        [StringLength(50)]
        public string? UpdatedBy { get; set; }

        public DateTime? UpdateDate { get; set; }

        public bool IsActive { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreateDate { get; set; }

        [StringLength(50)]
        public string? Ref_LastName { get; set; }

        [StringLength(50)]
        public string? Ref_FirstName { get; set; }

        [StringLength(10)]
        public string? Ref_PractNum { get; set; }

        [StringLength(255)]
        public string? Ref_OfficeName { get; set; }

        [StringLength(50)]
        public string? Ref_City { get; set; }

        [StringLength(10)]
        public string? Ref_StateOrProvince { get; set; }

        [StringLength(50)]
        public string? Ref_Phone { get; set; }

        [StringLength(50)]
        public string? Ref_Fax { get; set; }

        public byte? DiabetesType { get; set; }

        public bool? CoachingEnabled { get; set; }

        public DateTime? CoachingExpirationDate { get; set; }

        [StringLength(50)]
        public string? Email2 { get; set; }

        [Column(TypeName = "date")]
        public DateTime? Lab_Date { get; set; }

        public byte? Lab_DidYouFast { get; set; }

        public float? Lab_A1C { get; set; }

        public float? Lab_Glucose { get; set; }

        public virtual Portal Portal { get; set; }

        public DateTime? FirstEligibleDate { get; set; }

        public string? Ref_Address1 { get; set; }

        public string? Ref_Address2 { get; set; }

        public string? Ref_Zip { get; set; }

        public string? Ref_Country { get; set; }

        public string? Ref_Phone2 { get; set; }

        public string? Ref_Email { get; set; }

        public virtual ICollection<CanriskQuestionnaire> CanriskQuestionnaire { get; set; }

        public virtual ICollection<CanriskTracking> CanriskTracking { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEligibilityLog> IntuityEligibilityLogs { get; set; }

        [StringLength(50)]
        public string? Race { get; set; }

        public string? TerminationReason { get; set; }

        public DateTime? EnrollmentStatusDate { get; set; }

        public virtual DeclinedEnrollmentReason DeclinedEnrollmentReason1 { get; set; }
    }
}
