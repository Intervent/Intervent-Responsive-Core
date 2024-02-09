namespace Intervent.Web.DTO
{
    public class EditApptCommentRequest
    {
        public int id { get; set; }

        public string comments { get; set; }

        public int length { get; set; }

        public string timezone { get; set; }

        public bool videoRequired { get; set; }

        public int updatedBy { get; set; }

        public int? stateId { get; set; }
    }
}
