namespace Intervent.Web.DTO
{
    public class CallReminderLogDto
    {
        public int UserId { get; set; }

        public int AppointmentId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public System.DateTime Date { get; set; }

        public string Timezone { get; set; }

        public string TimezoneId { get; set; }

        //English - 0
        //Spanish - 1
        //Portuguese - 2
        //Russian - 3
        //Korean - 7
        //Mandarin - 8
        //Cantonese – 9
        //Creole – 67
        public string Language { get; set; }

        public string ContactMode { get; set; }

        public string PhoneNumber { get; set; }

        public string ClientPhone { get; set; }
    }
}
