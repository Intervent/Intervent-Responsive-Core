namespace Intervent.Web.DTO
{
    public class GetEligibilityNotesResponse
    {
        public IList<EligibilityNotesDto> participantEligibilityNotes { get; set; }

        public string EnrollmentStatus { get; set; }

        public string DeclinedEnrollmentReason { get; set; }
    }
}