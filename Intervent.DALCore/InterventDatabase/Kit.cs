namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class Kit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Kit()
        {
            KitsinPrograms = new HashSet<KitsinProgram>();
            KitsinUserPrograms = new HashSet<KitsinUserProgram>();
            PromptsInKits = new HashSet<PromptsInKit>();
            QuizinKits = new HashSet<QuizinKit>();
            StepsinKits = new HashSet<StepsinKit>();
            KitsinPortalFollowUps = new HashSet<KitsinPortalFollowUp>();
        }

        public int Id { get; set; }

        [StringLength(50)]
        public string? InvId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(255)]
        public string Description { get; set; }

        public int Topic { get; set; }

        public string? KeyConcepts { get; set; }

        [StringLength(250)]
        public string? Pdf { get; set; }

        [StringLength(50)]
        public string? Audio { get; set; }

        public bool Active { get; set; }

        public virtual KitTopic KitTopic { get; set; }

        public DateTime? PublishedDate { get; set; }

        public DateTime? LastUpdated { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinProgram> KitsinPrograms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinUserProgram> KitsinUserPrograms { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromptsInKit> PromptsInKits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuizinKit> QuizinKits { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StepsinKit> StepsinKits { get; set; }

        public virtual ICollection<KitTranslation> KitTranslations { get; set; }

        public virtual ICollection<Portal> Portal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinPortalFollowUp> KitsinPortalFollowUps { get; set; }
    }
}
