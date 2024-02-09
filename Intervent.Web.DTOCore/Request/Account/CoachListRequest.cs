namespace Intervent.Web.DTO
{
    public class CoachListRequest
    {

        public string OrganizationIds { get; set; }

        public bool? active { get; set; }

        public int? stateId { get; set; }

        public bool? allowAppt { get; set; }

    }

    public class FilteredCoachListRequest
    {
        public string language { get; set; }

        public string speciality { get; set; }

        public string coachName { get; set; }

        public int? OrganizationId { get; set; }

        public bool? IsAdmin { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }

        public bool? byCoach { get; set; }

        public string TimeZone { get; set; }

        public int? stateId { get; set; }

    }
}
