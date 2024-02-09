using System.ComponentModel.DataAnnotations;

namespace Intervent.DAL
{
    public partial class Languages
    {
        [Key]
        public int Id { get; set; }

        [StringLength(50)]
        public string? Language { get; set; }

        [StringLength(10)]
        public string? LanguageCode { get; set; }

        [StringLength(10)]
        public string? LanguageItem { get; set; }

        public virtual ICollection<Portal> Portal { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}
