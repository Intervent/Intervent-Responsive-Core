namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StrengthTrainingLog")]
    public partial class StrengthTrainingLog
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public StrengthTrainingLog()
        {
            StrengthTrainingSets = new HashSet<StrengthTrainingSet>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        [StringLength(100)]
        public string? MuscleGroup { get; set; }

        public int? TrainingTypeId { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        public virtual StrengthTrainingType StrengthTrainingType { get; set; }

        public virtual User User { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<StrengthTrainingSet> StrengthTrainingSets { get; set; }
    }
}
