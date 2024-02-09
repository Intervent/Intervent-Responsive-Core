namespace Intervent.Web.DTO
{
    public class ListStressManagementLogResponse
    {
        public IList<StressManagementLogDto> stressManagementLogs { get; set; }

        public IList<StressManagementLogDto> stressManagementLog { get; set; }

        public int totalRecords { get; set; }
    }
}
