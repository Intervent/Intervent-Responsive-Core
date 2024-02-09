namespace Intervent.Web.DTO
{
    public class UserTrackingStatusDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public bool DeclinedEnrollment { get; set; }

        public bool DoNotTrack { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public PortalDto Portal { get; set; }

        public byte? DeclinedEnrollmentReason { get; set; }

    }
}
