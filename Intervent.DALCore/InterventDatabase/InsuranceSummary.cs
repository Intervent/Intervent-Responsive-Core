namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("InsuranceSummary")]
    public partial class InsuranceSummary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public InsuranceSummary()
        {
            CandidateConditions = new HashSet<CandidateCondition>();
            CandidateMedications = new HashSet<CandidateMedication>();
            CandidateReasonForLastChanges = new HashSet<CandidateReasonForLastChange>();
        }

        public int ID { get; set; }

        public DateTime? EarliestServiceDate { get; set; }

        public bool Eligible { get; set; }

        public bool HRA { get; set; }

        [StringLength(3)]
        public string? EnrollType { get; set; }

        [StringLength(30)]
        public string? DataSrc { get; set; }

        public DateTime? CreateDate { get; set; }

        public DateTime? LastModifiedDate { get; set; }

        public int ConditionCount { get; set; }

        public bool Inactive { get; set; }

        [StringLength(50)]
        public string? UniqueID { get; set; }

        public int? OrganizationId { get; set; }

        public bool Spouse { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateCondition> CandidateConditions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CandidateMedication> CandidateMedications { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ClaimConditionCode> ClaimConditionCodes { get; set; }

        public virtual ICollection<CandidateReasonForLastChange> CandidateReasonForLastChanges { get; set; }
    }
}
