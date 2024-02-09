namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class StepsinKit
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StepsinKit()
        {
            ActivitiesinSteps = new HashSet<ActivitiesinStep>();
            QuizinSteps = new HashSet<QuizinStep>();
        }

        public int Id { get; set; }

        public int KitId { get; set; }

        [StringLength(100)]
        public string? Name { get; set; }

        [StringLength(10)]
        public string? StepNo { get; set; }

        public string? Text { get; set; }

        public bool IsSubStep { get; set; }

        public bool IsActive { get; set; }

        public bool IsAppendix { get; set; }

        public bool IsGoal { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        [StringLength(64)]
        public string? NameLangItemCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ActivitiesinStep> ActivitiesinSteps { get; set; }

        public virtual Kit Kit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuizinStep> QuizinSteps { get; set; }
    }
}
