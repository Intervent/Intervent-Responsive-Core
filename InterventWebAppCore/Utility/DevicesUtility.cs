using Intervent.HWS;
using Intervent.Web.DataLayer;
using Intervent.Web.DTO;
using System.Globalization;

namespace InterventWebApp
{
    public class DevicesUtility
    {
        public static DateTime FirstDateOfWeekISO8601(int year, int weekOfYear)
        {
            DateTime jan1 = new DateTime(year, 1, 1);
            int daysOffset = DayOfWeek.Thursday - jan1.DayOfWeek;

            DateTime firstThursday = jan1.AddDays(daysOffset);
            var cal = CultureInfo.CurrentCulture.Calendar;
            int firstWeek = cal.GetWeekOfYear(firstThursday, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);

            var weekNum = weekOfYear;

            if (firstWeek == 1)
            {
                weekNum -= 1;
            }

            var result = firstThursday.AddDays(weekNum * 7);

            return result.AddDays(-3);
        }

        public static DeviceModel PullOverviewData(DateTime? startDate, DateTime? endDate, int participantId, string timeZone)
        {
            DeviceModel model = new DeviceModel();

            //Physical activity
            var readings = GetSummariesData(startDate.Value, endDate.Value, participantId);
            var workoutData = GetWorkoutData(startDate, endDate, participantId);
            if (readings.Summaries.Count() > 0)
                model.physicalActivityCount = readings.Summaries.Count();
            if (workoutData.Workouts.Count() > 0)
                model.physicalActivityCount = model.physicalActivityCount + workoutData.Workouts.Count();

            //weight
            var weightData = GetWeightData(startDate, endDate, participantId);
            if (weightData.Weights.Count() > 0)
                model.weightCount = weightData.Weights.Count();

            //BP
            var biometric = GetBloodPressureData(startDate, endDate, participantId);
            if (biometric.BloodPressures.Count() > 0)
                model.bpCount = biometric.BloodPressures.Count();

            //Glucose
            var glucoseData = GetDiabetesData(startDate, endDate, participantId, timeZone);
            if (glucoseData.glucose.Count() > 0)
                model.glucCount = glucoseData.glucose.Count();

            //nutrition
            var nutritionData = GetNutritionData(startDate, endDate, 0, 100, 0, participantId);
            if (nutritionData.nutrition.Count() > 0)
                model.nutritionCount = nutritionData.nutrition.Count();

            //sleep
            var sleepData = GetSleepData(startDate, endDate, participantId);
            if (sleepData.sleep.Count() > 0)
                model.sleepCount = sleepData.sleep.Count();

            if (model.physicalActivityCount > 0 || model.weightCount > 0 || model.bpCount > 0 || model.glucCount > 0
                || model.nutritionCount > 0 || model.sleepCount > 0)
                model.isEmpty = false;
            return model;
        }

        public static GetWeightResponse GetWeightData(DateTime? startdate, DateTime? enddate, int participantId)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.StartDate = startdate;
            request.EndDate = enddate;
            return reader.GetWeightData(request);
        }

