namespace Intervent.Web.DTO
{
    public class UpdateEligibilityDetailsRequest
    {
        public int EligibilityId { get; set; }

        public bool FalseReferral { get; set; }

        public string Language { get; set; }

        public byte? enrollmentStatus { get; set; }

        public byte? declinedEnrollmentReason { get; set; }

        public bool isSecEmail { get; set; }

        public string email2 { get; set; }
    }
}
