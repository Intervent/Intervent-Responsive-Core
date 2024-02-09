namespace Intervent.Web.DTO
{
    public class ListExerciseDairyRequest
    {
        public int ParticipantId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

        public DateTime? startDate { get; set; }

        public DateTime? endDate { get; set; }
    }
}