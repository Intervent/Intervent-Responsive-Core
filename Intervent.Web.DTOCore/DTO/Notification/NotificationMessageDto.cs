namespace Intervent.Web.DTO
{
    public class NotificationMessageDto
    {
        public bool isBodyHtml { get; set; }

        public long? Id { get; set; }

        public long NotificationEventId { get; set; }

        public string FromAddress { get; set; }

        public string ToAddress { get; set; }

        public string MessageBody { get; set; }

        public string BccAddress { get; set; }

        public string CcAddress { get; set; }

        public string Subject { get; set; }

        public string Attachment { get; set; }

        public string SubjectLine { get; set; }

        public string Date { get; set; }

    }
}
