namespace Intervent.Web.DTO
{
    public class AwardIncentivesRequest
    {
        public IncentiveTypes incentiveType { get; set; }

        public int userId { get; set; }

        public int portalId { get; set; }

        public bool isEligible { get; set; }

        public string pointsIncentiveMessage { get; set; }

        public string companyIncentiveMessage { get; set; }

        public string reference { get; set; }

        public int? adminId { get; set; }

    }
}
