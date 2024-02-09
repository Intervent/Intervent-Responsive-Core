namespace Intervent.Web.DTO
{
    public class DiscontinueDrugRequest
    {
        public int Id { get; set; }
        public DateTime DiscontinuedOn { get; set; }
        public int UpdatedBy { get; set; }
    }
}
