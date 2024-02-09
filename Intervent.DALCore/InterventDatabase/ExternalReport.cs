namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class ExternalReport
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(50)]
        public string? ReportName { get; set; }

        public byte[] ReportData { get; set; }

        public DateTime? CreatedOn { get; set; }

        public virtual User User { get; set; }
    }
}
