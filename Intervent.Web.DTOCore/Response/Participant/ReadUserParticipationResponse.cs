namespace Intervent.Web.DTO
{
    public class ReadUserParticipationResponse
    {
        public int UserId { get; set; }

        public PortalDto Portal { get; set; }

        public HRADto HRA { get; set; }

        public UserDto user { get; set; }

        public UsersinProgramDto usersinProgram { get; set; }

        public AppointmentDTO appointment { get; set; }

        public bool hasActivePortal { get; set; }

        public string UserStatus { get; set; }
    }
}