namespace Intervent.Web.DTO
{
    public class ListPaymentTransactionRequest
    {

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public int? orgId { get; set; }

        public string type { get; set; }

        public int AdminId { get; set; }

        public string timezone { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }
    }
}
