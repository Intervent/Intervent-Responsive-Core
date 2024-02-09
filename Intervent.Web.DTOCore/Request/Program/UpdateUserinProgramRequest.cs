namespace Intervent.Web.DTO
{
    public class UpdateUserinProgramRequest
    {
        public int UsersinProgramId { get; set; }

        public int PrograminPortalId { get; set; }

        public int PortalId { get; set; }

        public string Language { get; set; }

        public int? CoachId { get; set; }

        public int? LoginId { get; set; }

        public byte? InactiveReasonId { get; set; }

        public bool AssignedFollowup { get; set; }

        public bool UpdateSubscriptionRenewal { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }
    }
}
