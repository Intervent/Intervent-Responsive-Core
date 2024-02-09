namespace Intervent.Web.DTO
{
    public class AdvancedSearchUsersResponse
    {
        public IList<ListSearchUsers_ResultsDto> Result { get; set; }

        public int TotalRecords { get; set; }
    }
}
