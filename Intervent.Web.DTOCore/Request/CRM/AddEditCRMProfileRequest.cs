namespace Intervent.Web.DTO
{
    public class AddEditCRMProfileRequest
    {
        public CRM_ContactDto crm_Contact { get; set; }

        public int userId { get; set; }

        public bool fromWebForm { get; set; }

        public int intuityOrgId { get; set; }
    }
}
