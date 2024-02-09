namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class CarePlanReport
    {
        public int Id { get; set; }

        public int? Type { get; set; }

        [StringLength(50)]
        public string? RefId { get; set; }

        public bool? ReportGenerated { get; set; }

        public int? CreatedBy { get; set; }

        [Column(TypeName = "date")]
        public DateTime? CreatedOn { get; set; }

        public virtual User User { get; set; }
    }
}
