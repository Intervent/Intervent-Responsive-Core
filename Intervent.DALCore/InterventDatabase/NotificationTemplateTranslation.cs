namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class NotificationTemplateTranslation
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int NotificationTemplateId { get; set; }

        [Key]
        [Column(Order = 1)]
        [StringLength(20)]
        public string LanguageCode { get; set; }

        [Key]
        [Column(Order = 2)]
        [StringLength(250)]
        public string Subject { get; set; }

        public string? TemplateSource { get; set; }

        public virtual NotificationTemplate NotificationTemplate { get; set; }
    }
}
