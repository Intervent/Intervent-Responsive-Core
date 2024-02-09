namespace Intervent.Web.DTO
{
    public class AdvancedSearchRequest
    {
        public string SearchText { get; set; }

        public int? OrganizationId { get; set; }

        public int? CoachId { get; set; }

        public string MedicalCondition { get; set; }

        public byte? ProgramType { get; set; }

        public string RecentStats { get; set; }

        public int? ContactRequirement { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public int UserId { get; set; }

        public DateTime? HraStartDate;

        public DateTime? HraEndDate { get; set; }
    }
}