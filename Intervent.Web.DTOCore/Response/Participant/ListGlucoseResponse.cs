namespace Intervent.Web.DTO
{
    public class ListGlucoseResponse
    {
        public IList<EXT_GlucoseDto> listGlucose { get; set; }

        public int totalRecords { get; set; }
    }
}
