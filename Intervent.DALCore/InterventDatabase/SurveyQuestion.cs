namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class SurveyQuestion
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public SurveyQuestion()
        {
            SurveyResponses = new HashSet<SurveyResponse>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string QuestionLanguageId { get; set; }

        public bool IsActive { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveyResponse> SurveyResponses { get; set; }
    }
}
