namespace Intervent.Web.DTO
{
    public class GetPrevYearStatusResponse
    {
        public HRADto hra { get; set; }

        public bool prevPortal { get; set; }

        public bool prevCompBiometrics { get; set; }

        public bool prevCompCoaching { get; set; }

        public bool prevCompFirstCoaching { get; set; }

        public ReadFollowupReportResponse followupResponse { get; set; }
    }
}
