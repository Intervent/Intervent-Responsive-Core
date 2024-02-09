namespace Intervent.Web.DTO
{
    public class ListEduKitsResponse
    {
        public IList<KitsDto> EduKits { get; set; }
        public int totalRecords { get; set; }
    }
}