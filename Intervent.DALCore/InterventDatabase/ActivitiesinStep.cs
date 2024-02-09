namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ActivitiesinStep
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ActivitiesinStep()
        {
            PassiveQuestionsInActivities = new HashSet<PassiveQuestionsInActivity>();
            QuestionsinActivities = new HashSet<QuestionsinActivity>();
        }

        public int Id { get; set; }

        public int StepId { get; set; }

        public string? TopText { get; set; }

        public string? BottomText { get; set; }

        public bool WithinStep { get; set; }

        public short SequenceNo { get; set; }

        public bool IsActive { get; set; }

        public bool AllowUpdate { get; set; }

        [StringLength(64)]
        public string? BottomTextLangItemCode { get; set; }

        [StringLength(64)]
        public string? TopTextLangItemCode { get; set; }

        public virtual StepsinKit StepsinKit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PassiveQuestionsInActivity> PassiveQuestionsInActivities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<QuestionsinActivity> QuestionsinActivities { get; set; }
    }
}
