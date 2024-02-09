namespace Intervent.DAL
{

    public partial class InvoiceDetail
    {
        public int Id { get; set; }

        public int InvoiceId { get; set; }

        public int UserId { get; set; }

        public int Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual BillingServiceType BillingServiceType { get; set; }

        public virtual InvoiceBilledDetail InvoiceBilledDetail { get; set; }

        public virtual User User { get; set; }
    }
}
