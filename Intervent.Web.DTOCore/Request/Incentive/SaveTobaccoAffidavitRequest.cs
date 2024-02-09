namespace Intervent.Web.DTO
{
    public class SaveTobaccoAffidavitRequest
    {
        public int userId { get; set; }

        public int portalId { get; set; }

        public string reference { get; set; }

        public int? programsInPortalId { get; set; }

    }
}
