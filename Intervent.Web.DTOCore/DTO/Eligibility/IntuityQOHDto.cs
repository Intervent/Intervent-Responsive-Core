namespace Intervent.Web.DTO
{
    public class IntuityQOHDto
    {
        public int Id { get; set; }

        public int IntuityEligibilityId { get; set; }

        public int QuantityOnHand { get; set; }

        public bool? Submitted { get; set; }

        public int CreatedBy { get; set; }

        public DateTime CreatedOn { get; set; }

    }
}
