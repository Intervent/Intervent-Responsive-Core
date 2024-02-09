namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class UsersinProgram
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public UsersinProgram()
        {
            FollowUps = new HashSet<FollowUp>();
            KitsinUserPrograms = new HashSet<KitsinUserProgram>();
            SurveyResponses = new HashSet<SurveyResponse>();
        }

        public int Id { get; set; }

        public int UserId { get; set; }

        public int ProgramsinPortalsId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public int? CoachId { get; set; }

        public bool IsActive { get; set; }

        public int? InactiveReason { get; set; }

        [Column(TypeName = "date")]
        public DateTime? InactiveDate { get; set; }

        public int? HRAId { get; set; }

        public int? EnrolledBy { get; set; }

        public DateTime? EnrolledOn { get; set; }

        [StringLength(10)]
        public string? Language { get; set; }

        public byte? AssignedFollowUp { get; set; }

        public string? UserComments { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? CompIntroKitsOnTime { get; set; }

        public byte? YearsPaid { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<FollowUp> FollowUps { get; set; }

        public virtual HRA HRA { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<KitsinUserProgram> KitsinUserPrograms { get; set; }

        public virtual ProgramInactiveReason ProgramInactiveReason { get; set; }

        public virtual ProgramsinPortal ProgramsinPortal { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<SurveyResponse> SurveyResponses { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }
    }
}
