namespace Intervent.Web.DTO
{
    public class UnapprovedCarePlanRequest
    {
        public int userId { get; set; }
        public int? poralId { get; set; }
        public int? assessmentType { get; set; }
        public string userStatus { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int? TotalRecords { get; set; }

    }
}