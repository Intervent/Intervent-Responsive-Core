namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SleepLog")]
    public partial class SleepLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Start { get; set; }

        public DateTime End { get; set; }

        public byte? SleepQuality { get; set; }

        public byte? DaytimeAlertness { get; set; }

        public byte? Mood { get; set; }

        [StringLength(200)]
        public string? Notes { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public virtual User User { get; set; }
    }
}
