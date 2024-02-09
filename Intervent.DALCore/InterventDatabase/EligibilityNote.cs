namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class EligibilityNote
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string UniqueId { get; set; }

        public int ModuleType { get; set; }

        public int NoteType { get; set; }

        [Required]
        public string Notes { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public int? OrganizationId { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool Active { get; set; }

        public virtual User User { get; set; }
    }
}
