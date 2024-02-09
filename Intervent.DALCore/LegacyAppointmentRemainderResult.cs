namespace Intervent.DAL
{
    public class LegacyAppointmentRemainderResult
    {
        public int AppRef { get; set; }

        public string? FirstName { get; set; }

        public DateTime AppDate { get; set; }

        public string? PhoneNumber { get; set; }

        public string? CurrLangCode { get; set; }

        public string? TimeZoneID { get; set; }

        public string? ContactNumber { get; set; }
    }
}