namespace Intervent.Web.DTO
{
    public class EmailDto
    {
        public int Id { get; set; }

        public int MailId { get; set; }

        public int From { get; set; }

        public int To { get; set; }

        public string Mail1 { get; set; }

        public string Subject { get; set; }

        public DateTime Datetime { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public List<EmailDto> Mail11 { get; set; }

        public bool Read { get; set; }
    }
}
