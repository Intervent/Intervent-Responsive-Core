namespace Intervent.HWS
{
    public class IntuityEnrolmentURLResponse : ProcessResponse
    {
        public Patient patient_lead { get; set; }

        public class Patient
        {
            public object id { get; set; }
            public object patient_unique_id { get; set; }
            public object smart_url { get; set; }
        }
    }
}
