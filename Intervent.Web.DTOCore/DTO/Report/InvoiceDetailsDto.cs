namespace Intervent.Web.DTO
{
    public class InvoiceDetailsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int InvoiceId { get; set; }

        public int Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual BillingServiceTypeDto BillingServiceType { get; set; }

        public virtual InvoiceBilledDetailsDto InvoiceBilledDetail { get; set; }

        public virtual UserDto User { get; set; }
    }
}
