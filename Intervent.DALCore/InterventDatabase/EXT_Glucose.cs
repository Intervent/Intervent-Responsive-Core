namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class EXT_Glucose
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public EXT_Glucose()
        {
            Ext_GlucoseSetting = new HashSet<Ext_GlucoseSetting>();
            Ext_GlucoseTags = new HashSet<Ext_GlucoseTags>();
        }

        public int Id { get; set; }

        [StringLength(64)]
        public string? UniqueId { get; set; }

        public DateTime? EffectiveDateTime { get; set; }

        public DateTime? DateTime { get; set; }

        public int Value { get; set; }

        [Required]
        [StringLength(16)]
        public string? Code { get; set; }

        [Required]
        [StringLength(8)]
        public string Unit { get; set; }

        public int? OrganizationId { get; set; }

        [StringLength(128)]

        public string? ExtId { get; set; }

        public int? UserId { get; set; }

        public byte? Source { get; set; }

        public bool? IsValid { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ext_GlucoseSetting> Ext_GlucoseSetting { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Ext_GlucoseTags> Ext_GlucoseTags { get; set; }
    }
}
