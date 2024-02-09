namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class SecurityCode
    {
        public int Id { get; set; }

        [Required]
        [StringLength(6)]
        public string Code { get; set; }

        public DateTime ExpireTime { get; set; }

        public byte Status { get; set; }

        public int DeviceId { get; set; }

        public virtual UserLoggedInDevice UserLoggedInDevice { get; set; }
    }
}