        public static GetBloodPressureResponse GetBloodPressureData(DateTime? startdate, DateTime? enddate, int participantId)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.StartDate = startdate;
            request.EndDate = enddate;
            return reader.GetBloodPressureData(request);
        }

        public static BiometricsData ListBiometricsData(int days, string startDate, string endDate, int participantId)
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
            GetBloodPressureResponse response = GetBloodPressureData(startDateFilter, endDateFilter, participantId);
            var biometric = response.BloodPressures.Select(x => new Biometric
            {
                timestamp = x.timestamp.ToShortDateString(),
                diastolic = x.diastolic,
                systolic = x.systolic
            }).ToList();
            BiometricsData biometricData = new BiometricsData();
            biometricData.SystolicRange1Value = Biometrics.SystolicRange1;
            biometricData.SystolicRange2Value = Biometrics.SystolicRange2;
            biometricData.SystolicRange3Value = Biometrics.SystolicRange3;

            biometricData.DiastolicRange1Value = Biometrics.DiastolicRange1;
            biometricData.DiastolicRange2Value = Biometrics.DiastolicRange2;
            biometricData.DiastolicRange3Value = Biometrics.DiastolicRange3;
            if (biometric.Count > 0)
            {
                biometricData.recentDate = Convert.ToDateTime(biometric[0].timestamp).ToString("MMM dd, yyyy");
                biometricData.SystolicCount = biometricData.DiastolicCount = biometric.Count();

                double systolicTotal = 0, diastolicTotal = 0;
                foreach (var biometricValue in biometric)
                {
                    if (biometricValue.systolic.HasValue && biometricValue.diastolic.HasValue)
                    {
                        biometricData.LatestDiastolic = biometricValue.diastolic;
                        biometricData.LatestSystolic = biometricValue.systolic;

                        systolicTotal = systolicTotal + biometricValue.systolic.Value;
                        diastolicTotal = diastolicTotal + biometricValue.diastolic.Value;

                        biometricData.SystolicMax = biometricValue.systolic.Value > biometricData.SystolicMax ? biometricValue.systolic.Value : biometricData.SystolicMax;
                        biometricData.SystolicMin = biometricData.SystolicMin == 0 || biometricValue.systolic.Value < biometricData.SystolicMin ? biometricValue.systolic.Value : biometricData.SystolicMin;

                        biometricData.DiastolicMax = biometricValue.diastolic > biometricData.DiastolicMax ? biometricValue.diastolic.Value : biometricData.DiastolicMax;
                        biometricData.DiastolicMin = biometricData.DiastolicMin == 0 || biometricValue.diastolic < biometricData.DiastolicMin ? biometricValue.diastolic.Value : biometricData.DiastolicMin;

                        if (biometricValue.systolic >= biometricData.SystolicRange3Value)
                            biometricData.SystolicRange3Count = biometricData.SystolicRange3Count + 1;
                        if (biometricValue.systolic >= biometricData.SystolicRange2Value)
                            biometricData.SystolicRange2Count = biometricData.SystolicRange2Count + 1;
                        if (biometricValue.systolic >= biometricData.SystolicRange1Value)
                            biometricData.SystolicRange1Count = biometricData.SystolicRange1Count + 1;

                        if (biometricValue.diastolic >= biometricData.DiastolicRange3Value)
                            biometricData.DiastolicRange3Count = biometricData.DiastolicRange3Count + 1;
                        if (biometricValue.diastolic >= biometricData.DiastolicRange2Value)
                            biometricData.DiastolicRange2Count = biometricData.DiastolicRange2Count + 1;
                        if (biometricValue.diastolic >= biometricData.DiastolicRange1Value)
                            biometricData.DiastolicRange1Count = biometricData.DiastolicRange1Count + 1;
                    }
                }
                biometricData.DiastolicValue = (int)Math.Round((diastolicTotal / biometricData.DiastolicCount), 0);
                biometricData.SystolicValue = (int)Math.Round((systolicTotal / biometricData.SystolicCount), 0);
                biometricData.graphData = biometric.Select(x => new GraphData3
                {
                    date = x.timestamp,
                    open = x.diastolic ?? 0,
                    close = x.systolic ?? 0
                }).ToList();
                biometricData.systolicGraph = biometric.Select(x => new GraphData1
                {
                    date = x.timestamp,
                    value = x.systolic ?? 0
                }).ToList();
                biometricData.diastolicGraph = biometric.Select(x => new GraphData1
                {
                    date = x.timestamp,
                    value = x.diastolic ?? 0
                }).ToList();
            }
            return biometricData;
        }

        public static GetSleepResponse GetSleepData(DateTime? startdate, DateTime? enddate, int participantId)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.StartDate = startdate;
            request.EndDate = enddate;
            return reader.GetSleepData(request);
        }

        public static GetSummariesResponse GetSummariesData(DateTime startDate, DateTime endDate, int participantId)
        {
            DeviceReader reader = new DeviceReader();
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            request.UserId = participantId;
            request.StartDate = startDate;
            request.EndDate = endDate;
            return reader.getSummariesData(request);
        }

        public static bool AddEditWeeklyGoals(int? stepsGoal, int? MIminGoal, int? VIminGoal, int participantId, int userId)
        {
            WeeklyGoalsDto weeklygoals = new WeeklyGoalsDto();
            weeklygoals.UserId = participantId;
            weeklygoals.stepsGoal = stepsGoal;
            weeklygoals.ModerateIntensityGoal = MIminGoal;
            weeklygoals.VigorousIntensityGoal = VIminGoal;
            weeklygoals.UpdatedBy = userId;
            weeklygoals.UpdatedOn = DateTime.UtcNow;
            DeviceReader reader = new DeviceReader();
            return reader.UpdateWeeklyGoals(weeklygoals);
        }

        public static GetWorkoutResponse GetWorkoutData(DateTime? startdate, DateTime? enddate, int participantId)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.StartDate = startdate;
            request.EndDate = enddate;
            return reader.GetWorkoutsData(request);
        }

        public static RoutineData ListPhysicalActivityData(int days, string startDate, string endDate, int participantId, string participantTimeZone)
        {
            DateTime? startDateFilter = null, endDateFilter = null;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(participantTimeZone);
            if (!string.IsNullOrEmpty(startDate) && !string.IsNullOrEmpty(endDate))
            {
                startDateFilter = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(startDate), custTZone);
                endDateFilter = TimeZoneInfo.ConvertTimeToUtc(Convert.ToDateTime(endDate), custTZone).AddDays(1);
                days = (int)(endDateFilter.Value - startDateFilter.Value).TotalDays;
            }
            else if (days > 0)
            {
                endDateFilter = TimeZoneInfo.ConvertTimeToUtc(TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, custTZone).Date, custTZone);
                startDateFilter = TimeZoneInfo.ConvertTimeToUtc(TimeZoneInfo.ConvertTimeFromUtc(endDateFilter.Value.AddDays(-days), custTZone).Date, custTZone);
            }
            var response = GetWorkoutData(startDateFilter, endDateFilter, participantId);
            if (response != null && response.Workouts != null && response.Workouts.Count() > 0)
            {
                foreach (var item in response.Workouts)
                {
                    item.starttimestamp = TimeZoneInfo.ConvertTimeFromUtc(item.starttimestamp, custTZone);
                    item.endtimestamp = TimeZoneInfo.ConvertTimeFromUtc(item.endtimestamp, custTZone);
                }
            }
            //Time data
            var timeData = response.Workouts.Where(x => x.duration > 0).GroupBy(y => y.starttimestamp.Date).Select(groupedData => new Fitness
            {
                timestamp = groupedData.Key.ToShortDateString(),
                dateTime = groupedData.Key,
                duration = groupedData.Sum(y => y.duration / 60),
                calories = groupedData.Sum(y => y.caloriesburned)
            }).ToList();
            //Step data
            var workoutSteps = response.Workouts.Where(x => x.distance > 0).Select(x => new Fitness
            {
                timestamp = x.starttimestamp.ToShortDateString(),
                dateTime = x.starttimestamp,
                distance = x.distance,
                calories = x.caloriesburned,
            }).ToList();

            var summaries = GetSummariesData(startDateFilter.Value, endDateFilter.Value, participantId);
            if (summaries != null && summaries.Summaries != null && summaries.Summaries.Count() > 0)
            {
                foreach (var item in summaries.Summaries)
                {
                    item.StartTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(item.StartTimeStamp, custTZone);
                    item.EndTimeStamp = TimeZoneInfo.ConvertTimeFromUtc(item.EndTimeStamp, custTZone);
                }
            }
            var summariesSteps = summaries.Summaries.Where(x => x.Steps != null && x.Steps > 0).Select(x => new Fitness
            {
                timestamp = x.StartTimeStamp.ToShortDateString(),
                dateTime = x.StartTimeStamp,
                distance = x.Steps,
                calories = x.CaloriesBurned,
            }).ToList();
            var stepData = workoutSteps.Concat(summariesSteps).GroupBy(y => y.dateTime.Date).Select(groupedData => new Fitness
            {
                timestamp = groupedData.Key.ToShortDateString(),
                dateTime = groupedData.Key,
                distance = groupedData.Sum(y => y.distance),
                calories = groupedData.Sum(y => y.calories)
            }).ToList();

            RoutineData routineData = new RoutineData();

            routineData.graphData = new List<GraphData2>();
            routineData.graphData1 = new List<GraphData2>();
            routineData.StepsRange1Value = RoutineRange.StepsRange1;
            routineData.StepsRange2Value = RoutineRange.StepsRange2;
            routineData.StepsRange3Value = RoutineRange.StepsRange3;
            routineData.StepsRange4Value = RoutineRange.StepsRange4;
            routineData.TimeRange1Value = RoutineRange.TimeRange1;
            routineData.TimeRange2Value = RoutineRange.TimeRange2;
            routineData.TimeRange3Value = RoutineRange.TimeRange3;

            routineData.StepsCount = stepData.Count();
            routineData.TimeCount = timeData.Count();
            routineData.avgTime = (int)Math.Round((timeData.Sum(x => x.duration.Value)) / Math.Abs(days));
            routineData.avgSteps = (int)Math.Round(stepData.Sum(x => x.distance.Value) / Math.Abs(days));
            double caloriesonWorkouts = response.Workouts.Where(x => x.distance > 0 || x.duration > 0).Sum(y => y.caloriesburned);
            double caloriesonSummaries = (double)summaries.Summaries.Where(x => x.Steps != null && x.Steps > 0).Sum(y => y.CaloriesBurned);
            routineData.avgCalories = (int)Math.Round((caloriesonWorkouts + caloriesonSummaries) / Math.Abs(days));


            //Daily Physical Activity for Steps
            routineData.dailyAvgSteps = routineData.avgSteps;
            if (stepData.Count > 0)
            {
                var daysWithoutSteps = days - stepData.Count;
                if (daysWithoutSteps <= 0)
                {
                    routineData.leastActiveDayValue = stepData.Min(x => x.distance ?? 0);
                    routineData.leastActiveDay = stepData.OrderBy(x => x.distance).FirstOrDefault().dateTime.ToString("MMMM d");
                }
                else
                {
                    if (daysWithoutSteps == 1)
                    {
                        var dateList = stepData.Select(x => x.dateTime);
                        var start = startDateFilter.Value;
                        var end = endDateFilter.Value;

                        var range = Enumerable.Range(0, (int)(end - start).TotalDays + 1)
                                              .Select(i => start.AddDays(i));

                        var totalList = range.Except(dateList).ToList<DateTime>().FirstOrDefault();
                        routineData.leastActiveDay = "0 steps on " + totalList.ToString("MMMM d") + "";
                    }
                    else
                        routineData.leastActiveDay = "0 steps on multiple days";
                }
                routineData.mostActiveDayValue = stepData.Max(x => x.distance ?? 0);
                routineData.mostActiveDay = stepData.OrderByDescending(x => x.distance).FirstOrDefault().dateTime.ToString("MMMM d");

            }

            foreach (var routineValue in stepData)
            {
                if (routineValue.distance > 0)
                {
                    if (routineValue.distance >= routineData.StepsRange4Value)
                        routineData.StepsRange4Count++;
                    if (routineValue.distance >= routineData.StepsRange3Value)
                        routineData.StepsRange3Count++;
                    if (routineValue.distance >= routineData.StepsRange2Value)
                        routineData.StepsRange2Count++;
                    if (routineValue.distance >= routineData.StepsRange1Value)
                        routineData.StepsRange1Count++;
                    routineData.graphData1.Add(new GraphData2 { date = routineValue.timestamp, value = (int)routineValue.distance });
                }
            }

            if (response.Workouts.Count > 0)
            {

                double weeklyduration = 0;

                var fitnessValueByWeek = timeData.GroupBy(x => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            Convert.ToDateTime(x.timestamp), CalendarWeekRule.FirstDay, DayOfWeek.Monday)).Select(g => new
                            {
                                timestamp = g.Key,
                                year = Convert.ToDateTime((g.Select(s => s.timestamp).FirstOrDefault())).Year,
                                Duration = g.Sum(s => s.duration),
                                date = Convert.ToDateTime((g.Select(s => s.timestamp).FirstOrDefault()))

                            });
                var test = timeData.GroupBy(x => CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(
                            Convert.ToDateTime(x.timestamp), CalendarWeekRule.FirstDay, DayOfWeek.Monday)).ToList();

                var date1 = startDateFilter.Value;
                var date2 = endDateFilter.Value;
                var currentCulture = CultureInfo.CurrentCulture;
                var weeks = new List<Tuple<int, int>>();

                for (var date = date1; date < date2; date = date.AddDays(1))
                {
                    var weekNo = currentCulture.Calendar.GetWeekOfYear(
                                          date,
                                          currentCulture.DateTimeFormat.CalendarWeekRule,
                                          currentCulture.DateTimeFormat.FirstDayOfWeek);
                    if (weeks.Where(x => x.Item1 == weekNo && x.Item2 == date.Year).Count() == 0)
                        weeks.Add(Tuple.Create(weekNo, date.Year));
                }
                var WeeksWithoutMin = weeks.Where(x => !fitnessValueByWeek.Any(y => y.timestamp == x.Item1 && y.year == x.Item2)).ToList();

                foreach (var routineValue in timeData)
                {

                    if (routineValue.duration >= routineData.TimeRange3Value)
                        routineData.TimeRange3Count++;
                    if (routineValue.duration >= routineData.TimeRange2Value)
                        routineData.TimeRange2Count++;
                    if (routineValue.duration >= routineData.TimeRange1Value)
                        routineData.TimeRange1Count++;
                    routineData.graphData.Add(new GraphData2 { date = routineValue.timestamp, value2 = (int)(routineValue.duration) });
                }
                //Weekly Physical Activity for Time
                foreach (var weeklydata in fitnessValueByWeek)
                {
                    weeklyduration += weeklydata.Duration ?? 0;
                }
                routineData.weeklyAvgTime = (int)Math.Round(weeklyduration / (Math.Abs(days) / 7));
                if (WeeksWithoutMin.Count() == 0)
                {
                    var leastActiveWeekTimeDay = FirstDateOfWeekISO8601(fitnessValueByWeek.OrderBy(x => x.Duration).FirstOrDefault().date.Year, fitnessValueByWeek.OrderBy(x => x.Duration).FirstOrDefault().timestamp);
                    routineData.leastActiveWeekTimenumber = leastActiveWeekTimeDay.ToString("MMM d") + " - " + leastActiveWeekTimeDay.AddDays(6).ToString("MMM d");
                    routineData.leastActiveWeekTime = Math.Round(fitnessValueByWeek.Min(x => x.Duration ?? 0), 1);
                }
                else
                {
                    if (WeeksWithoutMin.Count() == 1)
                    {
                        var leastActiveWeek = FirstDateOfWeekISO8601(WeeksWithoutMin.FirstOrDefault().Item2, WeeksWithoutMin.FirstOrDefault().Item1);
                        routineData.leastActiveWeekTimenumber = "0 min on " + leastActiveWeek.ToString("MMM d") + " - " + leastActiveWeek.AddDays(6).ToString("MMM d") + "";
                    }
                    else
                        routineData.leastActiveWeekTimenumber = "0 min on multiple weeks";
                }
                if (fitnessValueByWeek != null && fitnessValueByWeek.Count() > 0)
                {
                    routineData.mostActiveWeekTime = Math.Round(fitnessValueByWeek.Max(x => x.Duration ?? 0), 1);
                    var mostActiveWeekTimeDay = FirstDateOfWeekISO8601((fitnessValueByWeek.OrderByDescending(x => x.Duration).FirstOrDefault().date).Year, (fitnessValueByWeek.OrderByDescending(x => x.Duration).FirstOrDefault().timestamp));
                    routineData.mostActiveWeekTimenumber = mostActiveWeekTimeDay.ToString("MMM d") + " - " + mostActiveWeekTimeDay.AddDays(6).ToString("MMM d");
                }
            }
            return routineData;
        }

        public static GetNutritionResponse GetNutritionData(DateTime? startDate, DateTime? endDate, int page, int pageSize, int totalRecords, int participantId)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.StartDate = startDate;
            request.EndDate = endDate;
            request.page = page;
            request.pageSize = pageSize;
            request.totalRecords = totalRecords;
            return reader.GetNutritionData(request);
        }

        public static GetDiabetesResponse GetDiabetesData(DateTime? startDate, DateTime? endDate, int participantId, string timeZone)
        {
            GetDeviceDataRequest request = new GetDeviceDataRequest();
            DeviceReader reader = new DeviceReader();
            request.UserId = participantId;
            request.TimeZone = timeZone;
            request.StartDate = startDate;
            request.EndDate = endDate;
            return reader.GetDiabetesData(request);
        }

        public static GlucoseData ListGlucoseData(int days, string startDate, string endDate, int participantId, string timeZone, int hraId, string hraCompleteDate)
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
            var response = GetDiabetesData(startDateFilter, endDateFilter, participantId, timeZone);
            var glucoseData = response.glucose.Select(x => new Diabetes
            {
                blood_glucose = x.Value,
                timestamp = TimeZoneInfo.ConvertTimeFromUtc(x.EffectiveDateTime.Value, TimeZoneInfo.FindSystemTimeZoneById(timeZone)).ToString(),
                relationship_to_meal = FindGlucoseCodeName(x.Code)
            }).ToList();
            GlucoseData result = new GlucoseData();
            bool diabetes = false;
            if (!string.IsNullOrEmpty(hraCompleteDate))
                diabetes = result.diabetes = ReportUtility.ReadHRAGoals(hraId).hraGoals.Diabetes;
            if (response.glucose.Count() > 0)
                result.recentDate = Convert.ToDateTime(response.glucose[0].DateTime).ToString("MMM dd");
            else
                result.recentDate = null;
            //average
            GlucometerData glucometerData = new GlucometerData();

            glucometerData.Range1Value = DeviceGlucoseRange.PostMealRange1;
            glucometerData.Range2Value = DeviceGlucoseRange.PostMealRange2;
            if (diabetes)
            {

                glucometerData.Range3Value = DeviceGlucoseRange.GlucoseRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.GlucoseRange4;
            }
            else
            {
                glucometerData.Range3Value = DeviceGlucoseRange.NonDiabetesGlucoseRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.NonDiabetesGlucoseRange4;
            }
            glucometerData.Count = glucoseData.Count;
            if (glucoseData.Count > 0)
            {
                glucometerData.dateTo = glucoseData.OrderByDescending(x => Convert.ToDateTime(x.timestamp)).FirstOrDefault().timestamp;
                //for Blood Glucose()
                var preglucoseValueByDay = glucoseData.Where(x => x.relationship_to_meal.Contains("Pre-meal")).Select(g => new GraphData6
                {
                    x = g.timestamp,
                    ay = (double)g.blood_glucose,
                    aValue = g.blood_glucose.ToString()
                }).ToList();
                var postglucoseValueByDay = glucoseData.Where(x => x.relationship_to_meal.Contains("Post-meal")).Select(g => new GraphData6
                {
                    x = g.timestamp,
                    by = (double)g.blood_glucose,
                    bValue = g.blood_glucose.ToString()
                }).ToList();
                var untaggedValueByDay = glucoseData.Where(x => x.relationship_to_meal.Equals("BG")).Select(g => new GraphData6
                {
                    x = g.timestamp,
                    cy = (double)g.blood_glucose,
                    cValue = g.blood_glucose.ToString()
                }).ToList();
                var glucoseValueByDay = new List<GraphData6>();
                glucoseValueByDay.AddRange(preglucoseValueByDay);
                glucoseValueByDay.AddRange(postglucoseValueByDay);
                glucoseValueByDay.AddRange(untaggedValueByDay);
                result.graphData = glucoseValueByDay.OrderBy(x => Convert.ToDateTime(x.x)).ToList();
                var bloodGlucose = glucoseData;
                if (bloodGlucose.Count > 0)
                {
                    glucometerData.ChecksperDay = (float)Math.Round((bloodGlucose.Count / (float)Math.Abs(days)), 2);
                    glucometerData.NameLI = "L2764";
                    glucometerData.Max = (int)bloodGlucose.Max(x => x.blood_glucose);
                    glucometerData.Min = (int)bloodGlucose.Min(x => x.blood_glucose);
                    glucometerData.Value = (int)Math.Round(bloodGlucose.Average(x => x.blood_glucose), 0);

                    glucometerData.Range1Count = bloodGlucose.Where(x => x.blood_glucose < glucometerData.Range1Value).Count();
                    glucometerData.Range2Count = bloodGlucose.Where(x => x.blood_glucose < glucometerData.Range2Value).Count();
                    glucometerData.Range3Count = bloodGlucose.Where(x => x.blood_glucose > glucometerData.Range3Value).Count();
                    glucometerData.Range4Count = bloodGlucose.Where(x => x.blood_glucose > glucometerData.Range4Value).Count();
                }
            }
            result.average = glucometerData;
            //pre meal
            glucometerData = new GlucometerData();
            glucometerData.Range1Value = DeviceGlucoseRange.PreMealRange1;
            glucometerData.Range2Value = DeviceGlucoseRange.PreMealRange2;
            if (diabetes)
            {
                glucometerData.Diabetic = "True";
                glucometerData.Range3Value = DeviceGlucoseRange.PreMealRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.PreMealRange4;
                glucometerData.preMealGoalTo = DeviceGlucoseRange.PreMealGoalTo;
                glucometerData.preMealGoalFrom = DeviceGlucoseRange.PreMealGoalFrom;
            }
            else
            {
                glucometerData.Range3Value = DeviceGlucoseRange.NonDiabetesPreMealRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.NonDiabetesPreMealRange4;
                glucometerData.preMealGoalTo = DeviceGlucoseRange.NonDiabetesPreMealGoalTo;
                glucometerData.preMealGoalFrom = DeviceGlucoseRange.NonDiabetesPreMealGoalFrom;
            }
            var premealGlucose = glucoseData.Where(x => x.relationship_to_meal.Contains("Pre-meal")).ToList();
            glucometerData.Count = premealGlucose.Count;
            if (premealGlucose.Count > 0)
            {
                glucometerData.ChecksperDay = (float)Math.Round((premealGlucose.Count / (float)Math.Abs(days)), 2);
                glucometerData.NameLI = "L2765";
                glucometerData.Max = (int)premealGlucose.Max(x => x.blood_glucose);
                glucometerData.Min = (int)premealGlucose.Min(x => x.blood_glucose);
                glucometerData.Value = (int)Math.Round(premealGlucose.Average(x => x.blood_glucose), 0);
                glucometerData.Range1Count = premealGlucose.Where(x => x.blood_glucose < glucometerData.Range1Value).Count();
                glucometerData.Range2Count = premealGlucose.Where(x => x.blood_glucose < glucometerData.Range2Value).Count();
                glucometerData.Range3Count = premealGlucose.Where(x => x.blood_glucose > glucometerData.Range3Value).Count();
                glucometerData.Range4Count = premealGlucose.Where(x => x.blood_glucose > glucometerData.Range4Value).Count();
                glucometerData.GoalsCount = premealGlucose.Where(x => x.blood_glucose >= glucometerData.preMealGoalFrom && x.blood_glucose <= glucometerData.preMealGoalTo).Count();
            }
            result.premeal = glucometerData;
            // For Post-meal BG
            glucometerData = new GlucometerData();
            glucometerData.Range1Value = DeviceGlucoseRange.PostMealRange1;
            glucometerData.Range2Value = DeviceGlucoseRange.PostMealRange2;
            if (diabetes)
            {
                glucometerData.Diabetic = "True";
                glucometerData.Range3Value = DeviceGlucoseRange.PostMealRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.PostMealRange4;
                glucometerData.postMealGoalFrom = DeviceGlucoseRange.PostMealGoalFrom;
                glucometerData.postMealGoalTo = DeviceGlucoseRange.PostMealGoalTo;
            }
            else
            {
                glucometerData.Range3Value = DeviceGlucoseRange.NonDiabetesPostMealRange3;
                glucometerData.Range4Value = DeviceGlucoseRange.NonDiabetesPostMealRange4;
                glucometerData.postMealGoalFrom = DeviceGlucoseRange.NonDiabetesPostMealGoalFrom;
                glucometerData.postMealGoalTo = DeviceGlucoseRange.NonDiabetesPostMealGoalTo;
            }
            var postmealGlucose = glucoseData.Where(x => x.relationship_to_meal.Contains("Post-meal")).ToList();
            glucometerData.Count = postmealGlucose.Count;
            if (postmealGlucose.Count > 0)
            {
                glucometerData.NameLI = "L2766";
                glucometerData.ChecksperDay = (float)Math.Round((postmealGlucose.Count / (float)Math.Abs(days)), 2);
                glucometerData.Max = (int)postmealGlucose.Max(x => x.blood_glucose);
                glucometerData.Min = (int)postmealGlucose.Min(x => x.blood_glucose);
                glucometerData.Value = (int)Math.Round(postmealGlucose.Average(x => x.blood_glucose), 0);
                glucometerData.Range1Count = postmealGlucose.Where(x => x.blood_glucose < glucometerData.Range1Value).Count();
                glucometerData.Range2Count = postmealGlucose.Where(x => x.blood_glucose < glucometerData.Range2Value).Count();
                glucometerData.Range3Count = postmealGlucose.Where(x => x.blood_glucose > glucometerData.Range3Value).Count();
                glucometerData.Range4Count = postmealGlucose.Where(x => x.blood_glucose > glucometerData.Range4Value).Count();
                glucometerData.GoalsCount = postmealGlucose.Where(x => x.blood_glucose >= glucometerData.postMealGoalFrom && x.blood_glucose <= glucometerData.postMealGoalTo).Count();
            }
            result.postmeal = glucometerData;
            return result;
        }
        public static string FindGlucoseCodeName(string code)
        {
            if (code.Contains("1521-4"))
                return ("Post-meal BG");
            else if (code.Contains("53049-3") || code.Contains("1558-6"))
                return ("Pre-meal BG");
            else
                return "BG";
        }

        public static byte? GetGender(int participantId)
        {
            byte? gender;
            DeviceReader reader = new DeviceReader();
            gender = reader.GetGender(participantId);

            return gender;
        }


    }
}