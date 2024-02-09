namespace Intervent.Web.DTO
{
    public class ListExerciseDairyResponse
    {
        public IList<ExerciseDiaryDto> ExcerciseDiary { get; set; }

        public IList<ExerciseDiaryDto> ExcerciseDiaries { get; set; }

        public int totalRecords { get; set; }
    }
}
