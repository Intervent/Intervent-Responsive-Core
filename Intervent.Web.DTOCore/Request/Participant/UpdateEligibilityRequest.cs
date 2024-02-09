namespace Intervent.Web.DTO
{
    public class UpdateEligibilityRequest
    {
        public int PortalId { get; set; }

        public string ExistingUniqueId { get; set; }

        public string NewUniqueId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }
    }
}
