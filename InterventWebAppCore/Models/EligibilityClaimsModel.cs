namespace InterventWebApp.Models
{
    public class EligibilityClaimsModel
    {
        public IEnumerable<ClaimsMedicationModel> ClaimsMedications { get; set; }

        public IEnumerable<ClaimsConditionCostModel> ClaimsConditionCosts { get; set; }

        public IEnumerable<ClaimsConditionModel> ClaimsConditions { get; set; }
    }

    public class ClaimsMedicationModel
    {
        public string MedicationName { get; set; }

        public string MedicationDate { get; set; }

        public string MedicationType { get; set; }

        public string TotalAmountPaid { get; set; }
    }

    public class ClaimsConditionCostModel
    {
        public string ConditionName { get; set; }

        public string ConditionType { get; set; }

        public string ConditionDate { get; set; }

        public string BilledAmount { get; set; }
    }

    public class ClaimsConditionModel
    {
        public string ConditionType { get; set; }

        public string ConditionDate { get; set; }

        public string ConditionCodes { get; set; }

        public string RecentConditionDate { get; set; }
    }
}