namespace Intervent.HWS
{
    public class LabResponse
    {
        public string UniqueId { get; set; }

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

        public float? Waist { get; set; }

        public string HL7 { get; set; }

        public byte[] ReportData { get; set; }

        public float? BMI { get; set; }

        public bool? HighCotinine { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public DateTime? DOB { get; set; }

        public string OrderId { get; set; }

        public string trumpiaKey { get; set; }

    }
    public class LabOrderResponse
    {
        public string Error { get; set; }
        public bool Status { get; set; }
    }
}
