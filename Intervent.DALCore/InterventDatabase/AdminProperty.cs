namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public partial class AdminProperty
    {
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int Id { get; set; }

        [StringLength(1012)]
        public string? Profile { get; set; }

        public bool? Video { get; set; }

        public string? MeetingId { get; set; }

        public bool? AllowAppt { get; set; }

        [StringLength(40)]
        public string? ProfileLanguageItem { get; set; }

        public virtual User User { get; set; }

        public virtual ApplicationUser appUser { get; set; }
    }
}
