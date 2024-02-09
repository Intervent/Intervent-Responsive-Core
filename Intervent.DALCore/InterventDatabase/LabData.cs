namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("LabData")]
    public partial class LabData
    {
        public int Id { get; set; }

        public int LabId { get; set; }

        [Required]
        public byte[] ReportData { get; set; }

        public DateTime DateCreated { get; set; }

        public virtual Lab Lab { get; set; }
    }
}
