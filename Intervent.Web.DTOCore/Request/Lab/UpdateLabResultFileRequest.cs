namespace Intervent.Web.DTO
{
    public class UpdateLabResultFileRequest
    {
        public int? Id { get; set; }

        public string FileName { get; set; }

        public int UpdatedBy { get; set; }

        public byte? RejectionReason { get; set; }

        public DateTime? RejectedOn { get; set; }

        public int? RejectedBy { get; set; }

    }
}
