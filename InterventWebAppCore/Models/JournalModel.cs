using Intervent.Web.DTO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace InterventWebApp
{
    public class ExerciseModel
    {
        public IEnumerable<SelectListItem> ExerciseTypes { get; set; }

        public int ExerciseType { get; set; }

        public IEnumerable<SelectListItem> RPES { get; set; }

        public int RPE { get; set; }

        public int ExerciseDiaryAccess { get; set; }

        public short? shortTermPoints { get; set; }

        public short? longTermPoints { get; set; }

        public int? shortTermMinutes { get; set; }

        public int? longTermMinutes { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }

        public string TimeZone { get; set; }
    }

    public class JournalModel
    {
        public int BoosterJournalAccess { get; set; }

        public int ExerciseDiaryAccess { get; set; }

        public int FoodDiaryAccess { get; set; }

        public int TobaccoLogAccess { get; set; }

        public int StrengthTrainingLogAccess { get; set; }

        public int StressLogAccess { get; set; }

        public int StressManagementLogAccess { get; set; }

        public int sleepLogAcess { get; set; }
    }
    public class WeightLossDiaryModel
    {
        public string WeightText { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }

    }
    public class StressDiaryModel
    {
        public StressDiaryDto stressDiary { get; set; }

        public IEnumerable<SelectListItem> MealTypes { get; set; }

        public IEnumerable<SelectListItem> FoodGroups { get; set; }

        public int FoodDiaryAccess { get; set; }
    }

    public class FoodDiaryModel
    {
        public IEnumerable<SelectListItem> MealTypes { get; set; }

        public IEnumerable<SelectListItem> FoodGroups { get; set; }

        public int FoodDiaryAccess { get; set; }

        public string DateFormat { get; set; }

        public bool HasActivePortal { get; set; }

    }

    public class FoodDiaryDetailsModel
    {
        public IList<FoodDiaryDto> FoodDiaryList { get; set; }

        public FoodDiaryDto FoodDiary { get; set; }

        public int FoodDiaryAccess { get; set; }

        public string startDate { get; set; }

        public string endDate { get; set; }

        public DateTime startDateTime { get; set; }

        public DateTime endDateTime { get; set; }

        public bool mousehover { get; set; }

        public IList<string> fg1RecentItems { get; set; }

        public IList<string> fg2RecentItems { get; set; }

        public IList<string> fg3RecentItems { get; set; }

        public IList<string> fg4RecentItems { get; set; }

        public IList<string> fg5RecentItems { get; set; }

        public IList<string> fg6RecentItems { get; set; }

        public string foodDate { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }
    }

    public class FoodDiaryListModel
    {
        public string startDate { get; set; }

        public string endDate { get; set; }

        public DateTime startDateTime { get; set; }

        public DateTime endDateTime { get; set; }

        public int FoodDiaryAccess { get; set; }

        public bool mousehover { get; set; }
    }

    public class DiaryListModel
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int? Days { get; set; }
    }

    public class TobaccoLogModel
    {
        public TobaccoLogDto tobaccoLog { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }
    }

    public class StrengthTrainingLogModel
    {
        public string WeightText { get; set; }

        public StrengthTrainingLogDto trainingLog { get; set; }

        public IList<StrengthTrainingSetDto> trainingLogSet { get; set; }

        public IEnumerable<SelectListItem> RPE { get; set; }

        public IEnumerable<SelectListItem> TrainingSetList { get; set; }

        public IEnumerable<SelectListItem> TrainingTypes { get; set; }

        public IEnumerable<SelectListItem> FoodGroups { get; set; }

        public string DateFormat { get; set; }

        public bool HasActivePortal { get; set; }

        public string LanguagePreference { get; set; }
    }

    public class StrengthTrainingListModel
    {
        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }
    }

    public class StressManagementLogModel
    {
        public StressManagementLogDto streesLog { get; set; }

        public IEnumerable<SelectListItem> StressRatingList { get; set; }

        public bool HasActivePortal { get; set; }

        public string DateFormat { get; set; }
    }

    public class SleepLogModel
    {
        public SleepLogDto sleepLog { get; set; }

        public IEnumerable<SelectListItem> sleeplogList { get; set; }

        public double sleptHours { get; set; }

        public string DateFormat { get; set; }

        public bool HasActivePortal { get; set; }
    }

    public class DailyVitalsLogModel
    {
        public bool HasActivePortal { get; set; }
    }

    public class VitalsModel
    {
        public VitalsLogDto DailyVitals { get; set; }

        public int Gender { get; set; }

        public IList<MeasurementsDto> Measurements { get; set; }

    }
}