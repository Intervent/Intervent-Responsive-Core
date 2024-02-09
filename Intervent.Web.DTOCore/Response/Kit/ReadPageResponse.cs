namespace Intervent.Web.DTO
{
    public class ReadPageResponse
    {
        public StepsinKitsDto Step { get; set; }

        public string KeyConcepts { get; set; }

        public IList<QuizinStepDto> Quiz { get; set; }

        public IList<PromptDto> Prompts { get; set; }

        public IList<PromptsinKitsCompletedDto> PromptsinKitsCompleted { get; set; }

        public int kitId { get; set; }

        public string pageIdentifier { get; set; }

        public string languageCode { get; set; }

        public int KitsInUserProgramId { get; set; }

        public KitsinUserProgramGoalDto KitsinUserProgramGoal { get; set; }
    }
}
