namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Email")]
    public partial class Email
    {
        public int Id { get; set; }

        public int From { get; set; }

        public int To { get; set; }

        [Required]
        [StringLength(500)]
        public string Message { get; set; }

        public DateTime Date { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }
    }
}
