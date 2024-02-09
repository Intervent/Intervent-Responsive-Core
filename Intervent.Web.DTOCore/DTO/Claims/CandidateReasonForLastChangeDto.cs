namespace Intervent.Web.DTO
{
    public class CandidateReasonForLastChangeDto
    {
        public string ConditionType { get; set; }

        public DateTime? ConditionDate { get; set; }

        public int ClaimsId { get; set; }

        public string ConditionCode { get; set; }

        public DateTime? RecentConditionDate { get; set; }
    }
}
