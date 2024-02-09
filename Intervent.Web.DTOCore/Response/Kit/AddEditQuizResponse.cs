namespace Intervent.Web.DTO
{
    public class AddEditQuizResponse
    {
        public bool success { get; set; }

        public int QuizId { get; set; }

        public IList<QuizinStepDto> quizinKit { get; set; }
    }
}
