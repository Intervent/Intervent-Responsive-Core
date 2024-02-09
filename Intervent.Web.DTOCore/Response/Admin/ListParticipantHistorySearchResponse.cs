namespace Intervent.Web.DTO
{
    public class ListParticipantHistorySearchResponse
    {
        public IEnumerable<UserHistoryDto> UserChanges { get; set; }

        public int TotalRecords { get; set; }
    }
}
