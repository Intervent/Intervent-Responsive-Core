namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("SSOAttributeMapping")]
    public partial class SSOAttributeMapping
    {
        public int Id { get; set; }

        public int SSOProviderId { get; set; }

        [Required]
        [StringLength(50)]
        public string AttributeName { get; set; }

        public int TypeId { get; set; }

        [StringLength(100)]
        public string? Format { get; set; }

        public int UpdatedBy { get; set; }

        public DateTime UpdatedDate { get; set; }

        public bool? IsActive { get; set; }

        public virtual SSOProvider SSOProvider { get; set; }
    }
}
