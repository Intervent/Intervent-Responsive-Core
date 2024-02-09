namespace Intervent.Web.DTO
{
    public class GetWeightResponse
    {
        public List<EXT_WeightDto> Weights { get; set; }
    }

    public class GetBloodPressureResponse
    {
        public List<EXT_BloodPressureDto> BloodPressures { get; set; }
    }

    public class GetSleepResponse
    {
        public List<EXT_SleepsDto> sleep { get; set; }
    }

    public class GetWorkoutResponse
    {
        public List<EXT_WorkoutsDto> Workouts { get; set; }
    }

    public class GetSummariesResponse
    {
        public List<EXT_SummariesDto> Summaries { get; set; }
    }

    public class GetFoodResponse
    {
        public List<EXT_NutritionDto> Foods { get; set; }
    }

    public class GetNutritionResponse
    {
        public List<EXT_NutritionDto> nutrition { get; set; }

        public List<EXT_NutritionDto> nutritionList { get; set; }

        public int totalRecords { get; set; }
    }
    public class GetDiabetesResponse
    {
        public List<EXT_GlucoseDto> glucose { get; set; }
    }

}
