namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserLog")]
    public partial class UserLog
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public DateTime LastAccessedOn { get; set; }

        public DateTime? LoggedOutTime { get; set; }

        public bool? IsSuccess { get; set; }

        public int? LoggedInDeviceId { get; set; }

        public virtual UserLoggedInDevice UserLoggedInDevice { get; set; }

        public virtual User User { get; set; }
    }
}
