using Intervent.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace InterventWebApp.Controllers
{
    public class DrugController : BaseController
    {
        [Authorize]
        public ActionResult MedicationDashboard()
        {
            UserMedications model = new UserMedications();
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            model.userDrugs = DrugUtitlity.ListUserDrug(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).listUserDrugResponse.Select(x => new UserDrugDto
            {
                Id = x.Id,
                UserId = x.UserId,
                MedicationName = textInfo.ToTitleCase(x.MedicationName.ToLower()),
                Dosage = x.Dosage,
                Ingredient = x.Ingredient,
                Quantity = x.Quantity,
                Formulation = x.Formulation,
                Frequency = x.Frequency,
                Condition = x.Condition,
                Status = x.Status,
                AddedOn = x.AddedOn,
                AddedBy = x.AddedBy,
                DiscontinuedOn = x.DiscontinuedOn,
                UpdatedBy = x.UpdatedBy,
                MedicationStartDate = x.MedicationStartDate,
                Duration = x.Duration,
                Notes = x.Notes,
                User = x.User,
                User1 = x.User1,
                User2 = x.User2,
                Drug_Frequency = x.Drug_Frequency,
                Drug_Formulation = x.Drug_Formulation,
                Drug_Condition = x.Drug_Condition,
                Drug_Duration = x.Drug_Duration,
                UserDrugAllergy = x.UserDrugAllergy
            }).ToList();
            model.DrugAllergy = DrugUtitlity.ListDrugAllergy().Schedule.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });

            return View(model);
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public ActionResult AdminMedicationDashboard()
        {
            var listuserdrugs = DrugUtitlity.ListUserDrug(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            UserDrugModel model = new UserDrugModel();
            var user = ParticipantUtility.ReadParticipantInfo(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, Convert.ToInt32(HttpContext.Session.GetInt32(SessionContext.UserId).Value));
            model.Name = user.user.FirstName + " " + user.user.LastName;
            model.ConditionList = DrugUtitlity.ListCondition().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.DurationList = DrugUtitlity.ListDuration().Schedule.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.FrequencyList = DrugUtitlity.ListFrequency().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.FormulationList = DrugUtitlity.ListFormulation().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.DrugAllergy = DrugUtitlity.ListDrugAllergy().Schedule.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            model.drugs = listuserdrugs.listUserDrugResponse.Where(x => !x.DiscontinuedOn.HasValue).Select(x => new ListUserDrugModel
            {
                Id = x.Id,
                medication = textInfo.ToTitleCase(x.MedicationName.ToLower()),
                Dosage = x.Dosage,
                Ingredient = x.Ingredient,
                Quantity = x.Quantity,
                Formulation = x.Drug_Formulation.Formulation,
                Frequency = x.Drug_Frequency.DrugFrequency,
                Duration = x.Drug_Duration.Duration,
                MedicationStartDate = x.MedicationStartDate,
                Condition = x.Drug_Condition.Condition,
                AddedBy = x.User1.FirstName + ' ' + x.User1.LastName,
                AddedOn = x.AddedOn,
                Notes = x.Notes,
                Allergy = x.UserDrugAllergy != null ? x.UserDrugAllergy.Drug_Allergy.AllergyType : "",
                AllergyNotes = x.UserDrugAllergy != null ? x.UserDrugAllergy.Notes : "",
                ImageUrl = x.Drug_Formulation.ImageUrl
            }).ToList();
            model.DiscontinuedDrugs = listuserdrugs.listUserDrugResponse.Where(x => x.DiscontinuedOn.HasValue).Select(x => new ListUserDrugModel
            {
                Id = x.Id,
                medication = textInfo.ToTitleCase(x.MedicationName.ToLower()),
                Dosage = x.Dosage,
                Frequency = x.Drug_Frequency.DrugFrequency,
                Condition = x.Drug_Condition.Condition,
                AddedBy = x.User1.FirstName + ' ' + x.User1.LastName,
                AddedOn = x.AddedOn,
                Notes = x.Notes,
                Allergy = x.UserDrugAllergy != null ? x.UserDrugAllergy.Drug_Allergy.AllergyType : "",
                DiscontinuedOn = x.DiscontinuedOn.Value.ToLongDateString(),
                AllergyNotes = x.UserDrugAllergy != null ? x.UserDrugAllergy.Notes : "",
                ImageUrl = x.Drug_Formulation.ImageUrl
            }).ToList();
            return PartialView(model);
        }

        [Authorize]
        public ActionResult AddMedication(int? id)
        {
            UserDrugModel model = new UserDrugModel();
            model.ConditionList = DrugUtitlity.ListCondition().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.DurationList = DrugUtitlity.ListDuration().Schedule.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.FrequencyList = DrugUtitlity.ListFrequency().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.FormulationList = DrugUtitlity.ListFormulation().Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() });
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            if (id.HasValue)
            {
                var Drug = DrugUtitlity.ReadUserDrug(id.Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).userDrug;
                if (Drug == null)
                    return RedirectToAction("NotAuthorized", "Account");
                model.DrugId = Drug.Id;
                model.Condition = Drug.Condition.ToString();
                model.Duration = Drug.Duration.ToString();
                model.Frequency = Drug.Frequency.ToString();
                model.Formulation = Drug.Formulation.ToString();
                model.MedicationName = textInfo.ToTitleCase(Drug.MedicationName.ToLower());
                model.Ingredient = !string.IsNullOrEmpty(model.Ingredient) ? textInfo.ToTitleCase(Drug.Ingredient.ToLower()) : null;
                model.Dosage = Drug.Dosage;
                model.Quantity = Drug.Quantity;
                model.MedicationStartDate = Drug.MedicationStartDate;
                model.Notes = Drug.Notes;
            }
            return View(model);
        }

        [Authorize]
        public JsonResult SearchDrugData(String search, string form)
        {
            if (String.IsNullOrEmpty(search))
            {
                return Json(new { Result = "Failed" });
            }
            var response = DrugUtitlity.ListDrug(search, form);
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            var Formulation_Types = DrugUtitlity.ListFormulation();
            //var canada = response.DrugsCA.Select(x => new
            //{
            //    label = textInfo.ToTitleCase((x.BRAND_NAME)).ToLower(),
            //    Dosage = textInfo.ToTitleCase(x.Drug_Ingredients.Count > 0 ? x.Drug_Ingredients.FirstOrDefault().Strength + " " + x.Drug_Ingredients.FirstOrDefault().Strength_Unit : "").ToLower(),
            //    Ingredient = textInfo.ToTitleCase(x.Drug_Ingredients.Count > 0 ? x.Drug_Ingredients.FirstOrDefault().Strength + " " + x.Drug_Ingredients.FirstOrDefault().Ingredient : "").ToLower(),
            //    ProductsForm = textInfo.ToTitleCase(x.Drug_Ingredients.Count > 0 ? x.Drug_ProductsForm.FirstOrDefault().PHARMACEUTICAL_FORM : "").ToLower()
            //});
            var USA = response.DrugsUS.Select(x => new
            {
                label = textInfo.ToTitleCase(x.DrugName).ToLower(),
                Dosage = textInfo.ToTitleCase(x.Strength).ToLower(),
                Ingredient = textInfo.ToTitleCase(x.ActiveIngredient).ToLower(),
                ProductsForm = textInfo.ToTitleCase(x.Form).ToLower(),
                FormulationType = Formulation_Types.Where(y => y.Formulation == x.Form).FirstOrDefault().Id
            });
            //var drugList = canada.Concat(USA).ToList();
            return Json(new { Result = "OK", Drugs = USA });
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddUserDrug([FromBody] DrugModel model)
        {
            bool IsDrugAlreadyAdded = false;
            IsDrugAlreadyAdded = DrugUtitlity.ValidateUserDrugAlreadyAdded(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            if (!IsDrugAlreadyAdded)
            {
                var Drug = DrugUtitlity.AddUserDrug(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, Convert.ToInt32(Convert.ToInt32(User.UserId())));
                if (Drug.Succeeded)
                    return Json(new { Result = "success", drugId = Drug.id });
                else
                    return Json(new { Result = "Fail" });
            }
            return Json(new { Result = "Duplicate" });

        }

        [Authorize]
        [HttpPost]
        public JsonResult AddDrugAllergy(int drugId, int allergy, string Comments, bool? remove)
        {
            var Drug = DrugUtitlity.AddDrugAllergy(drugId, allergy, Comments, remove, Convert.ToInt32(Convert.ToInt32(User.UserId())));
            return Json("success");
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ReadUserDrug(int Id)
        {
            var Drug = DrugUtitlity.ReadUserDrug(Id, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;
            return Json(new
            {
                Id = Drug.userDrug.Id,
                MedicationName = textInfo.ToTitleCase(Drug.userDrug.MedicationName.ToLower()),
                Date = Drug.userDrug.MedicationStartDate.ToShortDateString(),
                Dosage = Drug.userDrug.Dosage,
                Ingredient = Drug.userDrug.Ingredient,
                Quantity = Drug.userDrug.Quantity,
                Formulation = Drug.userDrug.Formulation,
                Frequency = Drug.userDrug.Frequency,
                Notes = Drug.userDrug.Notes,
                Condition = Drug.userDrug.Condition,
                Duration = Drug.userDrug.Duration,
                Allergy = Drug.userDrug.UserDrugAllergy
            });
        }

        [Authorize]
        public JsonResult DiscontinueDrug(int Id)
        {
            var response = DrugUtitlity.DiscontinueDrug(Id, Convert.ToInt32(Convert.ToInt32(User.UserId())));
            return Json(new { Result = "OK" });
        }

        [ModuleControl(null, RoleCode.Administrator, RoleCode.Coach, RoleCode.CSR)]
        public JsonResult ShowDrugHistory()
        {
            var listuserdrugs = DrugUtitlity.ShowDrugHistory(HttpContext.Session.GetInt32(SessionContext.AdminId).Value);
            return Json(new
            {
                Result = "OK",
                Records = listuserdrugs.drugHistoryResponse.Select(x => new
                {
                    Name = x.MedicationName,
                    Dosage = x.Dosage,
                    Quantity = x.Quantity,
                    Frequency = x.Drug_Frequency.DrugFrequency,
                    Formulation = x.Drug_Formulation.Formulation,
                    Condition = x.Drug_Condition.Condition,
                    AddedOn = x.AddedOn.ToShortDateString(),
                    UpdatedOn = x.DiscontinuedOn.HasValue ? x.DiscontinuedOn.Value.ToShortDateString() : null
                })
            });
        }
    }
}