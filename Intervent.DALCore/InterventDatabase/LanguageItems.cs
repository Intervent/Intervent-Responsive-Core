using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Intervent.DAL
{
    public class LanguageItems
    {
        [Key]
        [Column(Order = 0)]
        [StringLength(20)]
        public string LanguageCode { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(64)]
        public string ItemCode { get; set; }

        public string? Text { get; set; }
    }
}
