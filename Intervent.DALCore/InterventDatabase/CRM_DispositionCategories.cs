namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;

    public partial class CRM_DispositionCategories
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }
    }
}
