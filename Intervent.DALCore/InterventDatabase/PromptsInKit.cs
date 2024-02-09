namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class PromptsInKit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public PromptsInKit()
        {
            PromptsinKitsCompleteds = new HashSet<PromptsinKitsCompleted>();
        }

        public int Id { get; set; }

        public int KitId { get; set; }

        [Required]
        public string Description { get; set; }

        public bool IsBottom { get; set; }

        public int RefId { get; set; }

        public bool IsActive { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? UpdatedBy { get; set; }

        public int? RefType { get; set; }

        public byte? DisplayOrder { get; set; }

        public virtual Kit Kit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromptsinKitsCompleted> PromptsinKitsCompleteds { get; set; }
    }
}
