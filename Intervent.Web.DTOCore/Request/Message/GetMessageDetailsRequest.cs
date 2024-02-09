namespace Intervent.Web.DTO
{
    public class GetMessageDetailsRequest
    {
        public int messageId { get; set; }

        public int userId { get; set; }

        public string timeZone { get; set; }

        public bool updateStatus { get; set; }

        public bool IsAdmin { get; set; }

        public int? adminId { get; set; }

        public int systemAdminId { get; set; }
    }
}
