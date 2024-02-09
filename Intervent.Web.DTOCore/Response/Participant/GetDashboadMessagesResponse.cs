namespace Intervent.Web.DTO
{
    public class GetDashboadMessagesResponse
    {
        public IList<UserDashboardMessageDto> dashboardMessages { get; set; }

        public bool hasReadMessages { get; set; }

        public int totalRecords { get; set; }
    }
}
