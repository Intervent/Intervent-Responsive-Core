namespace Intervent.Web.DTO

{
    public class IntuityEventTypeDto
    {
        public int Id { get; set; }

        public string Type { get; set; }

        public virtual IList<IntuityEventDto> IntuityEvents { get; set; }
    }
}
