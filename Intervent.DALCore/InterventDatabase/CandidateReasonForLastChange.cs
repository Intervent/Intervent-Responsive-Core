namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("CandidateReasonForLastChange")]
    public partial class CandidateReasonForLastChange
    {
        public int ID { get; set; }

        [StringLength(50)]
        public string? ConditionType { get; set; }

        public DateTime? ConditionDate { get; set; }

        public int? ClaimsId { get; set; }

        public InsuranceSummary InsuranceSummary { get; set; }
    }
}
