namespace Intervent.Web.DTO
{
    public class EXT_SleepsDto
    {
        public int id { get; set; }

        public int UserId { get; set; }

        public DateTime starttimestamp { get; set; }

        public int totalsleepduration { get; set; }

        public int awakeduration { get; set; }

        public int awakecount { get; set; }

        public int wakeCount { get; set; }

        public int deepduration { get; set; }

        public int lightduration { get; set; }

        public int remduration { get; set; }

        public bool IsActive { get; set; }

        public string inputmethod { get; set; }

        public string source { get; set; }

        public int? SleepScore { get; set; }

        public UserDto user { get; set; }
    }
}
