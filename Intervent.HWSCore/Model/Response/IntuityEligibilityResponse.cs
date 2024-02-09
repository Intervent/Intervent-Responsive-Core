namespace Intervent.HWS
{
    public class IntuityEligibilityResponse : ProcessResponse
    {
        public string unique_id { get; set; }
        public string eligibility_status { get; set; }
        public string eligibility_message { get; set; }
    }
}
