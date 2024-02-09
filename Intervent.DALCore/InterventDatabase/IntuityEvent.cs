namespace Intervent.DAL
{

    public partial class IntuityEvent
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public string? UniqueId { get; set; }

        public int? EventType { get; set; }

        public DateTime? EventDate { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool Processed { get; set; }

        public virtual IntuityEventType IntuityEventType { get; set; }

        public virtual User User { get; set; }
    }
}
