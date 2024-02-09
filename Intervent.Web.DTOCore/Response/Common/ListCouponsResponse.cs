namespace Intervent.Web.DTO
{
    public class ListCouponsResponse
    {
        public IList<CouponCodeDto> Coupons { get; set; }

        public string Organization { get; set; }
    }
}
