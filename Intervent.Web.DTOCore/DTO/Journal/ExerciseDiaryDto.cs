namespace Intervent.Web.DTO
{
    public class ExerciseDiaryDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public int Type { get; set; }

        public int? Duration { get; set; }

        public byte? RPE { get; set; }

        public short? HeartRate { get; set; }

        public int? Points { get; set; }

        public int? StepsPerDay { get; set; }

        public string Notes { get; set; }

        public ExerciseTypeDto ExerciseType { get; set; }
    }
}
