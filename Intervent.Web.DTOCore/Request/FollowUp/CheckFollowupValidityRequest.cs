namespace Intervent.Web.DTO
{
    public class CheckFollowupValidityRequest
    {
        public int usersInProgramsId { get; set; }

        public LabDto lab { get; set; }

        public DateTime? bloodTestDate { get; set; }
    }
}
