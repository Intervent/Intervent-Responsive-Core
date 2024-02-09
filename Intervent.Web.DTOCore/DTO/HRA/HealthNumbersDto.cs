namespace Intervent.Web.DTO
{
    public class HealthNumbersDto
    {
        public int HRAId { get; set; }

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

        public float? HeightInch { get; set; }

        public float? HeightCM { get; set; }

        public float? Waist { get; set; }

        public short? RMR { get; set; }

        public short? THRFrom { get; set; }

        public short? THRTo { get; set; }

        public HRADto HRA { get; set; }

        public double? CAC { get; set; }

        public float? DesiredWeight { get; set; }

        public float? CRF { get; set; }

        public float? RHR { get; set; }

        public string UniqueId { get; set; }

        public int? OrganizationId { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public int? LabId { get; set; }

        public LabDto Lab { get; set; }
    }
}