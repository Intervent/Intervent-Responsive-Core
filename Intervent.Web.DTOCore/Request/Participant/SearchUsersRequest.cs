namespace Intervent.Web.DTO
{
    public class SearchUsersRequest
    {
        public int userId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public int? organizationId { get; set; }

        public int? Id { get; set; }

        public string Email { get; set; }
        public string Phone { get; set; }

        public string RiskCode { get; set; }

        public int PageSize { get; set; }

        public int Page { get; set; }

        public int? TotalRecords { get; set; }

        public DateTime? HraStartDate { get; set; }

        public DateTime? HraEndDate { get; set; }

        public string timeZone { get; set; }
    }
}
