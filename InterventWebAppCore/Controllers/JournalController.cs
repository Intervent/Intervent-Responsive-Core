using Intervent.DAL;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Globalization;

namespace InterventWebApp
{
    public class JournalController : BaseController
    {
        [Authorize]
        public ActionResult Journals()
        {
            JournalModel model = new JournalModel();
            //Assign access status
            var KitsinUserProgram = KitUtility.ListAssignedKits(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).KitsinUserPrograms;
            if (KitsinUserProgram != null)
            {
                model.ExerciseDiaryAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("N17") || x.Kit.InvId.Equals("N18") || x.Kit.InvId.Equals("N17_IVWM") || x.Kit.InvId.Equals("N18_IVWM"))).Count() > 0)
                    model.BoosterJournalAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("N2") || x.Kit.InvId.Equals("N4A") || x.Kit.InvId.Equals("N4B")
                     || x.Kit.InvId.Equals("N15") || x.Kit.InvId.Equals("P21") || x.Kit.InvId.Equals("N2_CA") || x.Kit.InvId.Equals("N4A_CA") || x.Kit.InvId.Equals("N5_CA")
                     || x.Kit.InvId.Equals("N15_CA"))).Count() > 0)
                    model.FoodDiaryAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("E9"))).Count() > 0)
                    model.StrengthTrainingLogAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("P1") || x.Kit.InvId.Equals("P1A") || x.Kit.InvId.Equals("P1_CA"))).Count() > 0)
                    model.TobaccoLogAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("S1") || x.Kit.InvId.Equals("S1T") || x.Kit.InvId.Equals("S1_CA"))).Count() > 0)
                    model.StressLogAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("S2") || x.Kit.InvId.Equals("S2_CA"))).Count() > 0)
                    model.StressManagementLogAccess = 1;
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && (x.Kit.InvId.Equals("P36") || x.Kit.InvId.Equals("P35"))).Count() > 0)
                    model.sleepLogAcess = 1;
            }
            return PartialView("_Journals", model);
        }

        #region Weight Loss Diary

        [Authorize]
        public ActionResult WeightLossDiary()
        {
            WeightLossDiaryModel model = new WeightLossDiaryModel();
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.WeightText = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View(model);
        }

        [Authorize]
        public JsonResult ListWeightLossDiary([FromBody] DiaryListModel model)
        {
            var result = JournalUtility.ListWeightLossJournal(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            if (result.weightLossList != null && result.weightLossList.Count > 0 && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                foreach (var weightLoss in result.weightLossList)
                {
                    weightLoss.Weight = weightLoss.Weight.HasValue ? (float)Math.Round(CommonUtility.ToMetric(weightLoss.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : weightLoss.Weight;
                    weightLoss.Waist = weightLoss.Waist.HasValue ? (float)Math.Round(CommonUtility.ToMetric(weightLoss.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : weightLoss.Waist;
                }
            }
            if (result.weightLossLists != null && result.weightLossLists.Count > 0 && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                foreach (var weightLoss in result.weightLossLists)
                {
                    weightLoss.Weight = weightLoss.Weight.HasValue ? (float)Math.Round(CommonUtility.ToMetric(weightLoss.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : weightLoss.Weight;
                    weightLoss.Waist = weightLoss.Waist.HasValue ? (float)Math.Round(CommonUtility.ToMetric(weightLoss.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : weightLoss.Waist;
                }
            }
            return Json(new { Records = result });
        }

        [Authorize]
        public ActionResult AddtoWeightLoss(int? id)
        {
            WeightLossJournalDto dto = new WeightLossJournalDto();
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            if (id.HasValue)
            {
                dto = JournalUtility.WeightLossDetails(id.Value);
                if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    dto.Weight = dto.Weight.HasValue ? CommonUtility.ToMetric(dto.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : dto.Weight;
                    dto.Waist = dto.Waist.HasValue ? CommonUtility.ToMetric(dto.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : dto.Waist;
                }
            }
            dto.WeightText = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
            dto.WaistText = string.Format(Translate.Message("L320"), Translate.Message(Measurements[BioLookup.Waist].MeasurementUnit));
            return PartialView("_AddtoWeightLoss", dto);
        }

        [Authorize]
        [HttpPost]
        public JsonResult CreateOrEdit(WeightLossJournalDto journal)
        {
            if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                journal.Weight = journal.Weight.HasValue ? CommonUtility.ToImperial(journal.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : journal.Weight;
                journal.Waist = journal.Waist.HasValue ? CommonUtility.ToImperial(journal.Waist.Value, BioLookup.Waist, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : journal.Waist;
            }
            return Json(new { Result = JournalUtility.AddToWeightLossJournal(journal, HttpContext.Session.GetInt32(SessionContext.UserId).Value, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value) });
        }

        #endregion

        #region Stress Diary

        [Authorize]
        public ActionResult StressDiary()
        {
            ViewData["DateFormat"] = HttpContext.Session.GetString(SessionContext.DateFormat);
            ViewData["HasActivePortal"] = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View();
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddtoStressDiary(StressDiaryModel model)
        {
            var result = JournalUtility.AddtoStressDiary(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListStress([FromBody] DiaryListModel model)
        {
            var response = JournalUtility.ListStress(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            var diary = from stress in response.StressDiaries
                        orderby stress.Date.Date ascending
                        group stress by stress.Date.Date;
            return Json(new { Result = "OK", Records = response, GraphData = diary });
        }

        [Authorize]
        public JsonResult ReadStress(int id)
        {
            var response = JournalUtility.ReadStress(id);
            return Json(new { Result = "OK", Record = response.stress });
        }

        #endregion

        #region Food Diary

        [Authorize]
        public ActionResult FoodDiary()
        {
            FoodDiaryModel model = new FoodDiaryModel();
            var Goals = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hraGoals;
            var KitsinUserProgram = KitUtility.ListAssignedKits(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).KitsinUserPrograms;
            if (KitsinUserProgram != null)
            {
                if (KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N2")).Count() > 0)
                    model.FoodDiaryAccess = 1;
                if (KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N4A")).Count() > 0 || KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N4A_CA")).Count() > 0)
                    model.FoodDiaryAccess = 2;
                if (KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N4B")).Count() > 0)
                    model.FoodDiaryAccess = 3;
                if (KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N15")).Count() > 0 || KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("P20")).Count() > 0
                        || KitsinUserProgram.Where(x => !string.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("N15_CA")).Count() > 0)
                    model.FoodDiaryAccess = 4;
            }
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View(model);
        }

        [Authorize]
        public ActionResult ListFoodDetails([FromBody] FoodDiaryListModel model)
        {
            model.startDateTime = Convert.ToDateTime(model.startDate);
            model.endDateTime = Convert.ToDateTime(model.endDate);
            FoodDiaryDetailsModel response = new FoodDiaryDetailsModel();
            var foodDetails = JournalUtility.ListFoodDetails(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            response.FoodDiaryList = foodDetails.FoodDiaryList;
            response.fg1RecentItems = foodDetails.fg1RecentItems;
            response.fg2RecentItems = foodDetails.fg2RecentItems;
            response.fg3RecentItems = foodDetails.fg3RecentItems;
            response.fg4RecentItems = foodDetails.fg4RecentItems;
            response.fg5RecentItems = foodDetails.fg5RecentItems;
            response.fg6RecentItems = foodDetails.fg6RecentItems;
            response.FoodDiaryAccess = model.FoodDiaryAccess;
            response.mousehover = model.mousehover;
            response.foodDate = model.startDateTime.ToString(HttpContext.Session.GetString(SessionContext.DateFormat));
            response.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            response.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return PartialView("_AddFoodDiary", response);
        }

        [Authorize]
        public ActionResult LoadFoodDetails([FromBody] FoodDiaryListModel model)
        {
            model.startDateTime = Convert.ToDateTime(model.startDate);
            model.endDateTime = Convert.ToDateTime(model.endDate);
            FoodDiaryDetailsModel response = new FoodDiaryDetailsModel();
              response.FoodDiaryList = JournalUtility.ListFood(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).FoodDiaryList;
            response.FoodDiaryAccess = model.FoodDiaryAccess;
            response.startDate = model.startDate;
            response.startDateTime = model.startDateTime;
            response.endDate = model.endDate;
            response.endDateTime = model.endDateTime;
            return PartialView("_EditFoodDiary", response);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddtoFoodDiary(FoodDiaryDetailsModel model)
        {
            model.startDateTime = Convert.ToDateTime(model.startDate);
            model.endDateTime = Convert.ToDateTime(model.endDate);
            var result = JournalUtility.AddtoFoodDiary(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        [HttpPost]
        public JsonResult DeleteFood(int foodId)
        {
            var result = JournalUtility.DeleteFood(foodId).success;
            return Json(result);
        }

        [Authorize]
        public JsonResult ListFood([FromBody] FoodDiaryListModel model)
        {
            model.startDateTime = Convert.ToDateTime(model.startDate);
            model.endDateTime = Convert.ToDateTime(model.endDate);
            var foodListResponse = JournalUtility.ListFood(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            List<double> FatGrams = new List<double>();
            List<double> CarbChoices = new List<double>();
            List<double> CarbGrams = new List<double>();
            List<double> Meat = new List<double>();
            List<double> Milk = new List<double>();
            List<double> Fruit = new List<double>();
            List<double> Starchy = new List<double>();
            List<double> Grains = new List<double>();
            List<double> Others = new List<double>();
            List<int> FoodDiaryAccess = new List<int>();
            List<string> Goals = new List<string>();
            double meat = 0;
            double milk = 0;
            double fruit = 0;
            double starchy = 0;
            double grains = 0;
            double others = 0;
            int foodDiaryAccess = 0;
            double meatGoal = 0;
            double milkGoal = 0;
            double fruitGoal = 0;
            double starchyGoal = 0;
            double grainsGoal = 0;
            DateTime date = Convert.ToDateTime(model.startDate).Date;
            var wellnessData = ParticipantUtility.ListWellnessData(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).WellnessData.LastOrDefault();
            var goals = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hraGoals;
            var nutritionGoal = ReportUtility.NutritionGoal(goals, null, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
            if (wellnessData != null && wellnessData.Weight.HasValue && goals.LtWt > wellnessData.Weight.Value && date >= wellnessData.CollectedOn.Date && nutritionGoal.SecondNutPlanArray != null)
            {
                meatGoal = Convert.ToDouble(nutritionGoal.SecondNutPlanArray.GetValue(0, 4).ToString());
                milkGoal = Convert.ToDouble(nutritionGoal.SecondNutPlanArray.GetValue(1, 4).ToString());
                fruitGoal = Convert.ToDouble(nutritionGoal.SecondNutPlanArray.GetValue(2, 4).ToString());
                starchyGoal = Convert.ToDouble(nutritionGoal.SecondNutPlanArray.GetValue(3, 4).ToString());
                grainsGoal = Convert.ToDouble(nutritionGoal.SecondNutPlanArray.GetValue(4, 4).ToString());
            }
            else
            {
                meatGoal = Convert.ToDouble(nutritionGoal.FirstNutPlanArray.GetValue(0, 4).ToString());
                milkGoal = Convert.ToDouble(nutritionGoal.FirstNutPlanArray.GetValue(1, 4).ToString());
                fruitGoal = Convert.ToDouble(nutritionGoal.FirstNutPlanArray.GetValue(2, 4).ToString());
                starchyGoal = Convert.ToDouble(nutritionGoal.FirstNutPlanArray.GetValue(3, 4).ToString());
                grainsGoal = Convert.ToDouble(nutritionGoal.FirstNutPlanArray.GetValue(4, 4).ToString());
            }
            while (date.Date <= Convert.ToDateTime(model.endDate).Date)
            {
                meat = grains = fruit = starchy = milk = others = 0;
                FatGrams.Add(Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Sum(x => x.FatGrams).GetValueOrDefault(), 1));
                CarbChoices.Add(Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Sum(x => x.CarbChoices).GetValueOrDefault(), 1));
                CarbGrams.Add(Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Sum(x => x.CarbGrams).GetValueOrDefault(), 1));
                if (date.Date <= DateTime.Now)
                {
                    if (model.FoodDiaryAccess == 1 || (foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Count() > 0 && foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Sum(x => x.ServingSize) == 0))
                    {
                        meat = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 1).Count() > 0 ? 1 : 0;
                        grains = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 2).Count() > 0 ? 1 : 0;
                        fruit = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 3).Count() > 0 ? 1 : 0;
                        starchy = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 4).Count() > 0 ? 1 : 0;
                        milk = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 5).Count() > 0 ? 1 : 0;
                        others = foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 6).Count() > 0 ? 1 : 0;
                        foodDiaryAccess = 1;

                        if (foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Count() == 0)
                            Goals.Add("none");
                        else if ((others == 0 || others > 0) && meat > 0 && milk > 0 && fruit > 0 && starchy > 0 && grains > 0)
                            Goals.Add("green");
                        else
                            Goals.Add("red");

                    }
                    else
                    {
                        meat = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 1).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        grains = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 2).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        fruit = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 3).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        starchy = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 4).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        milk = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 5).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        others = Math.Round(foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date && x.FoodGroupId == 6).Sum(x => x.ServingSize).GetValueOrDefault(), 1);
                        foodDiaryAccess = model.FoodDiaryAccess;

                        if (foodListResponse.FoodDiaryList.Where(x => x.Date.Date == date.Date).Count() == 0)
                            Goals.Add("none");
                        else if (meatGoal == meat && milkGoal == milk && fruitGoal == fruit && starchyGoal == starchy && grainsGoal == grains)
                            Goals.Add("green");
                        else
                            Goals.Add("red");
                    }
                }
                else
                {
                    Goals.Add("none");
                }

                Meat.Add(meat);
                Milk.Add(milk);
                Fruit.Add(fruit);
                Starchy.Add(starchy);
                Grains.Add(grains);
                Others.Add(others);
                FoodDiaryAccess.Add(foodDiaryAccess);
                date = date.AddDays(1);
            }
            return Json(new
            {
                Result = "OK",
                FatGrams,
                CarbChoices,
                CarbGrams,
                Meat,
                Milk,
                Fruit,
                Starchy,
                Grains,
                Others,
                FoodDiaryAccess,
                Goals,
                meatGoal,
                milkGoal,
                fruitGoal,
                starchyGoal,
                grainsGoal,
                nutFatNum = nutritionGoal.NutFatNum.Value,
                newNutFatNum = nutritionGoal.NewNutFatNum,
                nutCarbFrom = nutritionGoal.NutCarbFrom,
                nutCarbTo = nutritionGoal.NutCarbTo,
                haveDataforaWeek = foodListResponse.FoodDiaryList.Count() == 0 ? false : true,
            });
        }

        #endregion

        #region Exercise Diary

        [Authorize]
        public ActionResult ExerciseDiary()
        {
            ExerciseModel model = new ExerciseModel();
            //Get goals
            if (HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null)
            {
                var reportResponse = ReportUtility.ReadHRAGoals(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
                if (reportResponse.hraGoals.StExPt.HasValue)
                {
                    model.shortTermPoints = reportResponse.hraGoals.StExPt;
                    model.shortTermMinutes = Convert.ToInt16(model.shortTermPoints.Value) * 3;
                }
                if (reportResponse.hraGoals.LtExPt.HasValue)
                {
                    model.longTermPoints = reportResponse.hraGoals.LtExPt;
                    model.longTermMinutes = Convert.ToInt16(model.longTermPoints.Value) * 3;
                }
            }
            model.ExerciseDiaryAccess = 2;
            var KitsinUserProgram = KitUtility.ListAssignedKits(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).KitsinUserPrograms;
            if (KitsinUserProgram != null)
            {
                if (KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("E1_CA")).Count() > 0
                    || KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("E3_CA")).Count() > 0
                    || KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("E3")).Count() > 0
                    || KitsinUserProgram.Where(x => !String.IsNullOrEmpty(x.Kit.InvId) && x.Kit.InvId.Equals("E1")).Count() > 0)
                    model.ExerciseDiaryAccess = 3;
            }
            model.ExerciseTypes = JournalUtility.ListExerciseType().ExcerciseType.Select(x => new SelectListItem { Text = Translate.Message(x.LanguageItem), Value = x.Id.ToString() + "^" + x.Value.ToString() });
            model.RPES = CommonUtility.GetRPE().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.TimeZone = User.TimeZone();
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddtoExercsieDiary(int? id, DateTime ExerciseDate, int Type, int? Duration, byte? RPE, short? HeartRate, int? Points, int? StepsPerDay, string Notes)
        {

            var result = JournalUtility.AddtoExerciseDiary(id, ExerciseDate, Type, Duration, RPE, HeartRate, Points, StepsPerDay, Notes, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListExercise([FromBody] DiaryListModel model)
        {
            if (model.startDate.HasValue && model.endDate.HasValue)
            {
                model.startDate = model.startDate.Value;
                model.endDate = model.endDate.Value.AddDays(1);
            }
            else if (model.Days != null && model.Days > 0)
            {
                model.endDate = DateTime.UtcNow;
                model.startDate = model.endDate.Value.AddDays(-(model.Days.Value));
            }
            var response = JournalUtility.ListExercise(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            Func<DateTime, int> weekProjector =
                d => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                    d,
                    CalendarWeekRule.FirstFourDayWeek,
                    DayOfWeek.Monday);
            var exerciseByDay = from exercise in response.ExcerciseDiaries
                                where exercise.Type != 90
                                group exercise by exercise.Date.Date;
            var exerciseByWeek = from exercise in response.ExcerciseDiaries
                                 where exercise.Type != 90
                                 group exercise by weekProjector(exercise.Date.Date);

            return Json(new { Result = "OK", Records = response, DailyRecord = exerciseByDay, WeeklyRecord = exerciseByWeek });
        }

        [Authorize]
        public JsonResult ReadExercise(int id)
        {
            var response = JournalUtility.ReadExercise(id);
            return Json(new { Result = "OK", Record = response.Exercise });
        }

        [Authorize]
        public JsonResult GetExercisePoints(string Exercise, int Internsity, int Duration)
        {
            var response = JournalUtility.GetExercisePoints(Exercise, Internsity, Duration);
            return Json(new { Result = "OK", Record = response });
        }

        #endregion

        #region Tobacco Log
        [Authorize]
        public ActionResult TobaccoLog()
        {
            TobaccoLogModel model = new TobaccoLogModel();
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View(model);
        }

        [Authorize]
        public JsonResult ListTobaccoLog([FromBody] DiaryListModel model)
        {
            return Json(new { Result = "OK", Records = JournalUtility.ListTobaccoLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value) });
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddtoTobaccoLog(TobaccoLogModel model)
        {
            var result = JournalUtility.AddtoTobaccoLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ReadTobaccoLog(int id)
        {
            var response = JournalUtility.ReadTobaccoLog(id);
            return Json(new { Result = "OK", Record = response.tobaccoLog });
        }
        #endregion

        #region Strength Training Log

        [Authorize]
        public ActionResult StrengthTrainingLog()
        {
            StrengthTrainingLogModel model = new StrengthTrainingLogModel();
            var Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.WeightText = string.Format(Translate.Message("L318"), Translate.Message(Measurements[BioLookup.Weight].MeasurementUnit));
            model.TrainingSetList = CommonUtility.GetTrainingSet();
            model.TrainingTypes = JournalUtility.ListTrainingType().TrainingTypes.Select(c => new SelectListItem { Text = Translate.Message(c.LanguageItem), Value = c.Id.ToString() }).OrderBy(s => s.Text);
            model.RPE = CommonUtility.GetRPE().Select(x => new SelectListItem { Text = x.Text, Value = x.Value });
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddEditTrainingLog(StrengthTrainingLogModel model)
        {
            if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                foreach (var trainingListset in model.trainingLogSet)
                {
                    trainingListset.Weight = trainingListset.Weight.HasValue ? CommonUtility.ToImperial(trainingListset.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : trainingListset.Weight;
                }
            }
            var result = JournalUtility.AddEditTrainingLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).trainingsetCount;
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListTraining([FromBody] StrengthTrainingListModel model)
        {
            var trainingListResponse = JournalUtility.ListTraining(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            if (trainingListResponse.listStrengthTrainingLogResponse != null && trainingListResponse.listStrengthTrainingLogResponse.Count > 0 && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                foreach (var trainingList in trainingListResponse.listStrengthTrainingLogResponse)
                {
                    foreach (var trainingListset in trainingList.StrengthTrainingLogSet)
                    {
                        trainingListset.Weight = trainingListset.Weight.HasValue ? (float)Math.Round(CommonUtility.ToMetric(trainingListset.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1) : trainingListset.Weight;
                    }
                }
            }
            return Json(new { Result = "OK", Records = trainingListResponse });
        }

        [Authorize]
        public JsonResult ReadTraining(int id)
        {
            var response = JournalUtility.ReadTraining(id);
            if (Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
            {
                foreach (var trainingListset in response.training.StrengthTrainingLogSet)
                {
                    trainingListset.Weight = trainingListset.Weight.HasValue ? CommonUtility.ToMetric(trainingListset.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value) : trainingListset.Weight;
                }
            }
            return Json(new { Result = "OK", Record = response.training });
        }

        #endregion

        #region Stress Management Log
        [Authorize]
        public ActionResult StressManagementLog()
        {
            StressManagementLogModel model = new StressManagementLogModel();
            model.StressRatingList = CommonUtility.GetRatingList();
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddEditStressManagement(StressManagementLogModel model)
        {
            var result = JournalUtility.AddEditStressManagement(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListStressManagement([FromBody] DiaryListModel model)
        {
            return Json(new { Result = "OK", Records = JournalUtility.ListStressManagement(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value) });
        }

        [Authorize]
        public JsonResult ReadStressManagement(int id)
        {
            var response = JournalUtility.ReadStressManagement(id);
            return Json(new { Result = "OK", Record = response.stress });
        }

        #endregion

        #region Sleep Log

        [Authorize]
        public ActionResult SleepLog()
        {
            SleepLogModel model = new SleepLogModel();
            model.sleeplogList = CommonUtility.GetSleepLogSelectList();
            model.DateFormat = HttpContext.Session.GetString(SessionContext.DateFormat);
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View("SleepLog", model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddtoSleepLog(SleepLogModel model)
        {
            var result = JournalUtility.AddtoSleepLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName));
            return Json(new { Result = "OK", Record = result });
        }

        [Authorize]
        public JsonResult ListSleepLog([FromBody] DiaryListModel model)
        {
            var dropWonText = CommonUtility.GetSleepLogSelectList().ToList();
            ListSleepLogResponse response = JournalUtility.ListSleepLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);

            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone));
            var sleepLogLists = response.sleepLogLists.Select(x => new
            {
                Id = x.Id,
                UserId = x.UserId,
                Start = TimeZoneInfo.ConvertTimeFromUtc(x.Start, custTZone).ToString(),
                End = TimeZoneInfo.ConvertTimeFromUtc(x.End, custTZone).ToString(),
                SleepQuality = x.SleepQuality,
                DaytimeAlertness = x.DaytimeAlertness,
                Mood = x.Mood,
                sleptHours = x.sleptHours,
                Notes = x.Notes,
                SleepQualityText = x.SleepQuality.HasValue && x.SleepQuality != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.SleepQuality).FirstOrDefault().Text : String.Empty,
                DaytimeAlertnessText = x.DaytimeAlertness.HasValue && x.DaytimeAlertness != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.DaytimeAlertness).FirstOrDefault().Text : String.Empty,
                MoodText = x.Mood.HasValue && x.Mood != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.Mood).FirstOrDefault().Text : String.Empty
            });
            var sleepLogList = response.sleepLogList.Select(x => new
            {
                Id = x.Id,
                UserId = x.UserId,
                Start = TimeZoneInfo.ConvertTimeFromUtc(x.Start, custTZone).ToString(),
                End = TimeZoneInfo.ConvertTimeFromUtc(x.End, custTZone).ToString(),
                SleepQuality = x.SleepQuality,
                DaytimeAlertness = x.DaytimeAlertness,
                Mood = x.Mood,
                sleptHours = x.sleptHours,
                Notes = x.Notes,
                SleepQualityText = x.SleepQuality.HasValue && x.SleepQuality != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.SleepQuality).FirstOrDefault().Text : String.Empty,
                DaytimeAlertnessText = x.DaytimeAlertness.HasValue && x.DaytimeAlertness != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.DaytimeAlertness).FirstOrDefault().Text : String.Empty,
                MoodText = x.Mood.HasValue && x.Mood != 0 ? dropWonText.Where(y => Convert.ToByte(y.Value) == x.Mood).FirstOrDefault().Text : String.Empty

            });
            return Json(new { Result = "OK", sleepLogLists = sleepLogLists, sleepLogList = sleepLogList, totalRecords = response.totalRecords });
        }

        [Authorize]
        public JsonResult ReadSleepLog(int id)
        {
            var response = JournalUtility.ReadSleepLog(id, HttpContext.Session.GetString(SessionContext.ParticipantTimeZoneName));
            return Json(new { Result = "OK", Record = response.sleepLog });
        }

        #endregion

        #region Daily Vitals Log
        [Authorize]
        public ActionResult DailyVitalsLog()
        {
            DailyVitalsLogModel model = new DailyVitalsLogModel { HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal)) };
            return View(model);
        }

        [Authorize]
        [HttpPost]
        public JsonResult AddEditDailyVitals(VitalsModel model)
        {
            if (model.DailyVitals.HasWeight.HasValue && model.DailyVitals.HasWeight == 1)
            {
                if (model.DailyVitals.Weight.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    model.DailyVitals.Weight = CommonUtility.ToImperial(model.DailyVitals.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
            }
            else
            {
                model.DailyVitals.Weight = null;
            }
            int? adminId = null;
            if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                adminId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            var result = JournalUtility.AddEditDailyVitals(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, adminId, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetString(SessionContext.ParticipantTimeZone), model);
            return Json(new { Result = "OK", Record = result.success });
        }

        [Authorize]
        public ActionResult AddtoDailyVitals(int? id)
        {
            VitalsModel model = new VitalsModel();
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone));
            model.Measurements = CommonUtility.ListMeasurements(Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit))).Measurements;
            model.Gender = HttpContext.Session.GetInt32(SessionContext.Gender).Value;
            if (id.HasValue)
            {
                var response = JournalUtility.ReadVitals(id.Value);
                if (response.dailyVital.Weight.HasValue && Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric)
                {
                    response.dailyVital.Weight = CommonUtility.ToMetric(response.dailyVital.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                }
                model.DailyVitals = response.dailyVital;
            }
            return PartialView("_AddtoVitalsLog", model);
        }

        [Authorize]
        public JsonResult ListVitalsLog([FromBody] DiaryListModel model)
        {
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone));
            var response = JournalUtility.ListVitalsLog(model, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(new
            {
                Result = "OK",
                vitalsLog = response.dailyVitals.Select(x => new
                {
                    x.Id,
                    x.UserId,
                    Date = x.Date.HasValue ? x.Date.Value.ToString(HttpContext.Session.GetString(SessionContext.DateFormat)) : "",
                    Weight = x.Weight.HasValue ? Convert.ToInt16(HttpContext.Session.GetInt32(SessionContext.Unit)) == (int)Unit.Metric ? Math.Round(CommonUtility.ToMetric(x.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value), 1).ToString() : x.Weight.ToString() : "",
                    Points = x.Points.HasValue ? x.Points.Value : 0,
                    PointsText = x.Points.HasValue ? x.Points.Value.ToString() + "/10" : ""
                }),
                TotalRecords = response.totalRecords,
                ShowAdd = response.dailyVitals.Count > 0 && response.dailyVitals.FirstOrDefault().Date == TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date ? false : true
            });
        }
        #endregion

        [Authorize]
        public ActionResult DailyVitalsLogTest()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddEditDailyVitalsLog(int vitalsId, string ques, string Ans, float? weight)
        {
            VitalsModel model = new VitalsModel();
            model.DailyVitals = new VitalsLogDto();
            model.DailyVitals.Id = vitalsId;
            switch (ques)
            {
                case "weightQuestion":
                    model.DailyVitals.HasWeight = weight.HasValue ? (byte)1 : (byte)2;
                    model.DailyVitals.Weight = weight;
                    if (model.DailyVitals.Weight.HasValue && HttpContext.Session.GetInt32(SessionContext.Unit).HasValue && HttpContext.Session.GetInt32(SessionContext.Unit) == (int)Unit.Metric)
                    {
                        model.DailyVitals.Weight = CommonUtility.ToImperial(model.DailyVitals.Weight.Value, BioLookup.Weight, HttpContext.Session.GetInt32(SessionContext.Unit).Value);
                    }
                    break;
                case "exerciseQuestion":
                    model.DailyVitals.AerobicExercise = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "eatingQuestion":
                    model.DailyVitals.HealthyEating = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "hydrationQuestion":
                    model.DailyVitals.Hydration = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "alcoholQuestion":
                    model.DailyVitals.Alcohol = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "tobaccoQuestion":
                    model.DailyVitals.Tobacco = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "medicationQuestion":
                    model.DailyVitals.Medications = Ans.Equals("3") ? (byte)3 : Ans.Equals("2") ? (byte)2 : (byte)1;
                    break;
                case "sleepQuestion":
                    model.DailyVitals.Sleep = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "stressQuestion":
                    model.DailyVitals.Stress = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                case "happyQuestion":
                    model.DailyVitals.Happy = Ans.Equals("1") ? (byte)1 : (byte)2;
                    break;
                default: break;
            }

            int? adminId = null;
            if (HttpContext.Session.GetInt32(SessionContext.AdminId).HasValue)
                adminId = HttpContext.Session.GetInt32(SessionContext.AdminId).Value;
            var result = JournalUtility.AddEditDailyVitals(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, adminId, HttpContext.Session.GetInt32(SessionContext.ParticipantPortalId).Value, HttpContext.Session.GetString(SessionContext.ParticipantTimeZone), model);
            return Json(new { Result = "OK", Record = result, IncentivePoints = result.vitalsCompletionPoints });
        }
    }
}
