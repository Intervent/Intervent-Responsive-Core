namespace Intervent.Web.DTO
{
    public class TimeTrackerDispositionDto
    {
        public TimeTrackerDispositionDto()
        {
            UserTimeTrackers = new HashSet<UserTimeTrackerDto>();
        }

        public int Id { get; set; }

        public string Type { get; set; }

        public string Description { get; set; }

        public bool ShowInUI { get; set; }

        public virtual ICollection<UserTimeTrackerDto> UserTimeTrackers { get; set; }
    }
}