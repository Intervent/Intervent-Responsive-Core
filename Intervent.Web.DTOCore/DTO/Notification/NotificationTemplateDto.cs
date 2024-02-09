namespace Intervent.Web.DTO
{
    public class NotificationTemplateDto
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public int NotificationEventTypeId { get; set; }
        public bool Active { get; set; }
        public int TemplateRendererId { get; set; }
        public bool DataPacketRequired { get; set; }
        public string EmailFrom { get; set; }
        public string EmailSubject { get; set; }
        public string NotificationContactMethodId { get; set; }
        public string TemplateSource { get; set; }
        public string LastUpdatedUser { get; set; }
        public DateTime LastUpdatedDate { get; set; }
        public bool HasAttachment { get; set; }
        public IList<NotificationTemplateTranslationDto> NotificationTemplateTranslations { get; set; }
    }
}
