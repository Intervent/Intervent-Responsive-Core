namespace Intervent.Web.DTO
{
    public class DrugHistoryResponse
    {
        public IList<UserDrugDto> drugHistoryResponse { get; set; }
        public int TotalRecords { get; set; }
    }
}
