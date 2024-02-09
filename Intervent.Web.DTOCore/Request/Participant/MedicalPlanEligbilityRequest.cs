namespace Intervent.Web.DTO
{
    public class MedicalPlanEligbilityRequest
    {
        public int participantId { get; set; }

        public int portalId { get; set; }

        public bool checkValidCode { get; set; }

        public bool isProgram { get; set; }
    }
}
