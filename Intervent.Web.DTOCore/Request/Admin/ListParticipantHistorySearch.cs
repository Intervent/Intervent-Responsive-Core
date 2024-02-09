namespace Intervent.Web.DTO
{
    public class ListParticipantHistorySearchRequest
    {
        public int UserId { get; set; }

        public int? UserHistoryCategoryId { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int? TotalRecords { get; set; }
    }
}
