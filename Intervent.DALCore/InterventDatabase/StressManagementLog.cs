namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StressManagementLog")]
    public partial class StressManagementLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public int? StressRating { get; set; }

        [StringLength(100)]
        public string? TechniqueUsed { get; set; }

        public int? MinutesPerformed { get; set; }

        [StringLength(250)]
        public string? Notes { get; set; }

        public DateTime AddedOn { get; set; }

        public virtual User User { get; set; }
    }
}
