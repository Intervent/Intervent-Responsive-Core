namespace Intervent.Web.DTO
{
    public class UpdateUserTrackingStatusRequest
    {
        public int UserId { get; set; }

        public int PortalId { get; set; }

        public bool? DeclinedEnroll { get; set; }

        public byte? DeclinedEnrollmentReason { get; set; }

        public bool? DoNotTrack { get; set; }

    }
}
