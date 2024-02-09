namespace Intervent.Web.DTO
{
    public class MedicalPlanCodeDto
    {
        public string Code { get; set; }

        public bool IVEligible { get; set; }

        public bool VendorEligible { get; set; }

        public string Description { get; set; }
    }
}
