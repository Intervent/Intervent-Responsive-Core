namespace Intervent.Web.DTO
{
    public class AssignedMotivationMessageDto
    {
        public int Id { get; set; }

        public int MessagesID { get; set; }

        public int OrganizationID { get; set; }

        public DateTime Date { get; set; }

        public bool? Completed { get; set; }

        public string MessageType { get; set; }

        public virtual MotivationMessagesDto MotivationMessage { get; set; }

        public OrganizationDto Organization { get; set; }
    }
}
