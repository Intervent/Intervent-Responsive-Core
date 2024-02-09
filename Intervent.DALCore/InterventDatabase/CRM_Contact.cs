namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CRM_Contacts")]
    public partial class CRM_Contact
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CRM_Contact()
        {
            CRM_ChangeLogs = new HashSet<CRM_ChangeLog>();
            CRM_PogoMeterNumbers = new HashSet<CRM_PogoMeterNumbers>();
            InsuranceTypes = new HashSet<InsuranceType>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Column(TypeName = "date")]
        public DateTime? DOB { get; set; }

        public byte? Gender { get; set; }

        public int? OrganizationId { get; set; }

        [StringLength(25)]
        public string? PhoneNumber1 { get; set; }

        [StringLength(25)]
        public string? PhoneNumber2 { get; set; }

        [StringLength(25)]
        public string? PhoneNumber3 { get; set; }

        [StringLength(50)]
        public string? Email { get; set; }

        [StringLength(255)]
        public string? Address { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        public int? State { get; set; }

        public int? Country { get; set; }

        [StringLength(15)]
        public string? Zip { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        [StringLength(255)]
        public string? Address2 { get; set; }

        [StringLength(50)]
        public string? City2 { get; set; }

        public int? State2 { get; set; }

        public int? Country2 { get; set; }

        [StringLength(15)]
        public string? Zip2 { get; set; }

        public string? Notes { get; set; }

        public byte OptedIn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_ChangeLog> CRM_ChangeLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Note> CRM_Notes { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual Organization Organization { get; set; }

        public virtual Country Countries { get; set; }

        public virtual State States { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_PogoMeterNumbers> CRM_PogoMeterNumbers { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<InsuranceType> InsuranceTypes { get; set; }

        public string? UniqueId { get; set; }
    }
}
