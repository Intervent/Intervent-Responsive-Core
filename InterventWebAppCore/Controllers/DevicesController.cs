using Intervent.HWS;
using Intervent.Web.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InterventWebApp.Controllers
{
    public class DevicesController : BaseController
    {
        public ActionResult Index(int? showGraphFor)
        {
            DateTime endDate = DateTime.UtcNow;
            DateTime startDate = endDate.AddDays(-7);
            var participantId = HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value;
            var timeZone = User.TimeZone();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            var model = DevicesUtility.PullOverviewData(startDate, endDate, participantId, timeZone);
            if (showGraphFor.HasValue)
                model.showGraphFor = showGraphFor.Value;
            model.connectedDevicesCount = WearableUtility.GetUserWearableDevices(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).Count;
            model.HasActivePortal = Convert.ToBoolean(HttpContext.Session.GetString(SessionContext.HasActivePortal));
            return View(model);
        }

        public JsonResult Summaries()
        {
            DateTime startDate = default(DateTime), endDate = default(DateTime);
            endDate = DateTime.UtcNow;
            startDate = endDate.AddDays(-(7));
            var readings = DevicesUtility.GetSummariesData(startDate, endDate, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            SummariesData summariesData = new SummariesData();
            if (readings.Summaries.Count() > 0)
            {
                summariesData.recentDate = Convert.ToDateTime(readings.Summaries[0].EndTimeStamp).ToString("MMM dd");
                summariesData.steps = readings.Summaries.Select(x => x.Steps).Sum();
                summariesData.remainingSteps = 7500 - summariesData.steps;
                summariesData.minutes = readings.Summaries.Select(x => (x.EndTimeStamp.Subtract(x.StartTimeStamp)).Minutes).Sum(); ;
                summariesData.remainingMinutes = 75 - summariesData.minutes;
            }
            return Json(summariesData);
        }

        public JsonResult WeeklyGoals(int? stepsGoal, int? MIminGoal, int? VIminGoal)
        {
            var result = DevicesUtility.AddEditWeeklyGoals(stepsGoal, MIminGoal, VIminGoal, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, HttpContext.Session.GetInt32(SessionContext.UserId).Value);
            return Json(result);
        }

        [Authorize]
        public ActionResult PhysicalActivity()
        {
            return PartialView("_PhysicalActivity");
        }

        [HttpPost]
        public JsonResult ListPhysicalActivityData(int days, string startDate, string endDate)
        {
            RoutineData routineData = DevicesUtility.ListPhysicalActivityData(days, startDate, endDate, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(routineData);
        }


        [Authorize]
        public ActionResult Weight()
        {
            return PartialView("_Weight");
        }

        public JsonResult ListWeightData(int days, string startDate, string endDate)
        {
            DateTime? startDateFilter = null, endDateFilter = null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateFilter = Convert.ToDateTime(startDate);
                endDateFilter = Convert.ToDateTime(endDate).AddDays(1); ;
                days = (int)(startDateFilter.Value - endDateFilter.Value).TotalDays;
            }
            else if (days > 0)
            {
                endDateFilter = DateTime.UtcNow;
                startDateFilter = endDateFilter.Value.AddDays(-(days));
            }
            float height = 0;
            if (HttpContext.Session.GetInt32(SessionContext.HRAId).HasValue)
            {
                height = HRAUtility.ReadHRA(HttpContext.Session.GetInt32(SessionContext.HRAId).Value).hra.HealthNumbers.Height.Value;
            }
            List<Weight> Weight = new List<Weight>();
            var response = DevicesUtility.GetWeightData(startDateFilter, endDateFilter, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            if (response.Weights.Count() > 0)
            {
                Weight = response.Weights.Select(x => new Weight { weight = x.weight, bmi = x.bmi, timestamp = x.timestamp.ToString() }).ToList();
            }
            WeightData weightData = new WeightData();
            weightData.graphdata = new List<GraphData1>();
            if (Weight.Count > 0)
            {
                double bmi = 0;
                weightData.recentDate = Weight[0].timestamp;
                weightData.recentWeight = Weight[0].weight.Value;
                if (Weight[0].bmi.Value <= 0 && height != 0)
                {
                    weightData.recentBMI = CommonUtility.GetBMI(height, (float)Math.Round(weightData.recentWeight, 1));
                }
                else
                {
                    weightData.recentBMI = Math.Round(Weight[0].bmi.Value, 1);
                }
                double total = 0, secondlatestweight = 0, initialWeight = 0;
                int i = 0;
                foreach (var weightValue in Weight)
                {
                    if (weightValue.weight.HasValue)
                    {
                        if (weightData.latestWeight == 0)
                            weightData.latestWeight = weightValue.weight.Value;
                        total = total + weightValue.weight.Value;
                        weightData.Count++;
                        if (i > 0 && secondlatestweight == 0)
                        {
                            secondlatestweight = weightValue.weight.Value;
                            weightData.diffDays = (int)(Convert.ToDateTime(weightData.recentDate) - Convert.ToDateTime(weightValue.timestamp)).TotalDays;
                        }
                        initialWeight = weightValue.weight.Value;
                        height = weightValue.height.HasValue ? (float)weightValue.height.Value : height;
                        i++;
                    }
                    if (weightValue.bmi == 0 && height != 0)
                    {
                        weightData.startingBMI = CommonUtility.GetBMI(height, (float)(weightValue.weight));
                    }
                    else
                    {
                        weightData.startingBMI = Math.Round(weightValue.bmi.Value, 1);
                    }
                    bmi += weightData.startingBMI;
                    weightData.graphdata.Add(new GraphData1 { date = Convert.ToDateTime(weightValue.timestamp).ToShortDateString(), value = Math.Round((weightValue.weight ?? 0), 1) });
                }
                weightData.StartingWeight = Math.Round((initialWeight), 1);
                weightData.recentWeight = Math.Round((weightData.recentWeight), 1);
                weightData.latestWeight = Math.Round((weightData.latestWeight), 1);
                weightData.avgWeight = Math.Round(((total / weightData.graphdata.Count())), 1);
                weightData.avgBMI = Math.Round((bmi / weightData.graphdata.Count()), 1);
                weightData.startingBMI = Math.Round((weightData.startingBMI), 1);
                if (Weight.Count() > 1)
                {
                    weightData.weightDiffRecent = Math.Round(((weightData.latestWeight) - (secondlatestweight)), 1);
                    weightData.weightDiffHistory = Math.Round(((weightData.latestWeight) - (weightData.StartingWeight)), 1);
                    weightData.overallBMIdiff = Math.Round(((weightData.recentBMI) - (weightData.startingBMI)), 1);
                }

            }
            return Json(weightData);
        }

        [Authorize]
        public ActionResult BloodPressure()
        {
            return PartialView("_BloodPressureData");
        }

        public JsonResult ListBiometricsData(int days, string startDate, string endDate)
        {
            var biometricData = DevicesUtility.ListBiometricsData(days, startDate, endDate, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(biometricData);
        }

        [Authorize]
        public ActionResult Glucose()
        {
            if (CommonUtility.HasAdminRole(User.RoleCode()) && CommonUtility.IsIntegratedWithIntuity(HttpContext.Session.GetInt32(SessionContext.IntegrationWith)))
            {
                if (!ExternalUtility.IsValidIntuityUserId(null, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value))
                {
                    TempData["NoIntegMessage"] = "No connection has been established with patterns app.";
                }
            }
            return PartialView("_Glucose");
        }
        public JsonResult ListGlucoseData(int days, string startDate, string endDate)
        {

            var timeZone = User.TimeZone();
            if (!string.IsNullOrEmpty(HttpContext.Session.GetString(SessionContext.ParticipantTimeZone)))
                timeZone = HttpContext.Session.GetString(SessionContext.ParticipantTimeZone);
            string hraCompleteDate = ""; int hraId = 0;
            if (HttpContext.Session.GetString(SessionContext.HRACompleteDate) != null)
            {
                hraCompleteDate = HttpContext.Session.GetString(SessionContext.HRACompleteDate);
                hraId = HttpContext.Session.GetInt32(SessionContext.HRAId).Value;
            }
            GlucoseData result = DevicesUtility.ListGlucoseData(days, startDate, endDate, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value, timeZone, hraId, hraCompleteDate);
            return Json(result);
        }

        [Authorize]
        public ActionResult Nutrition()
        {
            return PartialView("_NutritionData");
        }

        public JsonResult ListNutritionData(int days, int page, int pageSize, int totalRecords, string startDate, string endDate)
        {
            DateTime? startDateFilter = null, endDateFilter = null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateFilter = Convert.ToDateTime(startDate);
                endDateFilter = Convert.ToDateTime(endDate).AddDays(1); ;
                days = (int)(startDateFilter.Value - endDateFilter.Value).TotalDays;
            }
            else if (days > 0)
            {
                endDateFilter = DateTime.UtcNow;
                startDateFilter = endDateFilter.Value.AddDays(-(days));
            }
            var result = new NutritionData();
            var response = DevicesUtility.GetNutritionData(startDateFilter, endDateFilter, page, pageSize, totalRecords, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            var dataCount = 0;
            if (response.nutritionList.Count() > 0)
            {
                result.recentDate = Convert.ToDateTime(response.nutritionList[response.nutrition.Count() - 1].Timestamp).ToString("MMM dd");
                dataCount = response.nutritionList.GroupBy(y => y.Timestamp.Date).Count();
            }
            result.data = response.nutrition.Select(x => new NutritionModel
            {
                calories = x.Calories.HasValue ? Math.Round(x.Calories.Value, 1).ToString() : Translate.Message("L1149"),
                carbohydrates = x.Carbohydrates.HasValue ? Math.Round(x.Carbohydrates.Value, 1).ToString() : Translate.Message("L1149"),
                fat = x.Fat.HasValue ? Math.Round(x.Fat.Value, 1).ToString() : Translate.Message("L1149"),
                fiber = x.Fiber.HasValue ? Math.Round(x.Fiber.Value, 1).ToString() : Translate.Message("L1149"),
                meal = !string.IsNullOrEmpty(x.Meal) ? x.Meal : !string.IsNullOrEmpty(x.Name) ? x.Name : Translate.Message("L1149"),
                protein = x.Protein.HasValue ? Math.Round(x.Protein.Value, 1).ToString() : Translate.Message("L1149"),
                sodium = x.Sodium.HasValue ? Math.Round(x.Sodium.Value, 1).ToString() : Translate.Message("L1149"),
                timestamp = x.Timestamp.ToShortTimeString(),
                datestamp = DateTime.Parse(x.Timestamp.ToLongDateString()).ToString("MMM dd, yyyy"),
                user_id = x.UserId.ToString()
            }).ToList();
            var hrareport = ReportUtility.ReadHRAReport(HttpContext.Session.GetInt32(SessionContext.HRAId).Value);
            result.avgFat = response.nutritionList.Sum(x => x.Fat).HasValue ? Math.Round(response.nutritionList.Sum(x => x.Fat).Value / dataCount, 1).ToString() : "";
            result.avgCarb = response.nutritionList.Sum(x => x.Carbohydrates).HasValue ? Math.Round(response.nutritionList.Sum(x => x.Carbohydrates).Value / dataCount, 1).ToString() : "";
            result.avgSodium = response.nutritionList.Sum(x => x.Sodium).HasValue ? Math.Round(response.nutritionList.Sum(x => x.Sodium).Value / dataCount, 1).ToString() : "";

            if (hrareport != null && hrareport.hra != null)
            {
                var nutritionGoals = ReportUtility.NutritionGoal(hrareport.hra.Goals, null, HttpContext.Session.GetInt32(SessionContext.ProgramType), HttpContext.Session.GetString(SessionContext.AssessmentName), HttpContext.Session.GetInt32(SessionContext.IntegrationWith), HttpContext.Session.GetInt32(SessionContext.Gender), ShowSelfScheduling());
                result.fatGoal = "<p>" + Translate.Message("L1640") + " " + nutritionGoals.FirstNutPlan + "<span class='monthly-nutrient'>" + nutritionGoals.NutFatNum.Value.ToString() + "-" + nutritionGoals.NewNutFatNum.ToString() + " g" + "</span></p>";
                result.carbGoal = "<p>" + Translate.Message("L1640") + " " + nutritionGoals.FirstNutPlan + "<span class='monthly-nutrient'>" + nutritionGoals.NutCarbFrom.ToString() + "-" + nutritionGoals.NutCarbTo.ToString() + " g" + "</span></p>";
                result.sodiumGoal = "<p>" + Translate.Message("L1640") + " " + nutritionGoals.FirstNutPlan + "<span class='monthly-nutrient'>" + Translate.Message("L1528") + " 1500 mg</span></p>";
                if (nutritionGoals.NutMaint)
                {
                    result.fatGoal = result.fatGoal + "<p>" + Translate.Message("L1640") + " " + nutritionGoals.NutMaintMealPlan + "<span class='monthly-nutrient'>" + nutritionGoals.NutLtFatNum.Value.ToString() + "-" + nutritionGoals.NewNutLtFatNum.ToString() + " g" + "</span></p>";
                    result.carbGoal = result.carbGoal + "<p>" + Translate.Message("L1640") + " " + nutritionGoals.NutMaintMealPlan + "<span class='monthly-nutrient'>" + nutritionGoals.NutLtCarbFrom.ToString() + "-" + nutritionGoals.NutLtCarbTo.ToString() + " g" + "</span></p>";
                    result.sodiumGoal = result.sodiumGoal + "<p>" + Translate.Message("L1640") + " " + nutritionGoals.NutMaintMealPlan + "<span class='monthly-nutrient'>" + Translate.Message("L1528") + " 1500 mg</span></p>";
                }
            }
            result.totalRecords = response.totalRecords;
            var totalCalories = response.nutritionList.Select(x => x.Calories).Sum();
            if (totalCalories.HasValue && totalCalories != 0)
                result.avgCalories = Math.Round(totalCalories.Value / dataCount, 1);
            result.gender = DevicesUtility.GetGender(HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value);
            return Json(result);
        }


        [Authorize]
        public ActionResult Sleep()
        {
            return PartialView("_SleepData");
        }

        public JsonResult ListSleepData(int days, string startDate, string endDate)
        {
            DateTime? startDateFilter = null, endDateFilter = null;
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateFilter = Convert.ToDateTime(startDate);
                endDateFilter = Convert.ToDateTime(endDate).AddDays(1); ;
                days = (int)(startDateFilter.Value - endDateFilter.Value).TotalDays;
            }
            else if (days > 0)
            {
                endDateFilter = DateTime.UtcNow;
                startDateFilter = endDateFilter.Value.AddDays(-(days));
            }
            var data = new SleepData();
            var response = DevicesUtility.GetSleepData(startDateFilter, endDateFilter, HttpContext.Session.GetInt32(SessionContext.ParticipantId).Value).sleep;
            data.monthData = new List<GraphData1>();
            data.weekData = new List<SleepGraphData>();
            if (response.Count() > 0)
            {
                data.mostRecent = response[0].starttimestamp.ToString("MMM dd");
                var sleepData = response.GroupBy(x => x.starttimestamp).Select(groupedData => new EXT_SleepsDto
                {
                    starttimestamp = groupedData.Key,
                    totalsleepduration = groupedData.Sum(y => y.totalsleepduration),
                    deepduration = groupedData.Sum(y => y.deepduration),
                    lightduration = groupedData.Sum(y => y.lightduration),
                    awakeduration = groupedData.Sum(y => y.awakeduration),

                }).ToList();
                TimeSpan t = TimeSpan.FromSeconds(sleepData.Average(x => x.totalsleepduration));
                data.avgSleep = t.ToString(@"hh\:mm");
                t = TimeSpan.FromSeconds(sleepData.Average(x => x.deepduration));
                data.avgDeepSleep = t.ToString(@"hh\:mm");
                t = TimeSpan.FromSeconds(sleepData.Average(x => x.lightduration));
                data.avgLightSleep = t.ToString(@"hh\:mm");
                t = TimeSpan.FromSeconds(sleepData.Average(x => x.awakeduration));
                data.avgTimeAwake = t.ToString(@"hh\:mm");
                foreach (var sleep in sleepData)
                {
                    var sleepDuration = (TimeSpan.FromSeconds(sleep.totalsleepduration)).ToString(@"hh\.mm");
                    data.monthData.Add(new GraphData1 { date = sleep.starttimestamp.ToShortDateString(), value = Convert.ToDouble(sleepDuration), label = sleepDuration.Split('.')[0] + " " + Translate.Message("L4046") + " " + sleepDuration.Split('.')[1] + " " + Translate.Message("L4047") });
                }
            }
            return Json(data);
        }
    }
}