namespace Intervent.Web.DTO
{
    public class UserDrugDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string MedicationName { get; set; }
        public string Dosage { get; set; }
        public byte Quantity { get; set; }
        public int Formulation { get; set; }
        public int Frequency { get; set; }
        public int Condition { get; set; }
        public bool Status { get; set; }
        public DateTime AddedOn { get; set; }
        public int AddedBy { get; set; }
        public DateTime? DiscontinuedOn { get; set; }
        public int? UpdatedBy { get; set; }
        public DateTime MedicationStartDate { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
        public string Ingredient { get; set; }
        public UserDto User { get; set; }
        public UserDto User1 { get; set; }
        public UserDto User2 { get; set; }
        public Drug_FrequencyDto Drug_Frequency { get; set; }
        public Drug_FormulationDto Drug_Formulation { get; set; }
        public Drug_ConditionDto Drug_Condition { get; set; }
        public Drug_DurationDto Drug_Duration { get; set; }
        public UserDrugAllergyDto UserDrugAllergy { get; set; }
    }
}