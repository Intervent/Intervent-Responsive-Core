namespace Intervent.Web.DTO
{
    public class AddEditReportFeedbackRequest
    {
        public int HRAId { get; set; }

        public byte Type { get; set; }

        public string Comments { get; set; }

        public int CreatedBy { get; set; }
    }
}