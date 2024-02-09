namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("KitsinUserProgram")]
    public partial class KitsinUserProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public KitsinUserProgram()
        {
            PromptsinKitsCompleteds = new HashSet<PromptsinKitsCompleted>();
            UserQuizChoices = new HashSet<UserQuizChoice>();
            UserChoices = new HashSet<UserChoice>();
            KitsinUserProgramGoals = new HashSet<KitsinUserProgramGoal>();
        }

        public int Id { get; set; }

        public int UsersinProgramsId { get; set; }

        public int KitId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public int PercentCompleted { get; set; }

        public bool IsActive { get; set; }

        public bool ListenedAudio { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual Kit Kit { get; set; }

        public virtual UsersinProgram UsersinProgram { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PromptsinKitsCompleted> PromptsinKitsCompleteds { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserQuizChoice> UserQuizChoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<UserChoice> UserChoices { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinUserProgramGoal> KitsinUserProgramGoals { get; set; }
    }
}
