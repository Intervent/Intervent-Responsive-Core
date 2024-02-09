namespace Intervent.Web.DTO
{
    public class ReportFeedbackDto
    {
        public int Id { get; set; }

        public int HRAId { get; set; }

        public byte Type { get; set; }

        public string Comments { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual HRADto HRA { get; set; }
    }
}
