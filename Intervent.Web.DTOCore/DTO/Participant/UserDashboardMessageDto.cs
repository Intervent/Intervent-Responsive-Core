namespace Intervent.Web.DTO
{
    public class UserDashboardMessageDto
    {
        public int Id { get; set; }

        public string Message { get; set; }

        public string Url { get; set; }

        public bool New { get; set; }

        public int UserId { get; set; }

        public DateTime CreatedOn { get; set; }

        public string CreatedOnString { get; set; }

        public int MessageType { get; set; }

        public int? RelatedId { get; set; }

        public bool Active { get; set; }

        public virtual DashboardMessageTypeDto DashboardMessageType { get; set; }

        public string LanguageItem { get; set; }

        public string Parameters { get; set; }

        public byte Status { get; set; }

        public AppointmentDTO appointment { get; set; }

        public virtual UserDto User { get; set; }
    }
}
