namespace Intervent.Web.DTO
{
    public class ListWebinarsRequest
    {
        public bool dateFilter { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

    }
}
