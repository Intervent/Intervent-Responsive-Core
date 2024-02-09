namespace Intervent.Web.DTO
{
    public class ReadSurveyResponse
    {
        public List<SurveyQuestionDto> SurveyQuestions { get; set; }

        public List<SurveyResponseDto> SurveyResponse { get; set; }

        public string Comments { get; set; }
    }
}
