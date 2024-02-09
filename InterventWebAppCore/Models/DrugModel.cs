using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class UserDrugModel
    {
        public int? DrugId { get; set; }

        public string Name { get; set; }

        public IEnumerable<SelectListItem> FrequencyList { get; set; }

        public string Frequency { get; set; }

        public IEnumerable<SelectListItem> FormulationList { get; set; }

        public string Formulation { get; set; }

        public IEnumerable<SelectListItem> ConditionList { get; set; }

        public string Condition { get; set; }

        public IEnumerable<SelectListItem> DurationList { get; set; }

        public string Duration { get; set; }

        public string Schedule { get; set; }

        public List<ListUserDrugModel> drugs { get; set; }

        public List<ListUserDrugModel> DiscontinuedDrugs { get; set; }

        public string MedicationName { get; set; }

        public string Dosage { get; set; }

        public int Quantity { get; set; }

        public DateTime MedicationStartDate { get; set; }

        public string Notes { get; set; }

        public IEnumerable<SelectListItem> DrugAllergy { get; set; }

        public string Allergy { get; set; }

        public string Ingredient { get; set; }

        public string DateFormat { get; set; }

        public bool HasActivePortal { get; set; }

    }
    public class DrugModel
    {
        public int Id { get; set; }

        public string medication { get; set; }

        public string Dosage { get; set; }

        public byte Quantity { get; set; }

        public int Formulation { get; set; }

        public int Frequency { get; set; }

        public int Condition { get; set; }

        public DateTime MedicationStartDate { get; set; }

        public int Duration { get; set; }

        public string Notes { get; set; }

        public string Ingredient { get; set; }
    }

    public class AddReactionModel
    {
        public int Id { get; set; }
        public string Drug { get; set; }
        public string Reaction { get; set; }
    }

    public class UserMedications
    {
        public IList<UserDrugDto> userDrugs { get; set; }

        public IEnumerable<SelectListItem> DrugAllergy { get; set; }

        public string Allergy { get; set; }

        public string LanguagePreference { get; set; }

        public bool HasActivePortal { get; set; }
    }

    public class ListUserDrugModel
    {
        public int Id { get; set; }

        public string medication { get; set; }

        public string Dosage { get; set; }

        public string Ingredient { get; set; }

        public byte Quantity { get; set; }

        public string Formulation { get; set; }

        public string Frequency { get; set; }

        public string Condition { get; set; }

        public string Schedule { get; set; }

        public DateTime MedicationStartDate { get; set; }

        public string Duration { get; set; }

        public string Notes { get; set; }

        public string AddedBy { get; set; }

        public DateTime AddedOn { get; set; }

        public string DiscontinuedOn { get; set; }

        public string Allergy { get; set; }

        public string AllergyNotes { get; set; }

        public string ImageUrl { get; set; }
    }
}