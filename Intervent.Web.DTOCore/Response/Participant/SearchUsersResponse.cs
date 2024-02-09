namespace Intervent.Web.DTO
{
    public class SearchUsersResponse
    {
        public IList<ListUsers_ResultsDto> result { get; set; }
        public int totalRecords { get; set; }
    }
}
