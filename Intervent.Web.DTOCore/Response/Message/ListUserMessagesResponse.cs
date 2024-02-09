namespace Intervent.Web.DTO
{
    public class ListUserMessagesResponse
    {
        public ICollection<MessageDto> parentMessages { get; set; }

        public ICollection<MessageDto> messageDetails { get; set; }

        public MessageCountResponse messageCount { get; set; }

        public int totalRecords { get; set; }
    }
}
