namespace Intervent.Web.DTO
{
    public class AddTestimonialRequest
    {
        public int userid { get; set; }
        public int portalid { get; set; }
        public string feedback { get; set; }
        public string SignedName { get; set; }
        public string Date { get; set; }
    }
}
