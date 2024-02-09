namespace Intervent.Web.DTO
{
    public class AddCustomIncentiveRequest
    {
        public int userId { get; set; }

        public int adminId { get; set; }

        public int PortalIncentiveId { get; set; }

        public int CustomIncentiveType { get; set; }

        public double Points { get; set; }

        public string comments { get; set; }

    }
}
