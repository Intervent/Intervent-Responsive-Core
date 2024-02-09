namespace Intervent.Web.DTO
{
    public class AddEditOptionsforQuizResponse
    {
        public bool success { get; set; }

        public IList<OptionsforQuizDto> optionsforQuiz { get; set; }
    }
}
