namespace Intervent.Web.DTO
{
    public class CoachListResponse
    {
        public IList<UserDto> users { get; set; }
    }

    public class FilteredCoachListResponse
    {
        public List<FilteredCoachListResultDto> coachList { get; set; }
    }

    public class FilteredCoachListResultDto
    {
        public int CoachId { get; set; }
        public string CoachName { get; set; }
        public string Picture { get; set; }
        public string Profile { get; set; }
        public string Languages { get; set; }
        public string Speciality { get; set; }
        public string RoleCode { get; set; }
        public int AvailabilityCount { get; set; }
    }
}
