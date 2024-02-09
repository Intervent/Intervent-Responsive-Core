namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class QuestionsinActivity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuestionsinActivity()
        {
            OptionsforActivityQuestions = new HashSet<OptionsforActivityQuestion>();
            PassiveQuestionsInActivities = new HashSet<PassiveQuestionsInActivity>();
            UserChoices = new HashSet<UserChoice>();
            OptionsforActivityQuestion = new OptionsforActivityQuestion();
        }

        public int Id { get; set; }

        public int ActivityId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuestionText { get; set; }

        public byte QuestionType { get; set; }

        public bool IsActive { get; set; }

        public bool IsRequired { get; set; }

        public short SequenceNo { get; set; }

        public bool ShowVertical { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        public int? ParentId { get; set; }

        public virtual OptionsforActivityQuestion OptionsforActivityQuestion { get; set; }

        public virtual ActivitiesinStep ActivitiesinStep { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptionsforActivityQuestion> OptionsforActivityQuestions { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PassiveQuestionsInActivity> PassiveQuestionsInActivities { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserChoice> UserChoices { get; set; }
    }
}
