namespace Intervent.Web.DTO
{
    public class VitalsLogDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public byte? HasWeight { get; set; }

        public byte? AerobicExercise { get; set; }

        public byte? HealthyEating { get; set; }

        public byte? Hydration { get; set; }

        public byte? Alcohol { get; set; }

        public byte? Tobacco { get; set; }

        public byte? Medications { get; set; }

        public byte? Sleep { get; set; }

        public byte? Stress { get; set; }

        public byte? Happy { get; set; }

        public float? Weight { get; set; }

        public int? Points { get; set; }

        public string TimeZoneId { get; set; }

        public DateTime? Date { get; set; }

        public UserDto User { get; set; }
    }
}
