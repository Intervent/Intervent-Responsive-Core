namespace Intervent.Web.DTO
{
    public class OptionsforActivityQuestionDto
    {
        public int Id { get; set; }

        public int QuestionId { get; set; }

        public string OptionText { get; set; }

        public bool? IsAnswer { get; set; }

        public bool? IsActive { get; set; }

        public int? Points { get; set; }

        public short? SequenceNo { get; set; }

        public string TextLangItemCode { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }
    }
}
