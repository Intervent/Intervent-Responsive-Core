namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class KitTopic
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KitTopic()
        {
            Kits = new HashSet<Kit>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(255)]
        public string? Description { get; set; }

        public bool Active { get; set; }

        [StringLength(50)]
        public string? Color { get; set; }


        [StringLength(50)]
        public string? Image { get; set; }

        [StringLength(20)]
        public string? LanguageItem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Kit> Kits { get; set; }
    }
}
