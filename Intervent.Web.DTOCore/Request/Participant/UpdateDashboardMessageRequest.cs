namespace Intervent.Web.DTO
{
    public class UpdateDashboardMessageRequest
    {
        public int? id { get; set; }

        public int? relatedId { get; set; }

        public int? messageType { get; set; }

        public bool? New { get; set; }

        public bool? Active { get; set; }

        public byte? Status { get; set; }
    }
}
