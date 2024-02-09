namespace Intervent.Web.DTO
{
    public class UserFormDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public int FormTypeId { get; set; }

        public string Form { get; set; }

        public bool Approved { get; set; }
    }
}
