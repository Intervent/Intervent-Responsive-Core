namespace Intervent.Web.DTO
{
    public class LabDto
    {
        public int Id { get; set; }

        public int UserId { get; set; }

        public int PortalId { get; set; }

        public byte? BPArm { get; set; }

        public short? SBP { get; set; }

        public short? DBP { get; set; }

        public byte? DidYouFast { get; set; }

        public DateTime? BloodTestDate { get; set; }

        public float? TotalChol { get; set; }

        public float? LDL { get; set; }

        public float? HDL { get; set; }

        public float? Trig { get; set; }

        public float? Glucose { get; set; }

        public float? A1C { get; set; }

        public float? Weight { get; set; }

        public float? Height { get; set; }

        public float? HeightCM { get; set; }

        public bool? HighCotinine { get; set; }

        public bool AdditionalLab { get; set; }

        public string BloodTestDateString
        {
            get
            {
                return BloodTestDate.HasValue ? BloodTestDate.Value.ToShortDateString() : null;
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                    BloodTestDate = Convert.ToDateTime(value);
            }
        }

        public float? HeightFt { get; set; }

        public float? HeightInch { get; set; }

        public float? Waist { get; set; }

        public short? RMR { get; set; }

        public short? THRFrom { get; set; }

        public short? THRTo { get; set; }

        public float? BMI { get; set; }

        public string LabOrder { get; set; }

        public string OrderNo { get; set; }

        public string HL7 { get; set; }

        public byte? LabSelection { get; set; }

        public int? ModifiedBy { get; set; }

        public DateTime? DateCompleted { get; set; }

        public DateTime? ReviewedOn { get; set; }

        public string CoachAlert { get; set; }

        public int? ReviewedBy { get; set; }

        public byte? SwitchCount { get; set; }

        public byte? ReminderEmailSent { get; set; }

        public DateTime? CreatedOn { get; set; }

        public int? CreatedBy { get; set; }

        public string CriticalAlert { get; set; }

        public UserDto User { get; set; }

        public UserDto User1 { get; set; }

        public UserDto User2 { get; set; }

        public string LetterofAction { get; set; }

        public int? WeightMin { get; set; }

        public int? WeightMax { get; set; }

        public int? DiagnosticLabId { get; set; }

        public bool hasLabCorpPdf { get; set; }

        public String LabSource { get; set; }

    }
}
