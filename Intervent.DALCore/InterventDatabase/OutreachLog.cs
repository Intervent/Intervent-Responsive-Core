namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("OutreachLog")]
    public partial class OutreachLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [Required]
        [StringLength(30)]
        public string PhoneNumber1 { get; set; }

        [StringLength(30)]
        public string? PhoneNumber2 { get; set; }

        [StringLength(30)]
        public string? PhoneNumber3 { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Required]
        [StringLength(50)]
        public string Company { get; set; }

        [StringLength(100)]
        public string? Street { get; set; }

        [StringLength(50)]
        public string? City { get; set; }

        [StringLength(20)]
        public string? State { get; set; }

        [StringLength(10)]
        public string? Zip { get; set; }

        [Required]
        [StringLength(10)]
        public string Language { get; set; }

        public DateTime? SentOn { get; set; }
    }
}
