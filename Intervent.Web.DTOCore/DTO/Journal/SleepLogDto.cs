namespace Intervent.Web.DTO
{
    public class SleepLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public byte? SleepQuality { get; set; }

        public byte? DaytimeAlertness { get; set; }

        public byte? Mood { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public double sleptHours { get; set; }

        public string StartDT { get; set; }

        public string EndDT { get; set; }

    }
}
