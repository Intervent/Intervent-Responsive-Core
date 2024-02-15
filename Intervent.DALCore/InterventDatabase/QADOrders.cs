namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public partial class QADOrders
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Order { get; set; }

        public DateTime? OrderDate { get; set; }

        public int? MeterQuantity { get; set; }

        [StringLength(50)]
        public string ItemNumber { get; set; }

        [StringLength(10)]
        public string QtyOrdered { get; set; }
    }
}