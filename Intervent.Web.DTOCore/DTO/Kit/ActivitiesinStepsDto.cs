namespace Intervent.Web.DTO
{
    public class ActivitiesinStepsDto
    {
        public int Id { get; set; }

        public int StepId { get; set; }

        public string TopText { get; set; }

        public string BottomText { get; set; }

        public bool WithinStep { get; set; }

        public bool AllowUpdate { get; set; }

        public virtual IList<QuestionsinActivityDto> QuestionsinActivities { get; set; }

        public virtual IList<PassiveQuestionsInActivitiesDto> PassiveQuestionsInActivities { get; set; }

        public int SequenceNo { get; set; }

        public bool IsActive { get; set; }

        public List<LanguageItemDto> TopTextLanguageValue { get; set; }

        public List<LanguageItemDto> BottomTextLanguageValue { get; set; }
    }
}
