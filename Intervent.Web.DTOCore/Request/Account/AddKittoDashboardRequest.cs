namespace Intervent.Web.DTO
{
    public class AddKittoDashboardRequest
    {
        public int userId { get; set; }

        public int kitId { get; set; }

        public int kitsinUserProgramId { get; set; }

        public string languageCode { get; set; }
    }
}
