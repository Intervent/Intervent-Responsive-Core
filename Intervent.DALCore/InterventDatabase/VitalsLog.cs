namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("VitalsLog")]
    public partial class VitalsLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public float? Weight { get; set; }

        public byte? AerobicExercise { get; set; }

        public byte? HealthyEating { get; set; }

        public byte? Hydration { get; set; }

        public byte? Alcohol { get; set; }

        public byte? Tobacco { get; set; }

        public byte? Medications { get; set; }

        public byte? Sleep { get; set; }

        public byte? Stress { get; set; }

        public DateTime? Date { get; set; }

        public byte? HasWeight { get; set; }

        public byte? Happy { get; set; }

        public int? Points { get; set; }

        public virtual User User { get; set; }
    }
}
