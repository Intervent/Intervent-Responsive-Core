namespace Intervent.Web.DTO
{
    public class StepsinKitsDto
    {
        public int Id { get; set; }

        public int KitId { get; set; }

        public string Name { get; set; }

        public string StepNo { get; set; }

        public string Text { get; set; }

        public IList<ActivitiesinStepsDto> ActivitiesinSteps { get; set; }

        public IList<QuizinStepDto> QuizInSteps { get; set; }

        public bool IsSubStep { get; set; }

        public bool IsActive { get; set; }

        public bool IsAppendix { get; set; }

        public bool IsGoal { get; set; }

        public List<LanguageItemDto> TextLanguageValue { get; set; }

        public List<LanguageItemDto> NameLanguageValue { get; set; }
    }
}
