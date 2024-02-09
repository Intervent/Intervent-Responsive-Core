namespace Intervent.Web.DTO
{
    public class EXT_WorkoutsDto
    {
        public int id { get; set; }

        public int UserId { get; set; }

        public DateTime starttimestamp { get; set; }

        public int duration { get; set; }

        public DateTime endtimestamp { get; set; }

        public int caloriesburned { get; set; }

        public int distance { get; set; }

        public string name { get; set; }

        public string category { get; set; }

        public bool IsActive { get; set; }

        public string inputmethod { get; set; }

        public string source { get; set; }

        public UserDto user { get; set; }

    }
}
