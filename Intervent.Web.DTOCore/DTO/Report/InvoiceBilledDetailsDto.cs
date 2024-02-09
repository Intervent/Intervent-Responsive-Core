namespace Intervent.Web.DTO
{
    public class InvoiceBilledDetailsDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public decimal Total { get; set; }

        public bool Billed { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public virtual UserDto User { get; set; }

        public virtual List<InvoiceDetailsDto> InvoiceDetails { get; set; }
    }
}
