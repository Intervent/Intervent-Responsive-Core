namespace Intervent.DAL
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Mail")]
    public partial class Mail
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Mail()
        {
            Mail11 = new HashSet<Mail>();
        }

        public int Id { get; set; }

        public int? MailId { get; set; }

        public int? From { get; set; }

        public int? To { get; set; }

        [StringLength(150)]
        public string? Subject { get; set; }

        [Column("Mail")]
        public string? Mail1 { get; set; }

        public DateTime? Datetime { get; set; }

        public bool? Read { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Mail> Mail11 { get; set; }

        public virtual Mail Mail2 { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
