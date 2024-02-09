namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("StressDiary")]
    public partial class StressDiary
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime Date { get; set; }

        public byte Duration { get; set; }

        [Required]
        [StringLength(200)]
        public string Trigger { get; set; }

        [Required]
        [StringLength(500)]
        public string Response { get; set; }

        public byte Intensity { get; set; }

        public bool Active { get; set; }

        public DateTime AddedOn { get; set; }

        public virtual User User { get; set; }
    }
}
