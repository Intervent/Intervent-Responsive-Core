namespace Intervent.Web.DTO
{
    public class TobaccoLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public string Location { get; set; }

        public string UsedWith { get; set; }

        public string Trigger { get; set; }

        public string Notes { get; set; }

        public bool Active { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }
    }
}
