namespace Intervent.Web.DTO
{
    public class ListCouponsRequest
    {
        public int OrganizationId { get; set; }

        public bool onlyActive { get; set; }
    }
}
