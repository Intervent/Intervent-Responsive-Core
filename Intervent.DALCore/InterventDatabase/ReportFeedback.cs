namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class ReportFeedback
    {
        public int Id { get; set; }

        public int HRAId { get; set; }

        public byte Type { get; set; }

        [StringLength(1000)]
        public string? Comments { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual HRA HRA { get; set; }
    }
}
