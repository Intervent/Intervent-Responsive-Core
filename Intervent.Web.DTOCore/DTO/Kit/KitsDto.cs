namespace Intervent.Web.DTO
{
    public class KitsDto
    {
        public int Id { get; set; }

        public string InvId { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string KeyConcepts { get; set; }

        public int Topic { get; set; }

        public string Audio { get; set; }

        public string Pdf { get; set; }

        public bool? Active { get; set; }

        public DateTime? PublishedDate { get; set; }

        public DateTime? LastUpdated { get; set; }

        public IList<StepsinKitsDto> StepsinKits { get; set; }

        public IList<PromptDto> PromptsinKits { get; set; }

        public KitTopicsDto KitTopic { get; set; }

        public IList<PromptsinKitsCompletedDto> PromptsinKitsCompleted { get; set; }

        public virtual IList<KitTranslationDto> KitTranslations { get; set; }
    }
}