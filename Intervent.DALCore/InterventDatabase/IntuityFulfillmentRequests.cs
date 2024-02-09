namespace Intervent.DAL
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("IntuityFulfillmentRequests")]
    public partial class IntuityFulfillmentRequests
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public bool? ImmediateShipment { get; set; }

        public int? ReplenishmentQuantity { get; set; }

        [StringLength(50)]
        public string? Reason { get; set; }

        public bool? SendMeter { get; set; }

        public bool? Submitted { get; set; }

        public int NumberOfShipments { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

        public virtual IntuityEligibility IntuityEligibility { get; set; }
    }
}
