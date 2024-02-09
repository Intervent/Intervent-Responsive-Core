namespace Intervent.DAL
{
    public class ListEligibilityResult
    {
        public int? Id { get; set; }

        public string? FirstName { get; set; }

        public string? LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string? UniqueId { get; set; }

        public int? PortalId { get; set; }

        public string? PortalName { get; set; }

        public DateTime? ReferredOn { get; set; }

        public DateTime? LastEligNotesDate { get; set; }

        public DateTime? LastUserNotesDate { get; set; }

        public int? Records { get; set; }
    }
}
