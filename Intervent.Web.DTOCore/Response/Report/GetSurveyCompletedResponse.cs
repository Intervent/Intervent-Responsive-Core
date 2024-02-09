namespace Intervent.Web.DTO
{
    public class GetSurveyCompletedResponse
    {
        public bool SurveyCompleted { set; get; }

        public bool PartiallyCompleted { set; get; }

        public List<SurveyResponseDto> surveyResponse { get; set; }

        public string Comments { get; set; }
    }
}
