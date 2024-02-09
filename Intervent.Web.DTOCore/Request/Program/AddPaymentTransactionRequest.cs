namespace Intervent.Web.DTO
{
    public class AddPaymentTransactionRequest
    {
        public int UserId { get; set; }
        public int RelatedId { get; set; }
        public string Type { get; set; }
        public string TransactionId { get; set; }
    }
}