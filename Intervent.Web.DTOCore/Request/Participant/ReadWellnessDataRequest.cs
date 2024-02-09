namespace Intervent.Web.DTO
{
    public class ReadWellnessDataRequest
    {
        public int? id { get; set; }
        public int participantId { get; set; }
    }

    public class ReadTeamsBP_PPRDataRequest
    {
        public int participantId { get; set; }
    }
}