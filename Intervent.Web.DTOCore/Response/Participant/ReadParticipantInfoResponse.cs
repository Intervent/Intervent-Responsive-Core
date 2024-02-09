namespace Intervent.Web.DTO
{
    public class ReadParticipantInfoResponse
    {
        public PortalDto portal { get; set; }

        public UserDto user { get; set; }

        public ParticipantProfile_ResultDto participant { get; set; }

        public HRADto hra { get; set; }

        public LabDto lab { get; set; }

        public int labNo { get; set; }

        public List<UsersinProgramDto> UsersinProgram { get; set; }

        public DateTime? ConditionDate { get; set; }
        public string ConditionType { get; set; }

        public bool? enrolledinCoaching { get; set; }

        public NotesDto note { get; set; }

        public float? Weight { get; set; }

        public bool? hasEMR { get; set; }

        public string WellnessVision { get; set; }

        public bool success { get; set; }

        public string WellnessDate { get; set; }

        public IList<UserFormDto> UserForms { get; set; }
    }
}