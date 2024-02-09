namespace Intervent.Web.DTO
{
    public class UpdatePatternPairingDateRequest
    {
        public int UserId { get; set; }

        public bool Status { get; set; }

        public DateTime PairingDate { get; set; }

        public string Devices { get; set; }

        public int systemAdminId { get; set; }

        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }
    }
}
