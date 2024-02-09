namespace Intervent.Web.DTO
{
    public class UpdateIntuityOptingOutRequest
    {
        public int UserId { get; set; }

        public bool Status { get; set; }

        public DateTime OptedOutDate { get; set; }

        public bool OptingOut { get; set; }

        public IntuityEligibilityLogDto IntuityEligibilityLog { get; set; }
    }
}
