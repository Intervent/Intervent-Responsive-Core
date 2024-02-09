namespace Intervent.Web.DTO
{
    public class QuestionsinActivityDto
    {
        public int Id { get; set; }

        public int ActivityId { get; set; }

        public string QuestionText { get; set; }

        public byte QuestionType { get; set; }

        public string QuestionTypeText { get; set; }

        public string Value { get; set; }

        public virtual IList<OptionsforActivityQuestionDto> OptionsforActivityQuestions { get; set; }

        public short SequenceNo { get; set; }

        public bool IsActive { get; set; }

        public bool IsRequired { get; set; }

        public bool ShowVertical { get; set; }

        public bool IsPassive { get; set; }

        public string TextLangItemCode { get; set; }

        public int? ParentId { get; set; }

        public List<LanguageItemDto> LanguageTextValue { get; set; }

        public QuestionsinActivityDto() { }

        public QuestionsinActivityDto(PassiveQuestionsInActivitiesDto passive)
        {
            IsPassive = true;
            this.IsActive = passive.IsActive;
            this.Id = passive.QuestionId;
            this.ActivityId = passive.ActivityId;
            this.SequenceNo = passive.SequenceNo;
            this.QuestionText = passive.QuestionText;
        }
    }
}
