namespace Intervent.Web.DTO
{
    public class GetTaskListResponse
    {
        public IList<TasksDto> tasks { get; set; }

        public int totalRecords { get; set; }
    }
}
