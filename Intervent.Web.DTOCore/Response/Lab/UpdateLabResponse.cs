namespace Intervent.Web.DTO
{
    public class UpdateLabResponse
    {
        public bool success { get; set; }

        public bool updatedByAdmin { get; set; }

        public string emailId { get; set; }

        public string languagePreference { get; set; }

        public string criticalalert { get; set; }

        public int labId { get; set; }

        public string userName { get; set; }

        public int organizationId { get; set; }
    }
}
