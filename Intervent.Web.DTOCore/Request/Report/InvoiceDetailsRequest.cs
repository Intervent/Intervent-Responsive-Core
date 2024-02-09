namespace Intervent.Web.DTO
{
    public class InvoiceDetailsRequest
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int Type { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime? UpdatedOn { get; set; }
    }
}