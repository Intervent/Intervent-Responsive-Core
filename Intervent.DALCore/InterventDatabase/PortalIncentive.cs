namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PortalIncentive
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PortalIncentive()
        {
            UserIncentives = new HashSet<UserIncentive>();
        }

        public int Id { get; set; }

        public int PortalId { get; set; }

        public int IncentiveTypeId { get; set; }

        public int? RefId { get; set; }

        public double? Points { get; set; }

        public bool IsPoint { get; set; }

        public bool IsCompanyIncentive { get; set; }

        public int? RefValue { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateCreated { get; set; }

        [StringLength(1024)]
        public string? Name { get; set; }

        public int? RefValue2 { get; set; }

        public string? MoreInfo { get; set; }

        [StringLength(100)]
        public string? ImageUrl { get; set; }

        public int? RefValue3 { get; set; }

        [StringLength(250)]
        public string? Attachment { get; set; }

        [StringLength(15)]
        public string? PointsText { get; set; }

        [StringLength(10)]
        public string? LanguageItemName { get; set; }

        [StringLength(10)]
        public string? LanguageItemMoreInfo { get; set; }

        public bool? isGiftCard { get; set; }

        public bool? removeSurcharge { get; set; }

        public virtual IncentiveType IncentiveType { get; set; }

        public virtual Portal Portal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserIncentive> UserIncentives { get; set; }
    }
}
