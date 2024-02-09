namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IntuityEligibilityLog")]
    public partial class IntuityEligibilityLog
    {

        public int Id { get; set; }

        [StringLength(128)]

        public string? FirstName { get; set; }

        [StringLength(128)]
        public string? LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        [StringLength(128)]
        public string? email { get; set; }

        [StringLength(25)]
        public string? PhoneNumber { get; set; }

        [StringLength(255)]
        public string? AddressLine1 { get; set; }

        [StringLength(255)]
        public string? AddressLine2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        public byte? Gender { get; set; }

        [StringLength(128)]
        public string? UniqueId { get; set; }

        [StringLength(64)]
        public string? OrganizationCode { get; set; }

        public int? OrganizationId { get; set; }

        public int? EligibilityId { get; set; }

        public byte? APIStatus { get; set; }

        public int? HRAId { get; set; }

        public DateTime? DateCreated { get; set; }

        public byte? EligibilityReason { get; set; }

        public byte? TakeDiabetesMed { get; set; }

        public byte? TakeInsulin { get; set; }

        public byte? HadA1CTest { get; set; }

        public float? A1CValue { get; set; }

        public byte? DiabetesType { get; set; }

        public byte? HasDiabetes { get; set; }

        public byte? HasPreDiabetes { get; set; }

        public byte? EligibilityStatus { get; set; }

        public string? DiabetesDate { get; set; }

        public string? A1CTestDate { get; set; }

        public float? Height { get; set; }

        public float? Weight { get; set; }

        public byte? NoA1cTestReason { get; set; }

        public virtual Eligibility Eligibility { get; set; }

        public virtual HRA HRA { get; set; }

        public virtual Organization Organization { get; set; }

        public DateTime? OptingOut { get; set; }

        public DateTime? PairedDate { get; set; }

        public DateTime? PatternsRegDate { get; set; }

        public int? CreatedBy { get; set; }

        public string? Devices { get; set; }
    }
}
