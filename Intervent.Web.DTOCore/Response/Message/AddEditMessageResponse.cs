namespace Intervent.Web.DTO
{
    public class AddEditMessageRequest
    {
        public int? messageId { get; set; }

        public int? parentMessageId { get; set; }

        public int userId { get; set; }

        public int recipientId { get; set; }

        public string subject { get; set; }

        public string messageBody { get; set; }

        public string attachement { get; set; }

        public bool isSent { get; set; }
    }
}
