namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("UserTrackingStatuses")]
    public partial class UserTrackingStatus
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public bool? DeclinedEnrollment { get; set; }

        public byte? DeclinedEnrollmentReason { get; set; }

        public bool? DoNotTrack { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual Portal Portal { get; set; }

        public virtual User User { get; set; }

        public virtual DeclinedEnrollmentReason DeclinedEnrollmentReason1 { get; set; }
    }
}
