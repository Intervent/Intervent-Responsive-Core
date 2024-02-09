namespace Intervent.Web.DTO
{
    public class EXT_WeightDto
    {
        public int id { get; set; }

        public int UserId { get; set; }

        public DateTime timestamp { get; set; }

        public double weight { get; set; }

        public double bmi { get; set; }

        public double fatpercent { get; set; }

        public bool IsActive { get; set; }

        public string inputmethod { get; set; }

        public string source { get; set; }

        public UserDto user { get; set; }
    }
}
