namespace Intervent.Web.DTO
{
    public class CheckTobaccoUserRequest
    {
        public int participantId { get; set; }

        public int portalId { get; set; }

        public bool checkEligibility { get; set; }

    }
}
