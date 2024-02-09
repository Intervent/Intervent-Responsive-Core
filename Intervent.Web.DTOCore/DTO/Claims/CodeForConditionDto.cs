namespace Intervent.Web.DTO
{
    public class ClaimConditionCodeDto
    {
        public int Id { get; set; }

        public int? ClaimsID { get; set; }

        public string Condition { get; set; }

        public DateTime? ConditionDate { get; set; }

        public string Code { get; set; }

        public string CodeDescription { get; set; }

        public string LvNDCCode { get; set; }

        public InsuranceSummaryDto InsuranceSummary { get; set; }

    }
}
