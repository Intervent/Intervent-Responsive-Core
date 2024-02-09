using Intervent.Web.DataLayer;
using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class DrugUtitlity
    {
        public static IList<Drug_FrequencyDto> ListFrequency()
        {
            DrugReader reader = new DrugReader();
            return reader.ListFrequency().Frequency;
        }
        public static IList<Drug_FormulationDto> ListFormulation()
        {
            DrugReader reader = new DrugReader();
            return reader.ListFormulation().Formulation;
        }
        public static IList<Drug_ConditionDto> ListCondition()
        {
            DrugReader reader = new DrugReader();
            return reader.ListCondition().Condition;
        }

        public static ListDrugDurationResponse ListDuration()
        {
            DrugReader reader = new DrugReader();
            return reader.ListDrugDuration();
        }

        public static ListDrugAllergysResponse ListDrugAllergy()
        {
            DrugReader reader = new DrugReader();
            return reader.ListDrugAllergy();
        }
        public static AddDrugResponse AddUserDrug(DrugModel model, int participantId, int userId)
        {
            AddOrEditDrugRequest request = new AddOrEditDrugRequest();
            UserDrugDto drug = new UserDrugDto();
            drug.Id = model.Id;
            drug.MedicationName = model.medication.ToUpper();
            drug.Quantity = model.Quantity;
            drug.Dosage = model.Dosage;
            drug.Formulation = model.Formulation;
            drug.Ingredient = model.Ingredient;
            drug.Frequency = model.Frequency;
            drug.Condition = model.Condition;
            drug.MedicationStartDate = model.MedicationStartDate;
            drug.Notes = model.Notes;
            drug.Duration = model.Duration;
            drug.UserId = participantId;
            drug.AddedBy = userId;
            drug.AddedOn = DateTime.UtcNow;
            request.drug = drug;
            DrugReader reader = new DrugReader();
            var response = reader.AddOrEditUserDrug(request);
            return response;
        }
        public static ListDrugResponse ListDrug(string search, string form)
        {
            DrugReader reader = new DrugReader();
            ListDrugRequest request = new ListDrugRequest();
            request.search = search;
            request.form = form;
            return reader.ListDrug(request);
        }

        public static bool AddDrugAllergy(int drugId, int allergy, string Comments, bool? remove, int userId)
        {
            DiscontinueDrugRequest request = new DiscontinueDrugRequest();
            request.Id = drugId;
            request.DiscontinuedOn = DateTime.UtcNow;
            request.UpdatedBy = userId;
            DrugReader reader = new DrugReader();
            return reader.AddDrugAllergy(drugId, allergy, Comments, remove);
        }

        public static ListUserDrugResponse ListUserDrug(int participantId)
        {
            DrugReader reader = new DrugReader();
            ListUserDrugRequest request = new ListUserDrugRequest();
            request.UserId = participantId;
            return reader.ListUserDrug(request);
        }

        public static bool DiscontinueDrug(int Id, int userId)
        {
            DiscontinueDrugRequest request = new DiscontinueDrugRequest();
            request.UpdatedBy = userId;
            request.Id = Id;
            request.DiscontinuedOn = DateTime.UtcNow;
            DrugReader reader = new DrugReader();
            return reader.DiscontinueDrug(request);
        }

        public static UserDrugWithIdResponse ReadUserDrug(int Id, int participantId)
        {
            ReadUserDrugRequest request = new ReadUserDrugRequest();
            request.drugId = Id;
            request.participantId = participantId;
            DrugReader reader = new DrugReader();
            return reader.ReadUserDrug(request);
        }

        public static DrugHistoryResponse ShowDrugHistory(int adminId)
        {
            DrugReader reader = new DrugReader();
            DrugHistoryRequest request = new DrugHistoryRequest();
            request.UserId = adminId;
            return reader.ShowDrugHistory(request);
        }

        public static bool ValidateUserDrugAlreadyAdded(DrugModel drug, int participantId)
        {
            bool IsDrugAlreadyAdded = false;
            List<UserDrugDto> userMedications = ListUserDrug(participantId).listUserDrugResponse.Where(x => x.DiscontinuedOn == null && (drug.Id == 0 || x.Id != drug.Id)).Select(x => new UserDrugDto
            {
                Id = x.Id,
                UserId = x.UserId,
                MedicationName = x.MedicationName.ToLower(),
                Dosage = x.Dosage.ToLower(),
                Quantity = x.Quantity,
                Formulation = x.Formulation,
                Frequency = x.Frequency,
                DiscontinuedOn = x.DiscontinuedOn

            }).ToList();

            UserDrugDto userDrug = userMedications.Where(x => x.MedicationName.Equals(drug.medication.ToLower())).Where(x => x.Dosage.Equals(drug.Dosage.ToLower())).Where(x => x.Formulation.Equals(drug.Formulation)).FirstOrDefault();
            if (userDrug != null)
            {
                IsDrugAlreadyAdded = true;
            }
            return IsDrugAlreadyAdded;
        }
    }
}