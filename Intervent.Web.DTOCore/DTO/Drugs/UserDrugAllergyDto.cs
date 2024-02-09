namespace Intervent.Web.DTO
{
    public class UserDrugAllergyDto
    {
        public int UserDrugId { get; set; }

        public int AllergyId { get; set; }

        public string Notes { get; set; }

        public Drug_AllergyDto Drug_Allergy { get; set; }
    }
}