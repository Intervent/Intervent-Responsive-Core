namespace Intervent.Web.DTO
{
    public class ListCoachTrackingRequest
    {


        public int? orgId { get; set; }

        public int? coach { get; set; }

        public int AdminId { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

        public bool forDownload { get; set; }
    }
}
