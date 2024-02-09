namespace Intervent.Web.DTO
{
    public class UpdateIntuityFulfillmentRequest
    {
        public int IntuityEligibilityId { get; set; }

        public bool ImmediateShipment { get; set; }

        public int Quantity { get; set; }

        public string Reason { get; set; }

        public int numberofshipments { get; set; }

        public int UserId { get; set; }

        public bool SendMeter { get; set; }

        public bool Submitted { get; set; }
    }
}
