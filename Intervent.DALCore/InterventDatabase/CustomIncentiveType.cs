namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class CustomIncentiveType
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(10)]
        public string? LanguageCode { get; set; }
    }
}
