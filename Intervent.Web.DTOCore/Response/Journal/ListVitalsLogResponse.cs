namespace Intervent.Web.DTO
{
    public class ListVitalsLogResponse
    {
        public IList<VitalsLogDto> dailyVitals { get; set; }

        public int totalRecords { get; set; }
    }
    public class ReadVitalsResponse
    {
        public VitalsLogDto dailyVital { get; set; }
    }
}
