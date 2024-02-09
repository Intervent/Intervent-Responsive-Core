namespace Intervent.Web.DTO
{
    public class UpdateParticipantInfoRequest
    {
        public string inquiryNumber { get; set; }

        public string trkMode { get; set; }

        public string note { get; set; }

        public int coachId { get; set; }
    }
}