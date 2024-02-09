namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("WeightLossJournal")]
    public partial class WeightLossJournal
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public int DayNo { get; set; }

        public float? Waist { get; set; }

        public float? Weight { get; set; }

        [StringLength(500)]
        public string? Food { get; set; }

        [StringLength(500)]
        public string? NotAuthorizedFood { get; set; }

        public bool HadWater { get; set; }

        public bool CutSodium { get; set; }

        public bool SideEffects { get; set; }

        public byte? MotivationScale { get; set; }

        public int? Exercise { get; set; }

        [StringLength(500)]
        public string? Activity { get; set; }

        [StringLength(500)]
        public string? Comments { get; set; }

        public DateTime DateUpdated { get; set; }

        public bool IsActive { get; set; }

        public virtual User User { get; set; }
    }
}
