namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class FaxedReport
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int SentBy { get; set; }

        public DateTime? SentOn { get; set; }

        [StringLength(50)]
        public string? FaxNumber { get; set; }

        public int? RefId { get; set; }

        public int? ReportType { get; set; }

        public bool? IsSent { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual ReportType ReportType1 { get; set; }
    }
}
