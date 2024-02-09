namespace Intervent.Web.DTO
{
    public class HealthDataDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public float Weight { get; set; }

        public int Source { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public virtual UserDto User { get; set; }

        public virtual UserDto User1 { get; set; }
    }
}