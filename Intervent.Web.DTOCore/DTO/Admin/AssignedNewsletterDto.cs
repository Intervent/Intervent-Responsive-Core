namespace Intervent.Web.DTO
{
    public class AssignedNewsletterDto
    {
        public int Id { get; set; }

        public int OrganizationId { get; set; }

        public int NewsletterId { get; set; }

        public DateTime Date { get; set; }

        public bool Completed { get; set; }

        public OrganizationDto Organization { get; set; }
    }
}
