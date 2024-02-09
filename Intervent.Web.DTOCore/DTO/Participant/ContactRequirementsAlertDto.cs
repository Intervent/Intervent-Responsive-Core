namespace Intervent.Web.DTO
{
    public class ContactRequirementsAlertDto
    {
        public ContactRequirementsAlertDto()
        {
            ContactRequirements = new HashSet<ContactRequirementDto>();
        }

        public int Id { get; set; }

        public string AlertType { get; set; }

        public string CoachingMessage { get; set; }

        public string SelfHelpMessage { get; set; }

        public int? CoachingOutBound { get; set; } //Days count

        public int? SelfHelpOutBound { get; set; } //Days count

        public int? Min { get; set; }

        public int? Max { get; set; }

        public string Code { get; set; }

        public int? Frequency { get; set; } //Days count

        public int? Level { get; set; }

        public int? AppointmentWithIn { get; set; } //Days count

        public int? DoNotTrack { get; set; } //Days count

        public virtual ICollection<ContactRequirementDto> ContactRequirements { get; set; }
    }
}
