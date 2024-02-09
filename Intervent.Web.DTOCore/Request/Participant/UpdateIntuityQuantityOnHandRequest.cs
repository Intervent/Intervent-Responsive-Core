namespace Intervent.Web.DTO
{
    public class UpdateIntuityQuantityOnHandRequest
    {
        public int IntuityEligibilityId { get; set; }

        public int Quantity { get; set; }

        public int UserId { get; set; }

        public bool Submitted { get; set; }
    }
}
