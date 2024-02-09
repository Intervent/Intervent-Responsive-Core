namespace Intervent.Web.DTO
{
    public class ListDrugResponse
    {
        public IList<Drug_ProductDto> DrugsCA { get; set; }
        public IList<Drug_Products_FDADto> DrugsUS { get; set; }
    }
}