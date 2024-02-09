namespace Intervent.Web.DTO
{
    public class LogLabErrorsRequest
    {
        public List<string> Errors { get; set; }

        public int? portalId { get; set; }

        public int? userId { get; set; }

        public byte[] data { get; set; }

    }
}
