namespace Intervent.Web.DTO
{
    public class KitTranslationDto
    {
        public int KitId { get; set; }
        public string LanguageCode { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string KeyConcepts { get; set; }
        public string Pdf { get; set; }
        public string Audio { get; set; }
        public DateTime? PublishedDate { get; set; }
        public DateTime? LastUpdated { get; set; }
    }
}
