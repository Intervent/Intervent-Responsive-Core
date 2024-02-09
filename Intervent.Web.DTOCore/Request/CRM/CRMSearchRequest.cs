namespace Intervent.Web.DTO
{
    public class CRMSearchRequest
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int? CRMId { get; set; }

        public string Email { get; set; }

        public string Phone { get; set; }

        public string RMAandQADNumber { get; set; }

        public string MasterControlNo { get; set; }

        public int? CSRId { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

        public bool ComplaintsSearch { get; set; }

        public bool UnhandledWebforms { get; set; }
    }
}
