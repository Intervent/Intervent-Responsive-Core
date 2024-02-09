namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CandidateMedication
    {
        public int Id { get; set; }

        public DateTime SourceDataDate { get; set; }

        public string? MedicationName { get; set; }

        public string? MedicationType { get; set; }

        [Column(TypeName = "date")]
        public DateTime? MedicationDate { get; set; }

        [Column("Total Amount Paid by All Source", TypeName = "money")]
        public decimal? Total_Amount_Paid_by_All_Source { get; set; }

        [Column("Patient Pay Amount", TypeName = "money")]
        public decimal? Patient_Pay_Amount { get; set; }

        [Column("Amount of Copay", TypeName = "money")]
        public decimal? Amount_of_Copay { get; set; }

        [Column("Amount of Coinsurance", TypeName = "money")]
        public decimal? Amount_of_Coinsurance { get; set; }

        [Column("Net Amount Due (Total Amount Billed-Paid)", TypeName = "money")]
        public decimal? Net_Amount_Due__Total_Amount_Billed_Paid_ { get; set; }

        public int? ClaimsID { get; set; }

        public virtual InsuranceSummary InsuranceSummary { get; set; }
    }
}
