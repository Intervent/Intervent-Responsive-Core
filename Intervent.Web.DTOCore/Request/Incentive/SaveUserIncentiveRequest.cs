namespace Intervent.Web.DTO
{
    public class SaveUserIncentiveRequest
    {
        public int userId { get; set; }

        public int incentiveId { get; set; }

        public int portalId { get; set; }

        public int? adminId { get; set; }

        public string Reference { get; set; }

    }
}
