namespace Intervent.DAL
{
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;

    public partial class ExerciseType
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public ExerciseType()
        {
            ExerciseDiaries = new HashSet<ExerciseDiary>();
        }

        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Activity { get; set; }

        public int? IsActive { get; set; }

        [StringLength(10)]
        public string? Value { get; set; }

        [StringLength(10)]
        public string? LanguageItem { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ExerciseDiary> ExerciseDiaries { get; set; }
    }
}
