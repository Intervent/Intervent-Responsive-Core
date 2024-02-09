namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("ExternalUser")]
    public partial class ExternalUser
    {
        [Key]
        [Column(Order = 0)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int UserId { get; set; }

        [Key]
        [Column(Order = 1)]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int ExternalDevice { get; set; }

        [Required]
        [StringLength(50)]
        public string ExternalId { get; set; }

        [StringLength(512)]
        public string? DeviceSyncURL { get; set; }

        [StringLength(512)]
        public string? ExternalToken { get; set; }

        [StringLength(100)]
        public string? MobileToken { get; set; }

        public DateTime DateCreated { get; set; }

        public DateTime DateUpdated { get; set; }

        public int? DeviceCount { get; set; }

        public bool IsActive { get; set; }

        public virtual User User { get; set; }
    }
}
