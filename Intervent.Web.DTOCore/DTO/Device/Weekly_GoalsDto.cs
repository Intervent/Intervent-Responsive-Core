namespace Intervent.Web.DTO
{
    public class WeeklyGoalsDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }

        public int? stepsGoal { get; set; }

        public int? ModerateIntensityGoal { get; set; }

        public int? VigorousIntensityGoal { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedOn { get; set; }
    }
}