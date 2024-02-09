namespace Intervent.Web.DTO
{
    public class WebinarOccurrenceDto
    {
        public int Id { get; set; }

        public int WebinarId { get; set; }

        public string OccurrenceId { get; set; }

        public DateTime StartTime { get; set; }

        public string Status { get; set; }

        public int Duration { get; set; }

        public string VideoUrl { get; set; }

        public string Handout { get; set; }
    }
}
