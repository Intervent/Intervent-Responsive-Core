namespace Intervent.Web.DTO
{
    public class GetUserProgramHistoryResponse
    {
        public IList<UsersinProgramDto> usersinPrograms { get; set; }

        public bool hasActiveProgram { get; set; }
    }
}
