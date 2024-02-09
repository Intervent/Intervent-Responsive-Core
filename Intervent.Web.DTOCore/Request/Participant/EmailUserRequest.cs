namespace Intervent.Web.DTO
{
    public class EmailUserRequest
    {
        public int userId { get; set; }

        public int page { get; set; }

        public int pageSize { get; set; }

        public int? totalRecords { get; set; }

    }
}
