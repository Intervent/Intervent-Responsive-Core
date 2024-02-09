namespace Intervent.Web.DTO
{
    public class AddEditEligibilityToCRMRequest
    {
        public string UniqueId { get; set; }

        public int CRMId { get; set; }

        public int? EligibilityOrgId { get; set; }

        public int intuityOrgId { get; set; }
    }
}
