namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class AdminTask
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int TaskTypeId { get; set; }

        [Required]
        [StringLength(1)]
        public string Status { get; set; }

        public DateTime CreatedOn { get; set; }

        public int Owner { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        public int? UpdatedBy { get; set; }

        public int CreatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public bool IsActive { get; set; }

        public virtual TaskType TaskType { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

        public virtual User User2 { get; set; }

        public virtual User User3 { get; set; }
    }
}
