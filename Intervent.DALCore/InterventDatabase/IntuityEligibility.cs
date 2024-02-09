namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IntuityEligibility")]
    public partial class IntuityEligibility
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public IntuityEligibility()
        {
            IntuityFulfillments = new HashSet<IntuityFulfillments>();
            IntuityFulfillmentRequests = new HashSet<IntuityFulfillmentRequests>();
            IntuityQOHs = new HashSet<IntuityQOH>();
            IntuityEPDatas = new HashSet<IntuityEPData>();
        }

        public int Id { get; set; }

        public byte EligibilityStatus { get; set; }

        public byte? EligibilityReason { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime? DateUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public bool? OverrideStatus { get; set; }

        [Required]
        [StringLength(128)]
        public string UniqueId { get; set; }

        public int OrganizationId { get; set; }

        [StringLength(512)]
        public string? SerialNumber { get; set; }

        public string? FirstName { get; set; }

        [StringLength(128)]
        public string? LastName { get; set; }

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

        public DateTime? OptingOut { get; set; }

        public virtual Organization Organization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityFulfillments> IntuityFulfillments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityFulfillmentRequests> IntuityFulfillmentRequests { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityQOH> IntuityQOHs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEPData> IntuityEPDatas { get; set; }
    }
}
