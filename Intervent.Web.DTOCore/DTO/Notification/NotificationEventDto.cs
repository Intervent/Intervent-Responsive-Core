namespace Intervent.Web.DTO
{
    public class NotificationEventDto
    {
        public long? Id { get; set; }

        public int NotificationEventTypeId { get; set; }

        public int NotificationTemplateId { get; set; }

        public int NotificationStatusId { get; set; }

        public string DataPacket { get; set; }

        public int UserId { get; set; }

        public DateTime NotificationEventDate { get; set; }

        public string Subject { get; set; }

        public string FromEmailAddress { get; set; }

        public string ToEmailAddress { get; set; }

        public string CcAddress { get; set; }

        public string BccAddress { get; set; }

        public string Attachment { get; set; }

        public int? PortalId { get; set; }

        public string UniqueId { get; set; }

        public NotificationEventTypeDto NotificationEventType { get; set; }

        public NotificationTemplateDto NotificationTemplate { get; set; }

        public int OrganizationId { get; set; }

        public int From { get; set; }
        public int To { get; set; }
    }
}
