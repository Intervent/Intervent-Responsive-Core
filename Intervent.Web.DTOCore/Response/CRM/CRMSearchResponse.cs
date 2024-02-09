namespace Intervent.Web.DTO
{
    public class CRMSearchResponse
    {
        public IList<CRM_ContactDto> CRM_ContactDto { get; set; }

        public int TotalRecords { get; set; }
    }
}
