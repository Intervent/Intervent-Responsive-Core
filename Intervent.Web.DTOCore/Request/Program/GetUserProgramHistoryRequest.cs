namespace Intervent.Web.DTO
{
    public class GetUserProgramHistoryRequest
    {
        public int userId { get; set; }

        public string timeZone { get; set; }

        public int portalId { get; set; }

        public string languageCode { get; set; }
    }
}
