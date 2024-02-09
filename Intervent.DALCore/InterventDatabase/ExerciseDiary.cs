namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExerciseDiary")]
    public partial class ExerciseDiary
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Column(TypeName = "date")]
        public DateTime Date { get; set; }

        public int? Type { get; set; }

        public int? Duration { get; set; }

        public byte? RPE { get; set; }

        public short? HeartRate { get; set; }

        public int? Points { get; set; }

        public int? StepsPerDay { get; set; }

        public string? Notes { get; set; }

        public virtual ExerciseType ExerciseType { get; set; }

        public virtual User User { get; set; }
    }
}
