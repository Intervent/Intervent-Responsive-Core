namespace Intervent.Web.DTO
{
    public class EXT_BloodPressureDto
    {
        public int id { get; set; }

        public int UserId { get; set; }

        public DateTime timestamp { get; set; }

        public int restingheartrate { get; set; }

        public int systolic { get; set; }

        public int diastolic { get; set; }

        public bool IsActive { get; set; }

        public string inputmethod { get; set; }

        public string source { get; set; }

        public UserDto user { get; set; }
    }
}
