namespace Intervent.Web.DTO
{
    public class AssignMotivationMessagesRequest
    {
        public int MessageId { get; set; }

        public int? AssignedMessageId { get; set; }

        public string OrganizationIds { get; set; }

        public string MessageTypes { get; set; }

        public bool isRemove { get; set; }
    }
}
