namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HCPLists")]
    public partial class HCPList
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public HCPList()
        {
            CRM_Notes = new HashSet<CRM_Note>();
        }
        public int Id { get; set; }

        public int? OrganizationId { get; set; }

        public bool UserAdded { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        public string? AddressLine1 { get; set; }

        public string? AddressLine2 { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        [StringLength(50)]
        public string? ZipCode { get; set; }

        [StringLength(50)]
        public string? PhoneNumber { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(50)]
        public string? NPINumber { get; set; }

        [StringLength(50)]
        public string? SpecialtyCode { get; set; }

        public int? SpecialtyId { get; set; }

        public virtual HCPSpecialty HCPSpecialties { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual State State1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes { get; set; }
    }
}
