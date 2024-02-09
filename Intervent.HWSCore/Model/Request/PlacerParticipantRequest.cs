namespace Intervent.HWS
{
    public class PlacerParticipantRequest
    {
        public string participant_id { get; set; }
        public string provider { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }
        public string middle_name { get; set; }
        public string email { get; set; }
        public string phone_number { get; set; }
        public DateTime? date_of_birth { get; set; }
        public string zip { get; set; }
        public string gender { get; set; }
        public DateTime? discharge_date { get; set; }
        public string discharge_disposition { get; set; }
        public string presenting_diagnosis { get; set; }
    }
}
