namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("Log")]
    public partial class Log
    {
        public int Id { get; set; }

        public DateTime TimeStamp { get; set; }

        [Required]
        [StringLength(5)]
        public string Level { get; set; }

        [Required]
        [StringLength(200)]
        public string Logger { get; set; }

        [Required]
        public string Message { get; set; }

        public string? ExceptionType { get; set; }

        public string? Operation { get; set; }

        public string? ExceptionMessage { get; set; }

        public string? StackTrace { get; set; }
    }
}
