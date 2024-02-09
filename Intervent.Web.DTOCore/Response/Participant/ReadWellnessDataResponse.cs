namespace Intervent.Web.DTO
{
    public class ReadWellnessDataResponse
    {
        public WellnessDataDto WellnessData { get; set; }
    }

    public class ReadTeamsBP_PPRResponse
    {
        public List<TeamsBP_PPRDto> TeamsBP_PPR { get; set; }
    }

    public class PendingWellnessDataResponse
    {
        public List<TeamsBP_PPRDto> TeamsBP_PPR { get; set; }
    }
}