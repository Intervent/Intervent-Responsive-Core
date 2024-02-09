namespace Intervent.Web.DTO
{
    public class AddEditQuestionResponse
    {
        public bool success { get; set; }

        public int QuestionId { get; set; }

        public IList<QuestionsinActivityDto> questionsinActivity { get; set; }

        public IList<PassiveQuestionsInActivitiesDto> passiveQuestions { get; set; }
    }
}
