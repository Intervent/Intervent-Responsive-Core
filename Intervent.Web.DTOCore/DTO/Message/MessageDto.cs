namespace Intervent.Web.DTO
{
    public class MessageDto
    {
        public int Id { get; set; }

        public string Subject { get; set; }

        public int CreatorId { get; set; }

        public string CreatorName { get; set; }

        public string DisplayName { get; set; }

        public int DisplayId { get; set; }

        public string Picture { get; set; }

        public string CreatorRole { get; set; }

        public string MessageBody { get; set; }

        public string RecentMessage { get; set; }

        public DateTime CreateDate { get; set; }

        public DateTime LastMessageDate { get; set; }

        public int? ParentMessageId { get; set; }

        public string Attachment { get; set; }

        public bool IsSent { get; set; }

        public byte StatusId { get; set; }

        public string SeenBy { get; set; }

        public bool IsRead { get; set; }

        public bool StatusChange { get; set; }

        public int NewStatus { get; set; }

        public bool hasAttachment { get; set; }

        public UserDto User { get; set; }

        public ICollection<MessageRecipientDto> MessageRecipients { get; set; }

        public bool CanShowDelete { get; set; }

        public bool? NoActionNeeded { get; set; }

        public DateTime? CaseClosedDate { get; set; }

        public bool CanShowUnread { get; set; }

    }
}
