namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class PaymentTransaction
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        [StringLength(50)]
        public string? Type { get; set; }

        public int? RelatedId { get; set; }

        [StringLength(100)]
        public string? TransactionId { get; set; }

        public DateTime? Date { get; set; }

        public virtual User User { get; set; }
    }
}
