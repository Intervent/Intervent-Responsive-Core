namespace Intervent.Web.DTO
{
    public class LegacyAppointmentRemainderResultDto
    {
        public int AppRef { get; set; }

        public string FirstName { get; set; }

        public DateTime ApptDate { get; set; }

        public string PhoneNumber { get; set; }

        public string TimeZoneId { get; set; }

        public string CurrLangCode { get; set; }
    }
}
