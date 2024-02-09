namespace Intervent.Web.DTO
{
    public class GetCoachCalendarResponse
    {
        public List<AvailabilityDto> Availabilities { get; set; }

        public List<AppointmentDTO> Appointments { get; set; }
    }
}
