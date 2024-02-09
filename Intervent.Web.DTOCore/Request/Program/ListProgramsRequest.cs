namespace Intervent.Web.DTO
{
    public class ListProgramsRequest
    {
        public bool? onlyActive { get; set; }
        public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? totalRecords { get; set; }
    }
}