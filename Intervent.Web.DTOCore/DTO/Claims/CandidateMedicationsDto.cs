namespace Intervent.Web.DTO
{
    public class CandidateMedicationsDto
    {
        public int Id { get; set; }

        public int CMRef { get; set; }

        public DateTime SourceDataDate { get; set; }

        public string MedicationName { get; set; }

        public string MedicationType { get; set; }

        public DateTime? MedicationDate { get; set; }

        public decimal? Total_Amount_Paid_by_All_Source { get; set; }

        public decimal? Patient_Pay_Amount { get; set; }

        public decimal? Amount_of_Copay { get; set; }

        public decimal? Amount_of_Coinsurance { get; set; }

        public decimal? Net_Amount_Due__Total_Amount_Billed_Paid_ { get; set; }

        public int? ClaimsId { get; set; }

        public virtual InsuranceSummaryDto InsuranceSummary { get; set; }
    }
}
