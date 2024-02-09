namespace Intervent.Web.DTO
{
    public class OptionsforQuizDto
    {
        public int Id { get; set; }

        public int QuizId { get; set; }

        public string OptionText { get; set; }

        public bool IsDefault { get; set; }

        public bool IsActive { get; set; }

        public string TextLangItemCode { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }
    }
}
