namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("PromptsinKitsCompleted")]
    public partial class PromptsinKitsCompleted
    {
        public int Id { get; set; }

        public int KitsInUserProgramId { get; set; }

        public int PromptId { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool? IsActive { get; set; }

        public virtual KitsinUserProgram KitsinUserProgram { get; set; }

        public virtual PromptsInKit PromptsInKit { get; set; }
    }
}
