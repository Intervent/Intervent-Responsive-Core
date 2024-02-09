namespace Intervent.Web.DTO
{
    public class CRM_PogoMeterNumbersDto
    {
        public int Id { get; set; }

        public int CRMContactId { get; set; }

        public string PogoMeterNumber { get; set; }

        public bool IsActive { get; set; }

        public virtual CRM_ContactDto CRM_Contacts { get; set; }
    }
}
