namespace Intervent.Web.DTO
{
    public class GlucometerUserDto
    {
        public string UniqueId { get; set; }
        public DateTime? RegisteredDate { get; set; }
        public DateTime? ActivationDate { get; set; }
        public int? OrganizationId { get; set; }
    }
}
