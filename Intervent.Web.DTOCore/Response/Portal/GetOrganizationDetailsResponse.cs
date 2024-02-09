namespace Intervent.Web.DTO
{
    public class GetOrganizationDetailsResponse
    {
        public IList<OrganizationDto> Organizations { get; set; }

        public int totalRecords { get; set; }

        public int ActiveOrganizationsCount { get; set; }

        public int CoachingOrganizationsCount { get; set; }

        public int SelfHelpOrganizationsCount { get; set; }

        public int InActiveOrganizationsCount { get; set; }

    }
}