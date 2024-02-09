namespace Intervent.Web.DTO
{
    public class PassiveQuestionsInActivitiesDto
    {
        public int ActivityId { get; set; }

        public int QuestionId { get; set; }

        public short SequenceNo { get; set; }

        public string QuestionText { get; set; }

        public bool IsActive { get; set; }

        public string TextLangItemCode { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }
    }
}
