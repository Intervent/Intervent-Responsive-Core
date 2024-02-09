namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ScreeningDataErrorLog")]
    public partial class ScreeningDataErrorLog
    {
        public int Id { get; set; }

        public int? OrgId { get; set; }

        [Required]
        [StringLength(100)]
        public string UniqueId { get; set; }

        [StringLength(100)]
        public string? Error { get; set; }

        public DateTime? Date { get; set; }

        public virtual Organization Organization { get; set; }
    }
}
