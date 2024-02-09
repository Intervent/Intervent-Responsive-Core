namespace Intervent.Web.DTO
{
    public class StrengthTrainingLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public System.DateTime Date { get; set; }

        public string MuscleGroup { get; set; }

        public int? TrainingTypeId { get; set; }

        public string Notes { get; set; }

        public IList<StrengthTrainingSetDto> StrengthTrainingLogSet { get; set; }

        public StrengthTrainingTypeDto StrengthTrainingType { get; set; }
    }
}
