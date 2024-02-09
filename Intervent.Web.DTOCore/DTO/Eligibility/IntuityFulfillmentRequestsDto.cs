namespace Intervent.Web.DTO
{
    public class IntuityFulfillmentRequestsDto
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public bool? ImmediateShipment { get; set; }

        public int? ReplenishmentQuantity { get; set; }

        public string Reason { get; set; }

        public bool? SendMeter { get; set; }

        public bool? Submitted { get; set; }

        public int NumberOfShipments { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
