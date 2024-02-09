namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("HealthData")]
    public partial class HealthData
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public float Weight { get; set; }

        [Required]
        public int Source { get; set; }

        public DateTime CreatedOn { get; set; }

        public int CreatedBy { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
