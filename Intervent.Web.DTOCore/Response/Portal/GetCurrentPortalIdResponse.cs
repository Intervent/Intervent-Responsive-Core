namespace Intervent.Web.DTO
{
    public class GetCurrentPortalIdResponse
    {
        public int? PortalId { get; set; }

        public bool ProcessLivongoCodes { get; set; }

        public bool ProcessInterventCodes { get; set; }

        public string ClaimsDirectoryPath { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime? CampaignStartDate { get; set; }

        public DateTime? CampaignEndDate { get; set; }

        public byte? EligtoIntuity { get; set; }
    }
}
