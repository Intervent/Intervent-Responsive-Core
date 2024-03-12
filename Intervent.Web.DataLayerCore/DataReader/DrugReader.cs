using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class DrugReader
    {
        InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());
        public ListFrequencyResponse ListFrequency()
        {
            ListFrequencyResponse response = new ListFrequencyResponse();

            IList<Drug_Frequency> frequency;
            frequency = context.Drug_Frequency.OrderBy(x => x.SortOrder).ToList();

            response.Frequency = Utility.mapper.Map<IList<DAL.Drug_Frequency>, IList<Drug_FrequencyDto>>(frequency);

            return response;
        }
        public ListFormulationResponse ListFormulation()
        {
            ListFormulationResponse response = new ListFormulationResponse();

            IList<Drug_Formulation> formulation;
            formulation = context.Drug_Formulation.ToList();

            response.Formulation = Utility.mapper.Map<IList<DAL.Drug_Formulation>, IList<Drug_FormulationDto>>(formulation);

            return response;
        }

        public ListConditionResponse ListCondition()
        {
            ListConditionResponse response = new ListConditionResponse();

            IList<Drug_Condition> condition;
            condition = context.Drug_Condition.OrderBy(x => x.SortOrder).ThenBy(x => x.Condition).ToList();

            response.Condition = Utility.mapper.Map<IList<DAL.Drug_Condition>, IList<Drug_ConditionDto>>(condition);

            return response;
        }

        public ListDrugDurationResponse ListDrugDuration()
        {
            ListDrugDurationResponse response = new ListDrugDurationResponse();

            IList<Drug_Duration> duration = context.Drug_Duration.ToList();

            response.Schedule = Utility.mapper.Map<IList<DAL.Drug_Duration>, IList<Drug_DurationDto>>(duration);

            return response;
        }

        public ListDrugAllergysResponse ListDrugAllergy()
        {
            ListDrugAllergysResponse response = new ListDrugAllergysResponse();

            IList<Drug_Allergy> allergys = context.Drug_Allergy.ToList();

            response.Schedule = Utility.mapper.Map<IList<DAL.Drug_Allergy>, IList<Drug_AllergyDto>>(allergys);

            return response;
        }

        public AddDrugResponse AddOrEditUserDrug(AddOrEditDrugRequest request)
        {
            AddDrugResponse response = new AddDrugResponse();
            UserDrug drug = null;
            if (request.drug.Id != 0)
                drug = context.UserDrugs.Where(x => x.Id == request.drug.Id).FirstOrDefault();
            else
                drug = context.UserDrugs.Where(x => x.UserId == request.drug.UserId && x.MedicationName == request.drug.MedicationName && x.MedicationStartDate.Date == request.drug.MedicationStartDate.Date).FirstOrDefault();
            if (drug == null)
            {
                UserDrug daldrug = new UserDrug();
                daldrug.UserId = request.drug.UserId;
                daldrug.MedicationName = request.drug.MedicationName;
                daldrug.Ingredient = request.drug.Ingredient;
                daldrug.Dosage = request.drug.Dosage;
                daldrug.Formulation = request.drug.Formulation;
                daldrug.Frequency = request.drug.Frequency;
                daldrug.Condition = request.drug.Condition;
                daldrug.Duration = request.drug.Duration;
                daldrug.MedicationStartDate = request.drug.MedicationStartDate;
                if (request.drug.DiscontinuedOn.HasValue && request.drug.DiscontinuedOn.Value != DateTime.MinValue)
                    daldrug.DiscontinuedOn = request.drug.DiscontinuedOn;
                daldrug.Quantity = request.drug.Quantity;
                daldrug.Notes = request.drug.Notes;
                daldrug.AddedBy = request.drug.AddedBy;
                daldrug.AddedOn = request.drug.AddedOn;
                context.UserDrugs.Add(daldrug);
                context.SaveChanges();
                response.id = daldrug.Id;
            }
            else
            {
                drug.UserId = request.drug.UserId;
                drug.MedicationName = request.drug.MedicationName;
                drug.Ingredient = request.drug.Ingredient;
                drug.Dosage = request.drug.Dosage;
                drug.Formulation = request.drug.Formulation;
                drug.Frequency = request.drug.Frequency;
                drug.Condition = request.drug.Condition;
                drug.Duration = request.drug.Duration;
                drug.MedicationStartDate = request.drug.MedicationStartDate;
                if (request.drug.Duration != 5)
                {
                    var days = context.Drug_Duration.Where(x => x.Id == request.drug.Duration).FirstOrDefault().InDays;
                    if (days.HasValue)
                    {
                        var discontinueDate = request.drug.MedicationStartDate.AddDays(days.Value);
                        if (discontinueDate < DateTime.UtcNow.Date)
                        {
                            drug.DiscontinuedOn = discontinueDate;
                        }
                    }
                }
                drug.Quantity = request.drug.Quantity;
                drug.Notes = request.drug.Notes;
                drug.UpdatedBy = request.drug.AddedBy;
                drug.AddedBy = request.drug.AddedBy;
                drug.AddedOn = request.drug.AddedOn;
                context.UserDrugs.Attach(drug);
                context.Entry(drug).State = EntityState.Modified;
                context.SaveChanges();
            }
            response.Succeeded = true;
            return response;
        }

        public ListUserDrugResponse ListUserDrug(ListUserDrugRequest request)
        {
            ListUserDrugResponse response = new ListUserDrugResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.UserDrugs.Where(x => x.UserId == request.UserId).OrderByDescending(x => x.Id).Count();
            }
            var LabAlertDAL = context.UserDrugs.Where(x => x.UserId == request.UserId).OrderByDescending(x => x.UserId).Include("User").Include("User1")
                               .Include("Drug_Frequency").Include("Drug_Condition").Include("Drug_Formulation").Include("Drug_Duration").Include("UserDrugAllergy").Include("UserDrugAllergy.Drug_Allergy").ToList();

            response.listUserDrugResponse = Utility.mapper.Map<IList<DAL.UserDrug>, IList<UserDrugDto>>(LabAlertDAL);
            response.TotalRecords = totalRecords;
            return response;
        }

        public ListDrugResponse ListDrug(ListDrugRequest request)
        {
            ListDrugResponse response = new ListDrugResponse();
            var drugs = context.Drug_Products_FDA.Where(x => (x.DrugName.StartsWith(request.search) || x.ActiveIngredient.StartsWith(request.search)) && (string.IsNullOrEmpty(request.form) || request.form == "All Forms" || x.Form == request.form)).OrderBy(x => x.ActiveIngredient.Length).ToList();
            response.DrugsUS = Utility.mapper.Map<IList<DAL.Drug_Products_FDA>, IList<Drug_Products_FDADto>>(drugs);
            var CADrugDAL = context.Drug_Products.Include("Drug_ProductsForm").Include("Drug_Ingredients").Where(x => x.BRAND_NAME.StartsWith(request.search) || (x.Drug_Ingredients.Where(y => y.Ingredient.StartsWith(request.search)).Count() > 0)).OrderByDescending(x => x.BRAND_NAME).ToList();
            response.DrugsCA = Utility.mapper.Map<IList<DAL.Drug_Products>, IList<Drug_ProductDto>>(CADrugDAL);
            return response;
        }

        public bool DiscontinueDrug(DiscontinueDrugRequest request)
        {
            var drug = context.UserDrugs.Where(x => x.Id == request.Id).FirstOrDefault();
            if (drug != null)
            {
                drug.DiscontinuedOn = request.DiscontinuedOn;
                drug.UpdatedBy = request.UpdatedBy;
                context.UserDrugs.Attach(drug);
                context.Entry(drug).State = EntityState.Modified;
                context.SaveChanges();
                return true;
            }
            return false;

        }

        public bool AddDrugAllergy(int drugId, int allergy, string Comments, bool? remove)
        {
            var drugAllergy = context.UserDrugAllergies.Where(x => x.UserDrugId == drugId).FirstOrDefault();
            if (remove.HasValue && remove.Value)
            {
                if (drugAllergy != null)
                {
                    context.UserDrugAllergies.Remove(drugAllergy);
                    return true;
                }
            }
            if (drugAllergy == null)
            {
                DAL.UserDrugAllergy drug_Allergy = new DAL.UserDrugAllergy();
                drug_Allergy.UserDrugId = drugId;
                drug_Allergy.AllergyId = allergy;
                drug_Allergy.Notes = Comments;
                context.UserDrugAllergies.Add(drug_Allergy);
            }
            else
            {
                drugAllergy.AllergyId = allergy;
                drugAllergy.Notes = Comments;
                context.Entry(drugAllergy).State = EntityState.Modified;
            }
            context.SaveChanges();
            return true;
        }

        public DrugHistoryResponse ShowDrugHistory(DrugHistoryRequest request)
        {
            DrugHistoryResponse response = new DrugHistoryResponse();
            var totalRecords = request.TotalRecords.HasValue ? request.TotalRecords.Value : 0;
            if (totalRecords == 0)
            {
                totalRecords = context.UserDrugs.Where(x => x.UserId == request.UserId).Count();
            }
            var LabAlertDAL = context.UserDrugs.Where(x => x.UserId == request.UserId).OrderBy(x => x.MedicationName)
                               .Include("Drug_Condition").Include("Drug_Formulation").Include("Drug_Frequency").ToList();

            response.drugHistoryResponse = Utility.mapper.Map<IList<DAL.UserDrug>, IList<UserDrugDto>>(LabAlertDAL);
            response.TotalRecords = totalRecords;
            return response;
        }

        public UserDrugWithIdResponse ReadUserDrug(ReadUserDrugRequest request)
        {
            UserDrugWithIdResponse response = new UserDrugWithIdResponse();
            var drug = context.UserDrugs.Where(x => x.Id == request.drugId && x.UserId == request.participantId).Include("UserDrugAllergy").FirstOrDefault();
            response.userDrug = Utility.mapper.Map<DAL.UserDrug, UserDrugDto>(drug);
            return response;
        }
    }
}