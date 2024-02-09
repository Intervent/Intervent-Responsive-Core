namespace Intervent.Web.DTO
{
    public class CreateMissedApptEventRequest
    {
        public NotificationEventTypeDto evt { get; set; }

        public int userId { get; set; }

        public int coachId { get; set; }

        public string orgContactNumber { get; set; }

        public string orgContactEmail { get; set; }
    }
}
