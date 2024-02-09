namespace Intervent.Web.DTO
{
    public class QuizinStepDto
    {
        public int Id { get; set; }

        public int StepId { get; set; }

        public string QuizText { get; set; }

        public byte QuizType { get; set; }

        public string QuizTypeText { get; set; }

        public string Value { get; set; }

        public virtual IList<OptionsforQuizDto> optionsforQuiz { get; set; }

        public bool IsActive { get; set; }

        public string TextLangItemCode { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }
    }
}
