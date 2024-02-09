namespace Intervent.Business
{
    public class PlacerBloodPressureRequest
    {
        public string participant_id { get; set; }

        public int pulse { get; set; }

        public int systolic { get; set; }

        public int diastolic { get; set; }

        public DateTime time_stamp { get; set; }
    }
}
