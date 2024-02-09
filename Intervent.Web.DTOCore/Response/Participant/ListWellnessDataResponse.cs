namespace Intervent.Web.DTO
{
    public class ListWellnessDataResponse
    {
        public IList<WellnessDataDto> WellnessData { get; set; }

        public IList<WellnessDataDto> TableWellnessData { get; set; }

        public int TotalRecords { get; set; }
    }
}