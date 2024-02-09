namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("FollowUp")]
    public partial class FollowUp
    {
        public int Id { get; set; }

        public int UsersinProgramsId { get; set; }

        public DateTime StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        [StringLength(50)]
        public string? PageSeqDone { get; set; }

        public int? CreatedBy { get; set; }

        public virtual FollowUp_Goals FollowUp_Goals { get; set; }

        public virtual UsersinProgram UsersinProgram { get; set; }

        public virtual FollowUp_HealthConditions FollowUp_HealthConditions { get; set; }

        public virtual FollowUp_MedicalConditions FollowUp_MedicalConditions { get; set; }

        public virtual FollowUp_HealthNumbers FollowUp_HealthNumbers { get; set; }

        public virtual FollowUp_OtherRiskFactors FollowUp_OtherRiskFactors { get; set; }
    }
}
