namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Organization
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Organization()
        {
            AssignedRecipes = new HashSet<AssignedRecipe>();
            CouponCodes = new HashSet<CouponCode>();
            Organizations1 = new HashSet<Organization>();
            Portals = new HashSet<Portal>();
            ScreeningDataErrorLogs = new HashSet<ScreeningDataErrorLog>();
            SSOProviders = new HashSet<SSOProvider>();
            Users = new HashSet<User>();
            UserRoles = new HashSet<UserRole>();
            IntuityEligibilities = new HashSet<IntuityEligibility>();
            IntuityEligibilityLogs = new HashSet<IntuityEligibilityLog>();
            CRM_Contacts = new HashSet<CRM_Contact>();
            OrganizationsforWebinars = new HashSet<OrganizationsforWebinar>();
            HCPLists = new HashSet<HCPList>();
            Locations = new HashSet<Location>();
            Providers = new HashSet<Provider>();
            AssignedMotivationMessages = new HashSet<AssignedMotivationMessage>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [StringLength(256)]
        public string? Description { get; set; }

        public int? ParentOrganizationId { get; set; }

        public bool Active { get; set; }

        [StringLength(100)]
        public string? Url { get; set; }

        public bool EmailValidationRequired { get; set; }

        [StringLength(50)]
        public string? ContactNumber { get; set; }

        [StringLength(50)]
        public string? ContactEmail { get; set; }

        public int? LegacyDBOrgId { get; set; }

        public bool? TermsForSSO { get; set; }

        public bool? SSO { get; set; }

        public bool OwnCoach { get; set; }

        public byte? IntegrationWith { get; set; }

        public string? Code { get; set; }

        public string? IntuityEmpUrl { get; set; }

        public string? IntuityEmpToken { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedRecipe> AssignedRecipes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedNewsletter> AssignedNewsletters { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CouponCode> CouponCodes { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Organization> Organizations1 { get; set; }

        public virtual Organization Organization1 { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Portal> Portals { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ScreeningDataErrorLog> ScreeningDataErrorLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SSOProvider> SSOProviders { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<User> Users { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEligibility> IntuityEligibilities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<IntuityEligibilityLog> IntuityEligibilityLogs { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CRM_Contact> CRM_Contacts { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OrganizationsforWebinar> OrganizationsforWebinars { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<HCPList> HCPLists { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Location> Locations { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Provider> Providers { get; set; }

        //public virtual AssignedMotivationMessage AssignedMotivationMessage { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<AssignedMotivationMessage> AssignedMotivationMessages { get; set; }
    }
}
