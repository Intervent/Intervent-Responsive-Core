namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class Note
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]

        public int Id { get; set; }

        public int userId { get; set; }

        public int PortalId { get; set; }

        public int? Type { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime NotesDate { get; set; }

        public int Admin { get; set; }

        public int? RefId { get; set; }

        public bool? Pinned { get; set; }

        public int? RefId2 { get; set; }

        public virtual NoteType NoteType { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
