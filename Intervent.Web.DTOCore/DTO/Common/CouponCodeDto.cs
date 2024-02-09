namespace Intervent.Web.DTO
{
    public class CouponCodeDto
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }
        public string Code { get; set; }

        public string Name { get; set; }

        public bool Active { get; set; }

        public OrganizationDto organization { get; set; }
    }
}
