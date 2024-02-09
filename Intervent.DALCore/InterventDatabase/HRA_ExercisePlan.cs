namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HRA_ExercisePlans")]
    public partial class HRA_ExercisePlan
    {
        public int Id { get; set; }

        public int HraId { get; set; }

        public int Weeknumber { get; set; }

        public string? DaysPerWeek { get; set; }

        public string? Duration { get; set; }

        public bool Completed { get; set; }
    }
}
