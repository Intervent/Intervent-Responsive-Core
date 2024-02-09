namespace Intervent.Web.DTO
{
    public class GetCRMNotesRequest
    {
        public int? ContactId { get; set; }

        public Int64? CallId { get; set; }

        public DateTime? CallTime { get; set; }
    }
}
