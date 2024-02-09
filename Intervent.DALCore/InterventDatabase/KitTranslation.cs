using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public class KitTranslation
    {
        [Key]
        [Column(Order = 0)]
        public int KitId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string LanguageCode { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        public string? KeyConcepts { get; set; }

        [StringLength(250)]
        public string? Pdf { get; set; }

        [StringLength(50)]
        public string? Audio { get; set; }

        public virtual Kit Kits { get; set; }

        public DateTime? PublishedDate { get; set; }

        public DateTime? LastUpdated { get; set; }

    }
}
