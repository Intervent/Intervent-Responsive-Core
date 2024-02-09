namespace Intervent.Web.DTO
{
    public class ListMotivationMessagesResponse
    {
        public IEnumerable<MotivationMessagesDto> MotivationMessages { get; set; }

        public int TotalRecords { get; set; }
    }
}
