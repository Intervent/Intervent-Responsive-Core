namespace Intervent.Web.DTO
{
    public class IntuityEventDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string UniqueId { get; set; }

        public int? EventType { get; set; }

        public DateTime? EventDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual IntuityEventTypeDto IntuityEventType { get; set; }

    }
}
