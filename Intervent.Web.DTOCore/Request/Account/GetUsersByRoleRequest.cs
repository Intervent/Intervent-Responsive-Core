namespace Intervent.Web.DTO
{
    public class GetUsersByRoleRequest
    {
        public bool allUserswithRole { get; set; }

        public int? organization { get; set; }

        public string firstName { get; set; }

        public string LastName { get; set; }

        public int? page { get; set; }
        public int? pageSize { get; set; }
        public int? totalRecords { get; set; }
        public int? userId { get; set; }
    }
}