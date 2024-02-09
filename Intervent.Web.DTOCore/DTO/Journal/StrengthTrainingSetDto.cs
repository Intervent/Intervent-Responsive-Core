namespace Intervent.Web.DTO
{
    public class StrengthTrainingSetDto
    {
        public int Id { get; set; }
        public int TrainingLogId { get; set; }
        public int? TrainingSet { get; set; }
        public float? Weight { get; set; }
        public int? Repetitions { get; set; }
        public byte? RPE { get; set; }
    }
}
