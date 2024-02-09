namespace Intervent.Web.DTO
{
    public class OrganizationsforWebinarDto
    {
        public int Id { get; set; }

        public int WebinarId { get; set; }

        public int OrganizationId { get; set; }

        public virtual OrganizationDto Organization { get; set; }

        public virtual WebinarDto Webinar { get; set; }


    }
}
