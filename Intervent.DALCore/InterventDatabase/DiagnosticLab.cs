namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Diagnosticlabs")]
    public partial class DiagnosticLab
    {
        [Key]
        public int Id { get; set; }

        [StringLength(200)]
        public string? LabName { get; set; }

        [StringLength(10)]
        public string? LanguageItem { get; set; }

    }
}
