namespace Intervent.Web.DTO
{
    public class AddEditTrainingLogRequest
    {
        public StrengthTrainingLogDto trainingLog { get; set; }

        public IList<StrengthTrainingSetDto> trainingLogSet { get; set; }

    }
}
