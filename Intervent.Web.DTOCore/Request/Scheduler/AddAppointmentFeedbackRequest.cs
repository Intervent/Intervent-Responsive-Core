namespace Intervent.Web.DTO
{
    public class AddAppointmentFeedbackRequest
    {
        public int id { get; set; }

        public int rating { get; set; }

        public string comments { get; set; }
    }
}
