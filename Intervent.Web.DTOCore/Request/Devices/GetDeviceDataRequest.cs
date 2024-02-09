namespace Intervent.Web.DTO
{
    public class GetDeviceDataRequest
    {
        public int UserId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int? page { get; set; }

        public string TimeZone { get; set; }

        public int? pageSize { get; set; }

        public int? totalRecords { get; set; }
    }
}
