namespace Intervent.Web.DTO
{
    public class AddEditCouponRequest
    {
        public int id { get; set; }
        public string name { get; set; }
        public int orgId { get; set; }
        public string code { get; set; }
        public bool active { get; set; }
    }
}
