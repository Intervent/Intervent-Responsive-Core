namespace Intervent.Web.DTO
{
    public class ListPaymentTransactionResponse
    {
        public IList<PaymentTransactionDto> report { get; set; }

        public int totalRecords { get; set; }
    }
}
