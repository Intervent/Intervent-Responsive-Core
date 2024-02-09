namespace Intervent.Web.DTO
{
    public class ExercisePlanDto
    {
        public int Id { get; set; }

        public int HraId { get; set; }

        public int Weeknumber { get; set; }

        public string DaysPerWeek { get; set; }

        public string Duration { get; set; }

        public bool Completed { get; set; }
    }
}