using Intervent.Web.DTO;

namespace InterventWebApp
{
    public class MessagingModel
    {
        public IEnumerable<MessageDto> parentMessages { get; set; }

        public MessageCountModel MessageCount { get; set; }

        public int participantId { get; set; }

        public bool hasActivePortal { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }

    }

    public class MessageDashboardModel
    {
        public int userId { get; set; }

        public string emailUrl { get; set; }
    }

    public class MessageCountModel
    {

        public int MessageBoardCount { get; set; }

        public int MyMessagesCount { get; set; }

        public int PendingMessagesCount { get; set; }

        public int FollowUpMessagesCount { get; set; }

        public int ActiveMessagesCount { get; set; }

    }

    public class ChatMessageModel
    {
        public int participantId { get; set; }

        public bool hasActivePortal { get; set; }

        public int userId { get; set; }

        public int systemAdminId { get; set; }
    }
}