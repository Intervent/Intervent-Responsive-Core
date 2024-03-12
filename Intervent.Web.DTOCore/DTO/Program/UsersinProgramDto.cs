namespace Intervent.Web.DTO
{
    public class UsersinProgramDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int? CoachId { get; set; }

        public int ProgramsinPortalId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? CompleteDate { get; set; }

        public string Language { get; set; }

        public bool IsActive { get; set; }

        public DateTime? InactiveDate { get; set; }

        public int? InactiveReason { get; set; }

        public int? HRAId { get; set; }

        public int? EnrolledBy { get; set; }

        public DateTime EnrolledOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool? CompIntroKitsOnTime { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public ProgramsinPortalDto ProgramsinPortal { get; set; }

        public IList<KitsinUserProgramDto> KitsinUserPrograms { get; set; }

        public ProgramInactiveReasonDto ProgramInactiveReason { get; set; }

        public byte AssignedFollowUp { get; set; }

        public HRADto HRA { get; set; }

        public IList<FollowUpDto> FollowUps { get; set; }
    }
}
