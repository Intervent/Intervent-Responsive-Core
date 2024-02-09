namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class QuizinStep
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public QuizinStep()
        {
            OptionsforQuizs = new HashSet<OptionsforQuiz>();
            UserQuizChoices = new HashSet<UserQuizChoice>();
        }

        public int Id { get; set; }

        public int StepId { get; set; }

        [Required]
        [StringLength(500)]
        public string QuizText { get; set; }

        public byte QuizType { get; set; }

        public bool IsActive { get; set; }

        [StringLength(64)]
        public string? TextLangItemCode { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<OptionsforQuiz> OptionsforQuizs { get; set; }

        public virtual StepsinKit StepsinKit { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserQuizChoice> UserQuizChoices { get; set; }
    }
}
