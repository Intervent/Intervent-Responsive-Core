namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LabErrorLog")]
    public partial class LabErrorLog
    {
        public int Id { get; set; }

        public int? UserId { get; set; }

        public int? PortalId { get; set; }

        public string? Error { get; set; }

        public DateTime? LogDate { get; set; }

        public byte[] Data { get; set; }

        public virtual User User { get; set; }
    }
}
