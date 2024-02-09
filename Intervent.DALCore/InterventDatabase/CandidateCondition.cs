namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CandidateCondition
    {
        public int Id { get; set; }

        public DateTime SourceDataDate { get; set; }

        public string? ConditionName { get; set; }

        public string? ConditionType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ConditionDate { get; set; }

        [Column(TypeName = "money")]
        public decimal? BilledAmount { get; set; }

        [Column(TypeName = "money")]
        public decimal? Copay { get; set; }

        [Column(TypeName = "money")]
        public decimal? Deductible { get; set; }

        [Column(TypeName = "money")]
        public decimal? Coinsurance { get; set; }

        [Column(TypeName = "money")]
        public decimal? NetPaid { get; set; }

        public int? ClaimsID { get; set; }

        public virtual InsuranceSummary InsuranceSummary { get; set; }
    }
}
