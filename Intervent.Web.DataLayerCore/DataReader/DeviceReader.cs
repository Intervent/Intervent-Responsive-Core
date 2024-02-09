using Intervent.DAL;
using Intervent.Web.DTO;
using Microsoft.EntityFrameworkCore;

namespace Intervent.Web.DataLayer
{
    public class DeviceReader
    {
        private InterventDatabase context = new InterventDatabase(InterventDatabase.GetInterventDatabaseOption());

        public GetSummariesResponse getSummariesData(GetDeviceDataRequest request)
        {
            GetSummariesResponse response = new GetSummariesResponse();
            var summariesDAL = context.EXT_Summaries.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.EndTimeStamp.Date >= request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.EndTimeStamp.Date <= request.EndDate.Value.Date)).OrderByDescending(x => x.EndTimeStamp).ToList();
            response.Summaries = Utility.mapper.Map<List<DAL.EXT_Summaries>, List<EXT_SummariesDto>>(summariesDAL);
            return response;
        }

        public bool UpdateWeeklyGoals(WeeklyGoalsDto weeklygoals)
        {
            var CurrentDAL = context.Workout_Goals.Where(x => x.UserId == weeklygoals.UserId).FirstOrDefault();
            if (CurrentDAL != null)
            {
                CurrentDAL.UserId = weeklygoals.UserId;
                CurrentDAL.stepsGoal = weeklygoals.stepsGoal;
                CurrentDAL.UpdatedBy = weeklygoals.UpdatedBy;
                CurrentDAL.UpdatedOn = weeklygoals.UpdatedOn;
                CurrentDAL.ModerateIntensityGoal = weeklygoals.ModerateIntensityGoal;
                CurrentDAL.VigorousIntensityGoal = weeklygoals.VigorousIntensityGoal;
                context.Workout_Goals.Attach(CurrentDAL);
                context.Entry(CurrentDAL).State = EntityState.Modified;
            }
            else
            {
                CurrentDAL = Utility.mapper.Map<WeeklyGoalsDto, WorkoutGoals>(weeklygoals);
                context.Workout_Goals.Add(CurrentDAL);
            }
            context.SaveChanges();
            return true;

        }

        public GetWeightResponse GetWeightData(GetDeviceDataRequest request)
        {
            GetWeightResponse response = new GetWeightResponse();
            var weights = context.EXT_Weights.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.TimeStamp.Date >= request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.TimeStamp.Date <= request.EndDate.Value.Date)).OrderByDescending(x => x.TimeStamp).ToList();
            response.Weights = Utility.mapper.Map<List<DAL.EXT_Weights>, List<EXT_WeightDto>>(weights);
            return response;
        }

        public GetBloodPressureResponse GetBloodPressureData(GetDeviceDataRequest request)
        {
            GetBloodPressureResponse response = new GetBloodPressureResponse();
            var bloodpressure = context.EXT_BloodPressures.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.TimeStamp.Date >= request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.TimeStamp.Date <= request.EndDate.Value.Date)).OrderBy(x => x.TimeStamp).ToList();
            response.BloodPressures = Utility.mapper.Map<List<DAL.EXT_BloodPressures>, List<EXT_BloodPressureDto>>(bloodpressure);
            return response;
        }

        public GetWorkoutResponse GetWorkoutsData(GetDeviceDataRequest request)
        {
            GetWorkoutResponse response = new GetWorkoutResponse();
            var Workoutss = context.EXT_Workouts.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.StartTimeStamp.Value.Date >= request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.StartTimeStamp.Value.Date <= request.EndDate.Value.Date)).OrderBy(x => x.StartTimeStamp).ToList();
            response.Workouts = Utility.mapper.Map<List<DAL.EXT_Workouts>, List<EXT_WorkoutsDto>>(Workoutss);
            return response;
        }

        public GetSleepResponse GetSleepData(GetDeviceDataRequest request)
        {
            GetSleepResponse response = new GetSleepResponse();
            var Sleeps = context.EXT_Sleeps.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.StartTimeStamp.Date >= request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.StartTimeStamp.Date <= request.EndDate.Value.Date)).OrderBy(x => x.StartTimeStamp).ToList();
            response.sleep = Utility.mapper.Map<List<DAL.EXT_Sleeps>, List<EXT_SleepsDto>>(Sleeps);
            return response;
        }

        public GetNutritionResponse GetNutritionData(GetDeviceDataRequest request)
        {
            GetNutritionResponse response = new GetNutritionResponse();
            var totalRecords = request.totalRecords.HasValue ? request.totalRecords.Value : 0;
            var nutritionsList = context.EXT_Nutrition.Where(x => x.UserId == request.UserId && (!request.StartDate.HasValue || x.TimeStamp.Value.Date > request.StartDate.Value.Date) && (!request.EndDate.HasValue || x.TimeStamp.Value.Date <= request.EndDate.Value.Date)).OrderByDescending(x => x.TimeStamp).ToList();
            if (totalRecords == 0)
            {
                totalRecords = nutritionsList.Count();
            }
            var nutritions = nutritionsList.Skip(request.page.Value * request.pageSize.Value).OrderByDescending(x => x.TimeStamp).Take(request.pageSize.Value).ToList();
            response.nutritionList = Utility.mapper.Map<List<DAL.EXT_Nutrition>, List<EXT_NutritionDto>>(nutritionsList);
            response.nutrition = Utility.mapper.Map<List<DAL.EXT_Nutrition>, List<EXT_NutritionDto>>(nutritions);
            response.totalRecords = totalRecords;
            return response;
        }

        public GetDiabetesResponse GetDiabetesData(GetDeviceDataRequest request)
        {
            GetDiabetesResponse response = new GetDiabetesResponse();
            DateTime? startDate = null, endDate = null;
            TimeZoneInfo custTZone = TimeZoneInfo.FindSystemTimeZoneById(request.TimeZone);
            if (request.StartDate.HasValue)
            {
                if (request.StartDate.Value.Kind != DateTimeKind.Utc)
                {
                    startDate = TimeZoneInfo.ConvertTimeToUtc(request.StartDate.Value, custTZone);
                }
                else
                    startDate = request.StartDate;
            }
            if (request.EndDate.HasValue)
            {
                if (request.EndDate.Value.Kind != DateTimeKind.Utc)
                {
                    endDate = TimeZoneInfo.ConvertTimeToUtc(request.EndDate.Value, custTZone);
                }
                else
                    endDate = request.EndDate;
            }
            var glucose = context.EXT_Glucose.Where(x => x.UserId == request.UserId && (!x.IsValid.HasValue || x.IsValid.Value) && (!startDate.HasValue || x.EffectiveDateTime.Value.Date >= startDate.Value.Date) && (!endDate.HasValue || x.EffectiveDateTime.Value.Date <= endDate.Value.Date)).OrderByDescending(x => x.DateTime).ToList();
            response.glucose = Utility.mapper.Map<List<DAL.EXT_Glucose>, List<EXT_GlucoseDto>>(glucose);
            return response;
        }

        public bool HasGlucoseData(int userId)
        {
            return context.EXT_Glucose.Where(x => x.UserId == userId).Count() > 0;
        }

        public byte? GetGender(int UserId)
        {
            var UserDetails = context.Users.Where(x => x.Id == UserId).FirstOrDefault();
            byte? g;
            g = UserDetails.Gender.HasValue ? UserDetails.Gender : 0;
            return g;
        }
    }
}