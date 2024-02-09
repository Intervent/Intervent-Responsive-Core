namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class UserEligibilitySetting
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UniqueId { get; set; }

        public int OrganizationId { get; set; }

        [StringLength(20)]
        public string? Language { get; set; }
    }
}
