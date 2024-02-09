namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class GlucometerUser
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UniqueId { get; set; }

        [Column(TypeName = "date")]
        public DateTime? RegisteredDate { get; set; }

        [Column(TypeName = "date")]
        public DateTime? ActivationDate { get; set; }

        public int? OrganizationId { get; set; }
    }
}
