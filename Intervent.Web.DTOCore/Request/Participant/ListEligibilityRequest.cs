namespace Intervent.Web.DTO
{
    public class ListEligibilityRequest
    {
        //public int? orgId { get; set;}

        public int userId { get; set; }

        public string uniqueId { get; set; }

        public string firstName { get; set; }

        public string lastName { get; set; }

        public string Email { get; set; }

        public string Telephone { get; set; }

        public string Language { get; set; }

        public int? portalId { get; set; }

        public DateTime? EligibilityStartDate { get; set; }

        public DateTime? EligibilityEndDate { get; set; }

        public DateTime? ClaimStartDate { get; set; }

        public DateTime? ClaimEndDate { get; set; }

        public byte? enrollmentStatus { get; set; }

        public string ClaimDiagnosisCode { get; set; }

        public bool? canrisk { get; set; }

        public bool? CoachingEnabled { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int? TotalRecords { get; set; }

        public bool eBenChild { get; set; }
    }
}
