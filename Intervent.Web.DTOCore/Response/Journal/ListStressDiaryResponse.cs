namespace Intervent.Web.DTO
{
    public class ListStressDiaryResponse
    {
        public IList<StressDiaryDto> StressDiary { get; set; }

        public IList<StressDiaryDto> StressDiaries { get; set; }

        public int totalRecords { get; set; }
    }
}
