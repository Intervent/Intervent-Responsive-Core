namespace Intervent.Web.DTO
{
    public class StressManagementLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public System.DateTime Date { get; set; }

        public int? StressRating { get; set; }

        public string TechniqueUsed { get; set; }

        public int? MinutesPerformed { get; set; }

        public string Notes { get; set; }

        public DateTime AddedOn { get; set; }
    }
}
