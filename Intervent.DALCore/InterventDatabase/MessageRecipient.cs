namespace Intervent.DAL
{
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("MessageRecipients")]
    public partial class MessageRecipient
    {
        public int Id { get; set; }

        public int? RecipientId { get; set; }

        public int MessageId { get; set; }

        public bool IsRead { get; set; }

        public int? ReadBy { get; set; }

        public virtual Message Message { get; set; }

        public virtual User User { get; set; }

        public virtual User User1 { get; set; }

    }
}
