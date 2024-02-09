namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SSOProvider
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SSOProvider()
        {
            SSOAttributeMappings = new HashSet<SSOAttributeMapping>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ProviderName { get; set; }

        public int OrganizationId { get; set; }

        public bool HasEligibility { get; set; }

        [StringLength(250)]
        public string? LogoutUrl { get; set; }

        [StringLength(250)]
        public string? RedirectUrl { get; set; }

        [StringLength(100)]
        public string? Issuer { get; set; }

        public bool IsActive { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public string? Certificate { get; set; }

        public virtual Organization Organization { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SSOAttributeMapping> SSOAttributeMappings { get; set; }
    }
}
