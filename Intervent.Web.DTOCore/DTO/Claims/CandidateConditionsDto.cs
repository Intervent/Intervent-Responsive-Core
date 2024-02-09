namespace Intervent.Web.DTO
{
    public class CandidateConditionsDto
    {
        public string LastReasonForChange { get; set; }

        public int Id { get; set; }

        public DateTime SourceDataDate { get; set; }

        public string ConditionName { get; set; }

        public string ConditionType { get; set; }

        public DateTime? ConditionDate { get; set; }

        public decimal? BilledAmount { get; set; }

        public decimal? Copay { get; set; }

        public decimal? Deductible { get; set; }

        public decimal? Coinsurance { get; set; }

        public decimal? NetPaid { get; set; }

        public int? ClaimsID { get; set; }
    }
}
