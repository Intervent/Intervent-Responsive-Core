namespace Intervent.Web.DTO
{
    public class MotivationMessagesDto
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public string MessageContent { get; set; }

        public virtual AssignedMotivationMessageDto AssignedMotivationMessage { get; set; }

    }
}
