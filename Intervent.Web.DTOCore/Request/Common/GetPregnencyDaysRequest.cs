namespace Intervent.Web.DTO
{
    public class GetPregnencyDaysRequest
    {
        public int userId { get; set; }

        public int hraId { get; set; }

        public DateTime startDate { get; set; }

        public DateTime? PregDueDate { get; set; }
    }
}
