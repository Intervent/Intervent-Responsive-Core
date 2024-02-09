namespace Intervent.Web.DTO
{
    public class AddNewMailRequest
    {
        public int? MailId { get; set; }
        public int? From { get; set; }
        public int? To { get; set; }
        public string Subject { get; set; }
        public string Mail { get; set; }
        public DateTime datetime { get; set; }
    }
}
