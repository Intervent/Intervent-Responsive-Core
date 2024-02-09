namespace Intervent.Web.DTO
{
    public class ListVitalsLogRequest
    {
        public int userId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }
    }

    public class ReadVitalsRequest
    {
        public int Id { get; set; }
    }
}
