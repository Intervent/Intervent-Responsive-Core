namespace Intervent.Web.DTO
{
    public class PaymentTransactionDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; }
        public int? RelatedId { get; set; }
        public string TransactionId { get; set; }
        public DateTime? Date { get; set; }
        public UserDto User { get; set; }
    }
}
