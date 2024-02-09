namespace Intervent.Web.DTO
{
    public class WebinarDto
    {
        public int Id { get; set; }

        public string HostEmail { get; set; }

        public string HostId { get; set; }

        public string WebinarId { get; set; }

        public string UniqueId { get; set; }

        public int? PresentedBy { get; set; }

        public string Agenda { get; set; }

        public int? Duration { get; set; }

        public string Password { get; set; }

        public string JoinUrl { get; set; }

        public string StartUrl { get; set; }

        public DateTime? StartTime { get; set; }

        public string Topic { get; set; }

        public int? Type { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? UpdatedBy { get; set; }

        public DateTime? UpdatedOn { get; set; }

        public string ImageUrl { get; set; }

        public string VideoUrl { get; set; }

        public string Handout { get; set; }

        public string StartTimeString { get; set; }

        public string PresentedByName { get; set; }

        public int? RecurrenceType { get; set; }

        public int? RecurrenceInterval { get; set; }

        public int? RecurrenceTimes { get; set; }

        public virtual IList<OrganizationsforWebinarDto> OrganizationsforWebinars { get; set; }

        public virtual IList<WebinarOccurrenceDto> WebinarOccurrences { get; set; }

        public virtual IList<RegisteredUsersforWebinarDto> RegisteredUsersforWebinars { get; set; }

        public virtual UserDto User { get; set; }

        public virtual UserDto User1 { get; set; }

        public virtual UserDto User2 { get; set; }

    }
}
