namespace Intervent.Web.DTO
{
    public class GetDashboadMessagesRequest
    {
        public int userId { get; set; }

        public string timeZone { get; set; }

        public DateTime? portalStartDate { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public bool newMessage { get; set; }

        public bool isBoth { get; set; }

        public int messageType { get; set; }
    }

    public sealed class BulkAddUserDashboardMessage
    {
        public IEnumerable<UserDashboardMessageDto> Meassages { get; set; }
    }
}
