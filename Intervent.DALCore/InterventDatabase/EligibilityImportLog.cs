namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("EligibilityImportLog")]
    public partial class EligibilityImportLog
    {
        public int ID { get; set; }

        [Required]
        [StringLength(100)]
        public string UniqueId { get; set; }

        public int PortalId { get; set; }

        [Required]
        [StringLength(2)]
        public string Action { get; set; }

        public DateTime LogDate { get; set; }

        [Required]
        [StringLength(100)]
        public string CreatedByUser { get; set; }

        public int? EligibilityId { get; set; }

        public string? ChangedFields { get; set; }

        public byte? IsLoadError { get; set; }

        [StringLength(2000)]
        public string? ErrorDetails { get; set; }

        [StringLength(100)]
        public string? FirstName { get; set; }

        [StringLength(100)]
        public string? LastName { get; set; }
    }
}
